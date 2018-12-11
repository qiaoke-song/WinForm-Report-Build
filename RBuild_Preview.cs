using System;
using System.Windows.Forms;
using System.Drawing;
using static RbControls;
using static Define_PrintPage;
using static Define_System;
using static Define_ReportFunction;

public class RBuild_Preview
{
    public static RbControls_FormCreate preview_Form;
    private PanelEx[] panel_Tool = new PanelEx[2];
    private Label[] menu_labels = new Label[2];
    private int total_Menu;

    private PanelEx page_View;
    private PanelEx page_Container;
    private PanelEx page_Install;
    private PanelEx[] panel_Outer;
    public static PanelEx[] panel_Page;
    private PanelEx panel_Info;

    private ContextMenuStrip Preview_Menu;
    private string[] pm_text = new string[10] { "    200%", "    150%", "    100%", "    75%", "    50%", "    25%", "    10%", "    页宽", "    全页", "    双页     " };
    private ToolStripMenuItem[] pm_Item = new ToolStripMenuItem[10];

    private RbControls_TextLabel page_Info, pg_Total, pg_Now;
    private int select_MenuNum = 2;
    private int Now_Page;

    public void View()
    {
        Use_HotKey = false;

        preview_Form = new RbControls_FormCreate(); // 建立窗体
        preview_Form._formObject.Size = new Size(1056, 729);
        preview_Form._formObject.FormClosed += Preview_FormClosed;
        zoomScale = 1;
        Now_Page = 1;

        for (int i = 0; i < 2; i++)
        {
            panel_Tool[i] = new PanelEx() { Dock = DockStyle.Top, Height = 35, Cursor = Cursors.Default, BackColor = Color.FromArgb(241, 241, 241) };
            PanelEx panel_Line = new PanelEx() { Dock = DockStyle.Bottom, Height = 1, BackgroundImage = EzRBuild.EzResource.line, BackgroundImageLayout = ImageLayout.Stretch };
            panel_Tool[i].Controls.Add(panel_Line);

            if (i == 0) panel_Tool[i].Visible = true;
            else panel_Tool[i].Visible = false;

            previewTool_Control(i, panel_Tool[i]);
            preview_Form._formObject.Controls.Add(panel_Tool[i]);
        }

        string[] menu_text = new string[2] { "预览", "帮助" };
        Point[] menu_point = new Point[] { new Point(3, 37), new Point(53, 37) };
        int[] menu_width = new int[] { 43, 43 };
        total_Menu = 2;
        RbControls_ConextMenu PreviewMenu = new RbControls_ConextMenu();
        PreviewMenu.Menu(preview_Form.panel_Title, total_Menu, menu_text, menu_point, 23, menu_width, system_Font,Color.White, Color.FromArgb(241, 241, 241), Color.Black, PreviewMenuMove);
        PreviewMenu.text_label[0].BackColor = Color.FromArgb(241, 241, 241);
        PreviewMenu.text_label[0].ForeColor = Color.Black;

        panel_Preview(preview_Form._formObject);

        display_Page(0);
        preview_Form.Create_Form(page_Container,"preview_Form", FormStartPosition.CenterScreen, new Size(1056, 729), new Point(200, 100), system_backColor, form_ShowDialog, false, null, PreView_Resize);
    }

    private void Preview_FormClosed(object sender, FormClosedEventArgs e)
    {
        Use_HotKey = true;
    }

