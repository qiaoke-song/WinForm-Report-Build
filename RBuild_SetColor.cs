using System;
using System.Drawing;
using System.Windows.Forms;
using static RbControls;
using static Define_System;
using static Define_Draggable;
using static Define_Design;
using static Define_DrawObject;

public class RBuild_SetColor
{
    private RbControls_DialogCreate setColor_Form = new RbControls_DialogCreate();
    private Color[] colors = new Color[48]
    {
        Color.FromArgb(255,128,128),Color.FromArgb(255,255,128),Color.FromArgb(128,255,128),Color.FromArgb(0,255,128),
        Color.FromArgb(128,255,255),Color.FromArgb(0,128,255),Color.FromArgb(255,128,192),Color.FromArgb(128,255,128),
        Color.FromArgb(255,0,0),Color.FromArgb(255,255,0),Color.FromArgb(128,255,0),Color.FromArgb(0,255,64),
        Color.FromArgb(0,255,255),Color.FromArgb(0,128,192),Color.FromArgb(128,128,192),Color.FromArgb(255,0,255),
        Color.FromArgb(128,64,64),Color.FromArgb(255,128,64),Color.FromArgb(0,255,0),Color.FromArgb(0,128,128),
        Color.FromArgb(0,64,128),Color.FromArgb(128,128,255),Color.FromArgb(128,0,64),Color.FromArgb(255,0,128),
        Color.FromArgb(128,0,0),Color.FromArgb(255,128,0),Color.FromArgb(0,128,0),Color.FromArgb(0,128,64),
        Color.FromArgb(0,0,255),Color.FromArgb(0,0,160),Color.FromArgb(128,0,128),Color.FromArgb(128,0,255),
        Color.FromArgb(64,0,0),Color.FromArgb(28,64,0),Color.FromArgb(0,64,0),Color.FromArgb(0,64,64),
        Color.FromArgb(0,0,128),Color.FromArgb(0,0,64),Color.FromArgb(0,0,64),Color.FromArgb(64,0,128),
        Color.FromArgb(0,0,0),Color.FromArgb(128,128,0),Color.FromArgb(128,128,64),Color.FromArgb(128,128,128),
        Color.FromArgb(64,128,128),Color.FromArgb(192,192,192),Color.FromArgb(64,0,64),Color.FromArgb(255,255,255),
    };
    private PanelEx[] panel_Color = new PanelEx[48];
    private RbControls_CheckBox[] selectType = new RbControls_CheckBox[2];

    public void Set_Colors()
    {
        int _stepx = 0, _stepy = 0;
        for (int i = 0; i < 48; i++)
        {
            if (i % 8 == 0)
            {
                _stepy += 1;
                _stepx = 0;
            }
            PanelEx select_Color = new PanelEx()
            {
                Size = new Size(17, 17),
                Location = new Point(10 + _stepx * 22, 20 + _stepy * 22),
                BackColor = Color.FromArgb(175, 175, 175),
                Cursor = Cursors.Hand,
                Tag = i
            };
            _stepx += 1;
            select_Color.Click += SetColor_Click;
            setColor_Form.form_panel.Controls.Add(select_Color);

            panel_Color[i] = new PanelEx()
            {
                Size = new Size(15, 15),
                Location = new Point(1, 1),
                BackColor = colors[i],
                Tag = i
            };
            panel_Color[i].Click += SetColor_Click;
            select_Color.Controls.Add(panel_Color[i]);
        }

        new RbControls_SpearLine().Spear_line(setColor_Form.form_panel, new Size(20, 20), new Point(5, 10), EzRBuild.EzResource.set_color);

        string[] selectText = new string[2] { "背景色", "边框色" };
        for (int i = 0; i < 2; i++)
        {
            selectType[i] = new RbControls_CheckBox();
            if (i == 0) selectType[i].check_Box(setColor_Form.form_panel, true, i, new Point(30 + i * 60, 10), system_FontColor, system_Font,selectText[i], check_AlwaysTrue, select_Click);
            else selectType[i].check_Box(setColor_Form.form_panel, false, i, new Point(30 + i * 60, 10), system_FontColor, system_Font,selectText[i], check_AlwaysTrue, select_Click);
        }

        setColor_Form.Create_Dialog(
                "setColor_Form", FormStartPosition.Manual, new Size(192, 178), new Point(RBuild_Design.design_Form.LocationEX.X + 305, RBuild_Design.design_Form.LocationEX.Y + 90),
                Color.White, form_ShowDialog, true, null
            );
    }

    private void select_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        for (int i = 0; i < 2; i++)
        {
            if ((int)pL.Tag != i) selectType[i].set_Flag(false);
        }
    }

    private void SetColor_Click(object sender, EventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        if (control_Num > -1)
        {
            if (selectType[0].select)
            {
                DraggableObjects[control_Num].Field_BackColor = colors[(int)pL.Tag];
                DraggableObjects[control_Num].Setimage = LinBox(
                    DraggableObjects[control_Num].Region.Width,
                    DraggableObjects[control_Num].Region.Height,
                    1,
                    DraggableObjects[control_Num].ControlType,
                    control_Num
                    );
                Print_PageType.Invalidate();
                ReportChange_Flag = true;
            }
            else
            if (selectType[1].select)
            {
                if (Array.IndexOf(DraggableObjects[control_Num].Field_BoxLine, true) != -1)
                {
                    DraggableObjects[control_Num].Field_LineColor = colors[(int)pL.Tag];
                    DraggableObjects[control_Num].Setimage = LinBox(
                        DraggableObjects[control_Num].Region.Width,
                        DraggableObjects[control_Num].Region.Height,
                        1,
                        DraggableObjects[control_Num].ControlType,
                        control_Num
                        );
                    Print_PageType.Invalidate();
                    ReportChange_Flag = true;
                }

            }
        }
    }
}
