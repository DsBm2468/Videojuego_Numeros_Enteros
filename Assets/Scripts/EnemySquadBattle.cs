using UnityEngine;

public class EnemySquad : MonoBehaviour
{
    [Header("Panel of math combos")]
    public GameObject canvasMathCombos; // Se selecciona el panel en el que se mostrarán las operaciones a resolver
    private bool battleIiciated = false;

    [Header("Enemy Squad settings")]
    // Se usa [SerializeField] para hacer facil modificaciones a los valores hasta tener los definitivos
    [SerializeField] private float health = 15f; // Vida del enemigo
    [SerializeField] private float damageToPlayer = 2f; // Daño que provoca al jugador
    [SerializeField] public float speed = 4f;
    [SerializeField] public float visionRange = 5f;
    //[SerializeField] private float attackRange = 1.5f; // Distancia mínima para empezar a atacar
    //[SerializeField] private float timeBeetweenAttacks = 1.5f; // Tiempo de espera entre cada ataque

    void Start()
    {
        
    }

    void Update()
    {
        if (!battleIiciated) // Si no está en posición de batalla...
        {
            ScanPerimeter();
        }
    }

    void ScanPerimeter()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(transform.position, visionRange, LayerMask.GetMask("Player")); // Funciona como radar para detectar si el player está cerca

        if (playerHit != null) // Si en la zona se detectó algo...
        {
            Debug.Log("<color=yellow> Player detectado </color>");
            battleIiciated = true;
            MathBattle(playerHit.gameObject);
        }
    }

    void MathBattle(GameObject player)
    {

    }
}
