using System.Reflection;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(SingleSourceConfigSO))]
public class SingleSourceConfigSOEditor : Editor
{
    private SingleSourceConfigSO _singleSourceConfig;

    private void OnEnable()
    {
        _singleSourceConfig = target as SingleSourceConfigSO;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Play")) PlayOrStopClip(_singleSourceConfig.clip, true);
        if (GUILayout.Button("Stop")) PlayOrStopClip(_singleSourceConfig.clip, false);
    }

    private static void PlayOrStopClip(Object clip, bool play)
    {
        if (clip == null) return;

        var command = play ? "PlayPreviewClip" : "StopClip";

        var unityEditorAssembly = typeof(AudioImporter).Assembly;
        var audioUtil = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        var method = audioUtil.GetMethod
        (
            command, BindingFlags.Default | BindingFlags.Static | BindingFlags.Public, null, new[] {typeof(AudioClip)}, null
        );
        if (method != null)
        {
            method.Invoke(null, new object[] {clip});
        }
        else
        {
            var methodInfo = audioUtil.GetMethods();
            foreach (var mInfo in methodInfo) Debug.Log(mInfo);
        }
    }
}