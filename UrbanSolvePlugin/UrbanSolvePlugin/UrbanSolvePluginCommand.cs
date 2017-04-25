using Rhino;
using Rhino.Commands;


namespace UrbanSolvePlugin
{
    [   
        System.Runtime.InteropServices.Guid("3d9e37a0-6684-43dc-a261-c071b052d91d"),
        CommandStyle(Style.ScriptRunner)
    ]
    public class UrbanSolvePluginCommand : Command
    {

        public UrbanSolvePluginCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static UrbanSolvePluginCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "UrbanSOLve"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            new UrbanSolveController(doc);
            return Result.Success;
        }
    }
}