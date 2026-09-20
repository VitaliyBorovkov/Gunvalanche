using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class DamageTextUIController : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float floatSpeed = 1.5f;

    private TextMeshPro text;
    private Camera cam;
    private ObjectPool objectPool;
    private float timer;
    private Color startColor;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        startColor = text.color;
    }

    private void OnEnable()
    {
        timer = 0f;
        text.color = startColor;
    }

    public void Initialize(ObjectPool pool, int damage)
    {
        objectPool = pool;
        text.text = damage.ToString();
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam != null)
        {
            transform.rotation = cam.transform.rotation;
        }

        timer += Time.deltaTime;
        float alpha = Mathf.Clamp01(1f - timer / lifetime);
        text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer >= lifetime)
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        if (objectPool != null)
        {
            objectPool.Despawn(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
