using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [Header("Dummy settings")]
    // Se usa [SerializeField] para hacer facil modificaciones a los valores hasta tener los definitivos
    [SerializeField] private float maxHealth = 10;
    private float currentHealth;

    [Header("Visual Effect when its damaged")]
    [SerializeField] private SpriteRenderer SpriteDummyRenderer; // Sprite del maniquí
    [SerializeField] private Color damageColor = new Color(1f, 0.4f, 0.4f, 1f);
    [SerializeField] private float flashDuration = 0.15f;
    private Color originalColor;

    [Header("Shoot setting (Shield Tutorial)")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform beginPointProjectile; // Punto de lanzamiento de proyectil
    [SerializeField] private float TimeBetweenProjectile = 2f;
    [SerializeField] private bool canShoot = false; // Inicialmente, no podrá disparár hasta llegar el momento de combate básico en el tutorial
    private float nextShootTime = 0; // Contador interno para el cooldown de ataques (Cronometro para indicar cuando el maniquí tiene permitido disparar nuevamente)

    void Start()
    {
        currentHealth = maxHealth;

        if (SpriteDummyRenderer == null) // Si no tiene su sprite definido...
        {
            SpriteDummyRenderer = GetComponent<SpriteRenderer>(); // Le agrega el componente automáticamente
        }

        if (SpriteDummyRenderer != null) // Si ya lo encontró...
        {
            originalColor = SpriteDummyRenderer.color; // Guardará el color original del sprite para tenerlo a la mano
        }
    }

    void Update()
    {
        if (canShoot == true && Time.time >= nextShootTime)  // Si el tutorial ya se encuentra en la fase de combate básico y ya pasó el tiempo de espera para el siguiente disparo...
        {
            Shoot(); // Se ejecuta el disparo
        }
    }

    public void Shoot()
    {
        nextShootTime = Time.time + TimeBetweenProjectile; // Hace que tengas que esperar un poco para dar tu siguiente ataque (reinicia el contador)

        if (projectile != null && beginPointProjectile != null) // Si fue asignado el proyectil y el punto de disparo de este...
        {
            Instantiate(projectile, beginPointProjectile.position, beginPointProjectile.rotation); // Se crean clones del proyectile en el punto de lanzamiento

            Debug.Log("<color=yellow>Disparo del dummy ejecutado</color>");
        }
    }

    public void DummyTakingDamage(float quantify)
    {
        currentHealth -= quantify;
        Debug.Log($"El maniquí recibió {quantify} de daño. Vida restante: " + currentHealth);

        if (SpriteDummyRenderer != null)
        {
            StopAllCoroutines();// Detiene los parpadeos anteriores
            StartCoroutine(FlashRedDummy());
        }

        if (currentHealth <= 0)
        {
            ResetDummy();
        }
    }

    // IEnumerator es conocido como corrutina, es una función que va a pausarse en algún momento, por tal razon es necesario el uso de yield return para pausarlo
    private System.Collections.IEnumerator FlashRedDummy() // Cambia el color del sprite debido al daño en modo de parpadeo o flash (ejecutandose frame a frame)
    {
        SpriteDummyRenderer.color = damageColor; // El maniquí cambia de color a rojo
            yield return new WaitForSeconds(flashDuration); // Se pausa la ejecución de la corrutina en este punto y lo reanuda justo en el siguiente frame del juego
        // Espera el tiempo indicado previamente antes de volver al maniquí a su color original (siguiente linea de codigo)

        SpriteDummyRenderer.color = originalColor;
    }

    // --------------------------------------------------------

    // Funciones que serán activadas desde TutorialManager.cs
    public void StartShooting() => canShoot = true;

    public void StopShooting() => canShoot = false;

    // --------------------------------------------------------
    public void ResetDummy()
    {
        currentHealth = maxHealth;
        Debug.Log("<color=green>[MANIQUÍ]</color> ¡Vida reiniciada al máximo!");
    }
}