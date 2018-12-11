using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static RbControls;

public class Define_Design
{
    public static PanelEx page_Desing; // 设计框
    public static PanelEx page_Container; // 页面容器
    public static PanelEx page_Install; // 页面装入容器
    public static PanelEx Print_PageType; // 设计页面
    public static int select_Band = 0; // 选择哪个栏目
    public static bool Show_Line = true; // 是否显示组件标线
    public static PanelEx[] panel_Tool = new PanelEx[4]; // 工具栏

    public static PointF[] CursorArea = new PointF[8]; // 组件鼠标区域
    /// <summary>
    /// 组件图标
    /// </summary>
    public static Bitmap[] control_icon = new Bitmap[6] {
        EzRBuild.EzResource.txtbox,
        EzRBuild.EzResource.img,
        EzRBuild.EzResource.shape,
        EzRBuild.EzResource.field,
        EzRBuild.EzResource.pagecode,
        EzRBuild.EzResource.func
    };
    /// <summary>
    /// 形状组件图像
    /// </summary>
    public static RbControls_FontAwesome.Type[] shaps_type = new RbControls_FontAwesome.Type[] {
       RbControls_FontAwesome.Type.Square,
       RbControls_FontAwesome.Type.Star,
       RbControls_FontAwesome.Type.CheckCircle,
       RbControls_FontAwesome.Type.Bookmark,
       RbControls_FontAwesome.Type.Certificate,
       RbControls_FontAwesome.Type.Commenting,
       RbControls_FontAwesome.Type.ExclamationTriangle,
       RbControls_FontAwesome.Type.Flag,
       RbControls_FontAwesome.Type.Circle,
       RbControls_FontAwesome.Type.Cloud,
       RbControls_FontAwesome.Type.Flash,
       RbControls_FontAwesome.Type.PieChart,
       RbControls_FontAwesome.Type.ArrowLeft,
       RbControls_FontAwesome.Type.ArrowRight,
       RbControls_FontAwesome.Type.ArrowUp,
       RbControls_FontAwesome.Type.ArrowDown,
       RbControls_FontAwesome.Type.FileText,
       RbControls_FontAwesome.Type.QuestionCircle,
       RbControls_FontAwesome.Type.Cny,
       RbControls_FontAwesome.Type.Dollar
    };

    public static Label ReportFile_Name; // 当前报表文件名
    public static bool View_Info = false; // 线程显示组件信息状态
    public static bool ReportChange_Flag = false; // 报表修改状态
    public static int oper_Record = -1; // 操作动作序号

   
}
