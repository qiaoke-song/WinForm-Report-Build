using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using static RbControls;
using static Define_System;
using static Define_PrintPage;
using static Define_DrawObject;
using static Define_Design;
using static Define_Draggable;
using static Define_DataLink;

public static class RBuild_Info
{
    public static PanelEx panel_State;
    public static RbControls_TextLabel Composite_Info; // 综合信息

    private static RbControls_SpearLine pic_ControlType; // 显示组件图标
    private static RbControls_TextLabel Label_ControlType, Label_ControlSize, Label_ControlPoint; // 显示组件类型、大小、位置

    private static PanelEx[] panel_setControl = new PanelEx[6]; // 组件设置框
    private static RbControls_TextBox[,] attribute_Input = new RbControls_TextBox[6, 4]; // 属性输入框
    private static RbControls_ColorPad[] Content_Color = new RbControls_ColorPad[6]; // 颜色设置
    private static RbControls_TextLabel[] color_Flag = new RbControls_TextLabel[6];
    private static RbControls_TextLabel[] font_type = new RbControls_TextLabel[6];
    private static RbControls_TextLabel[] font_size = new RbControls_TextLabel[6];
    private static RbControls_CheckBox[,] font_render = new RbControls_CheckBox[6, 3];
    private static RbControls_CheckBox[,] align_Check = new RbControls_CheckBox[6, 6];

    private static RbControls_TextBox content_TextInput;
    private static RbControls_TextBox content_ImgInput;
    private static RbControls_TextBox content_DataInput;
    private static RbControls_TextBox content_PcodeInput;
    public static RbControls_TextBox content_FunctionInput;
    private static RbControls_CheckBox[] ImgZoom_CheckBox = new RbControls_CheckBox[2];

    private static RbControls_DrawTextMethod ds = new RbControls_DrawTextMethod();
    private static Label[] shaps_pic = new Label[20];
    private static RbControls_CheckBox[] shap_select = new RbControls_CheckBox[20];

    private static ContextMenuStrip pgCode_Menu;
    private static string[] pgCode_Value = new string[8] { "[x]", "(x)", "-x-", "x", "-x", "x-xx", "Page x of xx", "第x页" };
    private static ContextMenuStrip DataField_Menu;

    private static FontFamily[] FontFamilies;
    private static ContextMenuStrip Font_Menu; // 字体菜单
    private static ContextMenuStrip FontSize_Menu; // 字体大小菜单

    public static void Initialize_Info(Control obj)
    {
        PanelEx panel_TopLine = new PanelEx() // 间隔线
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackgroundImage = EzRBuild.EzResource.line,
            BackgroundImageLayout = ImageLayout.Stretch
        };

        panel_State = new PanelEx() // 底部显示组件信息框
        {
            Size = new Size(obj.Width - 4, 62),
            Location = new Point(2, obj.Height - 64),
            Cursor = Cursors.Default,
            BackColor = system_backColor
        };
        obj.Controls.Add(panel_State);

        pic_ControlType = new RbControls_SpearLine();
        pic_ControlType.Spear_line(panel_State, new Size(25, 25), new Point(2, 5), EzRBuild.EzResource.mouse);

        panel_State.Controls.Add(panel_TopLine);

        Label_ControlType = new RbControls_TextLabel();
        Label_ControlType.Text_Label(panel_State, new Point(27, 6), system_Font,system_FontColor, "");
        Label_ControlSize = new RbControls_TextLabel();
        Label_ControlSize.Text_Label(panel_State, new Point(7, 28), system_Font, system_FontColor, "范围：");
        Label_ControlPoint = new RbControls_TextLabel();
        Label_ControlPoint.Text_Label(panel_State, new Point(7, 44), system_Font, system_FontColor, "位置：");

        new RbControls_SpearLine().Spear_line(panel_State, new Size(5, 50), new Point(160, 8), EzRBuild.EzResource.spearInfo);

        panel_State.Controls.Add(panel_TopLine);

