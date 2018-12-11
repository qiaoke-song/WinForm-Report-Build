using System;
using System.Drawing;
using System.Windows.Forms;
using static Define_System;
using static Define_Draggable;
using static Define_Design;
using static Define_PrintPage;

public class RBuild_SetPageType
{
    private RbControls_DialogCreate setPage_Form = new RbControls_DialogCreate();
    private RbControls_TextBox Input_Type = new RbControls_TextBox();
    private RbControls_TextBox[] input_def = new RbControls_TextBox[2];
    private RbControls_TextBox[] input_Margin = new RbControls_TextBox[4];
    private RbControls_CheckBox[] Select_Direct = new RbControls_CheckBox[2];
    private ContextMenuStrip printPage_Menu;
    private TextBox tmp;
    private int _record;

    public void Set_Type()
    {
        tmp = new TextBox()
        {
            Size = new Size(0, 0),
            Location = new Point(0, 0),
            BorderStyle = BorderStyle.None
        };
        setPage_Form.form_panel.Controls.Add(tmp);

        _record = _pgselect;

        new RbControls_SpearLine().Spear_line(setPage_Form.form_panel, new Size(20, 20), new Point(15, 10), EzRBuild.EzResource.ptype);
        new RbControls_TextLabel().Text_Label(setPage_Form.form_panel, new Point(40, 12), system_Font,system_FontColor, "纸张类型 - 改变纸型,将清除当前报表组件！");

        new RbControls_ButtonLabel().Button_Label(setPage_Form.form_panel, new Point(170, 38), system_btnEnter, system_btnEnter, system_btnEnter, "◥", system_Font,0, pgSelect_Click);

        string[] Text_def = new string[2] { "自定义宽度(毫米)：", "自定义高度(毫米)：" };
        string[] Text_Direct = new string[2] { "纵向", "横向" };
        for (int i = 0; i < 2; i++)
        {
            input_def[i] = new RbControls_TextBox();
            input_def[i].input_Box(setPage_Form.form_panel, 30, new Point(120 + i * 145, 65), system_Font,system_inputColor, Color.White, "", 4, false, input_Number, null, null);
            input_def[i].textBox.Enabled = false;

            Select_Direct[i] = new RbControls_CheckBox();
            if (i == _pgdirect) Select_Direct[i].check_Box(setPage_Form.form_panel, true, i, new Point(195 + i * 60, 35), Color.Black, system_Font,Text_Direct[i], check_AlwaysTrue, SelectDirect_Click);
            else Select_Direct[i].check_Box(setPage_Form.form_panel, false, i, new Point(195 + i * 60, 35), Color.Black, system_Font,Text_Direct[i], check_AlwaysTrue, SelectDirect_Click);

            new RbControls_TextLabel().Text_Label(setPage_Form.form_panel, new Point(15 + i * 145, 65), system_Font,Color.Black, Text_def[i]);

            new RbControls_ButtonLabel().Button_Label(setPage_Form.form_panel, new Point(165 + i * 85, 170), system_buttonColor, system_btnEnter, system_buttonColor, acceptSetMenu[i], system_Font,i, acceptSet_Click);
        }

        if (_pgselect == -1)
        {
            Input_Type.input_Box(setPage_Form.form_panel, 150, new Point(20, 36), system_Font,system_inputColor, Color.White, "自定义纸张", 1024, true, input_Normal, inputType_Click, inputType_keyPress);
            input_def[0].textBox.Text = page_TypeFace.Rect_mm[0];
            input_def[1].textBox.Text = page_TypeFace.Rect_mm[1];
            input_def[0].textBox.Enabled = true;
            input_def[1].textBox.Enabled = true;
        }
        else Input_Type.input_Box(setPage_Form.form_panel, 150, new Point(20, 36), system_Font,system_inputColor, Color.White, page_types[_record] + ", " + page_size[_record] + " 毫米", 1024, true, input_Normal, inputType_Click, inputType_keyPress);


        new RbControls_SpearLine().Spear_line(setPage_Form.form_panel, new Size(308, 5), new Point(5, 95), EzRBuild.EzResource.hspear);

        new RbControls_SpearLine().Spear_line(setPage_Form.form_panel, new Size(20, 20), new Point(15, 110), EzRBuild.EzResource.margin);
        new RbControls_TextLabel().Text_Label(setPage_Form.form_panel, new Point(40, 112), system_Font,system_FontColor, "边距设置 ( 单位：像素 )");

        string[] Text_margin = new string[4] { "左：", "上：", "右：", "下：" };
        for (int i = 0; i < 4; i++)
        {
            input_Margin[i] = new RbControls_TextBox();
            input_Margin[i].input_Box(setPage_Form.form_panel, 30, new Point(42 + i * 70, 137), system_Font,system_inputColor, Color.White, page_TypeFace.Page_Margin[i] + "", 4, false, input_Number, null, null);
            new RbControls_TextLabel().Text_Label(setPage_Form.form_panel, new Point(15 + i * 70, 137), system_Font,Color.Black, Text_margin[i]);
        }

        // 打印纸张菜单
        printPage_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };
        ToolStripMenuItem[] printType_Item = new ToolStripMenuItem[pageType_Lists.Count];
        for (int i = 0; i < pageType_Lists.Count; i++)
        {
            printType_Item[i] = new ToolStripMenuItem()
            {
                AutoSize = true,
                Text = pageType_Lists[i].pageType,
                Tag = i
            };
            printType_Item[i].Click += printTypeItem_Click;
            printPage_Menu.Items.Add(printType_Item[i]);
        }

