using System.Collections.Generic;
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
        int index = Random.Range(0, positivePuzzles.Count);
        return positivePuzzles[index];
    }

    public MathPuzzles UseNegativeOneRandom()
    {
        int index = Random.Range(0, negativePuzzles.Count);
        return negativePuzzles[index];
    }
}
