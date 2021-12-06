using System;

namespace BattleShips.Customs
{
    internal class ShipPlacer
    {

        private Coordinate[] shipCords = new Coordinate[12];
        private int corrCord;

        public Coordinate[] SetupShips()
        {
            CarrierCordCalc();
            corrCord = 0;
            DestroyerCordCalc(4);
            corrCord = 0;
            DestroyerCordCalc(7);
            corrCord = 0;
            HunterCordCalc();
            return shipCords;
        }

        public bool ManualCarrier(Coordinate start, Coordinate nextC)
        {
            Coordinate[][] possibleCords = CarrPoss(start);
            return ManualCheck(possibleCords, nextC);
        }

        public Coordinate[][] ManualCarrierCords(Coordinate start)
        {
            return CarrPoss(start);
        }

        public bool ManualDestroyer(Coordinate start, Coordinate nextC)
        {
            corrCord = 0;
            Coordinate[][] possibleCords = DestrPoss(start);
            return ManualCheck(possibleCords, nextC);
        }

        public Coordinate[][] ManualDestroyerCords(Coordinate start)
        {
            return DestrPoss(start);
        }

        public bool ManualHunter(Coordinate start, Coordinate nextC)
        {
            corrCord = 0;
            Coordinate[][] possibleCords = HuntPoss(start);
            return ManualCheck(possibleCords, nextC);
        }

        public Coordinate[][] ManualHunterCords(Coordinate start)
        {
            return HuntPoss(start);
        }

        private bool ManualCheck(Coordinate[][] possibleCords, Coordinate nextC)
        {
            bool goodPlace = false;
            for (int i = 0; i < possibleCords.Length; i++)
            {
                if (goodPlace || possibleCords[i] == null)
                {
                    break;
                }
                for (int j = 0; j < possibleCords[i].Length; j++)
                {
                    if (nextC.Equals(possibleCords[i][j]))
                    {
                        goodPlace = true;
                        break;
                    }
                }
            }
            return goodPlace;
        }

        private void CarrierCordCalc()
        {
            Coordinate start = new();
            Random random = new();
            start.R = random.Next(1,7);
            start.C = random.Next(1,7);

            Coordinate[][] possibleCords = CarrPoss(start);
            int place = random.Next(0,2);
            shipCords[0] = possibleCords[place][0];
            shipCords[1] = possibleCords[place][1];
            shipCords[2] = possibleCords[place][2];
            shipCords[3] = possibleCords[place][3];
        }

        private static Coordinate[][] CarrPoss(Coordinate start)
        {
            Coordinate[][] possibleCords = new Coordinate[2][];
            int goodCord = 0;
            if (start.R - 3 >= 1)
            {
                possibleCords[goodCord] = new Coordinate[]
                    {start,new Coordinate(start.R-1,start.C),new Coordinate(start.R-2,start.C), new Coordinate(start.R-3,start.C)};
                goodCord++;
            }
            if (start.R + 3 <= 6)
            {
                possibleCords[goodCord] = new Coordinate[]
                    {start, new Coordinate(start.R+1,start.C), new Coordinate(start.R+2,start.C), new Coordinate(start.R+3,start.C)};
                goodCord++;
            }
            if (goodCord < 2 && start.C - 3 >= 1)
            {
                possibleCords[goodCord] = new Coordinate[]
                        {start, new Coordinate(start.R,start.C-1), new Coordinate(start.R,start.C-2), new Coordinate(start.R,start.C-3)};
                goodCord++;
            }
            if (goodCord < 2 && start.C + 3 <= 6)
            {
                possibleCords[goodCord] = new Coordinate[]
                    {start, new Coordinate(start.R,start.C+1), new Coordinate(start.R,start.C+2), new Coordinate(start.R,start.C+3)};
            }
            
            return possibleCords;
        }

