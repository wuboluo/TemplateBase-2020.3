using Luna.Unity;
using UnityEngine;

public class LunaManager : MonoBehaviour
{
    public static LunaManager Instance;

    [LunaPlaygroundField("1-xx 2-xx", 1, "Start")]
    public int start;

    [LunaPlaygroundField("1-xx 2-xx", 1, "Middle")]
    public int middle;

    [LunaPlaygroundField("1-xx 2-xx", 1, "End")]
    public int end;

    [Space(15)] public string copyCode;

    public static bool InUnityEditor
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    public int StartParameter
    {
        get
        {
            if (InUnityEditor) return start;
            return TestSCR.Instance.ReturnIsLuan()
                ? start
                : TestSCR.Instance.ReturnNumberIndex(0);
        }
    }

    public int MiddleParameter
    {
        get
        {
            if (InUnityEditor) return middle;
            return TestSCR.Instance.ReturnIsLuan()
                ? middle
                : TestSCR.Instance.ReturnNumberIndex(1);
        }
    }

    public int EndParameter
    {
        get
        {
            if (InUnityEditor) return end;
            return TestSCR.Instance.ReturnIsLuan()
                ? end
                : TestSCR.Instance.ReturnNumberIndex(2);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public static void LunaGoStore()
    {
#if !UNITY_EDITOR
        TestSCR.Instance.Finish();
        TestSCR.Instance.GoStore();
#endif
        Playable.InstallFullGame();
    }
}