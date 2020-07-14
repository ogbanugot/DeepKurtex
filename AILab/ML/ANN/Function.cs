using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN
{
    public abstract class Function
    {
        protected double?[] constants = new double?[8];

        public Function() { }

        public virtual Function Configure(double?[] constants)
        {
            for (int i = 0; i < constants.Length; i++)
                this.constants[i] = constants[i];
            return this;
        }

        /// <summary>
        /// configures function
        /// </summary>
        /// <param name="configuration">configuration => "a=1.089,b=2.345,..."</param>
        /// <returns></returns>
        public virtual Function Configure(string configuration)
        {
            char[] dx = new char[] { ',' };
            char[] dy = new char[] { '=' };

            string[] a, b;

            if (configuration.CompareTo("void") != 0)
            {
                a = configuration.Split(dx, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < a.Length; i++)
                {
                    b = a[i].Split(dy, StringSplitOptions.RemoveEmptyEntries);
                    this[b[0]] = double.Parse(b[1]);
                }
            }

            return this;
        }

        public double? Field { get; set; }

        public double[] Input { get; set; }

        public abstract double? Inverse();

        public static Function MakeNew(string name)
        {
            switch (name)
            {
                case "avgp":
                case "avgpool":
                case "Avgpool":
                    return new Activation.Avgpool();

                case "logt":
                case "logistic":
                case "Logistic":
                    return new Activation.Logistic();

                case "lin":
                case "linear":
                case "Linear":
                    return new Activation.Linear();

                case "maxp":
                case "maxpool":
                case "Maxpool":
                    return new Activation.Maxpool();

                case "minp":
                case "minpool":
                case "Minpool":
                    return new Activation.Minpool();

                case "relu":
                case "ReLU":
                    return new Activation.ReLU();

                case "sign":
                case "signum":
                case "Signum":
                    return new Activation.Signum();

                case "tanh":
                case "Tanh":
                    return new Activation.Tanh();
            }

            throw new Exception();
        }

        /// <summary>
        /// forward propagation of signal
        /// </summary>
        /// <returns></returns>
        public abstract double Next();

        /// <summary>
        /// back propagation of error or derivative
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract double Next(double y);

        public double this[string name]
        {
            get
            {
                switch (name)
                {
                    case "a":
                    case "A":
                        return constants[0].Value;
                    case "b":
                    case "B":
                        return constants[1].Value;
                    case "c":
                    case "C":
                        return constants[2].Value;
                    case "d":
                    case "D":
                        return constants[3].Value;
                    case "e":
                    case "E":
                        return constants[4].Value;
                    case "f":
                    case "F":
                        return constants[5].Value;
                    case "g":
                    case "G":
                        return constants[6].Value;
                    case "h":
                    case "H":
                        return constants[7].Value;
                }

                throw new Exception();
            }
            set
            {
                switch (name)
                {
                    case "a":
                    case "A":
                        constants[0] = value;
                        break;
                    case "b":
                    case "B":
                        constants[1] = value;
                        break;
                    case "c":
                    case "C":
                        constants[2] = value;
                        break;
                    case "d":
                    case "D":
                        constants[3] = value;
                        break;
                    case "e":
                    case "E":
                        constants[4] = value;
                        break;
                    case "f":
                    case "F":
                        constants[5] = value;
                        break;
                    case "g":
                    case "G":
                        constants[6] = value;
                        break;
                    case "h":
                    case "H":
                        constants[7] = value;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
    }
}