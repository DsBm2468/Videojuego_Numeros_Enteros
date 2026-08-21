using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MathFastExercisesForCombos
{
    public string Operation;
    public int Answer;
}
public class ListOfMathExercisesForCombos : MonoBehaviour
{
    [Header("Excercises")]
    public List<MathCombos> excercises;

    public MathCombos UseOneRandom()
    {
        int index = Random.Range(0, excercises.Count);
        return excercises[index];
    }
}