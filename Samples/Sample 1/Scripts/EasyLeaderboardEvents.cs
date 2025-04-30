using System;

namespace GAG.EasyLeaderboard
{
    [System.Serializable]
    public class EasyLeaderboardEvents
    {
        //Add leaderboard entry
        public static event Action<LeaderboardEntry> OnLeaderboardEntryAdded;
        public static void RaiseOnLeaderboardEntryAdded(LeaderboardEntry leaderboardEntry) { OnLeaderboardEntryAdded?.Invoke(leaderboardEntry); }

        // requist to load leaderboard
        public static event Action OnLeaderboardLoadRequested;
        public static void RaiseOnLeaderboardLoadRequested() { OnLeaderboardLoadRequested?.Invoke(); }

        //load leaderboard
        public static event Action<Leaderboard> OnLeaderboardLoaded;
        public static void RaiseOnLeaderboardLoaded(Leaderboard leaderboard) { OnLeaderboardLoaded?.Invoke(leaderboard); }
    }
}
