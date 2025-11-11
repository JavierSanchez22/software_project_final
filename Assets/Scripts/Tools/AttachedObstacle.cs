using UnityEngine;

public class AttachedObstacle : MonoBehaviour {
	[Tooltip("The minimum angle threshold which colliding with obstacles will result in gameover")]
	[SerializeField] private float _angleThreshold = 40f;

	private void OnCollisionEnter(Collision other) {
        // --- LÍNEA MODIFICADA ---
		bool shouldObstacleDie = PlayerCollisionHelper.HandleObstacleCollision(other, _angleThreshold);
        if (shouldObstacleDie) {
            Destroy(gameObject); // Este no es "hitable", así que lo destruimos
        }
	}
}