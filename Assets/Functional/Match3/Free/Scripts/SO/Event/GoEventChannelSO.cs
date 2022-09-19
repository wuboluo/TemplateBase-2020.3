using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameObjectEvent", menuName = "Yang/Event/GameObject Event")]
public class GoEventChannelSO : DescriptionBaseSO
{
    public event UnityAction<GameObject> OnEventRaised;

    public void RaiseEvent(GameObject go)
    {
        OnEventRaised?.Invoke(go);
    }
}