using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;

public class PuzzlesLawOfSigns : MonoBehaviour
{
    [Header("Panel of math puzzles")]
    public GameObject canvasMathPuzzles; // Se selecciona el panel en el que se mostrarán las operaciones a resolver
    private bool transformationSpell = false; // Inicialmente, el jugador no invocará un hechizo de trasformación

    public TMPro.TextMeshProUGUI playerInputText; // Referencia al objeto de texto de la UI (La respuesta del player)
    public TMPro.TextMeshProUGUI operationText; // Referencia al objeto de texto de la UI (La operación matemática que aparecerá en el panel de acertijos/Puzzles)
                                                // TMPro.TextMeshProUGUI es el tipo de dato
                                                // TMPro es el espacio de nombres (namespace) de TextMesh Pro
                                                // TextMeshProUGUI es la clase específica para texto de UI dentro de un Canvas
    public TMPro.TextMeshProUGUI feedbackText; // Referencia al objeto de texto de la UI (Indica si la respuesta del player fue correcta o no)
    public bool growPlayer; // Indicador de que tipo de transformación es el acertijo

    private int correctAnswer; // Información sobre la respuesta correcta de las operaciones
    private string currentInput = ""; // Respuesta dada por el jugador
    private Rigidbody2D playerRb;
    private GameObject gameObjectPlayer;

    [Header("Puzzle Area settings")]
    [SerializeField] public float detectionRange = 2.5f;
    [SerializeField] private InputActionReference StartPuzzle;
    [SerializeField] private InputActionReference ConfirmAnswer;
    [SerializeField] private InputActionReference ExitPuzzle;

    // LIST OF PUZZLES
   // MathPuzzles puzzleSelected; // Se crea una variable de tipo de la clase MathPuzzles para guardar el ejercicio seleccionado

    [Header("UI Interact")]
    public GameObject UIPromptInteraction;

    [Header("Sizes of transformation")]
    public float gigantSize = 2.5f; // Se vuelve Gigante
    public float smallSize = 0.4f; // Se vuelve Pequeño

    private bool puzzleSolved = false;
    //private bool PuzzleActivate = false; // Indica si el acertijo está activado o no, facilitando la opción de salir de este sin quedar en un bucle infinito

    void Start()
    {
        canvasMathPuzzles.SetActive(false); // Inicialmente el panel estará oculto
        UIPromptInteraction.SetActive(false); // Inicialmente la indicación de interacción estará oculto
    }

    void Update()
    {
        if (!canvasMathPuzzles.activeSelf) // Si el panel está apagado...
        {
            ScanPerimeter(); // Se revisará el área
        }
        else
        {
            ReadPlayerInput(); // Si no, entonces el jugador debe hacer el hechizo de transformación, entonces prosigue en detectar las teclas dadas por el player

            if (ExitPuzzle != null && ExitPuzzle.action.triggered) // Si el jugador indica que quiere salir del acertijo (presionando X) ESTO SE INDICA ATRAVÉS DEL .action.triggered, detectando el golpe físico en el teclado)
            {
                Debug.Log("Jugador prefirió salir del acertijo.");
                ClosePuzzlePanel();
            }
        }
    }

