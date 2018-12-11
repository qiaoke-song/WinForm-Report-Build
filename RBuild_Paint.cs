using System;
using System.Drawing;
using System.Windows.Forms;
using static RbControls;
using static Define_Draggable;
using static Define_Design;
using static Define_PrintPage;
using static Define_DrawObject;

public static class RBuild_Paint
{
    public static void pageContainer_Paint(object sender, PaintEventArgs e)
    {
        if (control_Num > -1)
        {
            if (Show_Line)
            {
                Pen flagPen = new Pen(Color.FromArgb(170, 170, 170), 1);
                flagPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                e.Graphics.DrawLine(
                    flagPen,
                    0,
                    DraggableObjects[control_Num].Region.Top + 13 + page_Install.Top,
                    page_Install.Left,
                    DraggableObjects[control_Num].Region.Top + 13 + page_Install.Top
                    );
                e.Graphics.DrawLine(
                    flagPen,
                    0,
                    DraggableObjects[control_Num].Region.Bottom + 6 + page_Install.Top,
                    page_Install.Left,
                    DraggableObjects[control_Num].Region.Bottom + 6 + page_Install.Top
                    );
            }
        }
    }

    public static void pageInstall_Paint(object sender, PaintEventArgs e)
    {
        if (control_Num > -1)
        {
            if (Show_Line)
            {
                Pen flagPen = new Pen(Color.FromArgb(170, 170, 170), 1);
                flagPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                e.Graphics.DrawLine(
                    flagPen,
                    0,
                    DraggableObjects[control_Num].Region.Top + 13,
                    20,
                    DraggableObjects[control_Num].Region.Top + 13
                    );
                e.Graphics.DrawLine(
                    flagPen,
                    0,
                    DraggableObjects[control_Num].Region.Bottom + 6,
                    20,
                    DraggableObjects[control_Num].Region.Bottom + 6
                    );
                e.Graphics.DrawLine(
                    flagPen,
                    DraggableObjects[control_Num].Region.Left + 13,
                    0,
                    DraggableObjects[control_Num].Region.Left + 13,
                    10
                    );
                e.Graphics.DrawLine(
                    flagPen,
                    DraggableObjects[control_Num].Region.Left + DraggableObjects[control_Num].Region.Width + 6,
                    0,
                    DraggableObjects[control_Num].Region.Left + DraggableObjects[control_Num].Region.Width + 6,
                    10
                    );
            }
        }
    }

    public static void PrintPageType_Paint(object sender, PaintEventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        int _wnum = (int)Math.Ceiling((double)page_TypeFace.Page_Area.Width / 39);
        int _hnum = (int)Math.Ceiling((double)page_TypeFace.Page_Area.Height / 38);

        for (int t = 0; t < _hnum; t++)
        {
            for (int i = 0; i < _wnum; i++)
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(232, 232, 232), 1), new Rectangle(i * 39, 20 + t * 38, 39, 38));
                e.Graphics.DrawLine(new Pen(Color.FromArgb(248, 248, 248), 1), 19 + i * 39, 20 + t * 38 + 1, 19 + i * 39, 20 + t * 38 + 36);
            }
            e.Graphics.DrawLine(new Pen(Color.FromArgb(248, 248, 248), 1), 0, 38 + t * 38, page_TypeFace.Page_Area.Width, 38 + t * 38);
        }

        for (int i = 0; i < 3; i++)
        {
            DraggableBandObjects[i].Setimage = LinBand(
                DraggableBandObjects[i].Region.Width,
                DraggableBandObjects[i].Region.Height,
                DraggableBandObjects[i].Id,
                DraggableBandObjects[i].Region.Top,
                i
                );
            DraggableBandObjects[i].OnPaint(e);
        }

        if (control_Num > -1)
        {
            foreach (DraggableObject item in DraggableObjects)
            {
                item.OnPaint(e);
            }

            if (Show_Line) // 显示标尺
            {
                Pen flagPen = new Pen(Color.FromArgb(170, 170, 170), 1);
                flagPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                e.Graphics.DrawLine(
                    flagPen,
                    DraggableObjects[control_Num].Region.Left + 3,
                    0,
                    DraggableObjects[control_Num].Region.Left + 3,
                    DraggableObjects[control_Num].Region.Top
                    );
                e.Graphics.DrawLine(
                    flagPen,
                    DraggableObjects[control_Num].Region.Left + DraggableObjects[control_Num].Region.Width - 4,
                    0,
                    DraggableObjects[control_Num].Region.Left + DraggableObjects[control_Num].Region.Width - 4,
                    DraggableObjects[control_Num].Region.Top
                    );
                e.Graphics.DrawLine(
                    flagPen,
                    0,
                    DraggableObjects[control_Num].Region.Top + 3,
                    DraggableObjects[control_Num].Region.Left,
                    DraggableObjects[control_Num].Region.Top + 3
                    );
                e.Graphics.DrawLine(
                    flagPen,
                    0,
                    DraggableObjects[control_Num].Region.Bottom - 4,
                    DraggableObjects[control_Num].Region.Left,
                    DraggableObjects[control_Num].Region.Bottom - 4
                    );

                page_Install.Invalidate();
                page_Container.Invalidate();
            }
        }

        // 边距线
        e.Graphics.DrawRectangle(
            new Pen(Color.Pink, 1),
            new Rectangle(
                page_TypeFace.Page_Margin[0] - 1,
                page_TypeFace.Page_Margin[1] + 20,
                pL.Width - page_TypeFace.Page_Margin[2] - page_TypeFace.Page_Margin[0] + 1,
                pL.Height - page_TypeFace.Page_Margin[3] - page_TypeFace.Page_Margin[1] - 41
                )
        );
    }
}
