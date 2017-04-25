using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Rhino.DocObjects;
using System.Linq;
using Rhino.Geometry;
using Rhino.Display;
using System.Drawing.Imaging;

namespace UrbanSolvePlugin
{
    public partial class MainWindow : Form
    {
        public VariantDescription variantDescription { get; private set; }

        delegate void SetSolutionsCallback(object sender, UpdateEventArg e);

        public event EventHandler<RefreshChartsEventArg> RefreshCharts;
        public event EventHandler<RefreshBuildingFormEventArg> RefreshBuildingFrom;
        public event EventHandler<BuildingAddedOrRemovedEventArg> BuildingAddedOrRemoved;

        private UrbanSolveController controller;
        // Variante initiale, entrée par l'utilisateur
        public Variant initialVariant { get; set; }
        // Liste des variantes générées par l'optimisation
        public List<List<Variant>> variants { get; private set; }

        public Pair<int, int> selectedSolution { get; }

        private List<buildingForm> buildingForms;
        private List<ResultWindow> resultWindows;
        private bool seedStatus;

        private string watermarkContext = "Enter context layer name...";
        private string watermarkPlot = "Enter plot layer name...";
        private bool contextLayerSet;
        private bool plotLayerSet;
        private System.Drawing.Color watermarkColor = System.Drawing.Color.Gray;

        public MainWindow(UrbanSolveController controller)
        {
            InitializeComponent();

            this.controller = controller;
            variantDescription = new VariantDescription();

            variants = new List<List<Variant>>();
            buildingForms = new List<buildingForm>();
            resultWindows = new List<ResultWindow>();

            this.controller.Update += new EventHandler<UpdateEventArg>(update);
            FormClosing += Form_FormClosing;
            BuildingAddedOrRemoved += new EventHandler<BuildingAddedOrRemovedEventArg>(buildingAddedOrRemoved);
            txtBox_plot.GotFocus += new EventHandler(enterPlotTxtBox);
            txtBox_plot.Leave += new EventHandler(leavePlotTxtBox);
            txtBox_context.GotFocus += new EventHandler(enterContextTxtBox);
            txtBox_context.Leave += new EventHandler(leaveContextTxtBox);

            setInitialValues();

            seedStatus = false;
            checkBox_seed.Checked = false;
            numUpDown_seed.Enabled = false;

            checkBox_showSolutions.Checked = true;
            checkBox_showDescriptions.Checked = true;
            numUpDown_minDistanceBtwBuildings.Enabled = false;

            txtBox_plot.ForeColor = watermarkColor;
            txtBox_context.ForeColor = watermarkColor;
            txtBox_context.Text = watermarkContext;
            txtBox_plot.Text = watermarkPlot;
            plotLayerSet = false;
            contextLayerSet = false;

            checkBox_showDescriptions.ForeColor = CST.DESCRIPTION_COLOR_BLACK;
            checkBox_showSolutions.ForeColor = CST.SOLUTION_COLOR_BLACK;

            selectedSolution = new Pair<int, int>(0, 0);

            enableInterface(true);
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "Are you sure that you would like to close the application?";
            string caption = "Close Application";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            e.Cancel = (result == DialogResult.No);           
            if (result == DialogResult.Yes)
            {
                controller.RequestStop();
                controller.drawDescription(false);
                controller.drawSolution(false);
            }
        }

        /// <summary>
        /// http://developer.rhino3d.com/api/RhinoCommonWin/html/P_Rhino_Display_RhinoView_ActiveViewport.htm
        /// </summary>
        private void captureView()
        {
            Rhino.Input.Custom.GetPoint gp = new Rhino.Input.Custom.GetPoint();
            Rhino.Display.RhinoView view = gp.View();
            if (view == null)
            {
                view = controller.doc.Views.ActiveView;
                if (view == null)
                {
                    // return Rhino.Commands.Result.Failure;
                    return;
                }           
            }

            System.Drawing.Size size = new System.Drawing.Size(600, 600);
            System.Drawing.Bitmap bitmap = view.CaptureToBitmap(size, true, true, true);

            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //int width = Convert.ToInt32(drawImage.Width);
                //int height = Convert.ToInt32(drawImage.Height);
                //Bitmap bmp = new Bitmap(width, height);
               // drawImage.DrawToBitmap(bmp, new Rectangle(0, 0, width, height);
                bitmap.Save(dialog.FileName, ImageFormat.Jpeg);
            }


