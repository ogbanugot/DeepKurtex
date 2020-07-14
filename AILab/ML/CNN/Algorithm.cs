using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;

namespace AI.ML.CNN
{
    [Serializable]
	public class Algorithm : Foundation.Algorithms.Iterative
    {
        public Algorithm(int _logt, TerminationOption _topt, int _mcnt, double? _terr) 
            : base(_logt, _topt, _mcnt, _terr)
        {

        }

        public override string Name
        {
            get { return "Convolutional Neural Network"; }
        }

        public override int? Next()
        {
            // run an epoch

            return base.Next();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}