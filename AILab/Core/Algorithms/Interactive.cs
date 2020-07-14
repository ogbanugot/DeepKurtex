using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Foundation;

using Reports;
using Reports.Entries;
using Reports.Ledgers;

namespace AI.Core.Algorithms
{
    public abstract class Interactive : Algorithm
    {
        protected IList<Agent> players = new List<Agent>();

        protected int? currPlayerID = null;

        public Interactive()
            : base(1, TerminationOption.ByIterations, 0, null) { }

        public Interactive(int _logt, TerminationOption _topt, int _mcnt, double? _terr)
            : base(_logt, _topt, _mcnt, _terr)
        {
            // abort if not default setting
            if ((_logt != 1) || (_topt != TerminationOption.ByIterations) || (_mcnt != 0) || (_terr != null))
                throw new Exception();
        }

        public void Add(params Agent[] players)
        {
            for (int i = 0; i < players.Length; i++)
                this.players.Add(players[i]);
        }

        public int? CurrentPlayerID
        {
            get { return currPlayerID; }
        }

        public T[] GetPlayers<T>()
            where T : Agent
        {
            T[] t = new T[players.Count];

            for (int i = 0; i < t.Length; i++)
                t[i] = (T)players[i];

            return t;
        }

        public abstract double? GetUntility(INode node, Agent player);

        /// <summary>
        /// chooses next player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public abstract void Next(object sender, EventArgs e);

        public virtual void Next()
        {
            // log records
            Log(_recs);

            int x = _cntr.Value / _logt.Value;

            if ((x == 0) || (x > _intv))
            {
                ++_intv;
                // log logs
                Log(_logs);
            }

            switch (_topt)
            {
                case TerminationOption.ByIterations:
                    if (_cntr == _mcnt)
                        Terminate();
                    break;

                default:
                    throw new Exception();
            }
        }

        public override void Run()
        {
            Initialize();

            SelectFirstPlayer();
        }

        public abstract void Start(params object[] gameConfigData);

        protected abstract void SelectFirstPlayer();

        public abstract int? TestIfTerminal(INode node);
    }
}