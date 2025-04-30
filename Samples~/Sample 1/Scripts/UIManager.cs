using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GAG.EasyLeaderboard
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] TMP_InputField _name;
        [SerializeField] TMP_InputField _score;
        [SerializeField] TMP_InputField _time;

        [SerializeField] Transform _entryParent;
        [SerializeField] Transform _entry;

        [SerializeField] Toggle _scoreToggle;
        [SerializeField] Toggle _timeToggle;

        void OnEnable()
        {
            // Subscribe to the events
            AppEvents.OnLeaderboardLoaded += LoadLeaderboard;
        }

        void OnDisable()
        {
            // Unsubscribe from the events
            AppEvents.OnLeaderboardLoaded -= LoadLeaderboard;
        }

        public void AddLeaderboardEntry()
        {
            AppEvents.RaiseOnLeaderboardEntryAdded(new LeaderboardEntry(_name.text, int.Parse(_score.text), float.Parse(_time.text)));
        }

        public void RequestLeaderboardEntries()
        {
            AppEvents.RaiseOnLeaderboardLoadRequested();
        }

        void LoadLeaderboard(Leaderboard leaderboard)
        {
            if (_scoreToggle.isOn)
            {

                leaderboard.SortByScoreDescending();
            }
            else if (_timeToggle.isOn)
            {
                leaderboard.SortByTimeAscending();
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

        public void FilterEntries(Transform entry)
        {
            if (_scoreToggle.isOn)
            {
                entry.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                entry.GetChild(1).gameObject.SetActive(false);
            }

            if (_timeToggle.isOn)
            {
                entry.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                entry.GetChild(2).gameObject.SetActive(false);
            }
        }
    }
}
