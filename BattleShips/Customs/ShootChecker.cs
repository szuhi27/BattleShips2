using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Customs
{
    internal class ShootChecker
    {
        public bool ShotMatch(Coordinate coordinate, Coordinate[] coordArray)
        {
            bool match = false;
            for (int i = 0; i < coordArray.Length; i++)
            {
                if (coordinate.Equals(coordArray[i]))
                {
                    match = true;
                    break;
                }
            }
            return match;
        }

        public string HitCheck(Coordinate lastHit,Coordinate[] hits, Coordinate[] ships)
        {
            string msg;
            if (CarrierCheck(hits,ships) && LastHitCheck(lastHit,ships,0,4))
            {
                msg = "Carrier";
            }else if ((DestroyerCheck(4,hits,ships) && LastHitCheck(lastHit,ships,4,7)) || 
                (DestroyerCheck(7,hits,ships) && LastHitCheck(lastHit, ships, 7, 10)))
            {
                msg = "Destroyer";
            }else if (HunterCheck(hits,ships) && LastHitCheck(lastHit,ships,10,12))
            {
                msg = "Hunter";
            }
            else
            {
                msg = "Hit";
            }
            return msg;
        }

        private bool CarrierCheck(Coordinate[] hits, Coordinate[] ships)
        {
            bool sank = false;
            if(Runner(ships[0], hits) && Runner(ships[1], hits) && Runner(ships[2], hits) && Runner(ships[3], hits))
            {
                sank = true;
            }
            return sank;
        }

        private bool DestroyerCheck(int v, Coordinate[] hits, Coordinate[] ships)
        {
            bool sank = false;
            if (Runner(ships[v], hits) && Runner(ships[v+1], hits) && Runner(ships[v+2], hits))
            {
                sank = true;
            }
            return sank;
        }

        private bool HunterCheck(Coordinate[] hits, Coordinate[] ships)
        {
            bool sank = false;
            if (Runner(ships[10], hits) && Runner(ships[11], hits))
            {
                sank = true;
            }
            return sank;
        }

        //Check if ship is destroyed
        private bool Runner(Coordinate shipC, Coordinate[] hits)
        {
            bool hit = false;
            for(int i = 0; i < hits.Length; i++)
            {
                if (shipC.Equals(hits[i]))
                {
                    hit = true;
                    break;
                }
            }
            return hit;
        }

        //Check is last hit hit the ship
        private bool LastHitCheck(Coordinate lastHit, Coordinate[] shipCords, int shipStart, int shipEnd)
        {
            bool lastHitHitShip = false;
            for (int i = shipStart; i < shipEnd; i++)
            {
                if (lastHit.Equals(shipCords[i]))
                {
                    lastHitHitShip = true;
                    break;
                }
            }
            return lastHitHitShip;
        }
    }
}
