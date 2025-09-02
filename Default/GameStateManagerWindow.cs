#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class GameStateManagerEditorWindow : EditorWindow
{
    private GameStateManager.GameSettings gameSettings;

    private Vector2 scrollPosition;

    [MenuItem("Manager/GameStateManager GUI")]
    public static void OpenWindow()
    {
        GameStateManagerEditorWindow window = GetWindow<GameStateManagerEditorWindow>("Game State Manager");
        window.minSize = new Vector2(400, 600);
        window.Show();
    }

    private void OnEnable()
    {
        LoadData();
    }

    private void OnDisable()
    {
        SaveData();
    }

    private void OnGUI()
    {
        GUILayout.Label("🎮 Game State Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (gameSettings == null)
        {
            EditorGUILayout.HelpBox("GameSettings가 로드되지 않았습니다!", MessageType.Warning);
            if (GUILayout.Button("Load Default Settings"))
            {
                LoadData();
            }
            return;
        }

        // 스크롤뷰 시작
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 50));

        EditorGUILayout.LabelField("🔑 Login Settings", EditorStyles.boldLabel);
        gameSettings.playfabId = EditorGUILayout.TextField("Playfab ID", gameSettings.playfabId);
        gameSettings.customId = EditorGUILayout.TextField("Custom ID", gameSettings.customId);
        gameSettings.stoveId = EditorGUILayout.TextField("Stove ID", gameSettings.stoveId);
        gameSettings.region = EditorGUILayout.TextField("Region", gameSettings.region);
        gameSettings.autoLogin = EditorGUILayout.Toggle("Auto Login", gameSettings.autoLogin);
        gameSettings.nickName = EditorGUILayout.TextField("Nickname", gameSettings.nickName);
        gameSettings.loginType = (LoginType)EditorGUILayout.EnumPopup("Login Type", gameSettings.loginType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Language Settings", EditorStyles.boldLabel);
        gameSettings.language = (LanguageType)EditorGUILayout.EnumPopup("Language", gameSettings.language);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Game Settings", EditorStyles.boldLabel);
        gameSettings.screenMode = (ScreenMode)EditorGUILayout.EnumPopup("Screen Mode", gameSettings.screenMode);
        gameSettings.resolution = (Resolution)EditorGUILayout.EnumPopup("Resolution", gameSettings.resolution);
        EditorGUILayout.Space();
        gameSettings.music = EditorGUILayout.Toggle("Music", gameSettings.music);
        gameSettings.musicValue = EditorGUILayout.FloatField("Music Value", gameSettings.musicValue);
        EditorGUILayout.Space();
        gameSettings.sfx = EditorGUILayout.Toggle("SFX", gameSettings.sfx);
        gameSettings.sfxValue = EditorGUILayout.FloatField("SFX Value", gameSettings.sfxValue);
        gameSettings.vibration = EditorGUILayout.Toggle("Vibration", gameSettings.vibration);
        gameSettings.mouseSensitivity = EditorGUILayout.IntField("Mouse Sensitivity", gameSettings.mouseSensitivity);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Player Settings", EditorStyles.boldLabel);
        gameSettings.isPlaying = EditorGUILayout.Toggle("IsPlaying", gameSettings.isPlaying);
        gameSettings.playerMoveSpeed = EditorGUILayout.FloatField("Move Speed", gameSettings.playerMoveSpeed);
        gameSettings.playerAcceleration = EditorGUILayout.FloatField("Acceleration", gameSettings.playerAcceleration);
        gameSettings.playerRotationalSpeed = EditorGUILayout.FloatField("Rotational Speed", gameSettings.playerRotationalSpeed);
        gameSettings.missileSpeed = EditorGUILayout.FloatField("Missile Speed", gameSettings.missileSpeed);
        gameSettings.deathNumber = EditorGUILayout.IntField("Death Number", gameSettings.deathNumber);
        gameSettings.adCount = EditorGUILayout.IntField("Ad Number", gameSettings.adCount);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Cut Scene", EditorStyles.boldLabel);
        gameSettings.first = EditorGUILayout.Toggle("First", gameSettings.first);
        gameSettings.opening = EditorGUILayout.Toggle("Opening", gameSettings.opening);
        gameSettings.mid = EditorGUILayout.Toggle("Mid", gameSettings.mid);
        gameSettings.ending = EditorGUILayout.Toggle("Ending", gameSettings.ending);

        GUILayout.EndScrollView();

        EditorGUILayout.Space();

        // 저장 및 불러오기 버튼
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Settings", GUILayout.Height(30)))
        {
            SaveData();
            Debug.Log("Settings saved successfully!");
        }

        if (GUILayout.Button("Load Settings", GUILayout.Height(30)))
        {
            LoadData();
            Debug.Log("Settings loaded successfully!");
        }
        GUILayout.EndHorizontal();
    }

    private void LoadData()
    {
        try
        {
            Debug.Log("Loading Settings...");
            string json = FileIO.LoadData(GameStateManager.DEVICESETTINGFILENAME, true);
            if (!string.IsNullOrEmpty(json))
            {
                gameSettings = JsonUtility.FromJson<GameStateManager.GameSettings>(json);
            }
            else
            {
                gameSettings = new GameStateManager.GameSettings();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Load Error: " + e.Message);
            gameSettings = new GameStateManager.GameSettings();
        }
    }

    private void SaveData()
    {
        try
        {
            Debug.Log("Saving Settings...");
            string json = JsonUtility.ToJson(gameSettings, true);
            FileIO.SaveData(GameStateManager.DEVICESETTINGFILENAME, json, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save Error: " + e.Message);
        }
    }
}

#endif
