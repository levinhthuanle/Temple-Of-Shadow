using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class AuditProjectAudio
{
    private const string ManagerPath =
        "Assets/_Project/Prefabs/System&Manager/Manager.prefab";

    [MenuItem("Tools/Temple Of Shadow/Audit Project Audio")]
    public static void Run()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ManagerPath);
        SoundManager manager = prefab != null
            ? prefab.GetComponentInChildren<SoundManager>(true)
            : null;

        if (manager == null)
        {
            throw new InvalidOperationException($"SoundManager missing from {ManagerPath}");
        }

        var report = new StringBuilder();
        report.AppendLine("# Temple Of Shadow — Audio Mapping & Technical Audit");
        report.AppendLine();
        report.AppendLine("Generated from the serialized `Manager.prefab` mapping.");
        report.AppendLine();
        report.AppendLine($"- SFX master volume: `{GetPrivateFloat(manager, "sfxVolume"):0.00}`");
        report.AppendLine($"- BGM master volume: `{GetPrivateFloat(manager, "bgmVolume"):0.00}`");
        report.AppendLine("- Peak/RMS are measured from decoded PCM samples.");
        report.AppendLine("- Estimated output peak includes the SoundManager master-volume multiplier.");
        report.AppendLine();

        AppendEntries(report, "SFX", manager.sfxEntries, GetPrivateFloat(manager, "sfxVolume"));
        AppendEntries(report, "BGM", manager.bgmEntries, GetPrivateFloat(manager, "bgmVolume"));

        string outputPath = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", "AudioMappingReport.md"));
        File.WriteAllText(outputPath, report.ToString(), new UTF8Encoding(false));
        Debug.Log($"[AudioAudit] Wrote {outputPath}");
    }

    private static void AppendEntries(
        StringBuilder report,
        string category,
        List<SoundEntry> entries,
        float masterVolume)
    {
        report.AppendLine($"## {category}");
        report.AppendLine();
        report.AppendLine("| Event key | Audio file | Duration | Peak | RMS | Est. output peak | Technical note |");
        report.AppendLine("|---|---|---:|---:|---:|---:|---|");

        foreach (SoundEntry entry in entries)
        {
            if (entry == null || entry.clips == null)
            {
                continue;
            }

            foreach (AudioClip originalClip in entry.clips)
            {
                if (originalClip == null)
                {
                    report.AppendLine($"| `{entry.key}` | **NULL** | — | — | — | — | Missing clip |");
                    continue;
                }

                string assetPath = AssetDatabase.GetAssetPath(originalClip);
                AudioStats stats = Analyze(assetPath);
                float outputPeak = stats.peakDb + LinearToDb(masterVolume);
                string note = Assess(stats, outputPeak, category);
                string link = assetPath.Replace(" ", "%20");

                report.AppendLine(
                    $"| `{entry.key}` | [{Escape(originalClip.name)}]({link}) | " +
                    $"{stats.duration:0.00}s | {FormatDb(stats.peakDb)} | {FormatDb(stats.rmsDb)} | " +
                    $"{FormatDb(outputPeak)} | {note} |");
            }
        }

        report.AppendLine();
    }

    private static AudioStats Analyze(string assetPath)
    {
        AudioImporter importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;
        AudioImporterSampleSettings originalSettings = default;
        bool temporarilyDecompressed = false;

        try
        {
            if (importer != null)
            {
                originalSettings = importer.defaultSampleSettings;
                if (originalSettings.loadType == AudioClipLoadType.Streaming)
                {
                    AudioImporterSampleSettings auditSettings = originalSettings;
                    auditSettings.loadType = AudioClipLoadType.DecompressOnLoad;
                    auditSettings.preloadAudioData = true;
                    importer.defaultSampleSettings = auditSettings;
                    importer.SaveAndReimport();
                    temporarilyDecompressed = true;
                }
            }

            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
            if (clip == null)
            {
                return AudioStats.Unavailable;
            }

            clip.LoadAudioData();
            const int framesPerChunk = 32768;
            float[] buffer = new float[framesPerChunk * Math.Max(1, clip.channels)];
            double sumSquares = 0d;
            long sampleCount = 0;
            float peak = 0f;

            for (int frameOffset = 0; frameOffset < clip.samples; frameOffset += framesPerChunk)
            {
                int frames = Math.Min(framesPerChunk, clip.samples - frameOffset);
                int values = frames * Math.Max(1, clip.channels);

                if (buffer.Length != values)
                {
                    buffer = new float[values];
                }

                if (!clip.GetData(buffer, frameOffset))
                {
                    return new AudioStats(clip.length, float.NaN, float.NaN);
                }

                for (int i = 0; i < values; i++)
                {
                    float magnitude = Mathf.Abs(buffer[i]);
                    if (magnitude > peak)
                    {
                        peak = magnitude;
                    }

                    sumSquares += buffer[i] * buffer[i];
                }

                sampleCount += values;
            }

            float rms = sampleCount > 0
                ? Mathf.Sqrt((float)(sumSquares / sampleCount))
                : 0f;
            return new AudioStats(clip.length, LinearToDb(peak), LinearToDb(rms));
        }
        finally
        {
            if (temporarilyDecompressed && importer != null)
            {
                importer.defaultSampleSettings = originalSettings;
                importer.SaveAndReimport();
            }
        }
    }

    private static string Assess(AudioStats stats, float outputPeak, string category)
    {
        if (float.IsNaN(stats.peakDb))
        {
            return "PCM data unavailable";
        }

        if (stats.peakDb >= -0.05f)
        {
            return "Source reaches digital full scale; monitor overlapping playback";
        }

        if (outputPeak > -0.5f)
        {
            return "Hot output; overlap may clip";
        }

        if (category == "SFX" && stats.rmsDb < -30f)
        {
            return "Quiet SFX; listen for audibility in combat";
        }

        if (category == "BGM" && outputPeak > -3f)
        {
            return "BGM may compete with SFX";
        }

        return "Level is technically reasonable";
    }

    private static float GetPrivateFloat(SoundManager manager, string fieldName)
    {
        var field = typeof(SoundManager).GetField(
            fieldName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic);
        return field != null ? (float)field.GetValue(manager) : 1f;
    }

    private static float LinearToDb(float value)
    {
        return value > 0f ? 20f * Mathf.Log10(value) : -120f;
    }

    private static string FormatDb(float value)
    {
        return float.IsNaN(value) ? "n/a" : $"{value:0.0} dBFS";
    }

    private static string Escape(string value)
    {
        return value.Replace("|", "\\|");
    }

    private readonly struct AudioStats
    {
        public static readonly AudioStats Unavailable =
            new(0f, float.NaN, float.NaN);

        public readonly float duration;
        public readonly float peakDb;
        public readonly float rmsDb;

        public AudioStats(float duration, float peakDb, float rmsDb)
        {
            this.duration = duration;
            this.peakDb = peakDb;
            this.rmsDb = rmsDb;
        }
    }
}
