using System;
using System.Windows.Forms;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace UrbanSolvePlugin
{

    public enum Orientatation
    {
        orient0,
        orient90,
        orient180,
        orient270,
        orientCust
    };

    public partial class buildingForm : Form
    {

        public event EventHandler<AddBuildingEventArg> AddBuilding;
        public event EventHandler<DeleteBuildingsEventArg> DeleteBuildings;

        public buildingForm()
        {
            InitializeComponent();

            ComboboxItem residential = new ComboboxItem("Residential", Program.residential);
            ComboboxItem office = new ComboboxItem("Office", Program.office);
            comboBox_function.Items.Add(residential);
            comboBox_function.Items.Add(office);
            comboBox_function.SelectedIndex = -1;
            comboBox_function.SelectedIndex = 0;

            ComboboxItemGeneric geneva = new ComboboxItemGeneric("Geneva", 0);
            comboBox_climate.Items.Add(geneva);
            comboBox_climate.SelectedItem = geneva;
            comboBox_climate.Enabled = false;

            ComboboxItemGeneric sia380 = new ComboboxItemGeneric("SIA 2024, 380, 180", 0);
            comboBox_compliance.Items.Add(sia380);
            comboBox_compliance.SelectedItem = sia380;
            comboBox_compliance.Enabled = false;

            rdBtn_courtyard.Checked = false;
            rdBtn_lShaped.Checked = true;
            rdBtn_simpleVolume.Checked = true;
            rdButton_rot0.Checked = true;
            rdBtn_center.Checked = false;
            rdBtn_border.Checked = true;
            numUpDown_storeyHeight.Enabled = false;

            setMinMaxValues();

            Paint += new PaintEventHandler(deselectCombobox);
            Resize += new EventHandler(deselectCombobox);
        }

        private void deselectCombobox(object sender, EventArgs e)
        {
            comboBox_function.SelectionLength = 0;
            comboBox_climate.SelectionLength = 0;
            comboBox_compliance.SelectionLength = 0;
        }

        public void refreshForm(object sender, RefreshBuildingFormEventArg e)
        {
            if (e.numberOfBuildings == 0)
            {
                btn_deleteAll.Enabled = false;
                btn_deleteOne.Enabled = false;
                numUpDown_deleteOne.Enabled = false;
                numUpDown_deleteOne.Minimum = 0;
                numUpDown_deleteOne.Maximum = 0;
                numUpDown_deleteOne.Value = numUpDown_deleteOne.Minimum;
            }
            else
            {
                btn_deleteAll.Enabled = true;
                btn_deleteOne.Enabled = true;
                numUpDown_deleteOne.Enabled = true;
                numUpDown_deleteOne.Minimum = 1;
                numUpDown_deleteOne.Maximum = e.numberOfBuildings;
                numUpDown_deleteOne.Value = numUpDown_deleteOne.Maximum;
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_addBuilding_Click(object sender, EventArgs e)
        {
            addBuilding();
        }

        private void setMinMaxValues()
        {
            numUpDown_deleteOne.Minimum = 0;
            numUpDown_deleteOne.Maximum = 0;

            numUpDown_rotation.Minimum = 0;
            numUpDown_rotation.Maximum = 360;

            numUpDown_storeyHeight.Minimum = (decimal)CST.MIN_STOREY_HEIGHT;
            numUpDown_storeyHeight.Maximum = (decimal)CST.MAX_STOREY_HEIGHT;
            numUpDown_storeyHeight.Value = (decimal)CST.STOREY_HEIGHT;

            numUpDown_length.Minimum = (decimal)CST.MIN_LENGTH_WIDTH_SIMPLE;
            numUpDown_length.Maximum = (decimal)CST.MAX_LENGTH_WIDTH_SIMPLE;
            numUpDown_length.Value = (decimal)CST.INITIAL_LENGTH_WIDTH_SIMPLE;
            numUpDown_minLength.Minimum = numUpDown_length.Minimum;
            numUpDown_minLength.Maximum = numUpDown_length.Maximum;
            numUpDown_maxLength.Minimum = numUpDown_length.Minimum;
            numUpDown_maxLength.Maximum = numUpDown_length.Maximum;

            numUpDown_width.Minimum = (decimal)CST.MIN_LENGTH_WIDTH_SIMPLE;
            numUpDown_width.Maximum = (decimal)CST.MAX_LENGTH_WIDTH_SIMPLE;
            numUpDown_width.Value = (decimal)CST.INITIAL_LENGTH_WIDTH_SIMPLE;
            numUpDown_minWidth.Minimum = numUpDown_width.Minimum;
            numUpDown_minWidth.Maximum = numUpDown_width.Maximum;
            numUpDown_maxWidth.Minimum = numUpDown_width.Minimum;
            numUpDown_maxWidth.Maximum = numUpDown_width.Maximum;

            numUpDown_depth.Minimum = (decimal)CST.MIN_DEPTH;
            numUpDown_depth.Maximum = (decimal)CST.MAX_DEPTH;
            numUpDown_depth.Value = (decimal)CST.INITIAL_DEPTH;
            numUpDown_minDepth.Minimum = numUpDown_depth.Minimum;
            numUpDown_minDepth.Maximum = numUpDown_depth.Maximum;
            numUpDown_maxDepth.Minimum = numUpDown_depth.Minimum;
            numUpDown_maxDepth.Maximum = numUpDown_depth.Maximum;

            numUpDown_storeyNumber.Minimum = CST.MIN_STOREY_NB;
            numUpDown_storeyNumber.Maximum = CST.MAX_STOREY_NB;
            numUpDown_storeyNumber.Value = CST.INITIAL_STOREY_NB;
            numUpDown_minStoreyNumber.Minimum = numUpDown_storeyNumber.Minimum;
            numUpDown_minStoreyNumber.Maximum = numUpDown_storeyNumber.Maximum;
            numUpDown_maxStoreyNumber.Minimum = numUpDown_storeyNumber.Minimum;
            numUpDown_maxStoreyNumber.Maximum = numUpDown_storeyNumber.Maximum;
        }

        protected virtual void OnAddBuilding(AddBuildingEventArg e)
        {
            EventHandler<AddBuildingEventArg> handler = AddBuilding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnDeleteBuildings(DeleteBuildingsEventArg e)
        {
            EventHandler<DeleteBuildingsEventArg> handler = DeleteBuildings;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void numUpDown_minLength_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_maxLength.Value < numUpDown_minLength.Value)
            {
                numUpDown_maxLength.Value = numUpDown_minLength.Value;
            }

            if (numUpDown_minLength.Value > numUpDown_length.Value)
            {
                numUpDown_length.Value = numUpDown_minLength.Value;
            }
        }

        private void numUpDown_maxLength_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_minLength.Value > numUpDown_maxLength.Value)
            {
                numUpDown_minLength.Value = numUpDown_maxLength.Value;
            }
            
            if (numUpDown_maxLength.Value < numUpDown_length.Value)
            {
                numUpDown_length.Value = numUpDown_maxLength.Value;
            }
        }

        private void numUpDown_minWidth_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_maxWidth.Value < numUpDown_minWidth.Value)
            {
                numUpDown_maxWidth.Value = numUpDown_minWidth.Value;
            }

            if (numUpDown_minWidth.Value > numUpDown_width.Value)
            {
                numUpDown_width.Value = numUpDown_minWidth.Value;
            }
        }

        private void numUpDown_maxWidth_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_minWidth.Value > numUpDown_maxWidth.Value)
            {
                numUpDown_minWidth.Value = numUpDown_maxWidth.Value;
            }

            if (numUpDown_maxWidth.Value < numUpDown_width.Value)
            {
                numUpDown_width.Value = numUpDown_maxWidth.Value;
            }
        }

        private void numUpDown_minDepth_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_maxDepth.Value < numUpDown_minDepth.Value)
            {
                numUpDown_maxDepth.Value = numUpDown_minDepth.Value;
            }

            if (numUpDown_minDepth.Value > numUpDown_depth.Value)
            {
                numUpDown_depth.Value = numUpDown_minDepth.Value;
            }
        }

        private void numUpDown_maxDepth_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_minDepth.Value > numUpDown_maxDepth.Value)
            {
                numUpDown_minDepth.Value = numUpDown_maxDepth.Value;
            }

            if (numUpDown_maxDepth.Value < numUpDown_depth.Value)
            {
                numUpDown_depth.Value = numUpDown_maxDepth.Value;
            }
        }

        private void numUpDown_minStoreyNumber_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_maxStoreyNumber.Value < numUpDown_minStoreyNumber.Value)
            {
                numUpDown_maxStoreyNumber.Value = numUpDown_minStoreyNumber.Value;
            }

            if (numUpDown_minStoreyNumber.Value > numUpDown_storeyNumber.Value)
            {
                numUpDown_storeyNumber.Value = numUpDown_minStoreyNumber.Value;
            }
        }

        private void numUpDown_maxStoreyNumber_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_minStoreyNumber.Value > numUpDown_maxStoreyNumber.Value)
            {
                numUpDown_minStoreyNumber.Value = numUpDown_maxStoreyNumber.Value;
            }

            if (numUpDown_maxStoreyNumber.Value < numUpDown_storeyNumber.Value)
            {
                numUpDown_storeyNumber.Value = numUpDown_maxStoreyNumber.Value;
            }
        }

        private void numUpDown_length_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_length.Value > numUpDown_maxLength.Value)
            {
                numUpDown_maxLength.Value = numUpDown_length.Value;
            } 
            else if (numUpDown_length.Value < numUpDown_minLength.Value)
            {
                numUpDown_minLength.Value = numUpDown_length.Value;
            }
        }

        private void numUpDown_width_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_width.Value > numUpDown_maxWidth.Value)
            {
                numUpDown_maxWidth.Value = numUpDown_width.Value;
            }
            else if (numUpDown_width.Value < numUpDown_minWidth.Value)
            {
                numUpDown_minWidth.Value = numUpDown_width.Value;
            }
        }

        private void numUpDown_depth_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_depth.Value > numUpDown_maxDepth.Value)
            {
                numUpDown_maxDepth.Value = numUpDown_depth.Value;
            }
            else if (numUpDown_depth.Value < numUpDown_minDepth.Value)
            {
                numUpDown_minDepth.Value = numUpDown_depth.Value;
            }
        }

        private void numUpDown_storeyNumber_ValueChanged(object sender, EventArgs e)
        {
            if (numUpDown_storeyNumber.Value > numUpDown_maxStoreyNumber.Value)
            {
                numUpDown_maxStoreyNumber.Value = numUpDown_storeyNumber.Value;
            }
            else if (numUpDown_storeyNumber.Value < numUpDown_minStoreyNumber.Value)
            {
                numUpDown_minStoreyNumber.Value = numUpDown_storeyNumber.Value;
            }
        }

        private void rdBtn_courtyard_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtn_courtyard.Checked)
            {
                rdBtn_simpleVolume.Checked = false;
                rdBtn_lShaped.Checked = false;

                if (rdButton_rot0.Checked || rdButton_rotCust.Checked)
                {
                    setAlignementPicture(Orientatation.orient0);
                }
                else if (rdButton_rot90.Checked)
                {
                    setAlignementPicture(Orientatation.orient90);
                } 
                else if (rdButton_rot180.Checked)
                {
                    setAlignementPicture(Orientatation.orient180);
                }
                else if (rdButton_rot270.Checked)
                {
                    setAlignementPicture(Orientatation.orient270);
                }

                pict_rot_0.Image = Properties.Resources.C01;
                pict_rot_90.Image = Properties.Resources.C02;
                pict_rot_180.Image = Properties.Resources.C03;
                pict_rot_270.Image = Properties.Resources.C04;
                pict_rot_cust.Image = Properties.Resources.C05;

                numUpDown_length.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_length.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
                numUpDown_maxLength.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_maxLength.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
                numUpDown_minLength.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_minLength.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;

                numUpDown_width.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_width.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
                numUpDown_minWidth.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_minWidth.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
                numUpDown_maxWidth.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_maxWidth.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
            }
            numUpDown_depth.Enabled = rdBtn_courtyard.Checked;
            numUpDown_minDepth.Enabled = rdBtn_courtyard.Checked;
            numUpDown_maxDepth.Enabled = rdBtn_courtyard.Checked;
        }

        private void rdBtn_simpleVolume_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtn_simpleVolume.Checked)
            {
                rdBtn_courtyard.Checked = false;
                rdBtn_lShaped.Checked = false;

                if (rdButton_rot0.Checked || rdButton_rotCust.Checked)
                {
                    setAlignementPicture(Orientatation.orient0);
                }
                else if (rdButton_rot90.Checked)
                {
                    setAlignementPicture(Orientatation.orient90);
                }
                else if (rdButton_rot180.Checked)
                {
                    setAlignementPicture(Orientatation.orient180);
                }
                else if (rdButton_rot270.Checked)
                {
                    setAlignementPicture(Orientatation.orient270);
                }

                pict_rot_0.Image = Properties.Resources.B01;
                pict_rot_90.Image = Properties.Resources.B02;
                pict_rot_180.Image = Properties.Resources.B03;
                pict_rot_270.Image = Properties.Resources.B04;
                pict_rot_cust.Image = Properties.Resources.B05;

                numUpDown_length.Minimum = (decimal)CST.MIN_LENGTH_WIDTH_SIMPLE;
                numUpDown_length.Maximum = (decimal)CST.MAX_LENGTH_WIDTH_SIMPLE;
                numUpDown_maxLength.Minimum = (decimal)CST.MIN_LENGTH_WIDTH_SIMPLE;
                numUpDown_maxLength.Maximum = (decimal)CST.MAX_LENGTH_WIDTH_SIMPLE;
                numUpDown_minLength.Minimum = (decimal)CST.MIN_LENGTH_WIDTH_SIMPLE;
                numUpDown_minLength.Maximum = (decimal)CST.MAX_LENGTH_WIDTH_SIMPLE;

                numUpDown_width.Minimum = (decimal)CST.MIN_LENGTH_WIDTH_SIMPLE;
                numUpDown_width.Maximum = (decimal)CST.MAX_LENGTH_WIDTH_SIMPLE;
                numUpDown_minWidth.Minimum = (decimal)CST.MIN_LENGTH_WIDTH_SIMPLE;
                numUpDown_minWidth.Maximum = (decimal)CST.MAX_LENGTH_WIDTH_SIMPLE;
                numUpDown_maxWidth.Minimum = (decimal)CST.MIN_LENGTH_WIDTH_SIMPLE;
                numUpDown_maxWidth.Maximum = (decimal)CST.MAX_LENGTH_WIDTH_SIMPLE;
            }
        }

        private void rdBtn_lShaped_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtn_lShaped.Checked)
            {
                rdBtn_courtyard.Checked = false;
                rdBtn_simpleVolume.Checked = false;

                if (rdButton_rot0.Checked || rdButton_rotCust.Checked)
                {
                    setAlignementPicture(Orientatation.orient0);
                }
                else if (rdButton_rot90.Checked)
                {
                    setAlignementPicture(Orientatation.orient90);
                }
                else if (rdButton_rot180.Checked)
                {
                    setAlignementPicture(Orientatation.orient180);
                }
                else if (rdButton_rot270.Checked)
                {
                    setAlignementPicture(Orientatation.orient270);
                }

                pict_rot_0.Image = Properties.Resources.L01;
                pict_rot_90.Image = Properties.Resources.L02;
                pict_rot_180.Image = Properties.Resources.L03;
                pict_rot_270.Image = Properties.Resources.L04;
                pict_rot_cust.Image = Properties.Resources.L05;

                numUpDown_length.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_length.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
                numUpDown_maxLength.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_maxLength.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
                numUpDown_minLength.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_minLength.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;

                numUpDown_width.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_width.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
                numUpDown_minWidth.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_minWidth.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
                numUpDown_maxWidth.Minimum = (decimal)CST.MIN_LENGTH_WIDTH;
                numUpDown_maxWidth.Maximum = (decimal)CST.MAX_LENGTH_WIDTH;
            }

            numUpDown_depth.Enabled = rdBtn_lShaped.Checked;
            numUpDown_minDepth.Enabled = rdBtn_lShaped.Checked;
            numUpDown_maxDepth.Enabled = rdBtn_lShaped.Checked;
        }

        private void rdBtn_center_CheckedChanged(object sender, EventArgs e)
        {
            rdBtn_border.Checked = !rdBtn_center.Checked;
        }

        private void rdBtn_border_CheckedChanged(object sender, EventArgs e)
        {
            rdBtn_center.Checked = !rdBtn_border.Checked;
        }

        private void btn_deleteAll_Click(object sender, EventArgs e)
        {
            DeleteBuildingsEventArg args = new DeleteBuildingsEventArg();
            args.all = true;
            args.buildingNumber = 0;
            
            OnDeleteBuildings(args);
        }

        private void btn_deleteOne_Click(object sender, EventArgs e)
        {
            DeleteBuildingsEventArg args = new DeleteBuildingsEventArg();
            args.all = false;
            args.buildingNumber = (int)numUpDown_deleteOne.Value - 1;
            OnDeleteBuildings(args);
        }

        public void addBuilding()
        {
            GetPoint gp = new GetPoint();
            gp.SetCommandPrompt("Set new building");
            gp.AcceptPoint(true);

            while (true)
            {
                // perform the get operation. This will prompt the user to input a point, but also
                // allow for command line options defined above
                GetResult get_rc = gp.Get();

                if (gp.CommandResult() != Result.Success)
                {
                    return;
                }

                if (get_rc == GetResult.Point)
                {
                    var centerPoint = gp.Point();
                    if (centerPoint == Point3d.Unset)
                    {
                        return;
                    }
                    else
                    {
                        Typology typology = Typology.square;
                        bool centered = rdBtn_center.Checked ? true : false;
                        double rotation = 0.0;

                        if (rdBtn_simpleVolume.Checked)
                        {
                            typology = Typology.square;
                        }
                        else if (rdBtn_lShaped.Checked)
                        {
                            typology = Typology.lShaped;
                        }
                        else if (rdBtn_courtyard.Checked)
                        {
                            typology = Typology.emptySquare;
                        }

                        if (rdButton_rot0.Checked)
                        {
                            rotation = 0.0;
                        }
                        else if (rdButton_rot90.Checked)
                        {
                            rotation = 90.0;
                        } 
                        else if (rdButton_rot180.Checked)
                        {
                            rotation = 180.0;
                        }
                        else if (rdButton_rot270.Checked)
                        {
                            rotation = 270.0;
                        } 
                        else if (rdButton_rotCust.Checked)
                        {
                            rotation = (double)numUpDown_rotation.Value;
                        }

                        double length = (double)numUpDown_length.Value;
                        double minLength = (double)numUpDown_minLength.Value;
                        double maxLength = (double)numUpDown_maxLength.Value;
                        double width = (double)numUpDown_width.Value;
                        double minWidth = (double)numUpDown_minWidth.Value;
                        double maxWidth = (double)numUpDown_maxWidth.Value;
                        double depth = (double)numUpDown_depth.Value;
                        double minDepth = (double)numUpDown_minDepth.Value;
                        double maxDepth = (double)numUpDown_maxDepth.Value;
                        int storeyNumber = (int)numUpDown_storeyNumber.Value;
                        int minStoreyNumber = (int)numUpDown_minStoreyNumber.Value;
                        int maxStoreyNumber = (int)numUpDown_maxStoreyNumber.Value;
                        double glazingRatio = (double)numUpDown_glazingRatio.Value;
                        double storeyHeight = (double)numUpDown_storeyHeight.Value;
                        Program program = ((ComboboxItem)comboBox_function.SelectedItem).Value;
                    
                        BuildingDescription description = null;
                        if (typology == Typology.square)
                        {
                            description = new SquareDescription(centerPoint, program, glazingRatio, minLength, maxLength, length,
                                minWidth, maxWidth, width, minStoreyNumber, maxStoreyNumber, storeyNumber,
                                storeyHeight, rotation, centered);
                        }
                        else if (typology == Typology.emptySquare)
                        {
                            description = new EmptyDescription(centerPoint, program, glazingRatio, minLength, maxLength, length,
                                minWidth, maxWidth, width, minStoreyNumber, maxStoreyNumber, storeyNumber,
                                minDepth, maxDepth, depth, storeyHeight, rotation, centered);
                        }
                        else if (typology == Typology.lShaped)
                        {
                            description = new LShapedDescription(centerPoint, program, glazingRatio, minLength, maxLength, length,
                                minWidth, maxWidth, width, minStoreyNumber, maxStoreyNumber, storeyNumber,
                                minDepth, maxDepth, depth, storeyHeight, rotation, centered);
                        }

                        if (null != description)
                        {
                            AddBuildingEventArg args = new AddBuildingEventArg();
                            args.building = description;
                            OnAddBuilding(args);
                        }
                    }
                }
                break;
            }
        }

        private void comboBox_function_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apartment
            if (comboBox_function.SelectedIndex == 0)
            {
                numUpDown_glazingRatio.Minimum = (decimal)CST.MIN_GLAZING_RATIO_APPART;
                numUpDown_glazingRatio.Maximum = (decimal)CST.MAX_GLAZING_RATIO_APPART;
            }
            // Office
            else if (comboBox_function.SelectedIndex == 1)
            {
                numUpDown_glazingRatio.Minimum = (decimal)CST.MIN_GLAZING_RATIO_OFFICE;
                numUpDown_glazingRatio.Maximum = (decimal)CST.MAX_GLAZING_RATIO_OFFICE;
            }
        }

        private void rdButton_rot180_CheckedChanged(object sender, EventArgs e)
        {
            if (rdButton_rot180.Checked)
            {
                rdButton_rot0.Checked = false;
                rdButton_rot90.Checked = false;
                rdButton_rot270.Checked = false;
                rdButton_rotCust.Checked = false;

                numUpDown_rotation.Enabled = false;

                setAlignementPicture(Orientatation.orient180);
            }
        }

        private void rdButton_rot0_CheckedChanged(object sender, EventArgs e)
        {
            if (rdButton_rot0.Checked)
            {
                rdButton_rot90.Checked = false;
                rdButton_rot180.Checked = false;
                rdButton_rot270.Checked = false;
                rdButton_rotCust.Checked = false;

                numUpDown_rotation.Enabled = false;

                setAlignementPicture(Orientatation.orient0);
            }
        }

        private void rdButton_rot90_CheckedChanged(object sender, EventArgs e)
        {
            if (rdButton_rot90.Checked)
            {
                rdButton_rot0.Checked = false;
                rdButton_rot180.Checked = false;
                rdButton_rot270.Checked = false;
                rdButton_rotCust.Checked = false;

                numUpDown_rotation.Enabled = false;

                setAlignementPicture(Orientatation.orient90);
            }
        }

        private void rdButton_rot270_CheckedChanged(object sender, EventArgs e)
        {
            if (rdButton_rot270.Checked)
            {
                rdButton_rot90.Checked = false;
                rdButton_rot180.Checked = false;
                rdButton_rot0.Checked = false;
                rdButton_rotCust.Checked = false;

                numUpDown_rotation.Enabled = false;

                setAlignementPicture(Orientatation.orient270);
            }
        }

        private void rdButton_rotCust_CheckedChanged(object sender, EventArgs e)
        {
            if (rdButton_rotCust.Checked)
            {
                rdButton_rot90.Checked = false;
                rdButton_rot180.Checked = false;
                rdButton_rot0.Checked = false;
                rdButton_rot270.Checked = false;

                numUpDown_rotation.Enabled = true;

                setAlignementPicture(Orientatation.orientCust);
            }
        }

        private void setAlignementPicture(Orientatation orientation)
        {

            switch (orientation)
            {
                case Orientatation.orient90:
                    if (rdBtn_lShaped.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.L_Border_90;
                        pictureBox_center.Image = Properties.Resources.L_Center_90;
                    }
                    else if (rdBtn_simpleVolume.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.B_Corner_90;
                        pictureBox_center.Image = Properties.Resources.C_02;
                    }
                    else if (rdBtn_courtyard.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.C_Corner_90;
                        pictureBox_center.Image = Properties.Resources.O_02;
                    }
                    break;
                case Orientatation.orient180:
                    if (rdBtn_lShaped.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.L_Border_180;
                        pictureBox_center.Image = Properties.Resources.L_Center_180;
                    }
                    else if (rdBtn_simpleVolume.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.B_Corner_180;
                        pictureBox_center.Image = Properties.Resources.C_02;
                    }
                    else if (rdBtn_courtyard.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.C_Corner_180;
                        pictureBox_center.Image = Properties.Resources.O_02;
                    }
                    break;
                case Orientatation.orient270:
                    if (rdBtn_lShaped.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.L_Border_270;
                        pictureBox_center.Image = Properties.Resources.L_Center_270;
                    }
                    else if (rdBtn_simpleVolume.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.B_Corner_270;
                        pictureBox_center.Image = Properties.Resources.C_02;
                    }
                    else if (rdBtn_courtyard.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.C_Corner_270;
                        pictureBox_center.Image = Properties.Resources.O_02;
                    }
                    break;
                default:
                    if (rdBtn_lShaped.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.L_Border_0;
                        pictureBox_center.Image = Properties.Resources.L_Center_0;
                    }
                    else if (rdBtn_simpleVolume.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.B_Corner_0;
                        pictureBox_center.Image = Properties.Resources.C_02;
                    }
                    else if (rdBtn_courtyard.Checked)
                    {
                        pictureBox_border.Image = Properties.Resources.C_Corner_0;
                        pictureBox_center.Image = Properties.Resources.O_02;
                    }
                    break;
            }
        }
    }

    public class DeleteBuildingsEventArg : EventArgs
    {
        public bool all { get; set; }
        public int buildingNumber { get; set; }
    }

    public class AddBuildingEventArg : EventArgs
    {
        public BuildingDescription building { get; set; }
    }
}