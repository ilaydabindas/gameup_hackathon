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

    [Header("Butonlar (Görsel Efekt ve Tıklama İçin)")]
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
    private Coroutine animCoroutine;
    private Vector3 originalBtnScale = Vector3.one;
    private Vector3 highlightBtnScale = new Vector3(1.1f, 1.1f, 1.1f);
    
    private CrisisCardData currentCardData;

    private void Awake()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();
            initialPosition = rectTransform.anchoredPosition;
        }
        if (btnLeft != null && originalBtnScale == Vector3.one) 
        {
            originalBtnScale = btnLeft.localScale;
        }
    }

    public void SetupCard(CrisisCardData cardData)
    {
        InitializeReferences(); // Güvenlik kontrolü
        currentCardData = cardData;

        if (sectorText != null) sectorText.text = cardData.sectorTag;
        if (titleText != null) titleText.text = cardData.title;
        if (descText != null) descText.text = cardData.description;

        if (btnRightText != null) btnRightText.text = cardData.ecoChoice.choiceLabel;
        if (btnMidText != null) btnMidText.text = cardData.techChoice.choiceLabel;
        if (btnLeftText != null) btnLeftText.text = cardData.indChoice.choiceLabel;

        if (btnRightImpactText != null) btnRightImpactText.text = FormatImpact(cardData.ecoChoice);
        if (btnMidImpactText != null) btnMidImpactText.text = FormatImpact(cardData.techChoice);
        if (btnLeftImpactText != null) btnLeftImpactText.text = FormatImpact(cardData.indChoice);

        ResetCardPosition();
        gameObject.SetActive(true);

        // Yeni kart gelince süreyi sıfırla
        FindAnyObjectByType<CrisisTimer>()?.ResetAndStartTimer();
    }

    private string FormatImpact(CardChoice choice)
{
    // Üstteki HUD barlarının renklerine uyumlu hex kodları
    string carbonColor = "#966107";  // Karbon için turuncu/sarı
    string budgetColor = "#0a3e92";  // Bütçe için mavi
    string supportColor = "#035925"; // Destek için yeşil

    // Artı işareti ve değerleri renklendirme etiketleriyle birlikte oluştur
    string c = choice.carbonImpact > 0 ? $"+{choice.carbonImpact}" : choice.carbonImpact.ToString();
    string b = choice.budgetImpact > 0 ? $"+{choice.budgetImpact}" : choice.budgetImpact.ToString();
    string s = choice.supportImpact > 0 ? $"+{choice.supportImpact}" : choice.supportImpact.ToString();

    // Harfler kaldırıldı, sadece renkli sayılar bırakıldı
    return $"<color={carbonColor}>{c}</color>   <color={budgetColor}>{b}</color>   <color={supportColor}>{s}</color>";
}

    public void OnClickLeftChoice() { ForceSwipeLeft(); }
    public void OnClickMidChoice() { ForceSwipeUp(); }
    public void OnClickRightChoice() { ForceSwipeRight(); }

    // --- ANİMASYONLU FIRLATMA FONKSİYONLARI ---

    public void ForceSwipeLeft()
    {
        InitializeReferences();
        if (!gameObject.activeInHierarchy) { OnSwipeLeft?.Invoke(); return; }

        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateAndSwipeRoutine(new Vector2(-900f, 0f), OnSwipeLeft));
    }

    public void ForceSwipeRight()
    {
        InitializeReferences();
        if (!gameObject.activeInHierarchy) { OnSwipeRight?.Invoke(); return; }

        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateAndSwipeRoutine(new Vector2(900f, 0f), OnSwipeRight));
    }

    public void ForceSwipeUp()
    {
        InitializeReferences();
        if (!gameObject.activeInHierarchy) { OnSwipeUp?.Invoke(); return; }

        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateAndSwipeRoutine(new Vector2(0f, 900f), OnSwipeUp));
    }

    private IEnumerator AnimateAndSwipeRoutine(Vector2 targetOffset, Action callback)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = startPos + targetOffset;

        while (elapsed < duration)
        {
            // Zaman durdurulsa bile animasyonun takılmaması için unscaledDeltaTime kullanıyoruz
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            rectTransform.localEulerAngles = new Vector3(0, 0, -rectTransform.anchoredPosition.x * rotationFactor);
            yield return null;
        }

        callback?.Invoke();
        HideCard();
    }

    // --- DRAG / KAYDIRMA MEKANİĞİ ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        if (animCoroutine != null) StopCoroutine(animCoroutine);
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

        if (pos.y > thresholdY && pos.y > Mathf.Abs(pos.x)) 
        { 
            if (btnMid != null) btnMid.localScale = highlightBtnScale;
            if (HUDManager.Instance != null)
                HUDManager.Instance.ShowPreview(currentCardData.techChoice.carbonImpact, currentCardData.techChoice.budgetImpact, currentCardData.techChoice.supportImpact);
        }
        else if (pos.x > thresholdX) 
        { 
            if (btnRight != null) btnRight.localScale = highlightBtnScale;
            if (HUDManager.Instance != null)
                HUDManager.Instance.ShowPreview(currentCardData.ecoChoice.carbonImpact, currentCardData.ecoChoice.budgetImpact, currentCardData.ecoChoice.supportImpact);
        }
        else if (pos.x < -thresholdX) 
        { 
            if (btnLeft != null) btnLeft.localScale = highlightBtnScale;
            if (HUDManager.Instance != null)
                HUDManager.Instance.ShowPreview(currentCardData.indChoice.carbonImpact, currentCardData.indChoice.budgetImpact, currentCardData.indChoice.supportImpact);
        }
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
        
        if (HUDManager.Instance != null)
            HUDManager.Instance.ClearPreview();
    }

    private void HideCard()
    {
        ResetCardPosition();
        gameObject.SetActive(false);
    }
}