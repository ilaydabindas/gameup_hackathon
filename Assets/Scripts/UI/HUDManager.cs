using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [System.Serializable]
    public class MetricBarUI
    {
        public string defaultTitle;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI valueText;
        public TextMeshProUGUI previewText;
        public Image fillImage;

        [Tooltip("True ise (Karbon/Kirlilik gibi) artışlar KIRMIZI, azalışlar YEŞİL görünür.")]
        public bool isInverseMetric; 
        public bool isCurrency; // Bütçe için $65M formatı

        [HideInInspector] public float currentValue = 50f;
        [HideInInspector] public Coroutine activeCoroutine;
    }

    [Header("3 Ana Metrik Barı")]
    public MetricBarUI carbonMetric;  // 1. Tehlike (Karbon / Erozyon / Kirlilik)
    public MetricBarUI budgetMetric;  // 2. Bütçe ($M)
    public MetricBarUI supportMetric; // 3. Destek (%)

    [Header("Animasyon ve Renk Ayarları")]
    [SerializeField] private float fillSmoothSpeed = 6f;
    [SerializeField] private Color goodPreviewColor = new Color(0.20f, 0.83f, 0.60f); // #34d399 (Yeşil)
    [SerializeField] private Color badPreviewColor = new Color(0.96f, 0.25f, 0.37f);  // #f43f5e (Kırmızı)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ClearAllPreviews();
    }

    private void Start()
    {
        InitializeMetrics(48, 65, 65);
    }

    public void InitializeMetrics(int initCarbon, int initBudget, int initSupport)
    {
        SetMetricImmediate(carbonMetric, initCarbon);
        SetMetricImmediate(budgetMetric, initBudget);
        SetMetricImmediate(supportMetric, initSupport);
        ClearAllPreviews();
    }

    public void SetRealmTitles(string metric1Title, string metric2Title, string metric3Title)
    {
        if (carbonMetric.nameText != null) carbonMetric.nameText.text = metric1Title;
        if (budgetMetric.nameText != null) budgetMetric.nameText.text = metric2Title;
        if (supportMetric.nameText != null) supportMetric.nameText.text = metric3Title;
    }

    public void ApplyMetricChanges(int carbonDelta, int budgetDelta, int supportDelta)
    {
        UpdateSingleMetric(carbonMetric, carbonMetric.currentValue + carbonDelta);
        UpdateSingleMetric(budgetMetric, budgetMetric.currentValue + budgetDelta);
        UpdateSingleMetric(supportMetric, supportMetric.currentValue + supportDelta);
        ClearAllPreviews();
    }

    public void SetMetricsTarget(int newCarbon, int newBudget, int newSupport)
    {
        UpdateSingleMetric(carbonMetric, newCarbon);
        UpdateSingleMetric(budgetMetric, newBudget);
        UpdateSingleMetric(supportMetric, newSupport);
    }

    public void ShowPreview(int cDelta, int bDelta, int sDelta)
    {
        SetSinglePreview(carbonMetric, cDelta);
        SetSinglePreview(budgetMetric, bDelta);
        SetSinglePreview(supportMetric, sDelta);
    }

    public void ClearAllPreviews()
    {
        if (carbonMetric.previewText != null) carbonMetric.previewText.gameObject.SetActive(false);
        if (budgetMetric.previewText != null) budgetMetric.previewText.gameObject.SetActive(false);
        if (supportMetric.previewText != null) supportMetric.previewText.gameObject.SetActive(false);
    }

    // =========================================================================
    // DİĞER SCRIPTLERİN ARADIĞI UYUMLULUK METOTLARI (ALIAS)
    // =========================================================================
    
    public void ClearPreview()
    {
        ClearAllPreviews();
    }

    public void UpdateMetrics(int cDelta, int bDelta, int sDelta)
    {
        ApplyMetricChanges(cDelta, bDelta, sDelta);
    }

    // GameManager.cs'in aradığı UpdateBars metodu eklendi:
    public void UpdateBars(int cVal, int bVal, int sVal)
    {
        SetMetricsTarget(cVal, bVal, sVal);
    }

    // =========================================================================

    private void SetMetricImmediate(MetricBarUI metric, float val)
    {
        float clamped = Mathf.Clamp(val, 0f, 100f);
        metric.currentValue = clamped;

        if (metric.fillImage != null)
            metric.fillImage.fillAmount = clamped / 100f;

        UpdateValueText(metric, Mathf.RoundToInt(clamped));
    }

    private void UpdateSingleMetric(MetricBarUI metric, float targetValue)
    {
        float clampedTarget = Mathf.Clamp(targetValue, 0f, 100f);
        metric.currentValue = clampedTarget;

        UpdateValueText(metric, Mathf.RoundToInt(clampedTarget));

        if (metric.fillImage != null)
        {
            if (metric.activeCoroutine != null)
                StopCoroutine(metric.activeCoroutine);

            metric.activeCoroutine = StartCoroutine(SmoothFillRoutine(metric.fillImage, clampedTarget / 100f));
        }
    }

    private IEnumerator SmoothFillRoutine(Image fillImg, float targetFill)
    {
        targetFill = Mathf.Clamp01(targetFill);

        while (!Mathf.Approximately(fillImg.fillAmount, targetFill))
        {
            fillImg.fillAmount = Mathf.MoveTowards(fillImg.fillAmount, targetFill, Time.unscaledDeltaTime * (fillSmoothSpeed * 0.15f));
            yield return null;
        }
        fillImg.fillAmount = targetFill;
    }

    private void UpdateValueText(MetricBarUI metric, int intVal)
    {
        if (metric.valueText == null) return;

        if (metric.isCurrency)
            metric.valueText.text = $"${intVal}M";
        else
            metric.valueText.text = $"{intVal}%";
    }

    private void SetSinglePreview(MetricBarUI metric, int delta)
    {
        if (metric.previewText == null) return;

        if (delta == 0)
        {
            metric.previewText.gameObject.SetActive(false);
            return;
        }

        metric.previewText.gameObject.SetActive(true);
        metric.previewText.text = delta > 0 ? $"+{delta}" : $"{delta}";

        bool isGood = metric.isInverseMetric ? (delta < 0) : (delta > 0);
        metric.previewText.color = isGood ? goodPreviewColor : badPreviewColor;
    }
}