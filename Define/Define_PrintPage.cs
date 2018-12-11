using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

public static class Define_PrintPage
{
    public static PrintDocument printDocument;
    public static string DefaultPrinter; // 默认打印机
    public static PaperSize _pType = null;

    // 预览纸张设置
    public static Size PreViewPage_Area;
    public static double zoomScale = 1; // 预览缩放比例

    public static int Print_Scope = -1; // 0.全部页 1.单数页 2.双数页
    public static List<int> page_Scope = new List<int>();  // 选择打印页范围

    public static int _pgselect = 4; // 纸张选择
    public static int _pgdirect = 0; // 方向-纵向

    /// <summary>
    /// 纸张设置
    /// </summary>
    [Serializable]
    public class Page_TypeFace
    {
        public int Page_Type = 4; // 纸张类型
        public int Page_Direction; // 纸张方向
        public Size Page_Area; // 大小
        public int[] Page_Margin = new int[] { 0, 0, 0, 0 }; // 边距
        public string[] Rect_mm = { "", "" }; // 自定义纸张宽度高度
    }
    public static Page_TypeFace page_TypeFace = new Page_TypeFace();

    /// 定义纸张大小类型
    public class Page_Mode
    {
        public string page_Type;
        public int _tag;
        public Size[] page_Direction = new Size[2];
    }
    public static List<Page_Mode> page_Modes = new List<Page_Mode>();

    /// <summary>
    /// 所有打印机
    /// </summary>
    public class Printer_List
    {
        public string printerName;
        public Printer_List(string _printer)
        {
            printerName = _printer;
        }
    }
    public static List<Printer_List> printers = new List<Printer_List>();

    /// <summary>
    /// 所有支持的纸张类型
    /// </summary>
    public class PrintType_List
    {
        public string pageType;
        public PrintType_List(string _type)
        {
            pageType = _type;
        }
    }
    public static List<PrintType_List> pageType_Lists = new List<PrintType_List>();

    public static string[] page_types = new string[24] {
            "A0","A1","A2","A3","A4","A5","A6","A7",
            "B0","B1","B2","B3","B4","B5","B6","B7",
            "C0","C1","C2","C3","C4","C5","C6","C7"
        };
    public static string[] page_size = new string[24] {
            "841×1189","594×841","420×594","297×420","210×297","148×210","105×148","74×105",
            "1000×1414","707×1000","500×707","353×500","250×353","176×250","125×176","88×125",
            "917×1297","648×917","458×648","324×458","229×324","162×229","114×162","81×114"
        };
    public static string[] page_pixel = new string[24] {
            "3178×4493","2245×3178","1587×2245","1122×1587","793×1122","559×793","396×559","279×396",
            "3779×5344","2672×3779","1889×2672","1334×1889","944×1334","665×944","472×665","332×472",
            "3465×4902","2449×3465","1731×2449","1235×1731","865×1224","612×865","430×612","306×430"
        };

    /// <summary>
    /// 设置打印纸张类型
    /// </summary>
    public static void get_pageType()
    {
        pageType_Lists.Clear();
        pageType_Lists.Add(new PrintType_List("自定义纸张"));

        for (int i = 0; i < 24; i++)
        {
            pageType_Lists.Add(new PrintType_List(page_types[i] + ", " + page_size[i] + " " + "毫米"));
        }
    }

    /// <summary>
    /// 添加定义的纸张类型
    /// </summary>
    public static void Initialize_PageMode()
    {
        for (int i = 0; i < 24; i++)
        {
            Page_Mode pm = new Page_Mode();
            pm.page_Type = page_types[i];
            pm._tag = i;
            string[] splitString = page_pixel[i].Split(new string[] { "×" }, StringSplitOptions.None);
            pm.page_Direction[0] = new Size(int.Parse(splitString[0]), int.Parse(splitString[1]) + 13 + 126);
            pm.page_Direction[1] = new Size(int.Parse(splitString[1]), int.Parse(splitString[0]) + 13 + 126);
            //pm.page_Direction[1] = new Size(int.Parse(splitString[1]+13), int.Parse(splitString[0]) + 126);
            page_Modes.Add(pm);
        }
    }

    /// <summary>
    /// 加载纸张类型
    /// </summary>
    /// <param name="_mode">类型</param>
    /// <param name="_direction">方向</param>
    public static void Set_PrintPageType(int _mode, int _direction)
    {
        page_TypeFace.Page_Type = page_Modes[_mode]._tag;
        page_TypeFace.Page_Direction = _direction;
        //////page_TypeFace.Page_Area = page_Modes[_mode].page_Direction[_direction];
        page_TypeFace.Page_Margin = new int[4] { 0, 0, 0, 0 };

        if (_direction == 0) page_TypeFace.Page_Area = page_Modes[_mode].page_Direction[_direction];
        else page_TypeFace.Page_Area = new Size(page_Modes[_mode].page_Direction[_direction].Width + 39, page_Modes[_mode].page_Direction[_direction].Height - 13);

        // 设置预览页面大小
        PreViewPage_Area = new Size(page_Modes[_mode].page_Direction[_direction].Width, page_Modes[_mode].page_Direction[_direction].Height - 126 - 13);
    }


}
