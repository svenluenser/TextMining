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



        public PlotModel ScatterPlot2d(double[,] data)
        {
            var model = new PlotModel();
            //var yAxis = new LinearAxis { MinimumPadding = 0 };
            //var xAxis = new LinearAxis{  };
            //model.Axes.Add(yAxis);
            //model.Axes.Add(xAxis);

            var series = new ScatterSeries {  MarkerSize = 2, MarkerType = MarkerType.Circle, MarkerFill= OxyColors.Orange };
            for(int i=0; i < 100; i++)
            {
                Console.WriteLine(data[i, 0] + " " + data[i, 1]);
                series.Points.Add(new ScatterPoint(data[i, 0], data[i, 1]));
                
            }
            model.Series.Add(series);
            Console.WriteLine("--------------------------------");
            var series1 = new ScatterSeries { MarkerSize = 2, MarkerType = MarkerType.Triangle, MarkerFill = OxyColors.LightSteelBlue };
            for (int i = 100; i < data.GetLength(0); i++)
            {
                Console.WriteLine(data[i, 0] + " " + data[i, 1]);
                series1.Points.Add(new ScatterPoint(data[i, 0], data[i, 1]));

            }
            model.Series.Add(series1);
            return model;
        }

        public PlotModel LineSeries(double[] data)
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

            var yAxis = new LogarithmicAxis(AxisPosition.Left) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "freq"  };
            model.Axes.Add(yAxis);
            model.Series.Add(series);

            return model;
        }

        public PlotModel ColumnSeries(List<DocTermFeatures> list)
        {
            var model = new PlotModel { Title = "Test" };
            var yAxis = new LinearAxis { MinimumPadding = 0 };
            var categoryAxis = new CategoryAxis {  MinorStep=250, MajorStep=100 };
            model.Axes.Add(yAxis);
            model.Axes.Add(categoryAxis);

            foreach(DocTermFeatures dvct in list)
            {
                 double[] d = dvct.toArray();
            
                string title="";
                if (dvct.GetType() == typeof(DocTermFeatures))
                    title = (dvct as DocTermFeatures).Title;
                var series = new ColumnSeries();
                int categoryIndex = 0;
                foreach(uint v in d )
                {
                    categoryIndex++;
                    series.Items.Add(new ColumnItem { CategoryIndex = categoryIndex, Value = v });
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
