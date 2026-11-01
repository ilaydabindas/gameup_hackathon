using System.Collections;
using UnityEngine;

public class WaterHazardSpawner : MonoBehaviour
{
    [Header("Prefab ve Hedef Alan")]
    public GameObject hazardPrefab;
    public RectTransform canvasRect;

    [Header("Spawn Konum Ayarları")]
    public float waterLevelY = -360f;
    public float yVariation = 30f;

    [Header("Zaman Ayarları")]
    public float minSpawnInterval = 0.5f; // Test için 2 saniyeye çekildi
    public float maxSpawnInterval = 1.5f;

    private float screenHalfWidth = 960f;

    private void Start()
    {
        if (hazardPrefab == null)
        {
            Debug.LogError("<color=red>HATA:</color> WaterHazardSpawner üzerinde 'Hazard Prefab' alanı boş bırakılmış!");
            return;
        }

        if (canvasRect != null)
        {
            screenHalfWidth = canvasRect.rect.width / 2f;
        }
        else
        {
            Debug.LogWarning("Canvas Rect atanmamış, varsayılan 960 genişlik kullanılıyor.");
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // Oyun açılır açılmaz ilk spawn'ı 1 saniye sonra yap
        yield return new WaitForSeconds(1f);
        SpawnHazard();

        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            SpawnHazard();
        }
    }

    private void SpawnHazard()
    {
        if (hazardPrefab == null) return;

        int direction = Random.value > 0.5f ? 1 : -1;
        float startX = direction > 0 ? -(screenHalfWidth + 80f) : (screenHalfWidth + 80f);
        float spawnY = waterLevelY + Random.Range(-yVariation, yVariation);

        // Doğrudan Canvas altına spawn et
        Transform parentTransform = canvasRect != null ? canvasRect : transform;
        GameObject instance = Instantiate(hazardPrefab, parentTransform);
        
        // Ekranda en öne getir
        instance.transform.SetAsLastSibling();

        RectTransform rect = instance.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(startX, spawnY);
            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f);
            rect.localScale = Vector3.one;
        }

        WaterHazardTarget target = instance.GetComponent<WaterHazardTarget>();
        if (target != null)
        {
            float speed = Random.Range(140f, 220f);
            target.Initialize(direction, speed, screenHalfWidth);
        }

        Debug.Log("<color=cyan>SU HEDEFİ BAŞARIYLA OLUŞTURULDU:</color> " + instance.name);
    }
}