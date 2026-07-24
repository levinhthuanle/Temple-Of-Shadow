using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// PlayMode tests for the new enemies (Bomber, HybridBoss, FlyingBat).
//
// Why reflection: the game scripts live in the default "Assembly-CSharp" (no asmdef),
// which a test assembly cannot reference directly. So we reach the game types by name
// via reflection instead of restructuring the project into asmdefs.
//
// Each test spawns the real enemy prefab next to a minimal Player stub (a GameObject
// carrying the REAL PlayerHealth + a DamagePopupSpawner so PlayerHealth.TakeDamage
// doesn't NRE), forces deterministic ranges/cooldowns on the enemy, lets the play loop
// run, and asserts the player's HP dropped (i.e. the attack actually fired and landed).
public class EnemyCombatTests
{
    const string BomberPath = "Assets/_Project/Prefabs/Enemies/Bomber.prefab";
    const string BossPath   = "Assets/_Project/Prefabs/Enemies/HybridBoss.prefab";
    const string BatPath    = "Assets/_Project/Prefabs/Enemies/FlyingBat.prefab";
    const string PopupPath  = "Assets/_Project/Prefabs/Others/DamagePopup.prefab";
    const string ManagerPath = "Assets/_Project/Prefabs/System&Manager/Manager.prefab";

    GameObject enemy;
    GameObject player;
    Component playerHealth;
    Component soundManager;

    [TearDown]
    public void TearDown()
    {
        if (enemy != null) UnityEngine.Object.Destroy(enemy);
        if (player != null) UnityEngine.Object.Destroy(player);
        if (soundManager != null) UnityEngine.Object.Destroy(soundManager.gameObject);
    }

    // ---------------- reflection helpers ----------------
    static Type GameType(string name)
    {
        Type t = Type.GetType(name + ", Assembly-CSharp");
        Assert.NotNull(t, $"Game type '{name}' not found in Assembly-CSharp");
        return t;
    }

