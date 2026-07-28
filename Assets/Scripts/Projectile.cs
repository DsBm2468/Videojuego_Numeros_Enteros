using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 1f; // Daño que hace al jugador
    [SerializeField] private float lifeTimeProjectile = 4f; // Segundos antes de destruirse

    private Vector2 directionObjetive; // Guarda la dirección a donde volará el proyectil

    void Start()
    {
        Destroy(gameObject, lifeTimeProjectile); // Se destruye el proyectil si ha pasado el tiempo y no le a dado a nada, esto se hace para no amontonar
        // gameObject es el mismo(el proyectil)

        GameObject Player = GameObject.FindGameObjectWithTag("Player"); // Busca la posición del player

        if (Player != null)
        {
            directionObjetive = (Player.transform.position - transform.position).normalized; // Se indica que la dirección a donde se dirigue el proyctil, se hace a partir de la resta de la posicion del player con la del origen del proyectil
        }
    }

    void Update()
    {
        transform.Translate(directionObjetive * speed * Time.deltaTime, Space.World); // Se mueve el proyectil frame por frame a la dirección indicada
    }

    private void OnTriggerEnter2D(Collider2D collision) // Se usa Ontrigger ya que atraviesa el empty en vez de moverlo (colisionarlo)
    {
        if (collision.CompareTag("Player")) // Si el objeto colisiona con un objeto del Tag Player...
        {
            Debug.Log("<color=red>El proyectil impactó al jugador</color>");
            Destroy(gameObject); // Finalmente, el item se destruye
        }
    }
}