using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ConfigureProjectAudio
{
    private const string AudioRoot = "Assets/_Project/Audio/";
    private const string Rpg = AudioRoot + "RPG_Essentials_Free/";
    private const string Jump = AudioRoot + "Jump SFX AmbroggioMusic/";
    private const string Dialogue = AudioRoot +
        "Super Dialogue Audio Pack v1/Super Dialogue Audio Pack v1/Step 2 - Audio Files/9 - Grunting/Male/";
    private const string Music = AudioRoot +
        "xDeviruchi - 16 bit Fantasy & Adventure (2025)/xDeviruchi - 16 bit Fantasy & Adventure (2025)/Loopable + one shots/ogg/";
    private const string Monster = AudioRoot + "External/OGA_MonsterGrowls_CC0/";

    private static readonly Dictionary<string, string[]> Sfx = new()
    {
        ["jump"] = new[]
        {
            Rpg + "12_Player_Movement_SFX/30_Jump_03.wav"
        },
        ["footstep"] = new[]
        {
            Rpg + "12_Player_Movement_SFX/03_Step_grass_03.wav",
            Rpg + "12_Player_Movement_SFX/08_Step_rock_02.wav",
            Rpg + "12_Player_Movement_SFX/12_Step_wood_03.wav"
        },
        ["landing"] = new[] { Rpg + "12_Player_Movement_SFX/45_Landing_01.wav" },
        ["player_slash"] = new[]
        {
            Rpg + "10_Battle_SFX/22_Slash_04.wav"
        },
        ["player_kick"] = new[]
        {
            Rpg + "10_Battle_SFX/15_Impact_flesh_02.wav",
            Rpg + "10_Battle_SFX/77_flesh_02.wav"
        },
        ["player_throw"] = new[]
        {
            Rpg + "8_Atk_Magic_SFX/46_Poison_01.wav",
            Rpg + "8_Atk_Magic_SFX/45_Charge_05.wav"
        },
        ["player_hit"] = new[]
        {
            Rpg + "12_Player_Movement_SFX/61_Hit_03.wav",
            Rpg + "10_Battle_SFX/15_Impact_flesh_02.wav"
        },
        ["hurt"] = new[]
        {
            Dialogue + "Sean Lenhart/grunting_1_sean.wav",
            Dialogue + "Sean Lenhart/grunting_3_sean.wav",
            Dialogue + "Sean Lenhart/grunting_6_sean.wav"
        },
        ["player_death"] = new[]
        {
            Dialogue + "Ian Lampert/grunting_2_ian.wav"
        },
        ["enemy_hit"] = new[]
        {
            Rpg + "10_Battle_SFX/77_flesh_02.wav",
            Rpg + "10_Battle_SFX/15_Impact_flesh_02.wav"
        },
        ["enemy_hurt"] = new[]
        {
            Monster + "monster.1.ogg",
            Monster + "monster.2.ogg",
            Monster + "monster.4.ogg",
            Monster + "monster.5.ogg"
        },
        ["enemy_death"] = new[]
        {
            Monster + "monster.8.ogg",
            Monster + "monster.11.ogg",
            Rpg + "10_Battle_SFX/69_Enemy_death_01.wav"
        },
        ["enemy_melee_attack"] = new[]
        {
            Rpg + "10_Battle_SFX/03_Claw_03.wav",
            Rpg + "10_Battle_SFX/08_Bite_04.wav"
        },
        ["enemy_ranged_attack"] = new[]
        {
            Rpg + "8_Atk_Magic_SFX/04_Fire_explosion_04_medium.wav",
            Rpg + "8_Atk_Magic_SFX/46_Poison_01.wav"
        },
        ["boss_melee_attack"] = new[]
        {
            Rpg + "10_Battle_SFX/22_Slash_04.wav",
            Rpg + "10_Battle_SFX/03_Claw_03.wav"
        },
        ["boss_ranged_attack"] = new[]
        {
            Rpg + "8_Atk_Magic_SFX/30_Earth_02.wav",
            Rpg + "8_Atk_Magic_SFX/04_Fire_explosion_04_medium.wav",
            Rpg + "8_Buffs_Heals_SFX/21_Debuff_01.wav"
        },
        ["bomber_explosion"] = new[]
        {
            Rpg + "8_Atk_Magic_SFX/04_Fire_explosion_04_medium.wav",
            Rpg + "8_Atk_Magic_SFX/30_Earth_02.wav"
        },
        ["coin_pickup"] = new[] { Rpg + "10_UI_Menu_SFX/079_Buy_sell_01.wav" },
        ["item_pickup"] = new[]
        {
            Rpg + "10_UI_Menu_SFX/051_use_item_01.wav",
            Rpg + "10_UI_Menu_SFX/070_Equip_10.wav"
        },
        ["item_drop"] = new[] { Rpg + "10_UI_Menu_SFX/071_Unequip_01.wav" },
        ["click_button"] = new[] { Rpg + "10_UI_Menu_SFX/013_Confirm_03.wav" },
        ["pause"] = new[] { Rpg + "10_UI_Menu_SFX/092_Pause_04.wav" },
        ["unpause"] = new[] { Rpg + "10_UI_Menu_SFX/098_Unpause_04.wav" }
    };

    private static readonly Dictionary<string, string[]> Bgm = new()
    {
        ["bgm_menu"] = new[] { Music + "02 - Title Theme.ogg" },
        ["bgm_dungeon"] = new[] { Music + "10 - Lost Shrine.ogg" },
        ["bgm_level2"] = new[] { Music + "04 - Silent Forest.ogg" },
        ["bgm_boss"] = new[] { Music + "17 - Decisive Battle 2 - The Calamity.ogg" },
        ["bgm_shop"] = new[] { Music + "08 - Shop.ogg" }
    };

    private static readonly Dictionary<string, string> SceneBgm = new()
    {
        ["MainMenu"] = "bgm_menu",
        ["SaveSlotSelect"] = "bgm_menu",
        ["CharacterSelect"] = "bgm_menu",
        ["Player2"] = "bgm_menu",
        ["SampleScene"] = "bgm_dungeon",
        ["Level1"] = "bgm_dungeon",
        ["Level 2"] = "bgm_level2",
        ["Level 3"] = "bgm_boss",
        ["Shop"] = "bgm_shop"
    };

    [MenuItem("Tools/Temple Of Shadow/Configure Project Audio")]
    public static void Run()
    {
        ConfigurePrefab("Assets/_Project/Prefabs/System&Manager/Manager.prefab");
        ConfigurePrefab("Assets/_Project/Prefabs/System&Manager/SoundManager.prefab");
        ConfigureScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[ProjectAudio] Configured {Sfx.Count} SFX keys, {Bgm.Count} BGM keys and {SceneBgm.Count} scenes.");
    }

    private static void ConfigurePrefab(string path)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            SoundManager manager = root.GetComponentInChildren<SoundManager>(true);
            if (manager == null)
                throw new InvalidOperationException($"SoundManager missing in {path}");

            SerializedObject serialized = new(manager);
            WriteEntries(serialized.FindProperty("sfxEntries"), Sfx, false);
            WriteEntries(serialized.FindProperty("bgmEntries"), Bgm, true);
            serialized.FindProperty("sfxVolume").floatValue = 0.68f;
            serialized.FindProperty("bgmVolume").floatValue = 0.5f;
            serialized.FindProperty("bgmFadeDuration").floatValue = 0.5f;
            serialized.FindProperty("warmSfxCutoff").floatValue = 8500f;
            serialized.FindProperty("bgmCutoff").floatValue = 12000f;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Debug.Log($"[ProjectAudio] Updated {path}");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void WriteEntries(
        SerializedProperty list,
        Dictionary<string, string[]> entries,
        bool streaming)
    {
        list.arraySize = entries.Count;
        int index = 0;

        foreach (KeyValuePair<string, string[]> pair in entries)
        {
            SerializedProperty entry = list.GetArrayElementAtIndex(index++);
            entry.FindPropertyRelative("key").stringValue = pair.Key;
            SerializedProperty clips = entry.FindPropertyRelative("clips");
            clips.arraySize = pair.Value.Length;

            for (int i = 0; i < pair.Value.Length; i++)
            {
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(pair.Value[i]);
                if (clip == null)
                    throw new InvalidOperationException($"Audio clip not found: {pair.Value[i]}");

                clips.GetArrayElementAtIndex(i).objectReferenceValue = clip;
                ConfigureImporter(pair.Value[i], streaming);
            }
        }
    }

    private static void ConfigureImporter(string path, bool streaming)
    {
        if (AssetImporter.GetAtPath(path) is not AudioImporter importer)
            return;

        AudioImporterSampleSettings settings = importer.defaultSampleSettings;
        settings.loadType = streaming ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
        settings.compressionFormat = AudioCompressionFormat.Vorbis;
        settings.quality = streaming ? 0.75f : 0.7f;
        settings.preloadAudioData = !streaming;
        importer.defaultSampleSettings = settings;
        importer.loadInBackground = streaming;
        importer.SaveAndReimport();
    }

    private static void ConfigureScenes()
    {
        foreach (KeyValuePair<string, string> pair in SceneBgm)
        {
            string path = $"Assets/_Project/Scenes/{pair.Key}.unity";
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            SceneMusic music = UnityEngine.Object.FindFirstObjectByType<SceneMusic>();

            if (music == null)
            {
                GameObject go = new("[Scene Music]");
                music = go.AddComponent<SceneMusic>();
            }

            SerializedObject serialized = new(music);
            serialized.FindProperty("bgmKey").stringValue = pair.Value;
            serialized.FindProperty("loop").boolValue = true;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[ProjectAudio] {pair.Key} -> {pair.Value}");
        }
    }
}
