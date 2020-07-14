using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public interface INode : ICloneable
    {
        string ArrayToString();

        double? Cost { get; set; }

        //IList<Action> GetAction();

        //double? GetCost();

        //double? GetHeuristic();

        double? Fitness { get; set; }

        double? Heuristic { get; set; }

        int? ID { get; set; }

        int IsEqual(INode n);

        int Size { get; }
    }
}