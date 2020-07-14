using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN.Enums;
using Foundation;

namespace AI.ML.CNN.Layers
{
    [Serializable]
	public class Concatenation : Layer
    {
        public Concatenation()
            : base() { }

        /// <summary>
        /// configures concatenation layer object
        /// </summary>
        /// <param name="configuration">outputfieldsize=2</param>
        /// <returns>concatenation layer object</returns>
        public override Model.Unit Configure(string configuration)
        {
            Filter ft = new Filter();

            for (int i = 0; i < Input.Count; i++)
                ft.Source = Input[i];
            ft.Configure(configuration);

            Output.Add(ft.Target);

            filters.Add(ft);

            return this;
        }

        public virtual Concatenation Configure(int? outputfieldsize)
        {
            Filter ft = new Filter();

            for (int i = 0; i < Input.Count; i++)
                ft.Source = Input[i];
            ft.Configure(outputfieldsize);

            Output.Add(ft.Target);

            filters.Add(ft);

            return this;
        }

        [Serializable]
	public class Filter : CNN.Filter
        {
            public Filter()
                : base() { }

            public override CNN.Filter Configure(string configuration)
            {
                string cfg = ANN.Global.Parser.RemoveWhiteSpaces(configuration);
                string[] a = ANN.Global.Parser.Split(cfg, "=");

                switch (a[0])
                {
                    case "outputfieldsize":
                        outputfieldsize = int.Parse(a[1]);
                        break;
                    case "void":
                        break;
                    default:
                        throw new Exception();
                }

                // 0. 
                int c = 0;
                for (int i = 0; i < sources.Count; i++)
                    c += sources[i].Columns * sources[i].Rows;

                // 1. set target
                target = new fMap();
                target.Configure(c, 1);

                // 2. set target elements
                for (int i = 0; i < c; i++)
                    target.SetElement(i, 0, new Node(new double?[outputfieldsize]));

                return this;
            }

            public virtual CNN.Filter Configure(int? outputfieldsize)
            {
                if (outputfieldsize != null)
                    this.outputfieldsize = outputfieldsize.Value;

                // 0. 
                int c = 0;
                for (int i = 0; i < sources.Count; i++)
                    c += sources[i].Columns * sources[i].Rows;

                // 1. set target
                target = new fMap();
                target.Configure(c, 1);

                // 2. set target elements
                for (int i = 0; i < c; i++)
                    target.SetElement(i, 0, new Node(new double?[this.outputfieldsize]));

                return this;
            }

            public override void Next<T>(Propagate prop)
            {
                switch (prop)
                {
                    case Propagate.Error:
                        for (int c = 0, i = 0; i < sources.Count; i++)
                            for (int j = 0; j < sources[i].Columns; j++)
                                for (int k = 0; k < sources[i].Rows; k++)
                                    ((double?[])sources[i].GetElement(k, j).Element)[ANN.Global.Err] =
                                        ((double?[])target.GetElement(c++, 0).Element)[ANN.Global.Err].Value;
                        return;

                    case Propagate.Signal:
                        for (int c = 0, i = 0; i < sources.Count; i++)
                            for (int j = 0; j < sources[i].Columns; j++)
                                for (int k = 0; k < sources[i].Rows; k++)
                                    ((double?[])target.GetElement(c++, 0).Element)[ANN.Global.Sig] = 
                                        ((double?[])sources[i].GetElement(k, j).Element)[ANN.Global.Sig].Value;
                        return;
                }
            }
        }
    }
}