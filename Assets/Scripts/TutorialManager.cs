using TMPro;
using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

public class TutorialManager : MonoBehaviour
{
    [Header("User interface")]
    public TextMeshProUGUI Instructions;

    [Header("Limits beetween instructions")]
    public GameObject LimitPhase1;
    public GameObject LimitPhase2;
    public GameObject LimitPhase3;

    [Header("Book of spells for go to level 1")]
    public GameObject TransportToLevel1;

    [Header("Objects part of the tutorial")]
    public TrainingDummy Mannequin; // Se llama asi en vez de gameobject para poder controlar los disparos desde este script

    // Control of tutorial phases
    public enum TutorialPhase { Move, JumpAndCrouch, BasicCombat, Finished }
    [HideInInspector] public TutorialPhase CurrentPhase = TutorialPhase.Move; // Inicialmente, se aprenderá a moverse en el juego

    // VERIFICATION OF ACTIONS
    // Inicialmente estas verificaciones son falsas

    // FASE 1: Move
    private bool playerMoved = false;
    private bool playerRan = false;

    // FASE 2: JumpAndCrounch
    private bool playerJumped = false;
    private bool playerDidDoubleJump = false;
    //private int jumpCount = 0; //ESTO YA NO APLICA
    private bool playerCrouched = false;

    // FASE 2: BasicCombat
    private bool DoFastAttack = false; // Tecla Z
    private bool DoHeavyAttack = false; // Tecla X
    private bool DoCounterAttack = false; // Tecla C
    private bool ActiveShield = false; // Tecla V

    // ---------------------------------------------------------------------

    void Start()
    {
        // Se activan los límites del tutorial
        if (LimitPhase1 != null && LimitPhase2 != null && LimitPhase3 != null ) 
        {
            LimitPhase1.SetActive(true);
            LimitPhase2.SetActive(true);
            LimitPhase3.SetActive(true);
        }

        if (TransportToLevel1 != null)
        {
            TransportToLevel1.SetActive(false); // El libro de hechizos para teletransportarse al nivel 1 no aparecerá por ahora
        }

        if (Mannequin != null)
        {
            Mannequin.StopShooting(); // Inicialmente el maniquí no va a estar disparando
        }

        ShowInstructionsOfPhase1();
    }

    void Update()
    {
        switch (CurrentPhase) 
        {
            case TutorialPhase.Move:
                CheckMove();
                break;
            case TutorialPhase.JumpAndCrouch:
                CheckJumpAndCrouch();
                break;
            case TutorialPhase.BasicCombat:
                CheckBasicCombat();
                break;
        }
    }

    // ---------------------------------------------------------------------

    // LOGIC OF PHASES

    // FASE 1: Move
    void ShowInstructionsOfPhase1()
    {
        Instructions.text = "Usa [A/D] o flechas para moverte";
    }

    void CheckMove()
    {
        if (Keyboard.current == null) return; // Keyboard.current busca la tecla física del PC
        // Esta línea evita que el juego marque error

        // MOVIMIENTO
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            playerMoved = true;
            Instructions.text = "Bien, ahora mantén [Shift] para correr";
        }

        // CORRER
        if (Keyboard.current.leftShiftKey.isPressed) // .isPressed es true mientras que se unda la tecla sin soltarla
        {
            playerRan = true;
        }

