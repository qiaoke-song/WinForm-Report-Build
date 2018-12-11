using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class RbControls_CustomMouse
{
    /// <summary>
    /// 自定义鼠标
    /// </summary>
    public struct IconInfo
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }
    [DllImport("user32.dll")]
    public static extern IntPtr CreateIconIndirect(ref IconInfo icon);
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool DestroyIcon(IntPtr hIcon);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    public static Cursor custom_MouseCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
    {
        IconInfo ii = new IconInfo();
        ii.fIcon = false;
        Bitmap img = bmp;
        IntPtr imgHandle = img.GetHbitmap();
        try
        {
            ii.hbmMask = imgHandle;
            ii.hbmColor = imgHandle;
            ii.xHotspot = xHotSpot;
            ii.yHotspot = yHotSpot;
            IntPtr handle = CreateIconIndirect(ref ii);
            if (handle == IntPtr.Zero)
            {
                MessageBox.Show("鼠标定义错误！");
            }
            return new Cursor(handle);

        }
        finally
        {
            DeleteObject(imgHandle);
        }
    }
}
