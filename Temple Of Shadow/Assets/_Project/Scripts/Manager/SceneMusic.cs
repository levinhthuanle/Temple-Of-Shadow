using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private string bgmKey;
    [SerializeField] private bool loop = true;

private System.Collections.IEnumerator Start()
    {
        if (string.IsNullOrWhiteSpace(bgmKey))
        {
            yield break;
        }

        while (SoundManager.Instance == null)
        {
            yield return null;
        }

        SoundManager.Instance.PlayBGM(bgmKey, loop);
    }
}
