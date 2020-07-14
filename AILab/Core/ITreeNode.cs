using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public interface ITreeNode : INode
    {
        Action Action { get; set; }

        void Add(params ITreeNode[] child);

        IList<ITreeNode> Child { get; }

        ITreeNode Configure(ITreeNode parent, int size);

        int Depth { get; }

        ITreeNode Parent { get; }

        double PathCost { get; set; }
    }
}