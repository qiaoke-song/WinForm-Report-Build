using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Define_Draggable;
using static Define_System;
using static Define_DrawObject;
using static Define_Design;

public partial class winFormEX : Form
{
    public bool Sizeable = true;

    [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn
    (
        int nLeftRect,
        int nTopRect,
        int nRightRect,
        int nBottomRect,
        int nWidthEllipse,
        int nHeightEllipse
    );

    [DllImport("dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll")]
    public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

    private bool m_aeroEnabled = false;
    private const int CS_DROPSHADOW = 0x00020000;
    private const int WM_NCPAINT = 0x0085;
    private const int WM_ACTIVATEAPP = 0x001C;

    public struct MARGINS
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;
    }

    private const int WM_NCHITTEST = 0x84;
    private const int HTCLIENT = 0x1;
    private const int HTCAPTION = 0x2;

    protected override CreateParams CreateParams
    {
        get
        {
            m_aeroEnabled = CheckAeroEnabled();

            CreateParams cp = base.CreateParams;
            if (!m_aeroEnabled)
                cp.ClassStyle |= CS_DROPSHADOW;

            return cp;
        }
    }

    private bool CheckAeroEnabled()
    {
        if (Environment.OSVersion.Version.Major >= 6)
        {
            int enabled = 0;
            DwmIsCompositionEnabled(ref enabled);
            return (enabled == 1) ? true : false;
        }
        return false;
    }

    const int Guying_HTLEFT = 10;
    const int Guying_HTRIGHT = 11;
    const int Guying_HTTOP = 12;
    const int Guying_HTTOPLEFT = 13;
    const int Guying_HTTOPRIGHT = 14;
    const int Guying_HTBOTTOM = 15;
    const int Guying_HTBOTTOMLEFT = 0x10;
    const int Guying_HTBOTTOMRIGHT = 17;

    bool convert = false;

