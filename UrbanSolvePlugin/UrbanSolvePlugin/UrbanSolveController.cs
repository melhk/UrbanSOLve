using System;
using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Text;
using Rhino.Geometry.Collections;
using System.Security.AccessControl;
using System.Linq;
using System.ComponentModel;
using MetaheuristicsLibrary.SolversMO;

namespace UrbanSolvePlugin
{
    public enum Typology
    {
        [Description("Simple volume")] square,
        [Description("Courtyard")] emptySquare,
        [Description("L-shaped")] lShaped
    };

    public enum Program
    {
        [Description("Office")]
        office,
        [Description("Residential")]
        residential
    };

    static class EnumMethods
    {
        public static string GetDescriptionFromEnumValue(Enum value)
        {
            DescriptionAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }

    public class UrbanSolveController
    {
        public event EventHandler<UpdateEventArg> Update;

        private Thread workerThread;
        // Volatile is used as hint to the compiler that this data
        // member will be accessed by multiple threads.
        private volatile bool _shouldStop;

        // The active Rhino model
        public RhinoDoc doc { get; private set; }

        // Plugin views
        public MainWindow mainWindow { get; set; }
        private NativeWindow nativeWindow;

        // Model
        private VariantDescription variantDescription;

        // For the display in Rhino views
        private DescriptionConduit descriptionConduit;
        private SolutionConduit solutionConduit;

        public UrbanSolveController(RhinoDoc doc)
        {
            this.doc = doc;
            variantDescription = null;
   
            descriptionConduit = new DescriptionConduit();
            solutionConduit = new SolutionConduit();

            workerThread = null;

            // Opens the main interface
            mainWindow = new MainWindow(this);
            IntPtr hWnd = RhinoApp.MainWindowHandle();  // assume the handle is returned correctly
            nativeWindow = new NativeWindow();
            nativeWindow.AssignHandle(hWnd);
            mainWindow.Show(nativeWindow);

            descriptionConduit.descriptions = mainWindow.variantDescription.buildingDescriptions;
            solutionConduit.variant = null;

            mainWindow.Show();  
        }

        internal void OnHandleDestroyed(object sender, EventArgs e)
        {
            // Window was destroyed, release hook.
            nativeWindow.ReleaseHandle();
        }

        protected virtual void OnUpdate(UpdateEventArg e)
        {
            Update?.Invoke(this, e);
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        public void drawDescription(bool status)
        {
            descriptionConduit.Enabled = status;
            doc.Views.Redraw();
        }

        public void setSolution(int generation, int solutionNumber)
        {
            try
            {
                solutionConduit.variant = variantDescription.getVariant(generation, solutionNumber);
                doc.Views.Redraw();
            } 
            catch (ArgumentOutOfRangeException e)
            {
                System.Diagnostics.Debug.WriteLine("out of range: " + e.ToString());
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("null reference: " + e.ToString());
            }
        }

        public void drawSolution(bool status)
        {
            solutionConduit.Enabled = status;
            doc.Views.Redraw();
        }

        public void exportSolution(int generation, int solutionNumber)
        {
            using (var fbd = new FolderBrowserDialog())
            {

                fbd.Description = "Select the directory that you want to use.";
                // Default to the My Documents folder.
                fbd.RootFolder = Environment.SpecialFolder.Desktop;

                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        DateTime date = DateTime.Now;
                        string txtPath = Path.Combine(fbd.SelectedPath, "urbanSOLve_" + date.Year + date.Month + date.Day + ".txt");
                        string csvPath = Path.Combine(fbd.SelectedPath, "urbanSOlve_" + date.Year + date.Month + date.Day + ".csv");

                        if (File.Exists(txtPath))
                        {
                            File.Delete(txtPath);
                        }
                        using (FileStream fs = File.Create(txtPath))
                        {
                            Byte[] toBytes = new UTF8Encoding(true).GetBytes(variantDescription.ToString());
                            fs.Write(toBytes, 0, toBytes.Length);
                        }

                        if (File.Exists(csvPath))
                        {
                            File.Delete(csvPath);
                        }
                        using (FileStream fs = File.Create(csvPath))
                        {
                            Byte[] toBytes = new UTF8Encoding(true).GetBytes(variantDescription.toCsv());
                            fs.Write(toBytes, 0, toBytes.Length);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid path!", "Error");
                    }
                }      
            }
        }

