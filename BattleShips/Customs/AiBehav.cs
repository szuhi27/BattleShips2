using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Customs
{
    
    internal class AiBehav
    {

        private ShipPlacer shipPlacerAi = new();
        private ShipPlacer shipPlacerP1 = new();
        private AiShoot aiShoot = new();

        public Coordinate[] GenerateShipsAi()
        {
            return shipPlacerAi.SetupShips();
        }

        public Coordinate[] GenerateShipsP1()
        {
            return shipPlacerP1.SetupShips();
        }

        public Coordinate Attack(Coordinate prev, Coordinate[] hits, Coordinate[] prevShots, int misses)
        {
            return aiShoot.Attack(prev,hits,prevShots,misses);
        }
        
    }
}
