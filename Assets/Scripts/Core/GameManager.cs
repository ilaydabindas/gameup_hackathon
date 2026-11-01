using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Oyun Metrikleri")]
    public int carbon = 48;
    public int budget = 65;
    public int support = 65;

    [Header("Tur ve Zaman Ayarları")]
    public int currentTurn = 1;
    public int maxTurns = 24;
    public int currentYear = 2026;

    // HTML prototipindeki mevsim dizilimi (Yılda 4 mevsim)
    private string[] seasonNames = { "İLKBAHAR", "YAZ", "SONBAHAR", "KIŞ" };
    private string[] seasonIcons = { "", "", "", "" };

    [Header("Referanslar")]
    public CardView cardView;
    public GameOverManager gameOverManager; // Bitiş ekranı scripti
    public List<CrisisCardData> availableCards; // Editörden kartları buraya sürükle

    private CrisisCardData currentCardData;

    private void Start()
    {
        // Eğer editörden atanmadıysa aynı obje üzerinden çekmeyi dene
        if (gameOverManager == null)
        {
            gameOverManager = GetComponent<GameOverManager>();
        }

        // CardView'in fırlattığı eventleri dinliyoruz
        cardView.OnSwipeRight += HandleEcoChoice;
        cardView.OnSwipeUp += HandleTechChoice;
        cardView.OnSwipeLeft += HandleIndChoice;
        
        HUDManager.Instance.UpdateBars(carbon, budget, support);
        
        // Oyun ilk açıldığında sol üst tur/yıl bilgisini ilklendir
        UpdateTurnDisplay();
        
        LoadNextCard();
    }

    private void OnDestroy()
    {
        // Hata almamak için event aboneliklerini iptal et
        if (cardView != null)
        {
            cardView.OnSwipeRight -= HandleEcoChoice;
            cardView.OnSwipeUp -= HandleTechChoice;
            cardView.OnSwipeLeft -= HandleIndChoice;
        }
    }

    private void LoadNextCard()
    {
        if (availableCards.Count == 0)
        {
            Debug.LogWarning("Deste bitti! Tüm kartlar oynandı.");
            return;
        }

        // Havuzdan rastgele kart çek
        int randomIndex = Random.Range(0, availableCards.Count);
        currentCardData = availableCards[randomIndex];
        availableCards.RemoveAt(randomIndex); // Çekilen kartı listeden çıkar (tekrar gelmesin)

        // UI'a kartı kur
        cardView.SetupCard(currentCardData);
    }

    private void HandleEcoChoice() { ApplyImpact(currentCardData.ecoChoice); }
    private void HandleTechChoice() { ApplyImpact(currentCardData.techChoice); }
    private void HandleIndChoice() { ApplyImpact(currentCardData.indChoice); }

    private void ApplyImpact(CardChoice choice)
    {
        carbon += choice.carbonImpact;
        budget += choice.budgetImpact;
        support += choice.supportImpact;

        // Değerleri 0-100 arasında tut
        carbon = Mathf.Clamp(carbon, 0, 100);
        budget = Mathf.Clamp(budget, 0, 100);
        support = Mathf.Clamp(support, 0, 100);

        Debug.Log($"METRİKLER GÜNCELLENDİ -> Karbon: {carbon} | Bütçe: {budget} | Destek: {support}");
        
        HUDManager.Instance.UpdateBars(carbon, budget, support);
        AdvanceTurn();
    }

    private void AdvanceTurn()
    {
        // 1. Kaybetme Şartları Kontrolü
        if (carbon >= 100)
        {
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver(
                    "KİRLİLİK MAKSİMUM!", 
                    "Atmosferik karbon %100 eşiğini aştı. Şehir yaşanmaz hale geldi ve ekosistem çöktü."
                );
            }
            return;
        }

        if (budget <= 0)
        {
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver(
                    "EKONOMİK İFLAS!", 
                    "Bütçe tamamen tükendi. Yeşil enerji projeleri ve operasyonlar durduruldu."
                );
            }
            return;
        }

        if (support <= 0)
        {
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver(
                    "TOPLUMSAL DESTEK BİTTİ!", 
                    "Halkın güveni kalmadı. Şehir çapında başlayan grevler ve protestolar yönetimi devirdi."
                );
            }
            return;
        }

        currentTurn++;

        // HTML prototipindeki mantık: Her 4 turda 1 yıl ilerler (Yılda 4 mevsim)
        currentYear = 2026 + Mathf.FloorToInt((currentTurn - 1) / 4);

        // Sol üst köşedeki tur ve mevsim göstergesini güncelle
        UpdateTurnDisplay();

        // 2. Zafer Şartı Kontrolü (Maksimum tura ulaşıldığında)
        if (currentTurn > maxTurns)
        {
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver(
                    "YEŞİL ZAFER!", 
                    $"2026-2031 döngüsünü başarıyla tamamladın! Karbon seviyesini kontrol altında tutarak iklim krizini durdurdun."
                );
            }
            return;
        }

        // Yeni tura geçildi, kısa bir gecikme ile yeni kartı yükle
        Invoke(nameof(LoadNextCard), 0.5f);
    }

    private void UpdateTurnDisplay()
    {
        if (HUDManager.Instance != null)
        {
            int seasonIndex = (currentTurn - 1) % 4;
            string activeSeasonName = seasonNames[seasonIndex];
            string activeSeasonIcon = seasonIcons[seasonIndex];

            HUDManager.Instance.UpdateTurnAndSeason(
                currentYear, 
                activeSeasonName, 
                activeSeasonIcon, 
                currentTurn, 
                maxTurns, 
                "1. OYUN: HAVA KOMUTASI", 
                "DENGELİ"
            );
        }
    }
}