    public void ScanPerimeter()
    {
        Collider2D detectionPlayer = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));  // Funciona como radar para detectar si el player está cerca

        if (detectionPlayer != null)  // Si en la zona se detectó algo...
        {
            UIPromptInteraction.SetActive(true);

            if (StartPuzzle != null && StartPuzzle.action.triggered) // Si el jugador indica que quiere iniciar el acertijo (presionando E) ESTO SE INDICA ATRAVÉS DEL .action.triggered, detectando el golpe físico en el teclado
            {
                UIPromptInteraction.SetActive(false);
                Debug.Log("<color=yellow> Player detectado, ACERTIJO ACTIVADO</color>");
                currentInput = "";
                playerInputText.text = "";
                LoadOperation(); // Se dirigue a la función de cargar operación
                canvasMathPuzzles.SetActive(true);
                feedbackText.text = "";

                gameObjectPlayer = detectionPlayer.gameObject; // Se guarda temporalmente al player, la referencia del objeto jugador detectado
                playerRb = detectionPlayer.GetComponent<Rigidbody2D>(); // Se selecciona el rigidbody del player
                
                if(playerRb != null)
                {
                    Time.timeScale = 0;
                    playerRb.linearVelocity = Vector2.zero; // Cuando sea detectado y activado un acertijo, la velocidad de movimiento del jugador va a cero para que no se mueva de ahí hasta que resuelva o salga del acertijo
                    //playerRb.bodyType = RigidbodyType2D.Static; // Por medio de ls propiedad bodytype que define el comportamiento de un objeto con fisica 2D hace que el rigidbody 2D del player se mantenga estatico
                }
            }
            //else
            //{
            //    UIPromptInteraction.SetActive(false);
            //}
        }
        else
        {
            UIPromptInteraction.SetActive(false);
        }
    }

    public void LoadOperation()
    {
        ListOfPuzzles Operations = Object.FindFirstObjectByType<ListOfPuzzles>(); // En esta variable se guardará el primer objeto detectado con el tipo ListOfPuzzles
        MathPuzzles puzzleSelected; // Se crea una variable de tipo de la clase MathPuzzles para guardar el ejercicio seleccionado
        
        if (Operations != null)
        {
            if (growPlayer == true) // Si en el inspector está indicado que el acertijo hará que el player crezca...
            {
                puzzleSelected = Operations.UsePositiveOneRandom(); // Da la operación de resultado positivo seleccionada de la lista en el panel

            }
            else // Si no, ...
            {
                puzzleSelected = Operations.UseNegativeOneRandom(); // Da la operación de resultado negativo seleccionada de la lista en el panel
            }

            if (puzzleSelected != null)
            {
                operationText.text = puzzleSelected.Operation;
                correctAnswer = puzzleSelected.Answer;
                Debug.Log(puzzleSelected.Answer);
            }
        }
    }

    void ReadPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) // Si el player decide borrar su respuesta (presiona el botón backspace)...
        {
            if (currentInput.Length > 0) // y Si el player ya había escrito algo...
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1); // Se borrará el último string que ha escrito (el 0 indica al primer elemento escrito)\
                playerInputText.text = currentInput; // Se muestra la respuesta del player en el panel
            }
            return; // Cuando ocurre esto, se sale de la funcion para que no detecte más teclas
        }

        foreach (char c in Input.inputString) // Por cada cáracter presionado...
        {
            //if (char.IsDigit(c) || c == '-') // Si el valor dado es un digito (0 a 9) o el valor es negativo (lleva el signo -) OJO: HASTA QUE NO SE PRESIONE LA TECLA PARA CONFIRMAR LA RESPUESTA, PUEDES ESCRIBIR CUANTOS CÁRACTERES QUIERAS
            //{
            //    currentInput += c; // Se van agregando los caracteres uno seguido del otro
            //    playerInputText.text = currentInput; // Se muestra la respuesta del player en el panel
            //}
            if (char.IsDigit(c)) // Si el valor dado es un digito (0 a 9)...
            {
                currentInput += c; // Se van agregando los caracteres uno seguido del otro
                playerInputText.text = currentInput; // Se muestra la respuesta del player en el panel
            } 
            else if (c == '-' && currentInput.Length == 0) // Si no, el valor es negativo (lleva el signo -) y el - es el primer caracter escrito [currentInput.Length == 0 hace que el jugador no pueda escribir el - en otras posiciones del texto]
                                                           // OJO: HASTA QUE NO SE PRESIONE LA TECLA PARA CONFIRMAR LA RESPUESTA, PUEDES ESCRIBIR CUANTOS CÁRACTERES QUIERAS
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
       // if (currentInput == "" || currentInput == "-") return; // Si el espacio está vacío o solo tiene -, entonces no pasa nada

        int answerPlayer = int.Parse(currentInput.Trim()); // Se convierte la respuesta del currentInput a un numero entero (.Trim() elimina espacios y caracteres invisibles antes de convertir a enter)

        //Debug.Log($"[PRUEBA] Tu respuesta: '{answerPlayer}' | Respuesta Correcta Guardada: '{puzzleSelected.Answer}'");

        if (answerPlayer == correctAnswer) // Si la respuesta dada por el jugador es la respuesta correcta...
        {
            feedbackText.text = "<color=green>CORRECTO!!!</color>";
            Debug.Log("Acertijo Resuelto");
            transformationSpell = true;
            puzzleSolved = true;
            ApplyChangeSize();
            //Invoke("ClosePuzzlePanel", 0.5f);
            ClosePuzzlePanel();
            Destroy(gameObject);
            Time.timeScale = 1;
        }
        else
        {
            feedbackText.text = "<color=red>INCORRECTO!!!</color>";
            Debug.Log("Acertijo Fallido");
            transformationSpell = false;
            //Invoke("ClosePuzzlePanel", 0.5f);
            Time.timeScale = 1;
            ClosePuzzlePanel();
        }

        //Finalmente, se borran los datos dados en el currentInput y playerInputText.text
        //currentInput = "";
        //playerInputText.text = "";
    }

    void ApplyChangeSize()
    {
        GameObject playerObj = GameObject.FindWithTag("Player"); // Se indica que el player cambiará de tamaño

        if (playerObj != null)
        {
            Player playerScript = playerObj.GetComponent<Player>();  // Se trae información del player
            if (playerScript != null)
            {
                playerScript.isTransformed = true; // Indicando que el player se esta transformando, bloqueandole la opcion de agacharse
            }

            if (growPlayer == true)
            {
                playerObj.transform.localScale = new Vector3(2.5f, 2.5f, 1f); // Se vuelve Gigante
                Debug.Log("<color=green>Player Gigante</color>");
                //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), true); // Hará al player invensible
            }
            else
            {
                playerObj.transform.localScale = new Vector3(0.4f, 0.4f, 1f); // Se vuelve Pequeño
                Debug.Log("<color=blue>Player Pequeño</color>");
                //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), false);
            }
        }
    }

    void ClosePuzzlePanel()
    {
        canvasMathPuzzles.SetActive(false);
        feedbackText.text = "";

        if (playerRb != null)
        {
            Time.timeScale = 1;
            playerRb.bodyType = RigidbodyType2D.Dynamic; // El player vuelve a sus físicas para poder moverse
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation; // Mantenemos tus restricciones
            playerRb = null; // Finalmente, se limpia la variable para el próximo uso
            //playerRb.linearVelocity = new Vector2(1, playerRb.linearVelocity.y);
        }

        //Finalmente, se borran los datos dados en el currentInput y playerInputText.text
        currentInput = "";
        playerInputText.text = "";

        //if (transformationSpell == true)
        //{
        //    ApplyChangeSize();
        //    transformationSpell = false;
        //}
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color (131f / 255f, 137f / 255f, 220f / 255f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}