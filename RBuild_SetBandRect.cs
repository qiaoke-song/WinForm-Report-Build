using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Define_System;
using static Define_Draggable;
using static Define_Design;

public class RBuild_SetBandRect
{
    private RbControls_DialogCreate setBand_Form = new RbControls_DialogCreate();
    private RbControls_CheckBox[,] select_label = new RbControls_CheckBox[3, 2];
    private RbControls_TextBox[] attribute_Input = new RbControls_TextBox[3]; // 属性输入框

    public void Set_Band(int _mode)
    {
        Bitmap[] band_Icon = new Bitmap[3] { EzRBuild.EzResource.band_pt, EzRBuild.EzResource.band_pd, EzRBuild.EzResource.band_pb };
        string[] band_Text = new string[3] { "页头栏 -", "内容栏 -", "页脚栏 -" };
        string[] band_setText = new string[2] { "自适应最小高度", "最大填充高度" };

        int _tag = 0;
        for (int i = 0; i < 3; i++)
        {
            new RbControls_SpearLine().Spear_line(setBand_Form.form_panel, new Size(25, 25), new Point(10, 10 + i * 50), band_Icon[i]);

            for (int t = 0; t < 2; t++)
            {
                select_label[i, t] = new RbControls_CheckBox();
                select_label[i, t].check_Box(setBand_Form.form_panel, false, _tag, new Point(30 + t * 110, 33 + i * 50), Color.Black, system_Font,band_setText[t], check_Normal, selectLabe_Click);
                _tag += 1;
            }

            new RbControls_TextLabel().Text_Label(setBand_Form.form_panel, new Point(35, 13 + i * 50), system_Font,system_FontColor, band_Text[i]);
            new RbControls_SpearLine().Spear_line(setBand_Form.form_panel, new Size(5, 25), new Point(240, 32 + i * 50), EzRBuild.EzResource.spear);
            attribute_Input[i] = new RbControls_TextBox();
            attribute_Input[i].input_Box(setBand_Form.form_panel, 40, new Point(285, 35 + i * 50), system_Font,Color.Salmon, Color.White, "", 4, false, 2, null, input_keyUp);
            attribute_Input[i].textBox.Tag = i;

            new RbControls_TextLabel().Text_Label(setBand_Form.form_panel, new Point(250, 35 + i * 50), system_Font,Color.Black, "高度：");
        }

        //string[] acceptSetMenu = new string[2] { "[✔ 应用设置 ]", "[✘ 取消 ]" };
        for (int i = 0; i < 2; i++)
        {
            new RbControls_ButtonLabel().Button_Label(setBand_Form.form_panel, new Point(195 + i * 85, 165), system_buttonColor, Color.Salmon, system_buttonColor, acceptSetMenu[i], system_Font,i, acceptSet_Click);
        }

        if (_mode == 1)
        {
            setBand_Form.Create_Dialog(
                "setBand_Form", FormStartPosition.CenterScreen, new Size(350, 190), new Point(RBuild_Design.design_Form.LocationEX.X + 70, RBuild_Design.design_Form.LocationEX.Y + 90),
                Color.White, form_ShowDialog, true, null);
        }
        else
            setBand_Form.Create_Dialog(
                "setBand_Form", FormStartPosition.Manual, new Size(350, 190), new Point(RBuild_Design.design_Form.LocationEX.X + 70, RBuild_Design.design_Form.LocationEX.Y + 90),
                Color.White, form_ShowDialog, true, null);
    }

    private void input_keyUp(object sender, KeyEventArgs e)
    {
        TextBox pL = (TextBox)sender;

        select_label[(int)pL.Tag, 0].set_Flag(false);
        select_label[(int)pL.Tag, 1].set_Flag(false);
    }

    private void selectLabe_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        if ((int)pL.Tag == 0) { select_label[0, 1].set_Flag(false); attribute_Input[0].textBox.Text = ""; }
        if ((int)pL.Tag == 1) { select_label[0, 0].set_Flag(false); attribute_Input[0].textBox.Text = ""; }

