using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Define_System
{
    public static int form_Normal = 0; // 标准窗体
    public static int form_Dialog = 1; // 会话窗体
    public static bool Use_HotKey = false;
    public static bool DialogUse_HotKey = false;
    public static string[] acceptSetMenu = new string[2] { "[✔ 应用设置 ]", "[✘ 取消 ]" };

    public static int form_Show = 0; // 打开方式show
    public static int form_ShowDialog = 1; // 打开方式ShowDialog

    public static int input_Normal = 0; // 输入默认方式
    public static int input_Pasword = 1; // 输入密码方式
    public static int input_Number = 2; // 输入整数方式

    public static int check_Normal = 0; // 默认checkbox 选中方式
    public static int check_AlwaysTrue = 1; // checkbox始终选中

    public static Font system_Font = new Font("微软雅黑", 9, FontStyle.Regular);
    public static Color system_FontColor = Color.FromArgb(125, 125, 125);
    public static Color system_backColor = Color.FromArgb(238, 238, 242);
    public static Color system_selectColor = Color.FromArgb(201, 222, 245);
    public static Color system_buttonColor = Color.FromArgb(90, 90, 90);
    public static Color system_btnEnter = Color.Salmon;
    public static Color system_inputColor = Color.Salmon;
    public static Color system_previewFontColor = Color.Salmon;

    public static Color[] colors = new Color[]
    {
        Color.FromArgb(0,0,0), Color.FromArgb(64,64,64), Color.FromArgb(255,0,0),Color.FromArgb(255,106,0),
        Color.FromArgb(225,216,0), Color.FromArgb(182,255,0), Color.FromArgb(76,255,0),Color.FromArgb(0,255,33),
        Color.FromArgb(0,255,144), Color.FromArgb(0,255,255), Color.FromArgb(0,148,255),Color.FromArgb(0,38,255),
        Color.FromArgb(72,0,255), Color.FromArgb(178,0,255), Color.FromArgb(255,0,255),Color.FromArgb(255,0,110),
    };
}
