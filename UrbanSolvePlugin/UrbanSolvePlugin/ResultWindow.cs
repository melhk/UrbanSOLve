using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System;
using System.Collections.Generic;

namespace UrbanSolvePlugin
{
    public partial class ResultWindow : Form
    {
        private Series series_daylightAutonomy_ActiveSolarEnergy;
        private Series series_daylightAutonomy_EnergyNeed;
        private Series series_ActiveSolarEnergy_EnergyNeed;

        private TableLayoutPanel resultPanel;

        Label lbl_initialVariant;
        Label lbl_selectedVariant;
        Label lbl_daylightAutonomy;
        Label lbl_init_daylightAutonomy;
        Label lbl_activeSolarEnergy;
        Label lbl_init_activeSolarEnergy;
        Label lbl_eneryNeed;
        Label lbl_init_energyNeed;
        Label lbl_siteCoverage;
        Label lbl_init_siteCoverage;
        Label lbl_floorAreaRatio;
        Label lbl_init_floorAreaRatio;
        Label lbl_totalFloorArea;
        Label lbl_init_totalFloorArea;
        Label lbl_totalEnvelopArea;
        Label lbl_init_totalEnvelopArea;
        Label lbl_var_daylightAutonomy;
        Label lbl_var_activeSolarEnergy;
        Label lbl_var_energyNeed;
        Label lbl_var_siteCoverage;
        Label lbl_var_floorAreaRatio;
        Label lbl_var_totalFloorArea;
        Label lbl_var_totalEnvelopArea;

        private List<Control> controls;


        public ResultWindow()
        {
            InitializeComponent();

            series_daylightAutonomy_ActiveSolarEnergy = new Series();
            series_daylightAutonomy_EnergyNeed = new Series();
            series_ActiveSolarEnergy_EnergyNeed = new Series();

            resultPanel = new TableLayoutPanel();
            controls = new List<Control>();

            series_daylightAutonomy_ActiveSolarEnergy.ChartType = SeriesChartType.ErrorBar;
            series_daylightAutonomy_EnergyNeed.ChartType = SeriesChartType.ErrorBar;
            series_ActiveSolarEnergy_EnergyNeed.ChartType = SeriesChartType.ErrorBar;

            series_daylightAutonomy_ActiveSolarEnergy["ErrorBarCenterMarkerStyle"] = "Circle";
            series_daylightAutonomy_EnergyNeed["ErrorBarCenterMarkerStyle"] = "Circle";
            series_ActiveSolarEnergy_EnergyNeed["ErrorBarCenterMarkerStyle"] = "Circle";
            series_daylightAutonomy_ActiveSolarEnergy["PixelPointWidth"] = "15";
            series_daylightAutonomy_EnergyNeed["PixelPointWidth"] = "15";
            series_ActiveSolarEnergy_EnergyNeed["PixelPointWidth"] = "15";

            series_daylightAutonomy_ActiveSolarEnergy.YValuesPerPoint = 3;
            series_daylightAutonomy_EnergyNeed.YValuesPerPoint = 3;
            series_ActiveSolarEnergy_EnergyNeed.YValuesPerPoint = 3;

            MSChartExtension.EnableZoomAndPanControls(chart1);
            MSChartExtension.EnableZoomAndPanControls(chart2);
            MSChartExtension.EnableZoomAndPanControls(chart3);

            series_daylightAutonomy_ActiveSolarEnergy.MarkerSize = 10;
            series_daylightAutonomy_EnergyNeed.MarkerSize = 10;
            series_ActiveSolarEnergy_EnergyNeed.MarkerSize = 10;

            series_daylightAutonomy_ActiveSolarEnergy.Color = Color.Black;
            series_daylightAutonomy_EnergyNeed.Color = Color.Black;
            series_ActiveSolarEnergy_EnergyNeed.Color = Color.Black;

            chart1.Series.Add(series_daylightAutonomy_ActiveSolarEnergy);
            chart2.Series.Add(series_daylightAutonomy_EnergyNeed);
            chart3.Series.Add(series_ActiveSolarEnergy_EnergyNeed);

            chart1.ChartAreas[0].AxisX.Title = "Annual on-site electricity production [kWh/m\u00B2]";
            chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            chart1.ChartAreas[0].AxisX.TitleFont = CST.AXIS_FONT;
            chart1.ChartAreas[0].AxisX.Minimum = 0.0;
            chart1.ChartAreas[0].AxisY.Title = "Daylit area over year [%]";
            chart1.ChartAreas[0].AxisY.TitleFont = CST.AXIS_FONT;
            chart1.ChartAreas[0].AxisY.Minimum = 0.0;

            chart2.ChartAreas[0].AxisX.Title = "Daylit area over year [%]";
            chart2.ChartAreas[0].AxisX.TitleFont = CST.AXIS_FONT;
            chart2.ChartAreas[0].AxisX.Minimum = 0.0;
            chart2.ChartAreas[0].AxisY.Title = "Annual energy need for heating and cooling [kWh/m\u00B2]";
            chart2.ChartAreas[0].AxisY.TitleFont = CST.AXIS_FONT;
            chart2.ChartAreas[0].AxisY.Minimum = 0.0;

            chart3.ChartAreas[0].AxisX.Title = "Annual on-site electricity production [kWh/m\u00B2]";
            chart3.ChartAreas[0].AxisX.TitleFont = CST.AXIS_FONT;
            chart3.ChartAreas[0].AxisX.Minimum = 0.0;
            chart3.ChartAreas[0].AxisY.Title = "Annual energy need for heating and cooling [kWh/m\u00B2]";
            chart3.ChartAreas[0].AxisY.TitleFont = CST.AXIS_FONT;
            chart3.ChartAreas[0].AxisY.Minimum = 0.0;

            // Sidebar
            createResultPannel();
        }

