using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public abstract class Sensor
    {
        protected Environment environment;

        public Sensor(Environment environment)
        {
            this.environment = environment;
        }

        public Environment Environment
        {
            get { return environment; }
        }

        public abstract Core.Percept Next();
    }
}
