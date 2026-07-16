using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class OpenTutorial : MonoBehaviour
{
    [Header("Scene settings")]
    [SerializeField] public float detectionRange = 2.5f;
    [SerializeField] private int numberBuildScene; // Se usa [SerializeField] para hacer facil modificaciones a los valores hasta tener los definitivos

    [SerializeField] private InputActionReference Interact;
    //public GameObject canvasFirstIndications;

    [Header("UI Interact")]
    public GameObject UIPromptInteraction;

    void Start()
    {
        //canvasFirstIndications.SetActive(false); // Inicialmente el panel estará oculto
        UIPromptInteraction.SetActive(false); // Inicialmente la indicación de interacción estará oculto
    }

    void Update()
    {
        //if (!canvasFirstIndications.activeSelf) // Si el panel está apagado...
        //{
            ScanPerimeter();
        //}
    }

    public void ScanPerimeter()
    {
        Collider2D detectionPlayer = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));  // Funciona como radar para detectar si el player está cerca

        if (detectionPlayer != null)  // Si en la zona se detectó algo...
        {
            UIPromptInteraction.SetActive(true);
            if (Interact != null && Interact.action.triggered) // Si el jugador indica que quiere iniciar el acertijo (presionando E) ESTO SE INDICA ATRAVÉS DEL .action.triggered, detectando el golpe físico enel teclado
            {
                SceneManager.LoadScene(numberBuildScene);
            }
        } else
        {
            UIPromptInteraction.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}