using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using static RbControls;
using static RbControls_CustomMouse;
using static Define_Design;
using static Define_DrawObject;
using static Define_Draggable;

public static class RBuild_MouseEvent
{
    private static Point A4location_XY;
    private static int periX = 0;
    private static int periY = 0;
    private static int sizeX = 0;
    private static int sizeY = 0;
    private static int sizeWidth = 0;
    private static int sizeHeight = 0;
    private static int band_state = -1;
    private static int primary_BandTop;
    private static int primary_BandBottom;
    public static int CursorFlag = -1;

    public static void PageType_Click(object sender, EventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        bool is_band = false;
        for (int i = 0; i < 3; i++)
        {
            is_band = false;
            if ((A4location_XY.X >= 0) && (A4location_XY.X <= Print_PageType.Width) &&
                (A4location_XY.Y >= DraggableBandObjects[i].Region.Top + 20) && (A4location_XY.Y <= DraggableBandObjects[i].Region.Bottom - 20))
            {
                is_band = true;
                break;
            }
        }

        if ((control_Type != -1) && (is_band))
        {
            if (control_Num > -1)
            {
                DraggableObjects[control_Num].Setimage = LinBox(
                    DraggableObjects[control_Num].Region.Width,
                    DraggableObjects[control_Num].Region.Height,
                    0,
                    DraggableObjects[control_Num].ControlType, control_Num
                    );
            }
            Point _Location = A4location_XY;

            control_Num = DraggableObjects.Count;

            if ((_Location.X + 51) > Print_PageType.Width) _Location.X = Print_PageType.Width - 51;
            if ((_Location.Y + 51) > DraggableBandObjects[band_Num].Region.Bottom - 20) _Location.Y = DraggableBandObjects[band_Num].Region.Bottom - 20 - 51;

            Draggable draggableBlock = new Draggable(_Location.X, _Location.Y, control_Type);
            DraggableObjects.Add(draggableBlock);

            int _width = 51;
            int _height = 51;

            DraggableObjects[control_Num].Belong_Band = band_Num;
            DraggableObjects[control_Num].Field_Align = "Left,Top";
            DraggableObjects[control_Num].Field_TextFont = "微软雅黑";
            DraggableObjects[control_Num].Field_ControlColor = Color.FromArgb(0, 0, 0);
            DraggableObjects[control_Num].Field_TextFontSize = 9;
            DraggableObjects[control_Num].Field_ImgZoom = 1;

            for (int i = 0; i < 8; i++) DraggableObjects[control_Num].Field_BoxLine[i] = false;
            DraggableObjects[control_Num].Setimage = LinBox(_width, _height, 1, control_Type, control_Num);
            control_Type = -1;
            Print_PageType.Invalidate();

            RBuild_Info.set_Info(DraggableObjects[control_Num].ControlType);

            Object_Record();
            ReportChange_Flag = true;
        }
    }