    public class AppHotKey
    {
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            KeyModifiers fsModifiers,
            Keys vk
            );
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,
            int id
            );
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }

        public static void RegKey(IntPtr hwnd, int hotKey_id, KeyModifiers keyModifiers, Keys key)
        {
            try
            {
                RegisterHotKey(hwnd, hotKey_id, keyModifiers, key);
            }
            catch (Exception) { }
        }

        public static void UnRegKey(IntPtr hwnd, int hotKey_id)
        {
            UnregisterHotKey(hwnd, hotKey_id);
        }
    }

    private const int WM_HOTKEY = 0x312; // 热键
    private const int WM_CREATE = 0x1; // 创建窗口消息
    private const int WM_DESTROY = 0x2; // 销毁

    private int[] hot_Keys = new int[] { 0x1000, 0x1001, 0x1002, 0x1003, 0x1004, 0x1005, 0x1006, 0x1007, 0x1008, 0x1009, 0x1010, 0x1011, 0x1012, 0x1013, 0x1014, 0x1015, 0x1016 };
    private Keys[] keys = new Keys[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Delete, Keys.X, Keys.C, Keys.V, Keys.Z, Keys.R, Keys.T, Keys.B, Keys.P };

    protected override void WndProc(ref Message m)
    {
        convert = false;
        int sizeX = 0;
        int sizeY = 0;
        int sizeWidth = 0;
        int sizeHeight = 0;
        if (DraggableObjects.Count > 0)
        {
            convert = true;
            sizeX = DraggableObjects[control_Num].Region.Left;
            sizeY = DraggableObjects[control_Num].Region.Top;
            sizeWidth = DraggableObjects[control_Num].Region.Width;
            sizeHeight = DraggableObjects[control_Num].Region.Height;
        }
        base.WndProc(ref m);

        switch (m.Msg)
        {
            case WM_NCPAINT: 
                if (m_aeroEnabled)
                {
                    var v = 2;
                    DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                    MARGINS margins = new MARGINS()
                    {
                        bottomHeight = 1,
                        leftWidth = 1,
                        rightWidth = 1,
                        topHeight = 1
                    };
                    DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                }
                break;
            case 0x0084:
                if (Sizeable)
                {
                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                        (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMLEFT;
                        else m.Result = (IntPtr)Guying_HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMRIGHT;
                        else m.Result = (IntPtr)Guying_HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)Guying_HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)Guying_HTBOTTOM;
                }
                break;
            case WM_HOTKEY:
                if (Use_HotKey)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case 0x1000: // Ctrl_Up: 
                            sizeY -= 1;
                            break;
                        case 0x1001: // Ctrl_Down: 
                            sizeY += 1;
                            break;
                        case 0x1002: // Ctrl_Left:
                            sizeX -= 1;
                            break;
                        case 0x1003: // Ctrl_Right:
                            sizeX += 1;
                            break;
                        case 0x1004: // Shift_Up:
                            sizeHeight -= 1;
                            break;
                        case 0x1005: // Shift_Down:
                            sizeHeight += 1;
                            break;
                        case 0x1006: // Shift_Left:
                            sizeWidth -= 1;
                            break;
                        case 0x1007: // Shift_Right:
                            sizeWidth += 1;
                            break;
                        case 0x1008: // Alt_Delete
                            Delete_Control(true);
                            convert = false;
                            break;
                        case 0x1009: // Alt_X
                            Object_OperationCopy();
                            Delete_Control(true);
                            convert = false;
                            break;
                        case 0x1010: // Alt_C
                            Object_OperationCopy();
                            convert = false;
                            break;
                        case 0x1011: // Alt_V
                            Object_OperationPast(0, 21);
                            convert = false;
                            break;
                        case 0x1012: // Alt_Z
                            Object_Operation(0);
                            convert = false;
                            break;
                        case 0x1013: // Alt_R
                            Object_Operation(1);
                            convert = false;
                            break;
                        case 0x1014: // Alt_T
                            ListSwap_Top();
                            convert = false;
                            break;
                        case 0x1015: // Alt_B
                            ListSwap_Bottom();
                            convert = false;
                            break;
                        case 0x1016: // Alt_P
                            new RBuild_Preview().View();
                            convert = false;
                            break;
                        default:
                            break;
                    }
                    if (convert)
                    {
                        if (sizeWidth < 14) sizeWidth = 14;
                        if (sizeHeight < 14) sizeHeight = 14;
                        if (sizeX < 0)
                        {
                            sizeX = 0;
                            sizeWidth = DraggableObjects[control_Num].Region.Width;
                        }
                        if ((sizeX + sizeWidth) > Print_PageType.Width)
                        {
                            sizeWidth = DraggableObjects[control_Num].Region.Width;
                            sizeX = DraggableObjects[control_Num].Region.Left;
                        }
                        if (sizeY < 0)
                        {
                            sizeY = 0;
                            sizeHeight = DraggableObjects[control_Num].Region.Height;
                        }
                        if ((sizeY + sizeHeight) > DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Bottom - 20)
                        {
                            sizeHeight = DraggableObjects[control_Num].Region.Height;
                            sizeY = DraggableObjects[control_Num].Region.Top;
                        }
                        if (sizeY < (DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Top + 20))
                        {
                            sizeY = DraggableObjects[control_Num].Region.Top;
                        }

                        DraggableObjects[control_Num].Setimage = LinBox(sizeWidth, sizeHeight, 1, DraggableObjects[control_Num].ControlType, control_Num);
                        DraggableObjects[control_Num].Region = new Rectangle(sizeX, sizeY, sizeWidth, sizeHeight);
                        Print_PageType.Invalidate();
                        View_Info = true;
                    }
                }
                break;
            case WM_CREATE: // 创建窗口消息
                if (Use_HotKey)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        AppHotKey.RegKey(Handle, hot_Keys[i], AppHotKey.KeyModifiers.Ctrl, keys[i]);
                        AppHotKey.RegKey(Handle, hot_Keys[i + 4], AppHotKey.KeyModifiers.Shift, keys[i]);
                    }
                    for (int i = 4; i < 13; i++) AppHotKey.RegKey(Handle, hot_Keys[i + 4], AppHotKey.KeyModifiers.Alt, keys[i]);
                }
                break;

            case WM_DESTROY: // 销毁窗口消息
                if (Use_HotKey)
                {
                    for (int i = 0; i < 16; i++) AppHotKey.UnRegKey(Handle, hot_Keys[i]);
                }
                break;
            default:
                break;
        }
    }

   


}
