using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core.Collections
{
    public abstract class Tree<T> : Collection<T>
        where T : ITreeNode
    {
        public Tree() { }

        public Tree(T root)
        {
            Initialize(root);
        }

        public abstract Tree<T> GenerateChild();

        public int? GetIndex(T t)
        {
            for (int i = 0; i < c.Count; i++)
                if (t.Equals(c[i]))
                    return i;
            return null;
        }

        public Tree<T> Initialize(T root)
        {
            if (c.Count != 0)
                throw new Exception();
            c.Add(root);
            return this;
        }

        public T Root
        {
            get
            {
                if (c.Count == 0)
                    return default(T);
                return c[0];
            }
        }

        public static IList<ITreeNode> Transverse(Tree<T> tree)
        {
            IList<ITreeNode> nodes = new List<ITreeNode>();

            return TreeNode<ITreeNode>.Transverse(tree.Root, nodes);
        }

        /// <summary>
        /// inserts child nodes in treelist
        /// </summary>
        /// <param name="t">root node</param>
        protected void Update(T t)
        {
            int? j = GetIndex(t), k;

            if (j == null)
                throw new Exception();

            if (t.Child.Count == 0)
                return;

            IList<ITreeNode> c = t.Child;

            for (int i = c.Count - 1; i >= 0; i--)
            {
                k = GetIndex((T)c[i]);
                if (k != null)
                    throw new Exception();
                this.c.Insert(j.Value, (T)c[i]);
            }
        }
    }
}