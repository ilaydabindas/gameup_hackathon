using UnityEngine;

[System.Serializable]
public class CardChoice
{
    public string choiceLabel;  // Örn: "Doğa Dostu"
    public int carbonImpact;    // Örn: -10 (Kirliliği azaltır)
    public int budgetImpact;    // Örn: -5 (Bütçe gider)
    public int supportImpact;   // Örn: +8 (Halk desteği artar)
}

[CreateAssetMenu(fileName = "New Crisis Card", menuName = "EcoRefactor/Crisis Card")]
public class CrisisCardData : ScriptableObject
{
    public string sectorTag;     // Örn: SANAYİ BÖLGESİ
    public string title;         // Örn: Kömür Santrali Tam Güç!
    [TextArea(3, 5)]
    public string description;

    [Header("Karar Etkileri")]
    public CardChoice ecoChoice;  // SAĞA Kaydırma (Doğa)
    public CardChoice techChoice; // YUKARI Kaydırma (Teknoloji)
    public CardChoice indChoice;  // SOLA Kaydırma (Endüstri)
}