using UnityEngine;

public class ResetSizePlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectamos si el objeto que cruzó tiene la etiqueta "Player"
        if (collision.CompareTag("Player"))
        {
            // Buscamos el componente del jugador
            Player playerScript = collision.GetComponent<Player>();

            if (playerScript != null)
            {
                // Le ordenamos al script del jugador que regrese a su escala inicial
                playerScript.ReturnOriginalSize();
                Debug.Log("¡Escala del jugador restaurada con éxito al cruzar el punto invisible!");
            }
        }
    }
    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Verde semitransparente
            Gizmos.DrawCube(transform.position, collider.size);
        }
    }
}
