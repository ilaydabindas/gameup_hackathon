using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class FieldFixMinigame : MonoBehaviour
{
    private enum PlotType { Hole, Rock, Planted }

    [Header("UI Referanslari")]
    [SerializeField] private Button[] plotButtons;          // Plot_0 - Plot_8
    [SerializeField] private TextMeshProUGUI statusText;    // Txt_Status
    [SerializeField] private Button closeButton;           // Btn_Close

    [Header("Oyun Parametreleri")]
    [SerializeField] private int requiredTrees = 5;        // Dikilecek fidan sayisi
    [SerializeField] private float timeLimit = 15f;        // Toplam sure (saniye)
    [SerializeField] private int rockCount = 2;            // Engel kaya sayisi

    [Header("Renk Paleti")]
    [SerializeField] private Color holeColor = new Color32(45, 30, 20, 255);      // Toprak rengi
    [SerializeField] private Color rockColor = new Color32(55, 65, 81, 255);      // Koyu tas rengi
    [SerializeField] private Color plantedColor = new Color32(16, 185, 129, 255); // Yeseren zemin

    public event Action<bool, int, int> OnMiniGameCompleted;

    private PlotType[] _plotStates = new PlotType[9];
    private TextMeshProUGUI[] _plotTexts = new TextMeshProUGUI[9];
    private Image[] _plotImages = new Image[9];
    private int _plantedCount = 0;
    private float _timeRemaining;
    private bool _isActive = false;
    private bool _isEnding = false;
    private bool _isHoveringWarning = false;
    private Coroutine _timerCoroutine;
    private Coroutine _feedbackCoroutine;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseWithoutReward);

        for (int i = 0; i < plotButtons.Length; i++)
        {
            int index = i;
            if (plotButtons[i] != null)
            {
                _plotTexts[i] = plotButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                _plotImages[i] = plotButtons[i].GetComponent<Image>();
                plotButtons[i].onClick.AddListener(() => OnPlotClicked(index));

                // Mouse ile uzerine gelme / cikma algilayicisi (EventTrigger)
                SetupHoverEvents(plotButtons[i].gameObject, index);
            }
        }
    }

    private void SetupHoverEvents(GameObject targetObj, int index)
    {
        EventTrigger trigger = targetObj.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = targetObj.AddComponent<EventTrigger>();

        // Mouse Üzerine Gelince (PointerEnter)
        EventTrigger.Entry enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener((_) => OnPlotHoverEnter(index));
        trigger.triggers.Add(enterEntry);

        // Mouse Üzerinden Çıkınca (PointerExit)
        EventTrigger.Entry exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener((_) => OnPlotHoverExit());
        trigger.triggers.Add(exitEntry);
    }

    private void OnEnable()
    {
        StartGame();
    }

    public void StartGame()
    {
        _plantedCount = 0;
        _timeRemaining = timeLimit;
        _isActive = true;
        _isEnding = false;
        _isHoveringWarning = false;

        SetupGrid();
        UpdateUI();

        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);
        _timerCoroutine = StartCoroutine(TimerRoutine());
    }

    private void SetupGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            _plotStates[i] = PlotType.Hole;
        }

        // Rastgele hucrelere kaya yerlestir
        List<int> indices = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        for (int i = 0; i < rockCount && indices.Count > 0; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, indices.Count);
            int selectedPlot = indices[randIndex];
            _plotStates[selectedPlot] = PlotType.Rock;
            indices.RemoveAt(randIndex);
        }

        // Yazi yerine emojileri ata
        for (int i = 0; i < 9; i++)
        {
            if (plotButtons[i] != null)
                plotButtons[i].interactable = true;

            if (_plotStates[i] == PlotType.Rock)
            {
                if (_plotImages[i] != null) _plotImages[i].color = rockColor;
                if (_plotTexts[i] != null) _plotTexts[i].text = "🪨";
            }
            else
            {
                if (_plotImages[i] != null) _plotImages[i].color = holeColor;
                if (_plotTexts[i] != null) _plotTexts[i].text = "🕳️";
            }
        }
    }

    private void OnPlotHoverEnter(int index)
    {
        if (!_isActive || _isEnding) return;

        if (_plotStates[index] == PlotType.Rock)
        {
            _isHoveringWarning = true;
            if (statusText != null)
                statusText.text = "<color=#FBBF24>⚠️ Dikkat: Kayalık bölge! Buraya ekim yapılamaz.</color>";
        }
    }

    private void OnPlotHoverExit()
    {
        if (!_isActive || _isEnding) return;

        if (_isHoveringWarning)
        {
            _isHoveringWarning = false;
            UpdateUI();
        }
    }

    private void OnPlotClicked(int index)
    {
        if (!_isActive || _isEnding) return;

        if (_plotStates[index] == PlotType.Hole)
        {
            // Basarili fidan dikimi
            _plotStates[index] = PlotType.Planted;
            _plantedCount++;

            if (_plotImages[index] != null) _plotImages[index].color = plantedColor;
            if (_plotTexts[index] != null) _plotTexts[index].text = "🌲";
            if (plotButtons[index] != null) plotButtons[index].interactable = false;

            UpdateUI();

            if (_plantedCount >= requiredTrees)
            {
                StartCoroutine(EndGameRoutine(true));
            }
        }
        else if (_plotStates[index] == PlotType.Rock)
        {
            // Kayaya basildi: Sure cezasi ve kirmizi hata uyarisi
            _timeRemaining = Mathf.Max(0f, _timeRemaining - 2f);
            
            if (_feedbackCoroutine != null)
                StopCoroutine(_feedbackCoroutine);
            _feedbackCoroutine = StartCoroutine(ShowRockErrorFeedback(index));
        }
    }

    private IEnumerator ShowRockErrorFeedback(int index)
    {
        _isHoveringWarning = false;

        // Butonu kirmizi flaslat
        if (_plotImages[index] != null)
            _plotImages[index].color = Color.red;

        // Ekrana hata mesaji bas
        if (statusText != null)
            statusText.text = "<color=#EF4444>❌ HATA: Kayalık zemine fidan dikilemez! (-2s Ceza)</color>";

        yield return new WaitForSeconds(0.4f);

        if (_plotImages[index] != null)
            _plotImages[index].color = rockColor;

        yield return new WaitForSeconds(0.6f);

        UpdateUI();
    }

    private IEnumerator TimerRoutine()
    {
        while (_isActive && _timeRemaining > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            _timeRemaining -= 0.1f;
            
            // Kullanici o an bir uyari/hata mesaji gormuyorsa sayaci guncelle
            if (!_isHoveringWarning && (_feedbackCoroutine == null))
            {
                UpdateUI();
            }
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
            statusText.text = $"🌱 Dikilen Fidan: {_plantedCount} / {requiredTrees}   |   Kalan Süre: {Mathf.CeilToInt(_timeRemaining)}s";
        }
    }

    private IEnumerator EndGameRoutine(bool success)
    {
        _isEnding = true;
        _isActive = false;

        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);
        if (_feedbackCoroutine != null)
            StopCoroutine(_feedbackCoroutine);

        if (statusText != null)
        {
            if (success)
            {
                statusText.text = "<color=#34D399>BAŞARILI! Saha Onarıldı (+18M Bütçe)</color>";
            }
            else
            {
                statusText.text = "<color=#EF4444>BAŞARISIZ! Süre Bitti (-5M Bütçe)</color>";
            }
        }

        yield return new WaitForSeconds(1.5f);

        if (success)
        {
            OnMiniGameCompleted?.Invoke(true, 18, 12);
        }
        else
        {
            OnMiniGameCompleted?.Invoke(false, -5, -4);
        }

        gameObject.SetActive(false);

        // Minigame kapandı, akışı devam ettir
        if (MiniGameManager.Instance != null)
        {
            MiniGameManager.Instance.CloseMiniGame();
        }
    }

    public void CloseWithoutReward()
    {
        _isActive = false;
        _isEnding = true;

        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);
        if (_feedbackCoroutine != null)
            StopCoroutine(_feedbackCoroutine);

        gameObject.SetActive(false);

        // Minigame ödülsüz kapatılsa da akışı devam ettir
        if (MiniGameManager.Instance != null)
        {
            MiniGameManager.Instance.CloseMiniGame();
        }
    }
}