        tmp.Focus();

        setPage_Form.Create_Dialog(
                "setPage_Form", FormStartPosition.Manual, new Size(318, 197), new Point(RBuild_Design.design_Form.LocationEX.X + 105, RBuild_Design.design_Form.LocationEX.Y + 90),
                Color.White, form_ShowDialog, true, null
            );
    }

    private void SelectDirect_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        for (int i = 0; i < 2; i++)
        {
            if (i == (int)pL.Tag) Select_Direct[i].set_Flag(true);
            else Select_Direct[i].set_Flag(false);
        }
        _pgdirect = (int)pL.Tag;
    }

    private void pgSelect_Click(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;
        printPage_Menu.Show(pL.PointToScreen(e.Location));
    }

    private void printTypeItem_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem tL = (ToolStripMenuItem)sender;
        Input_Type.textBox.Text = tL.Text;
        if ((int)tL.Tag == 0)
        {
            for (int i = 0; i < 2; i++) input_def[i].textBox.Enabled = true;
            input_def[0].textBox.Focus();
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                input_def[i].textBox.Enabled = false;
                input_def[i].textBox.Text = "";
            }
            tmp.Focus();
        }
        _pgselect = (int)tL.Tag - 1;
    }

    private void inputType_Click(object sender, EventArgs e)
    {
        tmp.Focus();
    }

    private void inputType_keyPress(object sender, KeyEventArgs e)
    {
        tmp.Focus();
    }

    private void acceptSet_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        if ((int)pL.Tag == 1)
        {
            _pgselect = _record;
            setPage_Form._formObject.Close();
        }
        else
        {
            for (int i = 0; i < 4; i++) page_TypeFace.Page_Margin[i] = int.Parse(input_Margin[i].textBox.Text.Trim());
            page_TypeFace.Page_Type = _pgselect;

            if ((_record != _pgselect) || (input_def[0].textBox.Text != page_TypeFace.Rect_mm[0]) || (input_def[1].textBox.Text != page_TypeFace.Rect_mm[1]) || (page_TypeFace.Page_Direction != _pgdirect))
            {
                RBuild_Info.Set_DefaultInfo();
                DraggableObjects.Clear();
                control_Num = -1;

                page_Container.VerticalScroll.Value = 0;
                page_Container.HorizontalScroll.Value = 0;

                if (_pgselect == -1)
                {
                    page_TypeFace.Page_Area.Width = (int)Math.Floor(int.Parse(input_def[0].textBox.Text) * 3.779527559055118);
                    page_TypeFace.Page_Area.Height = (int)Math.Floor(int.Parse(input_def[1].textBox.Text) * 3.779527559055118) + 13 + 126;
                    page_TypeFace.Rect_mm[0] = input_def[0].textBox.Text;
                    page_TypeFace.Rect_mm[1] = input_def[1].textBox.Text;
                }
                else
                {
                    Set_PrintPageType(_pgselect, _pgdirect);
                    page_TypeFace.Rect_mm[0] = "";
                    page_TypeFace.Rect_mm[1] = "";
                }
                // 设置预览页面大小
                PreViewPage_Area = new Size(page_TypeFace.Page_Area.Width, page_TypeFace.Page_Area.Height - 126);// 去掉-126

                int height = 120;
                int ly = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0) { height = 120; ly = 0; }
                    else if (i == 1) { height = 250; ly = 128; }
                    else if (i == 2) { height = 120; ly = page_TypeFace.Page_Area.Height - height; }
                    DraggableBandObjects[i].Region = new Rectangle(0, ly, page_TypeFace.Page_Area.Width, height);
                }

                page_Install.Size = new Size(page_TypeFace.Page_Area.Width + 20, page_TypeFace.Page_Area.Height + 20);
                int _iLeft = (page_Container.Width / 2) - (page_TypeFace.Page_Area.Width / 2);
                if (_iLeft < 0) _iLeft = 0;
                page_Install.Location = new Point(_iLeft, 0);
                page_Install.Invalidate();

                Print_PageType.Size = page_TypeFace.Page_Area;
            }
            RBuild_Info.Set_CompositeLocation();
            setPage_Form._formObject.Close();
        }
        Print_PageType.Invalidate();
    }
}