    public static void PageType_MouseClick(object sender, MouseEventArgs e)
    {

        for (int i = DraggableObjects.Count - 1; i >= 0; i--)
        {
            if (DraggableObjects[i].Region.Contains(e.Location))
            {
                if (i != control_Num)
                {
                    DraggableObjects[control_Num].Setimage = LinBox(
                        DraggableObjects[control_Num].Region.Width,
                        DraggableObjects[control_Num].Region.Height,
                        0,
                        DraggableObjects[control_Num].ControlType, control_Num
                        );
                    control_Num = i;
                    DraggableObjects[i].Setimage = LinBox(
                        DraggableObjects[i].Region.Width,
                        DraggableObjects[i].Region.Height,
                        1,
                        DraggableObjects[i].ControlType, i
                        );
                    Print_PageType.Invalidate();
                    RBuild_Info.set_Info(DraggableObjects[control_Num].ControlType);
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public static void PageType_MouseUp(object sender, MouseEventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        for (int i = 0; i < 3; i++) // 鼠标有可能超出范围延伸到其他band，所以判断全部，停止拖拽。
        {
            if ((DraggableBandObjects[i].IsDragging) || (DraggableBandObjects[i].IsSize))
            {
                DraggableBandObjects[i].IsDragging = false;
                DraggableBandObjects[i].IsSize = false;
                DraggableBandObjects[i].DraggingPoint = Point.Empty;

                var DraggableSync = DraggableObjects.Where(sync => sync.Belong_Band == i).ToList();
                for (int t = 0; t < DraggableSync.Count; t++)
                {
                    DraggableSync[t].Primary = DraggableSync[t].Region.Top;
                }
                break;
            }
        }

        if (band_state == 1) // 设置band高度，强制为内部组件靠下的组件的高度
        {
            if (DraggableBandObjects[0].Region.Bottom > DraggableBandObjects[1].Region.Top)
            {
                DraggableBandObjects[0].Region = new Rectangle(
                    0,
                    DraggableBandObjects[0].Region.Top,
                    DraggableBandObjects[0].Region.Width,
                    DraggableBandObjects[1].Region.Top - DraggableBandObjects[0].Region.Top
                    );
                pL.Invalidate();
            }

            if (DraggableBandObjects[1].Region.Bottom > DraggableBandObjects[2].Region.Top)
            {
                DraggableBandObjects[1].Region = new Rectangle(
                    0,
                    DraggableBandObjects[1].Region.Top,
                    DraggableBandObjects[1].Region.Width,
                    DraggableBandObjects[2].Region.Top - DraggableBandObjects[1].Region.Top
                    );
                pL.Invalidate();
            }

            if (DraggableBandObjects[2].Region.Bottom > Print_PageType.Bottom - 10)
            {
                DraggableBandObjects[2].Region = new Rectangle(
                    0,
                    DraggableBandObjects[2].Region.Top,
                    DraggableBandObjects[2].Region.Width,
                    Print_PageType.Bottom - 10 - DraggableBandObjects[2].Region.Top
                    );
                pL.Invalidate();
            }

            var DraggableMaxBottom = DraggableObjects.Where(bottom => bottom.Belong_Band == band_Num).OrderByDescending(sort => sort.Region.Bottom).ToList();

            if (DraggableMaxBottom.Count > 0)
            {
                int _maxHeight = 43;
                if (DraggableBandObjects[band_Num].Region.Bottom - DraggableMaxBottom[0].Region.Bottom < 20)
                {
                    _maxHeight = (DraggableMaxBottom[0].Region.Bottom - DraggableBandObjects[band_Num].Region.Top) + 20;

                    DraggableBandObjects[band_Num].Region = new Rectangle(
                        0,
                        DraggableBandObjects[band_Num].Region.Top,
                        DraggableBandObjects[band_Num].Region.Width,
                        _maxHeight
                        );
                    pL.Invalidate();
                }
            }
        }

        if (control_Num > -1)
        {
            if (DraggableObjects[control_Num].IsDragging)
            {
                DraggableObjects[control_Num].IsDragging = false;
                DraggableObjects[control_Num].DraggingPoint = Point.Empty;
                DraggableObjects[control_Num].Primary = DraggableObjects[control_Num].Region.Top;
                Object_Record();
                ReportChange_Flag = true;
            }
        }
    }

    public static void PageType_MouseDown(object sender, MouseEventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        if (band_state == 0)
        {
            DraggableBandObjects[band_Num].IsDragging = true;
            primary_BandTop = DraggableBandObjects[band_Num].Region.Top;
            primary_BandBottom = DraggableBandObjects[band_Num].Region.Bottom;
        }
        else
        if (band_state == 1)
        {
            DraggableBandObjects[band_Num].IsSize = true;
        }
        else
        if (control_Num > -1)
        {
            if (DraggableObjects[control_Num].Region.Contains(e.Location))
            {
                DraggableObjects[control_Num].IsDragging = true;
                DraggableObjects[control_Num].DraggingPoint = e.Location;
                if (e.Button == MouseButtons.Right)
                {
                    DraggableObjects[control_Num].IsDragging = false;
                    RBuild_Menu.Set_ControlMenuItem(pL, control_Num);
                    RBuild_Menu.control_Menu.Show(pL.PointToScreen(e.Location));
                }
                primary_BandTop = DraggableBandObjects[band_Num].Region.Top;
                primary_BandBottom = DraggableBandObjects[band_Num].Region.Bottom;
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    CursorFlag = 9;
                    RBuild_Menu.band_Menu.Show(pL.PointToScreen(e.Location));
                }
            }
        }
        else
        if (control_Num < 0)
        {
            if (e.Button == MouseButtons.Right)
            {
                RBuild_Menu.band_Menu.Show(pL.PointToScreen(e.Location));
            }
        }

        RBuild_Menu.Set_BandMenuItem();
    }

    public static void PageType_MouseMove(object sender, MouseEventArgs e)
    {
        PanelEx pL = (PanelEx)sender;

        A4location_XY = e.Location;

        for (int i = 0; i < 3; i++)
        {
            if (DraggableBandObjects[i].Region.Contains(e.Location))
            {
                band_Num = i;
                break;
            }
        }

        if (control_Type != -1)
        {
            if ((e.X >= 0) && (e.X <= Print_PageType.Width) &&
                (e.Y >= DraggableBandObjects[band_Num].Region.Y + 20) && (e.Y <= DraggableBandObjects[band_Num].Region.Bottom - 23))
            {
                pL.Cursor = custom_MouseCursor(control_icon[control_Type - 1], 5, 5);
            }
        }
        else
        {
            if ((control_Num > -1) && (band_state == -1))
            {
                if (DraggableObjects[control_Num].IsDragging)
                {
                    if (CursorFlag == 9)
                    {
                        int set_x = DraggableObjects[control_Num].Region.Left + e.X - DraggableObjects[control_Num].DraggingPoint.X;
                        int set_y = DraggableObjects[control_Num].Region.Top + e.Y - DraggableObjects[control_Num].DraggingPoint.Y;

                        if (set_x < 0) set_x = 0;
                        if ((set_x + DraggableObjects[control_Num].Region.Width) > Print_PageType.Width) set_x = Print_PageType.Width - DraggableObjects[control_Num].Region.Width;
                        if (set_y < primary_BandTop + 20) set_y = primary_BandTop + 20;
                        if ((set_y + DraggableObjects[control_Num].Region.Height) > (primary_BandBottom - 20)) set_y = (primary_BandBottom - 20) - DraggableObjects[control_Num].Region.Height;

                        DraggableObjects[control_Num].Region = new Rectangle(set_x, set_y, DraggableObjects[control_Num].Region.Width, DraggableObjects[control_Num].Region.Height);
                        DraggableObjects[control_Num].DraggingPoint = e.Location;
                    }
                    else
                    {
                        switch (CursorFlag)
                        {
                            case 0:
                                if (sizeWidth > 14) sizeX = e.Location.X;
                                if (sizeHeight > 14) sizeY = e.Location.Y;
                                if (sizeX >= 0) sizeWidth = periX - e.Location.X;
                                if (sizeY >= (primary_BandTop + 20)) sizeHeight = periY - e.Location.Y;
                                break;
                            case 1:
                                if (sizeWidth > 14) sizeX = e.Location.X;
                                sizeY = DraggableObjects[control_Num].Region.Top;
                                if (sizeX >= 0) sizeWidth = periX - e.Location.X;
                                sizeHeight = e.Location.Y - DraggableObjects[control_Num].Region.Top;
                                break;
                            case 2:
                                sizeX = DraggableObjects[control_Num].Region.Left;
                                if (sizeHeight > 14) sizeY = e.Location.Y;
                                sizeWidth = e.Location.X - DraggableObjects[control_Num].Region.Left;
                                if (sizeY >= (primary_BandTop + 20)) sizeHeight = periY - e.Location.Y;
                                break;
                            case 3:
                                sizeX = DraggableObjects[control_Num].Region.Left;
                                sizeY = DraggableObjects[control_Num].Region.Top;
                                sizeWidth = e.Location.X - DraggableObjects[control_Num].Region.Left;
                                sizeHeight = e.Location.Y - DraggableObjects[control_Num].Region.Top;
                                break;
                            case 4:
                                if (sizeWidth > 14) sizeX = e.Location.X;
                                sizeY = DraggableObjects[control_Num].Region.Top;
                                if (sizeX >= 0) sizeWidth = periX - e.Location.X;
                                sizeHeight = DraggableObjects[control_Num].Region.Height;
                                break;
                            case 5:
                                sizeX = DraggableObjects[control_Num].Region.Left;
                                sizeY = DraggableObjects[control_Num].Region.Top;
                                sizeWidth = e.Location.X - DraggableObjects[control_Num].Region.Left;
                                sizeHeight = DraggableObjects[control_Num].Region.Height;
                                break;
                            case 6:
                                sizeX = DraggableObjects[control_Num].Region.Left;
                                if (sizeHeight > 14) sizeY = e.Location.Y;
                                sizeWidth = DraggableObjects[control_Num].Region.Width;
                                if (sizeY >= (primary_BandTop + 20)) sizeHeight = periY - e.Location.Y;
                                break;
                            case 7:
                                sizeX = DraggableObjects[control_Num].Region.Left;
                                sizeY = DraggableObjects[control_Num].Region.Top;
                                sizeWidth = DraggableObjects[control_Num].Region.Width;
                                sizeHeight = e.Location.Y - DraggableObjects[control_Num].Region.Top;
                                break;
                            default:
                                break;
                        }
                        if (sizeWidth < 14) sizeWidth = 14;
                        if (sizeHeight < 14) sizeHeight = 14;

                        if (sizeX < 0) sizeX = 0;
                        if ((sizeX + sizeWidth) > Print_PageType.Width)
                        {
                            sizeWidth = Print_PageType.Width - sizeX;
                            sizeX = Print_PageType.Width - sizeWidth;
                        }
                        if (sizeY < (primary_BandTop + 20)) sizeY = primary_BandTop + 20;
                        if ((sizeHeight + DraggableObjects[control_Num].Region.Top) > (primary_BandBottom - 20)) sizeHeight = (primary_BandBottom - 20) - DraggableObjects[control_Num].Region.Top;

                        DraggableObjects[control_Num].Setimage = LinBox(sizeWidth, sizeHeight, 1, DraggableObjects[control_Num].ControlType, control_Num);
                        DraggableObjects[control_Num].Region = new Rectangle(sizeX, sizeY, sizeWidth, sizeHeight);
                    }
                    pL.Invalidate();
                    View_Info = true;
                }
                else
                {
                    if (DraggableObjects[control_Num].Region.Contains(e.Location))
                    {
                        SetCursorArea(control_Num);
                        for (int i = 0; i < 8; i++)
                        {
                            if ((e.X >= DraggableObjects[control_Num].Region.Left + CursorArea[i].X) && (e.X <= DraggableObjects[control_Num].Region.Left + CursorArea[i].X + 7) &&
                                (e.Y >= DraggableObjects[control_Num].Region.Top + CursorArea[i].Y) && (e.Y <= DraggableObjects[control_Num].Region.Top + CursorArea[i].Y + 7))
                            {
                                periX = DraggableObjects[control_Num].Region.Left + DraggableObjects[control_Num].Region.Width; // 记录原始位置
                                periY = DraggableObjects[control_Num].Region.Top + DraggableObjects[control_Num].Region.Height; // 记录原始位置

                                CursorFlag = i;
                                if ((i == 6) || (i == 7)) // 上下中
                                {
                                    pL.Cursor = Cursors.SizeNS;
                                    break;
                                }
                                if ((i == 0) || (i == 3)) // 左上右下
                                {
                                    pL.Cursor = Cursors.SizeNWSE;
                                    break;
                                }
                                if ((i == 1) || (i == 2)) // 左下右上
                                {
                                    pL.Cursor = Cursors.SizeNESW;
                                    break;
                                }
                                if ((i == 4) || (i == 5)) // 左右中
                                {
                                    pL.Cursor = Cursors.SizeWE;
                                    break;
                                }
                            }
                            pL.Cursor = Cursors.SizeAll;
                            CursorFlag = 9;
                        }
                    }
                    else
                    {
                        pL.Cursor = Cursors.Default;
                        CursorFlag = -1;
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            bool is_Draggin = true;
            if (CursorFlag == -1)
            {
                for (int i = 0; i < 3; i++)
                {
                    is_Draggin = true;
                    if ((DraggableBandObjects[i].IsDragging) || (DraggableBandObjects[i].IsSize))
                    {
                        is_Draggin = false;
                        break;
                    }
                }
                if (is_Draggin)
                {
                    band_state = -1;

                    if ((e.X >= Print_PageType.Width - 20) && (e.X <= Print_PageType.Width))
                    {
                        if ((e.Y >= DraggableBandObjects[band_Num].Region.Top) && (e.Y <= DraggableBandObjects[band_Num].Region.Top + 20))
                        {
                            band_state = 0;
                            DraggableBandObjects[band_Num].DraggingPoint = e.Location;
                            pL.Cursor = Cursors.SizeAll;
                        }
                        else
                        if ((e.Y <= DraggableBandObjects[band_Num].Region.Bottom) && (e.Y >= DraggableBandObjects[band_Num].Region.Bottom - 20))
                        {
                            band_state = 1;
                            DraggableBandObjects[band_Num].DraggingPoint = e.Location;
                            pL.Cursor = Cursors.SizeNS;
                        }
                    }
                }
            }

            if ((control_Num < 0) && (band_state == -1)) pL.Cursor = Cursors.Default;

            if (DraggableBandObjects[band_Num].IsDragging)
            {
                int set_y = DraggableBandObjects[band_Num].Region.Top + e.Y - DraggableBandObjects[band_Num].DraggingPoint.Y;

                if (band_Num == 0)
                {
                    if (set_y < 0) set_y = 0;
                    if (set_y + DraggableBandObjects[0].Region.Height > DraggableBandObjects[1].Region.Top) set_y = DraggableBandObjects[1].Region.Top - DraggableBandObjects[0].Region.Height;
                }
                else
                if (band_Num == 1)
                {
                    if (set_y < DraggableBandObjects[0].Region.Bottom) set_y = DraggableBandObjects[0].Region.Bottom;
                    if (set_y + DraggableBandObjects[1].Region.Height > DraggableBandObjects[2].Region.Top) set_y = DraggableBandObjects[2].Region.Top - DraggableBandObjects[1].Region.Height;
                }
                else
                if (band_Num == 2)
                {
                    if (set_y < DraggableBandObjects[1].Region.Bottom) set_y = DraggableBandObjects[1].Region.Bottom;
                    if (set_y + DraggableBandObjects[2].Region.Height > Print_PageType.Height) set_y = Print_PageType.Height - DraggableBandObjects[2].Region.Height;
                }

                DraggableBandObjects[band_Num].Region = new Rectangle(
                    0,
                    set_y,
                    DraggableBandObjects[band_Num].Region.Width,
                    DraggableBandObjects[band_Num].Region.Height
                    );
                DraggableBandObjects[band_Num].DraggingPoint = e.Location;

                int sync_y = (DraggableBandObjects[band_Num].Region.Top - primary_BandTop);
                if (control_Num > -1)
                {
                    var DraggableSync = DraggableObjects.Where(sync => sync.Belong_Band == band_Num).ToList();
                    for (int i = 0; i < DraggableSync.Count; i++)
                    {
                        DraggableSync[i].Region = new Rectangle(
                            DraggableSync[i].Region.Left,
                            DraggableSync[i].Primary + sync_y,
                            DraggableSync[i].Region.Width,
                            DraggableSync[i].Region.Height
                            );
                    }
                    View_Info = true;
                }
                pL.Invalidate();
            }
            else
            if (DraggableBandObjects[band_Num].IsSize)
            {
                int set_height = e.Location.Y - DraggableBandObjects[band_Num].Region.Top;
                if (set_height < 43) set_height = 43;

                DraggableBandObjects[band_Num].Region = new Rectangle(
                0,
                DraggableBandObjects[band_Num].Region.Top,
                DraggableBandObjects[band_Num].Region.Width,
                set_height
                );

                pL.Invalidate();
            }
        }
    }

}

