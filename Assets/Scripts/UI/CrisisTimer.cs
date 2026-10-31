using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrisisTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float totalTime = 10f; // Krizin süresi (saniye)
    private float currentTime;
    private bool isRunning = false;

    [Header("UI References")]
    [SerializeField] private Image timerBarFill;     // Sürüklenecek dolum çubuğu (Fill Area içindeki Image)
    [SerializeField] private TextMeshProUGUI timerText; // Kalan süreyi gösteren yazı (isteğe bağlı)

    void Start()
    {
        // Kriz kartı açıldığı an süreyi başlatabiliriz veya dışarıdan tetikleyebiliriz
        StartTimer(totalTime);
    }

    void Update()
    {
        if (!isRunning) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            
            // Çubuğu güncelle (0 ile 1 arasında oranlar)
            if (timerBarFill != null)
            {
                timerBarFill.fillAmount = currentTime / totalTime;
            }

            // Metni güncelle
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(currentTime).ToString();
            }
        }
        else
        {
            TimeIsUp();
        }
    }

    public void StartTimer(float duration)
    {
        totalTime = duration;
        currentTime = duration;
        isRunning = true;
    }

    void TimeIsUp()
    {
        isRunning = false;
        Debug.Log("Süre bitti! Kriz cezası uygulanabilir.");
        // Buraya süre bittiğinde otomatik olarak ne olacağını ekleyebilirsin (örn. kartın kapanması)
    }
}