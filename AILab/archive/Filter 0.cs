using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN.Enums;

namespace AI.ML.CNN
{
    public abstract class Filter : Foundation.Node
    {
        protected int outputfieldSize = 2;
        protected IList<fMap> source = new List<fMap>();
        protected fMap target = null;

        protected int? rows = null, cols = null;

        public Filter()
            : base() { }

        public abstract Filter Configure(string configuration);

        protected abstract void ConstructTargetMap();

        public abstract void Next<T>(Propagate prop);

        public fMap Source
        {
            set { source.Add(value); }
        }

        public virtual fMap Target
        {
            get { return target; }
        }
    }
}
