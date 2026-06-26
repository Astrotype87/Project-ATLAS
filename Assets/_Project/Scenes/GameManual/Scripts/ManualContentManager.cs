using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManualContentManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image contentImage;

    [Header("Default Page")]
    public GameObject defaultPanel;

    [Header("Scroll Rect (optional)")]
    public ScrollRect scrollRect;

    [Header("Optional Images")]
    public Sprite basicsImage;
    public Sprite campaignImage;
    public Sprite howToPlayImage;
    public Sprite freePlayImage;
    public Sprite libraryImage;
    public Sprite settingsImage;
    public Sprite aboutImage;
    public Sprite achievementsImage;
    public Sprite leaderboardImage;
    public Sprite profileImage;
    public Sprite progressImage;
    public Sprite levelTypeImage;
    public Sprite audioImage;

    private void Start()
    {
        
    }

    private void UpdateManualContent(string title, string body, Sprite image = null)
    {
        titleText.text = title;
        bodyText.text = body;

        contentImage.gameObject.SetActive(true);

        if (image != null)
        {
            contentImage.sprite = image;
        }

        if (defaultPanel != null)
            defaultPanel.SetActive(true);

        // Force UI to rebuild layout immediately
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(defaultPanel.GetComponent<RectTransform>());

        // Optional: scroll to top
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }



    public void ShowBasics()
    {
        UpdateManualContent(
            "Basics",
            "Learn how to play ATLAS movement, controls, UI layout, and getting started.",
            basicsImage
        );
    }

    public void ShowCampaign()
    {
        UpdateManualContent(
            "Campaign Mode",
            "Chapters and levels aligned with General Physics 1. Learn through lessons, quizzes, simulations, and challenges.",
            campaignImage
        );
    }

    public void ShowHowToPlay()
    {
        UpdateManualContent(
            "How to Play",
            "Navigate the story of Atlas and his robot companion Gizmo by completing levels across different chapters. Each level teaches a physics concept through lessons, simulations, or minigames. Your objective is to complete the levels, collect medals, and improve your understanding of General Physics 1 while enjoying a fun and engaging adventure.",
            howToPlayImage
        );
    }

    public void ShowFreePlay()
    {
        UpdateManualContent(
            "Free Play",
            "In Free Play Mode, you can revisit any unlocked quiz, simulation, or minigame without following the story. This is a great way to review lessons, improve scores, or explore physics at your own pace. All content unlocked in Campaign Mode becomes available here.",
            freePlayImage
        );
    }

    public void ShowLibrary()
    {
        UpdateManualContent(
            "Library",
            "The Library contains all the learning materials you’ve unlocked: Guidebooks: Short summaries and explanations of physics concepts. Glossary: Definitions and meanings of physics terms. Use the library to review past lessons or to study before tests.",
            libraryImage
        );
    }

    public void ShowSettings()
    {
        UpdateManualContent(
            "Settings",
            "Customize ATLAS to match your preferences and device performance. The Settings menu lets you adjust:\n\n" +
            "Audio Levels: Control individual sliders for:\n" +
            "All – Master volume for the entire game\n" +
            "Music – Background music volume\n" +
            "SFX – Sound effects for gameplay and UI\n\n" +
            "Controls: Modify on-screen control layouts and sensitivity\n\n" +
            "Display & Graphics: Optimize visuals and performance based on your device",
            settingsImage
        );
    }

    public void ShowAbout()
    {
        UpdateManualContent(
            "About",
            "ATLAS is a 2D educational mobile game developed by Grade 12 STEM students of STI College Baliuag as a capstone project. It combines physics education with interactive gameplay, designed to help students understand General Physics 1 in a fun and engaging way.",
            aboutImage
        );
    }

    public void ShowAchievements()
    {
        UpdateManualContent(
            "Achievements",
            "Achievements reward players for their progress, performance, and exploration in the game.\n\n" +
            "Progress Achievements: Earned by completing chapters, finishing levels, and collecting medals.\n" +
            "Performance Achievements: Based on quiz scores, fast completion times, and consistent performance.\n" +
            "Exploration Achievements: Given for unlocking all guidebooks, glossary terms, and playing all game modes.\n\n" +
            "Check your achievements to track how far you’ve come — and what challenges still await!",
            achievementsImage
        );
    }

    public void ShowLeaderboard()
    {
        UpdateManualContent(
            "Leaderboards",
            "Leaderboards in ATLAS track your best scores and learning progress across all levels.\n\n" +
            "Ranking is based on:\n" +
            "Total Points Acquired from quizzes, simulations, and challenges\n" +
            "Medals Earned in lesson and challenge levels\n" +
            "Chapters and Levels Completed\n\n" +
            "Leaderboard Displays:\n" +
            "Top-scoring levels based on your highest points\n" +
            "Fastest completion times for each level\n" +
            "Overall campaign performance and learning progress\n\n" +
            "Use the leaderboard to challenge yourself, aim for higher scores, and track your improvement over time.",
            leaderboardImage
        );
    }

    public void ShowProfile()
    {
        UpdateManualContent(
            "Profile",
            "Your Profile tracks everything:\n" +
            "Chapters and levels completed\n" +
            "Scores, medals, and time played\n" +
            "Achievements and leaderboard rank\n" +
            "You can also customize your avatar and account details here.",
            profileImage
        );
    }

    public void ShowProgress()
    {
        UpdateManualContent(
            "Progress",
            "The game tracks your learning journey across all chapters, levels, and activities.\n\n" +
            "Campaign Progress:\nSee your completed chapters and levels\nView medals earned (Bronze, Silver, Gold)\nTrack unlocked guidebooks and glossary terms\n\n" +
            "Level Records:\nMonitor quiz scores, simulation completions, and challenge outcomes\nCheck time taken, number of attempts, and best scores\n\n" +
            "Test Results:\nView your Pre-Test and Post-Test results for each chapter\nCompare scores to measure improvement and mastery\n\n" +
            "Overall Stats:\nTotal play time\nNumber of completions and fails\nHighest scoring levels and fastest completions\n\n" +
            "Progress is automatically saved and can be viewed in your profile at any time.",
            progressImage
        );
    }

    public void ShowLevelType()
    {
        UpdateManualContent(
            "Level Type",
            "Each level in ATLAS serves a unique purpose:\n" +
            "Pre-Test: Measures what you already know.\n" +
            "Lesson: Teaches bite-sized content with visuals and narration.\n" +
            "Simulation: Interactive experiments to explore physics in action.\n" +
            "Minigame (Challenge): Fast-paced games to apply what you’ve learned.\n" +
            "Post-Test: Tests your understanding of the entire chapter.\n" +
            "Completing levels earns medals and unlocks guidebooks.",
            levelTypeImage
        );
    }
}
