using UnityEngine;

[CreateAssetMenu(fileName = "Ability_ExtraLife", menuName = "Skins/Abilities/Extra Life")]
public class ExtraLifeAbility : SkinAbility {

	public override void ApplyAbility(GameObject player) {
		GameState gsm = Object.FindObjectOfType<GameState>();
		if (gsm != null) {
			gsm.SetExtraLife(true);
		}
	}

	public override void RemoveAbility(GameObject player) {
		GameState gsm = Object.FindObjectOfType<GameState>();
		if (gsm != null) {
			gsm.SetExtraLife(false);
		}
	}
}