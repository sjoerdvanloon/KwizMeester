using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    Quiz _quiz;
    EndScreen _endScreen;

    void Awake()
    {
        _quiz = FindObjectOfType<Quiz>();
        _endScreen = FindObjectOfType<EndScreen>();
    }
    // Start is called before the first frame update
    void Start()
    {


        _quiz.gameObject.SetActive(true);
        _endScreen.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (_quiz.CurrentState == Quiz.QuizState.Complete)
        {
            _quiz.gameObject.SetActive(false);
            _endScreen.gameObject.SetActive(true);
            _endScreen.ShowFinalScore();
        }
    }

    public void OnReplayLevel()
    {
        var buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(buildIndex);
    }
}
