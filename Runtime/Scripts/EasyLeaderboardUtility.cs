using GAG.EasyLeaderboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GAG.EasyLeaderboard
{
    public static class EasyLeaderboardUtility
    {
        public enum LeaderboardType { JSON, CSV }
        public enum EntryFormat { NameScore, NameTime, NameScoreTime }
        public enum LeaderboardDeployPlatform { PC, Android, IOS }

        public static void SaveLeaderboard(Leaderboard leaderboard, string path, LeaderboardType type, EntryFormat format, bool allowDuplicates)
        {
            switch (type)
            {
                case LeaderboardType.JSON:
                    SaveAsJSON(leaderboard, path, format, allowDuplicates);
                    break;
                case LeaderboardType.CSV:
                    SaveAsCSV(leaderboard, path, format, allowDuplicates);
                    break;
            }
        }

        public static Leaderboard LoadLeaderboard(string path, LeaderboardType type)
        {
            return type switch
            {
                LeaderboardType.JSON => LoadFromJSON(path),
                LeaderboardType.CSV => LoadFromCSV(path),
                _ => new Leaderboard()
            };
        }

        public static void DeleteLeaderboard(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Cleared leaderboard file: " + path);
            }
            else
            {
                Debug.LogWarning("Leaderboard file not found: " + path);
            }
        }

        public static void EnsureFileExists(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("Leaderboard file not found. Creating new file...");
                File.WriteAllText(path, JsonUtility.ToJson(new Leaderboard(), true));
            }
        }

        private static void SaveAsJSON(Leaderboard leaderboard, string path, EntryFormat format, bool allowDuplicates)
        {
            Leaderboard existing = File.Exists(path)
                ? JsonUtility.FromJson<Leaderboard>(File.ReadAllText(path))
                : new Leaderboard();

            MergeEntries(existing, leaderboard, format, allowDuplicates);

            File.WriteAllText(path, JsonUtility.ToJson(existing, true));
            Debug.Log("Saved JSON leaderboard to: " + path);
        }

        private static Leaderboard LoadFromJSON(string path)
        {
            if (!File.Exists(path)) return new Leaderboard();

            string json = File.ReadAllText(path);
            Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(json);
            leaderboard.SortByScoreDescending();
            return leaderboard;
        }

        private static void SaveAsCSV(Leaderboard leaderboard, string path, EntryFormat format, bool allowDuplicates)
        {
            Leaderboard existing = new Leaderboard();

            if (File.Exists(path))
            {
                existing = LoadFromCSV(path);
                MergeEntries(existing, leaderboard, format, allowDuplicates);
            }
            else
            {
                existing = leaderboard;
            }

            List<string> lines = new List<string> { "PlayerName,Score,Time" };
            foreach (var entry in existing.Entries)
            {
                string formattedTime = TimeSpan.FromSeconds(entry.Time).ToString(@"mm\:ss\:fff");
                lines.Add($"{entry.PlayerName},{entry.Score},{formattedTime}");
            }

            File.WriteAllLines(path, lines);
            Debug.Log("Saved CSV leaderboard to: " + path);
        }

        private static Leaderboard LoadFromCSV(string path)
        {
            Leaderboard leaderboard = new Leaderboard();
            if (!File.Exists(path)) return leaderboard;

            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                if (values.Length == 3 &&
                    int.TryParse(values[1], out int score) &&
                    TimeSpan.TryParseExact(values[2], @"mm\:ss\:fff", null, out TimeSpan timeSpan))
                {
                    leaderboard.Entries.Add(new LeaderboardEntry(values[0], score, (float)timeSpan.TotalSeconds));
                }
            }

            return leaderboard;
        }

        private static void MergeEntries(Leaderboard existing, Leaderboard incoming, EntryFormat format, bool allowDuplicates)
        {
            if (allowDuplicates)
            {
                existing.Entries.AddRange(incoming.Entries);
                return;
            }

            foreach (var newEntry in incoming.Entries)
            {
                var existingEntry = existing.Entries
                    .FirstOrDefault(e => e.PlayerName.Equals(newEntry.PlayerName, StringComparison.OrdinalIgnoreCase));

                if (existingEntry != null)
                {
                    bool shouldReplace = ShouldReplaceEntry(existingEntry, newEntry, format);

                    if (shouldReplace)
                    {
                        // Replace existing entry
                        existing.Entries.Remove(existingEntry);
                        existing.Entries.Add(newEntry);
                    }
                }
                else
                {
                    // Add new unique entry
                    existing.Entries.Add(newEntry);
                }
            }

            //foreach (var newEntry in incoming.Entries)
            //{
            //    var existingEntry = existing.Entries.FirstOrDefault(e => e.PlayerName == newEntry.PlayerName);
            //    if (existingEntry != null)
            //    {
            //        bool shouldReplace = false;

            //        if (format == EntryFormat.NameScore)
            //            shouldReplace = newEntry.Score > existingEntry.Score;
            //        else if (format == EntryFormat.NameTime)
            //            shouldReplace = newEntry.Time < existingEntry.Time;

            //        if (shouldReplace)
            //        {
            //            existing.Entries.Remove(existingEntry);
            //            existing.Entries.Add(newEntry);
            //        }
            //    }
            //    else
            //    {
            //        existing.Entries.Add(newEntry);
            //    }
            //}
        }

        private static bool ShouldReplaceEntry(LeaderboardEntry existing, LeaderboardEntry incoming, EntryFormat format)
        {
            return format switch
            {
                EntryFormat.NameScore => incoming.Score > existing.Score,

                EntryFormat.NameTime => incoming.Time < existing.Time,

                EntryFormat.NameScoreTime =>
                    incoming.Score > existing.Score ||
                    (incoming.Score == existing.Score && incoming.Time < existing.Time),

                _ => false
            };
        }
    }
}