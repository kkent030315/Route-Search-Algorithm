using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 経路探査アルゴリズム
{
    class Nodes
    {
        public int cost { get; set; }
        public int huristick_cost { get; set; }
        public int score { get; set; }

        public Nodes(int icost, int ihuristick_cost)
        {
            cost = icost;
            huristick_cost = ihuristick_cost;
            score = cost + huristick_cost;
        }
    }
}
