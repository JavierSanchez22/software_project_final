using UnityEngine;

public static class PlayerCollisionHelper {

	public static bool HandleObstacleCollision(Collision other, float angleThreshold) {
		if (other.gameObject.CompareTag("Player")) {
			for (int i = 0; i < other.contacts.Length; i++) {
				float currentAngleRefUp = Vector3.Angle(other.contacts[i].normal, Vector3.up);
				float currentAngleRefDown = Vector3.Angle(other.contacts[i].normal, Vector3.down);

				if (currentAngleRefUp <= angleThreshold || currentAngleRefDown <= angleThreshold) {
					PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();
					if (movement != null) {
						movement.RechargeJumps();
					}
					return false;
				}
				else {
					GameState gsm = Object.FindObjectOfType<GameState>();
					if (gsm != null) {
						if (gsm.HasExtraLife) {
							gsm.SetExtraLife(false);
							AudioService.Instance.PlaySoundOneShot(Sound.Type.Switch, 2);
							return true; // <-- Devuelve TRUE (destruir obstáculo)
						} else {
							gsm.GameOver(Sound.Type.Death);
							return false; // El juego terminó, no importa
						}
					}
				}
			}
		}
		return false;
	}
}