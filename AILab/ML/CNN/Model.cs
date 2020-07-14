using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN;
using Function = AI.ML.ANN.Function; 
using AI.ML.CNN;
using AI.ML.CNN.Layers;
using GrayImage = AI.ML.CNN.Images.Gray;
using ColorImage = AI.ML.CNN.Images.Color;



namespace AI.ML.CNN
{
    [Serializable]
	public class Model
    {
        protected Unit[] unit;
        
        public Model() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public Model Configure(string configuration)
        {
            string c = Global.Parser.RemoveWhiteSpaces(configuration);

            string[] cfg = Global.Parser.Split(c, "]");
            string[] lyr, a, b = null;

            unit = new Unit[cfg.Length];

            for (int i = 0; i < cfg.Length; i++)
            {
                lyr = Global.Parser.Split(cfg[i], "[");

                switch(lyr[0])
                {
                    case "conn":
                        unit[i] = new Connected();
                        ((Layer)unit[i]).Input = unit[i - 1].Output;
                        unit[i].Configure(lyr[1]);
                        break;
                    case "conv":
                        unit[i] = new Convolution();
                        ((Layer)unit[i]).Input = unit[i - 1].Output;
                        unit[i].Configure(lyr[1]);
                        break;
                    case "conc":
                        unit[i] = new Concatenation();
                        ((Layer)unit[i]).Input = unit[i - 1].Output;
                        unit[i].Configure(lyr[1]);
                        break;
                    case "imag":
                        a = Global.Parser.Split(lyr[1], ";");
                        c = Global.Parser.Extract<string>(a, new string[] { "image" }, Global.Parser.Option.None, out b);
                        switch (c)
                        {
                            case "color":
                            case "Color":
                                unit[i] = new Images.Color();
                                break;
                            case "gray":
                            case "Gray":
                                unit[i] = new Images.Gray();
                                break;
                            default:
                                throw new Exception("Image type must be color or gray.");
                        }
                        unit[i].Configure(Global.Parser.Build(b, 0, ";"));
                        continue;
                    case "pool":
                        unit[i] = new Pooling();
                        ((Layer)unit[i]).Input = unit[i - 1].Output;
                        unit[i].Configure(lyr[1]);
                        continue;
                }
            }

            return this;
        }

        public Layer GetLayer(int index)
        {
            return (Layer)unit[index];
        }

        public Unit Input
        {
            get { return unit.First(); }
        }

        public Unit[] Layers
        {
            get { return unit; }
        }

        /// <summary>
        /// propagates signal and error
        /// </summary>
        /// <param name="p"></param>
        public void Next(ANN.Enums.Propagate p)
        {
            switch (p)
            {
                case ANN.Enums.Propagate.Error:
                    int n = unit.Length - 1;
                    for (int i = 0; i < n; i++)
                        ((Layer)unit[n - i]).Next(p);
                    break;
                case ANN.Enums.Propagate.Signal:
                    for (int i = 1; i < unit.Length; i++)
                        ((Layer)unit[i]).Next(p);
                    break;
            }
        }

        public Layer Output
        {
            get { return (Layer)unit.Last(); }
        }

        public double[] Predict(Core.fData image)
        {
            if (Input.Output.Count == 3)
            {
                ColorImage inputlayer = (ColorImage)Input;
                inputlayer.fData = image;
            }
            else
            {
                GrayImage inputlayer = (GrayImage)Input;
                inputlayer.fData = image;
            }
            Next(ANN.Enums.Propagate.Signal);
            Foundation.Node node;
            double[] probs = new double[Output.Output[0].Rows];
            for (int l = 0; l < Output.Output[0].Rows; l++)
            {
                node = Output.Output[0].GetElement(l, 0);
                probs[l] = (double)((double?[])node.Element)[Global.Sig];
            }
            return probs;
        }

        public Unit this[int index]
        {
            get { return unit[index]; }
        }

        [Serializable]
        public abstract class Unit
        {
            protected IList<fMap> fmaps = new List<fMap>();

            public Unit()
            {

            }

            public abstract Unit Configure(string configuration);

            public IList<fMap> Output
            {
                get { return fmaps; }
            }           

            /// <summary>
            /// convert to string
            /// </summary>
            /// <returns></returns>
            public static string ToString(IList<fMap> fMaps)
            {
                string s = ""; double?[] d; object e;

                fMap f;

                for (int i = 0; i < fMaps.Count; i++)
                {
                    f = fMaps[i];
                    s += "\nMap[" + i.ToString("00") + "]...";

                    for (int j = 0; j < f.Columns; j++)
                    {
                        for (int k = 0; k < f.Rows; k++)
                        {
                            s += "\nidx[" + k.ToString("00");
                            s += ", " + j.ToString("00") + "]";

                            e = f.GetElement(k, j).Element;

                            if (e is double?[])
                            {
                                d = (double?[])e;
                                for (int n = 0; n < d.Length; n++)
                                    s += " " + (d[n] == null ? "#" : System.Math.Round(d[n].Value, 2).ToString("0.00"));
                            }
                            else
                            {
                                s += " " + (e == null ? "#" : System.Math.Round((double)e, 2).ToString("0.00"));
                            }
                        }
                    }
                }

                return s;
            }
        }
    }
}