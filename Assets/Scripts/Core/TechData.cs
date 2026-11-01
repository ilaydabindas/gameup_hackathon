using UnityEngine;

[CreateAssetMenu(fileName = "NewTech", menuName = "EcoRefactor/Tech Data")]
public class TechData : ScriptableObject
{
    public string techId;         // "drone", "fusion", "shield", "forest", "mirror"
    public RealmType realm;       // Air, Soil, Water
    public string techName;       // Örn: "Avcı Dron", "Füzyon Kule"
    public string emoji;          // "🛸", "⚛️" vb.
    public int cost;              // Satın alma maliyeti ($18M gibi)
    public int maxTurns = 5;      // Kaç tur aktif kalacağı (Varsayılan: 5 Tur)
    [TextArea(1, 2)]
    public string description;    // Açıklama (Örn: "Oto CO2 Avı (5T)")
    public Sprite techIcon;
}

