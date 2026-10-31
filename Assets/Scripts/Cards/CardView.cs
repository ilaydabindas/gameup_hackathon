using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Kart Metinleri")]
    [SerializeField] private TextMeshProUGUI sectorText; // Yeni: Üstteki kategori (Örn: SANAYİ BÖLGESİ)
    [SerializeField] private TextMeshProUGUI titleText;  // Ana başlık
    [SerializeField] private TextMeshProUGUI descText;   // Açıklama

    [Header("Butonlar (Görsel Efekt İçin)")]
    [SerializeField] private Transform btnLeft;
    [SerializeField] private Transform btnMid;
    [SerializeField] private Transform btnRight;

    [Header("Kaydırma Ayarları")]
    [SerializeField] private float thresholdX = 120f;
    [SerializeField] private float thresholdY = 100f;
    [SerializeField] private float rotationFactor = 0.05f;

    // Karar event'leri
    public event Action OnSwipeRight;
    public event Action OnSwipeLeft;
    public event Action OnSwipeUp;

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Vector2 initialPosition;
    private Coroutine resetCoroutine;

    private Vector3 originalBtnScale = Vector3.one;
    private Vector3 highlightBtnScale = new Vector3(1.1f, 1.1f, 1.1f); // Seçilen buton %10 büyür

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        initialPosition = rectTransform.anchoredPosition;
        
        if (btnLeft != null) originalBtnScale = btnLeft.localScale;
    }

    /// <summary>
    /// AI'dan veya veritabanından gelen yeni kriz kartını doldurur.
    /// </summary>
    public void SetupCard(string sector, string title, string description)
    {
        if (sectorText != null) sectorText.text = sector;
        if (titleText != null) titleText.text = title;
        if (descText != null) descText.text = description;

        ResetCardPosition();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float scale = (parentCanvas != null) ? parentCanvas.scaleFactor : 1f;
        rectTransform.anchoredPosition += eventData.delta / scale;
        
        // Sağa/sola kaydırırken hafif döndür
        rectTransform.localEulerAngles = new Vector3(0, 0, -rectTransform.anchoredPosition.x * rotationFactor);

        UpdateVisualFeedback();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 pos = rectTransform.anchoredPosition;

        // 1. Yukarı Eşik Kontrolü
        if (pos.y > thresholdY && pos.y > Mathf.Abs(pos.x))
        {
            Debug.Log("<color=cyan>[CardView] YUKARI Kararı</color>");
            OnSwipeUp?.Invoke();
            resetCoroutine = StartCoroutine(ResetPositionRoutine());
        }
        // 2. Sağa Eşik Kontrolü
        else if (pos.x > thresholdX)
        {
            Debug.Log("<color=green>[CardView] SAĞ Kararı</color>");
            OnSwipeRight?.Invoke();
            resetCoroutine = StartCoroutine(ResetPositionRoutine());
        }
        // 3. Sola Eşik Kontrolü
        else if (pos.x < -thresholdX)
        {
            Debug.Log("<color=orange>[CardView] SOL Kararı</color>");
            OnSwipeLeft?.Invoke();
            resetCoroutine = StartCoroutine(ResetPositionRoutine());
        }
        else
        {
            // Yeterince sürüklenmediyse geri dön
            resetCoroutine = StartCoroutine(ResetPositionRoutine());
        }

        ResetButtonScales();
    }

    private void UpdateVisualFeedback()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        ResetButtonScales(); // Önce hepsini normal boyuta getir

        // Yeterince sürüklendiği yöndeki butonu vurgula (Büyüt)
        if (pos.y > thresholdY && pos.y > Mathf.Abs(pos.x))
        {
            if (btnMid != null) btnMid.localScale = highlightBtnScale;
        }
        else if (pos.x > thresholdX)
        {
            if (btnRight != null) btnRight.localScale = highlightBtnScale;
        }
        else if (pos.x < -thresholdX)
        {
            if (btnLeft != null) btnLeft.localScale = highlightBtnScale;
        }
    }

    private void ResetButtonScales()
    {
        if (btnLeft != null) btnLeft.localScale = originalBtnScale;
        if (btnMid != null) btnMid.localScale = originalBtnScale;
        if (btnRight != null) btnRight.localScale = originalBtnScale;
    }

    private IEnumerator ResetPositionRoutine()
    {
        float duration = 0.15f;
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.localRotation;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, initialPosition, t);
            rectTransform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, t);
            yield return null;
        }
        
        ResetCardPosition();
    }

    private void ResetCardPosition()
    {
        rectTransform.anchoredPosition = initialPosition;
        rectTransform.localRotation = Quaternion.identity;
        ResetButtonScales();
    }
}