using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HazardSpawner : MonoBehaviour
{
    public static HazardSpawner Instance { get; private set; }

    [Header("Prefab")]
    [SerializeField] private GameObject orbPrefab;

    [Header("Ekranda Aynı Anda Bulunacak Top Sayısı")]
    [SerializeField] private int startingOrbCount = 7;   // Başlangıçta ekranda gezecek top sayısı
    [SerializeField] private int maxOrbCount = 12;       // Oyun ilerledikçe çıkacak maksimum top sayısı
    [SerializeField] private float rampUpTime = 60f;     // Top sayısının 7'den 12'ye çıkma süresi (sn)

    [Header("Doğma Alanı Sınırları (Canvas Piksel)")]
    [SerializeField] private float minX = -480f;
    [SerializeField] private float maxX = 480f;
    [SerializeField] private float minY = -150f;
    [SerializeField] private float maxY = 220f;

    private RectTransform _canvasRT;
    private int _currentActiveCount = 0;
    private float _gameTimer = 0f;
    private bool _isSpawning = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();

        if (canvas != null)
        {
            _canvasRT = canvas.GetComponent<RectTransform>();
            // Başlangıçta sahneyi hemen toplarla doldur
            StartCoroutine(InitialPopulateRoutine());
            // Sürekli kontrol ve yenileme döngüsünü başlat
            StartCoroutine(ContinuousSpawnLoop());
        }
        else
        {
            Debug.LogError("HazardSpawner: Sahnede Canvas bulunamadı!");
        }
    }

    private void Update()
    {
        if (_isSpawning)
        {
            _gameTimer += Time.deltaTime;
        }
    }

    // Oyun başlar başlamaz ekrana topları hızlıca dağıtır
    private IEnumerator InitialPopulateRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < startingOrbCount; i++)
        {
            SpawnSingleOrb();
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Ekranda eksilen top oldukça anında yenisini üretir
    private IEnumerator ContinuousSpawnLoop()
    {
        while (_isSpawning)
        {
            // Zaman geçtikçe ekrandaki hedef top sayısını artır (7 -> 12)
            float progress = Mathf.Clamp01(_gameTimer / rampUpTime);
            int targetCount = Mathf.RoundToInt(Mathf.Lerp(startingOrbCount, maxOrbCount, progress));

            if (_currentActiveCount < targetCount)
            {
                SpawnSingleOrb();
            }

            // Kontrol sıklığı (her 0.3 saniyede bir eksik var mı diye bakar)
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void SpawnSingleOrb()
    {
        if (orbPrefab == null || _canvasRT == null) return;

        // Ekranın dört bir yanına rastgele dağıt
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        GameObject newOrb = Instantiate(orbPrefab, _canvasRT);
        _currentActiveCount++;

        RectTransform rt = newOrb.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(randomX, randomY);
            rt.localScale = Vector3.one;
        }
    }

    // Küre patladığında çağrılır
    public void OnOrbDestroyed()
    {
        _currentActiveCount--;
        if (_currentActiveCount < 0) _currentActiveCount = 0;
    }

    public void StopSpawning() => _isSpawning = false;
}
