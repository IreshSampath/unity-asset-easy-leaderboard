using System.IO;
using UnityEngine;

public class LeaderboardManagerNew : MonoBehaviour
{
    [Header("Leaderboard Settings")]
    [SerializeField] LeaderboardUtility.LeaderboardType _leaderboardType = LeaderboardUtility.LeaderboardType.JSON;
    [SerializeField] LeaderboardUtility.EntryFormat _entryFormat = LeaderboardUtility.EntryFormat.NameScoreTime;
    [SerializeField] bool _allowDuplicateNames = false;

    [Header("Deployment Platform")]
    [SerializeField] LeaderboardUtility.LeaderboardDeployPlatform _deployPlatform = LeaderboardUtility.LeaderboardDeployPlatform.PC;

    Leaderboard _leaderboard = new Leaderboard();

    void OnEnable()
    {
        AppEvents.OnLeaderboardEntryAdded += AddEntry;
        AppEvents.OnLeaderboardLoadRequested += LoadLeaderboard;
    }

    void OnDisable()
    {
        AppEvents.OnLeaderboardEntryAdded -= AddEntry;
        AppEvents.OnLeaderboardLoadRequested -= LoadLeaderboard;
    }

    void AddEntry(LeaderboardEntry entry)
    {
        TrimEntry(entry);
        _leaderboard.Entries.Clear();
        _leaderboard.AddEntry(entry);
        string path = GetSavePath();
        LeaderboardUtility.EnsureFileExists(path);
        LeaderboardUtility.SaveLeaderboard(_leaderboard, path, _leaderboardType, _entryFormat, _allowDuplicateNames);
    }

    void LoadLeaderboard()
    {
        string path = GetSavePath();
        LeaderboardUtility.EnsureFileExists(path);
        _leaderboard = LeaderboardUtility.LoadLeaderboard(path, _leaderboardType);
        AppEvents.RaiseOnLeaderboardLoaded(_leaderboard);
    }

    void TrimEntry(LeaderboardEntry entry)
    {
        switch (_entryFormat)
        {
            case LeaderboardUtility.EntryFormat.NameScore:
                entry.Time = 0;
                break;
            case LeaderboardUtility.EntryFormat.NameTime:
                entry.Score = 0;
                break;
        }
    }

    string GetSavePath()
    {
        string fileName = "Leaderboard" + (_leaderboardType == LeaderboardUtility.LeaderboardType.JSON ? ".json" : ".csv");

        string basePath = _deployPlatform switch
        {
            LeaderboardUtility.LeaderboardDeployPlatform.PC => Application.streamingAssetsPath,
            LeaderboardUtility.LeaderboardDeployPlatform.Android => Application.persistentDataPath,
            LeaderboardUtility.LeaderboardDeployPlatform.IOS => Application.persistentDataPath,
            _ => Application.persistentDataPath
        };

        return Path.Combine(basePath, fileName);
    }
}
