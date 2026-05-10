using NUnit.Framework;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

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
    [SerializeField] public float visionRange = 5f;
    //[SerializeField] private float attackRange = 1.5f; // Distancia mínima para empezar a atacar
    //[SerializeField] private float timeBeetweenAttacks = 1.5f; // Tiempo de espera entre cada ataque

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
            Debug.Log("<color=yellow> Player detectado </color>");
            battleIniciated = true;
            MathBattle(playerHit.gameObject);
        }
    }

    void MathBattle(GameObject player)
    {
        IsInMathBattle = true; // Activa la indicación que z será usada para confirmacion de respuesta
        canvasMathCombos.SetActive(true); // Aparece el panel de batalla por combos matemáticos
        virtualCamera.Lens.OrthographicSize = zoomCamera; // Hace el zoom indicado hacia la escena de combate

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

        if (Input.GetKeyDown(KeyCode.Z)) // Si presionas la tecla Z...
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
        }
        else 
        {
            feedbackText.text = "<color=red>INCORRECTO!!!</color>";
        }

        //Finalmente, se borran los datos dados en el currentInput y playerInputText.text
        currentInput = "";
        playerInputText.text = "";
        EndBattle(); // Y se dirigue al proceso de finalización de combate
    }

    void EndBattle()
    {
        IsInMathBattle = false; // Apaga la indicación que z será usada para confirmacion de respuesta
        battleIniciated = false;
        canvasMathCombos.SetActive(false);
        virtualCamera.Lens.OrthographicSize = originalZoomCamera; // Vuelve al zoom original de la pantalla

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}