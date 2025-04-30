using System.IO;
using UnityEngine;

namespace GAG.EasyLeaderboard
{
    public class EasyLeaderboardManager : MonoBehaviour
    {
        public static EasyLeaderboardManager Instance { get; private set; }
        public string LeaderboardPath => GetSavePath();

        [Header("Leaderboard Settings")]
        [SerializeField] EasyLeaderboardUtility .LeaderboardType _leaderboardType = EasyLeaderboardUtility .LeaderboardType.JSON;
        public EasyLeaderboardUtility .EntryFormat EntryFormat = EasyLeaderboardUtility .EntryFormat.NameScoreTime;
        [SerializeField] bool _allowDuplicateNames = false;
        public bool UseScoreSortingInNameScoreTime = false;

        [Header("Deployment Platform")]
        [SerializeField] EasyLeaderboardUtility .LeaderboardDeployPlatform _deployPlatform = EasyLeaderboardUtility .LeaderboardDeployPlatform.PC;

        Leaderboard _leaderboard = new Leaderboard();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {
            EasyLeaderboardEvents.OnLeaderboardEntryAdded += AddEntry;
            EasyLeaderboardEvents.OnLeaderboardLoadRequested += LoadLeaderboard;
        }

        void OnDisable()
        {
            EasyLeaderboardEvents.OnLeaderboardEntryAdded -= AddEntry;
            EasyLeaderboardEvents.OnLeaderboardLoadRequested -= LoadLeaderboard;
        }

        void AddEntry(LeaderboardEntry entry)
        {
            TrimEntry(entry);
            _leaderboard.Entries.Clear();
            _leaderboard.AddEntry(entry);
            string path = GetSavePath();
            EasyLeaderboardUtility .EnsureFileExists(path);
            EasyLeaderboardUtility .SaveLeaderboard(_leaderboard, path, _leaderboardType, EntryFormat, _allowDuplicateNames);
        }

        void LoadLeaderboard()
        {
            string path = GetSavePath();
            EasyLeaderboardUtility .EnsureFileExists(path);
            _leaderboard = EasyLeaderboardUtility .LoadLeaderboard(path, _leaderboardType);
            EasyLeaderboardEvents.RaiseOnLeaderboardLoaded(_leaderboard);
        }

        void TrimEntry(LeaderboardEntry entry)
        {
            switch (EntryFormat)
            {
                case EasyLeaderboardUtility .EntryFormat.NameScore:
                    entry.Time = 0;
                    break;
                case EasyLeaderboardUtility .EntryFormat.NameTime:
                    entry.Score = 0;
                    break;
            }
        }

        string GetSavePath()
        {
            string fileName = "Leaderboard" + (_leaderboardType == EasyLeaderboardUtility .LeaderboardType.JSON ? ".json" : ".csv");

            string basePath = _deployPlatform switch
            {
                EasyLeaderboardUtility .LeaderboardDeployPlatform.PC => Application.streamingAssetsPath,
                EasyLeaderboardUtility .LeaderboardDeployPlatform.Android => Application.persistentDataPath,
                EasyLeaderboardUtility .LeaderboardDeployPlatform.IOS => Application.persistentDataPath,
                _ => Application.persistentDataPath
            };

            return Path.Combine(basePath, fileName);
        }
    }
}