    /// <summary>
    /// 添加图标到工具栏
    /// </summary>
    /// <param name="panelID">序号</param>
    /// <param name="obj">在哪个控件内</param>
    public void previewTool_Control(int panelID, Control obj)
    {
        switch (panelID)
        {
            case 0:
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(10, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.print, new Point(20, 5), "打印报表", "PREVIEW_PRINT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.search, new Point(50, 5), "查找", "PREVIEW_SEARCH", true);
                addToolControl(obj, 35, 25, EzRBuild.EzResource.zoom1, new Point(80, 5), "缩放", "PREVIEW_ZOOM", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(120, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.first, new Point(130, 5), "最前页", "PREVIEW_FIRST", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.ppirv, new Point(160, 5), "前一页", "PREVIEW_PREVIOUS", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.pnext, new Point(190, 5), "下一页", "PREVIEW_NEXT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.last, new Point(220, 5), "最后一页", "PREVIEW_LAST", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(250, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.close, new Point(260, 5), "关闭", "PREVIEW_CLOSE", true);
                break;
            case 1:
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(10, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.help, new Point(20, 5), "使用帮助", "PREVIEW_HELP", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.icon, new Point(50, 5), "关于系统", "PREVIEW_ABOUT", true);
                break;
            default:
                break;
        }
    }

    private void addToolControl(Control obj, int width, int height, Bitmap img, Point location, string tips, string id, bool showtips)
    {
        ToolTip toolTip = new ToolTip();
        PictureBoxEx toolControl = new PictureBoxEx()
        {
            Size = new Size(width, height),
            SizeMode = PictureBoxSizeMode.CenterImage,
            Image = img,
            Location = location,
            BackColor = Color.Transparent,
            Tag = id
        };
        if (showtips)
        {
            toolTip.SetToolTip(toolControl, tips);
            toolControl.MouseEnter += new EventHandler(toolControl_MouseEnter);
            toolControl.MouseLeave += new EventHandler(toolControl_MouseLeave);
            toolControl.MouseDown += ToolControl_MouseDown;
        }
        obj.Controls.Add(toolControl);
    }

    /// 鼠标移至菜单
    private static void toolControl_MouseEnter(object sender, EventArgs e)
    {
        PictureBoxEx pL = (PictureBoxEx)sender;
        pL.BackColor = system_selectColor;
        pL.Cursor = Cursors.Hand;
    }

    /// 鼠标离开菜单
    private static void toolControl_MouseLeave(object sender, EventArgs e)
    {
        PictureBoxEx pL = (PictureBoxEx)sender;
        pL.BackColor = Color.Transparent;
        pL.Cursor = Cursors.Default;
    }

    /// 点击菜单
    private void ToolControl_MouseDown(object sender, MouseEventArgs e)
    {
        PictureBoxEx pL = (PictureBoxEx)sender;

        int pheight = (int)((PreViewPage_Area.Height) * zoomScale) + 20; //每页高度

        switch (pL.Tag)
        {
            case "PREVIEW_PRINT":
                DialogUse_HotKey = false;
                print_File();
                break;
            case "PREVIEW_SEARCH":
                DialogUse_HotKey = false;
                new RBuild_PreviewSearch().Search();
                break;
            case "PREVIEW_ZOOM":
                Preview_Menu.Show(pL.PointToScreen(new Point(0, 25)));
                break;
            case "PREVIEW_FIRST":
                page_Container.AutoScrollPosition = new Point(0, 0);
                break;
            case "PREVIEW_PREVIOUS":
                if (Now_Page - 1 >= 1)
                {
                    page_Container.AutoScrollPosition = new Point(0, Now_Page * pheight - 2 * pheight);
                }
                break;
            case "PREVIEW_NEXT":
                if (Now_Page + 1 <= Total_Page)
                {
                    page_Container.AutoScrollPosition = new Point(0, Now_Page * pheight);
                }
                break;
            case "PREVIEW_LAST":
                page_Container.AutoScrollPosition = new Point(0, Now_Page * pheight + (Total_Page - Now_Page - 1) * pheight);
                break;
            case "PREVIEW_CLOSE":
                preview_Form._formObject.Close();
                break;
            case "PREVIEW_ABOUT":
                break;
            default:
                break;
        }
    }

    // 鼠标在菜单内
    private void PreviewMenuMove(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;

        for (int i = 0; i < total_Menu; i++)
        {
            if (i != (int)pL.Tag)
            {
                panel_Tool[i].Visible = false;
            }
        }
        panel_Tool[(int)pL.Tag].Visible = true;
    }


    public void panel_Preview(Control obj)
    {
        page_View = new PanelEx()
        {
            Size = new Size(obj.Width - 4, obj.Height - 125),
            Location = new Point(2, 97),
            Cursor = Cursors.Default,
            BackColor = Color.FromArgb(150, 150, 150)
        };
        obj.Controls.Add(page_View);

        page_Container = new PanelEx()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.FromArgb(150, 150, 150)
        };
        page_View.Controls.Add(page_Container);

        page_Install = new PanelEx()
        {
            Size = new Size(page_Container.Width - 20, page_Container.Height),
            Location = new Point(0, 0),
            BackColor = Color.Transparent
        };
        page_Install.MouseDown += PageInstall_MouseDown;
        page_Install.LocationChanged += PageInstall_LocationChanged;
        page_Container.Controls.Add(page_Install);

        // 装入预览页面图像
        Report_generation(page_TypeFace.Page_Direction);
        if (report_Pages.Count > 0)
        {
            int pwidth = PreViewPage_Area.Width;
            int pheight = PreViewPage_Area.Height;

            int px = (page_Install.Width - pwidth + 20) / 2;
            if (px < 0) px = 0;

            panel_Outer = new PanelEx[report_Pages.Count];
            panel_Page = new PanelEx[report_Pages.Count];

            for (int i = 0; i < report_Pages.Count; i++)
            {
                panel_Outer[i] = new PanelEx()
                {
                    Size = new Size(pwidth + 20, pheight + 20),
                    Location = new Point(px, i * (pheight + 20)),
                    BackColor = Color.FromArgb(150, 150, 150),
                };
                panel_Outer[i].MouseDown += PageInstall_MouseDown;

                panel_Page[i] = new PanelEx()
                {
                    Size = new Size(pwidth, pheight),
                    Location = new Point(10, 10),
                    BackColor = Color.FromArgb(250, 250, 250),
                    BackgroundImage = report_Pages[i].pageImage
                };
                panel_Page[i].MouseDown += PageInstall_MouseDown;
                panel_Outer[i].Controls.Add(panel_Page[i]);
                page_Install.Controls.Add(panel_Outer[i]);
                page_Install.AutoSize = true;
            }
        }

        // 缩放菜单
        Preview_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };
        for (int i = 0; i < 10; i++)
        {
            pm_Item[i] = new ToolStripMenuItem()
            {
                AutoSize = true,
                Text = pm_text[i],
                Tag = i
            };
            pm_Item[i].Click += PreView_Click;
            Preview_Menu.Items.Add(pm_Item[i]);
        }
        pm_Item[2].Text = "●  " + pm_text[2].Trim();

        // 底部信息栏
        panel_Info = new PanelEx()
        {
            Size = new Size(obj.Width - 4, 25),
            Location = new Point(2, obj.Height - 27),
            BackColor = Color.Transparent,
        };
        obj.Controls.Add(panel_Info);
        PanelEx panel_LineInfo = new PanelEx()
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackgroundImage = EzRBuild.EzResource.line,
            BackgroundImageLayout = ImageLayout.Stretch
        };
        panel_Info.Controls.Add(panel_LineInfo);

        // 预览信息
        PictureBoxEx info_Icon = new PictureBoxEx()
        {
            Size = new Size(20, 20),
            Location = new Point(5, 2),
            Image = EzRBuild.EzResource.prview,
            BackColor = Color.Transparent
        };
        panel_Info.Controls.Add(info_Icon);

        pg_Total = new RbControls_TextLabel();
        pg_Total.Text_Label(panel_Info, new Point(75, 5), system_Font,system_previewFontColor, Total_Page + " .");

        pg_Now = new RbControls_TextLabel();
        pg_Now.Text_Label(panel_Info, new Point(165, 5), system_Font,system_previewFontColor, Now_Page + " .");

        string _direct = page_TypeFace.Page_Type + "";
        if (page_TypeFace.Page_Type != -1)
        {
            _direct = page_types[page_TypeFace.Page_Type] + ", " + page_size[page_TypeFace.Page_Type] + " 毫米";
            if (page_TypeFace.Page_Direction == 0) _direct += "，纵向，";
            else _direct += "，横向，";
        }
        else _direct = "自定义纸张";

        new RbControls_TextLabel().Text_Label(panel_Info, new Point(280, 5),system_Font, system_previewFontColor, _direct);

        page_Info = new RbControls_TextLabel();
        page_Info.Text_Label(panel_Info, new Point(30, 5), system_Font,system_FontColor, "总页数：          当前页：                    纸张：");
    }