            // bitmap.Save(file);
        }

        private void setInitialValues()
        {
            numUpDown_minSiteCoverage.Minimum = (decimal)CST.MIN_SITE_COVERAGE;
            numUpDown_minSiteCoverage.Maximum = (decimal)CST.MAX_SITE_COVERAGE;
            numUpDown_maxSiteCoverage.Minimum = (decimal)CST.MIN_SITE_COVERAGE;
            numUpDown_maxSiteCoverage.Maximum = (decimal)CST.MAX_SITE_COVERAGE;

            numUpDown_minFloorAreaRatio.Minimum = (decimal)CST.MIN_FLOOR_AREA_RATIO;
            numUpDown_maxFloorAreaRatio.Maximum = (decimal)CST.MAX_FLOOR_AREA_RATIO;
            numUpDown_minFloorAreaRatio.Minimum = (decimal)CST.MIN_FLOOR_AREA_RATIO;
            numUpDown_maxFloorAreaRatio.Maximum = (decimal)CST.MAX_FLOOR_AREA_RATIO;

            numUpDown_numberOfGenerations.Value = variantDescription.numberOfGenerations;
            numUpDown_numberOfGenerations.Minimum = CST.MIN_NB_OF_GENERATIONS;
            numUpDown_numberOfGenerations.Maximum = CST.MAX_NB_OF_GENERATIONS;

            numUpDown_nbOfVariants.Value = variantDescription.populationSize;
            numUpDown_nbOfVariants.Minimum = CST.MIN_POPULATION_SIZE;
            numUpDown_nbOfVariants.Maximum = CST.MAX_POPULATION_SIZE;

            numUpDown_minDistanceBtwBuildings.Value = (decimal)variantDescription.minDistanceBetweenBuildings;

            numUpDown_maxFloorAreaRatio.Value = (decimal)variantDescription.maxFloorAreaRatio;
            numUpDown_minFloorAreaRatio.Value = (decimal)variantDescription.minFloorAreaRatio;

            numUpDown_maxSiteCoverage.Value = (decimal)variantDescription.maxSiteCoverage;
            numUpDown_minSiteCoverage.Value = (decimal)variantDescription.minSiteCoverage;

            numUpDown_seed.Value = variantDescription.seed;
        }

