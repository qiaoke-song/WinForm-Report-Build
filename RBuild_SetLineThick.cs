using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static RbControls;
using static Define_System;
using static Define_Draggable;
using static Define_Design;
using static Define_DrawObject;

public class RBuild_SetLineThick
{
    private RbControls_DialogCreate setThick_Form = new RbControls_DialogCreate();
    private PanelEx[] panel_Type = new PanelEx[5];
    private PanelEx[] panel_Coat = new PanelEx[5];
    private int[] thick = new int[] { 1, 2, 3, 4, 5 };

    PanelEx[] panel_LineType = new PanelEx[5];
    PanelEx[] panel_LineCoat = new PanelEx[5];
    DashStyle[] dashStyles = new DashStyle[] { DashStyle.Dash, DashStyle.DashDot, DashStyle.DashDotDot, DashStyle.Dot, DashStyle.Solid };


    public void Set_Thick()
    {
        new RbControls_SpearLine().Spear_line(setThick_Form.form_panel, new Size(20, 20), new Point(5, 10), EzRBuild.EzResource.thick_type);
        new RbControls_TextLabel().Text_Label(setThick_Form.form_panel, new Point(30, 12), system_Font,system_FontColor, "框线粗细");

        new RbControls_SpearLine().Spear_line(setThick_Form.form_panel, new Size(165, 5), new Point(5, 188), EzRBuild.EzResource.hspear);

        new RbControls_SpearLine().Spear_line(setThick_Form.form_panel, new Size(20, 20), new Point(5, 203), EzRBuild.EzResource.line_type);
        new RbControls_TextLabel().Text_Label(setThick_Form.form_panel, new Point(30, 203), system_Font,system_FontColor, "框线类型");

        for (int i = 0; i < 5; i++)
        {
            panel_Coat[i] = new PanelEx()
            {
                Size = new Size(150, 25),
                Location = new Point(10, 35 + i * 30),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            setThick_Form.form_panel.Controls.Add(panel_Coat[i]);

            panel_Type[i] = new PanelEx()
            {
                Size = new Size(panel_Coat[i].Width - 2, panel_Coat[i].Height - 2),
                Location = new Point(1, 1),
                BackColor = Color.Transparent,
                Tag = i
            };
            panel_Type[i].Paint += SetThickType_Paint;
            panel_Type[i].MouseEnter += SetThickType_MouseEnter;
            panel_Type[i].Click += SetThickness_Click;
            panel_Coat[i].Controls.Add(panel_Type[i]);
        }


        for (int i = 0; i < 5; i++)
        {
            panel_LineCoat[i] = new PanelEx()
            {
                Size = new Size(150, 25),
                Location = new Point(10, 225 + i * 30),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            setThick_Form.form_panel.Controls.Add(panel_LineCoat[i]);

            panel_LineType[i] = new PanelEx()
            {
                Size = new Size(panel_LineCoat[i].Width - 2, panel_LineCoat[i].Height - 2),
                Location = new Point(1, 1),
                BackColor = Color.Transparent,
                Tag = i
            };
            panel_LineType[i].Paint += SetLineType_Paint;
            panel_LineType[i].MouseEnter += SetLineType_MouseEnter;
            panel_LineType[i].Click += SetLineType_Click;
            panel_LineCoat[i].Controls.Add(panel_LineType[i]);
        }

        setThick_Form.Create_Dialog(
            "setThick_Form", FormStartPosition.Manual, new Size(175, 378), new Point(RBuild_Design.design_Form.LocationEX.X + 940, RBuild_Design.design_Form.LocationEX.Y + 90),
            Color.White, form_ShowDialog, true, null
        );

    }

    private void SetThickType_Paint(object sender, PaintEventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        Graphics g = e.Graphics;
        Pen pen = new Pen(Color.Black, thick[(int)pL.Tag]);
        g.DrawLine(pen, new Point(3, (24 - thick[(int)pL.Tag]) / 2), new Point(pL.Width - 3, (24 - thick[(int)pL.Tag]) / 2));
    }

    private void SetThickType_MouseEnter(object sender, EventArgs e)
    {
        PanelEx pL = (PanelEx)sender;
        for (int i = 0; i < 5; i++)
        {
            if (i == (int)pL.Tag)
            {
                panel_Coat[i].BackColor = Color.FromArgb(229, 195, 101);
                panel_Type[i].BackColor = Color.FromArgb(253, 244, 191);
            }
            else
            {
                panel_Coat[i].BackColor = Color.Transparent;
                panel_Type[i].BackColor = Color.Transparent;
            }
        }
    }

    private void SetLineType_Paint(object sender, PaintEventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        Graphics g = e.Graphics;
        Pen pen = new Pen(Color.Black, 2);
        pen.DashStyle = dashStyles[(int)pL.Tag];
        g.DrawLine(pen, new Point(3, 12), new Point(pL.Width - 3, 12));
    }

    private void SetLineType_MouseEnter(object sender, EventArgs e)
    {
        PanelEx pL = (PanelEx)sender;
        for (int i = 0; i < 5; i++)
        {
            if (i == (int)pL.Tag)
            {
                panel_LineCoat[i].BackColor = Color.FromArgb(229, 195, 101);
                panel_LineType[i].BackColor = Color.FromArgb(253, 244, 191);
            }
            else
            {
                panel_LineCoat[i].BackColor = Color.Transparent;
                panel_LineType[i].BackColor = Color.Transparent;
            }
        }
    }

    private void SetThickness_Click(object sender, EventArgs e)
    {
        PanelEx pL = (PanelEx)sender;
        if (control_Num > -1)
        {
            if (Array.IndexOf(DraggableObjects[control_Num].Field_BoxLine, true) != -1)
            {
                DraggableObjects[control_Num].Field_LineThickness = thick[(int)pL.Tag];
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

    private void SetLineType_Click(object sender, EventArgs e)
    {
        PanelEx pL = (PanelEx)sender;
        if (control_Num > -1)
        {
            if (Array.IndexOf(DraggableObjects[control_Num].Field_BoxLine, true) != -1)
            {
                DraggableObjects[control_Num].Field_LineType = dashStyles[(int)pL.Tag];
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
