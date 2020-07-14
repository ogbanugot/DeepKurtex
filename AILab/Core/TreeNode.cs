using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Math.Graphs;

namespace AI.Core
{
    [Serializable]
	public class TreeNode<T> : Node<T>, ITreeNode
    {
        private IList<ITreeNode> child = null;
        private int depth;
        private ITreeNode parent = null;

        public TreeNode()
            : base() { }

        public TreeNode(TreeNode<T> parent, int size)
            : base()
        {
            Configure(parent, size);
        }

        public Action Action { get; set; }

        public void Add(params ITreeNode[] child)
        {
            for (int i = 0; i < child.Length; i++)
            {
                if (child[i].Parent != this)
                    throw new Exception();

                if (this.child == null)
                    this.child = new List<ITreeNode>();

                child[i].ID = this.child.Count;
                this.child.Add(child[i]);
            }
        }

        public IList<ITreeNode> Child
        {
            get
            {
                if (child == null)
                    return null;
                return child;
            }
        }

        public virtual ITreeNode Configure(ITreeNode parent, int size)
        {
            this.parent = parent;

            depth = (parent == null ? 0 : parent.Depth + 1);

            Configure(new T[size]);

            return this;
        }

        public int Depth
        {
            get { return depth; }
        }

        public ITreeNode Parent
        {
            get { return parent; }
        }

        public double PathCost { get; set; }

        public static IList<ITreeNode> Transverse(ITreeNode node, IList<ITreeNode> nodes)
        {
            nodes.Add(node);

            if (node.Child == null)
                return nodes;

            for (int i = 0; i < node.Child.Count; i++)
                Transverse(node.Child[i], nodes);
            
            return nodes;
        }

        public override string ToString()
        {
            string s = "";
            s += "id:" + (ID == null ? "#" : ID.Value.ToString()) + " depth:" + Depth;
            return s;
        }
    }
}
