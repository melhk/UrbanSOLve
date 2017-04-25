using System.Windows.Forms.DataVisualization.Charting;

namespace UrbanSolvePlugin
{
    partial class ResultWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultWindow));
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox_variant = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.btn_close = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox_variant.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart1.Location = new System.Drawing.Point(2, 2);
            this.chart1.Margin = new System.Windows.Forms.Padding(2);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(371, 645);
            this.chart1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 675);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(555, 0);
            this.panel1.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(8);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(946, 675);
            this.splitContainer1.SplitterDistance = 383;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 8;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(383, 675);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chart1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(375, 649);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Daylight vs Production";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chart2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(379, 649);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Need vs Daylight";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chart2
            // 
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            this.chart2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart2.Location = new System.Drawing.Point(2, 2);
            this.chart2.Margin = new System.Windows.Forms.Padding(8);
            this.chart2.Name = "chart2";
            this.chart2.Size = new System.Drawing.Size(375, 645);
            this.chart2.TabIndex = 0;
            this.chart2.Text = "chart2";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chart3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(379, 649);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Need vs Production";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chart3
            // 
            chartArea3.Name = "ChartArea1";
            this.chart3.ChartAreas.Add(chartArea3);
            this.chart3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart3.Location = new System.Drawing.Point(0, 0);
            this.chart3.Margin = new System.Windows.Forms.Padding(2);
            this.chart3.Name = "chart3";
            this.chart3.Size = new System.Drawing.Size(379, 649);
            this.chart3.TabIndex = 0;
            this.chart3.Text = "chart3";
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.AutoScrollMargin = new System.Drawing.Size(15, 15);
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panel11);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(10);
            this.panel3.Size = new System.Drawing.Size(555, 675);
            this.panel3.TabIndex = 26;
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.BackColor = System.Drawing.SystemColors.Window;
            this.panel4.Controls.Add(this.groupBox_variant);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(10, 10);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(10);
            this.panel4.Size = new System.Drawing.Size(535, 53);
            this.panel4.TabIndex = 15;
            // 
            // groupBox_variant
            // 
            this.groupBox_variant.AutoSize = true;
            this.groupBox_variant.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox_variant.BackColor = System.Drawing.SystemColors.Window;
            this.groupBox_variant.Controls.Add(this.panel2);
            this.groupBox_variant.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_variant.ForeColor = System.Drawing.Color.Black;
            this.groupBox_variant.Location = new System.Drawing.Point(10, 10);
            this.groupBox_variant.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox_variant.Name = "groupBox_variant";
            this.groupBox_variant.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox_variant.Size = new System.Drawing.Size(515, 33);
            this.groupBox_variant.TabIndex = 13;
            this.groupBox_variant.TabStop = false;
            this.groupBox_variant.Text = "Information on initial and selected variants";
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.AutoSize = true;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(10, 23);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(495, 0);
            this.panel2.TabIndex = 1;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.panel10);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel11.Location = new System.Drawing.Point(10, 608);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(535, 57);
            this.panel11.TabIndex = 10;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.btn_close);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel10.Location = new System.Drawing.Point(0, 33);
            this.panel10.Margin = new System.Windows.Forms.Padding(0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(535, 24);
            this.panel10.TabIndex = 9;
            // 
            // btn_close
            // 
            this.btn_close.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_close.Location = new System.Drawing.Point(0, 0);
            this.btn_close.Margin = new System.Windows.Forms.Padding(0);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(535, 24);
            this.btn_close.TabIndex = 8;
            this.btn_close.Text = "Close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // ResultWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 675);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ResultWindow";
            this.Text = "UrbanSOLve - Show charts";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.groupBox_variant.ResumeLayout(false);
            this.groupBox_variant.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Chart chart1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private Chart chart2;
        private Chart chart3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.GroupBox groupBox_variant;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
    }
}