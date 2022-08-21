using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class Quiz : MonoBehaviour
{

    [Header("Questions")]
    [SerializeField] protected int numberOfAnswers = 4;
    [SerializeField] protected List<QuestionScriptableObject> questions = new();
    protected QuestionScriptableObject _currentQuestion;

    [SerializeField] protected TextMeshProUGUI questionText;
    [Header("Answers")]
    [SerializeField] protected GameObject answerButtonGroup;
    [SerializeField] protected GameObject answerButtonPrefab;
    [SerializeField] Sprite DefaultAnswerSprite;
    [SerializeField] Sprite CorrectAnswerSprite;
    private GameObject[] AnswerButtons;

    [Header("Timer")]
    [SerializeField] BetterTimerController _answeringTimer;
    [SerializeField] BetterTimerController _reviewingTimer;



    [Header("Progress")]
    [SerializeField] Slider progressSlider;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;

    ScoreKeeper _scoreKeeper;

    public QuizState CurrentState { get; private set; }

    private bool _stateChanged;


    public enum QuizState
    {
        Starting,
        NeedNewQuestion,
        AskingQuestion,
        ReviewingQuestion,
        Complete
    }

    void Awake()
    {
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //_timer = FindObjectOfType<TimerController>();
        progressSlider.maxValue = questions.Count;
        progressSlider.value = 0;

        _answeringTimer.TimeIsUpEvent.AddListener(AnsweringTimeIsUp);
        _reviewingTimer.TimeIsUpEvent.AddListener(ReviewingTimeIsUp);

        SetState(QuizState.NeedNewQuestion);
    }

    void Update()
    {
        if (_stateChanged)
        {
            _stateChanged = false;

            switch (CurrentState)
            {
                case QuizState.NeedNewQuestion:
                    Debug.Assert(questions.Any());

                    //StopAndHideAllTimers();

                    _currentSelectAnswerIndex = null;
                    _currentQuestion = GetRandomQuestion();
                    questions.Remove(_currentQuestion);
                    DisplayQuestion();
                    SetAnswerButtonState(true);
                    _scoreKeeper.IncrementQuestionsSeen();
                    progressSlider.value++;

                    SetState(QuizState.AskingQuestion); // After selecting go to next state

                    break;
                case QuizState.AskingQuestion:
                    StopAndHideTimer(_reviewingTimer);
                    StartAndShowTimer(_answeringTimer);

                    // This state can be stopped in two ways: by the _answeringTimer TimesUp event or when an answer is selected

                    break;
                case QuizState.ReviewingQuestion:
                    StopAndHideTimer(_answeringTimer);
                    StartAndShowTimer(_reviewingTimer);

                    // This state can be stopped by the _reviewingTimer TimesUp event
                    break;
                case QuizState.Complete:
                    StopAndHideAllTimers();
                    RemoveButtons();
                    questionText.text = "You are done! No more question";

                    // End state
                    break;

            }
        }

    }

    public void AnsweringTimeIsUp(bool displayed)
    {
        Debug.Log("AnsweringTimeIsUp");
        Debug.Assert(displayed);
        SetState(QuizState.ReviewingQuestion);
        DisplayWrongAnswer("Too late");
        SetAnswerButtonState(false);
    }

    public void ReviewingTimeIsUp(bool displayed)
    {
        Debug.Log("ReviewingTimeIsUp");

        Debug.Assert(displayed);
        var anyQuestionsLeft = questions.Any();

        if (anyQuestionsLeft)
        {

            SetState(QuizState.NeedNewQuestion);
        }
        else
        {
            SetState(QuizState.Complete);
        }
    }

    void SetState(QuizState state)
    {
        Debug.Assert(state != CurrentState);
        if (state == CurrentState)
        {
            // nothing to do when state is the same
            // I dont think it is possible to get to this state atm
        }
        else
        {
            CurrentState = state;
            _stateChanged = true;
        }
    }



    void StopAndHideTimer(BetterTimerController timer)
    {
        timer.StopTimer();
        //  timer.HideTimer();
    }

    void StartAndShowTimer(BetterTimerController timer)
    {
        timer.StartTimer();
        timer.ShowTimer();
    }

    void StopAndHideAllTimers()
    {
        StopAndHideTimer(_answeringTimer);
        StopAndHideTimer(_reviewingTimer);
    }

    private int? _currentSelectAnswerIndex = null;


    void OnAnswerSelected()
    {
        var button = EventSystem.current.currentSelectedGameObject;
        int selectedAnswerIndex = Array.IndexOf(AnswerButtons, button);
        _currentSelectAnswerIndex = selectedAnswerIndex;
        Debug.Log($"Selected answer index: {selectedAnswerIndex}");

        var correctAnswerIndex = _currentQuestion.GetCorrectAnswerIndex();
        if (correctAnswerIndex == selectedAnswerIndex)
        {
            _scoreKeeper.IncrementCorrectAnswers();
        }
        DisplayAnswer(selectedAnswerIndex);

        SetAnswerButtonState(false);

        scoreText.text = $"Score: {_scoreKeeper.CalculateScore()} %";

        SetState(QuizState.ReviewingQuestion);
    }

    private void DisplayAnswer(int selectedAnswerIndex)
    {
        var correctAnswerIndex = _currentQuestion.GetCorrectAnswerIndex();
        var answeredCorrectly = selectedAnswerIndex == correctAnswerIndex;
        if (answeredCorrectly)
        {
            questionText.text = "Correct!!!";
            HighlightCorrectAnswer(correctAnswerIndex);
        }
        else
        {
            DisplayWrongAnswer("WRONG");
        }
    }

    private void DisplayWrongAnswer(string message)
    {
        var correctAnswerIndex = _currentQuestion.GetCorrectAnswerIndex();

        var correctAnswer = _currentQuestion.GetAnswer(correctAnswerIndex);
        questionText.text = $"{message};\nThe correct answer was {correctAnswer}";
        HighlightCorrectAnswer(correctAnswerIndex);
    }

    void HighlightCorrectAnswer(int index)
    {
        Image buttonImage = AnswerButtons[index].GetComponent<Image>();
        buttonImage.sprite = CorrectAnswerSprite;
    }
    void SetAnswerButtonState(bool interactable)
    {
        if (AnswerButtons is null)
            return;  // Buttons not generated yet

        foreach (var answerButton in AnswerButtons)
        {
            var button = answerButton.GetComponent<Button>();
            button.interactable = interactable;
        }
    }

    void RemoveButtons()
    {
        if (AnswerButtons is null)
            return;  // Buttons already removed

        foreach (var answerButton in AnswerButtons)
        {
            GameObject.Destroy(answerButton.gameObject);
        }

        AnswerButtons = null;
    }

    int GetButtonIndex(GameObject button) => Array.IndexOf(AnswerButtons, button);

    void GetNextQuestion()
    {

    }

    QuestionScriptableObject GetRandomQuestion()
    {
        int index = UnityEngine.Random.Range(0, questions.Count);
        //_usedQuestionListIndexes.Add(index);
        return questions[index];


    }

    void DisplayQuestion()
    {
        questionText.text = _currentQuestion.GetQuestion();

        if (AnswerButtons is null || AnswerButtons.Length == 0)
        {
            AnswerButtons = new GameObject[4];
            for (int i = 0; i < numberOfAnswers; i++)
            {
                // Init prefab and add to group and internal list
                var answerButton = Instantiate(answerButtonPrefab);
                AnswerButtons[i] = answerButton;
                answerButton.name = $"AnswerButton_{i}";
                answerButton.transform.SetParent(answerButtonGroup.transform);

                SetupAnswerButton(answerButton);

                // Add event
                var button = answerButton.GetComponent<Button>();
                button.onClick.AddListener(OnAnswerSelected);
            }
        }
        else
        {
            for (int i = 0; i < numberOfAnswers; i++)
            {
                // Reset answer button
                SetupAnswerButton(AnswerButtons[i]);
            }

        }

    }

    void SetupAnswerButton(GameObject answerButton)
    {
        var index = GetButtonIndex(answerButton);
        TextMeshProUGUI buttonText = answerButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = _currentQuestion.GetAnswer(index);

        Image buttonImage = answerButton.GetComponentInChildren<Image>();
        buttonImage.sprite = DefaultAnswerSprite;

        var button = answerButton.GetComponent<Button>();

    }


}
