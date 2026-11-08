using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public EventChannel click;
    public EventChannel purchaseChannel;
    public EventChannel bhDmgChannel;
    public EventChannel bhDeathChannel;
    public EventChannel bhSpawnChannel;
    public EventChannel clickClock;

    public EventChannel halftime;
    public AudioSource firsthalf;
    public AudioSource intro;
    public AudioSource secondhalf;
    public AudioSource sfx;
    public AudioSource bhAmbience;

    public AudioClip clickSound;
    public AudioClip bhDamage;
    public AudioClip bhDeath;
    public AudioClip purchase;
    public AudioClip bhSpawn;
    public AudioClip clickClockSound;
    public AudioClip clickClockSound2;

    private int bhCount = 0;
    private bool tik = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        halftime?._event.AddListener(() => PlayHalftime());
        click?._event.AddListener(() => sfx.PlayOneShot(clickSound));
        clickClock?._event.AddListener(() =>
        {
            if (tik) sfx.PlayOneShot(clickClockSound);
            else sfx.PlayOneShot(clickClockSound);
            tik = !tik;
        });
        purchaseChannel?._event.AddListener(() => sfx.PlayOneShot(purchase));
        bhDmgChannel?._event.AddListener(() => sfx.PlayOneShot(bhDamage));
        bhDeathChannel?._event.AddListener(() =>
        {
            sfx.PlayOneShot(bhDeath);
            bhCount--;
            if (bhCount <= 0)
            {
                bhAmbience.Stop();
            }
        });

        bhSpawnChannel?._event.AddListener(() =>
        {
            sfx.PlayOneShot(bhSpawn);
            bhCount++;
            bhAmbience.Play();
        });


    }

    void PlayHalftime()
    {
        firsthalf.Stop();
        intro.Play();
        secondhalf.PlayDelayed(intro.clip.length);
    }
}
