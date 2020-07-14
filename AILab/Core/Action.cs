using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    [Serializable]
	public class Action
    {
        protected string script = "";
        protected object[] constraints;

        public Action(string script, params object[] constraints)
        {
            this.constraints = constraints;
            this.script = script.ToUpper();
        }

        /// <summary>
        /// constraints on action
        /// </summary>
        public object[] Constraints
        {
            get { return constraints; }
        }

        public double? Cost { get; set; }

        public virtual object Interpretation { get; }

        public string Script
        {
            get { return script; }
        }
    }
}