using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxSource;

    public void RandomizeSfx(params AudioClip[] clips)
    {
        var randomIndex = Random.Range(0, clips.Length);
        sfxSource.PlayOneShot(clips[randomIndex]);
    }
}