        if ((int)pL.Tag == 2) { select_label[1, 1].set_Flag(false); attribute_Input[1].textBox.Text = ""; }
        if ((int)pL.Tag == 3) { select_label[1, 0].set_Flag(false); attribute_Input[1].textBox.Text = ""; }

        if ((int)pL.Tag == 4) { select_label[2, 1].set_Flag(false); attribute_Input[2].textBox.Text = ""; }
        if ((int)pL.Tag == 5) { select_label[2, 0].set_Flag(false); attribute_Input[2].textBox.Text = ""; }
    }

    private void acceptSet_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        if ((int)pL.Tag == 1) setBand_Form._formObject.Close();
        else
        {
            for (int i = 0; i < 3; i++)
            {
                for (int t = 0; t < 2; t++)
                {
                    if (select_label[i, t].select)
                    {
                        if (t == 0)
                        {
                            int _maxHeight = 43;

                            var DraggableMaxBottom = DraggableObjects.Where(bottom => bottom.Belong_Band == i).OrderByDescending(sort => sort.Region.Bottom).ToList();
                            if (DraggableMaxBottom.Count > 0) _maxHeight = (DraggableMaxBottom[0].Region.Bottom - DraggableBandObjects[i].Region.Top) + 20;

                            DraggableBandObjects[i].Region = new Rectangle(0, DraggableBandObjects[i].Region.Top, DraggableBandObjects[i].Region.Width, _maxHeight);

                            Print_PageType.Invalidate();
                            ReportChange_Flag = true;
                            setBand_Form._formObject.Close();
                        }
                        if (t == 1)
                        {
                            int _top = 0, _height = 43;
                            if (i == 0)
                            {
                                _top = 0;
                                _height = DraggableBandObjects[1].Region.Top - DraggableBandObjects[0].Region.Bottom + DraggableBandObjects[0].Region.Height;
                            }
                            if (i == 1)
                            {
                                _top = DraggableBandObjects[0].Region.Bottom;
                                _height = DraggableBandObjects[2].Region.Top - DraggableBandObjects[1].Region.Bottom + DraggableBandObjects[1].Region.Height +
                                    DraggableBandObjects[1].Region.Top - DraggableBandObjects[0].Region.Bottom;
                            }
                            if (i == 2)
                            {
                                _top = DraggableBandObjects[1].Region.Bottom;
                                _height = Print_PageType.Height - DraggableBandObjects[2].Region.Bottom + DraggableBandObjects[2].Region.Height +
                                    DraggableBandObjects[2].Region.Top - DraggableBandObjects[1].Region.Bottom;
                            }

                            DraggableBandObjects[i].Region = new Rectangle(0, _top, DraggableBandObjects[i].Region.Width, _height);

                            Print_PageType.Invalidate();
                            ReportChange_Flag = true;
                            setBand_Form._formObject.Close();
                        }
                    }
                }

                if ((attribute_Input[i].textBox.Text != null) && (attribute_Input[i].textBox.Text != ""))
                {
                    int _maxHeight = int.Parse(attribute_Input[i].textBox.Text) + 42;

                    if (_maxHeight < 43) _maxHeight = 43;
                    var DraggableMaxBottom = DraggableObjects.Where(bottom => bottom.Belong_Band == i).OrderByDescending(sort => sort.Region.Bottom).ToList();
                    if (DraggableMaxBottom.Count > 0) _maxHeight = (DraggableMaxBottom[0].Region.Bottom - DraggableBandObjects[i].Region.Top) + 20;
                    DraggableBandObjects[i].Region = new Rectangle(0, DraggableBandObjects[i].Region.Top, DraggableBandObjects[i].Region.Width, _maxHeight);

                    Print_PageType.Invalidate();
                    ReportChange_Flag = true;
                    setBand_Form._formObject.Close();
                }
            }
        }
    }
}
