using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Kart Metinleri")]
    [SerializeField] private TextMeshProUGUI sectorText; 
    [SerializeField] private TextMeshProUGUI titleText;  
    [SerializeField] private TextMeshProUGUI descText;   

    [Header("Butonlar (Görsel Efekt İçin)")]
    [SerializeField] private Transform btnLeft;
    [SerializeField] private Transform btnMid;
    [SerializeField] private Transform btnRight;

    [Header("Buton Üst Başlıkları (İhracat İzni vb.)")]
    [SerializeField] private TextMeshProUGUI btnLeftText;
    [SerializeField] private TextMeshProUGUI btnMidText;
    [SerializeField] private TextMeshProUGUI btnRightText;

    [Header("Buton Alt Değerleri (+10 -5 vb.)")]
    [SerializeField] private TextMeshProUGUI btnLeftImpactText;
    [SerializeField] private TextMeshProUGUI btnMidImpactText;
    [SerializeField] private TextMeshProUGUI btnRightImpactText;

    [Header("Kaydırma Ayarları")]
    [SerializeField] private float thresholdX = 120f;
    [SerializeField] private float thresholdY = 100f;
    [SerializeField] private float rotationFactor = 0.05f;

    public event Action OnSwipeRight;
    public event Action OnSwipeLeft;
    public event Action OnSwipeUp;

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Vector2 initialPosition;
    private Coroutine resetCoroutine;
    private Vector3 originalBtnScale = Vector3.one;
    private Vector3 highlightBtnScale = new Vector3(1.1f, 1.1f, 1.1f);
    
    // O anki kartın verilerini hafızada tutarız (Önizleme rakamları için gerekli)
    private CrisisCardData currentCardData;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        initialPosition = rectTransform.anchoredPosition;
        
        if (btnLeft != null) originalBtnScale = btnLeft.localScale;
    }

    public void SetupCard(CrisisCardData cardData)
    {
        currentCardData = cardData; // Veriyi hafızaya al

        if (sectorText != null) sectorText.text = cardData.sectorTag;
        if (titleText != null) titleText.text = cardData.title;
        if (descText != null) descText.text = cardData.description;

        // Üst Başlıkları Doldur
        if (btnRightText != null) btnRightText.text = cardData.ecoChoice.choiceLabel;
        if (btnMidText != null) btnMidText.text = cardData.techChoice.choiceLabel;
        if (btnLeftText != null) btnLeftText.text = cardData.indChoice.choiceLabel;

        // Alt Etki Değerlerini Doldur
        if (btnRightImpactText != null) btnRightImpactText.text = FormatImpact(cardData.ecoChoice);
        if (btnMidImpactText != null) btnMidImpactText.text = FormatImpact(cardData.techChoice);
        if (btnLeftImpactText != null) btnLeftImpactText.text = FormatImpact(cardData.indChoice);

        ResetCardPosition();
        gameObject.SetActive(true);
    }

    // Rakamları artı/eksi işaretiyle formatlayan yardımcı fonksiyon
    private string FormatImpact(CardChoice choice)
    {
        string c = choice.carbonImpact > 0 ? $"+{choice.carbonImpact}" : choice.carbonImpact.ToString();
        string b = choice.budgetImpact > 0 ? $"+{choice.budgetImpact}" : choice.budgetImpact.ToString();
        string s = choice.supportImpact > 0 ? $"+{choice.supportImpact}" : choice.supportImpact.ToString();
        
        return $"C:{c} B:{b} S:{s}";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float scale = (parentCanvas != null) ? parentCanvas.scaleFactor : 1f;
        rectTransform.anchoredPosition += eventData.delta / scale;
        rectTransform.localEulerAngles = new Vector3(0, 0, -rectTransform.anchoredPosition.x * rotationFactor);
        
        UpdateVisualFeedback();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 pos = rectTransform.anchoredPosition;

        if (pos.y > thresholdY && pos.y > Mathf.Abs(pos.x))
        {
            OnSwipeUp?.Invoke();
            HideCard();
        }
        else if (pos.x > thresholdX)
        {
            OnSwipeRight?.Invoke();
            HideCard();
        }
        else if (pos.x < -thresholdX)
        {
            OnSwipeLeft?.Invoke();
            HideCard();
        }
        else
        {
            resetCoroutine = StartCoroutine(ResetPositionRoutine());
        }
        
        ResetButtonScales();
    }

    private void UpdateVisualFeedback()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        ResetButtonScales();

        if (currentCardData == null) return;

        // YUKARI - TECH
        if (pos.y > thresholdY && pos.y > Mathf.Abs(pos.x)) 
        { 
            if (btnMid != null) btnMid.localScale = highlightBtnScale;
            if (HUDManager.Instance != null)
                HUDManager.Instance.ShowPreview(currentCardData.techChoice.carbonImpact, currentCardData.techChoice.budgetImpact, currentCardData.techChoice.supportImpact);
        }
        // SAĞA - ECO
        else if (pos.x > thresholdX) 
        { 
            if (btnRight != null) btnRight.localScale = highlightBtnScale;
            if (HUDManager.Instance != null)
                HUDManager.Instance.ShowPreview(currentCardData.ecoChoice.carbonImpact, currentCardData.ecoChoice.budgetImpact, currentCardData.ecoChoice.supportImpact);
        }
        // SOLA - IND
        else if (pos.x < -thresholdX) 
        { 
            if (btnLeft != null) btnLeft.localScale = highlightBtnScale;
            if (HUDManager.Instance != null)
                HUDManager.Instance.ShowPreview(currentCardData.indChoice.carbonImpact, currentCardData.indChoice.budgetImpact, currentCardData.indChoice.supportImpact);
        }
        // KART ORTADAYSA (Karar eşiğine ulaşmadıysa)
        else
        {
            if (HUDManager.Instance != null)
                HUDManager.Instance.ClearPreview();
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
        
        // Kart yerine oturduğunda önizleme yazılarını temizle
        if (HUDManager.Instance != null)
            HUDManager.Instance.ClearPreview();
    }

    private void HideCard()
    {
        ResetCardPosition();
        gameObject.SetActive(false);
    }
}