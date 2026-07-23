using UnityEngine;

public class LimitsOfLevels : MonoBehaviour
{
    [Header("Limit of Levels settings")]
    [SerializeField] private Color limitColor = new Color(0f, 191f, 0f, 0.40f); 
    [SerializeField] private Vector2 limitSize = new Vector2(2.33f, 21.72f);

    void OnDrawGizmos()
    {
        Gizmos.color = limitColor;
        Gizmos.DrawCube(transform.position, new Vector3(limitSize.x, limitSize.y, 1f));
    }
}
