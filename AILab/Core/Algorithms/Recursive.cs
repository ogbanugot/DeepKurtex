using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reports;
using Reports.Entries;
using Reports.Ledgers;

namespace AI.Core.Algorithms
{
    public abstract class Recursive : Algorithm
    {
        protected object[] problem = null;

        public Recursive(int _logt, Foundation.TerminationOption _topt, int _mcnt, double? _terr)
            : base(_logt, _topt, _mcnt, _terr) { }

        public abstract object Next(object[] problem);
        
        public virtual object Next()
        { 
            // log records
            
            Log(_recs);
            ++_cntr;
            int x = _cntr.Value / _logt.Value;
            if ((x == 0) || (x > _intv))
            {
                ++_intv;
                // log logs
                Log(_logs);
            }

            return _next;
        }

        public override void Run()
        {
            Initialize();

            Solution = Next(problem);

            Terminate();
        }

        public object Solution { get; set; }
    }
}
