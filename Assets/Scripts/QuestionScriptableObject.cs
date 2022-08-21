using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(menuName = "Quiz Question", fileName = "New Question")]
public class QuestionScriptableObject : ScriptableObject
{
    [SerializeField]
    [TextArea(2, 6)]
    protected string Question = "Enter new question text here";

    [SerializeField]
    protected string[] Answers = new string[4] { "Aswer 1", "Answer 2", "Answer 3", "Answer 4" };

    [SerializeField]
    protected int CorrectAnswerIndex = 0;
    
    public string GetQuestion()
    {
        return Question;
    }

    public string[] GetAnswers() => Answers.ToArray();
    public string GetAnswer(int index) => Answers[index];
    public int GetCorrectAnswerIndex() => CorrectAnswerIndex;

}
