using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>
{
    [SerializeField] private ObjectPoolingSO sourcePool;
    private readonly List<SourcePlayer> _playingSources = new List<SourcePlayer>();

    protected override void OnAwake()
    {
        sourcePool.Prewarm(transform);
    }

    public void PlaySource(ISource config)
    {
        var source = sourcePool.Request() as SourcePlayer;

        if (source == null) return;

        _playingSources.Add(source);
        source.OnFinished += OnSourceFinishedPlaying;

        switch (config)
        {
            case SingleSourceConfigSO singleSO:
                source.Play(singleSO.clip, singleSO.isLoop, singleSO.id, singleSO.volume);
                break;
            case RandomSourceConfigSO randomSO:
            {
                var randomIndex = Random.Range(0, randomSO.clips.Length);
                source.Play(randomSO.clips[randomIndex], false, randomSO.id, randomSO.volume);
                break;
            }
        }
    }

    public void StopAllPlayingSource()
    {
        for (var i = _playingSources.Count - 1; i >= 0; i--)
        {
            var source = _playingSources[i];
            sourcePool.Return(source);
            OnSourceFinishedPlaying(source);
        }
    }

    private void OnSourceFinishedPlaying(PooledObject source)
    {
        source.OnFinished -= OnSourceFinishedPlaying;
        _playingSources.Remove(source as SourcePlayer);
    }

    public void StopSourceAtOnceByID(params string[] ids)
    {
        if (ids == null) return;

        foreach (var id in ids)
        {
            var result = _playingSources.FindAll(s => s.ID == id);
            if (result.Count > 0)
                result.ForEach(r =>
                {
                    sourcePool.Return(r);
                    OnSourceFinishedPlaying(r);
                });
        }
    }

    public bool IsPlayingByID(string id)
    {
        return _playingSources.Count > 0 && _playingSources.Find(s => s.ID == id) == null;
    }
}