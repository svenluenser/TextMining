using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot.WindowsForms;

namespace TextMiningConsoleApp
{
    public partial class Form1 : Form
    {
        private PlotView plotview;
        public Form1(PlotModel plotmodel)
        {
            InitializeComponent();

            this.plotview = new PlotView();
            this.SuspendLayout();

            this.plotview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotview.Location = new System.Drawing.Point(0, 0);
            this.plotview.Margin = new System.Windows.Forms.Padding(0);
            this.plotview.Name = "plot1";
            this.plotview.Size = new System.Drawing.Size(632, 446);
            this.plotview.TabIndex = 0;

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.plotview);
            this.Name = "Form1";
            this.Text = "OxyPlot in Windows Forms";
            this.ResumeLayout(false);


            plotview.Model = plotmodel;

        }
    }
}
