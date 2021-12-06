using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Customs
{
    public class AiShoot
    {

        public Coordinate Attack(Coordinate prev, Coordinate[] hits, Coordinate[] prevShots, int misses)
        {
            Coordinate currentShot = new(0, 0);
            if ((!ShotMatch(prev, hits) && !HitAroundLastMiss(prev, hits)) || misses > 2)
            {
                currentShot = RandomAttack(prevShots);
            }
            else if (ShotMatch(prev, hits))
            {
                currentShot = RandomAroundLastHit(prev, prevShots, false);
            }
            else if (!ShotMatch(prev, hits) && HitAroundLastMiss(prev, hits))
            {
                currentShot = RandomAroundLastMiss(prev, hits, prevShots);
            }
            return currentShot;
        }

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


        //Check if there was a hit around the last miss
        public bool HitAroundLastMiss(Coordinate prev, Coordinate[] hits)
        {
            bool hit = false;
            List<Coordinate> cordsAround = CoordsAround(prev);
            for (int i = 0; i < cordsAround.Count; i++)
            {
                if (ShotMatch(cordsAround[i], hits))
                {
                    hit = true;
                    break;
                }
            }
            return hit;
        }

        public Coordinate RandomAttack(Coordinate[] previous)
        {
            Coordinate curr = new();
            Random random = new Random();
            do
            {
                curr.R = random.Next(1, 7);
                curr.C = random.Next(1, 7);
            } while (ShotMatch(curr, previous));
            return curr;
        }

        public List<Coordinate> CoordsAround(Coordinate prev)
        {
            List<Coordinate> cordsAround = new();
            if (prev.R < 6) { cordsAround.Add(new Coordinate(prev.R + 1, prev.C)); }
            if (prev.R > 1) { cordsAround.Add(new Coordinate(prev.R - 1, prev.C)); }
            if (prev.C < 6) { cordsAround.Add(new Coordinate(prev.R, prev.C + 1)); }
            if (prev.C > 1) { cordsAround.Add(new Coordinate(prev.R, prev.C - 1)); }
            return cordsAround;
        }

        public List<Coordinate> CoordsAroundExtended(Coordinate prev)
        {
            List<Coordinate> coordsAE = new();
            if (prev.R < 6 && prev.C < 6) { coordsAE.Add(new Coordinate(prev.R + 1, prev.C + 1)); }
            if (prev.R > 1 && prev.C > 1) { coordsAE.Add(new Coordinate(prev.R - 1, prev.C - 1)); }
            if (prev.C < 6 && prev.R > 1) { coordsAE.Add(new Coordinate(prev.R - 1, prev.C + 1)); }
            if (prev.C > 1 && prev.R < 6) { coordsAE.Add(new Coordinate(prev.R + 1, prev.C - 1)); }
            return coordsAE;
        }

        //TARGETED ATTACK ON HIT
        private Coordinate RandomAroundLastHit(Coordinate prev, Coordinate[] prevShots, bool searchPrevHit)
        {
            List<Coordinate> coordsAround = CoordsAround(prev);
            List<Coordinate> goodCords = FindGoodCords(prev, prevShots, searchPrevHit, coordsAround);
            return TargetedShot(goodCords, prevShots);
        }

        public List<Coordinate> FindGoodCords(Coordinate prev, Coordinate[] prevShots, bool searchPrevHit, List<Coordinate> coordsAround)
        {
            List<Coordinate> goodCords = new List<Coordinate>();
            for (int j = 0; j < coordsAround.Count; j++)
            {
                if (!ShotMatch(coordsAround[j], prevShots))
                {
                    goodCords.Add(coordsAround[j]);

                }
            }

            if (goodCords.Count == 0 && searchPrevHit)
            {
                List<Coordinate> coordsAroundExtended = CoordsAroundExtended(prev);
                goodCords = FindGoodCords(prev, prevShots, false, coordsAroundExtended);
            }
            return goodCords;
        }


        private Coordinate TargetedShot(List<Coordinate> goodCoords, Coordinate[] prevShots)
        {
            Coordinate coordinates = new();
            Random random = new Random();
            if (goodCoords.Count > 0)
            {
                coordinates = goodCoords[random.Next(0, goodCoords.Count)];
            }
            else
            {
                coordinates = RandomAttack(prevShots);
            }
            return coordinates;
        }

        //TARGETED ATTACK ON MISS
        private Coordinate RandomAroundLastMiss(Coordinate prev, Coordinate[] hits, Coordinate[] prevShots)
        {
            List<Coordinate> goodCords = FindHitCoords(prev, hits);
            Random random = new Random();
            Coordinate randomHit = goodCords[random.Next(0, goodCords.Count)];
            return RandomAroundLastHit(randomHit, prevShots, true);
        }

        public List<Coordinate> FindHitCoords(Coordinate prev, Coordinate[] prevHits)
        {
            List<Coordinate> goodCords = new List<Coordinate>();
            List<Coordinate> coordsAround = CoordsAround(prev);
            for (int j = 0; j < coordsAround.Count; j++)
            {
                if (ShotMatch(coordsAround[j], prevHits))
                {
                    goodCords.Add(coordsAround[j]);

                }
            }
            return goodCords;
        }

    }
}