        private void createResultPannel()
        {
            resultPanel.Dock = DockStyle.Fill;
            resultPanel.AutoSize = true;
            resultPanel.AutoScroll = true;
            resultPanel.AutoSizeMode = AutoSizeMode.GrowOnly;
            groupBox_variant.Controls.Add(resultPanel);

            resultPanel.RowCount = 8;
            for (int k = 0; k < 8; ++k)
            {
                resultPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            lbl_initialVariant = new Label()
            {
                Text = String.Format("Initial variant"),
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_initialVariant, 1, 0);

            lbl_selectedVariant = new Label()
            {  
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_selectedVariant, 2, 0);

            lbl_daylightAutonomy = new Label()
            {
                Text = String.Format("Daylit area"),
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_daylightAutonomy, 0, 1);

            lbl_init_daylightAutonomy = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_init_daylightAutonomy, 1, 1);

            lbl_var_daylightAutonomy = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_var_daylightAutonomy, 2, 1);


            lbl_eneryNeed = new Label()
            {
                Text = String.Format("Energy need"),
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_eneryNeed, 0, 2);

            lbl_init_energyNeed = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_init_energyNeed, 1, 2);

            lbl_var_energyNeed = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_var_energyNeed, 2, 2);


            lbl_activeSolarEnergy = new Label()
            {
                Text = String.Format("Electricity production"),
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_activeSolarEnergy, 0, 3);

            lbl_init_activeSolarEnergy = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_init_activeSolarEnergy, 1, 3);

            lbl_var_activeSolarEnergy = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_var_activeSolarEnergy, 2, 3);

            lbl_siteCoverage = new Label()
            {
                Text = String.Format("Site coverage"),
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_siteCoverage, 0, 4);

            lbl_init_siteCoverage = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_init_siteCoverage, 1, 4);

            lbl_var_siteCoverage = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_var_siteCoverage, 2, 4);


            lbl_floorAreaRatio = new Label()
            {
                Text = String.Format("Floor area ratio"),
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_floorAreaRatio, 0, 5);

            lbl_init_floorAreaRatio = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_init_floorAreaRatio, 1, 5);

            lbl_var_floorAreaRatio = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_var_floorAreaRatio, 2, 5);


            lbl_totalFloorArea = new Label()
            {
                Text = String.Format("Total floor area"),
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_totalFloorArea, 0, 6);

            lbl_init_totalFloorArea = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_init_totalFloorArea, 1, 6);

            lbl_var_totalFloorArea = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_var_totalFloorArea, 2, 6);


            lbl_totalEnvelopArea = new Label()
            {
                Text = String.Format("Total envelop area"),
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_totalEnvelopArea, 0, 7);

            lbl_init_totalEnvelopArea = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_init_totalEnvelopArea, 1, 7);

            lbl_var_totalEnvelopArea = new Label()
            {
                Margin = new Padding(5),
                Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true
            };
            resultPanel.Controls.Add(lbl_var_totalEnvelopArea, 2, 7);
        }

        public void refreshChart(object sender, RefreshChartsEventArg e)
        {
            if (!IsDisposed)
            {
                series_ActiveSolarEnergy_EnergyNeed.Points.Clear();
                series_daylightAutonomy_ActiveSolarEnergy.Points.Clear();
                series_daylightAutonomy_EnergyNeed.Points.Clear();

                if (e.totalVariants != null)
                {
                    int i = 1;
                    if (e.totalVariants.Count > 0)
                    {
                        if (e.selectedVariant.generation == -1 || e.selectedVariant.variant == -1)
                        {

                        }
                        else if (e.selectedVariant.generation < e.totalVariants.Count())
                        {
                            if (e.selectedVariant.variant < e.totalVariants.ElementAt(e.selectedVariant.generation).Count())
                            {
                                Hide();
                                resultPanel.Parent.SuspendLayout();
                                resultPanel.ResumeLayout();
                                panel3.Hide();

                                Variant selectedVariant = e.totalVariants.ElementAt(e.selectedVariant.generation).ElementAt(e.selectedVariant.variant);

                                // TEXT GENERAL
                                lbl_selectedVariant.Text = String.Format("Variant {0} - Generation {1}", e.selectedVariant.variant + 1, e.selectedVariant.generation);

                                if (e.initialVariant.isValid)
                                {
                                    lbl_init_daylightAutonomy.Text = String.Format("{0} %", (double)decimal.Round((decimal)e.initialVariant.daylightAutonomy, 2, MidpointRounding.AwayFromZero));
                                    lbl_init_energyNeed.Text = String.Format("{0} kWh/m\u00B2", (double)decimal.Round((decimal)e.initialVariant.energyNeed, 2, MidpointRounding.AwayFromZero));
                                    lbl_init_activeSolarEnergy.Text = String.Format("{0} kWh/m\u00B2", (double)decimal.Round((decimal)e.initialVariant.activeSolarEnergy, 2, MidpointRounding.AwayFromZero));
                                    lbl_init_siteCoverage.Text = String.Format("{0}", (double)decimal.Round((decimal)e.initialVariant.siteCoverage, 2, MidpointRounding.AwayFromZero));
                                    lbl_init_floorAreaRatio.Text = String.Format("{0}", (double)decimal.Round((decimal)e.initialVariant.floorAreaRatio, 2, MidpointRounding.AwayFromZero));
                                    lbl_init_totalFloorArea.Text = String.Format("{0} m\u00B2", (double)decimal.Round((decimal)e.initialVariant.totalFloorArea, 2, MidpointRounding.AwayFromZero));
                                    lbl_init_totalEnvelopArea.Text = String.Format("{0} m\u00B2", (double)decimal.Round((decimal)e.initialVariant.totalEnvelopArea, 2, MidpointRounding.AwayFromZero));
                                }
                                else
                                {
                                    lbl_init_daylightAutonomy.Text = String.Format("Invalid variant");
                                    lbl_init_energyNeed.Text = String.Format("Invalid variant");
                                    lbl_init_activeSolarEnergy.Text = String.Format("Invalid variant");
                                    lbl_init_siteCoverage.Text = String.Format("Invalid variant");
                                    lbl_init_floorAreaRatio.Text = String.Format("Invalid variant");
                                    lbl_init_totalFloorArea.Text = String.Format("Invalid variant");
                                    lbl_init_totalEnvelopArea.Text = String.Format("Invalid variant");
                                }

                                if (selectedVariant.isValid)
                                {
                                    lbl_var_daylightAutonomy.Text = String.Format("{0} %", (double)decimal.Round((decimal)selectedVariant.daylightAutonomy, 2, MidpointRounding.AwayFromZero));
                                    lbl_var_activeSolarEnergy.Text = String.Format("{0} kWh/m\u00B2", (double)decimal.Round((decimal)selectedVariant.activeSolarEnergy, 2, MidpointRounding.AwayFromZero));
                                    lbl_var_energyNeed.Text = String.Format("{0} kWh/m\u00B2", (double)decimal.Round((decimal)selectedVariant.energyNeed, 2, MidpointRounding.AwayFromZero));
                                    lbl_var_siteCoverage.Text = String.Format("{0}", (double)decimal.Round((decimal)selectedVariant.siteCoverage, 2, MidpointRounding.AwayFromZero));
                                    lbl_var_floorAreaRatio.Text = String.Format("{0}", (double)decimal.Round((decimal)selectedVariant.floorAreaRatio, 2, MidpointRounding.AwayFromZero));
                                    lbl_var_totalFloorArea.Text = String.Format("{0} m\u00B2", (double)decimal.Round((decimal)selectedVariant.totalFloorArea, 2, MidpointRounding.AwayFromZero));
                                    lbl_var_totalEnvelopArea.Text = String.Format("{0} m\u00B2", (double)decimal.Round((decimal)selectedVariant.totalEnvelopArea, 2, MidpointRounding.AwayFromZero));
                                }
                                else
                                {                           
                                    lbl_var_daylightAutonomy.Text = String.Format("Invalid variant");
                                    lbl_var_activeSolarEnergy.Text = String.Format("Invalid variant");
                                    lbl_var_energyNeed.Text = String.Format("Invalid variant");
                                    lbl_var_siteCoverage.Text = String.Format("Invalid variant");
                                    lbl_var_floorAreaRatio.Text = String.Format("Invalid variant");
                                    lbl_var_totalFloorArea.Text = String.Format("Invalid variant");
                                    lbl_var_totalEnvelopArea.Text = String.Format("Invalid variant");
                                }


                                // Solution ---------------------------------------------------------------------------------------------

                                resultPanel.RowCount = 8;
                                foreach (Control c in controls)
                                {
                                    resultPanel.Controls.Remove(c);
                                }
                           
                                for (int k = 0; k < e.totalVariants.ElementAt(e.selectedVariant.generation) 
                                                        .ElementAt(e.selectedVariant.variant).totalNumberOfBuildings; ++k)
                                {
                                    int rowNb = resultPanel.RowCount;
                                    // resultPanel.RowCount += 8;
                                    resultPanel.RowCount += 11;
                                    for (int m = 0; m < 11; ++m)
                                    {
                                        resultPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                                    }

                                    Label title = new Label()
                                    {
                                        Text = String.Format("------ Building number {0} ------", k + 1),
                                        Margin = new Padding(8),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true,
                                        TextAlign = ContentAlignment.MiddleLeft,                                 
                                    };
                                    resultPanel.Controls.Add(title, 1, rowNb);
                                    controls.Add(title);
                                    resultPanel.SetColumnSpan(title, 2);

                                    // Performance --------------------------------------------------------
                                    Label lbl_daylightAutonomy = new Label()
                                    {
                                        Text = "Daylit area",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_daylightAutonomy, 0, rowNb + 1);
                                    controls.Add(lbl_daylightAutonomy);

                                    Label lbl_energyNeed = new Label()
                                    {
                                        Text = "Energy need",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_energyNeed, 0, rowNb + 2);
                                    controls.Add(lbl_energyNeed);

                                    Label lbl_activeSolarEnergy = new Label()
                                    {
                                        Text = "Electricity production",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_activeSolarEnergy, 0, rowNb + 3);
                                    controls.Add(lbl_activeSolarEnergy);

                                    if (e.initialVariant.isValid)
                                    {
                                        Label lbl_init_daylightAutonomy = new Label()
                                        {
                                            Text = String.Format("{0} %", (double)decimal.Round((decimal)e.initialVariant.buildings.ElementAt(k).daylightAutonomy, 2, MidpointRounding.AwayFromZero)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_daylightAutonomy, 1, rowNb + 1);
                                        controls.Add(lbl_init_daylightAutonomy);

                                        Label lbl_init_energyNeed = new Label()
                                        {
                                            Text = String.Format("{0} kWh/m\u00B2", (double)decimal.Round((decimal)e.initialVariant.buildings.ElementAt(k).energyNeed, 2, MidpointRounding.AwayFromZero)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_energyNeed, 1, rowNb + 2);
                                        controls.Add(lbl_init_energyNeed);

                                        Label lbl_init_activeSolarEnergy = new Label()
                                        {
                                            Text = String.Format("{0} kWh/m\u00B2", (double)decimal.Round((decimal)e.initialVariant.buildings.ElementAt(k).activeSolarEnergy, 2, MidpointRounding.AwayFromZero)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_activeSolarEnergy, 1, rowNb + 3);
                                        controls.Add(lbl_init_activeSolarEnergy);
                                    }

                                    if (selectedVariant.isValid)
                                    {
                                        Label lbl_var_daylightAutonomy = new Label()
                                        { 
                                            Text = String.Format("{0} %", (double)decimal.Round((decimal)selectedVariant.buildings.ElementAt(k).daylightAutonomy, 2, MidpointRounding.AwayFromZero)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_daylightAutonomy, 2, rowNb + 1);
                                        controls.Add(lbl_var_daylightAutonomy);

                                        Label lbl_var_energyNeed = new Label()
                                        {
                                            Text = String.Format("{0} kWh/m\u00B2", (double)decimal.Round((decimal)selectedVariant.buildings.ElementAt(k).energyNeed, 2, MidpointRounding.AwayFromZero)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_energyNeed, 2, rowNb + 2);
                                        controls.Add(lbl_var_energyNeed);

                                        Label lbl_var_activeSolarEnergy = new Label()
                                        {
                                            Text = String.Format("{0} kWh/m\u00B2", (double)decimal.Round((decimal)selectedVariant.buildings.ElementAt(k).activeSolarEnergy, 2, MidpointRounding.AwayFromZero)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_activeSolarEnergy, 2, rowNb + 3);
                                        controls.Add(lbl_var_activeSolarEnergy);
                                    }

                                    // Typology ------------------------------------------------------
                                    Label lbl_typology = new Label()
                                    {
                                        Text = "Typology",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_typology, 0, rowNb + 4);
                                    controls.Add(lbl_typology);

                                    if (e.initialVariant.isValid)
                                    {
                                        Label lbl_init_typology = new Label()
                                        {
                                            Text = String.Format("{0}",
                                                EnumMethods.GetDescriptionFromEnumValue(e.initialVariant.buildings.ElementAt(k).description.typology)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_typology, 1, rowNb + 4);
                                        controls.Add(lbl_init_typology);
                                    }

                                    if (selectedVariant.isValid)
                                    {
                                        Label lbl_var_typology = new Label()
                                        {  
                                            Text = String.Format("{0}",
                                                EnumMethods.GetDescriptionFromEnumValue(selectedVariant.buildings.ElementAt(k).description.typology)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_typology, 2, rowNb + 4);
                                        controls.Add(lbl_var_typology);
                                    }

                                    // Function -------------------------------------------------------------
                                    Label lbl_function = new Label()
                                    {
                                        Text = "Function",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_function, 0, rowNb + 5);
                                    controls.Add(lbl_function);

                                    if (e.initialVariant.isValid)
                                    {
                                        Label lbl_init_function = new Label()
                                        {
                                            Text = String.Format("{0}",
                                               EnumMethods.GetDescriptionFromEnumValue(e.initialVariant.buildings.ElementAt(k).description.program)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_function, 1, rowNb + 5);
                                        controls.Add(lbl_init_function);
                                    }

                                    if (selectedVariant.isValid)
                                    {
                                        Label lbl_var_function = new Label()
                                        {
                                            Text = String.Format("{0}",
                                            EnumMethods.GetDescriptionFromEnumValue(selectedVariant.buildings.ElementAt(k).description.program)),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_function, 2, rowNb + 5);
                                        controls.Add(lbl_var_function);
                                    }

                                    // Glazing ratio ---------------------------------------------------------------------------
                                    Label lbl_glazingRatio = new Label()
                                    {
                                        Text = "Glazing ratio",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_glazingRatio, 0, rowNb + 6);
                                    controls.Add(lbl_glazingRatio);

                                    if (e.initialVariant.isValid)
                                    {
                                        Label lbl_init_glazingRatio = new Label()
                                        {
                                            Text = String.Format("{0}", e.initialVariant.buildings.ElementAt(k).description.glazingRatio),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_glazingRatio, 1, rowNb + 6);
                                        controls.Add(lbl_init_glazingRatio);
                                    }

                                    if (selectedVariant.isValid)
                                    {
                                        Label lbl_var_glazingRatio = new Label()
                                        {
                                            Text = String.Format("{0}",
                                           selectedVariant.buildings.ElementAt(k).description.glazingRatio),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_glazingRatio, 2, rowNb + 6);
                                        controls.Add(lbl_var_glazingRatio);
                                    }

                                    // Length ----------------------------------------------------------------------
                                    Label lbl_length = new Label()
                                    {
                                        Text = "Length",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_length, 0, rowNb + 7);
                                    controls.Add(lbl_length);

                                    if (e.initialVariant.isValid)
                                    {
                                        Label lbl_init_length = new Label()
                                        {
                                            Text = String.Format("{0} m", e.initialVariant.buildings.ElementAt(k).length),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_length, 1, rowNb + 7);
                                        controls.Add(lbl_init_length);
                                    }

                                    if (selectedVariant.isValid)
                                    {
                                        Label lbl_var_length = new Label()
                                        {
                                            Text = String.Format("{0} m",
                                                selectedVariant.buildings.ElementAt(k).length),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_length, 2, rowNb + 7);
                                        controls.Add(lbl_var_length);
                                    }

                                    // Width -----------------------------------------------------------------------------
                                    Label lbl_width = new Label()
                                    {
                                        Text = "Width",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_width, 0, rowNb + 8);
                                    controls.Add(lbl_width);

                                    if (e.initialVariant.isValid)
                                    {
                                        Label lbl_init_width = new Label()
                                        {
                                            Text = String.Format("{0} m", e.initialVariant.buildings.ElementAt(k).width),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_width, 1, rowNb + 8);
                                        controls.Add(lbl_init_width);
                                    }

                                    if (selectedVariant.isValid)
                                    {
                                        Label lbl_var_width = new Label()
                                        {
                                            Text = String.Format("{0} m",
                                           selectedVariant.buildings.ElementAt(k).width),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_width, 2, rowNb + 8);
                                        controls.Add(lbl_var_width);
                                    }
                                   
                                    // Number of floors -------------------------------------------------------------------
                                    Label lbl_nbOfFloors = new Label()
                                    {
                                        Text = "Number of floors",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_nbOfFloors, 0, rowNb + 9);
                                    controls.Add(lbl_nbOfFloors);

                                    if (e.initialVariant.isValid)
                                    {
                                        Label lbl_init_nbOfFloors = new Label()
                                        {
                                            Text = String.Format("{0}", e.initialVariant.buildings.ElementAt(k).numberOfFloors),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_nbOfFloors, 1, rowNb + 9);
                                        controls.Add(lbl_init_nbOfFloors);

                                    }

                                    if (selectedVariant.isValid)
                                    {
                                        Label lbl_var_nbOfFloors = new Label()
                                        {
                                            Text = String.Format("{0}",
                                                 selectedVariant.buildings.ElementAt(k).numberOfFloors),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_nbOfFloors, 2, rowNb + 9);
                                        controls.Add(lbl_var_nbOfFloors);
                                    }

                                    // Storey height -------------------------------------------------------------------------
                                    Label lbl_storeyHeight = new Label()
                                    {
                                        Text = "Storey height",
                                        Margin = new Padding(5),
                                        Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                        AutoSize = true
                                    };
                                    resultPanel.Controls.Add(lbl_storeyHeight, 0, rowNb + 10);
                                    controls.Add(lbl_storeyHeight);

                                    if (e.initialVariant.isValid)
                                    {
                                        Label lbl_init_storeyHeight = new Label()
                                        {
                                            Text = String.Format("{0} m", e.initialVariant.buildings.ElementAt(k).storeyHeight),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_init_storeyHeight, 1, rowNb + 10);
                                        controls.Add(lbl_init_storeyHeight);
                                    }

                                    if (selectedVariant.isValid)
                                    {
                                        Label lbl_var_storeyHeight = new Label()
                                        {
                                            Text = String.Format("{0} m",
                                                selectedVariant.buildings.ElementAt(k).storeyHeight),
                                            Margin = new Padding(5),
                                            Anchor = (AnchorStyles.Left | AnchorStyles.Right),
                                            AutoSize = true
                                        };
                                        resultPanel.Controls.Add(lbl_var_storeyHeight, 2, rowNb + 10);
                                        controls.Add(lbl_var_storeyHeight);
                                    }
                                }
          
                                resultPanel.Size = resultPanel.MinimumSize;
                                resultPanel.ResumeLayout(true);
                                resultPanel.Parent.ResumeLayout(true);
                                panel3.Show();
                                Show();

                                //  GRAPHIQUES -------------------------------------------------------------------------------------------------------
                                if (e.initialVariant.isValid)
                                {
                                   
                                    DataPoint point1_initialSol = new DataPoint(e.initialVariant.activeSolarEnergy, new double[] { e.initialVariant.daylightAutonomy, e.initialVariant.daylightAutonomy - CST.RESULT_ERROR_daylight, e.initialVariant.daylightAutonomy + CST.RESULT_ERROR_daylight });
                                    point1_initialSol.LabelForeColor = CST.DESCRIPTION_COLOR_WHITE;
                                    point1_initialSol.Label = "Initial variant";
                                    point1_initialSol.MarkerColor = CST.DESCRIPTION_COLOR_WHITE;
                                    series_daylightAutonomy_ActiveSolarEnergy.Points.Add(point1_initialSol);

                                    DataPoint point2_initialSol = new DataPoint(e.initialVariant.daylightAutonomy, new double[] { e.initialVariant.energyNeed, e.initialVariant.energyNeed - CST.RESULT_ERROR, e.initialVariant.energyNeed + CST.RESULT_ERROR });
                                    point2_initialSol.LabelForeColor = CST.DESCRIPTION_COLOR_WHITE;
                                    point2_initialSol.Label = "Initial variant";
                                    point2_initialSol.MarkerColor = CST.DESCRIPTION_COLOR_WHITE;
                                    series_daylightAutonomy_EnergyNeed.Points.Add(point2_initialSol);

                                    DataPoint point3_initialSol = new DataPoint(e.initialVariant.activeSolarEnergy, new double[] { e.initialVariant.energyNeed, e.initialVariant.energyNeed - CST.RESULT_ERROR, e.initialVariant.energyNeed + CST.RESULT_ERROR });
                                    point3_initialSol.LabelForeColor = CST.DESCRIPTION_COLOR_WHITE;
                                    point3_initialSol.Label = "Initial variant";
                                    point3_initialSol.MarkerColor = CST.DESCRIPTION_COLOR_WHITE;
                                    series_ActiveSolarEnergy_EnergyNeed.Points.Add(point3_initialSol);
                                }

                                DataPoint selectedSolutionPoint1 = null;
                                DataPoint selectedsolutionPoint2 = null;
                                DataPoint selectedSolutionPoint3 = null;

                                foreach (Variant variant in e.totalVariants.ElementAt(e.selectedVariant.generation))
                                {
                                    if (variant.isValid)
                                    {
                                        // Point coloré
                                        if (variant.solutionNumber == e.selectedVariant.variant)
                                        {
                                            selectedSolutionPoint1 = new DataPoint(variant.activeSolarEnergy, new double[] { variant.daylightAutonomy, variant.daylightAutonomy - CST.RESULT_ERROR_daylight, variant.daylightAutonomy + CST.RESULT_ERROR_daylight });
                                            selectedSolutionPoint1.LabelForeColor = Color.Black;
                                            selectedSolutionPoint1.LabelForeColor = Color.Black;
                                            selectedSolutionPoint1.Label = i.ToString();
                                            selectedSolutionPoint1.MarkerColor = CST.SOLUTION_COLOR_WHITE;
                                            selectedSolutionPoint1.MarkerBorderColor = Color.Black;

                                            selectedsolutionPoint2 = new DataPoint(variant.daylightAutonomy, new double[] { variant.energyNeed, variant.energyNeed - CST.RESULT_ERROR, variant.energyNeed + CST.RESULT_ERROR });
                                            selectedsolutionPoint2.LabelForeColor = Color.Black;
                                            selectedsolutionPoint2.Label = i.ToString();
                                            selectedsolutionPoint2.MarkerColor = CST.SOLUTION_COLOR_WHITE;
                                            selectedsolutionPoint2.MarkerBorderColor = Color.Black;

                                            selectedSolutionPoint3 = new DataPoint(variant.activeSolarEnergy, new double[] { variant.energyNeed, variant.energyNeed - CST.RESULT_ERROR, variant.energyNeed + CST.RESULT_ERROR });
                                            selectedSolutionPoint3.LabelForeColor = Color.Black;
                                            selectedSolutionPoint3.Label = i.ToString();
                                            selectedSolutionPoint3.MarkerColor = CST.SOLUTION_COLOR_WHITE;
                                            selectedSolutionPoint3.MarkerBorderColor = Color.Black;

                                            series_daylightAutonomy_ActiveSolarEnergy.Points.Add(selectedSolutionPoint1);
                                            series_daylightAutonomy_EnergyNeed.Points.Add(selectedsolutionPoint2);
                                            series_ActiveSolarEnergy_EnergyNeed.Points.Add(selectedSolutionPoint3);
                                        }
                                        // Point pas coloré
                                        else
                                        {
                                            DataPoint point1 = new DataPoint(variant.activeSolarEnergy, new double[] { variant.daylightAutonomy, variant.daylightAutonomy - CST.RESULT_ERROR_daylight, variant.daylightAutonomy + CST.RESULT_ERROR_daylight });
                                            point1.Label = i.ToString();
                                            series_daylightAutonomy_ActiveSolarEnergy.Points.Add(point1);

                                            DataPoint point2 = new DataPoint(variant.daylightAutonomy, new double[] { variant.energyNeed, variant.energyNeed - CST.RESULT_ERROR, variant.energyNeed + CST.RESULT_ERROR });
                                            point2.Label = i.ToString();
                                            series_daylightAutonomy_EnergyNeed.Points.Add(point2);

                                            DataPoint point3 = new DataPoint(variant.activeSolarEnergy, new double[] { variant.energyNeed, variant.energyNeed - CST.RESULT_ERROR, variant.energyNeed + CST.RESULT_ERROR });
                                            point3.Label = i.ToString();
                                            series_ActiveSolarEnergy_EnergyNeed.Points.Add(point3);
                                        }
                                    }                                  
                                    ++i;
                                }

                                chart1.DataBind();
                                chart2.DataBind();
                                chart3.DataBind();
                            }
                        }
                    } 
                }
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
