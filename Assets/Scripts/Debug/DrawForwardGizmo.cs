using UnityEngine;

public class DrawForwardGizmo : MonoBehaviour
{
    public float length = 0.5f;
    public Color color = Color.blue;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * length);
        Gizmos.DrawSphere(transform.position + transform.forward * length, 0.01f);
    }
}
