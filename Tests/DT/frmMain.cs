using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AI.Core;
using AI.Core.Collections;
using AI.Nodes;
using AI.Search.Informed;
using Queue = AI.Core.Collections.Queue<AI.Core.INode>;
using Stack = AI.Core.Collections.Stack<AI.Core.Node<int>>;

using DT.Core;
//using DT.ID3;
using System.Data;
using System.Data.SqlClient;

namespace Tests
{
    partial class frmMain : Form
    {
        partial void test_dt_id3_algorithmToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}

//DT.ID3.Algorithm alg = new DT.ID3.Algorithm();

//SqlConnection sqlConn = null;

//// 0. open dialog
//// 1. select database for training
//// 2. create sqlConnection string - sqlConn
//// 3. train
//alg.SqlConn = sqlConn;
//            alg.GenerateChild();
//            // 4. save tree as file

//            // 0. open dialog
//            // 1. load tree
//            // 2. open dialog
//            // 3. select test database for classification
//            // 4. create sqlConnection string - sqlConn

//            // classify
//            alg.SqlConn = sqlConn;
//            alg.Classify();

//            // 5. print testing results