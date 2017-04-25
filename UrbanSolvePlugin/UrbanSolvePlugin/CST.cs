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

        public const int NB_OF_GENERATIONS = 5;
        public const int MIN_NB_OF_GENERATIONS = 5;
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
        public const double MAX_LENGTH_WIDTH_SIMPLE = 36.0;

        // Bâtiments cours et en L
        public const double INITIAL_LENGTH_WIDTH = 20.0;
        public const double MIN_LENGTH_WIDTH = 15.0;
        public const double MAX_LENGTH_WIDTH = 55.0;

        public const double INITIAL_DEPTH = 8.0;
        public const double MIN_DEPTH = 6.0;
        public const double MAX_DEPTH = 20.0;

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
        public const double RESULT_ERROR = 8.0;
        #endregion

        public const string BAKE_LAYER_NAME = "UrbanSolve";
        public const string TMP_LAYER_NAME = "tmp";
        public static Color BAKE_LAYER_COLOR = Color.Red;

        #region OPTIMIZATION
        public const int MESH_DENSITY = 30;
        public const double DAYLIGHT_AUTONOMY_CST = 47.6673;

        // DAYLIGHT AUTONOMY -------------------------------------------------------------------
        public const double da_off_constant = -80.742562485692;
        public const double da_off_plotRatio = -9.6393337165692;
        public const double da_off_siteCoverage = 547.096561071363;
        public const double da_off_exEnvArea = 0.0042596026831734;
        public const double da_off_footprintArea = 0.166269008734934;
        public const double da_off_formFactor = -40.8010358472293;
        public const double da_off_eastFacRatio = -174.926844561956;
        public const double da_off_southFacRatio = -18.2548209923902;
        public const double da_off_wwRatio = -161.346611900301;
        public const double da_off_wfRatio = 304.091625539356;
        public const double da_off_meanRoofIrr = -0.00124669278508557;
        public const double da_off_meanFacIrr = 0.0563368857359545;
        public const double da_off_meanSouthFacIrr = -0.0103907708870867;
        public const double da_off_facIrrPerFa = 0.187308861661125;
        public const double da_off_eastFacIrrPrFa = -0.0280547955951425;
        public const double da_off_PlotRatio_ExpEnvArea = 0.00123832889465642;
        public const double da_off_SiteCoverage_MeanFacIrrad = -0.857390511693495;
        public const double da_off_ExpEnvArea_FootprintArea = -0.00000321764820669752;
        public const double da_off_ExpEnvArea_FormFactor = -0.00508061791889866;
        public const double da_off_FootprintArea_MeanRoofIrrad = -0.000120092877308541;
        public const double da_off_FormFactor_FacIrradPerFA = 0.0828921713701716;
        public const double da_off_EastFacRatio_MeanFacIrrad = 0.296426561524879;
        public const double da_off_WWRatio_MeanFacIrrad = 0.258065769115172;
        public const double da_off_WFRatio_FacIrradPerFA = -0.3854215973439;

        public const double da_app_constant = 81.1632920947334;
        public const double da_app_expEnvArea = -0.0000983165166846544;
        public const double da_app_formFactor = -125.77610906595;
        public const double da_app_wwRatio = -6.1028869159489;
        public const double da_app_wfRatio = 135.486601154908;
        public const double da_app_meanRoofIrr = -0.11084066706723;
        public const double da_app_meanFacIrr = 0.0620336225845539;
        public const double da_app_roofIrrPerFa = 0.00481984071744582;
        public const double da_app_northFacIrrPerFa = 0.212764031275194;
        public const double da_app_southFacIrrPerFa = -0.0689059101335217;
        public const double da_app_ExpEnvArea_WFRatio = 0.00801325064995131;
        public const double da_app_FormFactor_MeanRoofIrrad = 0.0962278093607832;
        public const double da_app_WWRatio_WFRatio = -253.454793675204;
        public const double da_app_WFRatio_MeanFacIrrad = 0.323756788929631;
        public const double da_app_WFRatio_RoofIrradPerFA = 0.140164041356989;

        // ------------------------------------------------------------------------
        public const double off_constant = 19.1141836219013;
        public const double off_plotRatio = 0.545327925719241;
        public const double off_siteCoverage = -4.21541349720615;
        // Surface de plancher totale / surface de l'enveloppe totale
        public const double off_formFactor = 5.18100882645298;
        public const double off_wwRatio = -4.031364260512;
        public const double off_wfRatio = 31.7371154663083;
        public const double off_meanRoofIrr = -0.00388886141753062;
        public const double off_meanSouthFacIrr = -0.00401012938870747;
        public const double off_envelopIrrPerFa = 0.0133796968776753;
        public const double off_meanEnvelopIrr = 0.00559981215318079;
        public const double off_roofIrrPerFa = -0.00822106779268896;
        public const double off_northFacIrrPerFa = 0.0354635370007791;
        public const double off_southFacIrrPerFa = -0.0224093062859373;

        public const double app_constant = 19.7397994132848;
        public const double app_nbOfFloors = 0.0615357387233081;
        // Surface de plancher totale / surface de l'enveloppe totale
        public const double app_formFactor = 14.6731606719712;
        // Rapport entre la surface de facade Nord et celle de l'enveloppe totale
        public const double app_northFacRatio = -1.78227727837842;
        // Rapport entre la surface vitrée et la surface de facade totale (donnée en entré de l'utilisateur)
        public const double app_wwRatio = -9.68907491773802;
        // Rapport entre la surface vitrée et la surface de plancher totale(à calculer à partir du WWRatio)
        public const double app_wfRatio = 54.6831757658107;
        public const double app_meanNorthFacIrr = 0.00832287683475975;
        public const double app_meanSouthFacIrr = -0.00939446283748767;
        public const double app_eastFacIrrPerFa = 0.0201894855414749;
        public const double app_roofRatio = 6.06499218329751;
        public const double app_meanEnvelopIrr = 0.00695622593372852;

        #endregion
    }
}
