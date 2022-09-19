using UnityEngine;

public class RandomSourceConfigSO : ScriptableObject, ISource
{
    public AudioClip[] clips;
    public string id;
    [Range(0, 1)] public float volume = 1;
}