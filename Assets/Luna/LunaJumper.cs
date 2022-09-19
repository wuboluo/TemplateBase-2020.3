using UnityEngine;
using UnityEngine.UI;

public class LunaJumper : MonoBehaviour
{
    [SerializeField] private ClickType type;

    private void Start()
    {
        if (LunaManager.InUnityEditor) return;
        gameObject.TryGetComponent<Button>(out var button);
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        switch (type)
        {
            case ClickType.Copy:
#if !UNITY_EDITOR
                TestSCR.Instance.CopyText(LunaManager.Instance.copyCode);
#endif
                LunaManager.LunaGoStore();
                break;

            case ClickType.Download:
                LunaManager.LunaGoStore();
                break;
        }
    }
}

public enum ClickType
{
    Download,
    Copy
}