        public void bakeInitialSolution()
        {
            int layerIndex;
            string layerName = CST.BAKE_LAYER_NAME + "_initial_variant";
            if ((layerIndex = doc.Layers.Find(layerName, true)) == -1)
            {
                doc.Layers.Add(layerName, CST.BAKE_LAYER_COLOR);
                layerIndex = doc.Layers.Find(layerName, true);
            }

            ObjectAttributes attr = new ObjectAttributes();
            attr.LayerIndex = layerIndex;
            attr.ColorSource = ObjectColorSource.ColorFromLayer;

            try
            {
                Variant var = variantDescription.initialVariant;
                foreach (Building b in var.buildings)
                {
                    doc.Objects.AddBrep(b.geometry.geometry, attr);
                    doc.Objects.AddPoint(b.refPoint, attr);
                }
                doc.Views.Redraw();
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("Null reference: " + e.ToString());
            }
            catch (IndexOutOfRangeException e)
            {
                System.Diagnostics.Debug.WriteLine("Out of range: " + e.ToString());
            }
        }

        public void bakeSolution(int generation, int solutionNumber)
        {
            int layerIndex;
            string layerName = CST.BAKE_LAYER_NAME + "_gen_" + generation + "_var_" + (solutionNumber + 1) ;
            if ((layerIndex = doc.Layers.Find(layerName,
                true)) == -1)
            {
                doc.Layers.Add(layerName, CST.BAKE_LAYER_COLOR);
                layerIndex = doc.Layers.Find(layerName, true);
            }

            ObjectAttributes attr = new ObjectAttributes();
            attr.LayerIndex = layerIndex;
            attr.ColorSource = ObjectColorSource.ColorFromLayer;

            try
            {
                Variant var = variantDescription.getSolution(generation, solutionNumber);
                foreach (Building b in var.buildings)
                {
                    doc.Objects.AddBrep(b.geometry.geometry, attr);
                    doc.Objects.AddPoint(b.refPoint, attr);
                }
                doc.Views.Redraw();
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("Null reference: " + e.ToString());
            }
            catch (IndexOutOfRangeException e)
            {
                System.Diagnostics.Debug.WriteLine("Out of range: " + e.ToString());
            }
        }

