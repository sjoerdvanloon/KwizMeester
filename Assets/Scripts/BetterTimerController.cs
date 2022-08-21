using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BetterTimerController : MonoBehaviour
{
    [SerializeField] float _amountOfTime = 30f; // 30 is a default
    [SerializeField] Image _timerImage;
    [SerializeField] public UnityEvent<bool> TimeIsUpEvent = new();

    public float CurrentTimerValue { get => _currentTimerValue; }
    public State CurrentState { get => _currentState; }

    public float _currentTimerValue;
    public State _currentState;
    public bool _displayed = false;


    public enum State
    {
        Initialized,
        Paused,
        Stopped,
        Running,
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentState = State.Initialized;
        _currentTimerValue = _amountOfTime;
        SetDisplayedState(false);

    }

    void SetDisplayedState(bool displayed)
    {
        _displayed = displayed;
        _timerImage.enabled = displayed;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsRunning())
        {
            _currentTimerValue -= Time.deltaTime;

            var isTimeUp = _currentTimerValue <= 0;

            if (isTimeUp)
            {
                _currentState = State.Stopped;
                _currentTimerValue = 0f;

                Debug.Log("Times up!");

                TimeIsUpEvent.Invoke(_timerImage.enabled);
            }
        }

        if (IsRunning() && _displayed)
        {
            _timerImage.fillAmount = GetFillFraction();
        }
    }

    public void HideTimer() => SetDisplayedState(false);


    public void ShowTimer() => SetDisplayedState(true);

    public void StartTimer()
    {
        if (CurrentState == State.Paused)
        {
            // Do not touch the current time
        }
        else
        {
            _currentTimerValue = _amountOfTime;
        }
        _currentState = State.Running;
    }

    public void PauseTimer()
    {
        _currentState = State.Paused;
    }

    public void StopTimer()
    {
        _currentState = State.Stopped;
        _currentTimerValue = 0f;
    }



    bool IsRunning() => CurrentState == State.Running;

    void UpdateTimer()
    {


    }

    private float GetFillFraction()
    {
        return _currentTimerValue / _amountOfTime;
    }

}
