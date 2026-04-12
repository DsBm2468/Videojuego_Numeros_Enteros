using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Life Statistics")]
    private float health = 6f;

    [Header("Player Movement")]
    private float walkSpeed = 7f;
    private float runSpeed = 18f;

    [Header("Detection of Input Actions")]
    private Rigidbody2D rbPlayer; // Detecta las físicas implementadas en el input actions
    private Vector2 moveInput;
    private bool isRunning; // Detecta si shift está presionado

    void Awake()
    { 
        // Se encarga que al iniciar el juego, el jugador permanezca en el escenario

        rbPlayer = GetComponent<Rigidbody2D>(); // Busca al componente rigidbody en el player
        rbPlayer.freezeRotation = true;
    }

    // Control de botones del player
    // (Mecánicas de combate básico y movimiento/intercción con el ambiente)

    public void OnMove(InputAction.CallbackContext context)
    {
        // Según la tecla presionada y la presión de esta, se detecta el input que va a hacerse

        moveInput = context.ReadValue<Vector2>(); // Registra la dirección que el jugador asignó
                                                  // Estos valores en x que definen el movimiento son detecdos como: -1 left, 0 static y 1 right

        Debug.Log("Tecla presionada, valor: " + moveInput.x);
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

    //void OnJump(InputAction.CallbackContext context)
    //{
    //    // Según la tecla presionada y la presión de esta, se detecta el input que va a hacerse

    //    if (context.started)
    //    {
            
    //    }
    //}

    void Update()
    {
        // Actualización del movimiento del player

        // Según el nuevo botón presionado, si detecta que está corriendo,
        // utilizará la velocidad de correr, si no, la velocidad actual
        // seguirá siendo la de caminar.

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Se define la velocidad horizontal que tendrá el player. Multiplicando la dirección por la velocidad actual
        float horizontalVelocity = moveInput.x * currentSpeed;
        
        // Da el resultado al rigidbody para que el prsonaje se desplace
        rbPlayer.linearVelocity = new Vector2(horizontalVelocity, rbPlayer.linearVelocity.y); // Se usa este nombre (linearVelocity) porque actualmente es rl más compatible con la vversión de unity 6
    }
}
