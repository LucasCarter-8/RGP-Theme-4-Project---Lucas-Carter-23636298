using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownTimer
{
    private float time;
    private float initialTime;
    public bool IsRunning { get; private set; }
    public CountDownTimer(float initialTime) { this.initialTime = initialTime; }
    // Start is called before the first frame update

    // Update is called once per frame
    public void Update(float deltaTime)
    {
        if (time > 0f && IsRunning)
        {
            time -= deltaTime;
            if (time < 0)
            {
                time = 0f;
            }
        }

        if (time == 0f && IsRunning)
        {
            Stop();
        }
    }

    public void Start()
    {
        time = initialTime;
        if (!IsRunning)
        {
            IsRunning = true;
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
        }
    }

    public void Reset()
    {
        time = initialTime;
        IsRunning = true;
    }

    public void Reset(float newTime)
    {
        initialTime = newTime;
        Reset();
    }
    public bool isFinished()
    {
        return time == 0f;
    }
}