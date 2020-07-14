using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public abstract class Actuator
    {
        protected Environment environment;

        public Actuator(Environment environment)
        {
            this.environment = environment;
        }

        public Environment Environment
        {
            get { return environment; }
        }

        public abstract void Next(IList<Core.Action> actions);
    }
}
