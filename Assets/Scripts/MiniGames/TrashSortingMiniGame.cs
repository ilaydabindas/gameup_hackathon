using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class TrashSortingMinigame : MonoBehaviour
{
    public enum WasteCategory { Organic, Plastic, Paper }

    [System.Serializable]
    public struct WasteItem
    {
        public string itemName;
        public string emoji;
        public WasteCategory category;
    }

    [Header("UI Referanslari")]
    [SerializeField] private TextMeshProUGUI itemEmojiText;   // Txt_ItemEmoji
    [SerializeField] private TextMeshProUGUI itemNameText;    // Txt_ItemName
    [SerializeField] private TextMeshProUGUI statusText;      // Txt_Status
    [SerializeField] private Image cardBackground;            // Card_Item
    [SerializeField] private Button btnOrganic;               // Btn_Bin_Organic
    [SerializeField] private Button btnPlastic;               // Btn_Bin_Plastic
    [SerializeField] private Button btnPaper;                 // Btn_Bin_Paper
    [SerializeField] private Button closeButton;              // Btn_Close

    [Header("Oyun Parametreleri")]
    [SerializeField] private int targetSortCount = 6;
    [SerializeField] private float timeLimit = 15f;

    public event Action<bool, int, int> OnMiniGameCompleted;

    private readonly List<WasteItem> _wastePool = new List<WasteItem>
    {
        new WasteItem { itemName = "Muz Kabuğu", emoji = "🍌", category = WasteCategory.Organic },
        new WasteItem { itemName = "Elma Çöpü", emoji = "🍎", category = WasteCategory.Organic },
        new WasteItem { itemName = "Plastik Şişe", emoji = "🧴", category = WasteCategory.Plastic },
        new WasteItem { itemName = "Tek Kullanımlık Bardak", emoji = "🥤", category = WasteCategory.Plastic },
        new WasteItem { itemName = "Karton Kutu", emoji = "📦", category = WasteCategory.Paper },
        new WasteItem { itemName = "Eski Gazete", emoji = "📄", category = WasteCategory.Paper }
    };

    private WasteItem _currentItem;
    private int _sortedCount = 0;
    private float _timeRemaining;
    private bool _isActive = false;
    private bool _isEnding = false;
    private Coroutine _timerCoroutine;
    private Coroutine _feedbackCoroutine;
    private Color _originalCardColor;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseWithoutReward);

        if (btnOrganic != null)
            btnOrganic.onClick.AddListener(() => SubmitChoice(WasteCategory.Organic));
        if (btnPlastic != null)
            btnPlastic.onClick.AddListener(() => SubmitChoice(WasteCategory.Plastic));
        if (btnPaper != null)
            btnPaper.onClick.AddListener(() => SubmitChoice(WasteCategory.Paper));

        if (cardBackground != null)
            _originalCardColor = cardBackground.color;
    }

    private void OnEnable()
    {
        StartGame();
    }

    public void StartGame()
    {
        _sortedCount = 0;
        _timeRemaining = timeLimit;
        _isActive = true;
        _isEnding = false;

        PickNextItem();
        UpdateUI();

        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);
        _timerCoroutine = StartCoroutine(TimerRoutine());
    }

    private void PickNextItem()
    {
        int randomIndex = UnityEngine.Random.Range(0, _wastePool.Count);
        _currentItem = _wastePool[randomIndex];

        if (itemEmojiText != null) itemEmojiText.text = _currentItem.emoji;
        if (itemNameText != null) itemNameText.text = _currentItem.itemName;
    }

    private void SubmitChoice(WasteCategory chosenCategory)
    {
        if (!_isActive || _isEnding) return;

        if (chosenCategory == _currentItem.category)
        {
            // Doğru seçim
            _sortedCount++;
            UpdateUI();

            if (_feedbackCoroutine != null) StopCoroutine(_feedbackCoroutine);
            _feedbackCoroutine = StartCoroutine(FlashCard(new Color32(16, 185, 129, 255))); // Yeşil flaş

            if (_sortedCount >= targetSortCount)
            {
                StartCoroutine(EndGameRoutine(true));
                return;
            }

            PickNextItem();
        }
        else
        {
            // Yanlış kutu cezası (-2 saniye)
            _timeRemaining = Mathf.Max(0f, _timeRemaining - 2f);
            UpdateUI();

            if (_feedbackCoroutine != null) StopCoroutine(_feedbackCoroutine);
            _feedbackCoroutine = StartCoroutine(FlashCard(new Color32(239, 68, 68, 255))); // Kırmızı flaş
        }
    }

    private IEnumerator FlashCard(Color targetColor)
    {
        if (cardBackground != null)
        {
            cardBackground.color = targetColor;
            yield return new WaitForSeconds(0.2f);
            cardBackground.color = _originalCardColor;
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (_isActive && _timeRemaining > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            _timeRemaining -= 0.1f;
            UpdateUI();
        }

        if (_isActive && _timeRemaining <= 0f && !_isEnding)
        {
            _timeRemaining = 0f;
            UpdateUI();
            StartCoroutine(EndGameRoutine(false));
        }
    }

    private void UpdateUI()
    {
        if (statusText != null && !_isEnding)
        {
            statusText.text = $"Ayrıştırılan: {_sortedCount} / {targetSortCount}   |   Kalan Süre: {Mathf.CeilToInt(_timeRemaining)}s";
        }
    }

    private IEnumerator EndGameRoutine(bool success)
    {
        _isEnding = true;
        _isActive = false;

        if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
        if (_feedbackCoroutine != null) StopCoroutine(_feedbackCoroutine);

        if (statusText != null)
        {
            statusText.text = success
                ? "<color=#34D399>BAŞARILI! Geri Dönüşüm Tamamlandı (+15M Bütçe)</color>"
                : "<color=#EF4444>BAŞARISIZ! Süre Bitti (-4M Bütçe)</color>";
        }

        yield return new WaitForSeconds(1.5f);

        if (success)
        {
            OnMiniGameCompleted?.Invoke(true, 15, 10);
        }
        else
        {
            OnMiniGameCompleted?.Invoke(false, -4, -3);
        }

        gameObject.SetActive(false);

        // Minigame tamamlandı, kart akışını devam ettir
        if (MiniGameManager.Instance != null)
        {
            MiniGameManager.Instance.CloseMiniGame();
        }
    }

    public void CloseWithoutReward()
    {
        _isActive = false;
        _isEnding = true;
        if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
        if (_feedbackCoroutine != null) StopCoroutine(_feedbackCoroutine);

        gameObject.SetActive(false);

        // Ödülsüz kapatılsa da kart akışını devam ettir
        if (MiniGameManager.Instance != null)
        {
            MiniGameManager.Instance.CloseMiniGame();
        }
    }
}