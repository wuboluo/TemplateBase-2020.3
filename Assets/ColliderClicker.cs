using UnityEngine;
using UnityEngine.Events;

public class ColliderClicker : MonoBehaviour
{
    public UnityEvent onClick;
    private bool _clicked;

    private void OnMouseDown()
    {
        if (_clicked || StateSwitcher.State == GameState.End) return;

        onClick?.Invoke();
        _clicked = true;
    }
}