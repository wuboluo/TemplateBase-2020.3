using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AudioConfigSOCreator : EditorWindow
{
    private const string SingleSourceConfigAssetMenuPath = "Assets/YANG/创建：独立音频";
    private const string RandomSourceConfigAssetMenuPath = "Assets/YANG/创建：随机音频组";

    [MenuItem(SingleSourceConfigAssetMenuPath)]
    public static void CreateSingleSourceConfig()
    {
        var selectObjs = Selection.objects;
        var folders = GetSelectAssetFolderPath();

        for (var i = 0; i < selectObjs.Length; i++)
        {
            var scAsset = CreateInstance<SingleSourceConfigSO>();
            scAsset.clip = selectObjs[i] as AudioClip;

            var savePath = $"{folders[i]}SourceConfig {selectObjs[i].name}.asset";

            SaveSourceConfigAsset(scAsset, savePath);
        }
    }

    [MenuItem(RandomSourceConfigAssetMenuPath)]
    public static void CreateRandomSourceConfig()
    {
        var selectObjs = Selection.objects;
        var folders = GetSelectAssetFolderPath();

        var scAsset = CreateInstance<RandomSourceConfigSO>();
        scAsset.clips = Array.ConvertAll(selectObjs, value => value as AudioClip);

        var savePath = $"{folders.First()}SourceConfig {selectObjs.First().name}.asset";

        SaveSourceConfigAsset(scAsset, savePath);
    }

    [MenuItem(SingleSourceConfigAssetMenuPath, true)]
    [MenuItem(RandomSourceConfigAssetMenuPath, true)]
    private static bool SelectedAsset()
    {
        return GetAssetExtension().ToList().All(e => e == "mp3" || e == "wav" || e == "ogg");
    }

    private static void SaveSourceConfigAsset(Object scAsset, string savePath)
    {
        AssetDatabase.CreateAsset(scAsset, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static string[] GetSelectAssetPath()
    {
        var paths = Array.ConvertAll(Selection.objects, value => AssetDatabase.GetAssetPath(value).Replace('/', Path.DirectorySeparatorChar));
        return paths;
    }

    private static IEnumerable<string> GetAssetExtension()
    {
        var extensions = Array.ConvertAll(GetSelectAssetPath(), value =>
        {
            var splits = value.Split('.');
            return splits[splits.Length - 1];
        });

        return extensions;
    }

    private static string[] GetSelectAssetFolderPath()
    {
        var folderPaths = Array.ConvertAll(GetSelectAssetPath(), value => value.Substring(0, value.LastIndexOf(Path.DirectorySeparatorChar) + 1));
        return folderPaths;
    }
}