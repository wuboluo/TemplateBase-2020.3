using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SourcePlayer : PooledObject
{
    private AudioSource _source;
    public string ID { get; private set; }

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    protected override void OnWakeUp()
    {
    }

    protected override void OnFinish()
    {
    }

    public void Play(AudioClip clip, bool isLoop = false, string id = "", float volume = 1)
    {
        _source.clip = clip;
        _source.loop = isLoop;
        _source.volume = volume;

        ID = id;

        if (!_source.loop) DelayedFinish(clip.length);
        _source.Play();
    }
}