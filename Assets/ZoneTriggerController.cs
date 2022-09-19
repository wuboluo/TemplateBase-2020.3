using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BoolEvent : UnityEvent<bool, GameObject>
{
}

public class ZoneTriggerController : MonoBehaviour
{
    [SerializeField] private BoolEvent enterZone;
    [SerializeField] private LayerMask layers;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0) enterZone.Invoke(true, other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0) enterZone.Invoke(false, other.gameObject);
    }
}