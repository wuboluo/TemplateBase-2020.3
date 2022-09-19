using UnityEngine;
using UnityEngine.Events;

public class AnimationEventInserter : MonoBehaviour
{
    public UnityEvent atACertainMoment;

    public void WhatHappenedAtACertainMoment()
    {
        atACertainMoment?.Invoke();
    }
}