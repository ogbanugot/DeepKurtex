using AI.ML.ANN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN;

namespace AI.ML.CNN.Layers
{
    [Serializable]
	public class Pooling : Layer
    {
        public Pooling()
            : base()
        {

        }

        public override Model.Unit Configure(string configuration)
        {
            Filter filter;

            for (int i = 0; i < Input.Count; i++)
            {
                filter = new Filter();
                filter.Source = Input[i];
                filter.Configure(configuration);
                // add to list of filters
                filters.Add(filter);
                // add to list of targets
                fmaps.Add(filter.Target);
            }

            return this;
        }

        public virtual Pooling Configure<T>(int kernelsize, int? kernelstride, int? kernelpadding, int? outputfieldsize)
            where T : CNN.Kernel, new()
        {
            Filter filter;

            for (int i = 0; i < Input.Count; i++)
            {
                filter = new Filter();
                filter.Source = Input[i];
                filter.Configure<T>(kernelsize, kernelstride, kernelpadding, outputfieldsize);
                // add to list of filters
                filters.Add(filter);
                // add to list of targets
                fmaps.Add(filter.Target);
            }

            return this;
        }

        [Serializable]
	public class Filter : CNN.Filter
        {
            public Filter()
                : base() { }

            public virtual Filter Configure<T>(int kernelsize, int? kernelstride, int? kernelpadding, int? outputfieldsize)
                where T : CNN.Kernel, new()
            {
                Kernel kernel = new T();
                kernel.Configure(kernelsize, kernelsize, kernelstride, kernelpadding);
                kernel.Source = sources[0];
                kernels.Add(kernel);

                this.outputfieldsize = outputfieldsize == null ? 2 : outputfieldsize.Value;

                // size = (((src.Size - kernel.Size) + (2 * kernel.Padding)) / kernel.Stride) + 1;
                int rows = ((sources[0].Rows - kernel.Rows + (2 * kernel.Padding)) / kernel.Stride) + 1;
                int cols = ((sources[0].Columns - kernel.Columns + (2 * kernel.Padding)) / kernel.Stride) + 1;
                target = new fMap();
                target.Configure<double?>(rows, cols, this.outputfieldsize);

                return this;
            }

            public override void Next<T>(Propagate prop)
            {
                switch (prop)
                {
                    case Propagate.Error:
                        for (int i = 0; i < target.Rows; i++)
                            for (int j = 0; j < target.Columns; j++)
                                kernels[0].Next(((double?[])target.GetElement(i, j).Element)[Global.Err].Value, i, j);
                        break;

                    case Propagate.Signal:
                        for (int i = 0; i < target.Rows; i++)
                            for (int j = 0; j < target.Columns; j++)
                                target.WriteAt<double?>(i, j, Global.Sig, kernels[0].Next(i, j));
                        break;
                }
            }
        }
    }
}
