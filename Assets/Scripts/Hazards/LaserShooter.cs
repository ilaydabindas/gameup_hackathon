using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LaserShooter : MonoBehaviour
{
    public static LaserShooter Instance { get; private set; }

    [Header("Baglantilar")]
<<<<<<< Updated upstream
    [SerializeField] private RectTransform turretFirePoint; // Isinin cikacagi kule noktasi
    [SerializeField] private RectTransform laserBeamUI;      // Cizgi gorevi gorecek UI Image
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip laserHitSound;        // Vurma sesi (varsa)

    [Header("Lazer Parametreleri")]
    [SerializeField] private float beamThickness = 6f;       // Isin kalinligi (px)
    [SerializeField] private float laserDuration = 0.12f;    // Isinin gorunme suresi
=======
    [SerializeField] private RectTransform turretFirePoint; // Isinin baslayacagi namlu noktasi
    [SerializeField] private RectTransform laserBeamUI;      // Cizgi gorevi gorecek UI Image
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip laserHitSound;        // Vurma sesi (opsiyonel)

    [Header("Lazer Parametreleri")]
    [SerializeField] private float beamThickness = 6f;       // Isin kalinligi (px)
    [SerializeField] private float laserDuration = 0.12f;    // Isinin ekranda kalma suresi
>>>>>>> Stashed changes
    [SerializeField] private Color laserColor = new Color32(56, 189, 248, 255); // Neon Turkuaz

    private Image _beamImage;
    private Coroutine _laserRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (laserBeamUI != null)
        {
            _beamImage = laserBeamUI.GetComponent<Image>();
            laserBeamUI.gameObject.SetActive(false);
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void FireAt(HazardOrb target)
    {
        if (turretFirePoint == null || laserBeamUI == null || target == null) return;

        RectTransform targetRT = (RectTransform)target.transform;
        Vector3 startWorldPos = turretFirePoint.position;
        Vector3 endWorldPos = targetRT.position;

<<<<<<< Updated upstream
        // Lazeri ac ve hedefle turret arasina hizala
        PositionLaserBeam(startWorldPos, endWorldPos);

        // Hedefe hasar ver
        target.OnHit();

        // Ses cal
        PlayLaserSound();

        // Lazeri anlik parlatip kapat
=======
        PositionLaserBeam(startWorldPos, endWorldPos);
        target.OnHit();
        PlayLaserSound();

>>>>>>> Stashed changes
        if (_laserRoutine != null) StopCoroutine(_laserRoutine);
        _laserRoutine = StartCoroutine(LaserFlashRoutine());
    }

    private void PositionLaserBeam(Vector3 worldStart, Vector3 worldEnd)
    {
        laserBeamUI.gameObject.SetActive(true);

<<<<<<< Updated upstream
        // Canvas ortaminda iki nokta arasi yon ve mesafe hesabi
=======
>>>>>>> Stashed changes
        Vector3 dir = worldEnd - worldStart;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        laserBeamUI.position = worldStart;
        laserBeamUI.rotation = Quaternion.Euler(0, 0, angle);
<<<<<<< Updated upstream
        laserBeamUI.pivot = new Vector2(0f, 0.5f); // Sol orta noktadan uzasin
=======
        laserBeamUI.pivot = new Vector2(0f, 0.5f); // Soldan saga uzasin
>>>>>>> Stashed changes
        laserBeamUI.sizeDelta = new Vector2(distance, beamThickness);

        if (_beamImage != null)
        {
            _beamImage.color = laserColor;
        }
    }

    private IEnumerator LaserFlashRoutine()
    {
        float elapsed = 0f;
        Color initialColor = laserColor;

        while (elapsed < laserDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / laserDuration);
            if (_beamImage != null)
            {
                Color c = initialColor;
                c.a = alpha;
                _beamImage.color = c;
            }
            yield return null;
        }

        laserBeamUI.gameObject.SetActive(false);
    }

    private void PlayLaserSound()
    {
        if (laserHitSound != null)
        {
            audioSource.PlayOneShot(laserHitSound);
        }
        else
        {
<<<<<<< Updated upstream
            // Eger AudioClip atanmadiysa anlik sentetik lazer sesi olusturur
=======
>>>>>>> Stashed changes
            AudioClip synthLaser = CreateProceduralLaserClip();
            audioSource.PlayOneShot(synthLaser);
        }
    }

<<<<<<< Updated upstream
    // Harici dosya yuklemeden calisabilen dinamik frekans kaymali lazer "pew" sesi
=======
>>>>>>> Stashed changes
    private AudioClip CreateProceduralLaserClip()
    {
        int sampleRate = 44100;
        float duration = 0.12f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
<<<<<<< Updated upstream
            float freq = Mathf.Lerp(900f, 150f, t / duration); // Yuksekten alcaga inen lazer sinyali
=======
            float freq = Mathf.Lerp(900f, 150f, t / duration);
>>>>>>> Stashed changes
            samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * (1f - (t / duration));
        }

        AudioClip clip = AudioClip.Create("LaserSFX", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