    static void SetField(object obj, string field, object value)
    {
        FieldInfo f = obj.GetType().GetField(field,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.NotNull(f, $"Field '{field}' not found on {obj.GetType().Name}");
        f.SetValue(obj, value);
    }

    static object GetField(object obj, string field)
    {
        FieldInfo f = obj.GetType().GetField(field,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.NotNull(f, $"Field '{field}' not found on {obj.GetType().Name}");
        return f.GetValue(obj);
    }

    static object Call(object obj, string method, params object[] args)
    {
        MethodInfo m = null;
        MethodInfo[] methods = obj.GetType().GetMethods(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (MethodInfo candidate in methods)
        {
            if (candidate.Name != method)
            {
                continue;
            }

            ParameterInfo[] parameters = candidate.GetParameters();
            if (parameters.Length != args.Length)
            {
                continue;
            }

            bool matches = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (args[i] != null && !parameters[i].ParameterType.IsInstanceOfType(args[i]))
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                m = candidate;
                break;
            }
        }

        Assert.NotNull(m, $"Method '{method}' not found on {obj.GetType().Name}");
        return m.Invoke(obj, args);
    }

    static GameObject LoadPrefab(string path)
    {
        GameObject prefab = null;
#if UNITY_EDITOR
        prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
#endif
        Assert.NotNull(prefab, $"Prefab not found at {path}");
        return prefab;
    }

    // ---------------- fixtures ----------------
    // Minimal, valid Player: real PlayerHealth (+ DamagePopupSpawner it needs), a big
    // collider on the Player layer so enemy OverlapCircle/projectiles connect.
    GameObject MakePlayer(Vector3 pos)
    {
        var go = new GameObject("TestPlayer");
        go.transform.position = pos;
        go.layer = LayerMask.NameToLayer("Player");

        // Match the REAL player's collider width (~4.1) so tests reflect real resting distances
        // (a narrow stub previously hid the boss's oversized-collider bug).
        var box = go.AddComponent<BoxCollider2D>();
        box.size = new Vector2(4.1f, 5.6f);
        box.offset = new Vector2(0f, 2.9f);

        // DamagePopupSpawner + its prefab, so PlayerHealth.TakeDamage doesn't NRE.
        var spawner = go.AddComponent(GameType("DamagePopupSpawner"));
        GameObject popupPrefab = LoadPrefab(PopupPath);
        SetField(spawner, "damagePopupPrefab", popupPrefab.GetComponent(GameType("DamagePopup")));

        // Real PlayerHealth (Awake sets currentHp = maxHp).
        playerHealth = go.AddComponent(GameType("PlayerHealth"));
        SetField(playerHealth, "damagePopupSpawner", spawner);
        SetField(playerHealth, "maxHp", 20);
        SetField(playerHealth, "currentHp", 20);

        player = go;
        return go;
    }

    GameObject SpawnEnemy(string path, Vector3 pos)
    {
        enemy = UnityEngine.Object.Instantiate(LoadPrefab(path), pos, Quaternion.identity);
        enemy.transform.position = pos;
        var rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0f; // keep it from falling in the empty test scene
        return enemy;
    }

    int Hp() { return (int)Call(playerHealth, "GetCurrentHp"); }

    IEnumerator WaitForHpDropOrTimeout(int startHp, float timeout)
    {
        float t = 0f;
        while (t < timeout)
        {
            if (Hp() < startHp) yield break;
            yield return null;
            t += Time.deltaTime;
        }
    }

    // ---------------- tests ----------------
    [UnityTest]
    public IEnumerator SoundManager_AllMappedEvents_AreReadyToPlay()
    {
        string[] sfxKeys =
        {
            "jump", "footstep", "landing",
            "player_slash", "player_kick", "player_throw", "player_hit", "hurt", "player_death",
            "enemy_hit", "enemy_hurt", "enemy_death", "enemy_melee_attack", "enemy_ranged_attack",
            "boss_melee_attack", "boss_ranged_attack", "bomber_explosion",
            "coin_pickup", "item_pickup", "item_drop", "click_button", "pause", "unpause"
        };
        string[] bgmKeys =
        {
            "bgm_menu", "bgm_dungeon", "bgm_level2", "bgm_boss", "bgm_shop"
        };

        enemy = UnityEngine.Object.Instantiate(LoadPrefab(ManagerPath));
        // SoundManager detaches itself from Manager during Awake so it can persist
        // across scene loads. Find that detached runtime component after one frame.
        yield return null;
        soundManager = UnityEngine.Object.FindFirstObjectByType(GameType("SoundManager")) as Component;
        Assert.NotNull(soundManager, "Manager prefab is missing SoundManager.");

        foreach (string key in sfxKeys)
        {
            Assert.IsTrue((bool)Call(soundManager, "HasSFX", key), $"SFX key is not ready: {key}");
        }

        foreach (string key in bgmKeys)
        {
            Assert.IsTrue((bool)Call(soundManager, "HasBGM", key), $"BGM key is not ready: {key}");
        }

        // Exercise both playback paths. A missing clip/source would fail or log an exception.
        Call(soundManager, "PlaySFX", "click_button");
        Call(soundManager, "PlayBGM", "bgm_menu", true);
        yield return null;
        Call(soundManager, "StopBGM");
    }

    [UnityTest]
    public IEnumerator Bomber_Explodes_And_Damages_Player()
    {
        // Player placed beyond contact range so the bomber must APPROACH and fuse at the boundary
        // (this is where the real explosionRadius vs fuseDistance mismatch bites).
        MakePlayer(new Vector3(8f, 0f, 0f));
        SpawnEnemy(BomberPath, Vector3.zero);

        var ai = enemy.GetComponent(GameType("BomberEnemyAI"));
        Assert.NotNull(ai, "Bomber prefab is missing BomberEnemyAI");
        // NOTE: fuseDistance AND explosionRadius are intentionally NOT overridden — the test
        // validates the prefab's real values (the fields that were the "never damages" bugs).
        SetField(ai, "detectRange", 20f);
        SetField(ai, "fuseTime", 0.2f);
        SetField(ai, "explosionDamage", 3);

        int startHp = Hp();
        yield return WaitForHpDropOrTimeout(startHp, 5f);

        Assert.Less(Hp(), startHp, "Player HP should drop from the bomber's explosion.");

        // Bomber self-destructs (EnemyHealth.DieEnemy -> Destroy after destroyDelay).
        float t = 0f;
        while (enemy != null && t < 3f) { yield return null; t += Time.deltaTime; }
        Assert.IsTrue(enemy == null, "Bomber should destroy itself after exploding.");
    }

    [UnityTest]
    public IEnumerator Boss_Melee_Damages_Player()
    {
        MakePlayer(new Vector3(3f, 0f, 0f));
        SpawnEnemy(BossPath, Vector3.zero);

        var boss = enemy.GetComponent(GameType("BossController"));
        Assert.NotNull(boss, "HybridBoss prefab is missing BossController");
        // NOTE: meleeRange is intentionally NOT overridden — the test validates the
        // prefab's real meleeRange (the field that was the "never attacks" bug).
        SetField(boss, "detectRange", 30f);
        SetField(boss, "meleeRadius", 4f);
        SetField(boss, "meleeCooldown", 0.3f);

        int startHp = Hp();
        yield return WaitForHpDropOrTimeout(startHp, 4f);

        Assert.Less(Hp(), startHp, "Player HP should drop from the boss's melee swing (phase 1, full HP).");
    }

    [UnityTest]
    public IEnumerator Boss_Ranged_Damages_Player()
    {
        MakePlayer(new Vector3(8f, 0f, 0f));
        SpawnEnemy(BossPath, Vector3.zero);

        var boss = enemy.GetComponent(GameType("BossController"));
        var health = enemy.GetComponent(GameType("EnemyHealth"));
        Assert.NotNull(boss, "HybridBoss prefab is missing BossController");

        Assert.IsNotNull(GetField(boss, "projectilePrefab"),
            "Boss 'Projectile Prefab' is not assigned on the prefab — ranged phase cannot fire.");

        SetField(boss, "detectRange", 40f);
        SetField(boss, "keepDistance", 8f);
        SetField(boss, "rangedCooldown", 0.5f);
        SetField(boss, "volleyCount", 4);

        // Drop the boss into the ranged phase (< 66% HP). maxHp is ~40 -> deal 20 => 50%.
        int maxHp = (int)Call(health, "GetMaxHp");
        Call(health, "TakeDamage", Mathf.RoundToInt(maxHp * 0.5f));

        int startHp = Hp();
        yield return WaitForHpDropOrTimeout(startHp, 6f);

        Assert.Less(Hp(), startHp, "Player HP should drop from the boss's ranged volley (phase 2).");
    }

    [UnityTest]
    public IEnumerator FlyingBat_Contact_Damages_Player()
    {
        MakePlayer(new Vector3(3f, 0f, 0f));
        SpawnEnemy(BatPath, Vector3.zero);

        var bat = enemy.GetComponent(GameType("FlyingEnemyAI"));
        Assert.NotNull(bat, "FlyingBat prefab is missing FlyingEnemyAI");
        // NOTE: contactRange is intentionally NOT overridden — the test validates the
        // prefab's real contactRange.
        SetField(bat, "detectRange", 20f);
        SetField(bat, "moveSpeed", 6f);

        int startHp = Hp();
        yield return WaitForHpDropOrTimeout(startHp, 5f);

        Assert.Less(Hp(), startHp, "Player HP should drop from the flying bat's contact damage.");
    }
}
