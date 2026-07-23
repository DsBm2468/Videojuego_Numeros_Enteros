using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class HealthItem : MonoBehaviour
{
    [SerializeField] private float healAmount = 2f;

    // Este se ejecuta automáticamente cuando cumple alguna de estas condiciones:
    //     * Ambos objetos deben tener un Collider2D (no importa la forma)
    //     * Al menos uno de los dos objetos debe tener un Rigidbody2D
    //     * Al menos UNO de los dos objetos debe tener marcada la casilla Is Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Si el objeto colisiona con un objeto del Tag Player...
        {
            Player playerScript = collision.GetComponent<Player>(); // Se indica una variable con acceso al script del player

            if (playerScript != null)
            {
                playerScript.Heal(healAmount); // Se aplica la función Heal del script del Player
                Destroy(gameObject); // Finalmente, el item se destruye
            }
        }
    }
}
