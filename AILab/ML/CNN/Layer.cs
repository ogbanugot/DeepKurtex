using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Propagate = AI.ML.ANN.Enums.Propagate;

namespace AI.ML.CNN
{
    [Serializable]
    public abstract class Layer : Model.Unit
    {
        protected IList<Filter> filters = new List<Filter>();

        public IList<Filter> Filters
        {
            get { return filters; }
        }

        public virtual IList<fMap> Input { get; set; }

        public virtual void Next(Propagate p)
        {
            switch (p)
            {
                case Propagate.Error:
                    for (int i = 0; i < Input.Count; i++)
                        Input[i].Set<double?>(ANN.Global.Err, 0.0);
                    foreach (CNN.Filter f in filters)
                        f.Next<double?>(Propagate.Error);
                    break;

                case Propagate.Signal:
                    foreach (CNN.Filter f in filters)
                        f.Next<double?>(Propagate.Signal);
                    break;
            }
        }
    }
}