using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HazardOrb : MonoBehaviour, IPointerDownHandler
{
    [Header("Hareket Ayarlari")]
    [SerializeField] private float floatSpeed = 25f;
    [SerializeField] private float floatAmplitude = 15f;
    [SerializeField] private float floatFrequency = 1.5f;

    [Header("Gorsel")]
    [SerializeField] private Image orbGlowImage;

    private Vector2 _startPos;
    private float _randomOffset;
    private bool _isDestroyed = false;

    private void Start()
    {
        _startPos = GetComponent<RectTransform>().anchoredPosition;
        _randomOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        if (_isDestroyed) return;

        // Ekranda organik olarak salinma / suzulme hareketi
        RectTransform rt = (RectTransform)transform;
        float yOffset = Mathf.Sin((Time.time + _randomOffset) * floatFrequency) * floatAmplitude;
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - floatSpeed * Time.deltaTime, _startPos.y + yOffset);

        // Ekranin solundan cikarsa yok et
        if (rt.anchoredPosition.x < -Screen.width / 2f - 100f)
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isDestroyed) return;

        if (LaserShooter.Instance != null)
        {
            LaserShooter.Instance.FireAt(this);
        }
    }

    public void OnHit()
    {
        if (_isDestroyed) return;
        _isDestroyed = true;
        StartCoroutine(PopAndDestroyRoutine());
    }

    private IEnumerator PopAndDestroyRoutine()
    {
        RectTransform rt = (RectTransform)transform;
        Vector3 initialScale = rt.localScale;
        Vector3 targetScale = initialScale * 1.4f;

        float elapsed = 0f;
        float duration = 0.15f;

        // Parlama ve buyuyerek patlama
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rt.localScale = Vector3.Lerp(initialScale, targetScale, t);
            if (orbGlowImage != null)
            {
                Color c = orbGlowImage.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                orbGlowImage.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}