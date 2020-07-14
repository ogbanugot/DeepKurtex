using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reports;
using Reports.Entries;
using Reports.Ledgers;

namespace AI.Core
{
    public abstract class Algorithm
    {
        protected int? _cntr = null;

        protected int? _intv = null;
        protected int? _logt = null;
        protected int? _mcnt = null;
        protected int? _next = null;

        protected string _outp = "";

        protected DateTime _stat;
        protected DateTime _stop;

        protected Journal<Section> _jour;

        protected IList<Data> _logs;
        protected IList<Record> _recs;
        protected IList<Section> _secs;
        protected IList<Block> _tbls;

        protected double? _terr = null;

        protected Foundation.TerminationOption? _topt = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_logt">intervals between section logs</param>
        /// <param name="_topt">termination option</param>
        /// <param name="_mcnt">termination options: [max iterations, max evaluations], [termination error]</param>
        /// <param name="_terr"></param>
        public Algorithm(int _logt, Foundation.TerminationOption _topt, int _mcnt, double? _terr)
        {
            // termination options...

            // byfault: _opt=[maxfevs]
            // byfnevs: _opt=[maxfevs]
            // byitrns: _opt=[maxitrs]
            // byerror: _opt=[maxitrs,error]

            this._logt = _logt;

            this._topt = _topt;
            this._mcnt = _mcnt;

            if ((_topt == Foundation.TerminationOption.ByTerminationError) && (_terr == null))
                throw new Exception();

            this._terr = _terr;
        }

        public Environment Environment { get; set; }

        /// <summary>
        /// initialize run
        /// </summary>
        public virtual void Initialize()
        {
            _stat = DateTime.Now;
            _outp += "\n\nAlgorithm:" + Name + "\nRun Starts:" + _stat;

            _cntr = 0;

            _intv = 0;
            _next = 1;

            _jour = new Journal<Section>();
            
            _logs = new List<Data>();
            _recs = new List<Record>();
            _secs = new List<Section>();
            _tbls = new List<Block>();
        }

        public virtual Journal<Section> Journal
        {
            get { return _jour; }
        }

        protected virtual void Log(IList<Data> logs)
        {
            Section sec = (Section)new Section()
                .Construct(_secs.Count, "");
            sec.Add(logs.ToArray());

            _secs.Add(sec);

            logs.Clear();
        }

        protected virtual void Log(IList<Record> records)
        {
            Log log = new Log(_logs.Count, "", AutoID.Default);
            log.Add(records.ToArray());

            _logs.Add(log);

            records.Clear();
        }

        public abstract string Name { get; }

        public string Output
        {
            get { return _outp; }
        }

        public abstract void Run();

        /// <summary>
        /// terminates run
        /// </summary>
        public virtual void Terminate()
        {
            // log sections
            _jour.Add(_secs);

            Section sec = (Section)new Section()
                .Construct(_jour.Entries.Count, "tables");
            sec.Add(_tbls.ToArray<Data>());
            _jour.Add(sec);

            DateTime _stop = DateTime.Now;
            _outp += "\n\nRun Stops:" + _stop + "\nTime elapsed:" + (_stop - _stat);
        }
    }
}