        // 获取字体
        InstalledFontCollection Fonts = new InstalledFontCollection();
        FontFamilies = Fonts.Families;
        Font_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };
        ToolStripMenuItem[] menu_Item = new ToolStripMenuItem[FontFamilies.Length];
        for (int i = FontFamilies.Length - 1; i > 0; i--)
        {
            menu_Item[i] = new ToolStripMenuItem()
            {
                AutoSize = true,
                Font = system_Font,
                Text = FontFamilies[i].Name,
            };
            menu_Item[i].Click += fontMenu_Click;
            Font_Menu.Items.Add(menu_Item[i]);
        }

        // 字体大小
        string[] size_value = new string[19] { "5", "6", "7", "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" };
        FontSize_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };
        ToolStripMenuItem[] menu_ItemSize = new ToolStripMenuItem[19];
        for (int i = 0; i < 19; i++)
        {
            menu_ItemSize[i] = new ToolStripMenuItem()
            {
                AutoSize = true,
                Font = system_Font,
                Text = size_value[i],
            };
            menu_ItemSize[i].Click += sizeMenu_Click;
            FontSize_Menu.Items.Add(menu_ItemSize[i]);
        }


        for (int i = 0; i < 6; i++)
        {
            panel_setControl[i] = new PanelEx() // 设置框
            {
                Size = new Size(1018, 50),
                Location = new Point(175, 8),
                Visible = false,
                BackColor = system_backColor
            };

            RbControls_TextLabel[] acceptLabels = new RbControls_TextLabel[2];
            string[] acceptMenu = new string[2] { "[✔ 应用设置 ]", "[✘ 删除组件 ]" };

            int[] _px = new int[2] { 180, 635 };
            for (int t = 0; t < 2; t++)
            {
                if (t == 0) new RbControls_ButtonLabel().Button_Label(panel_setControl[i], new Point(1 + t * 85, 4), system_buttonColor, Color.Salmon, system_buttonColor, acceptMenu[t], system_Font,t, accept_Attributes);
                else new RbControls_ButtonLabel().Button_Label(panel_setControl[i], new Point(1 + t * 85, 4), system_buttonColor, Color.Salmon, system_buttonColor, acceptMenu[t], system_Font,t, null);
                new RbControls_SpearLine().Spear_line(panel_setControl[i], new Size(5, 25), new Point(_px[t], 0), EzRBuild.EzResource.spear);
            }

            // 属性输入框
            int[] _inputX = new int[4] { 230, 335, 450, 565 };
            for (int t = 0; t < 4; t++)
            {
                attribute_Input[i, t] = new RbControls_TextBox();
                attribute_Input[i, t].input_Box(panel_setControl[i], 60, new Point(_inputX[t], 4), system_Font,Color.Salmon, system_backColor, "", 4, false, input_Number, null, null);
            }
            string label_txt = "宽度：                 高度：                 位置(x)：                位置(y)：                    颜色：";
            // 设置颜色
            if (i != 1)
            {
                new RbControls_PanelLine().Panel_Line(panel_setControl[i], new Size(30, 1), new Point(685, 20));
                Content_Color[i] = new RbControls_ColorPad();
                Content_Color[i].colorPad(panel_setControl[i], new Point(755, 7), new Size(11, 11), 15, Color.Transparent, colorSelect_Click);
                color_Flag[i] = new RbControls_TextLabel();
                color_Flag[i].Text_Label(panel_setControl[i], new Point(690, 3), system_Font,Color.Black, "█▌");
            }
            else label_txt = "宽度：                 高度：                 位置(x)：                位置(y)：";
            new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(195, 4), system_Font,system_FontColor, label_txt);

            // 对齐方式
            if (i != 2)
            {
                string[] align_label = new string[] { "左", "右", "顶", "底", "居中", "垂直" };
                int[] align_location = new int[] { 48, 83, 118, 153, 188, 233 };
                for (int t = 0; t < 6; t++)
                {
                    align_Check[i, t] = new RbControls_CheckBox();
                    if ((t == 0) || (t == 2)) align_Check[i, t].check_Box(panel_setControl[i], true, t, new Point(align_location[t], 30), system_FontColor, system_Font,align_label[t], check_AlwaysTrue, align_Click);
                    else align_Check[i, t].check_Box(panel_setControl[i], false, t, new Point(align_location[t], 30), system_FontColor, system_Font,align_label[t], check_AlwaysTrue, align_Click);
                }
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(1, 31), system_Font,system_FontColor, "[ 对齐：");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(281, 31), system_Font,system_FontColor, "]");
            }

            // 设置字体
            if ((i != 1) && (i != 2))
            {
                font_type[i] = new RbControls_TextLabel();
                font_type[i].Text_Label(panel_setControl[i], new Point(340, 31), system_Font,Color.Salmon, "微软雅黑");
                new RbControls_PanelLine().Panel_Line(panel_setControl[i], new Size(128, 1), new Point(345, 48));

                font_size[i] = new RbControls_TextLabel();
                font_size[i].Text_Label(panel_setControl[i], new Point(545, 31), system_Font,Color.Salmon, "9");
                new RbControls_PanelLine().Panel_Line(panel_setControl[i], new Size(37, 1), new Point(540, 48));

                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(300, 31), system_Font,system_FontColor, "[ 字体：");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(487, 31), system_Font, system_FontColor, "] [ 字号：");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(590, 31), system_Font, system_FontColor, "]");
                new RbControls_ButtonLabel().Button_Label(panel_setControl[i], new Point(475, 31), Color.Salmon, Color.Salmon, Color.Salmon, "◥",system_Font, 0, fontLabe_MouseDown);
                new RbControls_ButtonLabel().Button_Label(panel_setControl[i], new Point(575, 31), Color.Salmon, Color.Salmon, Color.Salmon, "◥",system_Font, 0, sizeLabe_MouseDown);

                string[] render_menu = new string[] { "加粗", "倾斜", "下划线" };
                int[] render_location = new int[] { 625, 675, 725 };
                for (int t = 0; t < 3; t++)
                {
                    font_render[i, t] = new RbControls_CheckBox();
                    font_render[i, t].check_Box(panel_setControl[i], false, t, new Point(render_location[t], 30), system_FontColor, system_Font,render_menu[t], check_Normal, null);
                }
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(610, 31), system_Font, system_FontColor, "[");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(785, 31), system_Font, system_FontColor, "]");
            }
            panel_State.Controls.Add(panel_setControl[i]);

            if (i == 0) // 文本框
            {
                content_TextInput = new RbControls_TextBox();
                content_TextInput.input_Box(panel_setControl[i], 168, new Point(830, 31), system_Font,Color.Salmon, system_backColor, "", 4096, false, input_Normal, null, null);
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(795, 31), system_Font, system_FontColor, "内容：");
                new RbControls_ButtonLabel().Button_Label(panel_setControl[i], new Point(1000, 33), Color.Salmon, Color.Salmon, Color.Salmon, "×", system_Font,0, TextClear_Click);
            }

            if (i == 1) // 图像
            {
                content_ImgInput = new RbControls_TextBox();
                content_ImgInput.input_Box(panel_setControl[i], 150, new Point(520, 31), system_Font,Color.Salmon, system_backColor, "", 4096, false, 0, null, null);
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(450, 31), system_Font,system_FontColor, "图像文件：");

                string[] Img_Zoom = new string[2] { "原始大小", "缩放" };
                for (int t = 0; t < 2; t++)
                {
                    ImgZoom_CheckBox[t] = new RbControls_CheckBox();
                    if (t == 0) ImgZoom_CheckBox[t].check_Box(panel_setControl[i], false, t, new Point(315 + t * 75, 30), system_FontColor, system_Font,Img_Zoom[t], check_AlwaysTrue, ImgZoom_Click);
                    else ImgZoom_CheckBox[t].check_Box(panel_setControl[i], true, t, new Point(315 + t * 75, 30), system_FontColor, system_Font,Img_Zoom[t], check_AlwaysTrue, ImgZoom_Click);
                }
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(300, 31), system_Font, system_FontColor, "[");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(435, 31), system_Font, system_FontColor, "]");

                string[] ImgZoom_text = new string[2] { "◥", "×" };
                RbControls_ButtonLabel[] ImgZoom_Button = new RbControls_ButtonLabel[2];
                for (int t = 0; t < 2; t++)
                {
                    ImgZoom_Button[t] = new RbControls_ButtonLabel();
                    if (t == 0) ImgZoom_Button[t].Button_Label(panel_setControl[i], new Point(673 + t * 18, 33), Color.Salmon, Color.Salmon, Color.Salmon, ImgZoom_text[t], system_Font,t, openImg_Click);
                    else ImgZoom_Button[t].Button_Label(panel_setControl[i], new Point(673 + t * 18, 33), Color.Salmon, Color.Salmon, Color.Salmon, ImgZoom_text[t], system_Font,t, openClear_Click);
                }
            }

            if (i == 2) // 形状
            {
                Bitmap[] shaps_bmp = new Bitmap[20];
                for (int t = 0; t < 20; t++)
                {
                    shap_select[t] = new RbControls_CheckBox();
                    shap_select[t].check_Box(panel_setControl[i], false, t, new Point(48 + t * 40, 30), system_backColor, system_Font,null, check_AlwaysTrue, ShapSelect_Click);

                    shaps_bmp[t] = new Bitmap(20, 20);
                    ds.DrawFontAwesome(shaps_bmp[t], shaps_type[t], 20, Color.FromArgb(150, 150, 150), new Point(0, 0), false);

                    shaps_pic[t] = new Label()
                    {
                        Size = new Size(20, 20),
                        Location = new Point(65 + t * 40, 31),
                        Image = shaps_bmp[t],
                        Cursor = Cursors.Hand,
                        BackColor = Color.Transparent,
                        Tag = t
                    };

                    shaps_pic[t].Click += ShapSelect_Click;
                    panel_setControl[i].Controls.Add(shaps_pic[t]);
                }
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(1, 31), system_Font,system_FontColor, "[ 样式：");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(845, 31), system_Font, system_FontColor, "]");
            }

            if (i == 3) // 数据库字段
            {
                content_DataInput = new RbControls_TextBox();
                content_DataInput.input_Box(panel_setControl[i], 150, new Point(840, 31), system_Font,Color.Salmon, system_backColor, "", 4096, true, input_Normal, null, null);
                new RbControls_ButtonLabel().Button_Label(panel_setControl[i], new Point(990, 33), Color.Salmon, Color.Salmon, Color.Salmon, "◥", system_Font,0, FieldSelect_MouseDown);
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(795, 31), system_Font, system_FontColor, "[ 字段：");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(1005, 31), system_Font, system_FontColor, "]");

                DataField_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };
            }

            if (i == 4) // 页码
            {
                content_PcodeInput = new RbControls_TextBox();
                content_PcodeInput.input_Box(panel_setControl[i], 150, new Point(840, 31), system_Font,Color.Salmon, system_backColor, "", 64, true, input_Normal, null, null);
                new RbControls_ButtonLabel().Button_Label(panel_setControl[i], new Point(990, 33), Color.Salmon, Color.Salmon, Color.Salmon, "◥", system_Font,0, pgCode_Click);
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(795, 31), system_Font, system_FontColor, "[ 样式：");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(1005, 31), system_Font, system_FontColor, "]");

                string[] pgCode_MenuText = new string[8] { "[1] , [2] , [3] , ...", "(1) , (2) , (3) , ...", "-1- , -2- , -3- , ...", "1 , 2 , 3 , ...", "-1 , -2 , -3 , ...", "1-99 , 2-99 , ...", "Page 1 of 99 , ...", "第1页 , 第2页 ..." };
                pgCode_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };
                ToolStripMenuItem[] menu_ItemCode = new ToolStripMenuItem[8];
                for (int t = 0; t < 8; t++)
                {
                    menu_ItemCode[t] = new ToolStripMenuItem()
                    {
                        AutoSize = true,
                        Font = system_Font,
                        Text = pgCode_MenuText[t],
                        Tag = t
                    };
                    menu_ItemCode[t].Click += pgMenuCode_Click;
                    pgCode_Menu.Items.Add(menu_ItemCode[t]);
                }
            }

            if (i == 5) // 数据库运算
            {
                content_FunctionInput = new RbControls_TextBox();
                content_FunctionInput.input_Box(panel_setControl[i], 150, new Point(840, 31), system_Font,Color.Salmon, system_backColor, "", 4096, true, input_Normal, null, null);
                new RbControls_ButtonLabel().Button_Label(panel_setControl[i], new Point(990, 33), Color.Salmon, Color.Salmon, Color.Salmon, "◥", system_Font,0, FieldFunction_MouseDown);
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(795, 31), system_Font, system_FontColor, "[ 运算：");
                new RbControls_TextLabel().Text_Label(panel_setControl[i], new Point(1005, 31), system_Font, system_FontColor, "]");

                DataField_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };
            }

        }

        Task View_ControlInfo = new Task(Set_ControlMoveInfo);
        View_ControlInfo.Start();
    }

    // 设置颜色
    private static void colorSelect_Click(object sender, EventArgs e)
    {
        PanelEx pL = (PanelEx)sender;
        color_Flag[DraggableObjects[control_Num].ControlType - 1].labelText.ForeColor = colors[(int)pL.Tag];
    }

    // 字体选项
    private static void fontLabe_MouseDown(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;
        Font_Menu.Show(pL.PointToScreen(e.Location));
    }

    // 字体大小选项
    private static void sizeLabe_MouseDown(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;
        FontSize_Menu.Show(pL.PointToScreen(e.Location));
    }

    // 设置字体
    private static void fontMenu_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem tL = (ToolStripMenuItem)sender;
        font_type[DraggableObjects[control_Num].ControlType - 1].labelText.Text = tL.Text;
    }

    // 设置字体大小
    private static void sizeMenu_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem tL = (ToolStripMenuItem)sender;
        font_size[DraggableObjects[control_Num].ControlType - 1].labelText.Text = tL.Text;
    }

    // 对齐方式点击
    private static void align_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        int _num = DraggableObjects[control_Num].ControlType - 1;
        if ((int)pL.Tag == 0)
        {
            align_Check[_num, 1].set_Flag(false);
            align_Check[_num, 4].set_Flag(false);
        }
        else
        if ((int)pL.Tag == 1)
        {
            align_Check[_num, 0].set_Flag(false);
            align_Check[_num, 4].set_Flag(false);
        }
        else
        if ((int)pL.Tag == 2)
        {
            align_Check[_num, 3].set_Flag(false);
            align_Check[_num, 5].set_Flag(false);
        }
        else
        if ((int)pL.Tag == 3)
        {
            align_Check[_num, 2].set_Flag(false);
            align_Check[_num, 5].set_Flag(false);
        }
        else
        if ((int)pL.Tag == 4)
        {
            align_Check[_num, 0].set_Flag(false);
            align_Check[_num, 1].set_Flag(false);
        }
        else
        if ((int)pL.Tag == 5)
        {
            align_Check[_num, 2].set_Flag(false);
            align_Check[_num, 3].set_Flag(false);
        }
    }

    // 文本框清除文字
    private static void TextClear_Click(object sender, EventArgs e)
    {
        content_TextInput.textBox.Text = "";
    }

    // 打开图像文件
    private static void openImg_Click(object sender, MouseEventArgs e)
    {
        OpenFileDialog openImgDialog = new OpenFileDialog();
        openImgDialog.Filter = "图像文件(*.png;*.jpg;*.gif;*.bmp)|*.png;*.jpg;*.gif;*.bmp";
        openImgDialog.RestoreDirectory = true;
        openImgDialog.FilterIndex = 1;
        if (openImgDialog.ShowDialog() == DialogResult.OK)
        {
            content_ImgInput.textBox.Text = openImgDialog.FileName;
        }
    }

    // 图像缩放选项
    private static void ImgZoom_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;
        if ((int)pL.Tag == 0) ImgZoom_CheckBox[1].set_Flag(false);
        else ImgZoom_CheckBox[0].set_Flag(false);
    }

    // 形状选择
    private static void ShapSelect_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        for (int i = 0; i < 20; i++)
        {
            if (i == (int)pL.Tag) shap_select[i].set_Flag(true);
            else shap_select[i].set_Flag(false);
        }
    }

    // 清除打开图像路径
    private static void openClear_Click(object sender, MouseEventArgs e)
    {
        content_ImgInput.textBox.Text = "";
    }

    // 数据库字段选择
    private static void FieldSelect_MouseDown(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;

        DataField_Menu.Items.Clear();
        if ((data_Pool.data_Type != 0) && (data_Pool.data_TableName != ""))
        {
            GetField(data_Pool.data_Type, data_Pool.data_TableName);
            ToolStripMenuItem[] menu_Item = new ToolStripMenuItem[data_Fields.Count];
            for (int i = 0; i < data_Fields.Count; i++)
            {
                menu_Item[i] = new ToolStripMenuItem()
                {
                    AutoSize = true,
                    Font = system_Font,
                    Text = data_Fields[i].Field_Name,
                };
                menu_Item[i].Click += FieldMenu_Click;
                DataField_Menu.Items.Add(menu_Item[i]);
            }
        }
        else
        {
            ToolStripMenuItem menu_Item = new ToolStripMenuItem()
            {
                AutoSize = true,
                Font = system_Font,
                Text = "没有数据库链接！",
            };
            DataField_Menu.Items.Add(menu_Item);
        }
        DataField_Menu.Show(pL.PointToScreen(e.Location));
    }

    // 数据库字段运算
    private static void FieldFunction_MouseDown(object sender, MouseEventArgs e)
    {
        ////////new Set_FieldFunction().Field_Calculate();
    }

    private static void FieldMenu_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem mL = (ToolStripMenuItem)sender;
        content_DataInput.textBox.Text = mL.Text;
    }

    // 页码选择菜单
    private static void pgCode_Click(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;
        pgCode_Menu.Show(pL.PointToScreen(e.Location));
    }

    // 页码菜单点击
    private static void pgMenuCode_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem tL = (ToolStripMenuItem)sender;
        content_PcodeInput.textBox.Text = pgCode_Value[(int)tL.Tag];
    }

    // 显示组件信息
    public static void set_Info(int _controlType)
    {
        string[] control_str = new string[6] { "- 文本框", "- 图像", "- 形状", "- 数据库字段", "- 页码", "- 数据库运算" };

        for (int i = 0; i < 6; i++)
        {
            if ((i + 1) == _controlType)
            {
                panel_setControl[i].Visible = true;
                Label_ControlType.labelText.Text = control_str[i];
                pic_ControlType.spear_Img.Image = control_icon[i];
            }
            else panel_setControl[i].Visible = false;
        }

        if ((_controlType != 2) && (_controlType != 3))
        {
            string[] splitString = DraggableObjects[control_Num].Field_TextFontStyle.ToString().Split(new string[] { "," }, StringSplitOptions.None);
            for (int i = 0; i < 3; i++) font_render[_controlType - 1, i].set_Flag(false);
            for (int i = 0; i < splitString.Length; i++)
            {
                if (splitString[i].Trim() == "Bold") font_render[_controlType - 1, 0].set_Flag(true);
                if (splitString[i].Trim() == "Italic") font_render[_controlType - 1, 1].set_Flag(true);
                if (splitString[i].Trim() == "Underline") font_render[_controlType - 1, 2].set_Flag(true);
            }
            font_type[_controlType - 1].labelText.Text = DraggableObjects[control_Num].Field_TextFont;
            font_size[_controlType - 1].labelText.Text = DraggableObjects[control_Num].Field_TextFontSize + "";

            if (_controlType == 1) content_TextInput.textBox.Text = DraggableObjects[control_Num].Field_Text;
            if (_controlType == 4) content_DataInput.textBox.Text = DraggableObjects[control_Num].Field_Text;
            if (_controlType == 5) content_PcodeInput.textBox.Text = DraggableObjects[control_Num].Field_Text;
        }

        if (_controlType != 3)
        {
            for (int i = 0; i < 6; i++)
            {
                align_Check[_controlType - 1, i].set_Flag(false);
            }
            string[] splitString = DraggableObjects[control_Num].Field_Align.Split(new string[] { "," }, StringSplitOptions.None);
            for (int i = 0; i < splitString.Length; i++)
            {
                if (splitString[i] == "Left") align_Check[_controlType - 1, 0].set_Flag(true);
                if (splitString[i] == "Right") align_Check[_controlType - 1, 1].set_Flag(true);
                if (splitString[i] == "Top") align_Check[_controlType - 1, 2].set_Flag(true);
                if (splitString[i] == "Bottom") align_Check[_controlType - 1, 3].set_Flag(true);
                if (splitString[i] == "Middle") align_Check[_controlType - 1, 4].set_Flag(true);
                if (splitString[i] == "Vertical") align_Check[_controlType - 1, 5].set_Flag(true);
            }
        }

        if (_controlType != 2)
        {
            color_Flag[_controlType - 1].labelText.ForeColor = DraggableObjects[control_Num].Field_ControlColor;
        }

        if (_controlType == 3)
        {
            for (int i = 0; i < 20; i++)
            {
                if (i == DraggableObjects[control_Num].Field_Shape) shap_select[i].set_Flag(true);
                else shap_select[i].set_Flag(false);
            }
        }

        if (_controlType == 6)
        {
            content_FunctionInput.textBox.Text = DraggableObjects[control_Num].Field_Calculate;
        }

        View_Info = true;
    }

    /// <summary>
    /// 线程 显示组件位置
    /// </summary>
    public static void Set_ControlMoveInfo()
    {
        while (true)
        {
            if (View_Info)
            {
                Label_ControlSize.labelText.CrossThreadCalls(() =>
                {
                    Label_ControlSize.labelText.Text = "范围：宽-" + DraggableObjects[control_Num].Region.Width + "；高-" + DraggableObjects[control_Num].Region.Height;
                    Label_ControlPoint.labelText.Text = "坐标- ( x- " + DraggableObjects[control_Num].Region.Left + "，y- " + DraggableObjects[control_Num].Region.Top + " )";
                    attribute_Input[DraggableObjects[control_Num].ControlType - 1, 0].textBox.Text = DraggableObjects[control_Num].Region.Width + "";
                    attribute_Input[DraggableObjects[control_Num].ControlType - 1, 1].textBox.Text = DraggableObjects[control_Num].Region.Height + "";
                    attribute_Input[DraggableObjects[control_Num].ControlType - 1, 2].textBox.Text = DraggableObjects[control_Num].Region.Left + "";
                    attribute_Input[DraggableObjects[control_Num].ControlType - 1, 3].textBox.Text = DraggableObjects[control_Num].Region.Top + "";
                    View_Info = false;
                });
            }
        }
    }

    // 应用设置
    private static void accept_Attributes(object sender, EventArgs e)
    {
        if (DraggableObjects[control_Num].ControlType == 1)
        {
            if (content_TextInput.textBox.Text == "") return;
        }
        else
        if (DraggableObjects[control_Num].ControlType == 2)
        {
            if (content_ImgInput.textBox.Text == "")
            {
                if (!DraggableObjects[control_Num].isContent) return;
            }
            else
            {
                try
                {
                    DraggableObjects[control_Num].Field_Img = Image.FromFile(content_ImgInput.textBox.Text) as Bitmap;
                }
                catch { return; }
            }
        }
        else
        if (DraggableObjects[control_Num].ControlType == 4)
        {
            if (content_DataInput.textBox.Text == "") return;
        }
        else
        if (DraggableObjects[control_Num].ControlType == 5)
        {
            if (content_PcodeInput.textBox.Text == "") return;
        }
        else
        if (DraggableObjects[control_Num].ControlType == 6)
        {
            if (content_FunctionInput.textBox.Text == "") return;
        }

        RbControls_DrawTextMethod DrawText = new RbControls_DrawTextMethod();
        int _controlType = 1;

        string TextAlign = "";
        if (DraggableObjects[control_Num].ControlType != 3) // 不是形状
        {
            if (align_Check[DraggableObjects[control_Num].ControlType - 1, 0].select) TextAlign += "Left,";
            if (align_Check[DraggableObjects[control_Num].ControlType - 1, 1].select) TextAlign += "Right,";
            if (align_Check[DraggableObjects[control_Num].ControlType - 1, 2].select) TextAlign += "Top,";
            if (align_Check[DraggableObjects[control_Num].ControlType - 1, 3].select) TextAlign += "Bottom,";
            if (align_Check[DraggableObjects[control_Num].ControlType - 1, 4].select) TextAlign += "Middle,";
            if (align_Check[DraggableObjects[control_Num].ControlType - 1, 5].select) TextAlign += "Vertical,";

            DraggableObjects[control_Num].Field_Align = TextAlign;
        }

        if ((DraggableObjects[control_Num].ControlType != 2) && (DraggableObjects[control_Num].ControlType != 3)) // 不是图像和形状
        {
            FontStyle fst = new FontStyle();
            if (font_render[DraggableObjects[control_Num].ControlType - 1, 0].select) fst = FontStyle.Bold;
            else fst = FontStyle.Regular;
            if (font_render[DraggableObjects[control_Num].ControlType - 1, 1].select) fst = fst | FontStyle.Italic;
            if (font_render[DraggableObjects[control_Num].ControlType - 1, 2].select) fst = fst | FontStyle.Underline;

            Font fnt = new Font(font_type[DraggableObjects[control_Num].ControlType - 1].labelText.Text, int.Parse(font_size[0].labelText.Text), fst, GraphicsUnit.Pixel);
            DraggableObjects[control_Num].Field_TextFontStyle = fst;

            Bitmap bmp_Text = new Bitmap(DraggableObjects[control_Num].Region.Width, DraggableObjects[control_Num].Region.Height);
            Graphics gText = Graphics.FromImage(bmp_Text);

            string _text = "";
            if (DraggableObjects[control_Num].ControlType == 1) _text = content_TextInput.textBox.Text;
            if (DraggableObjects[control_Num].ControlType == 4) _text = content_DataInput.textBox.Text;
            if (DraggableObjects[control_Num].ControlType == 5) _text = content_PcodeInput.textBox.Text;
            if (DraggableObjects[control_Num].ControlType == 6) _text = content_FunctionInput.textBox.Text;

            Point point_text = TextPoint(control_Num, bmp_Text.Width - 7, bmp_Text.Height - 7, gText, _text, fnt);

            DrawText.DrawString(bmp_Text, _text, fnt, color_Flag[DraggableObjects[control_Num].ControlType - 1].labelText.ForeColor, new Rectangle(point_text.X, point_text.Y, bmp_Text.Width, bmp_Text.Height));
            DraggableObjects[control_Num].Field_Img = bmp_Text;
            DraggableObjects[control_Num].Field_Text = _text;
            DraggableObjects[control_Num].Field_TextFont = font_type[DraggableObjects[control_Num].ControlType - 1].labelText.Text;
            DraggableObjects[control_Num].Field_TextFontSize = int.Parse(font_size[DraggableObjects[control_Num].ControlType - 1].labelText.Text);
            DraggableObjects[control_Num].Field_TextFontStyle = fst;
            DraggableObjects[control_Num].Field_Align = TextAlign;
            DraggableObjects[control_Num].Field_ControlColor = color_Flag[DraggableObjects[control_Num].ControlType - 1].labelText.ForeColor;

            if (DraggableObjects[control_Num].ControlType == 6) DraggableObjects[control_Num].Field_Calculate = content_FunctionInput.textBox.Text;
        }

        if (DraggableObjects[control_Num].ControlType == 2) // 图像
        {
            if (ImgZoom_CheckBox[0].select) DraggableObjects[control_Num].Field_ImgZoom = 0;
            else DraggableObjects[control_Num].Field_ImgZoom = 1;
            _controlType = 2;
        }

        if (DraggableObjects[control_Num].ControlType == 3) // 形状
        {
            for (int i = 0; i < 20; i++)
            {
                if (shap_select[i].select)
                {
                    DraggableObjects[control_Num].Field_Shape = i;
                    break;
                }
            }
            _controlType = 3;
            DraggableObjects[control_Num].Field_ControlColor = color_Flag[DraggableObjects[control_Num].ControlType - 1].labelText.ForeColor;
        }

        DraggableObjects[control_Num].isContent = true;

        DraggableObjects[control_Num].Region = new Rectangle(
                int.Parse(attribute_Input[DraggableObjects[control_Num].ControlType - 1, 2].textBox.Text),
                int.Parse(attribute_Input[DraggableObjects[control_Num].ControlType - 1, 3].textBox.Text),
                int.Parse(attribute_Input[DraggableObjects[control_Num].ControlType - 1, 0].textBox.Text),
                int.Parse(attribute_Input[DraggableObjects[control_Num].ControlType - 1, 1].textBox.Text)
            );

        DraggableObjects[control_Num].Setimage = LinBox(
            DraggableObjects[control_Num].Region.Width,
            DraggableObjects[control_Num].Region.Height,
            1,
            _controlType,
            control_Num
            );
        Print_PageType.Invalidate();
        Object_Record();
        ReportChange_Flag = true;
    }

    // 文字对齐显示
    public static Point TextPoint(int _controlNum, int width, int height, Graphics g, string text, Font font_family)
    {
        int _lx = 0, _ly = 0;
        SizeF sizeF = g.MeasureString(text, font_family);

        string[] splitString = DraggableObjects[_controlNum].Field_Align.Split(new string[] { "," }, StringSplitOptions.None);
        for (int i = 0; i < splitString.Length; i++)
        {
            if (splitString[i] == "Vertical") _ly = (height - (int)sizeF.Height) / 2;
            if (splitString[i] == "Middle") _lx = (width - (int)sizeF.Width) / 2;
            if (splitString[i] == "Left") _lx = 0;
            if (splitString[i] == "Right") _lx = width - (int)sizeF.Width;
            if (splitString[i] == "Top") _ly = 0;
            if (splitString[i] == "Bottom") _ly = height - (int)sizeF.Height;
        }
        if (_lx < 0) _lx = 0;
        if (_ly < 0) _ly = 0;
        Point point = new Point(_lx, _ly);
        return point;
    }

    // 设置图像位置
    public static Point ImagePoint(int _controlNum, int width, int height)
    {
        int _lx = 0, _ly = 0;

        string[] splitString = DraggableObjects[_controlNum].Field_Align.Split(new string[] { "," }, StringSplitOptions.None);
        for (int i = 0; i < splitString.Length; i++)
        {
            if (splitString[i] == "Vertical") _ly = (height - DraggableObjects[_controlNum].Field_Img.Height) / 2;
            if (splitString[i] == "Middle") _lx = (width - DraggableObjects[_controlNum].Field_Img.Width) / 2;
            if (splitString[i] == "Left") _lx = 0;
            if (splitString[i] == "Right") _lx = width - DraggableObjects[_controlNum].Field_Img.Width;
            if (splitString[i] == "Top") _ly = 0;
            if (splitString[i] == "Bottom") _ly = height - DraggableObjects[_controlNum].Field_Img.Height;
        }
        if (_lx < 0) _lx = 0;
        if (_ly < 0) _ly = 0;

        Point point = new Point(_lx, _ly);
        return point;
    }

    // 设置信息栏为默认
    public static void Set_DefaultInfo()
    {
        pic_ControlType.spear_Img.Image = EzRBuild.EzResource.mouse;
        Label_ControlType.labelText.Text = "";
        Label_ControlSize.labelText.Text = "范围：";
        Label_ControlPoint.labelText.Text = "位置：";
        for (int i = 0; i < 6; i++) panel_setControl[i].Visible = false;
    }

    
    // 综合信息显示
    public static void Set_CompositeLocation()
    {
        string _data = "None";
        if (data_Pool.data_Type == 0) _data = "None";
        else if (data_Pool.data_Type == 1) _data = "SQL Server";
        else if (data_Pool.data_Type == 2) _data = "Access";
        if (data_Pool.data_Type != 0) _data += " - " + data_Pool.data_TableName;

        string _page = "A4, 纵向";
        if (_pgselect == -1) _page = "自定义纸张";
        else
        {
            _page = page_types[page_TypeFace.Page_Type] + ", ";
            if (_pgdirect == 0) _page += "纵向";
            else _page += "横向";
        }

        string _info = "✐  当前纸张：" + _page + 
            "  ✐  打印机：" + DefaultPrinter + 
            "  ✐  数据库： "+ _data + "  ✐";
        Composite_Info.labelText.Text = _info;
    }
    
}

