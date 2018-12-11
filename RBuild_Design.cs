using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using static RbControls;
using static Define_Design;
using static Define_PrintPage;
using static Define_System;
using static Define_DrawObject;
using static Define_Draggable;

public class RBuild_Design
{
    public static RbControls_FormCreate design_Form;

    /// <summary>
    /// 报表编辑
    /// </summary>
    public static void Initialize_Design()
    {
        printDocument = new PrintDocument();
        get_pageType();
        Initialize_PageMode();
        Set_PrintPageType(4, 0);

        // 获取打印机
        DefaultPrinter = printDocument.PrinterSettings.PrinterName;//默认打印机名
        foreach (string sprint in PrinterSettings.InstalledPrinters)//获取所有打印机名称
        {
            printers.Add(new Printer_List(sprint));
        }

        Use_HotKey = true;
        design_Form = new RbControls_FormCreate(); // 建立窗体

        RbControls_ConextMenu _ConextMenu = new RbControls_ConextMenu();
        _ConextMenu.Menu(design_Form.panel_Title, RBuild_Menu.total_Menu, RBuild_Menu.menu_text, RBuild_Menu.menu_point, 23, RBuild_Menu.menu_width, system_Font, Color.White, Color.FromArgb(241, 241, 241), Color.Black, RBuild_Menu.ToolMenuMove);
        _ConextMenu.text_label[0].BackColor = Color.FromArgb(241, 241, 241);
        _ConextMenu.text_label[0].ForeColor = Color.Black;

        // 工具栏
        for (int i = 0; i < 4; i++)
        {
            panel_Tool[i] = new PanelEx() { Dock = DockStyle.Top, Height = 35, Cursor = Cursors.Default, BackColor = Color.FromArgb(238, 238, 242) };
            PanelEx panel_Line = new PanelEx() { Dock = DockStyle.Bottom, Height = 1, BackgroundImage = EzRBuild.EzResource.line, BackgroundImageLayout = ImageLayout.Stretch };
            panel_Tool[i].Controls.Add(panel_Line);

            if (i == 0) panel_Tool[i].Visible = true;
            else panel_Tool[i].Visible = false;

            RBuild_Menu.tool_Control(i, panel_Tool[i]);
            design_Form._formObject.Controls.Add(panel_Tool[i]);
        }

        // 页面设计
        page_Desing = new PanelEx()
        {
            Size = new Size(design_Form._formObject.Width - 4, design_Form._formObject.Height - 161),
            Cursor = Cursors.Default,
            Location = new Point(2, 97)
        };
        design_Form._formObject.Controls.Add(page_Desing);
        
        // 页面容器
        page_Container = new PanelEx()
        {
            Size = new Size(page_Desing.Width - 20, page_Desing.Height - 20),
            Location = new Point(20, 20),
            AutoScroll = true,
            Cursor = Cursors.Default,
            BackColor = Color.FromArgb(195, 195, 195)
        };
        page_Container.Paint += RBuild_Paint.pageContainer_Paint;
        page_Desing.Controls.Add(page_Container);

        // 页面装入框
        int _iLeft = (page_Container.Width / 2) - (page_TypeFace.Page_Area.Width / 2);
        if (_iLeft < 0) _iLeft = 0;
        page_Install = new PanelEx()
        {
            Size = new Size(page_TypeFace.Page_Area.Width + 20, page_TypeFace.Page_Area.Height + 20),
            Location = new Point(_iLeft, 0),
            Cursor = Cursors.Default,
            BackColor = Color.Transparent
        };
        page_Install.Paint += RBuild_Paint.pageInstall_Paint;
        page_Container.Controls.Add(page_Install);
        // 页面
        Print_PageType = new PanelEx()
        {
            Size = page_TypeFace.Page_Area,
            Location = new Point(10, 10),
            Cursor = Cursors.Default,
            BackColor = Color.White
        };
        Print_PageType.Paint += RBuild_Paint.PrintPageType_Paint;
        Print_PageType.Click += RBuild_MouseEvent.PageType_Click;
        Print_PageType.MouseClick += RBuild_MouseEvent.PageType_MouseClick;
        Print_PageType.MouseUp += RBuild_MouseEvent.PageType_MouseUp;
        Print_PageType.MouseDown += RBuild_MouseEvent.PageType_MouseDown;
        Print_PageType.MouseMove += RBuild_MouseEvent.PageType_MouseMove;
        page_Install.Controls.Add(Print_PageType);

        // 标尺
        PictureBoxEx pic_VRuler = new PictureBoxEx()
        {
            Dock = DockStyle.Left,
            Image = VRuler(1100),
            Width = 20,
            Cursor = Cursors.Default,
            BackColor = Color.White
        };
        page_Desing.Controls.Add(pic_VRuler);

        PictureBoxEx pic_HRuler = new PictureBoxEx()
        {
            Dock = DockStyle.Top,
            Image = HRuler(2000),
            Height = 20,
            Cursor = Cursors.Default,
            BackColor = Color.White
        };
        page_Desing.Controls.Add(pic_HRuler);

        Initialize_Band();
        RBuild_Menu.Initialize_MenuStrip();
        RBuild_Info.Initialize_Info(design_Form._formObject);


        RBuild_Info.Composite_Info = new RbControls_TextLabel();
        RBuild_Info.Composite_Info.Text_Label(
            design_Form.panel_Title,new Point(225, 40),system_Font, Color.FromArgb(230,230,230),
            "✐  当前纸张：" + page_types[page_TypeFace.Page_Type] +", 纵向"+ "  ✐  打印机：" + DefaultPrinter + "  ✐  数据库： None  ✐"
            );

        RbControls_DrawTextMethod ds = new RbControls_DrawTextMethod();
        Bitmap report_Icon = new Bitmap(20, 20);
        ds.DrawFontAwesome(report_Icon, RbControls_FontAwesome.Type.FileText, 20, Color.FromArgb(250, 250, 250), new Point(0, 0), false);
        PictureBoxEx report_pic = new PictureBoxEx()
        {
            Size = new Size(20, 20),
            Location = new Point(10, 6),
            Image = report_Icon,
            BackColor = Color.Transparent
        };
        design_Form.panel_Title.Controls.Add(report_pic);

        ReportFile_Name = new Label()
        {
            AutoSize = true,
            Location = new Point(30, 6),
            Font = system_Font,
            ForeColor = Color.White,
            Text = "MyReport.rpt",
            BackColor = Color.Transparent
        };
        design_Form.panel_Title.Controls.Add(ReportFile_Name);

        design_Form.Create_Form(page_Container, "design_Form", FormStartPosition.Manual, new Size(1200, 768), new Point(200, 100), system_backColor, form_ShowDialog, false, designForm_Close, formResize); // 显示窗体
    }

