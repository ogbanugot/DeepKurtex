using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public abstract class Environment
    {
        protected IList<Agent> agent = new List<Agent>();

        public Environment()
        {

        }

        public void Add(params Agent[] agent)
        {
            for (int i = 0; i < agent.Length; i++)
                this.agent.Add(agent[i]);
        }

        public IList<Agent> Agents
        {
            get { return Agents; }
        }

        public Problem Problem { get; set; }
    }
}