using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{

    public int CorrectAnswers { get; private set; } = 0;
    public int QuestionsSeen { get; private set; } = 0;

    public void IncrementCorrectAnswers()
    {
        CorrectAnswers++;
    }

    public void IncrementQuestionsSeen()
    {
        QuestionsSeen++;
    }

    public int CalculateScore()
    {
        var result = Mathf.RoundToInt(CorrectAnswers / (float)QuestionsSeen * 100);
        //Debug.Log($"Calculate score with: {CorrectAnswers} / {QuestionsSeen} * 100 = {result}");
        return result;
    }

}
