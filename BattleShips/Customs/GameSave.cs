using System;

namespace BattleShips.Customs
{
    [Serializable]
    public class GameSave
    {
        public string gameMode;
        public string player1;
        public string player2;
        public string winner;
        public int rounds;
        public int p1Hits;
        public int p2Hits;
    }
}
