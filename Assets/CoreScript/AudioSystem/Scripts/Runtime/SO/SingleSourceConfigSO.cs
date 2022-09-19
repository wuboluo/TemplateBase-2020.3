using UnityEngine;

public class SingleSourceConfigSO : ScriptableObject, ISource
{
    public AudioClip clip;
    public string id;
    public bool isLoop;
    [Range(0, 1)] public float volume = 1;
}