using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN
{
    [Serializable]
	public class DataSet
    {
        private IList<double[]> input, output;

        private int inputLength, outputLength;

        public DataSet(int inputLength, int outputLength)
        {
            input = new List<double[]>();
            output = new List<double[]>();

            this.inputLength = inputLength;
            this.outputLength = outputLength;
        }

        public void Add(double[] input, double[] output)
        {
            if ((input.Length != inputLength) || (output.Length != outputLength))
                throw new Exception();
            this.input.Add(input);
            this.output.Add(output);
        }

        public int Count
        {
            get { return input.Count; }
        }

        public double[][] Data
        {
            get
            {
                double[][] data = new double[Count * 2][];

                for (int i = 0; i < Count; i++)
                {
                    data[i * 2] = input[i];
                    data[(i * 2) + 1] = output[i];
                }

                return data;
            }
        }
    }
}
