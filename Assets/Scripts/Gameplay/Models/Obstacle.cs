using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : Hitable {
	[Tooltip("The minimum angle threshold which colliding with obstacles will result in gameover")]
	[SerializeField] private float _angleThreshold = 40f;

	protected override void OnCollisionEnter(Collision other) {
		if ((other.gameObject.CompareTag("Hitable Destroyer") || other.gameObject.CompareTag("Collapse Destroyer")) && !isReleased)
			_killAction(this);
		
        // --- LÍNEA MODIFICADA ---
		bool shouldObstacleDie = PlayerCollisionHelper.HandleObstacleCollision(other, _angleThreshold);
        if (shouldObstacleDie && !isReleased) {
            _killAction(this);
        }
	}

	public override Vector3 GenerateRandomPosition(float horizontalPosition) {
		return new Vector3(horizontalPosition, transform.position.y, transform.position.z);
	}
}