        protected virtual void OnBuildingAddedOrRemoved(BuildingAddedOrRemovedEventArg e)
        {
            EventHandler<BuildingAddedOrRemovedEventArg> handler = BuildingAddedOrRemoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool checkSiteCoverage(ref string msg)
        {
            double minimumPossibleSiteCoverage = variantDescription.getMinimumPossibleSiteCoverage();
            double maximumSiteCoverage = variantDescription.getMaximumPossibleSiteCoverage();

            if ((double)numUpDown_maxSiteCoverage.Value < minimumPossibleSiteCoverage 
                || (double)numUpDown_minSiteCoverage.Value > maximumSiteCoverage)
            {
                msg += "Minimum possible site coverage for the current input: " 
                    + minimumPossibleSiteCoverage + Environment.NewLine;
                msg += "Maximum possible site coverage for the current input: " 
                    + maximumSiteCoverage + Environment.NewLine;
                return false;
            }

            return true;
        }

        private bool checkFloorAreaRatio(ref string msg)
        {
            double minimumPossibleFloorAreaRatio = variantDescription.getMinimumPossibleFloorAreaRatio();
            double maximumPossibleFloorAreaRatio = variantDescription.getMaximumPossibleFloorAreaRatio();

            if ((double)numUpDown_maxFloorAreaRatio.Value < minimumPossibleFloorAreaRatio
                || (double)numUpDown_minFloorAreaRatio.Value > maximumPossibleFloorAreaRatio)
            {
                msg += "Minimum possible floor area ratio for the current input: " 
                    + minimumPossibleFloorAreaRatio + Environment.NewLine;
                msg += "Maximum possible floor area ratio for the current input: " 
                    + maximumPossibleFloorAreaRatio + Environment.NewLine;
                return false;
            }
            return true;
        }

        private bool checkLayers(ref string msg)
        {
            selectContext();
            if (!selectGround(ref msg))
            {
                return false;
            }
            return true;
        }

        private bool selectGround(ref string msg)
        {
            if (!plotLayerSet)
            {
                msg += "Ground layer not set." + Environment.NewLine;
                return false;
            }

            RhinoObject[] rhobj = controller.doc.Objects.FindByLayer(txtBox_plot.Text.Trim());
            if (rhobj == null || rhobj.Length < 1)
            {
                msg += "No object in ground layer." + Environment.NewLine;
                return false;
            }

            List<Brep> ground = new List<Brep>();
            for (int i = 0; i < rhobj.Length; ++i)
            {
                if (rhobj[i].Geometry.HasBrepForm)
                {
                    ground.Add(Brep.TryConvertBrep(rhobj[i].Geometry));
                }
            }
            variantDescription.setGround(ground);
            return true;
        }

        private bool selectContext()
        {
            if (!contextLayerSet)
            {
                return false;
            }

            RhinoObject[] rhobj = controller.doc.Objects.FindByLayer(txtBox_context.Text.Trim());
            if (rhobj == null || rhobj.Length < 1)
            {
                return false;
            }

            List<Brep> context = new List<Brep>();
            for (int i = 0; i < rhobj.Length; ++i)
            {
                if (rhobj[i].Geometry.HasBrepForm)
                {
                    context.Add(Brep.TryConvertBrep(rhobj[i].Geometry));
                }
            }
            variantDescription.setContext(context);
            return true;
        }

        private void leaveContextTxtBox(object sender, EventArgs e)
        {
            if (txtBox_context.Text.Trim().Length == 0)
            {
                txtBox_context.ForeColor = watermarkColor;
                txtBox_context.Text = watermarkContext;
                contextLayerSet = false;
            }

        }

        private void leavePlotTxtBox(object sender, EventArgs e)
        {
            if (txtBox_plot.Text.Trim().Length == 0)
            {
                txtBox_plot.ForeColor = watermarkColor;
                txtBox_plot.Text = watermarkContext;
                plotLayerSet = false;
            }
        }

        private void enterPlotTxtBox(object sender, EventArgs e)
        {
            if (!plotLayerSet)
            {
                txtBox_plot.Clear();
                plotLayerSet = true;
                txtBox_plot.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void enterContextTxtBox(object sender, EventArgs e)
        {
            if (!contextLayerSet)
            {
                txtBox_context.Clear();
                contextLayerSet = true;
                txtBox_context.ForeColor = System.Drawing.Color.Black;
            }
        }

        public void update(object sender, UpdateEventArg e)
        {
            string caption = "Error";
            string message = "No solution found!";

            if (!IsDisposed)
            {
                if (numUpDown_generation.InvokeRequired || numUpDown_solution.InvokeRequired)
                {
                    SetSolutionsCallback d = new SetSolutionsCallback(update);
                    this.Invoke(d, new object[] { sender, e });
                }
                else
                {
                    variants = e.variants;
                    initialVariant = e.initialVariant;

                    if (variants.Count == 0)
                    {
                        enableInterface(true);
                        MessageBox.Show(message, caption, MessageBoxButtons.OK);
                        return;
                    }

                    numUpDown_generation.Minimum = 0;
                    if (variants.Count > 0)
                    {
                        numUpDown_generation.Maximum = variants.Count - 1;
                    }
                    else
                    {
                        numUpDown_generation.Maximum = 0;
                    }
                    numUpDown_generation.Value = numUpDown_generation.Maximum;

                    numUpDown_solution.Minimum = 1;
                    numUpDown_solution.Value = numUpDown_solution.Minimum;

                    // vérifier que l'indice donné pour la génération est plus petit ou égal au nombre de générations
                    if (variants.Count > (int)numUpDown_generation.Value)
                    {
                        // Mettre la valeur maximum possible de la sélection de la variante 
                        numUpDown_solution.Maximum = variants.ElementAt((int)numUpDown_generation.Value).Count;
                    }

                    controller.setSolution((int)numUpDown_generation.Value, (int)numUpDown_solution.Value - 1);

                    RefreshChartsEventArg args = new RefreshChartsEventArg();
                    args.totalVariants = this.variants;
                    args.selectedVariant = this.selectedSolution;
                    args.initialVariant = this.initialVariant;
                    OnRefreshCharts(args);

                    enableInterface(true);
                }
            }

        }

        private void enableInterface(bool status)
        {
            btn_generateSolutions.Enabled = status;
            btn_cancel.Enabled = !status;

            if (status)
            {
                checkBox_seed.Checked = seedStatus;
                checkBox_seed.Enabled = status;
                numUpDown_seed.Enabled = seedStatus;
            }
            else
            {
                seedStatus = checkBox_seed.Checked;
                checkBox_seed.Enabled = status;
                numUpDown_seed.Enabled = status;
            }

            if (variants.Count == 0)
            {
                numUpDown_generation.Enabled = false;
                numUpDown_solution.Enabled = false;

                btn_bake.Enabled = false;
                btn_bakeInitialVariant.Enabled = false;
                btn_showResults.Enabled = false;
                btn_export.Enabled = false;
            }
            else
            {
                numUpDown_generation.Enabled = status;
                numUpDown_solution.Enabled = status;

                btn_bake.Enabled = status;
                btn_bakeInitialVariant.Enabled = status;
                btn_showResults.Enabled = status;
                btn_export.Enabled = status;
            }

            if (variantDescription.buildingDescriptions.Count > 0)
            {
                btn_generateSolutions.Enabled = status;
            } 
            else
            {
                btn_generateSolutions.Enabled = false;
            }

            btn_setBuildings.Enabled = status;
            numUpDown_maxFloorAreaRatio.Enabled = status;
            numUpDown_minFloorAreaRatio.Enabled = status;
            numUpDown_minDistanceBtwBuildings.Enabled = false;
            numUpDown_minSiteCoverage.Enabled = status;
            numUpDown_maxSiteCoverage.Enabled = status;
            numUpDown_nbOfVariants.Enabled = status;
            numUpDown_numberOfGenerations.Enabled = status;

            txtBox_context.Enabled = status;
            txtBox_plot.Enabled = status;

            foreach (ResultWindow form in resultWindows)
            {
                if (!form.IsDisposed)
                {
                    if (status)
                    {
                        form.Show();
                    }
                    else
                    {
                        form.Hide();
                    }
                }
            }

            foreach (buildingForm form in buildingForms)
            {
                if (!form.IsDisposed)
                {
                    if (status)
                    {
                        form.Show();
                    }
                    else
                    {
                        form.Hide();
                    }
                }
            }
        }

        public void buildingAddedOrRemoved(object sender, BuildingAddedOrRemovedEventArg e)
        {
            btn_generateSolutions.Enabled = e.numberOfBuildings == 0 ? false : true;

            RefreshBuildingFormEventArg args = new RefreshBuildingFormEventArg();
            args.numberOfBuildings = e.numberOfBuildings;
            OnRefreshBuildingForm(args);
        }

        protected virtual void OnRefreshCharts(RefreshChartsEventArg e)
        {
            EventHandler<RefreshChartsEventArg> handler = RefreshCharts;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRefreshBuildingForm(RefreshBuildingFormEventArg e)
        {
            EventHandler<RefreshBuildingFormEventArg> handler = RefreshBuildingFrom;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void generateSolutions(object sender, EventArgs e)
        {
            string caption = "Error Detected in Input";
            string message = "";

            // Vérifier si un terrain a été donné
            if (checkLayers(ref message))
            {
                // Vérifier qu'il y ait au moins 1 bâtiment
                if (variantDescription.buildingDescriptions.Count > 0)
                {
                    // Vérifier site coverage et floor area ratio
                    if (checkFloorAreaRatio(ref message) && checkSiteCoverage(ref message))
                    {
                        // Contrôls OK !
                        if (!checkBox_seed.Checked)
                        {
                            variantDescription.seed = new Random().Next();
                        } 
                        else
                        {
                            variantDescription.seed = (int)numUpDown_seed.Value;
                        }
                        enableInterface(false);
                        controller.start(variantDescription);
                    }
                    else
                    {
                        MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Minimum 1 building!", caption, MessageBoxButtons.OK);
                }
            } 
            else
            {
                MessageBox.Show(message, caption, MessageBoxButtons.OK);
            }
        }

        private void btn_setBuildings_Click(object sender, EventArgs e)
        {
            buildingForm form = new buildingForm();
            form.AddBuilding += addBuilding;
            form.DeleteBuildings += deleteBuildings;
            
            buildingForms.Add(form);

            RefreshBuildingFrom += new EventHandler<RefreshBuildingFormEventArg>(form.refreshForm);

            form.Owner = this;

            RefreshBuildingFormEventArg args = new RefreshBuildingFormEventArg();
            args.numberOfBuildings = variantDescription.buildingDescriptions.Count;
            OnRefreshBuildingForm(args);

            form.Show();
        }

        public void addBuilding(object sender, AddBuildingEventArg e)
        {
            variantDescription.buildingDescriptions.Add(e.building);

            BuildingAddedOrRemovedEventArg args = new BuildingAddedOrRemovedEventArg();
            args.numberOfBuildings = variantDescription.buildingDescriptions.Count;
            OnBuildingAddedOrRemoved(args);

            controller.doc.Views.Redraw();
        }

        public void deleteBuildings(object sender, DeleteBuildingsEventArg e)
        {
            try
            {
                if (e.all)
                {
                    if (variantDescription != null)
                    {
                        if (variantDescription.buildingDescriptions != null)
                        {
                            variantDescription.buildingDescriptions.Clear();
                        }
                        controller.clearAllVariants();
                    }
                }
                else
                {
                    try
                    {
                        variantDescription.buildingDescriptions.RemoveAt(e.buildingNumber);
                    }
                    catch (IndexOutOfRangeException outExc)
                    {

                        System.Diagnostics.Debug.WriteLine(outExc);
                    }
                    catch (ArgumentNullException nullExc)
                    {
                        System.Diagnostics.Debug.WriteLine(nullExc);
                    }
                }

                BuildingAddedOrRemovedEventArg args = new BuildingAddedOrRemovedEventArg();

                if (variantDescription != null)
                {
                    args.numberOfBuildings = variantDescription.buildingDescriptions.Count;
                }
                else
                {
                    args.numberOfBuildings = 0;
                }

                OnBuildingAddedOrRemoved(args);
                controller.doc.Views.Redraw();
            } 
            catch (NullReferenceException exc)
            {
                System.Diagnostics.Debug.WriteLine(exc);
            }         
        }

        private void btn_showResults_Click(object sender, EventArgs e)
        {
            ResultWindow window = new ResultWindow();
            RefreshCharts += new EventHandler<RefreshChartsEventArg>(window.refreshChart);
            window.Owner = this;
            resultWindows.Add(window);

            RefreshChartsEventArg args = new RefreshChartsEventArg();

            if (this.selectedSolution != null && this.initialVariant != null && this.variants != null)
            {
                args.totalVariants = this.variants;
                args.selectedVariant = this.selectedSolution;
                args.initialVariant = this.initialVariant;
                OnRefreshCharts(args);
                window.Show();
            }
        }

        private void numUpDown_nbOfSolutions_ValueChanged(object sender, EventArgs e)
        {
            variantDescription.populationSize = (int)numUpDown_nbOfVariants.Value;
        }

        private void numUpDown_minDistanceBtwBuildings_ValueChanged(object sender, EventArgs e)
        {
            variantDescription.minDistanceBetweenBuildings = (double)numUpDown_minDistanceBtwBuildings.Value;
        }

        private void numUpDown_minSiteCoverage_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_maxSiteCoverage.Value < numUpDown_minSiteCoverage.Value)
            {
                numUpDown_maxSiteCoverage.Value = numUpDown_minSiteCoverage.Value;
            }
            variantDescription.minSiteCoverage = (double)numUpDown_minSiteCoverage.Value;
        }

        private void numUpDown_maxSiteCoverage_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_minSiteCoverage.Value > numUpDown_maxSiteCoverage.Value)
            {
                numUpDown_minSiteCoverage.Value = numUpDown_maxSiteCoverage.Value;
            }
            variantDescription.maxSiteCoverage = (double)numUpDown_maxSiteCoverage.Value;
        }

        private void numUpDown_minFloorAreaRatio_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_maxFloorAreaRatio.Value < numUpDown_minFloorAreaRatio.Value)
            {
                numUpDown_maxFloorAreaRatio.Value = numUpDown_minFloorAreaRatio.Value;
            }
            variantDescription.minFloorAreaRatio = (double)numUpDown_minFloorAreaRatio.Value;
        }

