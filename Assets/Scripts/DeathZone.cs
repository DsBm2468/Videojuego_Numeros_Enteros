using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("El player se cayó XD");
            collision.GetComponent<Player>().TakeDamage(6f);
        }
    }
}
