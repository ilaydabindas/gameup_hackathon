using UnityEngine;
using UnityEngine.UI;
<<<<<<< Updated upstream
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
=======
using System.Collections;

[RequireComponent(typeof(Button))]
public class HazardOrb : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [SerializeField] private float speed = 70f;

    [Header("Hareket Alanı Sınırları (Canvas Piksel)")]
    [SerializeField] private float minX = -450f;
    [SerializeField] private float maxX = 450f;
    [SerializeField] private float minY = -180f;
    [SerializeField] private float maxY = 220f;

    private RectTransform _rt;
    private Image _image;
    private Button _btn;
    private Vector2 _moveDirection;
    private bool _isDestroyed = false;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _btn = GetComponent<Button>();

        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(OnClickOrb);

        // Ekranın herhangi bir yönüne rastgele hareket açısı
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        _moveDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
>>>>>>> Stashed changes
    }

    private void Update()
    {
        if (_isDestroyed) return;

<<<<<<< Updated upstream
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
=======
        // Pozisyonu güncelle
        Vector2 pos = _rt.anchoredPosition + (_moveDirection * (speed * Time.deltaTime));

        // Ekran sınırlarına çarpınca sekme (bounce)
        if (pos.x <= minX)
        {
            pos.x = minX;
            _moveDirection.x = Mathf.Abs(_moveDirection.x);
        }
        else if (pos.x >= maxX)
        {
            pos.x = maxX;
            _moveDirection.x = -Mathf.Abs(_moveDirection.x);
        }

        if (pos.y <= minY)
        {
            pos.y = minY;
            _moveDirection.y = Mathf.Abs(_moveDirection.y);
        }
        else if (pos.y >= maxY)
        {
            pos.y = maxY;
            _moveDirection.y = -Mathf.Abs(_moveDirection.y);
        }

        _rt.anchoredPosition = pos;
    }

    public void OnClickOrb()
    {
        if (_isDestroyed) return;

        LaserShooter shooter = LaserShooter.Instance;
        if (shooter == null)
            shooter = FindFirstObjectByType<LaserShooter>();

        if (shooter != null)
        {
            shooter.FireAt(this);
        }
        else
        {
            OnHit();
>>>>>>> Stashed changes
        }
    }

    public void OnHit()
    {
        if (_isDestroyed) return;
        _isDestroyed = true;
<<<<<<< Updated upstream
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
=======
        _btn.interactable = false;

        // Spawner'a bir topun yok edildiğini bildir
        if (HazardSpawner.Instance != null)
        {
            HazardSpawner.Instance.OnOrbDestroyed();
        }

        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        Vector3 initialScale = _rt.localScale;
        Vector3 targetScale = initialScale * 1.5f;
        Color initialColor = _image != null ? _image.color : Color.white;

        float elapsed = 0f;
        float duration = 0.12f;

>>>>>>> Stashed changes
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
<<<<<<< Updated upstream
            rt.localScale = Vector3.Lerp(initialScale, targetScale, t);
            if (orbGlowImage != null)
            {
                Color c = orbGlowImage.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                orbGlowImage.color = c;
=======
            _rt.localScale = Vector3.Lerp(initialScale, targetScale, t);
            if (_image != null)
            {
                Color c = initialColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                _image.color = c;
>>>>>>> Stashed changes
            }
            yield return null;
        }

        Destroy(gameObject);
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
