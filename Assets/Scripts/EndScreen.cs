using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EndScreen : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI _finalScoreText;
    [SerializeField] ScoreKeeper _scoreKeeper;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void ShowFinalScore()
    {
        Debug.Assert(_scoreKeeper is not null, "_scoreKeeper should  be set");
        Debug.Assert(_finalScoreText is not null, "_finalScoreText should be set");

        var score = _scoreKeeper.CalculateScore();
        _finalScoreText.text = $"Congratulations!\nYou got a score of {score}%";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
