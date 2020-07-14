using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDT
{
    public abstract class Algorithm : AI.Core.Algorithms.Recursive
    {
        public Algorithm(int _logt, Foundation.TerminationOption _topt, int _mcnt, double? _terr) 
            : base(_logt, _topt, _mcnt, _terr) { }
    }
}
