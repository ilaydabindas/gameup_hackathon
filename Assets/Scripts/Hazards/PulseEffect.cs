using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    [Header("Büyüme / Küçülme Ayarları")]
    [Tooltip("En küçük boyut çarpanı")]
    public float minScale = 0.85f;

    [Tooltip("En büyük boyut çarpanı")]
    public float maxScale = 1.25f;

    [Tooltip("Büyüyüp küçülme hızı")]
    public float pulseSpeed = 4f;

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        // Sinüs dalgası -1 ile 1 arasında değer üretir, bunu 0 ile 1 arasına çekiyoruz:
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        // minScale ile maxScale arasında pürüzsüz geçiş:
        float currentScaleFactor = Mathf.Lerp(minScale, maxScale, t);

        transform.localScale = baseScale * currentScaleFactor;
    }
}