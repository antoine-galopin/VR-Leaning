using UnityEngine;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private float lastCollisionTime = -1f;
    private Vector3 lastCollisionPosition;
    private string logFilePath;

    [SerializeField]
    public string gameName;

    private void Start()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "collision_" + gameName + "_log.txt");

        File.AppendAllText(logFilePath, "=== Collision Log Start ===\n");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("collectible"))
        {
            float currentTime = Time.time;
            Vector3 currentPos = collision.transform.position;

            // Write log entry
            if (lastCollisionTime >= 0f)
            {
                float delay = currentTime - lastCollisionTime;
                float distance = Vector3.Distance(currentPos, lastCollisionPosition);

                string logLine =
                    $"Collision at {currentTime:F3}s | " +
                    $"Delay: {delay:F3}s | " +
                    $"Pos: {currentPos} | " +
                    $"Distance from last: {distance:F3}\n";

                File.AppendAllText(logFilePath, logLine);

                Debug.Log(logLine);
            }
            else
            {
                // First collision
                string logLine =
                    $"First collision at {currentTime:F3}s | " +
                    $"Pos: {currentPos}\n";

                File.AppendAllText(logFilePath, logLine);

                Debug.Log(logLine);
            }

            // Save for next collision
            lastCollisionTime = currentTime;
            lastCollisionPosition = currentPos;

            // Destroy collectible
            Destroy(collision.gameObject);
        }
    }
}
