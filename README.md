# 🏆 EasyLeaderboard for Unity

**EasyLeaderboard** is a lightweight, extensible Unity package that allows you to save, display, and manage leaderboards using **JSON** or **CSV**. Perfect for games that need simple high-score tracking with customizable formats and UI.

![Unity](https://img.shields.io/badge/Unity-2022%2B-green.svg)
![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Android%20%7C%20iOS%20%7C%20Unity%20Editor-lightgrey.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

---

## 🚀 Features

- ✅ Supports both **JSON** and **CSV** file formats
- ✅ Choose between **NameScore**, **NameTime**, or **NameScoreTime** entry formats
- ✅ Automatic sorting and merging of entries
- ✅ Easy-to-use UI components and sample scene
- ✅ Add or overwrite entries by player name
- ✅ Supports duplicate entries if enabled
- ✅ Platform-specific save path configuration (PC, Android, iOS)
- ✅ Sample prefab and scene included
---


## 📦 Installation

### Via Unity Package Manager (Git URL)

1. Open Unity
2. Go to **Window → Package Manager**
3. Click **+** → **Add package from Git URL**
4. Paste:
   https://github.com/IreshSampath/unity-assets-easy-leaderboard.git
   
6. Click **Add**

### Requirements

- Unity 2022.3 or newer
- TextMeshPro (`com.unity.textmeshpro`)
   
---

## 🧠 How to Use

### ✅ Step 1 – Add Manager Components

There are two ways to get started: manual setup or using the provided sample prefab.

#### 🔹 Option 1: Manual Setup

1. Add the `EasyLeaderboardManager` component to a GameObject in your scene  
2. In the **Inspector**, configure:
- `Leaderboard Type`: `JSON` or `CSV`
- `Entry Format`: `NameScore`, `NameTime`, or `NameScoreTime`
- `Allow Duplicate Names`: toggle on or off
- `Use Score Sorting In NameScoreTime`: toggle on or off
- `Deploy Platform`: `PC`, `Android`, or `iOS`

3. Add the `EasyLeaderboardUIManager` component

4. Assign:
- `Entry Parent`: container for leaderboard entries
- `Entry`: the prefab for each entry (using `TMP_Text`)

#### 🔹 Option 2: Use Sample Prefab

1. Open the `Assets/Samples/Easy Leaderboard/1.0.3/Easy Leaderboard Sample/Prefabs` folder
2. Drag the **`EasyLeaderboard`** prefab into your scene
3. Customize the components via Inspector

##

### ✅ Step 2 – Add and Load Leaderboard Entries

➕ Add an Entry
```csharp
    EasyLeaderboardEvents.RaiseOnLeaderboardEntryAdded(
     new LeaderboardEntry(
         _nameInputField.text,
         int.Parse(_scoreInputField.text),
         float.Parse(_timeInputField.text)
     )
    );
```

🔄 Load the Leaderboard
```csharp
    EasyLeaderboardEvents.RaiseOnLeaderboardLoadRequested();
```
### ✅ Step 3 – UI Display

📃 UI Display

The `EasyLeaderboardUIManager` automatically populates a scrollable UI list using your assigned prefab.

Make sure to:

- Assign the input fields (Name, Score, Time)
- Assign the entry parent Transform
- Assign the entry prefab (with TMP_Text components)

---

## 🙏 Thank You
Thanks for using Easy Leaderboard!
- Feel free to contribute, report bugs, or request new features.

---

## 👤 Author:
Iresh Sampath 🔗 [LinkedIn Profile](https://www.linkedin.com/in/ireshsampath/)

