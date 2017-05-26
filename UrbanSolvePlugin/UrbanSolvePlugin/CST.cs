using System.Drawing;

namespace UrbanSolvePlugin
{
    class CST
    {
        public static Font textFont = new Font("Calibri", 11, FontStyle.Regular);
        public static Font textTitle = new Font("Calibri", 11, FontStyle.Bold);

        public const string TMP_DIRECTORY_NAME = "UrbanSolveTemp";
        public const string SKY_FILE_NAME = "sky";
        public const string FOLDER_PATH = "C:\\urb";

        public const int NB_OF_OBJECTIVES = 3;

        public const int NB_OF_GENERATIONS = 2;
        public const int MIN_NB_OF_GENERATIONS = 2;
        public const int MAX_NB_OF_GENERATIONS = 1000;

        public const int POPULATION_SIZE = 10;
        public const int MIN_POPULATION_SIZE = 10;
        public const int MAX_POPULATION_SIZE = 1000;

        public const double MIN_SITE_COVERAGE = 0.0;
        public const double MAX_SITE_COVERAGE = 1.0;

        // IUS
        public const double MIN_FLOOR_AREA_RATIO = 0.0;
        public const double MAX_FLOOR_AREA_RATIO = 10.0;

        public const double MIN_GLAZING_RATIO_OFFICE = 0.3;
        public const double MAX_GLAZING_RATIO_OFFICE = 0.8;
        public const double MIN_GLAZING_RATIO_APPART = 0.1;
        public const double MAX_GLAZING_RATIO_APPART = 0.5;

        public const double MIN_CENTER_SIZE = 3.0;

        // Bâtiments simples
        public const double INITIAL_LENGTH_WIDTH_SIMPLE = 12.0;
        public const double MIN_LENGTH_WIDTH_SIMPLE = 8.0;
        public const double MAX_LENGTH_WIDTH_SIMPLE = 200.0;

        // Bâtiments cours et en L
        public const double INITIAL_LENGTH_WIDTH = 20.0;
        public const double MIN_LENGTH_WIDTH = 15.0;
        public const double MAX_LENGTH_WIDTH = 200.0;

        public const double INITIAL_DEPTH = 8.0;
        public const double MIN_DEPTH = 6.0;
        public const double MAX_DEPTH = 199.0;

        // Pour toutes les typologies
        public const double STOREY_HEIGHT = 3.0;
        public const double MIN_STOREY_HEIGHT = 2.0;
        public const double MAX_STOREY_HEIGHT = 5.0;

        public const int INITIAL_STOREY_NB = 4;
        public const int MIN_STOREY_NB = 2;
        public const int MAX_STOREY_NB = 15;

        #region DISPLAY
        public const int TEXT_SIZE = 30;

        public const double MIN_IRR_VALUE = 0.0;
        public const double MAX_IRR_VALUE = 1200.0;

        // Texte dans les interfaces noires
        public static Color DESCRIPTION_COLOR_BLACK = Color.FromArgb(255, 255, 255);
        public static Color SOLUTION_COLOR_BLACK = Color.FromArgb(255, 239, 139);

        // Texte dans les interfaces blanches 
        public static Color DESCRIPTION_COLOR_WHITE = Color.FromArgb(0, 0, 0);
        public static Color SOLUTION_COLOR_WHITE = Color.FromArgb(255, 212, 13);

        // Volumes en 3D ------------------------------------------------------------------
        // Blanc
        public static Color DESCRIPTION_COLOR_3D = Color.FromArgb(255, 255, 255);
        // Gris clair
        public static Color SOLUTION_COLOR_3D = Color.FromArgb(200, 200, 200);

        public static Color DESCRIPTION_COLOR_3D_TEXT = Color.FromArgb(230, 230, 230);
        public static Color SOLUTION_COLOR_3D_TEXT = Color.FromArgb(255, 212, 1);

        public const double MATERIAL_TRANSPARENCY = 0.3;
        public const int CENTER_POINT_SIZE = 5;

        public const int IRRAD_POINT_SIZE = 10;
        #endregion

        #region CHARTS
        public static Font AXIS_FONT = new Font("Arial", 10, FontStyle.Regular);
        public const double RESULT_ERROR_daylight = 4.0;
        public const double RESULT_ERROR = 1.0;
        #endregion

        public const string BAKE_LAYER_NAME = "UrbanSolve";
        public const string TMP_LAYER_NAME = "tmp";
        public static Color BAKE_LAYER_COLOR = Color.Red;

        #region OPTIMIZATION
        public const int MESH_DENSITY = 30;
        #endregion
    }
}
