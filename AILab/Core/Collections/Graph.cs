using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core.Collections
{
    public struct Edge<T>
    {
        private T c;
        private INode s, t;

        public Edge(INode s, INode t, T c)
        {
            this.s = s;
            this.t = t;
            this.c = c;
        }

        public T Cost
        {
            get { return c; }
        }

        public INode Source
        {
            get { return s; }
        }

        public INode Target
        {
            get { return t; }
        }

        public override string ToString()
        {
            string str = "";

            str += "cost=" + Cost.ToString();
            str += " src=" + s.ToString();
            str += " trg=" + t.ToString();

            return str;
        }
    }

    [Serializable]
	public class Graph
    {
        public enum TypeOfEdge
        {
            Directed,
            Undirected
        }

        protected object[][] c = null;
        protected INode[] n = null;

        protected TypeOfEdge t = TypeOfEdge.Directed;

        public Graph() { }

        private void configure(TypeOfEdge tofE, int nofN)
        {
            t = tofE;
            n = new INode[nofN];

            switch (tofE)
            {
                case TypeOfEdge.Directed:

                    c = new object[nofN][];

                    for (int i = 0; i < nofN; i++)
                        c[i] = new object[nofN];

                    break;

                case TypeOfEdge.Undirected:

                    c = new object[nofN][];

                    for (int i = 0; i < nofN; i++)
                        c[i] = new object[nofN - i];

                    break;
            }
        }

        public Graph Configure<T, U>(TypeOfEdge tofE, T[] node, params U[] cost)
            where T : INode
        {
            configure(tofE, node.Length);

            for (int i = 0; i < node.Length; i++)
            {
                node[i].ID = i;
                n[i] = node[i];
            }

            int s = 0;

            if (cost != null)
            {
                switch (tofE)
                {
                    case TypeOfEdge.Directed:
                        s = node.Length * node.Length;
                        if (cost.Length != s)
                            throw new Exception();
                        for (int n = 0, i = 0; i < node.Length; i++)
                            for (int j = 0; j < node.Length; j++)
                                c[i][j] = cost[n++];

                        break;

                    case TypeOfEdge.Undirected:
                        s = (node.Length * (node.Length + 1)) / 2;
                        if (cost.Length != s)
                            throw new Exception();
                        for (int n = 0, i = 0; i < node.Length; i++)
                            for (int j = 0; j < node.Length - i; j++)
                                c[i][j] = cost[n++];

                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// configures a graph
        /// </summary>
        /// <param name="tofE"></param>
        /// <param name="nofN"></param>
        /// <param name="node"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        public Graph Configure<T, U>(TypeOfEdge tofE, int nofN, T node, params U[] cost)
            where T : INode
        {
            configure(tofE, nofN);

            for (int i = 0; i < nofN; i++)
            {
                n[i] = (INode)node.Clone();
                n[i].ID = i;
            }

            int s = 0;

            if (cost != null)
            {
                switch (tofE)
                {
                    case TypeOfEdge.Directed:
                        s = nofN * nofN;
                        if (cost.Length != s)
                            throw new Exception();
                        for (int n = 0, i = 0; i < nofN; i++)
                            for (int j = 0; j < nofN; j++)
                                c[i][j] = cost[n++];

                        break;

                    case TypeOfEdge.Undirected:
                        s = (nofN * (nofN + 1)) / 2;
                        if (cost.Length != s)
                            throw new Exception();
                        for (int n = 0, i = 0; i < nofN; i++)
                            for (int j = 0; j < nofN - i; j++)
                                c[i][j] = cost[n++];

                        break;
                }
            }

            return this;
        }

        public object[][] Costs
        {
            get { return c; }
        }

        public U GetCost<U>(int sID, int tID)
        {
            U c = default(U);

            switch (t)
            {
                case TypeOfEdge.Directed:
                    c = (U)this.c[sID][tID];
                    break;

                case TypeOfEdge.Undirected:
                    int s = sID, t = tID;
                    if (s > t)
                    {
                        s = tID;
                        t = sID;
                    }
                    c = (U)this.c[s][t - s];
                    break;
            }

            return c;
        }

        public Edge<T> GetEdge<T>(int sID, int tID)
        {
            T c = GetCost<T>(sID, tID);

            Edge<T> e = new Edge<T>(GetNode(sID), GetNode(tID), c);

            return e;
        }

        public INode GetNode(int nID)
        {
            return n[nID];
        }

        public int[] NextCycle()
        {
            int[] c = new int[n.Length + 1];
            int[] p = NextPath(n.Length);

            for (int i = 0; i < p.Length; i++)
                c[i] = p[i];
            c[c.Length - 1] = p[0];

            return c;
        }

        public int[] NextPath(int length)
        {
            if ((length > n.Length) || (length <= 0))
                throw new Exception();

            int[] p = new int[length]; int x = 0;

            IList<int> c = new List<int>();

            for (int i = 0; i < p.Length; i++)
                c[i] = i;

            for (int i = 0; i < length; i++)
            {
                x = Math.Daemon.Random.Next(c.Count);
                p[i] = c[x];
                c.RemoveAt(x);
            }

            return p;
        }

        public IList<INode> Nodes
        {
            get { return n.ToList(); }
        }

        public void SetCost<U>(int sID, U[] costs)
        {
            if (costs.Length != n.Length)
                throw new Exception();

            switch (t)
            {
                case TypeOfEdge.Directed:
                    for (int i = 0; i < n.Length; i++)
                    {
                        if (i < sID)
                        {
                            c[i][sID - i] = costs[i];
                            continue;
                        }

                        c[sID][i - sID] = costs[i];
                    }
                    break;

                case TypeOfEdge.Undirected:
                    for (int i = 0; i < n.Length; i++)
                        c[sID][i] = costs[i];
                    break;
            }
        }
    }
}