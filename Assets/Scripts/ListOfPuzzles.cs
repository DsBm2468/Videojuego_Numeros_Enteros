using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MathPuzzles
{
    public string Operation;
    public int Answer;
}

public class ListOfPuzzles : MonoBehaviour
{
    [Header("Positive Puzzles (Makes the player gigant)")]
    public List<MathPuzzles> positivePuzzles;

    [Header("Negative Puzzles (Makes the player small)")]
    public List<MathPuzzles> negativePuzzles;

    public MathPuzzles UsePositiveOneRandom()
    {
        if (positivePuzzles == null || positivePuzzles.Count == 0) // Si  la lista está vacia o no existe
        {
            Debug.LogError("Atención: La lista de acertijos positivos está vacía.");
            return null;
        }
        else
        {
            int index = Random.Range(0, positivePuzzles.Count);
            return positivePuzzles[index];
        }
    }

    public MathPuzzles UseNegativeOneRandom()
    {
        if (negativePuzzles == null || negativePuzzles.Count == 0) // Si  la lista está vacia o no existe
        {
            Debug.LogError("Atención: La lista de acertijos positivos está vacía.");
            return null;
        }
        else
        {
            int index = Random.Range(0, negativePuzzles.Count);
            return negativePuzzles[index];
        }
    }
}
