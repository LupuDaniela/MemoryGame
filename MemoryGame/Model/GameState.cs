using System;
using System.Collections.Generic;

namespace MemoryGame.Model
{
    [Serializable]
    public class GameState
    {
        public string CategoryPath { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Card> Cards { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public int Moves { get; set; }
        public int TimerDurationInSeconds { get; set; }
        public string PlayerName { get; set; }
    }
}
