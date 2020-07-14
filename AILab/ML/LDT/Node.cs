using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.Core;

namespace DT.Core
{
    [Serializable]
	public class Node : AI.Core.TreeNode<double>
    {
        public Node()
            : base() { }

        public IList<string> ColumnNames { get; set; } // name of all columns to get max information gain

        public string Label { get; set; } // name of column with max information gain

        public string Name { get; set; } // name of distinct value

        public string Query { get; set; }
    }
}