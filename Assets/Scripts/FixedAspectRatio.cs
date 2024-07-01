using UnityEngine;

public class ForceAspectRatio : MonoBehaviour
{
    private float targetAspect = 16.0f / 9.0f;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        AdjustAspectRatio();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            AdjustAspectRatio();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    private void AdjustAspectRatio()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        // 현재 창 위치 가져오기
        Vector2Int windowPosition = GetWindowPosition();

        if (scaleHeight < 1.0f)
        {
            int width = Screen.width;
            int height = Mathf.RoundToInt(width / targetAspect);
            Screen.SetResolution(width, height, false);
        }
        else
        {
            int height = Screen.height;
            int width = Mathf.RoundToInt(height * targetAspect);
            Screen.SetResolution(width, height, false);
        }

        // 창 위치 복원
        SetWindowPosition(windowPosition);
    }

    private Vector2Int GetWindowPosition()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        var window = GetActiveWindow();
        RECT rect;
        GetWindowRect(window, out rect);
        return new Vector2Int(rect.left, rect.top);
#else
        return new Vector2Int(0, 0);
#endif
    }

    private void SetWindowPosition(Vector2Int position)
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        var window = GetActiveWindow();
        SetWindowPos(window, 0, position.x, position.y, 0, 0, SWP_NOZORDER | SWP_NOSIZE);
#endif
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool GetWindowRect(System.IntPtr hWnd, out RECT lpRect);

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(System.IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    private struct RECT
    {
        public int left, top, right, bottom;
    }

    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOSIZE = 0x0001;
#endif
}
