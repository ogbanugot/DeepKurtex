using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Models
{
    [Serializable]
	public class Probabilistic
    {
        private const int CLASSES = 3;
        private const int DIMENSIONALITY = 2;
        private const int EXAMPLES = 10;

        private const double SIGMA = 0.5;

        private Dataset[] dataset = null;
        private Example[] example = null;

        public struct Dataset
        {
            private Example[] e;

            public Dataset(int size)
            {
                e = new Example[size];
            }

            public Example[] Example
            {
                get { return e; }
                set { e = value; }
            }
        }

        public struct Example
        {
            private int[] f;

            public Example(int size)
            {
                f = new int[size];
            }

            public int[] Feature
            {
                get { return f; }
                set { f = value; }
            }
        }

        public void Initialize()
        {
            dataset = new Dataset[] {

                new Dataset(EXAMPLES),
                new Dataset(EXAMPLES)
            };
        }

        /*

        typedef structure example_s {

        int feature[DIMENSIONALITY];
        } example_t;

        typedef struct data_set_s {
        example_t example[EXAMPLES];
        } data_set_t;

        data_set_t dataset[CLASSES] = {

            // Class 0
            {{{{13, 1}},
              {{11, 2}},
              {{13, 10}}}},
            // Class 1
            {{{{36, 4}},
              {{34, 5}},
              {{37, 11}}}},
            // Class 2
            {{{{24, 27}},
              {{22, 29}},
              {{22, 38}}}}
        };
        */

        public int pnn_classifier()
        {
            int c, e, d;
            double product;
            double[] output = new double[CLASSES];

            // Calculate the class sum of the example multiplied by each of
            // the feature vectors of the class

            for (c = 0; c < CLASSES; c++)
            {
                output[c] = 0.0;

                for (e = 0; e < EXAMPLES; e++)
                {
                    product = 0.0;

                    // Equation 8.13
                    for (d = 0; d < DIMENSIONALITY; d++)
                    {
                        product += (SCALE(example[d]) * SCALE(dataset[c].Example[e].Feature[d]));
                    }

                    // Equation 8.14 -- part 1
                    output[c] += System.Math.Exp((product - 1.0) / System.Math.Pow(SIGMA, 2.0));
                }

                // Equation 8.14 -- part 2
                output[c] = output[c] / EXAMPLES;
            }

            return winner_takes_all(output);
        }

        public static double SCALE(Example e)
        {
            return 0.0;
        }

        public static double SCALE(int c)
        {
            return 0.0;
        }

        private int winner_takes_all(double[] c)
        {
            return 0;
        }
    }
}
