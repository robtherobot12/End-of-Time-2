using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public EventChannel halftime;
    public AudioSource firsthalf;
    public AudioSource intro;
    public AudioSource secondhalf;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        halftime?._event.AddListener(() => PlayHalftime());
    }

    void PlayHalftime()
    {
        firsthalf.Stop();
        intro.Play();
        secondhalf.PlayDelayed(intro.clip.length);
    }
}
