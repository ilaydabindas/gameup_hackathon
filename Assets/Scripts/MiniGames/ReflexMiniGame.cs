using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ReflexMinigame : MonoBehaviour
{
    [Header("UI Bilesenleri (TextMesh Pro)")]
    [SerializeField] private RectTransform barBackground;
    [SerializeField] private RectTransform targetZone;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private TextMeshProUGUI hitsText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button closeButton;

    [Header("Hiz ve Zorluk Parametreleri")]
    [SerializeField] private float baseSpeed = 350f;
    [SerializeField] private float speedIncreasePerHit = 90f;
    [SerializeField] private int requiredHits = 4;
    [SerializeField] private int maxLives = 3;

    public event Action<bool, int, int> OnMiniGameCompleted;

    private float _currentSpeed;
    private float _minX;
    private float _maxX;
    private int _direction = 1;
    private int _currentHits = 0;
    private int _currentLives = 3;
    private bool _isActive = false;

    private void Awake()
    {
        if (hitButton != null)
            hitButton.onClick.AddListener(TryHit);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseWithoutReward);
    }

    private void OnEnable()
    {
        StartGame();
    }

    public void StartGame()
    {
        _currentHits = 0;
        _currentLives = maxLives;
        _currentSpeed = baseSpeed;
        _isActive = true;

        float barWidth = barBackground.rect.width;
        _minX = -barWidth / 2f + (cursor.rect.width / 2f);
        _maxX = barWidth / 2f - (cursor.rect.width / 2f);

        cursor.anchoredPosition = new Vector2(_minX, 0f);
        RandomizeTargetZone();
        UpdateUI();
    }

    private void Update()
    {
        if (!_isActive) return;

        MoveCursor();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryHit();
        }
    }

    private void MoveCursor()
    {
        Vector2 pos = cursor.anchoredPosition;
        pos.x += _direction * _currentSpeed * Time.deltaTime;

        if (pos.x >= _maxX)
        {
            pos.x = _maxX;
            _direction = -1;
        }
        else if (pos.x <= _minX)
        {
            pos.x = _minX;
            _direction = 1;
        }

        cursor.anchoredPosition = pos;
    }

    public void TryHit()
    {
        if (!_isActive) return;

        float cursorX = cursor.anchoredPosition.x;
        float targetX = targetZone.anchoredPosition.x;
        float halfTargetWidth = targetZone.rect.width / 2f;

        bool isInside = cursorX >= (targetX - halfTargetWidth) && cursorX <= (targetX + halfTargetWidth);

        if (isInside)
        {
            _currentHits++;
            _currentSpeed += speedIncreasePerHit;

            if (_currentHits >= requiredHits)
            {
                EndGame(true);
                return;
            }
            RandomizeTargetZone();
        }
        else
        {
            _currentLives--;
            if (_currentLives <= 0)
            {
                EndGame(false);
                return;
            }
        }

        UpdateUI();
    }

    private void RandomizeTargetZone()
    {
        float barWidth = barBackground.rect.width;
        float targetWidth = targetZone.rect.width;
        
        float availableWidth = (barWidth / 2f) - (targetWidth / 2f) - 15f;
        float randomX = UnityEngine.Random.Range(-availableWidth, availableWidth);

        targetZone.anchoredPosition = new Vector2(randomX, 0f);
    }

    private void UpdateUI()
    {
        if (hitsText != null)
            hitsText.text = $"İsabet: {_currentHits} / {requiredHits}";

        if (livesText != null)
        {
            int lostLives = maxLives - _currentLives;

            // Soldan eksilen kalpler saydam (gorunmez), kalanlar kirmizi
            string lostHearts = $"<color=#00000000>{new string('❤', Mathf.Max(0, lostLives))}</color>";
            string activeHearts = $"<color=#EF4444>{new string('❤', Mathf.Max(0, _currentLives))}</color>";

            livesText.text = $"{lostHearts}{activeHearts}  Kalan Hak: {_currentLives}";
        }
    }

    private void EndGame(bool success)
    {
        _isActive = false;

        if (success)
        {
            OnMiniGameCompleted?.Invoke(true, 22, 9);
        }
        else
        {
            OnMiniGameCompleted?.Invoke(false, -4, -3);
        }

        gameObject.SetActive(false);

        // Minigame kapandı, kart akışını devam ettir
        if (MiniGameManager.Instance != null)
        {
            MiniGameManager.Instance.CloseMiniGame();
        }
    }

    public void CloseWithoutReward()
    {
        _isActive = false;
        gameObject.SetActive(false);

        // Ödülsüz kapatılsa da kart akışını devam ettir
        if (MiniGameManager.Instance != null)
        {
            MiniGameManager.Instance.CloseMiniGame();
        }
    }
}