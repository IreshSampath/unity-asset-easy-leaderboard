using TMPro;
using UnityEngine;

namespace GAG.EasyLeaderboard
{
    public class EasyLeaderboardUIManager : MonoBehaviour
    {
        [Header("Add Leaderboard Entry")]
        [SerializeField] TMP_InputField _nameInputField;
        [SerializeField] TMP_InputField _scoreInputField;
        [SerializeField] TMP_InputField _timeInputField;

        [Header("Leaderboard Display")]
        [SerializeField] Transform _entryParent;
        [SerializeField] Transform _entry;

        [SerializeField] GameObject _headerScoreText;
        [SerializeField] GameObject _headerTimeText;

        void OnEnable()
        {
            // Subscribe to the events
            EasyLeaderboardEvents.OnLeaderboardLoaded += LoadLeaderboard;
        }

        void OnDisable()
        {
            // Unsubscribe from the events
            EasyLeaderboardEvents.OnLeaderboardLoaded -= LoadLeaderboard;
        }

        public void AddLeaderboardEntry()
        {
            EasyLeaderboardEvents.RaiseOnLeaderboardEntryAdded(new LeaderboardEntry(_nameInputField.text, int.Parse(_scoreInputField.text), float.Parse(_timeInputField.text)));
        }

        public void RequestLeaderboardEntries()
        {
            EasyLeaderboardEvents.RaiseOnLeaderboardLoadRequested();
        }

        public void FilterEntries(Transform entry)
        {
            if (EasyLeaderboardManager.Instance.EntryFormat == EasyLeaderboardUtility.EntryFormat.NameScore)
            {
                entry.GetChild(1).gameObject.SetActive(true);
                entry.GetChild(2).gameObject.SetActive(false);
            }


            if (EasyLeaderboardManager.Instance.EntryFormat == EasyLeaderboardUtility.EntryFormat.NameTime)
            {
                entry.GetChild(1).gameObject.SetActive(false);
                entry.GetChild(2).gameObject.SetActive(true);
            }

            if (EasyLeaderboardManager.Instance.EntryFormat == EasyLeaderboardUtility.EntryFormat.NameScoreTime)
            {
                entry.GetChild(1).gameObject.SetActive(true);
                entry.GetChild(2).gameObject.SetActive(true);
            }
        }

        public void ClearLeaderboard()
        {
            EasyLeaderboardUtility.DeleteLeaderboard(EasyLeaderboardManager.Instance.LeaderboardPath);
            foreach (Transform child in _entryParent)
            {
                Destroy(child.gameObject);
            }
        }

        void LoadLeaderboard(Leaderboard leaderboard)
        {
            var format = EasyLeaderboardManager.Instance.EntryFormat;
            var useScoreSort = EasyLeaderboardManager.Instance.UseScoreSortingInNameScoreTime;

            switch (format)
            {
                case EasyLeaderboardUtility.EntryFormat.NameScore:
                    leaderboard.SortByScoreDescending();
                    if (_headerScoreText != null) _headerScoreText.SetActive(true);
                    if (_headerTimeText != null) _headerTimeText.SetActive(false);
                    break;

                case EasyLeaderboardUtility.EntryFormat.NameTime:
                    leaderboard.SortByTimeAscending();
                    if (_headerScoreText != null) _headerScoreText.SetActive(false);
                    if (_headerTimeText != null) _headerTimeText.SetActive(true);
                    break;

                case EasyLeaderboardUtility.EntryFormat.NameScoreTime:
                    if (useScoreSort)
                        leaderboard.SortByScoreDescending();
                    else
                        leaderboard.SortByTimeAscending();

                    if (_headerScoreText != null) _headerScoreText.SetActive(true);
                    if (_headerTimeText != null) _headerTimeText.SetActive(true);
                    break;
            }

            // Clear existing entries
            foreach (Transform child in _entryParent)
                {
                    Destroy(child.gameObject);
                }

            // Handle the loaded leaderboard
            foreach (var entry in leaderboard.Entries)
            {
                // Instantiate a new entry UI element
                Transform newEntry = Instantiate(_entry, _entryParent);
                newEntry.GetChild(0).GetComponent<TMP_Text>().text = entry.PlayerName;

                newEntry.GetChild(1).GetComponent<TMP_Text>().text = entry.Score.ToString();
                newEntry.GetChild(2).GetComponent<TMP_Text>().text = FormattedTime(entry.Time).ToString();
                FilterEntries(newEntry);
            }
        }

        string FormattedTime(float time)
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(time);
            return timeSpan.ToString(@"mm\:ss\:ff");
        }
    }
}
