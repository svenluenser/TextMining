using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using System.IO;
using OxyPlot.Series;
using OxyPlot.Axes;
using TextMiningConsoleApp.Util;

namespace TextMiningConsoleApp.Plot
{
    class OxiPlot
    {
        private string outputDir=@".";

        public OxiPlot(string outputDir)
        {
            this.outputDir = outputDir;
        }

        public PlotModel BarSeries(uint[] data)
        {
            var model = new PlotModel
            {
                Title = "Test" 
            };

            var series = new LineSeries { Color = OxyColors.Gray};
            int i = 1;
            foreach(uint d in data)
            { 
                series.Points.Add(new DataPoint{ X = i, Y = d});
                i++;
            }

            var yAxis = new LinearAxis(AxisPosition.Left, 0) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "freq" };
            model.Axes.Add(yAxis);
            model.Series.Add(series);

            return model;
        }

        public PlotModel ColumnSeries(List<DocumentTermFrequencyVector> list)
        {
            var model = new PlotModel { Title = "Test" };
            var yAxis = new LinearAxis { MinimumPadding = 0 };
            var categoryAxis = new CategoryAxis {  MinorStep=250, MajorStep=100 };
            model.Axes.Add(yAxis);
            model.Axes.Add(categoryAxis);

            foreach(DocumentTermFrequencyVector dvct in list)
            {
                 uint[] d = dvct.toArray();
            
                var series = new ColumnSeries();
                int categoryIndex = 0;
                foreach(uint v in d )
                {
                    categoryIndex++;
                    series.Items.Add(new ColumnItem { CategoryIndex = categoryIndex, Value = v, Color = (dvct.Title.Contains("mathematic"))?OxyColors.LightSteelBlue : OxyColors.Olive });
                }

                model.Series.Add(series);
            }
            return model;
        }

     


        public void Export(PlotModel model, string name)
        {
            var filename = Path.Combine(outputDir, name + ".pdf");
            using (var stream = File.Create(filename))
            {
                var exporter = new PdfExporter { Width = 600d*72/96, Height = 400d*72/96 };
                exporter.Export(model, stream);
            }
        }
    }

    
}
