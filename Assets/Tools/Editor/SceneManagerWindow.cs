using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public sealed class SceneManagerWindow : EditorWindow
{
    private readonly List<SceneAsset> _customScenes = new List<SceneAsset>();
    private readonly List<SceneAsset> _indexedScenes = new List<SceneAsset>();
    private readonly List<SceneAsset> _otherScenes = new List<SceneAsset>();

    private Vector2 _scrollPos;
    private HashSet<string> _selectedScenes = new HashSet<string>();
    private Color _selectedColor = Color.green;
    private Color _buttonColor = Color.cyan;

    [MenuItem("Window/Scene Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<SceneManagerWindow>("Scene Manager");
        window.RefreshSceneLists();
    }

    private void OnEnable()
    {
        LoadSelectedScenes();
        RefreshSceneLists();
    }

    private void OnDisable()
    {
        SaveSelectedScenes();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scene Manager", EditorStyles.boldLabel);

        // Настройка цветов для выбранных сцен и кнопки обновления
        _selectedColor = EditorGUILayout.ColorField("Selected Scene Color", _selectedColor);
        _buttonColor = EditorGUILayout.ColorField("Refresh Button Color", _buttonColor);

        // Кнопка обновления списка сцен
        GUI.backgroundColor = _buttonColor;
        if (GUILayout.Button("Обновить список сцен"))
        {
            RefreshSceneLists();
        }
        GUI.backgroundColor = Color.white;

        // Отображение групп сцен
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        DisplaySceneGroup("Мои Сцены", _customScenes, true);
        DisplaySceneGroup("Сцены в Индексе", _indexedScenes, false);
        DisplaySceneGroup("Остальные Сцены", _otherScenes, false);

        EditorGUILayout.EndScrollView();
    }

    private void DisplaySceneGroup(string title, List<SceneAsset> scenes, bool isCustomGroup)
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        foreach (var scene in scenes)
        {
            if (scene == null) continue;

            EditorGUILayout.BeginHorizontal();

            // Цветное выделение для выбранных сцен
            bool isSelected = _selectedScenes.Contains(scene.name);
            GUI.backgroundColor = isSelected ? _selectedColor : Color.white;

            // Кнопка для перехода к сцене
            if (GUILayout.Button(scene.name, GUILayout.Width(200)))
            {
                OpenScene(scene);
            }

            GUI.backgroundColor = Color.white;

            // Тоггл для выбора/снятия сцены в "Мои Сцены"
            bool newSelection = GUILayout.Toggle(isSelected, "Добавить в Мои Сцены");
            if (newSelection && !isSelected)
            {
                _selectedScenes.Add(scene.name);
                RefreshSceneLists();
            }
            else if (!newSelection && isSelected)
            {
                _selectedScenes.Remove(scene.name);
                RefreshSceneLists();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void OpenScene(SceneAsset scene)
    {
        string scenePath = AssetDatabase.GetAssetPath(scene);
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    private void RefreshSceneLists()
    {
        _customScenes.Clear();
        _indexedScenes.Clear();
        _otherScenes.Clear();

        // Загружаем все сцены из Build Settings, находящиеся в папке Assets/Scenes
        var scenesInBuildSettings = EditorBuildSettings.scenes
            .Where(s => s.enabled && s.path.StartsWith("Assets/Scenes"))
            .Select(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path))
            .Where(s => s != null)
            .ToList();

        // Находим все сцены в папке Assets/Scenes
        var allScenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" })
            .Select(guid => AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(s => s != null)
            .ToList();

        // Разделение сцен по группам
        foreach (var scene in allScenes)
        {
            if (_selectedScenes.Contains(scene.name))
            {
                _customScenes.Add(scene);
            }
            else if (scenesInBuildSettings.Contains(scene))
            {
                _indexedScenes.Add(scene);
            }
            else
            {
                _otherScenes.Add(scene);
            }
        }
    }

    private void SaveSelectedScenes()
    {
        string selectedScenesStr = string.Join(",", _selectedScenes);
        EditorPrefs.SetString("SceneManagerWindow_SelectedScenes", selectedScenesStr);
    }

    private void LoadSelectedScenes()
    {
        string selectedScenesStr = EditorPrefs.GetString("SceneManagerWindow_SelectedScenes", "");
        _selectedScenes = new HashSet<string>(selectedScenesStr.Split(',').Where(s => !string.IsNullOrEmpty(s)));
    }
}