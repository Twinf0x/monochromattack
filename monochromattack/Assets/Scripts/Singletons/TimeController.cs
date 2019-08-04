using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

    public float AimSlowMotionFactor = 0;

    private List<float> timeModifiers = new List<float>();

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void UpdateTimeScale()
    {
        float scale = 1;

        foreach(var modifier in timeModifiers)
        {
            scale *= modifier;
        }

        Time.timeScale = scale;
    }

    public void StartAimSlowMotion()
    {
        timeModifiers.Add(AimSlowMotionFactor);
        UpdateTimeScale();
    }

    public void EndAimSlowMotion()
    {
        timeModifiers.Remove(AimSlowMotionFactor);
        UpdateTimeScale();
    }

    public void SetGameOver()
    {
        timeModifiers.Add(0);
        UpdateTimeScale();
    }

    public void RemoveGameOver()
    {
        timeModifiers.Remove(0);
        UpdateTimeScale();
    }
}
