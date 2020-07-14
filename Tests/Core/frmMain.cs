using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

using AI.Core;
using AI.Core.Collections;
using AI.Nodes;
using AI.Search.Informed.GBF;

using Board = AI.Search.Adversarial.Games.TicTacToe.Board;
using Queue = AI.Core.Collections.Queue<AI.Core.INode>;
using Stack = AI.Core.Collections.Stack<AI.Core.INode>;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Tests
{
    partial class frmMain : Form
    {
        partial void test_search_tictactoeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 0. create environment
            AI.Search.Adversarial.Environment envr = new AI.Search.Adversarial.Environment();

            // 1. create game
            int _logt = 1; // log interval
            Foundation.TerminationOption _topt = Foundation.TerminationOption.ByIterations;
            int _maxitrs = 0;
            double? _terr = null; // termination error 
            // create game 
            AI.Search.Adversarial.Games.TicTacToe.Game game = new AI.Search.Adversarial.Games.TicTacToe.Game(_logt, _topt, _maxitrs, _terr);

            // 2. set game environment
            game.Environment = envr;

            // 3. run game
            game.Run();

            // 4. print game logs
            richTextBox.Text = game.Output;
        }

        partial void test_ai_adversarialSearch_tictactoe_boardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int size = 5;
            const int winningLength = 3;

            Board b = new Board(size);
            b.WinningLength = winningLength;

            INode n = new Node<char>(size * size);

            char[] s = new char[] {
                'O', 'O', 'X', 'O', 'O',
                'O', 'X', 'O', 'O', 'O',
                'X', 'X', 'X', 'O', '_',
                '_', 'X', 'X', '_', 'O',
                'X', 'O', 'O', 'X', '_'
                };

            ((Node<char>)n).Configure(s);
            b.State = n;

            IList<Board.CellfMap> cellmaps = b.CreateMaps('X');

            string log = "";
            for (int i = 0; i < cellmaps.Count; i++)
            {
                log += "\ncellmap[" + i.ToString("00") + "] " + cellmaps[i].ToString();
            }

            richTextBox.Text = log;
        }

        partial void test_ai_core_domainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Domain d = new Domain(new int[] { 3 }, new string[] { "blue", "green", "red" });

            string log = "";
            log += "\nDomain of colors: " + d.ToString();
            log += "\n";

            d.Tags = new string[] { "color" };
            log += "\nDomain of colors: " + d.ToString();
            log += "\n";

            Domain m = new Domain(new int[] { 3, 5 }, new object[] { "blue", "green", "red", 20, 40, 60, 80, 100 });
            m.Tags = new string[] { "color", "generation" };
            log += "\nDomain of colors and generations: " + m.ToString();
            log += "\n";

            int[] gen = m.GetElements<int>(1);

            log += "\ngenerations: [";
            for (int i = 0; i < gen.Length; i++)
                log += gen[i] + ", ";
            log = log.TrimEnd(new char[] { ' ', ',' });
            log += "]";
            log += "\n";

            object[] col = m["color"];
            log += "\ncolors: [";
            for (int i = 0; i < col.Length; i++)
                log += (string)col[i] + ", ";
            log = log.TrimEnd(new char[] { ' ', ',' });
            log += "]";


            richTextBox.Text = log;
        }

        partial void test_core_fDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = ""; const int m = 8, n = 4;

            byte[] data = new byte[m];
            Math.Daemon.Random.NextBytes(data);
            log += "\nRaw Data: ";
            for (int i = 0; i < data.Length; i++)
                log += data[i].ToString() + " ";


            byte[] labl = new byte[n];
            Math.Daemon.Random.NextBytes(labl);

            fData fdata = new fData(data, -2.0, 2.0, labl, -1.0, 1.0);

            log += "\n\nfData...\n";
            log += fdata.ToString();

            richTextBox.Text = log;
        }

        partial void test_ai_core_graphToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            const int dims = 2;
            const int nofN = 5;

            const double min = 1.0;
            const double max = 9.0;

            Node<double> n; double d = max - min;
            double[] c;
            IList<Node<double>> nodes = new List<Node<double>>();

            for (int i = 0; i < nofN; i++)
            {
                c = new double[dims];
                for (int j = 0; j < dims; j++)
                    c[j] = min + (Math.Daemon.Random.NextDouble() * d);

                n = new Node<double>();
                n.Configure(c);

                nodes.Add(n);
            }

            string log = "";

            Graph g = new Graph();

            g.Configure(Graph.TypeOfEdge.Undirected, nodes.ToArray(), new double[] { 2.0, 1.2, 2.4, 5.6, 7.1, 2.5, 1.2,
                6.7, 3.0, 1.3, 0.9, 0.03, 3.4, 12.1, 1.007 });

            Edge<double> e0 = g.GetEdge<double>(0, 4);
            Edge<double> e1 = g.GetEdge<double>(4, 0);

            log += "\n[" + e0.ToString() + "]";
            log += "\n[" + e1.ToString() + "]";

            Node<int> u = new Node<int>();
            u.Configure(new int[] { 3 });

            Graph h = new Graph();

            h.Configure(Graph.TypeOfEdge.Directed, 4, u, new double[] { 2.0, 1.2, 2.4, 5.6, 7.1, 2.5, 1.2,
                6.7, 3.0, 1.3, 0.9, 0.03, 3.4, 12.1, 1.007, 9.0 });

            Edge<double> e2 = h.GetEdge<double>(1, 3);
            Edge<double> e3 = h.GetEdge<double>(3, 1);

            log += "\n";
            log += "\n[" + e2.ToString() + "]";
            log += "\n[" + e3.ToString() + "]";

            richTextBox.Text = log;
        }

        partial void test_ai_core_nodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string test = "";

            Node<int> node = new Node<int>();
            node.ID = 0;
            node.Configure(new int[] { 5, 3, 4, 5, 8 });
            node.Fitness = 25.0;

            test += node.ToString();

            richTextBox.Text = test;
        }

        partial void test_ai_core_queueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int n = 10, s = 4;

            Queue q0 = new Queue(null);

            NPuzzle np;

            for (int i = 0; i < n; i++)
            {
                np = NPuzzle.Next(s);
                np.Fitness = NPuzzle.Evaluate(np);
                q0.Enqueue(np);
            }

            richTextBox.Text = q0.ToString();
        }

        partial void test_ai_core_stackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            Stack stack = new Stack();

            //stack.Push(new Node<int>().);
            //stack.Push(4);
            //stack.Push(5);

            log += stack.ToString();

            int tos = 0;

            stack.Pop();

            log += "\n\ntos: " + tos + "\n";

            log += stack.ToString();

            richTextBox.Text = log;
        }

        partial void test_ai_core_treeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double? x = null, y = 3;
            double?[] z = new double?[] { x, y };
            string log = "\n";
            for (int i = 0; i < z.Length; i++)
                log += z[i] == null ? "# " : z[i].Value.ToString() + " ";

            z[0] = 4; z[1] = null;
            log += "\n";
            for (int i = 0; i < z.Length; i++)
                log += z[i] == null ? "# " : z[i].Value.ToString() + " ";

            richTextBox.Text = log;
        }

        partial void test_ai_nodes_npuzzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string test = "";

            NPuzzle p = NPuzzle.Next(4);
            p.Fitness = NPuzzle.Evaluate(p);

            NPuzzle q = new NPuzzle(null, p.ToArray());
            q.Fitness = NPuzzle.Evaluate(q);
            
            // tests IsEqual method
            int n = p.IsEqual(q);
            test += "\np IsEqual to q returns " + n;
            test += "\nroot:" + p.ToString();

            IList<ITreeNode> c = NPuzzle.GenerateChild(p);

            for (int i = 0; i < c.Count; i++)
            {
                c[i].Fitness = NPuzzle.Evaluate((NPuzzle)c[i]);
                test += "\n\nchild[" + i.ToString("00") + "]:" + ((NPuzzle)c[i]).ToString();
                test += "\np IsEqual to child[" + i.ToString("00") + "]: " + p.IsEqual(c[i]);
            }

            richTextBox.Text = test;
        }

        partial void test_ai_nodes_nqboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "\nTesting NQBoard...\n";
            const int size = 8;

            NQBoard board = new NQBoard(null, size);

            board = NQBoard.Next(size);
            board.Fitness = NQBoard.Evaluate(board);
            IList<ITreeNode> lst = NQBoard.GenerateChild(board);
            for (int i = 0; i < lst.Count; i++)
                lst[i].Fitness = NQBoard.Evaluate((NQBoard)lst[i]);

            log += "\nParent...";
            log += "\n" + board.ToString();
            log += "\nChildren...";
            for (int i = 0; i < lst.Count; i++)
                log += "\n[" + i + "]:" + lst[i].ToString();

            richTextBox.Text = log;
        }

        partial void test_ai_search_bestfirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int s = 3; // size of n puzzle board
            GreedyBestFirst alg = new GreedyBestFirst();

            alg.Alpha = 1.0;
            alg.Beta = 2.0;

            alg.Initialize(NPuzzle.Next(s));

            Tree<ICanGenerateChild> t = alg.GenerateChild();

            richTextBox.Text = alg.ToString();
        }

        partial void test_ai_uByteLoaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string dataFileName = @"C:\Users\ogban\Documents\mnist\train-images.idx3-ubyte";
            //string labelFileName = @"C:\Users\ogban\Documents\mnist\train-labels.idx1-ubyte";
            //string log = "";
            //IList<fData> dataList = AI.Core.UByteLoader.ReadGrayImage(dataFileName, null, -2.0, 2.0, labelFileName, -1.0, 1.0);
            //fDataSet dataSet = new fDataSet();
            //dataSet.fData = dataList;
            //Random random = new Random();
            //int rand = 0;
            //for (int i=0; i<20; i++)
            //{
            //    rand = random.Next(0, 60000);
            //    log += dataList[rand].ToImageString();
            //    log += dataList[rand].DecodeLabel.ToString();
            //}

            string dataFileName = @"C:\Users\ogban\Documents\mnist\cifar\cifar-10-batches-bin\data_batch_1.bin";
            string[] classes = new string[] {"airplane", "automobile", "bird","cat" ,"deer","dog","frog", "horse","ship","truck"};
            string log = "";
            IList<fData> dataList = AI.Core.UByteLoader.ReadColorImage(dataFileName, 1, -2.0, 2.0, -1.0, 1.0);
            fDataSet dataSet = new fDataSet();
            dataSet.fData = dataList;
            int size = (int)System.Math.Sqrt(dataSet.fData[0].Pixel.Length / 3);
            int[] pixel = null;
            pixel = dataSet.fData[0].Pixel;
            log += classes[dataSet.fData[0].DecodeLabel];
            Mat matImg = new Mat(size, size, DepthType.Cv8U, 3);
            Image<Bgr, Byte> img = matImg.ToImage<Bgr, Byte>();
            Image<Bgr, Byte> image = setColorPixels(img, size, pixel);
            int[] input_size = new int[] { 256, 256 };
            int[] resizedPixel = Resize_Color(image, input_size);
            //Random random = new Random();
            //int rand = 0;
            //for (int i = 0; i < 20; i++)
            //{
            //    rand = random.Next(0, 60000);
            //    log += dataList[rand].ToImageString();
            //    log += dataList[rand].DecodeLabel.ToString();
            //}

            richTextBox.Text = log;
        }

        public Image<Gray, Byte> setGrayPixels(Image<Gray, Byte> img, int size, int[] pixel)
        {
            byte[,,] pix = img.Data;
            int c = 0;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    pix[i, j, 0] = (byte)pixel[c++];
                }
            img.Data = pix;
            return img;
        }
        public int[] Resize_Gray(Image<Gray, Byte> img, int[] input_size)
        {
            System.Drawing.Size size = new System.Drawing.Size();
            size.Width = input_size[0];
            size.Height = input_size[1];
            Image<Gray, Byte> img2 = new Image<Gray, Byte>(size.Width, size.Height);
            CvInvoke.Resize(img, img2, size, 0.0, 0.0, Inter.Linear);

            int[] image = new int[size.Height * size.Width];

            byte[,,] pix = new byte[size.Height, size.Width, 1];

            pix = img2.Data;
            int c = 0;
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                {
                    image[c++] = pix[i, j, 0];
                }

            return image;
        }

        public Image<Bgr, Byte> setColorPixels(Image<Bgr, Byte> img, int size, int[] pixel)
        {
            byte[,,] pix = img.Data;
            int c = 0;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    pix[i, j, 0] = (byte)pixel[c++];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    pix[i, j, 1] = (byte)pixel[c++];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    pix[i, j, 2] = (byte)pixel[c++];
                }

            img.Data = pix;
            return img;
        }
        public int[] Resize_Color(Image<Bgr, Byte> img, int[] input_size)
        {
            System.Drawing.Size size = new System.Drawing.Size();
            size.Width = input_size[0];
            size.Height = input_size[1];
            Image<Bgr, Byte> img2 = new Image<Bgr, Byte>(size.Width, size.Height);
            CvInvoke.Resize(img, img2, size, 0.0, 0.0, Inter.Linear);

            int[] image = new int[size.Height * size.Width * 3];

            byte[,,] pix = new byte[size.Height, size.Width, 3];

            pix = img2.Data;
            int c = 0;
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                {
                    image[c++] = pix[i, j, 0];
                }
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                {
                    image[c++] = pix[i, j, 1];
                }
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                {
                    image[c++] = pix[i, j, 2];
                }

            return image;
        }
    }
}