using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using static RbControls;
using static RbControls_CustomMouse;
using static Define_Design;
using static Define_System;
using static Define_Draggable;

public static class RBuild_Menu
{
    public static int total_Menu = 4;
    public static string[] menu_text = new string[] { "文件", "编辑", "设计", "帮助" };
    public static Point[] menu_point = new Point[] { new Point(3, 37), new Point(53, 37), new Point(103, 37), new Point(153, 37), };
    public static int[] menu_width = new int[] { 43, 43, 43, 43 };

    public static ContextMenuStrip control_Menu;
    public static ContextMenuStrip band_Menu;

    public static void Initialize_MenuStrip()
    {
        // 栏目右键菜单
        band_Menu = new ContextMenuStrip()
        {
            ImageScalingSize = new Size(20, 20),
            Font = system_Font
        };
        ToolStripSeparator band_Separator = new ToolStripSeparator { AutoSize = true };
        ToolStripMenuItem[] menuband_Item = new ToolStripMenuItem[3];
        Bitmap[] menuband_img = new Bitmap[3] {
            EzRBuild.EzResource.set_band,
            EzRBuild.EzResource.past,
            EzRBuild.EzResource.prview
        };
        string[] menuband_str = new string[3] { "栏目设置", "粘贴", "打印预览" };
        for (int i = 0; i < 3; i++)
        {
            menuband_Item[i] = new ToolStripMenuItem()
            {
                AutoSize = true,
                Image = menuband_img[i],
                Text = menuband_str[i],
            };
            if (i == 1) menuband_Item[i].ShortcutKeys = Keys.Alt | Keys.V;
            if (i == 2) menuband_Item[i].ShortcutKeys = Keys.Alt | Keys.P;

            menuband_Item[i].MouseDown += BandMenu_MouseDown;

            band_Menu.Items.Add(menuband_Item[i]);
            if (i == 0) band_Menu.Items.Add(band_Separator);
        }

        // 组件右键菜单
        control_Menu = new ContextMenuStrip()
        {
            ImageScalingSize = new Size(20, 20),
            Font = system_Font
        };
        ToolStripSeparator[] menu_Separator = new ToolStripSeparator[4];/////3
        for (int i = 0; i < 4; i++) menu_Separator[i] = new ToolStripSeparator() { AutoSize = true }; ////3

        ToolStripMenuItem[] menu_Item = new ToolStripMenuItem[9];
        ToolStripMenuItem[] menu_subItem = new ToolStripMenuItem[9];

        Bitmap[] menu_img = new Bitmap[9] {
            EzRBuild.EzResource.cut,
            EzRBuild.EzResource.copy,
            EzRBuild.EzResource.past,
            EzRBuild.EzResource.del,
            EzRBuild.EzResource.cancel,
            EzRBuild.EzResource.redo,
            EzRBuild.EzResource.pgalign,
            EzRBuild.EzResource.topmose,
            EzRBuild.EzResource.botmost
        };
        Bitmap[] menu_subimg = new Bitmap[9] {
            EzRBuild.EzResource.setleft,
            EzRBuild.EzResource.setright,
            EzRBuild.EzResource.settop,
            EzRBuild.EzResource.setbot,
            EzRBuild.EzResource.a_left,
            EzRBuild.EzResource.a_center,
            EzRBuild.EzResource.a_right,
            EzRBuild.EzResource.a_top,
            EzRBuild.EzResource.a_bot
        };

        string[] menu_str = new string[9] { "剪切", "复制", "粘贴", "删除", "撤销", "重做", "页面对齐方式", "放置上层", "放置下层" };
        string[] menu_substr = new string[9] { "居左", "居右", "居顶", "居底", "左对齐", "垂直对齐", "右对齐", "顶对齐", "底对齐" };

        Keys[] menu_keys = new Keys[9]{
            Keys.Alt | Keys.X,
            Keys.Alt | Keys.C,
            Keys.Alt | Keys.V,
            Keys.Alt | Keys.Delete,
            Keys.Alt | Keys.Z,
            Keys.Alt | Keys.R,
            Keys.Control | Keys.Shift | Keys.D0,
            Keys.Alt | Keys.T,
            Keys.Alt | Keys.B
        };

        for (int i = 0; i < 9; i++)
        {
            menu_Item[i] = new ToolStripMenuItem()
            {
                AutoSize = true,
                Image = menu_img[i],
                Text = menu_str[i],
            };
            menu_Item[i].Click += ControlMenu_Click;
            if (i != 6) menu_Item[i].ShortcutKeys = menu_keys[i];
            if (i == 6)
            {
                for (int t = 0; t < 9; t++)
                {
                    menu_subItem[t] = new ToolStripMenuItem() { AutoSize = true, Text = menu_substr[t], Image = menu_subimg[t] };
                    menu_subItem[t].Click += ControlMenu_Click;
                }
                menu_Item[i].DropDownItems.AddRange(menu_subItem);
                menu_Item[i].DropDownItems.Insert(4, menu_Separator[3]);

            }

            control_Menu.Items.Add(menu_Item[i]);
            if (i == 3) control_Menu.Items.Add(menu_Separator[0]);
            if (i == 5) control_Menu.Items.Add(menu_Separator[1]);
            if (i == 6) control_Menu.Items.Add(menu_Separator[2]);
        }
    }

