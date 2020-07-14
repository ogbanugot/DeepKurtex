using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    [Serializable]
	public class Solution
    {
        private IList<Action> actions;
        private double? cost = 0.0;
        private readonly INode goal;
        private Enums.Outcome outcome;

        public Solution(ITreeNode goal, IList<Action> actions, double cost, Enums.Outcome outcome)
        {
            this.goal = goal;
            this.actions = actions;
            this.cost = cost;
            this.outcome = outcome;
        }

        /// <summary>
        /// sequence of actions that lead from initial state to goal
        /// </summary>
        public IList<Action> Actions
        {
            get { return ((List<Action>)actions).AsReadOnly(); }
        }

        /// <summary>
        /// path cost from initial state to goal
        /// </summary>
        public double Cost
        {
            get { return cost.Value; }
        }

        public INode Goal
        {
            get { return goal; }
        }

        public Enums.Outcome Outcome
        {
            get { return outcome; }
        }
    }
}