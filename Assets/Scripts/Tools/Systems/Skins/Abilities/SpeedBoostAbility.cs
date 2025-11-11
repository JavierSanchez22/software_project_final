using UnityEngine;

[CreateAssetMenu(fileName = "Ability_SpeedBoost", menuName = "Skins/Abilities/Speed Boost")]
public class SpeedBoostAbility : SkinAbility {

	public float speedMultiplier = 1.15f;

	public override void ApplyAbility(GameObject player) {
		DifficultyManager dm = Object.FindObjectOfType<DifficultyManager>();
		if (dm != null) {
			dm.SetSpeedMultiplier(speedMultiplier);
		}
	}

	public override void RemoveAbility(GameObject player) {
		DifficultyManager dm = Object.FindObjectOfType<DifficultyManager>();
		if (dm != null) {
			dm.SetSpeedMultiplier(1f);
		}
	}
}