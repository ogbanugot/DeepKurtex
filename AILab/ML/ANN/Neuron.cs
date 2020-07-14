using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN.Activation;
using Node = Foundation.Node;

namespace AI.ML.ANN
{
    [Serializable]
    public abstract class Neuron : Node
    {
        protected Function function = null;
        protected IList<Synapse> synapse = new List<Synapse>();
        protected Node output;
        Random random = new Random();

        public Neuron() { }

        public Neuron(string congfiguration)
        {
            Configure(congfiguration);
        }

        /// <summary>
        /// configuration string
        /// </summary>
        /// <param name="configuration">activation=logistic(a=2,b=3);(output)fieldsize=2(def:2)</param>
        public virtual Neuron Configure(string configuration)
        {
            string cfg = Global.Parser.RemoveWhiteSpaces(configuration), c;
            string[] a = Global.Parser.Split(cfg, ";"), b, d;
            c = Global.Parser.Extract<string>(a, new string[] { "act", "activation" }, Global.Parser.Option.None, out b);
            d = Global.Parser.Split(c, "(", ")");

            function = Function.MakeNew(d[0]);
            function.Configure(d[1]);

            int? outputfieldsize = Global.Parser.Extract<int?>(a, new string[] { "fieldsize", "outputfieldsize" },
                Global.Parser.Option.StripDefaultToken, out b);

            Gradient = null;

            BSource = new Node(new double?[] { 1.0, 0.0 }); // insert bias
            int fsize = (outputfieldsize == null ? 2 : outputfieldsize.Value);
            output = new Node(new double?[fsize]);

            return this;
        }

        public virtual Neuron Configure<T>(double?[] funcparams, int outputfieldsize)
            where T : Function, new()
        {
            function = new T()
                .Configure(funcparams);

            Gradient = null;

            Source = new Node(new double?[] { 1.0, 0.0 }); // insert bias
            output = new Node(new double?[outputfieldsize]);

            return this;
        }

        public double? Gradient
        {
            get { return GetElement<double?>(); }
            set { SetElement(value); }
        }

        public double? Field
        {
            get { return function.Field; }
        }

        public virtual void Next(Enums.Propagate prop)
        {
            switch (prop)
            {
                case Enums.Propagate.Error:
                    propErr();
                    return;
                case Enums.Propagate.Signal:
                    propSig();
                    return;
            }
        }

        public Node Output
        {
            get { return output; }
        }

        protected abstract void propErr();

        protected virtual void propSig()
        {
            for (int i = 0; i < synapse.Count; i++)
                function.Input[i] = synapse[i].Output;

            ((double?[])output.Element)[Global.Sig] = function.Next();
        }

        public Node Source
        {
            set
            {
                synapse.Add(new Synapse(value, RandomStandard(0.0001), 0.0, 0.0, 0.0));
                function.Input = new double[synapse.Count];
            }
        }

        public Node BSource
        {

            set
            {
                synapse.Add(new Synapse(value, 0.0, 0.0, 0.0, 0.0));
                function.Input = new double[synapse.Count];
            }
        }
        public double RandomStandard(double scale)
        {
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            //double u1 = 1.0 - Math.Daemon.NextDoubleArray(1, 0, 1)[0]; //uniform(0,1] random doubles
            //double u2 = 1.0 - Math.Daemon.NextDoubleArray(1, 0, 1)[0];
            double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
            return randStdNormal * scale;
        }
        public IList<Synapse> Synapse
        {
            get { return synapse; }
        }

        public override string ToString()
        {
            string s = ""; int n = synapse.Count;
            s += "[inp:";

            double?[] d;
            
            if (n == 0)
            {
                s += "##]";
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    s += "(" + i.ToString("00") + "):";
                    d = (double?[])synapse[i].Source.Element;
                    for (int j = 0; j < d.Length; j++)
                        s += (d[j] == null ? "#" : d[j].Value.ToString("e4")) + ", ";
                    s = s.TrimEnd(',', ' ');
                    s += "; ";
                }

                s = s.TrimEnd(';', ' ');
                s += "]";
            }

            s += "[fld:" + (function.Field == null ? "#" : function.Field.Value.ToString("e8"))  + "]";

            s += "[grd:" + (Gradient == null ? "#" : Gradient.Value.ToString("e8")) + "]";

            s += "[out:";

            d = (double?[])Output.Element;
            for (int i = 0; i < d.Length; i++)
                s += (d[i] == null ? "#" : d[i].Value.ToString("e4")) + ",";
            s = s.TrimEnd(',');
            s += " ]";

            s += "\nWeights...\n";
            for (int i = 0; i < n; i++)
            {
                s += "[" + i.ToString("00") + "]";
                s += synapse[i].W.Value.ToString("e6") + ", ";
            }

            s = s.TrimEnd(',', ' ');

            s += "\nWeightChange...\n";
            for (int i = 0; i < n; i++)
            {
                s += "[" + i.ToString("00") + "]";
                s += (synapse[i].dW == null ? "#" : synapse[i].dW.Value.ToString("e6")) + ", ";
            }

            s = s.TrimEnd(',', ' ');

            return s;
        }
    }
}