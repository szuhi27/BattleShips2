using System.Collections.Generic;
using BattleShips.Customs;
using BattleShips.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleShips.Tests
{
    [TestClass]
    public class AiBehavUnitTests
    {

        [DataRow(false, new int[] { 1, 1 }, new int[] { })]
        [DataRow(false, new int[] { 2, 3 }, new int[] { 1, 1, 5, 6, 4, 3, 5, 4, 2, 4 })]
        [DataRow(true, new int[] { 4, 6 }, new int[] { 1, 1, 5, 6, 4, 3, 5, 4, 2, 4, 4, 6 })]
        [DataRow(true, new int[] { 2, 1 }, new int[] { 2, 1 })]
        [DataTestMethod]
        public void ShotMatchTest(bool exp, int[] cord, int[] cordsArr)
        {
            Coordinate coord = new(cord[0], cord[1]);
            Coordinate[] coordsArr = new Coordinate[cordsArr.Length];
            int place = 0;
            for (int i = 0; i < cordsArr.Length; i += 2)
            {
                coordsArr[place] = new Coordinate(cordsArr[i], cordsArr[i + 1]);
            }

            var aiShoot = new AiShoot();

            bool testRes = aiShoot.ShotMatch(coord, coordsArr);

            Assert.AreEqual(exp, testRes);
        }

        [DataRow(false, new int[] { 1, 1 }, new int[] { })]
        [DataRow(true, new int[] { 2, 3 }, new int[] { 1, 1, 5, 6, 4, 3, 5, 4, 2, 4 })]
        [DataRow(false, new int[] { 2, 2 }, new int[] { 1, 1, 5, 6, 4, 3, 5, 4, 2, 4, 4, 6 })]
        [DataRow(true, new int[] { 2, 1 }, new int[] { 2, 2 })]
        [DataTestMethod]
        public void HitAroundLastMissTest(bool exp, int[] prev, int[] hits)
        {
            Coordinate coord = new(prev[0], prev[1]);
            Coordinate[] coordsArr = new Coordinate[hits.Length];
            int place = 0;
            for (int i = 0; i < hits.Length; i += 2)
            {
                coordsArr[place] = new Coordinate(hits[i], hits[i + 1]);
            }

            var aiShoot = new AiShoot();

            bool testRes = aiShoot.HitAroundLastMiss(coord, coordsArr);

            Assert.AreEqual(exp, testRes);
        }

        [DataRow(new int[] { }, new int[] { })]
        [DataRow(new int[] { 1, 1 }, new int[] { 1, 1 })]
        [DataRow(new int[] { 1,2,1,3,1,4,1,5,1,6, 2,1,2,2,2,3,2,4,2,5,2,6,
                             3,1,3,2,3,3,3,4,3,5,3,6, 4,1,4,2,4,3,4,4,4,5,4,6,
                             5,1,5,2,5,3,5,4,5,5,5,6, 6,1,6,2,6,3,6,4,6,5,6,6}, new int[] { 1, 1 })]
        [DataTestMethod]
        public void RandomAttackTest(int[] prevs, int[] exp)
        {
            Coordinate expC = new(exp[0], exp[1]);
            Coordinate[] coordsArr = new Coordinate[prevs.Length / 2];
            int place = 0;
            for (int i = 0; i < prevs.Length; i += 2)
            {
                coordsArr[place] = new Coordinate(prevs[i], prevs[i + 1]);
            }

            var aiShoot = new AiShoot();

            Assert.AreNotEqual(expC, aiShoot.RandomAttack(coordsArr));
        }

        [DataRow(new int[] { 1, 1 }, new int[] { 2, 1, 1, 2 })]
        [DataRow(new int[] { 3, 3 }, new int[] { 4, 3, 2, 3, 3, 4, 3, 2 })]
        [DataRow(new int[] { 4, 6 }, new int[] { 5, 6, 3, 6, 4, 5 })]
        [DataTestMethod]
        public void FindCoordsAround(int[] inCoord, int[] exptCoords)
        {
            Coordinate input = new(inCoord[0], inCoord[1]);
            List<Coordinate> exp = new();
            for (int i = 0; i < exptCoords.Length; i += 2)
            {
                exp.Add(new Coordinate(exptCoords[i], exptCoords[i + 1]));
            }

            var aiShoot = new AiShoot();

            List<Coordinate> testList = aiShoot.CoordsAround(input);

            CollectionAssert.AreEqual(testList, exp);
        }

        [DataRow(new int[] { 1, 1 }, new int[] { 2, 2 })]
        [DataRow(new int[] { 3, 3 }, new int[] { 4, 4, 2, 2, 2, 4, 4, 2 })]
        [DataRow(new int[] { 4, 6 }, new int[] { 3, 5, 5, 5 })]
        [DataTestMethod]
        public void FindCoordsAroundExtended(int[] inCoord, int[] exptCoords)
        {
            Coordinate input = new(inCoord[0], inCoord[1]);
            List<Coordinate> exp = new();
            for (int i = 0; i < exptCoords.Length; i += 2)
            {
                exp.Add(new Coordinate(exptCoords[i], exptCoords[i + 1]));
            }

            AiShoot aiShoot = new AiShoot();

            List<Coordinate> testList = aiShoot.CoordsAroundExtended(input);

            CollectionAssert.AreEqual(exp, testList);

        }

        [DataRow(new int[] { 1, 1 }, new int[] { 1, 1 }, false, new int[] { 2, 1, 1, 2 })]
        [DataRow(new int[] { 3, 3 }, new int[] { 3, 4, 2, 2 }, false, new int[] { 4, 3, 2, 3, 3, 2 })]
        [DataRow(new int[] { 2, 2 }, new int[] { 1, 2, 2, 1, 2, 3, 3, 2 }, true, new int[] { 3, 3, 1, 1, 1, 3, 3, 1 })]
        [DataRow(new int[] { 2, 2 }, new int[] { 1, 2, 2, 1, 2, 3, 3, 2 }, false, new int[] { })]
        [DataTestMethod]
        public void FindGoodCordsTest(int[] prev, int[] prevShots, bool searchPlus, int[] exp)
        {
            Coordinate coord = new(prev[0], prev[1]);
            Coordinate[] coordsArr = new Coordinate[prevShots.Length / 2];
            int place = 0;
            for (int i = 0; i < prevShots.Length; i += 2)
            {
                coordsArr[place] = new Coordinate(prevShots[i], prevShots[i + 1]);
                place++;
            }
            List<Coordinate> expL = new();
            for (int i = 0; i < exp.Length; i += 2)
            {
                expL.Add(new Coordinate(exp[i], exp[i + 1]));
            }

            AiShoot aiShoot = new AiShoot();
            List<Coordinate> cordsAround = aiShoot.CoordsAround(coord);

            List<Coordinate> testList = aiShoot.FindGoodCords(coord, coordsArr, searchPlus, cordsAround);

            CollectionAssert.AreEqual(expL, testList);
        }

        [DataRow(new int[] { 1, 1 }, new int[] { }, new int[] { })]
        [DataRow(new int[] { 1, 1 }, new int[] { 1, 2 }, new int[] { 1, 2 })]
        [DataRow(new int[] { 3, 4 }, new int[] { 3, 3, 5, 6, 4, 3, 2, 5, 2, 2, 2, 4 }, new int[] { 2, 4, 3, 3 })]
        [DataTestMethod]
        public void FindHitCordsTest(int[] prev, int[] prevhits, int[] exp)
        {
            Coordinate coord = new(prev[0], prev[1]);
            Coordinate[] coordsArr = new Coordinate[prevhits.Length / 2];
            int place = 0;
            for (int i = 0; i < prevhits.Length; i += 2)
            {
                coordsArr[place] = new Coordinate(prevhits[i], prevhits[i + 1]);
                place++;
            }
            List<Coordinate> expL = new();
            for (int i = 0; i < exp.Length; i += 2)
            {
                expL.Add(new Coordinate(exp[i], exp[i + 1]));
            }

            AiShoot aiShoot = new AiShoot();

            CollectionAssert.AreEqual(expL, aiShoot.FindHitCoords(coord, coordsArr));
        }


    }
}
