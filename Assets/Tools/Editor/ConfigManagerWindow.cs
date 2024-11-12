using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class ConfigManagerWindow : EditorWindow
{
    private readonly Dictionary<string, List<ScriptableObject>> _configGroups = new Dictionary<string, List<ScriptableObject>>();
    private readonly List<ScriptableObject> _favoriteConfigs = new List<ScriptableObject>();

    private Vector2 _scrollPos;
    private HashSet<string> _selectedConfigs = new HashSet<string>();
    private Color _selectedColor = Color.yellow;
    private Color _buttonColor = Color.cyan;

    [MenuItem("Window/Config Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<ConfigManagerWindow>("Config Manager");
        window.RefreshConfigLists();
    }

    private void OnEnable()
    {
        LoadSelectedConfigs();
        RefreshConfigLists();
    }

    private void OnDisable()
    {
        SaveSelectedConfigs();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Config Manager", EditorStyles.boldLabel);

        // Настройка цветов для выбранных конфигов и кнопки обновления
        _selectedColor = EditorGUILayout.ColorField("Favorite Config Color", _selectedColor);
        _buttonColor = EditorGUILayout.ColorField("Refresh Button Color", _buttonColor);

        // Кнопка обновления списка конфигов
        GUI.backgroundColor = _buttonColor;
        if (GUILayout.Button("Обновить список конфигов"))
        {
            RefreshConfigLists();
        }
        GUI.backgroundColor = Color.white;

        // Отображение групп конфигов
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        DisplayConfigGroup("Избранные Конфиги", _favoriteConfigs, true);
        foreach (var group in _configGroups)
        {
            DisplayConfigGroup(group.Key, group.Value, false);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DisplayConfigGroup(string title, List<ScriptableObject> configs, bool isFavoriteGroup)
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        foreach (var config in configs)
        {
            if (config == null) continue;

            EditorGUILayout.BeginHorizontal();

            // Цветное выделение для избранных конфигов
            bool isSelected = _selectedConfigs.Contains(config.name);
            GUI.backgroundColor = isSelected ? _selectedColor : Color.white;

            // Кнопка для открытия конфига
            if (GUILayout.Button(config.name, GUILayout.Width(200)))
            {
                OpenConfig(config);
            }

            GUI.backgroundColor = Color.white;

            // Тоггл для выбора/снятия конфига в "Избранные"
            bool newSelection = GUILayout.Toggle(isSelected, "Добавить в Избранное");
            if (newSelection && !isSelected)
            {
                _selectedConfigs.Add(config.name);
                RefreshConfigLists();
            }
            else if (!newSelection && isSelected)
            {
                _selectedConfigs.Remove(config.name);
                RefreshConfigLists();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void OpenConfig(ScriptableObject config)
    {
        AssetDatabase.OpenAsset(config);
    }

    private void RefreshConfigLists()
    {
        _configGroups.Clear();
        _favoriteConfigs.Clear();

        // Загружаем все ScriptableObject конфиги в папке Assets/Configs
        var allConfigs = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/Configs" })
            .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(config => config != null)
            .ToList();

        // Разделение конфигов по папкам
        foreach (var config in allConfigs)
        {
            string path = AssetDatabase.GetAssetPath(config);
            string folder = System.IO.Path.GetDirectoryName(path);

            if (_selectedConfigs.Contains(config.name))
            {
                _favoriteConfigs.Add(config);
            }
            else
            {
                if (!_configGroups.ContainsKey(folder))
                {
                    _configGroups[folder] = new List<ScriptableObject>();
                }
                _configGroups[folder].Add(config);
            }
        }
    }

    private void SaveSelectedConfigs()
    {
        string selectedConfigsStr = string.Join(",", _selectedConfigs);
        EditorPrefs.SetString("ConfigManagerWindow_SelectedConfigs", selectedConfigsStr);
    }

    private void LoadSelectedConfigs()
    {
        string selectedConfigsStr = EditorPrefs.GetString("ConfigManagerWindow_SelectedConfigs", "");
        _selectedConfigs = new HashSet<string>(selectedConfigsStr.Split(',').Where(s => !string.IsNullOrEmpty(s)));
    }
}
