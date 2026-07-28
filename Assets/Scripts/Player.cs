using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Life Statistics")]
    [SerializeField] private float maxHealth = 6f; // Se usa [SerializeField] para hacer facil modificaciones a los valores hasta tener los definitivos
    [SerializeField] private float health = 6f;
    public bool isTutorialScene = false; // Se activa esto solo en la escena del tutorial
    public bool isAlive = true; // Estado actual
    public TextMeshProUGUI TextLevelHealthUI;

    private Vector3 originalSizePlayer;

    [Header("Player Movement")]
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float runSpeed = 15f;
    [SerializeField] private float jumpForce = 12f; // Fuerza del impulso del salto
    [SerializeField] private int maxJumps = 2;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.5f; // El alcance del golpe
    [SerializeField] private float timeBetweenAttacks = 0.05f; // Tiempo de espera entre ataques
    private float nextAttackTime = 0; // Contador interno para el cooldown de ataques

    [Header("Shield Settings")]
    [SerializeField] private GameObject Shield; // Visual del hechizo de protección (Escudo)
    [SerializeField] private float shieldDuration = 3f; // Tiempo que dura el escudo activo
    private float shieldActiveTimer = 0f; // Control interno del tiempo del escudo

    [Header("Detection of Input Actions")]
    private Rigidbody2D rbPlayer; // Detecta las físicas implementadas en el input actions
    private Vector2 moveInput;
    private int jumpsRemaining; // Contador de saltos restantes del player durante cierto tiempo
    private bool isRunning; // Detecta si shift está presionado (Si el player está corriendo), al ser escrito asi, automáticamente empieza siendo false
    private bool isCrouching; // Detecta si la tecla de abajo está presionada (Si el player se agachó), al ser escrito asi, automáticamente empieza siendo false
    private bool usingShield; // Detecta si la tecla V está presionada (Si el player activó el escudo), al ser escrito asi, automáticamente empieza siendo false
    private float nextRepulsionTime; 

    void Awake()
    { 
        // Se encarga que al iniciar el juego, el jugador permanezca en el escenario, esto permite conectar componentes, a diferencia del start que da valores iniciales
        rbPlayer = GetComponent<Rigidbody2D>(); // Busca al componente rigidbody en el player
        rbPlayer.freezeRotation = true; // Hace que el objeto no gire
        jumpsRemaining = maxJumps; // Inicialmente el contador de saltos que puede dar el player estará al máximo
        originalSizePlayer = transform.localScale; // El tamaño original del player se guardará desde el inicio
    }

    void Start()
    {
        if (TextLevelHealthUI != null)
        {
            TextLevelHealthUI.text = "Vida: " + health;
        }
    }

    // Control de botones del player
    // MECÁNICAS DE MOVIMIENTO / INTERACCIÓN CON EL AMBIENTE

    public void OnMove(InputAction.CallbackContext context)
    {
        // Según la tecla presionada y la presión de esta, se detecta el input que va a hacerse

        moveInput = context.ReadValue<Vector2>(); // Registra la dirección que el jugador asignó
        // Estos valores en x que definen el movimiento son detecdos como: -1 left, 0 static y 1 right

        //Debug.Log("Tecla presionada, valor: " + moveInput.x);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        // Según la tecla presionada y la presión de esta, se detecta el input que va a hacerse

        if (context.performed) // Si se ha hecho una interacción (presionado la tecla shift)...
        {
            isRunning = true; // Entonces el player está corriendo
        }
        else // Si no, entonces seguirá caminando
        {
            isRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Según la tecla presionada y la presión de esta, se detecta el input que va a hacerse

        if (Mathf.Abs(rbPlayer.linearVelocity.y) < 0.01f) // Mathf.Abs(...) convierte los valores en positivo, además se usa linea velocity.y para que detecte la velocidad del player, dependiendo de esto se ajuste, el 0.01 es un margen de error
                                                          // Si hubo movimiento vertical, por más pequeño que sea (por eso ese margen de error)... CADA QUE ESTÁ EN EL PISO SE REINICIARÁ EL CONTADOR DE SALTOS RESTANTES
        {
            jumpsRemaining = maxJumps;
        }
        if (context.started && jumpsRemaining > 0) // Si fue presionada la barra espaciadora y aún le quedan saltos restantes al player...
        { 
            rbPlayer.linearVelocity = new Vector2(rbPlayer.linearVelocity.x,0); // Mantiene la velocidad de los lados para no frenar en seco, pero permite aplicar un impulso nuevo de salto desde 0 (Se crea un nuevo vector en donde se tiene esta información)
            rbPlayer.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // indica el movimiento de saltar
            // Usa el addForce con Impulse que actua como una explosión debajo del personaje, dandole la energia para hacer el movimiento para que parezca real

            jumpsRemaining--;
            Debug.Log("Saltos restantes: " + jumpsRemaining);
        }

        //if (context.started && Mathf.Abs(rbPlayer.linearVelocity.y) < 0.01f) // context.started detecta el momento exacto en el que se presiona la tecla de abajo, evitando que salte varias veces seguidas como pasa al usar performed
        //    // Mathf.Abs(...) convierte los valores en positivo, además se usa linea velocity.y para que detecte la velocidad del player, dependiendo de esto se ajuste, el 0.01 es un margen de error
        //    // Si fue presionada la barra espaciadora y hubo movimiento vertical, por más pequeño que sea (por eso ese margen de error)
        //{
        //    rbPlayer.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // indica el movimiento de saltar
        //    // Usa el addForce con Impulse que actua como una explosión debajo del personaje, dandole la energia para hacer el movimiento para que parezca real
        //}
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)  // Si se ha hecho una interacción (presionado la tecla S o la flecha de abajo)...
        {
            isCrouching = true;
            transform.localScale = new Vector3(originalSizePlayer.x, originalSizePlayer.y * 0.5f, originalSizePlayer.z); // Se reduce la escala vertical del player a la mitad para que se agache (multiplicarlo por 0.5 reduce el tamaño vertical al 50%)
            Debug.Log("Player agachado");
        }
        else if(context.canceled)
        {
            isCrouching = false;
            ReturnOriginalSize();
        }
    }

    // MECÁNICAS DE COMBATE BÁSICO

    public void OnFastAttack(InputAction.CallbackContext context) // Ataque rápido
    {
        if (!EnemySquadBattle.IsInMathBattle) // Revisa: Si en el enemigo Squad no se activo el modo de estar en batalla matemática...
        {
            // Entonces la tecla Z será usada como ataque rápido

            if (context.started && Time.time >= nextAttackTime) // context.started detecta el momento exacto en el que se presiona la tecla, evitando que ataque varias veces seguidas como pasa al usar performed
            // Si presiona el botón Z y si ya pasó el tiempo de espera entre ataques
            {
                Debug.Log("Ataque rápido ejecutado");
                GiveAttack(1f, 0f); // Envia el ataque al enemigo, en este caso al ser un ataque rápido, le otorga 1 punto de daño (Daño físico, empuje ocasionado por el ataque)
                nextAttackTime = Time.time + timeBetweenAttacks; // Hace que tengas que esperar un poco para dar tu siguiente ataque
            }
        }
        // Si no fue así, entonces Z será usada como confirmación de respuesta
    }

    public void OnHeavyAttack(InputAction.CallbackContext context) // Ataque pesado
    {
        if (!EnemySquadBattle.IsInMathBattle) // Revisa: Si en el enemigo Squad no se activo el modo de estar en batalla matemática...
        {
            // Entonces la tecla X será usada como ataque pesado

            if (context.started && Time.time >= nextAttackTime) // context.started detecta el momento exacto en el que se presiona la tecla, evitando que ataque varias veces seguidas como pasa al usar performed
            // Si presiona el botón X y si ya pasó el tiempo de espera entre ataques
            {
                Debug.Log("Ataque pesado ejecutado");
                GiveAttack(2f, 10f); // Envia el ataque al enemigo, en este caso al ser un ataque pesado, le otorga 2 puntos de daño (Daño físico, empuje ocasionado por el ataque)
                nextAttackTime = Time.time + timeBetweenAttacks; // Hace que tengas que esperar un poco para dar tu siguiente ataque
            }
        }
    }

    public void OnCounterAttack(InputAction.CallbackContext context) // Ataque de repulsión
    {
        if (!EnemySquadBattle.IsInMathBattle) // Revisa: Si en el enemigo Squad no se activo el modo de estar en batalla matemática...
        {
            // Entonces la tecla C será usada como ataque de repulsión
            if (context.started && Time.time >= nextRepulsionTime) // context.started detecta el momento exacto en el que se presiona la tecla, evitando que ataque varias veces seguidas como pasa al usar performed
            // Si presiona el botón C y si ya pasó el tiempo de espera entre ataques
            {
                Debug.Log("Ataque de repulsión ejecutado");
                GiveAttack(0.5f, 15f); // Envia el ataque al enemigo, en este caso al ser un ataque de reulsión, le otorga 0.5 puntoa de daño (Daño físico, empuje ocasionado por el ataque)
                nextRepulsionTime = Time.time + timeBetweenAttacks;
                //nextAttackTime = Time.time + timeBetweenAttacks; // Hace que tengas que esperar un poco para dar tu siguiente ataque
            }
        }
    }

    public void OnShield(InputAction.CallbackContext context) // Escudo
    {
        if (!EnemySquadBattle.IsInMathBattle) // Revisa: Si en el enemigo Squad no se activo el modo de estar en batalla matemática...
        {
            // Entonces la tecla V será usada como ataque pesado
            if (context.started && !usingShield) // context.started detecta el momento exacto en el que se presiona la tecla, evitando que ataque varias veces seguidas como pasa al usar performed
            // Si presiona el botón V y si ya pasó el tiempo de espera entre ataques
            {
                usingShield = true; // El player usa el escudo
                if (Shield != null)
                {
                    Shield.SetActive(true);
                }
                Invoke(nameof(DesactivateShield), shieldDuration);// Se programa la alarma para que el escudo se quite en el tiempo indicado luego se haber si presionado la tecla
            }
            //else if (context.canceled)
            //{
            //    usingShield = false;
            //    if(Shield != null)
            //    {
            //        Shield.SetActive(false);
            //    }
            //}
        }
    }
    private void DesactivateShield()
    {
        usingShield = false;
        if(Shield != null)
        {
            Shield.SetActive(false);
        }
    }

    private void GiveAttack(float damageCaused, float thrustApplied) // Detecta el ataque lanzado al enemigo, indicando el daño realizado al enemigo y el empuje que se haya hecho)
    {
        Collider2D[] enemys = Physics2D.OverlapCircleAll(transform.position, attackRange); // Guarda información sobre el area alrededor del player

        foreach (Collider2D obj in enemys)
        {
            if (obj.CompareTag("Enemy")) // Si lo que golpeó es un enemigo...
            {
                EnemyStandard scriptEnemyStandard = obj.GetComponent<EnemyStandard>(); // Se crea una variable temporal para acceder a la información de salud actual del enemigo, de esta manera se define hasta que momento se atacará

                EnemySquadBattle scriptEnemySquad = obj.GetComponent<EnemySquadBattle>();

                TrainingDummy scriptDummy = obj.GetComponent<TrainingDummy>();

                if (scriptEnemyStandard != null)
                {
                    scriptEnemyStandard.EnemyTakingDamage(damageCaused); // Dar la orden de recibir daño
                }

                if (scriptDummy != null)
                {
                    scriptDummy.DummyTakingDamage(damageCaused); // Dar la orden de recibir daño
                }

                if (thrustApplied > 0) // APLICAR EMPUJE (Si es que un valor llego a ser mayor a 0)
                {
                    Vector2 direction = (obj.transform.position - transform.position).normalized; // .normalized hace que unity olvide la distancia anterior
                    obj.GetComponent<Rigidbody2D>().AddForce(direction * thrustApplied, ForceMode2D.Impulse);
                }
            }
        }
    }

    public void TakeDamage(float quantify) // Sistema de vida del jugador
    {
        if (usingShield) // Si está usando el escudo, no se reduce la vida
        {
            return;
        }
        else // Sino, cuando el player es atacado, va reduciendo la cantidad de vida
        {
            health -= quantify;

            if (isTutorialScene && health < 1f) // Si el player está en el tutorial y su vida quiere llegar a 1...
            {
                health = 1f; // El valor queda congelado para que en el tutorial no llegue al gameover
            }

            Debug.Log("Vida Player: " + health);

            if (TextLevelHealthUI != null)
            {
                TextLevelHealthUI.text = "Vida: " + health;
            }

            if (health <= 0f)
            {
                isAlive = false;
                GameOverController EndPlayer = Object.FindFirstObjectByType<GameOverController>(); // Se llama al script del game over
                if (EndPlayer != null)
                {
                    EndPlayer.ActivateGameOver(); // Se muestra la pantalla de game over
                }

                Destroy(gameObject);
            }
        }
    }

    public void Heal(float valueHealth) // Recuperación de vida del player
    {
        if (health >= maxHealth) return; // Si la vida ya está al máximo (6), no pasa nada

        health = Mathf.Min(health + valueHealth, maxHealth);
        // Mathf.Min( ... , ... ) compara dos números y devuelve el más pequeño de ellos(Si el player tiene 5puntos y medio y con un item recupera 2 puntos, el valor sergirá siendo 6)

        if (TextLevelHealthUI != null)
        {
            TextLevelHealthUI.text = "Vida: " + health;
        }
        Debug.Log("Energía recuperada. Vida actual: " + health);
    }

    void Update()
    {
        // Actualización del movimiento del player

        // Si el personaje está estático (ej. metido en un acertijo), no intentamos moverlo
        if (rbPlayer.bodyType == RigidbodyType2D.Static) return;

        // Según el nuevo botón presionado, si detecta que está corriendo,
        // utilizará la velocidad de correr, si no, la velocidad actual seguirá siendo la de caminar.

        float currentSpeed = isRunning ? runSpeed : walkSpeed; // El valor inicial de isRunning es false

        // Se define la velocidad horizontal que tendrá el player. Multiplicando la dirección por la velocidad actual
        float horizontalVelocity = moveInput.x * currentSpeed;
        
        // Da el resultado al rigidbody para que el personaje se desplace
        rbPlayer.linearVelocity = new Vector2(horizontalVelocity, rbPlayer.linearVelocity.y); // Se usa este nombre (linearVelocity) porque actualmente es rl más compatible con la vversión de unity 6
    }

    public void ReturnOriginalSize()
    {
        transform.localScale = originalSizePlayer; // Vuelve al valor original del player
        Debug.Log("Player vuelve al tamaño original");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
