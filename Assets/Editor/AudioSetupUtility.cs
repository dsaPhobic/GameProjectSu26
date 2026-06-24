using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AudioSetupUtility
{
    private const string MainMenuScene = "Assets/_Project/Scenes/MainMenu.unity";
    private const string GameScene = "Assets/_Project/Scenes/GameScene.unity";

    private static readonly (string Key, string Path)[] BgmClips =
    {
        ("bgm_main_menu", "Assets/_Project/Audio/BGM/bgm_main_menu.mp3"),
        ("bgm_day", "Assets/_Project/Audio/BGM/bgm_day.mp3"),
        ("bgm_night", "Assets/_Project/Audio/BGM/bgm_night.mp3"),
        ("bgm_boss", "Assets/_Project/Audio/BGM/bgm_boss.mp3"),
        ("bgm_game_over", "Assets/_Project/Audio/BGM/bgm_game_over.mp3"),
    };

    private static readonly (string Key, string Path)[] SfxClips =
    {
        ("sfx_attack", "Assets/_Project/Audio/SFX/sfx_attack.mp3"),
        ("sfx_hit", "Assets/_Project/Audio/SFX/sfx_hit.mp3"),
        ("sfx_harvest", "Assets/_Project/Audio/SFX/sfx_harvest.mp3"),
        ("sfx_levelup", "Assets/_Project/Audio/SFX/sfx_levelup.mp3"),
        ("sfx_die", "Assets/_Project/Audio/SFX/sfx_die.mp3"),
        ("sfx_digging", "Assets/_Project/Audio/SFX/sfx_digging.mp3"),
        ("sfx_button_click", "Assets/_Project/Audio/SFX/sfx_button_click.mp3"),
        ("sfx_powerup_pickup", "Assets/_Project/Audio/SFX/sfx_powerup_pickup.mp3"),
    };

    [MenuItem("Magic Farm/Audio/Assign Audio Clips To Scenes")]
    public static void AssignAudioClipsToScenes()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("Stop Play Mode before assigning audio clips to scenes.");
            return;
        }

        ConfigureImportSettings();
        AssetDatabase.Refresh();

        string activeScenePath = SceneManager.GetActiveScene().path;
        ConfigureScene(MainMenuScene);
        ConfigureScene(GameScene);

        if (!string.IsNullOrEmpty(activeScenePath))
            EditorSceneManager.OpenScene(activeScenePath);

        AssetDatabase.SaveAssets();
        Debug.Log("Audio clips assigned to AudioManager in MainMenu and GameScene.");
    }

    private static void ConfigureImportSettings()
    {
        foreach (var clip in BgmClips)
            ConfigureAudioImporter(clip.Path, AudioClipLoadType.Streaming, AudioCompressionFormat.Vorbis);

        foreach (var clip in SfxClips)
            ConfigureAudioImporter(clip.Path, AudioClipLoadType.DecompressOnLoad, AudioCompressionFormat.ADPCM);
    }

    private static void ConfigureAudioImporter(string path, AudioClipLoadType loadType, AudioCompressionFormat compression)
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning($"Audio file not found: {path}");
            return;
        }

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        if (AssetImporter.GetAtPath(path) is not AudioImporter importer) return;

        AudioImporterSampleSettings settings = importer.defaultSampleSettings;
        settings.loadType = loadType;
        settings.compressionFormat = compression;
        importer.defaultSampleSettings = settings;
        importer.SaveAndReimport();
    }

    private static void ConfigureScene(string scenePath)
    {
        var scene = EditorSceneManager.OpenScene(scenePath);
        AudioManager[] managers = Object.FindObjectsOfType<AudioManager>(true);

        if (managers.Length == 0)
        {
            var audioObject = new GameObject("AudioManager");
            managers = new[] { audioObject.AddComponent<AudioManager>() };
        }

        foreach (AudioManager manager in managers)
            ConfigureAudioManager(manager);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void ConfigureAudioManager(AudioManager manager)
    {
        AudioSource bgmSource = GetOrCreateSource(manager.transform, "BGM_Source", true);
        AudioSource sfxSource = GetOrCreateSource(manager.transform, "SFX_Source", false);

        SerializedObject serialized = new(manager);
        serialized.FindProperty("_bgmSource").objectReferenceValue = bgmSource;
        serialized.FindProperty("_sfxSource").objectReferenceValue = sfxSource;

        AssignClipList(serialized.FindProperty("_bgmList"), BgmClips);
        AssignClipList(serialized.FindProperty("_sfxList"), SfxClips);

        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(manager);
    }

    private static AudioSource GetOrCreateSource(Transform parent, string name, bool loop)
    {
        Transform child = parent.Find(name);
        if (child == null)
        {
            var sourceObject = new GameObject(name);
            sourceObject.transform.SetParent(parent, false);
            child = sourceObject.transform;
        }

        AudioSource source = child.GetComponent<AudioSource>();
        if (source == null) source = child.gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = loop;
        EditorUtility.SetDirty(source);
        return source;
    }

    private static void AssignClipList(SerializedProperty listProperty, IReadOnlyList<(string Key, string Path)> clips)
    {
        listProperty.arraySize = clips.Count;

        for (int i = 0; i < clips.Count; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("key").stringValue = clips[i].Key;
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clips[i].Path);
            if (clip == null)
                Debug.LogWarning($"Could not load audio clip for key '{clips[i].Key}' at {clips[i].Path}");

            element.FindPropertyRelative("clip").objectReferenceValue = clip;
        }
    }
}
