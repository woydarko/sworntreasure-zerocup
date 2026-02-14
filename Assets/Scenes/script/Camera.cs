using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // Tarik objek Player (Circle) ke sini
    public float smoothing = 5f;    
    public float lockedX = 0f;      // Pastikan ini 0 jika grid kamu di tengah

    void LateUpdate()
    {
        if (target != null)
        {
            // Kamera hanya bergerak di sumbu Y (vertikal)
            Vector3 targetPos = new Vector3(lockedX, target.position.y, -10f);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
        }
    }
}