        /// <summary>
        /// Fonction où se déroule l'optimisation
        /// </summary>
        public void generateSolutions()
        {
            lock(variantDescription)
            { 
                // Création d'un dossier temporaire afin d'y stocker les fichiers nécessaires à l'optimisation en cours            
                DirectoryInfo dirInfo = Directory.CreateDirectory(Path.Combine(CST.FOLDER_PATH, CST.TMP_DIRECTORY_NAME));
                System.Security.Principal.WindowsIdentity winID = System.Security.Principal.WindowsIdentity.GetCurrent();
                DirectorySecurity fs = dirInfo.GetAccessControl();
                //Rule for this Directory
                fs.AddAccessRule(new FileSystemAccessRule(winID.Name, FileSystemRights.FullControl, AccessControlType.Allow));
                //Rule for Sub Directories and Files
                fs.AddAccessRule(new FileSystemAccessRule(winID.Name, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
                dirInfo.SetAccessControl(fs);

                // Effacer les résultats de la dernière simulation;
                variantDescription.clearVariants();
                _shouldStop = false;

                /* SOLUTION PARAMETERS */
                // Nombre de variable que possède le design voulu par l'utilisateur
                int nbOfVariables = variantDescription.getNumberOfVariables();
                // Dimensions minimum - maximum
                double[] lowerBounds = variantDescription.getLowerBounds();
                double[] upperBounds = variantDescription.getUpperBounds();
                // Certaines variables doivent être traitées en tant qu'entiers
                bool[] isInteger = Enumerable.Repeat(true, nbOfVariables).ToArray();

                // -----------------------------------------------------------------------------------------
                int generationNumber = 0;
                int variantNumber = 0;
                List<Variant> variants = new List<Variant>();

                // define your custom function here. this is where you should call Radiance and the Metamodel
                Func<double[], double[]> urbanSolve = geometry =>
                {
                    RhinoApp.WriteLine("generation {0}, variant {1}", generationNumber, variantNumber + 1);
                    if (_shouldStop)
                    {
                        ++variantNumber;
                        return new double[] { 0.0, 0.0, double.MaxValue };
                    }
                    else
                    {
                        // Check les dimensions, retourne null si la solution n'est pas réalisable avec ces valeurs
                        // Une solution est calculée à partir des dimensions générée par l'optimisation
                        Variant currentVariant = variantDescription.getSolution(generationNumber, variantNumber, geometry);

                        if (currentVariant.buildings.Count == 0)
                        {
                            variants.Add(new Variant(generationNumber, variantNumber, variantDescription, variantDescription.seed, 
                                variantDescription.minDistanceBetweenBuildings, variantDescription.totalGroundArea));
                            ++variantNumber;
                            // Problème de géométrie, les valeurs proposées ne permettent pas de créer une géométrie correcte
                            return new double[] { 0.0, 0.0, double.MaxValue };
                        }
                        else
                        {
                            // L'irradiation n'est calculée que si les contraintes sont toutes respectées
                            if (checkConstraints(currentVariant))
                            {
                                // Calcule l'irradiation de la solution courante et les 3 objectifs qui en découlent
                                calculateIrradiation(currentVariant);

                                // Metamodel
                                double[] result = new double[CST.NB_OF_OBJECTIVES];
                                result[0] = -1 * getActiveSolarEnergy(currentVariant);  // Maximiser
                                result[1] = -1 * getDaylightAutonomy(currentVariant);   // Maximiser
                                result[2] = getEnergyNeed(currentVariant);              // Minimiser

                                
                                variants.Add(new Variant(generationNumber, variantNumber, variantDescription, variantDescription.seed,
                                    variantDescription.minDistanceBetweenBuildings, -1 * result[0], -1 * result[1], result[2],
                                    currentVariant,
                                    currentVariant.getTotalFloorArea(), currentVariant.getTotalEnvelopArea(), variantDescription.totalGroundArea));
                                
                                ++variantNumber;
                                return result;
                            }
                            else
                            {
                                // Les contraintes ne sont pas respectées, les plus mauvaises valeurs sont associées à ces objectifs
                                variants.Add(new Variant(generationNumber, variantNumber, variantDescription, variantDescription.seed,
                                    variantDescription.minDistanceBetweenBuildings, variantDescription.totalGroundArea));
                                ++variantNumber;
                                return new double[] { 0.0, 0.0, double.MaxValue };
                            }
                        }
                    }                    
                };

                // Calcul des valeurs de la solution initiale
                RhinoApp.WriteLine("START");

                // Génération du fichier de description du ciel, commun à toutes les solutions
                generateSkyFile(CST.SKY_FILE_NAME);

                Variant initialVariant = variantDescription.getInitialSolution();
                if (null != initialVariant)
                {
                    calculateIrradiation(initialVariant);

                    variantDescription.initialVariant = new Variant(0, -1, variantDescription, variantDescription.seed,
                        variantDescription.minDistanceBetweenBuildings,
                        getActiveSolarEnergy(initialVariant), getDaylightAutonomy(initialVariant),
                        getEnergyNeed(initialVariant), initialVariant,
                        initialVariant.getTotalFloorArea(), initialVariant.getTotalEnvelopArea(),
                        variantDescription.totalGroundArea);
                }

                List<double[]> initialSolutions = new List<double[]>();
                initialSolutions.Add(initialVariant.variantDescription.getInitialSolutionValues());

                /* OPTIMIZATION - Calcul de toutes les autres solutions */
                SPEA2 optimizer_mo = new SPEA2(CST.NB_OF_OBJECTIVES, nbOfVariables, lowerBounds, upperBounds,
                    urbanSolve, variantDescription.seed, initialSolutions, isInteger);
                optimizer_mo.ga_genmax = variantDescription.numberOfGenerations;
                optimizer_mo.ga_nPop = variantDescription.populationSize;

                // L'initialisation "aléatoire" est gardées dans les solutions afin de conserver la variante
                // initialement donnée par l'utilisateur
                variants = new List<Variant>();
                optimizer_mo.initialize();

                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! A VOIR
                // optimizer_mo.ga_PCrossover = 0.0;              //no crossover. This is good, when variables tend to be highly mutually dependant.
                // optimizer_mo.ga_PMutation = 1.0;               //only mutation, i.e. random sampling
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                variantDescription.saveGeneration(variants);
                ++generationNumber;

                //start solver and read data in every iteration                
                while (optimizer_mo.terminateNow == false && !_shouldStop)
                {   
                    variantNumber = 0;
                    variants = new List<Variant>();
                    //this means, that only one iteration is run
                    optimizer_mo.Solve(1);
                    variantDescription.saveGeneration(variants);
                    ++generationNumber;                        
                }
               
                if (_shouldStop)
                {
                    variantDescription.clearVariants();
                }

                UpdateEventArg args = new UpdateEventArg();
                args.variants = variantDescription.simulationResults;
                args.initialVariant = variantDescription.initialVariant;
                OnUpdate(args);

                // Effacer tous les fichiers temporaires à la fin de l'optimisation
                dirInfo.Delete(true);
                RhinoApp.WriteLine("END");
            }
        }

        public void clearAllVariants()
        {
            UpdateEventArg args = new UpdateEventArg();

            if (variantDescription != null)
            {
                variantDescription.clearVariants();
                variantDescription.clearBuildingDescriptions();
                args.variants = variantDescription.simulationResults;
                args.initialVariant = variantDescription.initialVariant;
            }
            else
            {
                args.variants = null;
                args.initialVariant = null;
            }
            solutionConduit.variant = null;
            doc.Views.Redraw();       
            OnUpdate(args);
        }

        public void start(VariantDescription solution)
        {
            variantDescription = solution;
            workerThread = new Thread(new ThreadStart(generateSolutions));
            workerThread.Start();
        }

        public bool exportToObj(List<Brep> context, List<Brep> ground, List<Brep> breps, string fileName)
        {
            using (FileStream fs = File.Create(Path.Combine(Path.Combine(CST.FOLDER_PATH, CST.TMP_DIRECTORY_NAME), fileName + ".obj")))
            {
                byte[] toBytes = Encoding.ASCII.GetBytes("# urbanSOLve" + Environment.NewLine + Environment.NewLine);
                fs.Write(toBytes, 0, toBytes.Length);

                // Contexte --------------------------
                int nb = 0;
                foreach (Brep b in context)
                {
                    byte[] obj = Encoding.ASCII.GetBytes("g context" + Environment.NewLine);
                    fs.Write(obj, 0, obj.Length);

                    Mesh[] meshes = Mesh.CreateFromBrep(b, MeshingParameters.Default);

                    foreach (Mesh m in meshes)
                    {
                        MeshVertexList vertices = m.Vertices;
                        for (int i = 0; i < vertices.Count; ++i)
                        {
                            byte[] vertexToBytes = Encoding.ASCII.GetBytes("v "
                            + vertices[i].X + " "
                            + vertices[i].Y + " "
                            + vertices[i].Z + Environment.NewLine);
                            fs.Write(vertexToBytes, 0, vertexToBytes.Length);
                        }

                        MeshVertexNormalList normals = m.Normals;
                        for (int i = 0; i < vertices.Count; ++i)
                        {
                            byte[] vertexToBytes = Encoding.ASCII.GetBytes("vn "
                            + normals[i].X + " "
                            + normals[i].Y + " "
                            + normals[i].Z + Environment.NewLine);
                            fs.Write(vertexToBytes, 0, vertexToBytes.Length);
                        }
                    }

                    foreach (Mesh m in meshes)
                    {
                        MeshFaceList faces = m.Faces;
                        for (int i = 0; i < faces.Count; ++i)
                        {
                            byte[] faceToBytes = Encoding.ASCII.GetBytes("f "
                            + (faces[i].A + 1 + nb) + "//" + (faces[i].A + 1 + nb) + " "
                            + (faces[i].B + 1 + nb) + "//" + (faces[i].B + 1 + nb) + " "
                            + (faces[i].C + 1 + nb) + "//" + (faces[i].C + 1 + nb) + " "
                            + (faces[i].D + 1 + nb) + "//" + (faces[i].D + 1 + nb) + Environment.NewLine);
                            fs.Write(faceToBytes, 0, faceToBytes.Length);
                        }
                        nb += m.Vertices.Count;
                    }
                }

                // Parcelle --------------------------
                Mesh groundMesh = Mesh.CreateFromPlane(variantDescription.p, 
                    variantDescription.r.X, variantDescription.r.Y, 10, 10);

                byte[] groundMeshObj = Encoding.ASCII.GetBytes("g ground" + Environment.NewLine);
                fs.Write(groundMeshObj, 0, groundMeshObj.Length);

                MeshVertexList groundMeshVertices = groundMesh.Vertices;
                for (int i = 0; i < groundMeshVertices.Count; ++i)
                {
                    byte[] vertexToBytes = Encoding.ASCII.GetBytes("v "
                    + groundMeshVertices[i].X + " "
                    + groundMeshVertices[i].Y + " "
                    + groundMeshVertices[i].Z + Environment.NewLine);
                    fs.Write(vertexToBytes, 0, vertexToBytes.Length);
                }

                MeshVertexNormalList groundMeshNormals = groundMesh.Normals;
                for (int i = 0; i < groundMeshVertices.Count; ++i)
                {
                    byte[] vertexToBytes = Encoding.ASCII.GetBytes("vn "
                    + groundMeshNormals[i].X + " "
                    + groundMeshNormals[i].Y + " "
                    + groundMeshNormals[i].Z + Environment.NewLine);
                    fs.Write(vertexToBytes, 0, vertexToBytes.Length);
                }

                MeshFaceList groundMeshFaces = groundMesh.Faces;
                for (int i = 0; i < groundMeshFaces.Count; ++i)
                {
                    byte[] faceToBytes = Encoding.ASCII.GetBytes("f "
                    + (groundMeshFaces[i].A + 1 + nb) + "//" + (groundMeshFaces[i].A + 1 + nb) + " "
                    + (groundMeshFaces[i].B + 1 + nb) + "//" + (groundMeshFaces[i].B + 1 + nb) + " "
                    + (groundMeshFaces[i].C + 1 + nb) + "//" + (groundMeshFaces[i].C + 1 + nb) + " "
                    + (groundMeshFaces[i].D + 1 + nb) + "//" + (groundMeshFaces[i].D + 1 + nb) + Environment.NewLine);
                    fs.Write(faceToBytes, 0, faceToBytes.Length);
                }
                nb += groundMesh.Vertices.Count;

                // Bâtiments -------------------------
                foreach (Brep b in breps)
                {
                    byte[] obj = Encoding.ASCII.GetBytes("g object" + Environment.NewLine);
                    fs.Write(obj, 0, obj.Length);

                    Mesh[] meshes = Mesh.CreateFromBrep(b, MeshingParameters.Default);

                    foreach (Mesh m in meshes)
                    {
                        MeshVertexList vertices = m.Vertices;
                        for (int i = 0; i < vertices.Count; ++i)
                        {
                            byte[] vertexToBytes = Encoding.ASCII.GetBytes("v "
                            + vertices[i].X + " "
                            + vertices[i].Y + " "
                            + vertices[i].Z + Environment.NewLine);
                            fs.Write(vertexToBytes, 0, vertexToBytes.Length);
                        }

                        MeshVertexNormalList normals = m.Normals;
                        for (int i = 0; i < vertices.Count; ++i)
                        {
                            byte[] vertexToBytes = Encoding.ASCII.GetBytes("vn "
                            + normals[i].X + " "
                            + normals[i].Y + " "
                            + normals[i].Z + Environment.NewLine);
                            fs.Write(vertexToBytes, 0, vertexToBytes.Length);
                        }
                    }

                    foreach (Mesh m in meshes)
                    {
                        MeshFaceList faces = m.Faces;
                        for (int i = 0; i < faces.Count; ++i)
                        {
                            byte[] faceToBytes = Encoding.ASCII.GetBytes("f "
                            + (faces[i].A + 1 + nb) + "//" + (faces[i].A + 1 + nb) + " "
                            + (faces[i].B + 1 + nb) + "//" + (faces[i].B + 1 + nb) + " "
                            + (faces[i].C + 1 + nb) + "//" + (faces[i].C + 1 + nb) + " "
                            + (faces[i].D + 1 + nb) + "//" + (faces[i].D + 1 + nb) + Environment.NewLine);
                            fs.Write(faceToBytes, 0, faceToBytes.Length);
                        }
                        nb += m.Vertices.Count;
                    }
                }
            }
            return true;
        }
    
        public bool exportToPts(List<IrradiationPoint> points, string fileName)
        {
            using (FileStream fs = File.Create(Path.Combine(Path.Combine(CST.FOLDER_PATH, CST.TMP_DIRECTORY_NAME), fileName + ".pts")))
            {
                foreach (IrradiationPoint point in points)
                {
                    byte[] pointToBytes = Encoding.ASCII.GetBytes(
                        point.node.X.ToString() + " "
                        + point.node.Y.ToString() + " "
                        + point.node.Z.ToString() + " "
                        + point.vector.X.ToString() + " "
                        + point.vector.Y.ToString() + " "
                        + point.vector.Z.ToString() + Environment.NewLine);
                    fs.Write(pointToBytes, 0, pointToBytes.Length);   
                }
            }
            return true;
        }

        private void generateSkyFile(string skyFileName)
        {       
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo("sky.bat", skyFileName + ".cal");
            int exitCode;

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(processInfo);
            process.WaitForExit();
            exitCode = process.ExitCode;
            System.Diagnostics.Debug.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();
        }

        private void generateGcsky(string fileName, string skyFileName)
        {
            using (FileStream fs = File.Create(Path.Combine(Path.Combine(CST.FOLDER_PATH, CST.TMP_DIRECTORY_NAME), fileName + "_gcsky.rad")))
            {
                byte[] text = Encoding.ASCII.GetBytes("# Sky Definition" + Environment.NewLine);
                fs.Write(text, 0, text.Length);

                text = Encoding.ASCII.GetBytes(
                    "void brightfunc skyfunc" + Environment.NewLine
                    + "2 skybright " + skyFileName + ".cal" + Environment.NewLine
                    + "0" + Environment.NewLine
                    + "0" + Environment.NewLine
                    + "skyfunc glow sky_glow" + Environment.NewLine
                    + "0" + Environment.NewLine
                    + "0" + Environment.NewLine
                    + "4 1 1 1 0" + Environment.NewLine
                    + "sky_glow source sky" + Environment.NewLine
                    + "0" + Environment.NewLine
                    + "0" + Environment.NewLine
                    + "4 0 0 1 180" + Environment.NewLine
                    );
                fs.Write(text, 0, text.Length);
            }
        }

        private int generateRadFile(string fileName)
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo("irradiation.bat", 
                fileName + ".obj " + fileName + ".rad " + fileName + ".oct " + fileName + ".pts " + fileName + "_radmap.dat " + fileName + "_gcsky.rad");
            System.Diagnostics.Process process;
            int exitCode;
            
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = System.Diagnostics.Process.Start(processInfo);

            process.WaitForExit();
            exitCode = process.ExitCode;
            process.Close();
            return exitCode;
        }

        private void calculateIrradiation(Variant variant)
        {
            string fileName = "tmp_" + variant.Id;

            variant.createPoints();
            List<IrradiationPoint> irradiationPoints = new List<IrradiationPoint>();
            foreach (Building building in variant.buildings)
            {
                irradiationPoints.AddRange(building.irradiationPoints);
            }

            // Creates .obj file / Creates .pts file
            if (exportToObj(variant.variantDescription.context,
                            variant.variantDescription.ground,
                            variant.getGeometry(), fileName) && exportToPts(irradiationPoints, fileName))
            {
                generateGcsky(fileName, CST.SKY_FILE_NAME);
                generateRadFile(fileName);

                // Read the irradiation file and retrieve the values
                try
                {
                    string path = Path.Combine(CST.FOLDER_PATH, Path.Combine(CST.TMP_DIRECTORY_NAME, fileName + "_radmap.dat"));
                    using (StreamReader file = new StreamReader(path))
                    {
                        string line;
                        int lineNumber = 0;
                        while ((line = file.ReadLine()) != null)
                        {
                            string[] values = line.Split(new Char[] { }, StringSplitOptions.RemoveEmptyEntries);
                            try
                            {
                                irradiationPoints.ElementAt(lineNumber).irradiationValue = Convert.ToDouble(values[6]);
                                lineNumber++;
                            }
                            catch (FormatException e)
                            {
                                System.Diagnostics.Debug.WriteLine(e);
                            }
                            catch (OverflowException e)
                            {
                                System.Diagnostics.Debug.WriteLine(e);
                            }
                        }
                    }
                }
                catch (IOException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);                     
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Obj or Pts error");
            }
        }

        private bool checkConstraints(Variant solution)
        {
            if (solution.checkSiteCoverage() & solution.checkFloorAreaRatio())
            {
                if (solution.checkOnParcel() & solution.checkMinimumDistance())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        private double getDaylightAutonomy(Variant variant)
        {
            double daylightAutonomy = 0.0;
            double totalFloorArea = 0.0;
            double flooraraRatio = variant.getFloorAreaRatio();
            double siteCoverage = variant.getSiteCoverage();

            foreach (Building b in variant.buildings)
            {
                double buildingFloorArea = b.getTotalFloorArea();
                double buildingPerformance = b.getDaylightAutonomy(flooraraRatio, siteCoverage);

                daylightAutonomy += buildingPerformance * buildingFloorArea;
                totalFloorArea += buildingFloorArea;
            }
            return daylightAutonomy / totalFloorArea;
        }

        // Energy production
        private double getActiveSolarEnergy(Variant variant)
        {
            double totalBuildingProduction = 0.0;
            double totalBuildingFloorArea = 0.0;

            foreach (Building b in variant.buildings)
            {
                totalBuildingProduction += b.getActiveSolarEnergy();
                totalBuildingFloorArea += b.getTotalFloorArea();
            }
            return totalBuildingProduction / totalBuildingFloorArea; ;
        }

        /// <summary>
        /// Passive solar potential
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        private double getEnergyNeed(Variant variant)
        {
            double passiveSolarPotential = 0.0;
            double totalFloorArea = 0.0;

            foreach (Building b in variant.buildings)
            {
                double buildingFloorArea = b.getTotalFloorArea();
                double buildingPerformance = b.getEnergyNeed(variant.getFloorAreaRatio(), variant.getSiteCoverage());
                passiveSolarPotential += buildingPerformance * buildingFloorArea;
                totalFloorArea += buildingFloorArea;
            }
            return passiveSolarPotential / totalFloorArea;
        }
    }

    public class UpdateEventArg : EventArgs
    {
        public List<List<Variant>> variants { get; set; }
        public Variant initialVariant { get; set; }
    }
}
 