using UnityEngine;

public class ShotgunSpreadGizmo : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    [Header("Spread Settings")]
    [SerializeField] private bool useRadiusSpread = true;
    [SerializeField] private float spreadRadiusAtDistance = 0.75f;
    [SerializeField] private float spreadDistance = 10f;
    [SerializeField] private float spreadAngle = 10f;

    [Header("Debug")]
    [SerializeField] private Color centerRayColor = Color.green;
    [SerializeField] private Color spreadRayColor = Color.red;
    [SerializeField] private Color circleColor = Color.yellow;

    [SerializeField, Range(4, 64)] private int circleSegments = 24;

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        if (spawnPoint == null)
        {
            return;
        }

        DrawSpread();
    }

    private void DrawSpread()
    {
        Vector3 origin = spawnPoint.position;
        Vector3 forward = spawnPoint.forward;

        float radius = spreadRadiusAtDistance;

        if (!useRadiusSpread)
        {
            float angleRad = spreadAngle * Mathf.Deg2Rad;
            radius = Mathf.Tan(angleRad) * spreadDistance;
        }

        Vector3 center = origin + forward * spreadDistance;

        // 1️⃣ Центральный луч
        Debug.DrawLine(origin, center, centerRayColor);

        // 2️⃣ Краевые лучи конуса (4 направления)
        Vector3 right = spawnPoint.right;
        Vector3 up = spawnPoint.up;

        Debug.DrawLine(origin, center + right * radius, spreadRayColor);
        Debug.DrawLine(origin, center - right * radius, spreadRayColor);
        Debug.DrawLine(origin, center + up * radius, spreadRayColor);
        Debug.DrawLine(origin, center - up * radius, spreadRayColor);

        // 3️⃣ Круг пятна
        DrawCircle(center, forward, radius);
    }

    private void DrawCircle(Vector3 center, Vector3 normal, float radius)
    {
        Vector3 up = Vector3.Cross(normal, Vector3.right);
        if (up == Vector3.zero)
            up = Vector3.Cross(normal, Vector3.up);

        up.Normalize();
        Vector3 right = Vector3.Cross(normal, up).normalized;

        float step = 360f / circleSegments;

        Vector3 prev = center + right * radius;

        for (int i = 1; i <= circleSegments; i++)
        {
            float angle = step * i * Mathf.Deg2Rad;
            Vector3 next =
                center +
                (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * radius;

            Debug.DrawLine(prev, next, circleColor);
            prev = next;
        }
    }
}
