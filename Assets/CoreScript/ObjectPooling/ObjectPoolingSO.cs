using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ObjectPooling", menuName = "Yang/New Pool/New ObjectPoolingSO")]
public class ObjectPoolingSO : ScriptableObject
{
    public GameObject unit;
    public int size;

    private readonly Stack<PooledObject> _pool = new Stack<PooledObject>();

    private Transform _unitRoot;

    public void Prewarm(Transform root)
    {
        _unitRoot = root;
        for (var i = 0; i < size; i++) _pool.Push(Create());
    }

    public PooledObject Request()
    {
        var member = _pool.Count > 0 ? _pool.Pop() : Create();
        member.gameObject.SetActive(true);
        member.OnFinished += Return;

        return member;
    }

    public void Return(PooledObject member)
    {
        member.OnFinished -= Return;

        _pool.Push(member);
        member.gameObject.SetActive(false);
    }

    private PooledObject Create()
    {
        var tmp = Instantiate(unit, _unitRoot).GetComponent<PooledObject>();
        tmp.gameObject.SetActive(false);

        return tmp;
    }
}