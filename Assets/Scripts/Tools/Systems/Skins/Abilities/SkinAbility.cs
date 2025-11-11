using UnityEngine;

public abstract class SkinAbility : ScriptableObject {
	
	public string abilityName = "New Ability";
	[TextArea]
	public string abilityDescription = "Ability description.";

	// Método que se llama cuando la skin es equipada
	// Es "abstract" para forzar a las otras clases a implementarlo
	public abstract void ApplyAbility(GameObject player);

	// Método que se llama cuando la skin es desequipada
	public abstract void RemoveAbility(GameObject player);
}