    // 右键菜单
    private void PageInstall_MouseDown(object sender, MouseEventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        if (e.Button == MouseButtons.Right)
        {
            Preview_Menu.Show(pL.PointToScreen(e.Location));
        }
    }

    // 位置变化，计算当前页码
    private void PageInstall_LocationChanged(object sender, EventArgs e)
    {
        int pheight = (int)((PreViewPage_Area.Height) * zoomScale) + 20; //每页高度
        double _now = (double)(-page_Install.Top) / pheight;
        if (select_MenuNum != 9)
        {
            Now_Page = (int)Math.Floor(_now - 0.8 + 2);
            pg_Now.labelText.Text = Now_Page + " .";
        }
        else
        {
            int _np = (int)Math.Floor(_now - 0.8 + 2);
            _np = _np + (_np - 1);

            if (_np + 1 <= Total_Page) pg_Now.labelText.Text = _np + "," + (_np + 1) + " .";
            else pg_Now.labelText.Text = _np + " .";
            Now_Page = _np;
        }
    }

    /// 改变预览显示比例
    private void PreView_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < Total_Page; i++)
        {
            if (panel_Page[i].HasChildren) panel_Page[i].Controls.Clear();
        }
        setSearchs.Clear();

        ToolStripMenuItem mL = (ToolStripMenuItem)sender;

        select_MenuNum = (int)mL.Tag;

        for (int i = 0; i < 10; i++)
        {
            if (i == (int)mL.Tag) mL.Text = "●  " + pm_text[i].Trim();
            else pm_Item[i].Text = pm_text[i];
        }

        switch ((int)mL.Tag)
        {
            case 0:
                zoomScale = 2;
                break;
            case 1:
                zoomScale = 1.5;
                break;
            case 2:
                zoomScale = 1;
                break;
            case 3:
                zoomScale = 0.75;
                break;
            case 4:
                zoomScale = 0.5;
                break;
            case 5:
                zoomScale = 0.25;
                break;
            case 6:
                zoomScale = 0.1;
                break;
            case 7:
                zoomScale = page_Container.Width / (double)(PreViewPage_Area.Width + 50);
                break;
            case 8:
                zoomScale = page_Container.Height / (double)(PreViewPage_Area.Height + 40);
                break;
            case 9:
                zoomScale = page_Container.Height / (double)(PreViewPage_Area.Height + 40);
                break;
            default:
                break;
        }

        Report_generation(page_TypeFace.Page_Direction);

        if (select_MenuNum != 9) display_Page(0);
        else display_Page(1);
    }

    private void display_Page(int _type)
    {
        page_Container.VerticalScroll.Value = 0;
        page_Container.HorizontalScroll.Value = 0;
        int pwidth = (int)(PreViewPage_Area.Width * zoomScale);
        int pheight = (int)(PreViewPage_Area.Height * zoomScale);

        page_Install.Size = new Size(page_Container.Width - 20, page_Container.Height);
        page_Install.Location = new Point(0, 0);

        if (_type == 0)
        {
            int px = (page_Container.Width - (pwidth + 20)) / 2;
            if (px < 0) px = 0;
            for (int i = 0; i < report_Pages.Count; i++)
            {
                panel_Outer[i].Location = new Point(px, i * (pheight + 20));
                panel_Outer[i].Size = new Size(pwidth + 20, pheight + 20);
                panel_Page[i].Size = new Size(pwidth, pheight);
                panel_Page[i].Location = new Point(10, 10);
                panel_Page[i].BackgroundImage = report_Pages[i].pageImage;
                page_Install.AutoSize = true;
            }
        }

        if (_type == 1)
        {
            int px = (page_Container.Width - (pwidth + 20) * 2) / 2;
            if (px < 0) px = 0;
            int _step = 0;

            for (int i = 0; i < report_Pages.Count; i++)
            {
                if (i % 2 == 0)
                {
                    panel_Outer[i].Location = new Point(px, _step * (pheight + 20));
                    panel_Outer[i].Size = new Size(pwidth + 20, pheight + 20);
                    panel_Page[i].Size = new Size(pwidth, pheight);
                    panel_Page[i].Location = new Point(10, 10);
                    panel_Page[i].BackgroundImage = report_Pages[i].pageImage;
                }
                else
                {
                    panel_Outer[i].Location = new Point(px + pwidth + 10, _step * (pheight + 20));
                    panel_Outer[i].Size = new Size(pwidth + 20, pheight + 20);
                    panel_Page[i].Size = new Size(pwidth, pheight);
                    panel_Page[i].Location = new Point(10, 10);
                    panel_Page[i].BackgroundImage = report_Pages[i].pageImage;
                    page_Install.AutoSize = true;
                    _step += 1;
                }
            }
        }
    }

    /// <summary>
    ///  窗体大小变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PreView_Resize(object sender, EventArgs e)
    {
        page_Container.VerticalScroll.Value = 0;
        page_Container.HorizontalScroll.Value = 0;

        page_View.Size = new Size(preview_Form._formObject.Width - 4, preview_Form._formObject.Height - 125);
        if (report_Pages.Count > 0)
        {
            if (select_MenuNum == 7)
            {
                zoomScale = page_Container.Width / (double)(PreViewPage_Area.Width + 50);
                Report_generation(page_TypeFace.Page_Direction);
            }
            if (select_MenuNum == 8)
            {
                zoomScale = page_Container.Height / (double)(PreViewPage_Area.Height + 35);
                Report_generation(page_TypeFace.Page_Direction);
            }
            if (select_MenuNum == 9)
            {
                zoomScale = page_Container.Height / (double)(PreViewPage_Area.Height + 40);
                Report_generation(page_TypeFace.Page_Direction);
            }

            if (select_MenuNum != 9) display_Page(0);
            else display_Page(1);
        }

        panel_Info.Size = new Size(preview_Form._formObject.Width - 4, 25);
        panel_Info.Location = new Point(2, preview_Form._formObject.Height - 27);

        preview_Form.LocationEX = preview_Form._formObject.Location;
        preview_Form.SizeEX = preview_Form._formObject.Size;
    }
}
