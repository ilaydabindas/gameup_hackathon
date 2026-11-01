using UnityEngine;

public enum RealmType { Air, Soil, Water }
public enum ChoiceType { Eco, Tech, Industry }

[System.Serializable]
public struct CardChoice
{
    public string label;          // Butondaki yazı (örn: "Kömürü Kapat")
    public ChoiceType type;       // ECO, TECH, IND
    public int carbonDelta;       // Kirlilik/Erozyon etkisi
    public int budgetDelta;       // Bütçe etkisi
    public int supportDelta;      // Destek etkisi
}

[CreateAssetMenu(fileName = "NewCard", menuName = "EcoRefactor/Card Data")]
public class CardData : ScriptableObject
{
    public RealmType realm;       // Hava, Toprak, Su
    public string sector;         // Örn: "SANAYİ BÖLGESİ"
    public string title;          // Örn: "Kömür Santrali Tam Güç!"
    [TextArea(2, 4)]
    public string description;    // Kart açıklaması
    public string emoji;          // "🏭"
    public Sprite cardIcon;
    public bool isTimed;          // Süreli acil alarm mı?

    [Header("Seçenekler")]
    public CardChoice leftChoice;   // Sola swipe / Sanayi (IND)
    public CardChoice upChoice;     // Yukarı swipe / Teknoloji (TECH)
    public CardChoice rightChoice;  // Sağa swipe / Doğa (ECO)
}
