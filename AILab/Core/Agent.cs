using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public abstract class Agent
    {
        protected IList<Actuator> actuators = new List<Actuator>();
        protected IList<Sensor> sensors = new List<Sensor>();

        private int? id = null;
        private string name = "";

        public Agent()
        {

        }

        public Agent(int? id, string name, Actuator[] actuators, Sensor[] sensors)
        {
            Construct(id, name, actuators, sensors);
        }

        public Agent Construct(int? id, string name, Actuator[] actuators, Sensor[] sensors)
        {
            this.id = id;
            this.name = name;

            this.actuators.Clear();

            for (int i = 0; i < actuators.Length; i++)
                this.actuators.Add(actuators[i]);

            this.sensors.Clear();

            for (int i = 0; i < sensors.Length; i++)
                this.sensors.Add(sensors[i]);

            return this;
        }

        public int? ID
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// takes action and returns solution
        /// </summary>
        /// <returns></returns>
        public abstract void Next();

        public void Set(int? id)
        {
            this.id = id;
        }

        public void Set(string name)
        {
            this.name = name;
        }
    }
}
