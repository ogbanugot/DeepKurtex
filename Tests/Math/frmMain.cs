using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Math;
using Math.Graphs;

namespace Tests
{
    partial class frmMain : Form
    {
        partial void test_math_directedEdgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            DirectedEdge edge = new DirectedEdge();
            log += "\nEdge: " + edge.ToString();

            edge.Weight = 0.000234;
            log += "\n\nEdge with weight: " + edge.ToString();

            Node a = new Node(0);
            Node b = new Node(1);
            edge.Configure(a, b);
            log += "\n\nEdge with weight and nodes: " + edge.ToString();

            edge.Redirect();
            log += "\n\nEdge redirected: " + edge.ToString();

            richTextBox.Text = log;
        }

        partial void test_math_edgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            Edge edge = new Edge();
            log += "\nEdge: " + edge.ToString();

            edge.Weight = 0.000234;
            log += "\n\nEdge with weight: " + edge.ToString();

            Node a = new Node(0);
            Node b = new Node(1);
            edge.Configure(a, b);
            log += "\n\nEdge with weight and nodes: " + edge.ToString();

            richTextBox.Text = log;
        }

        partial void test_math_nodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            Node a = new Node(); // test constructor
            log += "\nNode " + a.ToString();
            a.ID = 0; // test ID
            Node b = new Node(1); // test constructor
            log += "\n\nNode " + a.ToString();
            log += "\nNode " + b.ToString();
            object[] c0 = { 1.19, 4.23 };
            a.Coordinate = c0;
            object[] c1 = { 2.90, 3.01 };
            b.Coordinate = c1; // test coordinate
            object[] c2 = { 1.09, 0.98 };
            Node c = new Node(2, c2); // test constructor

            Node[] n = { a, b, c };

            log += "\n";
            for (int i = 0; i < n.Length; i++)
                log += "\nNode " + n[i].ToString();

            Math.Vector x, y, z;

            // connect node 0 to node 1 with directed edge
            x = new Math.Vector((double)a.Coordinate[0], (double)a.Coordinate[1]);
            y = new Math.Vector((double)b.Coordinate[0], (double)b.Coordinate[1]);
            z = y - x;
            DirectedEdge e0 = new DirectedEdge(a, b);
            e0.Weight = z.Magnitude;
            a.Add(e0);
            b.Add(e0);
            // connect node 1 to node 2 with directed edge
            x = new Math.Vector((double)b.Coordinate[0], (double)b.Coordinate[1]);
            y = new Math.Vector((double)c.Coordinate[0], (double)c.Coordinate[1]);
            z = y - x;
            DirectedEdge e1 = new DirectedEdge(b, c);
            e1.Weight = z.Magnitude;
            b.Add(e1);
            c.Add(e1);
            // connect node 2 to node 0 with edge
            x = new Math.Vector((double)c.Coordinate[0], (double)c.Coordinate[1]);
            y = new Math.Vector((double)a.Coordinate[0], (double)a.Coordinate[1]);
            z = y - x;
            Edge e2 = (Edge)new Edge()
                .Configure(c, a);
            e2.Weight = z.Magnitude;
            c.Add(e2);
            a.Add(e2);

            log += "\n";
            for (int i = 0; i < n.Length; i++)
                log += "\nNode " + n[i].ToString();

            richTextBox.Text = log;
        }

        partial void test_math_graphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int nOfNodes = 6;

            int[][] heads = new int[nOfNodes][];
            heads[0] = new int[3] { 1, 3, 5 };
            heads[1] = new int[5] { 0, 2, 3, 4, 5 };
            heads[2] = new int[4] { 1, 2, 3, 5, };
            heads[3] = new int[2] { 0, 2 };
            heads[4] = new int[5] { 0, 1, 2, 3, 5 };
            heads[5] = new int[4] { 1, 2, 3, 4 };

            object[][] wgt = new object[nOfNodes][];
            wgt[0] = new object[3] { 1.0, 2.7, 5.7 };
            wgt[1] = new object[5] { 3.0, 1.2, 0.3, 4.1, 2.5 };
            wgt[2] = new object[4] { 1.17, 0.22, 0.31, 2.5, };
            wgt[3] = new object[2] { 0.1, 3.22 };
            wgt[4] = new object[5] { 3.02, 1.2, 2.4, 4.3, 6.5 };
            wgt[5] = new object[4] { 0.01, 0.02, 1.3, 4.5 };

            Graph<Edge, Node> gA = (Graph<Edge, Node>)new Graph<Edge, Node>()
                .Configure(nOfNodes, Math.Enums.Graph.Directed, Math.Enums.Matrix.Full, Math.Enums.ConnectOption.Asymmetric);

            int[] tails = { 0, 1, 2, 3, 4, 5 };

            gA.Fill(new Node());
            gA.Connect(new DirectedEdge(), tails, heads, wgt);
            gA.Refresh();

            string log = "\n\nGraph: gA";
            log += "\n" + gA.ToString();

            richTextBox.Text = log;
        }

        partial void test_math_graphBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            Math.Enums.ConnectOption option = Math.Enums.ConnectOption.Symmetric;
            const int nOfNodes = 4;

            IGraph graph = Builder.ConstructCompleteGraph<Edge, Graph<Edge, Node>, Node>(nOfNodes, Math.Enums.Matrix.LowerDiag, option);
            IEdge[][] edges = graph.Edges;

            for (int i = 0; i < edges.Length; i++)
            {
                for (int j = 0; j < edges[i].Length; j++)
                {
                    if (edges[i][j] == null)
                    {
                        edges[i][j].Weight = 0.0;
                        continue;
                    }

                    edges[i][j].Weight = Math.Daemon.Random.NextDouble() * 10.0;
                }
            }

            log += "\nGraph A:";
            log += graph.ToString();

            graph = Builder.ConstructCompleteGraph<Edge, Graph<Edge, Node>, Node>(nOfNodes, Math.Enums.Matrix.LowerDiag, option);
            edges = graph.Edges;

            for (int i = 0; i < edges.Length; i++)
            {
                for (int j = 0; j < edges[i].Length; j++)
                {
                    if (edges[i][j] == null)
                    {
                        edges[i][j].Weight = 0.0;
                        continue;
                    }

                    edges[i][j].Weight = Math.Daemon.Random.NextDouble() * 10.0;
                }
            }

            log += "\nGraph B:";
            log += graph.ToString();

            richTextBox.Text = log;
        }

        partial void test_math_randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = "";

            for (int i = 0; i < 5; i++)
            {
                text += "\n";
                for (int j = 0; j < 20; j++)
                    text += System.Math.Round(Daemon.Random.NextDouble(), 8).ToString() + ", ";
            }

            richTextBox.Text = text;
        }
    }
}
