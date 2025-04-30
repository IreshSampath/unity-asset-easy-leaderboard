using System.Collections.Generic;

namespace GAG.EasyLeaderboard
{
    [System.Serializable]
    public class Leaderboard
    {
        public List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();

        public Leaderboard() { }

        public Leaderboard(List<LeaderboardEntry> entries)
        {
            Entries = entries;
        }

        public void AddEntry(LeaderboardEntry entry)
        {
            Entries.Add(entry);
        }

        public void SortByScoreDescending()
        {
            Entries.Sort((a, b) => b.Score.CompareTo(a.Score));
        }

        public void SortByTimeAscending()
        {
            Entries.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string PlayerName;
        public int Score;
        public float Time;

        public LeaderboardEntry() { }

        public LeaderboardEntry(string playerName, int score = 0, float time = 0f)
        {
            PlayerName = playerName;
            Score = score;
            Time = time;
        }
    }
}