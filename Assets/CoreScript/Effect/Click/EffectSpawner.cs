using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPoolingSO effectPool;
    private Camera _mainCam;

    private void Start()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) Spawn();
    }

    private void Spawn()
    {
        var ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit)) return;
        if (!hit.collider || !hit.collider.CompareTag("EffectCollider")) return;

        var effect = effectPool.Request() as ClickEffect;
        
        if (effect == null) return;

        effect.transform.position = _mainCam.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        effect.OnFinished += OnFinishedPlaying;
        effect.Play();
    }

    private void OnFinishedPlaying(PooledObject effect)
    {
        effect.OnFinished -= OnFinishedPlaying;
        effectPool.Return(effect);
    }
}