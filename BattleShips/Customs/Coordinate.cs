using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Customs
{
    public struct Coordinate
    {
    
        public int R { get; set; }
        public int C { get; set; }


        public Coordinate(int row, int column)
        {
            this.R = row;
            this.C = column;
        }

    }
}