        private void numUpDown_maxFloorAreaRatio_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_minFloorAreaRatio.Value > numUpDown_maxFloorAreaRatio.Value)
            {
                numUpDown_minFloorAreaRatio.Value = numUpDown_maxFloorAreaRatio.Value;
            }
            variantDescription.maxFloorAreaRatio = (double)numUpDown_maxFloorAreaRatio.Value;
        }

        private void checkBox_seed_CheckedChanged(object sender, EventArgs e)
        {
            numUpDown_seed.Enabled = checkBox_seed.Checked;
        }

        private void numUpDown_seed_ValueChanged(object sender, EventArgs e)
        {
            variantDescription.seed = (int)numUpDown_seed.Value;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            controller.RequestStop();
        }

        private void checkBox_showSolutions_CheckedChanged(object sender, EventArgs e)
        {
            controller.drawSolution(checkBox_showSolutions.Checked);
        }

        private void checkBox_showDescriptions_CheckedChanged(object sender, EventArgs e)
        {
            controller.drawDescription(checkBox_showDescriptions.Checked);
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            controller.exportSolution((int)numUpDown_generation.Value, (int)numUpDown_solution.Value - 1);
        }

        private void btn_bake_Click(object sender, EventArgs e)
        {
            controller.bakeSolution((int)numUpDown_generation.Value, (int)numUpDown_solution.Value - 1);
        }

        private void btn_bakeInitialVariant_Click(object sender, EventArgs e)
        {
            controller.bakeInitialSolution();
        }

        private void numUpDown_generation_ValueChanged(object sender, EventArgs e)
        {
            selectedSolution.generation = (int)numUpDown_generation.Value;
            controller.setSolution(selectedSolution.generation, selectedSolution.variant);

            RefreshChartsEventArg args = new RefreshChartsEventArg();
            args.totalVariants = this.variants;
            args.selectedVariant = this.selectedSolution;
            args.initialVariant = this.initialVariant;
            OnRefreshCharts(args);
        }

        private void numUpDown_variant_ValueChanged(object sender, EventArgs e)
        {
            selectedSolution.variant = (int)numUpDown_solution.Value - 1;
            controller.setSolution(selectedSolution.generation, selectedSolution.variant);
            

            RefreshChartsEventArg args = new RefreshChartsEventArg();
            args.totalVariants = this.variants;
            args.selectedVariant = this.selectedSolution;
            args.initialVariant = this.initialVariant;
            OnRefreshCharts(args);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Owner = this;
            aboutBox.ShowDialog();
        }

        private void numUpDown_numberOfGenerations_ValueChanged(object sender, EventArgs e)
        {
            variantDescription.numberOfGenerations = (int)numUpDown_numberOfGenerations.Value;
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String openPDFFile = System.IO.Path.Combine(CST.FOLDER_PATH, "help.pdf");
            System.IO.File.WriteAllBytes(openPDFFile, Properties.Resources.faq);
            try
            {
                System.Diagnostics.Process.Start(openPDFFile);
            }
            catch 
            {
                string message = "You need a PDF viewer to view or print this document. "
                    + "(you can download and install most PDF viewers, such as Adobe Reader, for free)";
                string caption = "Error";
                MessageBox.Show(message, caption, MessageBoxButtons.OK);
            }
        }
    }


    public class BuildingAddedOrRemovedEventArg : EventArgs
    {
        public int numberOfBuildings { get; set; }
    } 

    public class RefreshChartsEventArg : EventArgs
    {
        public List<List<Variant>> totalVariants { get; set; }
        public Variant initialVariant { get; set; }
        public Pair<int, int> selectedVariant { get; set; }
    }

    public class RefreshBuildingFormEventArg : EventArgs
    {
        public int numberOfBuildings { get; set; }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public Program Value { get; set; }

        public ComboboxItem(string Text, Program Value)
        {
            this.Text = Text;
            this.Value = Value;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class ComboboxItemGeneric
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public ComboboxItemGeneric(string text, int value)
        {
            this.Text = text;
            this.Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class Pair<U, V>
    {
        public U generation { get; set; }
        public V variant { get; set; }

        public Pair(U generation, V variant)
        {
            this.generation = generation;
            this.variant = variant;
        }
    }
}