        // COMPROBACIÓN PARA PASAR A LA FASE 2 DEL TUTORIAL
        if (playerMoved && playerRan) // Si el player ya se ha movido y a corrido (presionado las teclas A, D o las flechas de izq y der Y también a presionado leftShift)
        {
            if (LimitPhase1 != null) LimitPhase1.SetActive(false); // Se desactiva el primer limite del tutorial

            CurrentPhase = TutorialPhase.JumpAndCrouch; // Cambia de fase
            ShowInstructionsOfPhase2(); // Mostrando las indicaciones de la fase 2
        }
    }

    // FASE 2: JumpAndCrounch
    void ShowInstructionsOfPhase2()
    {
        Instructions.text = "Presiona [Espacio], [flecha hacia arriba] o [W] para saltar";
    }

    void CheckJumpAndCrouch()
    {
        if (Keyboard.current == null) return; // Keyboard.current busca la tecla física del PC
        // Esta línea evita que el juego marque error

        // SALTOS
        // .wasPressedThisFrame es true solo en el fotograma exacto en que hundes la tecla
        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!playerJumped) // Si el jugador no ha saltado por primera vez...
            {
                playerJumped = true;
                Instructions.text = "Excelente, pulsa el botón de salto en el aire para hacer un salto doble";
            }
            else if(!playerDidDoubleJump) // Si ya salto una vez entonces revisará si aun no ha hecho un salto doble
            {
                playerDidDoubleJump = true;
                Instructions.text = "Ahora, mantén presionado [S] o [flecha hacia abajo] para agacharte";
            }

            //LOGICA ORIGINAL
            //
            //jumpCount++; // Si se detecta que el player presionó alguno de los botones para saltar se le sumará 1 al contador

            //if (jumpCount == 1)  // Si el contador es 1...
            //{
            //    playerJumped = true; // Se detecta que el jugador saltó
            //    jumpCount = 0; // el contador vuelve a ser 0
            //    Instructions.text = "Excelente, pulsa el botón de salto en el aire para hacer un salto doble";
            //}

            //jumpCount++; // Si se detecta que el player presionó alguno de los botones para saltar se le sumará 1 al contador

            //if (jumpCount >= 2) // Si el contador es igual o mayor a 2...
            //{
            //    playerDidDoubleJump = true; // Se detecta que el jugador ya ha hecho doble salto
            //    Instructions.text = "Ahora, mantén presionado [S] o [flecha hacia abajo] para agacharte";
            //}
        }

        // AGACHARSE
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) // .isPressed es true mientras que se unda la tecla sin soltarla
        {
            playerCrouched = true;
        }

        // COMPROBACIÓN PARA PASAR A LA FASE 3 DEL TUTORIAL
        if (playerJumped && playerDidDoubleJump && playerCrouched) // Si el player ya se ha hecho salto, doble salto y agacharse (presionado la tecla W, la barra espaciadora o la flecha de arriba 1 y 2 veces además der Y también a presionado leftShift)
        {
            if (LimitPhase2 != null) LimitPhase2.SetActive(false); // Se desactiva el segundo limite del tutorial

            CurrentPhase = TutorialPhase.BasicCombat; // Cambia de fase
            ShowInstructionsOfPhase3(); // Mostrando las indicaciones de la fase 3
        }
    }

    // FASE 3: Basic combat
    void ShowInstructionsOfPhase3()
    {
        Instructions.text = "Prueba su fuerza en combate. " +
            "Ataca al muñeco de prueba con ataques rápidos [Z], pesados [X] y de repulsión [C]";
    }

    void CheckBasicCombat()
    {
        if (Keyboard.current == null) return; // Keyboard.current busca la tecla física del PC
        // Esta línea evita que el juego marque error

        // ATAQUES
        // .wasPressedThisFrame es true solo en el fotograma exacto en que hundes la tecla
        if (Keyboard.current.zKey.wasPressedThisFrame) DoFastAttack = true; // ATAQUE RÁPIDO con Z
        if (Keyboard.current.xKey.wasPressedThisFrame) DoHeavyAttack = true; // ATAQUE PESADO con X
        if (Keyboard.current.cKey.wasPressedThisFrame) DoCounterAttack = true; // ATAQUE DE REPULSIÓN (Contrataque) con C

        if (DoFastAttack && DoHeavyAttack && DoCounterAttack) // Si el player ya ha probado los controles  Z, X, C...
        {
            Instructions.text = "CUIDADO!! Bolas de fuego a la vista, protegete con el escudo[V]";

            if (Mannequin != null)
            {
                Mannequin.StartShooting(); // Empieza a disparar
            }
        }

        // ESCUDO
        // .wasPressedThisFrame es true solo en el fotograma exacto en que hundes la tecla
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            ActiveShield = true;

            if (Mannequin != null)
            {
                Mannequin.StopShooting(); // Al haber activado el escudo, los disparon cesarán ya que el entrenamiento terminó
            }
        }

        // COMPROBACIÓN PARA PASAR A LA FASE 3 DEL TUTORIAL
        if (DoFastAttack && DoHeavyAttack && DoCounterAttack && ActiveShield) // Si el player ya ha probado todos los controles de combate básico (presionado la teclas Z, X, C, V)
        {
            if (LimitPhase3 != null) LimitPhase3.SetActive(false); // Se desactiva el último limite del tutorial

            CurrentPhase = TutorialPhase.Finished; // Cambia de fase
            TransportToLevel1.SetActive(true); // El libro de hechizos para teletransportarse al nivel 1 aparecerá
            Instructions.text = "¡Excelente trabajo! Completaste el entrenamiento. Acércate al libro de hechizos para viajar al Nivel 1.";
        }
    }
}