    /// <summary>
    /// 窗体改变大小
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void formResize(object sender, EventArgs e)
    {
        design_Form.LocationEX = design_Form._formObject.Location;
        design_Form.SizeEX = design_Form._formObject.Size;

        page_Desing.Size = new Size(design_Form.SizeEX.Width - 4, design_Form.SizeEX.Height - 161);
        page_Container.Size = new Size(page_Desing.Width - 20, page_Desing.Height - 20);

        int _iLeft = (page_Container.Width / 2) - (page_TypeFace.Page_Area.Width / 2);
        if (_iLeft < 0) _iLeft = 0;
        page_Install.Location = new Point(_iLeft, 0);

        RBuild_Info.panel_State.Size = new Size(design_Form.SizeEX.Width - 4, 62);
        RBuild_Info.panel_State.Location = new Point(2, design_Form.SizeEX.Height - 64);
    }

    // 关闭窗口
    public static void designForm_Close(object sender, EventArgs e)
    {
        if (ReportChange_Flag)
        {
            DialogResult _save = MessageBox.Show("是否保存报表文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (_save.ToString().Equals("Yes"))
            {
                RBuild_File.Save_File();
            }
            if (_save.ToString().Equals("No")) design_Form._formObject.Close();
        }
        else
        {
            design_Form._formObject.Close();
        }
    }
}
