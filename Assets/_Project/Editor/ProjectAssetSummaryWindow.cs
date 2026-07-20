using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public sealed class ProjectAssetSummaryWindow : EditorWindow
{
    private static readonly AssetCategory[] Categories =
    {
        new("Scenes", "t:Scene"),
        new("Prefabs", "t:Prefab"),
        new("Scripts", "t:MonoScript"),
        new("Textures", "t:Texture2D"),
        new("Audio clips", "t:AudioClip"),
        new("Materials", "t:Material"),
        new("Animations", "t:AnimationClip"),
        new("Scriptable objects", "t:ScriptableObject")
    };

    private readonly List<AssetCount> _assetCounts = new();
    private Vector2 _scrollPosition;
    private DateTime _lastRefreshTime;
    private GUIStyle _countStyle;
    private GUIStyle _subtitleStyle;

    [MenuItem("Tools/Project/Asset Summary")]
    private static void OpenWindow()
    {
        ProjectAssetSummaryWindow window = GetWindow<ProjectAssetSummaryWindow>();
        window.titleContent = new GUIContent("Asset Summary");
        window.minSize = new Vector2(380f, 320f);
        window.Show();
    }

    private void OnEnable()
    {
        RefreshAssetCounts();
    }

    private void OnGUI()
    {
        EnsureStyles();

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("Project Asset Summary", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(
            "A read-only overview of assets stored below the Assets folder.",
            _subtitleStyle);
        EditorGUILayout.Space(6f);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Refresh", GUILayout.Height(28f)))
                RefreshAssetCounts();

            using (new EditorGUI.DisabledScope(_assetCounts.Count == 0))
            {
                if (GUILayout.Button("Copy report", GUILayout.Height(28f)))
                    EditorGUIUtility.systemCopyBuffer = BuildTextReport();
            }
        }

        EditorGUILayout.Space(8f);
        DrawSummary();
        EditorGUILayout.Space(8f);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        foreach (AssetCount item in _assetCounts)
            DrawAssetCount(item);
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(
            $"Last refreshed: {_lastRefreshTime:yyyy-MM-dd HH:mm:ss}",
            EditorStyles.miniLabel);
    }

    private void DrawSummary()
    {
        int total = _assetCounts.Sum(item => item.Count);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Tracked assets", total.ToString(), _countStyle);
            EditorGUILayout.LabelField("Categories", _assetCounts.Count.ToString(), _countStyle);
        }
    }

    private static void DrawAssetCount(AssetCount item)
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField(item.Name, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(
                item.Count.ToString(),
                EditorStyles.boldLabel,
                GUILayout.Width(70f));
        }
    }

    private void RefreshAssetCounts()
    {
        _assetCounts.Clear();

        foreach (AssetCategory category in Categories)
        {
            string[] guids = AssetDatabase.FindAssets(category.SearchFilter, new[] { "Assets" });
            int validAssetCount = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Count(path => !string.IsNullOrWhiteSpace(path));

            _assetCounts.Add(new AssetCount(category.Name, validAssetCount));
        }

        _lastRefreshTime = DateTime.Now;
        Repaint();
    }

    private string BuildTextReport()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Project Asset Summary");
        builder.AppendLine($"Generated: {_lastRefreshTime:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine();

        foreach (AssetCount item in _assetCounts)
            builder.AppendLine($"{item.Name}: {item.Count}");

        builder.AppendLine();
        builder.AppendLine($"Tracked total: {_assetCounts.Sum(item => item.Count)}");
        return builder.ToString();
    }

    private void EnsureStyles()
    {
        _countStyle ??= new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold
        };

        _subtitleStyle ??= new GUIStyle(EditorStyles.wordWrappedMiniLabel)
        {
            normal = { textColor = EditorStyles.label.normal.textColor }
        };
    }

    private readonly struct AssetCategory
    {
        public AssetCategory(string name, string searchFilter)
        {
            Name = name;
            SearchFilter = searchFilter;
        }

        public string Name { get; }
        public string SearchFilter { get; }
    }

    private readonly struct AssetCount
    {
        public AssetCount(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public string Name { get; }
        public int Count { get; }
    }
}
