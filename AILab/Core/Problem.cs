using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public abstract class Problem
    {
        protected IList<Action> actions = new List<Action>();
        protected IList<INode> goals = new List<INode>();

        protected INode root = null;

        public Problem()
        {

        }

        /// <summary>
        /// adds a set of goals
        /// </summary>
        /// <param name="goals">goals</param>
        public virtual void Add(params INode[] goals)
        {
            this.goals.Clear();
            for (int i = 0; i < goals.Length; i++)
                this.goals.Add(goals[i]);
        }

        public Domain Domain { get; set; }

        /// <summary>
        /// list of actions available in the current state
        /// </summary>
        /// <param name="node">current state</param>
        /// <returns></returns>
        public abstract IList<Action> GetActions(INode node);

        /// <summary>
        /// transition model : returns child from action on current state
        /// </summary>
        /// <param name="state">current state</param>
        /// <param name="action">action</param>
        /// <returns></returns>
        public abstract INode GetChild(INode state, Action action);

        public abstract double GetHeuristic(INode node);

        /// <summary>
        /// returns 
        /// </summary>
        /// <param name="node">node with intial state</param>
        /// <returns></returns>
        public abstract double GetPathCost(INode node);

        internal double? GetFitness(INode child)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns the step cost
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="action"></param>
        /// <param name="nextState"></param>
        /// <returns></returns>
        public abstract double GetStepCost(INode currentState, Action action, INode nextState);

        public Collections.Graph Graph { get; set; }

        /// <summary>
        /// initializes problem
        /// </summary>
        /// <param name="root">node in initial state</param>
        public virtual void Initialize(INode root)
        {
            this.root = root;
        }

        /// <summary>
        /// node in inital state
        /// </summary>
        public INode Root
        {
            get { return root; }
        }

        /// <summary>
        /// extracts solution from node
        /// </summary>
        /// <param name="node"></param>
        /// <returns>solution</returns>
        public abstract Solution Solution(INode node);

        /// <summary>
        /// tests if state is goal
        /// </summary>
        /// <param name="node">state</param>
        /// <returns>0 if not goal, 1 if goal and null if state is unkown</returns>
        public abstract int? TestIfGoal(INode node);

        /// <summary>
        /// tests if state is goal
        /// </summary>
        /// <param name="node">state</param>
        /// <returns>0 if not goal, 1 if goal and null if state is unkown</returns>
        public abstract int? TestIfTerminal(INode node);
    }
}