// =============================================================================
// SoundManager.cs
// -----------------------------------------------------------------------------
// Central manager for all SFX and background music (BGM).
//
// HOW THE MAPPING WORKS (read this before adding sounds):
//   The `sfxEntries` / `bgmEntries` lists are filled in *manually via the
//   Inspector*. There is NO automatic folder scanning, no Resources.LoadAll and
//   no filename matching. This is intentional: our SFX packs name their files
//   completely differently (e.g. "69_Enemy_death_01.wav",
//   "JDSherbert - Ultimate UI SFX Pack - Cancel - 1.mp3", "Masc Jump.mp3").
//
//   Whenever a new sound is added (from ANY pack, ANY naming style):
//     1. Add a new entry to the list in the Inspector.
//     2. Type the desired `key` (e.g. "jump", "enemy_death").
//     3. Drag the AudioClip(s) into that entry's `clips` field.
//   No renaming, no code changes required.
//
// A single `key` can hold several clips (variants) — one is chosen at random
// each time it plays, so the same sound doesn't repeat identically.
//
// EXAMPLE USAGE (call from any script):
//     SoundManager.Instance.PlaySFX("jump");
//     SoundManager.Instance.PlaySFX("enemy_attack");
//     SoundManager.Instance.PlayBGM("bgm_dungeon");
// =============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// One mapping entry: a gameplay event key linked to one or more AudioClips.
[System.Serializable]
public class SoundEntry
{
    public string key;          // e.g. "jump", "attack", "enemy_death"
    public AudioClip[] clips;   // one or more variants; random one is played
}

public class SoundManager : MonoBehaviour
{
    // Global access point (singleton).
    public static SoundManager Instance { get; private set; }

    [Header("SFX Mapping (filled in the Inspector)")]
    public List<SoundEntry> sfxEntries = new List<SoundEntry>();

    [Header("BGM Mapping (filled in the Inspector)")]
    public List<SoundEntry> bgmEntries = new List<SoundEntry>();

    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float bgmVolume = 1f;
    [SerializeField] private float bgmFadeDuration = 0.5f; // seconds; 0 = instant

    // Runtime lookup tables built from the lists above (fast, no per-call loop).
    private Dictionary<string, AudioClip[]> sfxMap;
    private Dictionary<string, AudioClip[]> bgmMap;

    // Two dedicated audio sources: one looping for BGM, one for overlapping SFX.
    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private string currentBgmKey;       // which BGM key is currently playing
    private Coroutine bgmFadeRoutine;   // active fade, so we can cancel/replace it

    // ------------------------------------------------------------------ Setup
    private void Awake()
    {
        // Standard singleton guard: keep the first one, destroy any duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // survive scene loads

        // Auto-create the audio sources so no manual component setup is needed.
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;

        // Build the fast lookup tables once.
        sfxMap = BuildMap(sfxEntries, "SFX");
        bgmMap = BuildMap(bgmEntries, "BGM");

        PlayBGM("bgm_dungeon");   // <-- thêm dòng này để nhạc tự chạy khi game khởi động

    }

    // Turn an Inspector list into a dictionary, warning about mistakes early.
    private Dictionary<string, AudioClip[]> BuildMap(List<SoundEntry> entries, string label)
    {
        var map = new Dictionary<string, AudioClip[]>();

        foreach (SoundEntry entry in entries)
        {
            // Skip blank keys or entries with no clips assigned yet.
            if (entry == null || string.IsNullOrEmpty(entry.key))
            {
                continue;
            }

            if (entry.clips == null || entry.clips.Length == 0)
            {
                Debug.LogWarning($"[SoundManager] {label} key '{entry.key}' has no clips assigned.");
                continue;
            }

            // Catch duplicate keys (a common copy/paste typo) before they confuse anyone.
            if (map.ContainsKey(entry.key))
            {
                Debug.LogWarning($"[SoundManager] Duplicate {label} key '{entry.key}' — only the first one is used.");
                continue;
            }

            map.Add(entry.key, entry.clips);
        }

        return map;
    }

    // ------------------------------------------------------------------- SFX
    // Play an SFX by key (uses default SFX volume).
    public void PlaySFX(string key)
    {
        AudioClip clip = GetSfxClip(key);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    // Play an SFX by key with a per-call volume multiplier.
    public void PlaySFX(string key, float volume)
    {
        AudioClip clip = GetSfxClip(key);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume) * sfxVolume);
        }
    }

    // Look up a key and pick one clip (random if there are several).
    private AudioClip GetSfxClip(string key)
    {
        if (!sfxMap.TryGetValue(key, out AudioClip[] clips))
        {
            Debug.LogWarning($"[SoundManager] SFX key not found: {key}");
            return null;
        }

        if (clips.Length == 1)
        {
            return clips[0];
        }

        return clips[Random.Range(0, clips.Length)]; // random variant
    }

    // ------------------------------------------------------------------- BGM
    // Play a background music track by key. Only one BGM plays at a time.
    public void PlayBGM(string key, bool loop = true)
    {
        // Already playing this track — do nothing.
        if (key == currentBgmKey && bgmSource.isPlaying)
        {
            return;
        }

        if (!bgmMap.TryGetValue(key, out AudioClip[] clips))
        {
            Debug.LogWarning($"[SoundManager] BGM key not found: {key}");
            return;
        }

        AudioClip clip = clips.Length == 1 ? clips[0] : clips[Random.Range(0, clips.Length)];
        currentBgmKey = key;

        // Crossfade: fade the old track out, swap clip, fade the new one in.
        StartFade(clip, loop);
    }

    // Stop the current BGM (with a quick fade out).
    public void StopBGM()
    {
        currentBgmKey = null;
        StartFade(null, false);
    }

    // Start (or restart) the fade coroutine toward the given clip.
    private void StartFade(AudioClip nextClip, bool loop)
    {
        if (bgmFadeRoutine != null)
        {
            StopCoroutine(bgmFadeRoutine);
        }

        bgmFadeRoutine = StartCoroutine(FadeBGM(nextClip, loop));
    }

    // Fades the current track out, switches to nextClip (if any), fades it in.
    private IEnumerator FadeBGM(AudioClip nextClip, bool loop)
    {
        // ---- Fade out whatever is currently playing ----
        if (bgmSource.isPlaying && bgmFadeDuration > 0f)
        {
            float startVolume = bgmSource.volume;
            for (float t = 0f; t < bgmFadeDuration; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / bgmFadeDuration);
                yield return null;
            }
        }

        bgmSource.Stop();

        // Nothing to play next (StopBGM) — we're done.
        if (nextClip == null)
        {
            bgmSource.volume = bgmVolume;
            bgmFadeRoutine = null;
            yield break;
        }

        // ---- Start and fade in the new track ----
        bgmSource.clip = nextClip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmFadeDuration > 0f ? 0f : bgmVolume;
        bgmSource.Play();

        if (bgmFadeDuration > 0f)
        {
            for (float t = 0f; t < bgmFadeDuration; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t / bgmFadeDuration);
                yield return null;
            }
        }

        bgmSource.volume = bgmVolume;
        bgmFadeRoutine = null;
    }

    // --------------------------------------------------------------- Volumes
    // For a future Settings menu. Values are clamped to 0..1.
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);

        // Don't fight an in-progress fade; otherwise apply immediately.
        if (bgmFadeRoutine == null)
        {
            bgmSource.volume = bgmVolume;
        }
    }
}
