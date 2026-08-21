using NUnit.Framework;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[System.Serializable]
public class MathCombos
{
    public string Operation;
    public int Answer;
}

public class EnemySquadBattle : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineCamera virtualCamera;
    public float zoomCamera = 3f;
    private float originalZoomCamera;
    
    [Header("Panel of math combos")]
    public GameObject canvasMathCombos; // Se selecciona el panel en el que se mostrarán las operaciones a resolver
    private bool battleIniciated = false;
    public static bool IsInMathBattle = false; // Así se identifica si la tecla z será usada como confirmación de respuesta o como ataque rápido

    [Header("Math combos progress")]
    private int answersInRow = 0; // Contador de respuestas correctas consecutivas del jugador
    private float healthBeforeCombo;

    [Header("Timer settings")]
    public float timeToAnswer; // Tiempo en segundos para responder
    //public TMPro.TextMeshProUGUI timerText;
    public Slider timerSlider; // Barra de tiempo
    private Coroutine timerCoroutine; // Control interno para apagar y quitar el reloj de la pantalla


    public TMPro.TextMeshProUGUI playerInputText; // Referencia al objeto de texto de la UI (La respuesta del player)
    public TMPro.TextMeshProUGUI operationText; // Referencia al objeto de texto de la UI (La operación matemática que aparecerá en el panel de combos)
                                                // TMPro.TextMeshProUGUI es el tipo de dato
                                                // TMPro es el espacio de nombres (namespace) de TextMesh Pro
                                                // TextMeshProUGUI es la clase específica para texto de UI dentro de un Canvas
    public TMPro.TextMeshProUGUI feedbackText; // Referencia al objeto de texto de la UI (Indica si la respuesta del player fue correcta o no)
    
    private int correctAnswer; // Información sobre la respuesta correcta de las operaciones
    private string currentInput = ""; // Respuesta dada por el jugador
    public List<MathCombos> OperationsListLevel1;

    [Header("Enemy Squad settings")]
    // Se usa [SerializeField] para hacer facil modificaciones a los valores hasta tener los definitivos
    [SerializeField] private float health = 15f; // Vida del enemigo
    [SerializeField] private float damageToPlayer = 2f; // Daño que provoca al jugador
    [SerializeField] public float speed = 4f;
    [SerializeField] public float visionRange = 3.5f;
    //[SerializeField] private float attackRange = 1.5f; // Distancia mínima para empezar a atacar
    //[SerializeField] private float timeBeetweenAttacks = 1.5f; // Tiempo de espera entre cada ataque
    [SerializeField] private InputActionReference ConfirmAnswer;

    void Start()
    {
        originalZoomCamera = virtualCamera.Lens.OrthographicSize; // Guarda el tamaño original de la cámara, .Lens contiene las propiedades de la camara de cinemachine
        canvasMathCombos.SetActive(false); // Inicialmente el panel estará oculto
    }

    void Update()
    {
        if (!battleIniciated) // Si no está en posición de batalla...
        {
            ScanPerimeter(); // El enemigo revisará el área
        }
        else
        {
            ReadPlayerInput(); // Si no, entonces la batalla ya inició, entonces prosigue en detectar las teclas dadas por el player
        }
    }

    void ScanPerimeter()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(transform.position, visionRange, LayerMask.GetMask("Player")); // Funciona como radar para detectar si el player está cerca

        if (playerHit != null) // Si en la zona se detectó algo...
        {
            Debug.Log("<color=yellow> Player detectado, INICIO DE BATALLA</color>");
            battleIniciated = true;
            healthBeforeCombo = health; // La vida del enemigo quedará al máximo
            answersInRow = 0; // Se reinicia el contador de respuestas correctas
            MathBattle(playerHit.gameObject); // Se inicia la batalla
        }
    }

    void MathBattle(GameObject player)
    {
        IsInMathBattle = true; // Activa la indicación que z será usada para confirmacion de respuesta
        canvasMathCombos.SetActive(true); // Aparece el panel de batalla por combos matemáticos
        virtualCamera.Lens.OrthographicSize = zoomCamera; // Hace el zoom indicado hacia la escena de combate
        feedbackText.text = "";

        //NUEVO
        // -----------------------------------
        DisablePlayerMovement(player); // Se llama el void para desactivar los actions del action map de Player
        // -----------------------------------

        OperationInScreen(); // Se llama a una de las operaciones en pantalla

        if (timerCoroutine != null) StopCoroutine(timerCoroutine); // Si había anteriormente un contador corriendo, entonces será apagado
        timerCoroutine = StartCoroutine(StartTimerRoutine()); // Se inicia el contador del tiempo para responder las operaciones
    }

    //NUEVO
    // -----------------------------------
    private void DisablePlayerMovement(GameObject player) // DESACTIVAR MOVIMIENTO Y DEMÁS MECANICAS DEL PLAYER, DEJANDO SOLO HABILITADA LA DE CONFIRMAR RESPUESTA
    {
        if (player != null)
        {
            PlayerInput plInput = player.GetComponent<PlayerInput>(); // Tiene a la mano las actions del action map de player
            if (plInput != null)
            {
                plInput.actions.FindActionMap("Player")?.Disable(); // Se desactiva unicamente el action Map indicado en esta linea (Player)
            }

            Rigidbody2D plRb = player.GetComponent<Rigidbody2D>();
            if (plRb != null)
            {
                plRb.linearVelocity = Vector2.zero; // Se frena de golpe al player
            }
        }
    }
    // -----------------------------------

    System.Collections.IEnumerator StartTimerRoutine() // Inicia el temporizador de la barra (ejecutandose frame a frame)
    {
        float timeRemaining = timeToAnswer; // Se crea una cuenta regresiva para responder las operaciones antes que este tiempo se acabe, se crea esta nueva variable para evitar cambios no deseados en el valor original que está en timeToAnswer
        while (timeRemaining > 0) // Mientras que el tiempo restante para contestar sea mayor a 0 ...
        {
            if (timerSlider != null) // Si la barra de tiempo está asignada...
            {
                timerSlider.value = timeRemaining / timeToAnswer; // Actualizará su valor dividiendo el tiempo actual con el tiempo que tiene el usuario para responder
            }
            yield return null; // Se pausa la ejecución de la corrutina en este punto y lo reanuda justo en el siguiente frame del juego
            // Esto hace que el movimiento de reducción de tiempo sea suave y continuo
            timeRemaining -= Time.deltaTime; // Entonces, se va restando el tiempo entre el fotograma anterior y el actual
        }

        if (timeRemaining <= 0)
        {
            feedbackText.text = "<color=red>¡TIEMPO AGOTADO!</color>";
            PlayerTakingDamage();
            Invoke("ProcessError", 1.5f);
            //EndBattle();
        }
    }

    void OperationInScreen()
    {
        if (OperationsListLevel1 != null && OperationsListLevel1.Count > 0) // Si la lista de operaciones existe y su contenido es mayor a 0...
        {
            int randomOperationOfList = Random.Range(0, OperationsListLevel1.Count); // Selecciona una operación al azar de la lista para posteriormente utilizarla en el panel
            operationText.text = OperationsListLevel1[randomOperationOfList].Operation; // Da la operación seleccionada de la lista en el panel
                                                                                        // operationText(que es parte del TMPro.TextMeshProUGUI) es para que el texto aparezca fisicamente en el canvas, mientras que el .text da la orden de escribir en el objeto la informacion de la lista
            correctAnswer = OperationsListLevel1[randomOperationOfList].Answer; // Guarda la respuesta de la operación seleccionada de la lista, esta no es visible en el panel (el jugador la debe encontrar)
        }
    }

    void ReadPlayerInput()
    {
        foreach (char c in Input.inputString) // Por cada cáracter presionado...
        {
            if (char.IsDigit(c) || c == '-') // Si el valor dado es un digito (0 a 9) o el valor es negativo (lleva el signo -) OJO: HASTA QUE NO SE PRESIONE LA TECLA PARA CONFIRMAR LA RESPUESTA, PUEDES ESCRIBIR CUANTOS CÁRACTERES QUIERAS
            {
                currentInput += c; // Se van agregando los caracteres uno seguido del otro
                playerInputText.text = currentInput; // Se muestra la respuesta del player en el panel
            }
        }

        if (ConfirmAnswer != null && ConfirmAnswer.action.triggered) // Si el jugador indica que quiere confirmar la respuesta (presionando Z) ESTO SE INDICA ATRAVÉS DEL .action.triggered, detectando el golpe físico en el teclado
        {
            CheckAnswer();
        }
    }

    void CheckAnswer()
    {
        if (currentInput == "" || currentInput == "-") return; // Si el espacio está vacío o solo tiene -, entonces no pasa nada

        int answerPlayer = int.Parse(currentInput); // Se convierte la respuesta del currentInput a un numero entero

        if (answerPlayer == correctAnswer) // Si la respuesta dada por el jugador es la respuesta correcta...
        {
            feedbackText.text = "<color=green>CORRECTO!!!</color>";
            EnemyTakingDamage(5f);
            Debug.Log("Enemigo atacado. Vida restante: " + health);
            if (timerCoroutine != null) StopCoroutine(timerCoroutine); // Se pausa el tiempo del contador
            Invoke("WaitToNextCombo", 1.5f);
        }
        else 
        {
            feedbackText.text = "<color=red>INCORRECTO!!!</color>";
            PlayerTakingDamage();
            if (timerCoroutine != null) StopCoroutine(timerCoroutine); // Se pausa el tiempo del contador
            Invoke("ProcessError", 1.5f);
        }

        //Finalmente, se borran los datos dados en el currentInput y playerInputText.text
        currentInput = "";
        playerInputText.text = "";
        //EndBattle(); // Y se dirigue al proceso de finalización de combate
    }

    void WaitToNextCombo()
    {
        //Se borran los datos dados en el currentInput y playerInputText.text
        currentInput = "";
        playerInputText.text = "";
        feedbackText.text = "";
        OperationInScreen();
        if (timerCoroutine != null) StopCoroutine(timerCoroutine); // Si había anteriormente un contador corriendo, entonces será apagado
        timerCoroutine = StartCoroutine(StartTimerRoutine()); // Se inicia el contador del tiempo para responder las operaciones
    }

    void ProcessError()
    {
        //// Primero se apaga la interfaz de batalla y se vuelve a la vista original de la cámara
        //IsInMathBattle = false;
        //battleIniciated = false;
        //canvasMathCombos.SetActive(false);
        //virtualCamera.Lens.OrthographicSize = originalZoomCamera;

        ////Se borran los datos dados en el currentInput y playerInputText.text
        //currentInput = "";
        //playerInputText.text = "";

        // Knockback (Movimiento sutil hacia atras debido al golpe por fallar la respuesta o quedarse sin tiempo)
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>(); // Se busca el componente fisico del player
            if (playerRb != null) 
            {
                Vector2 pushDirection = (playerObj.transform.position - transform.position).normalized; // Se indica que la dirección del empuje se hace a partir de la resta de la posicion del player con la del enemigo
                float pushForce = 20f;
                playerRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse); // Se aplica la fuerza como un impulso físico
                Debug.Log("<color=red>¡Jugador empujado hacia atrás!</color>");
            }
        }
        EndBattle();
    }

    void EndBattle()
    {
        // Primero se apaga la interfaz de batalla y se vuelve a la vista original de la cámara
        IsInMathBattle = false; // Apaga la indicación que z será usada para confirmacion de respuesta
        battleIniciated = false;
        canvasMathCombos.SetActive(false);
        virtualCamera.Lens.OrthographicSize = originalZoomCamera; // Vuelve al zoom original de la pantalla

        //Se borran los datos dados en el currentInput y playerInputText.text
        currentInput = "";
        playerInputText.text = "";

        //NUEVO
        // -----------------------------------
        EnablePlayerMovement();
        // -----------------------------------

        if (health > 0) // Si el enemigo sigue vivo///
        {
            this.enabled = false; // Se desactiva el script de forma temporal, haciendo que en casos de equivocarse o quedarse sin tiempo, el player se salga del combate y pierda vida en el proceso
            Invoke("ReturnDinamicEnemy", 2f); // Indicará en el sistema que luego del tiempo asignado, se dirija al void de ReturnDinamicEnemy
        }
       
    }

    //NUEVO
    // -----------------------------------
    private void EnablePlayerMovement() // // VOLVER A ACTIVAR MOVIMIENTO Y DEMÁS MECANICAS DEL PLAYER
    {
        GameObject playerObj = GameObject.FindWithTag("Player"); // Se llama a los componentes del player
        if (playerObj != null)
        {
            PlayerInput pInput = playerObj.GetComponent<PlayerInput>(); // Tiene a la mano las actions del action map de player
            if (pInput != null)
            {
                pInput.actions.FindActionMap("Player")?.Enable(); // Reactiva el mapa de movimiento del personaje cuando termina el combate
            }
        }
    }
    // -----------------------------------

    void ReturnDinamicEnemy()
    {
        this.enabled = true; // Vuelve a activar el script del enemigo
    }

    public void EnemyTakingDamage(float quantify)
    {
        quantify = 5f;
        health -= quantify;
        Debug.Log("Enemigo herido. Vida restante: " + health);

        if (health <= 0)
        {
            EndBattle();
            Destroy(gameObject);
        }
    }

    private void PlayerTakingDamage()
    {
        GameObject playerObj = GameObject.FindWithTag("Player"); // Busca la información de salud del jugador

        if (playerObj != null)
        {
            Player scriptPlayer = playerObj.GetComponent<Player>();
            if (scriptPlayer != null && scriptPlayer.isAlive)
            {
                scriptPlayer.TakeDamage(damageToPlayer);
                Debug.Log("<color=red>Daño recibido del Player: </color>" + damageToPlayer);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}