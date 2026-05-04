using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EnemyStandard : MonoBehaviour
{
    [Header("Enemy Standard settings")]
    // Se usa [SerializeField] para hacer facil modificaciones a los valores hasta tener los definitivos
    [SerializeField] private float health = 5f; // Vida del enemigo
    [SerializeField] public float visionRange = 5f;
    [SerializeField] private float damageToPlayer = 1f; // Daño que provoca al jugador
    [SerializeField] private float attackRange = 1.5f; // Distancia mínima para empezar a atacar
    [SerializeField] private float timeBeetweenAttacks = 1.5f; // Tiempo de espera entre cada ataque
    
    //private float nextAttackTime; 
    //private Transform player; // Variable para identificar la ubicación del player

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EnemyTakingDamage(float quantify)
    {
        health -= quantify;
        Debug.Log("Enemigo herido. Vida restante: " + health);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Esto es solo para que el Player reciba daño si el enemigo lo toca
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damageToPlayer);
            }
        }
    }
}
