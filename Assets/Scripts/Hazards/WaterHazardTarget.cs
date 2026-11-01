using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WaterHazardTarget : MonoBehaviour, IPointerDownHandler
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 160f;
    private int direction = 1; // 1: Sağa, -1: Sola
    private float screenLimitX = 960f;

    [Header("Deniz Yüzme / Dalgalanma Ayarları")]
    [Tooltip("Dalganın dikeyde ne kadar yukarı-aşağı ineceği (piksel)")]
    public float waveHeight = 14f; 
    [Tooltip("Dalgalanma ve sallanma hızı")]
    public float waveFrequency = 3.5f; 
    [Tooltip("Sağa sola yatma açısı (derece)")]
    public float tiltAngle = 9f; 

    [Header("Ödül ve Cezalar")]
    public int rewardBudget = 10;
    public int rewardCarbonReduce = -5;
    public int penaltyBudget = -8;
    public int penaltyCarbonIncrease = 8;

    private RectTransform rectTransform;
    private bool isResolved = false;
    private float initialY;
    private float timeElapsed = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(int moveDirection, float speed, float limitX)
    {
        direction = moveDirection;
        moveSpeed = speed;
        screenLimitX = limitX;
        initialY = rectTransform.anchoredPosition.y;

        // Her nesnenin dalga döngüsünün farklı bir anda başlaması için ufak rastgele zaman ofseti
        timeElapsed = Random.Range(0f, 10f);

        // Yön sola doğruysa görseli ters çevir
        if (direction < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void Update()
    {
        if (isResolved) return;

        timeElapsed += Time.deltaTime;

        // 1. Yatayda düz ilerleme
        float newX = rectTransform.anchoredPosition.x + (direction * moveSpeed * Time.deltaTime);

        // 2. Dikeyde sinüs dalgası ile yüzme hareketi (yukarı-aşağı batıp çıkma)
        float newY = initialY + Mathf.Sin(timeElapsed * waveFrequency) * waveHeight;

        rectTransform.anchoredPosition = new Vector2(newX, newY);

        // 3. Su dalgasına göre sağa-sola hafifçe eğilme / beşik gibi sallanma (Z ekseni rotasyonu)
        float currentTilt = Mathf.Cos(timeElapsed * waveFrequency) * tiltAngle * direction;
        rectTransform.localEulerAngles = new Vector3(0f, 0f, currentTilt);

        // Ekran sınırını aşma kontrolü
        if ((direction > 0 && rectTransform.anchoredPosition.x > screenLimitX + 100f) ||
            (direction < 0 && rectTransform.anchoredPosition.x < -screenLimitX - 100f))
        {
            OnEscaped();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isResolved) return;
        isResolved = true;

        ApplyStats(rewardBudget, rewardCarbonReduce);
        Destroy(gameObject);
    }

    private void OnEscaped()
    {
        isResolved = true;

        ApplyStats(penaltyBudget, penaltyCarbonIncrease);
        Destroy(gameObject);
    }

    private void ApplyStats(int budgetDelta, int carbonDelta)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.budget = Mathf.Clamp(GameManager.Instance.budget + budgetDelta, 0, 100);
            GameManager.Instance.carbon = Mathf.Clamp(GameManager.Instance.carbon + carbonDelta, 0, 100);

            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.UpdateBars(
                    GameManager.Instance.carbon, 
                    GameManager.Instance.budget, 
                    GameManager.Instance.support
                );
            }
        }
    }
}