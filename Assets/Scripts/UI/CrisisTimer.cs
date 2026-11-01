using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrisisTimer : MonoBehaviour
{
    [Header("Zaman Ayarları")]
    [Tooltip("Her kart için süre (saniye)")]
    [SerializeField] private float cardDuration = 10f;
    private float currentTimer;
    private bool isTimerRunning = false;

    [Header("UI Elemanları")]
    [SerializeField] private Image timerFillImage;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Kart Referansı")]
    [SerializeField] private CardView cardView;

    private void Awake()
    {
        FindCardView();
    }

    private void Start()
    {
        ResetAndStartTimer();
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        currentTimer -= Time.deltaTime;
        UpdateUI();

        if (currentTimer <= 0f)
        {
            currentTimer = 0f;
            isTimerRunning = false;
            OnTimeExpired();
        }
    }

    public void ResetAndStartTimer()
    {
        FindCardView();
        currentTimer = cardDuration;
        isTimerRunning = true;
        UpdateUI();
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    private void UpdateUI()
    {
        if (timerFillImage != null)
        {
            timerFillImage.fillAmount = Mathf.Clamp01(currentTimer / cardDuration);
        }

        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(currentTimer).ToString();
        }
    }

    private void OnTimeExpired()
    {
        // YENİ LOG MESAJI BURADA
        Debug.Log("<color=red>Süre doldu!</color> Kart zorla sola kaydırılıyor...");

        FindCardView();

        if (cardView != null)
        {
            cardView.ForceSwipeLeft();
        }
    }

    private void FindCardView()
    {
        if (cardView == null)
        {
            cardView = GetComponent<CardView>();
            if (cardView == null)
            {
                cardView = FindAnyObjectByType<CardView>();
            }
        }
    }
}