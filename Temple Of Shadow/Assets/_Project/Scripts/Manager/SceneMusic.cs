using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private string bgmKey;
    [SerializeField] private bool loop = true;

    private void Start()
    {
        if (!string.IsNullOrWhiteSpace(bgmKey))
        {
            SoundManager.Instance?.PlayBGM(bgmKey, loop);
        }
    }
}