    /// <summary>
    /// 工具栏添加组件图标
    /// </summary>
    /// <param name="obj">在那个控件内</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="img">图像</param>
    /// <param name="location">位置</param>
    /// <param name="tips">提示文字</param>
    /// <param name="id">标识</param>
    /// <param name="showtips">是否显示提示</param>
    public static void addToolControl(Control obj, int width, int height, Bitmap img, Point location, string tips, string id, bool showtips)
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
            toolControl.Click += new EventHandler(toolControl_Click);

        }
        obj.Controls.Add(toolControl);
    }

    /// <summary>
    /// 添加图标到工具栏
    /// </summary>
    /// <param name="panelID">序号</param>
    /// <param name="obj">在哪个控件内</param>
    public static void tool_Control(int panelID, Control obj)
    {
        switch (panelID)
        {
            case 0:
                addToolControl(obj, 5, 25,  EzRBuild.EzResource.spear, new Point(10, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.newfile, new Point(20, 5), "新建报表文件", "FILE_NWE", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.open, new Point(50, 5), "打开报表文件", "FILE_OPEN", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.save, new Point(80, 5), "保存报表文件", "FILE_SAVE", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(110, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.ptype, new Point(120, 5), "纸张设置", "FILE_SETPAGE", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.datalink, new Point(150, 5), "数据库设置", "FILE_SETDATABASE", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(180, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.close, new Point(190, 5), "退出", "SYSTEM_CLOSE", true);
                break;
            case 1:
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(10, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.cut, new Point(20, 5), "剪切 (Alt+X)", "EDIT_CUT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.copy, new Point(50, 5), "复制 (Alt+C)", "EDIT_COPY", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.past, new Point(80, 5), "粘贴 (Alt+V)", "EDIT_PAST", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.del, new Point(110, 5), "删除组件 (Alt+Delete)", "EDIT_DELETE", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(140, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.delall, new Point(150, 5), "删除全部", "EDIT_DELALL", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.cancel, new Point(180, 5), "撤销 (Alt+Z)", "EDIT_CANCEL", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.redo, new Point(210, 5), "重做 (Alt+R)", "EDIT_REDO", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(240, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.prview, new Point(250, 5), "打印预览(Alt+P)", "EDIT_PREVIEW", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.print, new Point(280, 5), "报表打印", "EDIT_PRINT", true);
                break;
            case 2:
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(10, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.mouse, new Point(20, 5), "取消选择", "DESIGN_PONINTER", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.showline, new Point(50, 5), "显示标线", "DESIGN_SHOWLINE", true);
                addToolControl(obj, 35, 25, EzRBuild.EzResource.menu_band, new Point(80, 5), "栏目设置", "DESIGN_BAND", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(120, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.txtbox, new Point(130, 5), "文本框", "DESIGN_TEXTBOX", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.img, new Point(160, 5), "图像", "DESIGN_IMGBOX", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.shape, new Point(190, 5), "形状", "DESIGN_SHAPEBOX", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.field, new Point(220, 5), "数据库字段", "DESIGN_DATAFIELD", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.pagecode, new Point(250, 5), "页码", "DESIGN_PAGECODE", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.func, new Point(280, 5), "数据库运算", "DESIGN_FUNCTION", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(310, 5), null, null, false);
                addToolControl(obj, 35, 25, EzRBuild.EzResource.setcolor, new Point(320, 5), "设置颜色", "DESIGN_SETCOLOR", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.topmose, new Point(360, 5), "组件放置最上层 (Ctrl+T)", "DESIGN_TOPMOSE", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.botmost, new Point(390, 5), "组件放置最下层 (Ctrl+B)", "DESIGN_BOTMOST", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(420, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.setleft, new Point(430, 5), "居左", "DESIGN_SETLEFT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.setright, new Point(460, 5), "居右", "DESIGN_SETRIGHT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.settop, new Point(490, 5), "居顶", "DESIGN_SETTOP", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.setbot, new Point(520, 5), "居底", "DESIGN_SETBOT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.a_left, new Point(550, 5), "左对齐", "DESIGN_ALEFT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.a_center, new Point(580, 5), "垂直对齐", "DESIGN_ACENTER", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.a_right, new Point(610, 5), "右对齐", "DESIGN_ARIGHT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.a_top, new Point(640, 5), "顶对齐", "DESIGN_ATOP", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.a_bot, new Point(670, 5), "底对齐", "DESIGN_ABOTTOM", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(700, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.table_1, new Point(710, 5), "下框线", "DESIGN_LINEBOT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.table_2, new Point(740, 5), "上框线", "DESIGN_LINETOP", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.table_3, new Point(790, 5), "左框线", "DESIGN_LINELEFT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.table_4, new Point(800, 5), "右框线", "DESIGN_LINERIGHT", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.table_5, new Point(830, 5), "无线框", "DESIGN_LINENO", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.table_6, new Point(860, 5), "外侧框线", "DESIGN_LINEEXTER", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.table_7, new Point(890, 5), "斜下框线", "DESIGN_LINEBIASDOWN", true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.table_8, new Point(920, 5), "斜上框线", "DESIGN_LINE_BIASUP", true);
                addToolControl(obj, 35, 25, EzRBuild.EzResource.thick, new Point(950, 5), "框线类型", "DESIGN_LINETHICK", true);
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(990, 5), null, null, false);
                break;
            case 3:
                addToolControl(obj, 5, 25, EzRBuild.EzResource.spear, new Point(10, 5), null, null, false);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.help, new Point(20, 5), "使用帮助", null, true);
                addToolControl(obj, 25, 25, EzRBuild.EzResource.icon, new Point(50, 5), "关于系统", null, true);
                break;
            default:
                break;
        }
    }

    /// 点击工具栏图标
    private static void toolControl_Click(object sender, EventArgs e)
    {
        PictureBoxEx pL = (PictureBoxEx)sender;
        
        switch (pL.Tag)
        {
            case "FILE_NWE":
                RBuild_File.New_File();
                break;
            case "FILE_OPEN":
                RBuild_File.Open_File();
                break;
            case "FILE_SAVE":
                RBuild_File.Save_File();
                break;
            case "FILE_SETPAGE":
                DialogUse_HotKey = true;
                new RBuild_SetPageType().Set_Type();
                break;
            case "FILE_SETDATABASE":
                DialogUse_HotKey = true;
                new RBuild_SetDataBase().Set_Data();
                break;
            case "SYSTEM_CLOSE":
                RBuild_Design.designForm_Close(null, null);
                break;
                
            case "EDIT_CUT":
                Object_OperationCopy();
                Delete_Control(true);
                break;
            case "EDIT_COPY":
                Object_OperationCopy();
                break;
            case "EDIT_PAST":
                Object_OperationPast(0, 21);
                break;
            case "EDIT_DELETE":
                Delete_Control(true);
                break;
            case "EDIT_CANCEL":
                Object_Operation(0);
                break;
            case "EDIT_REDO":
                Object_Operation(1);
                break;
            case "EDIT_DELALL":
                Object_Record();
                for (int i = 0; i < DraggableObjects.Count; i++) Delete_Control(false);
                break;
            case "EDIT_PREVIEW":
                new RBuild_Preview().View();
                break;
            case "EDIT_PRINT":
                break;
            
            case "DESIGN_PONINTER":
                RBuild_Design.design_Form._formObject.Cursor = Cursors.Default;
                control_Type = -1;
                break;
            case "DESIGN_SHOWLINE":
                Show_Line = !Show_Line;
                page_Install.Invalidate();
                page_Container.Invalidate();
                Print_PageType.Invalidate();
                break;
            case "DESIGN_BAND":
                DialogUse_HotKey = true;
                new RBuild_SetBandRect().Set_Band(0);
                break;
            case "DESIGN_TEXTBOX":
                pL.Cursor =  custom_MouseCursor(control_icon[0], 5, 5);
                control_Type = 1;
                break;
            case "DESIGN_IMGBOX":
                pL.Cursor =  custom_MouseCursor(control_icon[1], 5, 5);
                control_Type = 2;
                break;
            case "DESIGN_SHAPEBOX":
                pL.Cursor =  custom_MouseCursor(control_icon[2], 5, 5);
                control_Type = 3;
                break;
            case "DESIGN_DATAFIELD":
                pL.Cursor =  custom_MouseCursor(control_icon[3], 5, 5);
                control_Type = 4;
                break;
            case "DESIGN_PAGECODE":
                pL.Cursor =  custom_MouseCursor(control_icon[4], 5, 5);
                control_Type = 5;
                break;
            case "DESIGN_FUNCTION":
                pL.Cursor =  custom_MouseCursor(control_icon[5], 5, 5);
                control_Type = 6;
                break;
            case "DESIGN_SETCOLOR":
                DialogUse_HotKey = true;
                new RBuild_SetColor().Set_Colors();
                break;
            case "DESIGN_TOPMOSE":
                ListSwap_Top();
                break;
            case "DESIGN_BOTMOST":
                ListSwap_Bottom();
                break;
            case "DESIGN_SETLEFT":
                Set_BoxPoint(0);
                break;
            case "DESIGN_SETRIGHT":
                Set_BoxPoint(1);
                break;
            case "DESIGN_SETTOP":
                Set_BoxPoint(2);
                break;
            case "DESIGN_SETBOT":
                Set_BoxPoint(3);
                break;
            case "DESIGN_ALEFT":
                Set_FillAlign(0);
                break;
            case "DESIGN_ACENTER":
                Set_FillAlign(1);
                break;
            case "DESIGN_ARIGHT":
                Set_FillAlign(2);
                break;
            case "DESIGN_ATOP":
                Set_FillAlign(3);
                break;
            case "DESIGN_ABOTTOM":
                Set_FillAlign(4);
                break;
            case "DESIGN_LINEBOT":
                Set_LineBorder(0);
                break;
            case "DESIGN_LINETOP":
                Set_LineBorder(1);
                break;
            case "DESIGN_LINELEFT":
                Set_LineBorder(2);
                break;
            case "DESIGN_LINERIGHT":
                Set_LineBorder(3);
                break;
            case "DESIGN_LINENO":
                Set_LineBorder(4);
                break;
            case "DESIGN_LINEEXTER":
                Set_LineBorder(5);
                break;
            case "DESIGN_LINEBIASDOWN":
                Set_LineBorder(6);
                break;
            case "DESIGN_LINE_BIASUP":
                Set_LineBorder(7);
                break;
            case "DESIGN_LINETHICK":
                DialogUse_HotKey = true;
                new RBuild_SetLineThick().Set_Thick();
                break;

            default:
                break;
        }
        
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

    /// 鼠标移动菜单，切换工具栏
    public static void ToolMenuMove(object sender, MouseEventArgs e)
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

    /// <summary>
    /// 设置右键菜单显示项
    /// </summary>
    /// <param name="obj">在哪个控件内</param>
    /// <param name="_num">当前对象序号</param>
    public static void Set_ControlMenuItem(Control obj, int _num)
    {
        var _controls = DraggableObjects.Where(band => band.Belong_Band == band_Num).ToList();
        if (_controls.Count < 2)
        {
            control_Menu.Items[10].Enabled = false;
            control_Menu.Items[11].Enabled = false;
        }
        else
        {
            control_Menu.Items[10].Enabled = true;
            control_Menu.Items[11].Enabled = true;
        }

        if (DraggableObjects[_num].Id == _controls[0].Id) control_Menu.Items[11].Enabled = false;
        else control_Menu.Items[11].Enabled = true;
        if (DraggableObjects[_num].Id == _controls[_controls.Count - 1].Id) control_Menu.Items[10].Enabled = false;
        else control_Menu.Items[10].Enabled = true;

        if (operationObject.Id != null) control_Menu.Items[2].Enabled = true;
        else control_Menu.Items[2].Enabled = false;
    }

    public static void Set_BandMenuItem()
    {
        if (operationObject.Id != null) band_Menu.Items[2].Enabled = true;
        else band_Menu.Items[2].Enabled = false;
    }

    private static void ControlMenu_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem mL = (ToolStripMenuItem)sender;

        switch (mL.Text)
        {
            case "剪切":
                Object_OperationCopy();
                Delete_Control(true);
                break;
            case "复制":
                Object_OperationCopy();
                break;
            case "粘贴":
                Object_OperationPast(0, 21);
                break;
            case "删除":
                Delete_Control(true);
                break;
            case "撤销":
                Object_Operation(0);
                break;
            case "重做":
                Object_Operation(1);
                break;
            case "居左":
                Set_BoxPoint(0);
                break;
            case "居右":
                Set_BoxPoint(1);
                break;
            case "居顶":
                Set_BoxPoint(2);
                break;
            case "居底":
                Set_BoxPoint(3);
                break;
            case "左对齐":
                Set_FillAlign(0);
                break;
            case "垂直对齐":
                Set_FillAlign(1);
                break;
            case "右对齐":
                Set_FillAlign(2);
                break;
            case "顶对齐":
                Set_FillAlign(3);
                break;
            case "底对齐":
                Set_FillAlign(4);
                break;
            case "放置上层":
                ListSwap_Top();
                break;
            case "放置下层":
                ListSwap_Bottom();
                break;
            default:
                break;
        }
    }

    private static void BandMenu_MouseDown(object sender, MouseEventArgs e)
    {
        ToolStripMenuItem mL = (ToolStripMenuItem)sender;
        switch (mL.Text)
        {
            case "栏目设置":
                DialogUse_HotKey = true;
                new RBuild_SetBandRect().Set_Band(1);
                break;
            case "粘贴":
                Object_OperationPast(0, 21);
                break;
            case "打印预览":
                new RBuild_Preview().View();
                break;
            default:
                break;
        }
    }
}
