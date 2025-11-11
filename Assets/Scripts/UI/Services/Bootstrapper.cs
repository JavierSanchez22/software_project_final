using UnityEngine;
using System.Collections; // <-- ¡AÑADE ESTO!

public class Bootstrapper : MonoBehaviour {

    private UIManager _uiManager;

	private void Start() {
        // Encontramos la referencia al UIManager
        _uiManager = FindObjectOfType<UIManager>();
        
        // Iniciamos la corrutina de carga
        StartCoroutine(LoadGameSequence());
	}
    
    private IEnumerator LoadGameSequence() {
        // En este punto, el Loading Screen (activado por defecto)
        // ya es visible y cubre la pantalla.

        // 1. (Opcional) Esperar un frame para asegurarnos
        // de que la pantalla de carga se ha renderizado
        yield return null; 

        // 2. Ejecutar la tarea "lenta" (Cargar datos del disco)
        // Esto ya no congelará el juego al inicio.
        SaveSystem.LoadAllData();
        
        // 3. Esperar un tiempo mínimo para que la pantalla
        // de carga no sea un flash si la carga es muy rápida.
        // AJUSTA ESTE TIEMPO (ej. 1.0f)
        yield return new WaitForSeconds(1.0f); 
        
        // 4. Decirle a la UI que termine la carga
        if (_uiManager != null) {
            _uiManager.ShowMainMenu();
        }
    }
}