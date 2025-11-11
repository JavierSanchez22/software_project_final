using UnityEngine;

[CreateAssetMenu(fileName = "Ability_HigherJump", menuName = "Skins/Abilities/Higher Jump")]
public class HigherJumpAbility : SkinAbility {

	public float jumpMultiplier = 1.25f;

	public override void ApplyAbility(GameObject player) {
		PlayerMovement movement = player.GetComponent<PlayerMovement>();
		if (movement != null) {
			movement.SetJumpMultiplier(jumpMultiplier);
		}
	}

	public override void RemoveAbility(GameObject player) {
		PlayerMovement movement = player.GetComponent<PlayerMovement>();
		if (movement != null) {
			movement.SetJumpMultiplier(1f);
		}
	}
}