        private void DestroyerCordCalc(int shipNumber)
        {
            Coordinate start = new();
            Random random = new();
            start.R = random.Next(1,7);
            start.C = random.Next(1,7);

            Coordinate[][] possibleCords = DestrPoss(start);

            bool anyGood = false;
            if (corrCord > 0) { anyGood = true; }

            if (anyGood)
            {
                int place = random.Next(0,corrCord);
                shipCords[shipNumber] = possibleCords[place][0];
                shipCords[shipNumber+1] = possibleCords[place][1];
                shipCords[shipNumber+2] = possibleCords[place][2];
            }
            else
            {
                DestroyerCordCalc(shipNumber);
            }
        }

        private Coordinate[][] DestrPoss(Coordinate start)
        {
            Coordinate[][] possibleCords = new Coordinate[4][];
            int goodCord = 0;
            if (start.R - 2 >= 1)
            {
                possibleCords[goodCord] = new Coordinate[] { start, new Coordinate(start.R - 1, start.C), new Coordinate(start.R - 2, start.C) };
                if (!CollisionCheck(possibleCords[goodCord]))
                {
                    goodCord++;
                }
            }
            if (start.R + 2 <= 6)
            {
                possibleCords[goodCord] = new Coordinate[] { start, new Coordinate(start.R + 1, start.C), new Coordinate(start.R + 2, start.C) };
                if (!CollisionCheck(possibleCords[goodCord]))
                {
                    goodCord++;
                }
            }
            if (start.C - 2 >= 1)
            {
                possibleCords[goodCord] = new Coordinate[] { start, new Coordinate(start.R, start.C - 1), new Coordinate(start.R, start.C - 2) };
                if (!CollisionCheck(possibleCords[goodCord]))
                {
                    goodCord++;
                }
            }
            if (start.C + 2 <= 6)
            {
                possibleCords[goodCord] = new Coordinate[] { start, new Coordinate(start.R, start.C + 1), new Coordinate(start.R, start.C + 2) };
                if (!CollisionCheck(possibleCords[goodCord]))
                {
                    goodCord++;
                }
            }
            corrCord = goodCord;
            return possibleCords;
        }

        private void HunterCordCalc()
        {
            Coordinate start = new();
            Random random = new();
            start.R = random.Next(1,7);
            start.C = random.Next(1,7);

            Coordinate[][] possibleCords = HuntPoss(start);

            bool anyGood = false;
            if (corrCord > 0) { anyGood = true; }

            if (anyGood)
            {
                int place = random.Next(0, corrCord);
                shipCords[10] = possibleCords[place][0];
                shipCords[11] = possibleCords[place][1];
            }
            else
            {
                HunterCordCalc();
            }
        }

        private Coordinate[][] HuntPoss(Coordinate start)
        {
            Coordinate[][] possibleCords = new Coordinate[4][];
            int goodCord = 0;
            if (start.R - 1 >= 1)
            {
                possibleCords[goodCord] = new Coordinate[] { start, new Coordinate(start.R - 1, start.C) };
                if (!CollisionCheck(possibleCords[goodCord]))
                {
                    goodCord++;
                }
            }
            if (start.R + 1 <= 6)
            {
                possibleCords[goodCord] = new Coordinate[] { start, new Coordinate(start.R + 1, start.C) };
                if (!CollisionCheck(possibleCords[goodCord]))
                {
                    goodCord++;
                }
            }
            if (start.C - 1 >= 1)
            {
                possibleCords[goodCord] = new Coordinate[] { start, new Coordinate(start.R, start.C - 1) };
                if (!CollisionCheck(possibleCords[goodCord]))
                {
                    goodCord++;
                }
            }
            if (start.C + 1 <= 6)
            {
                possibleCords[goodCord] = new Coordinate[] { start, new Coordinate(start.R, start.C + 1) };
                if (!CollisionCheck(possibleCords[goodCord]))
                {
                    goodCord++;
                }
            }
            corrCord = goodCord;
            return possibleCords;
        }

        private bool CollisionCheck(Coordinate[] coordinates)
        {
            bool didCollide = false;
            for (int i = 0; i < coordinates.Length; i++)
            {
                if (didCollide)
                {
                    break;
                }
                for (int j = 0; j < shipCords.Length; j++)
                {
                    if (coordinates[i].Equals(shipCords[j]))
                    {
                        didCollide = true;
                        break;
                    }
                }
            }
            return didCollide;
        }

    }
}