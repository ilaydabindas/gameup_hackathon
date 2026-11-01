using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Ses Efektleri")]
    public AudioClip swipeSound;
    public AudioClip laserSound;
    public AudioClip powerUpSound;
    public AudioClip badBuzzSound;

    [SerializeField] private AudioSource sfxSource;
    public bool isMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySwipe() => PlayOneShot(swipeSound);
    public void PlayLaser() => PlayOneShot(laserSound);
    public void PlayPowerUp() => PlayOneShot(powerUpSound);
    public void PlayBadBuzz() => PlayOneShot(badBuzzSound);

    private void PlayOneShot(AudioClip clip)
    {
        if (!isMuted && clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
    }
}