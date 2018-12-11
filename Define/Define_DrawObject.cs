using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using static Define_Draggable;
using static Define_System;
using static Define_Design;

public class Define_DrawObject
{
    /// <summary>
    /// 绘制横向标尺
    /// </summary>
    /// <param name="width">宽度</param>
    /// <returns></returns>
    public static Bitmap HRuler(int width)
    {
        Bitmap bmp_hr = new Bitmap(width, 20);

        Graphics ghr = Graphics.FromImage(bmp_hr);
        ghr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        for (int i = 0; i <= width; i += 5)
        {
            if (i % 25 != 0 && i % 50 != 0)
            {
                ghr.DrawLine(new Pen(new SolidBrush(Color.Black), 1), new Point(i+20, 0), new Point(i+20, 4));
            }

            if (i % 25 == 0 && i % 50 != 0)
            {
                ghr.DrawLine(new Pen(new SolidBrush(Color.Black), 1), new Point(i+20, 0), new Point(i+20, 7));
            }

            if (i % 50 == 0)
            {
                
                ghr.DrawLine(new Pen(new SolidBrush(Color.Black), 1), new Point(i+20, 0), new Point(i+20, 9));
                ghr.DrawString(i.ToString(),new Font("微软雅黑", 6, FontStyle.Regular), new SolidBrush(Color.FromArgb(50,50,50)),i+17,10);
            }

        }
        ghr.Dispose();
        return bmp_hr;
    }

    /// <summary>
    /// 绘制纵向标尺
    /// </summary>
    /// <param name="height">高度</param>
    /// <returns></returns>
    public static Bitmap VRuler(int height)
    {
        Bitmap bmp_vr = new Bitmap(20, height);
        Graphics gvr = Graphics.FromImage(bmp_vr);
        gvr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        for (int i = 0; i <= height; i += 5)
        {
            if (i % 25 != 0 && i % 50 != 0)
            {
                gvr.DrawLine(new Pen(new SolidBrush(Color.Black), 1), new Point(0, i+4), new Point(4, i+4));
            }

            if (i % 25 == 0 && i % 50 != 0)
            {
                gvr.DrawLine(new Pen(new SolidBrush(Color.Black), 1), new Point(0, i+4), new Point(7, i+4));
            }

            if (i % 50 == 0)
            {
                gvr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                gvr.DrawLine(new Pen(new SolidBrush(Color.Black), 1), new Point(0, i+4), new Point(9, i+4));
                Bitmap bmp_tvr = new Bitmap(50, 10);
                Graphics gtvr = Graphics.FromImage(bmp_tvr);
                gtvr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                gtvr.DrawString(i.ToString(), new Font("微软雅黑", 6, FontStyle.Regular), new SolidBrush(Color.FromArgb(50, 50, 50)), 0, 0);
                bmp_tvr.RotateFlip(RotateFlipType.Rotate270FlipNone);
                gvr.DrawImage(bmp_tvr, 10, -42+i);
                gtvr.Dispose();
            }
        }
        gvr.Dispose();
        return bmp_vr;
    }

    private static Bitmap[] page_flag = new Bitmap[3] { EzRBuild.EzResource.band_pt, EzRBuild.EzResource.band_pd, EzRBuild.EzResource.band_pb };

    /// <summary>
    /// 绘制栏目框
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="_flag">栏目类型</param>
    /// <returns></returns>
    public static Bitmap LinBand(int width, int height, int _flag, int ly, int _num)
    {
        string[] band_text = new string[3] { "页头", "内容", "页脚" };
        Pen blockPen = new Pen(Color.FromArgb(95, 166, 210), 1);
        blockPen.DashStyle = DashStyle.Dot;

        Bitmap _bmp = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(_bmp);

        g.DrawLine(blockPen, 0, 20, width, 20);
        g.DrawLine(blockPen, 0, height - 20 - 1, width, height - 20 - 1);

        g.FillRectangle(new SolidBrush(Color.FromArgb(115, 246, 246, 246)), new Rectangle(new Point(0, 21), new Size(width, height - 42)));

        g.DrawImage(page_flag[_flag], 0, 0, 20, 20);
        g.DrawImage(EzRBuild.EzResource.band_move, width - 20, 1, 20, 20);
        g.DrawImage(EzRBuild.EzResource.band_size, width - 20, height - 20 - 1, 20, 20);

        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        g.DrawString(
            band_text[_flag] + "-区域：" + width + "," + (height - 42) + "    位置：0," + (ly - (_flag * 43) + _flag),
            system_Font, new SolidBrush(Color.FromArgb(150, 150, 150)), 22, 2);

        g.DrawString(
            "组件数：" + DraggableObjects.Where(count => count.Belong_Band == _num).Count(),
            system_Font, new SolidBrush(Color.FromArgb(150, 150, 150)), width - 120, height - 19);
        g.Dispose();

        return _bmp;
    }


    /// <summary>
    /// 绘制组件框
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="selectType">是否选中</param>
    /// <param name="_controlType">组件类型</param>
    /// <param name="_controlNum">组件序号</param>
    /// <returns></returns>
    public static Bitmap LinBox(int width, int height, int selectType, int _controlType, int _controlNum)
    {
        Bitmap blockBox_Select = EzRBuild.EzResource.r1;
        Bitmap blockDot_Select = EzRBuild.EzResource.r2;
        Bitmap blockBox = EzRBuild.EzResource.r1s;
        Bitmap blockDot = EzRBuild.EzResource.r2s;

        Bitmap _bmp = new Bitmap(width, height);
        Graphics Block_Item = Graphics.FromImage(_bmp);
        Block_Item.PageUnit = GraphicsUnit.Pixel;

        Bitmap block, dot;

        Block_Item.FillRectangle(new SolidBrush(DraggableObjects[_controlNum].Field_BackColor), new Rectangle(0, 0, width, height));

        if (DraggableObjects[_controlNum].isContent)
        {
            if ((_controlType == 1) || (_controlType == 4) || (_controlType == 5) || (_controlType == 6)) Draw_Text(Block_Item, control_Num);
            if (_controlType == 2) Draw_Image(Block_Item, control_Num);
            if (_controlType == 3) Draw_Shape(Block_Item, control_Num);
        }

        Pen pen = new Pen(DraggableObjects[_controlNum].Field_LineColor, DraggableObjects[_controlNum].Field_LineThickness);
        pen.DashStyle = DraggableObjects[_controlNum].Field_LineType;
        if (DraggableObjects[_controlNum].Field_BoxLine[4]) { for (int i = 0; i < 8; i++) DraggableObjects[_controlNum].Field_BoxLine[i] = false; }
        if (DraggableObjects[_controlNum].Field_BoxLine[5]) { for (int i = 0; i < 4; i++) DraggableObjects[_controlNum].Field_BoxLine[i] = true; }
        if (DraggableObjects[_controlNum].Field_BoxLine[0]) Block_Item.DrawLine(pen, 3, height - 4, width - 4, height - 4);
        if (DraggableObjects[_controlNum].Field_BoxLine[1]) Block_Item.DrawLine(pen, 3, 3, width - 4, 3);
        if (DraggableObjects[_controlNum].Field_BoxLine[2]) Block_Item.DrawLine(pen, 3, 3, 3, height - 4);
        if (DraggableObjects[_controlNum].Field_BoxLine[3]) Block_Item.DrawLine(pen, width - 4, 3, width - 4, height - 4);
        if (DraggableObjects[_controlNum].Field_BoxLine[6]) Block_Item.DrawLine(pen, 3, 3, width - 4, height - 4);
        if (DraggableObjects[_controlNum].Field_BoxLine[7]) Block_Item.DrawLine(pen, width - 4, 3, 3, height - 4);

        Block_Item.DrawImage(control_icon[DraggableObjects[_controlNum].ControlType - 1], 1, 1, 20, 20);

        if (selectType == 0)
        {
            block = blockBox;
            dot = blockDot;
        }
        else
        {
            block = blockBox_Select;
            dot = blockDot_Select;
        }

        int set_midX = (width - 14 + 5) / 2;
        int set_midY = (height - 14 + 5) / 2;

        CursorArea[0] = new PointF(0, 0);
        CursorArea[1] = new PointF(0, height - 7);
        CursorArea[2] = new PointF(width - 7, 0);
        CursorArea[3] = new PointF(width - 7, height - 7);
        CursorArea[4] = new PointF(0, set_midY);
        CursorArea[5] = new PointF(width - 7, set_midY);
        CursorArea[6] = new PointF(set_midX, 0);
        CursorArea[7] = new PointF(set_midX, height - 7);

        for (int i = 0; i < 8; i++) Block_Item.DrawImage(block, CursorArea[i]);


        for (int i = 7; i < set_midY; i++)
        {
            if (i % 2 == 0)
            {
                Block_Item.DrawImage(dot, new PointF(3, i));
                Block_Item.DrawImage(dot, new PointF(width - 4, i));
            }
        }

        for (int i = set_midY + 7; i < height - 7; i++)
        {
            if (i % 2 == 0)
            {
                Block_Item.DrawImage(dot, new PointF(3, i));
                Block_Item.DrawImage(dot, new PointF(width - 4, i));
            }
        }

        for (int i = 7; i < set_midX; i++)
        {
            if (i % 2 == 0)
            {
                Block_Item.DrawImage(dot, new PointF(i, 3));
                Block_Item.DrawImage(dot, new PointF(i, height - 4));
            }
        }

        for (int i = set_midX + 7; i < width - 7; i++)
        {
            if (i % 2 == 0)
            {
                Block_Item.DrawImage(dot, new PointF(i, 3));
                Block_Item.DrawImage(dot, new PointF(i, height - 4));
            }
        }

        Block_Item.Dispose();
        return _bmp;
    }


    /// <summary>
    /// 设置绘制区域
    /// </summary>
    /// <param name="_controlNum">组件序号</param>
    public static void SetCursorArea(int _controlNum)
    {
        int set_midX = (DraggableObjects[_controlNum].Region.Width - 14 + 5) / 2;
        int set_midY = (DraggableObjects[_controlNum].Region.Height - 14 + 5) / 2;

        CursorArea[0] = new PointF(0, 0);
        CursorArea[1] = new PointF(0, DraggableObjects[_controlNum].Region.Height - 7);
        CursorArea[2] = new PointF(DraggableObjects[_controlNum].Region.Width - 7, 0);
        CursorArea[3] = new PointF(DraggableObjects[_controlNum].Region.Width - 7, DraggableObjects[_controlNum].Region.Height - 7);
        CursorArea[4] = new PointF(0, set_midY);
        CursorArea[5] = new PointF(DraggableObjects[_controlNum].Region.Width - 7, set_midY);
        CursorArea[6] = new PointF(set_midX, 0);
        CursorArea[7] = new PointF(set_midX, DraggableObjects[_controlNum].Region.Height - 7);
    }


    /// <summary>
    /// 绘制文字
    /// </summary>
    /// <param name="gString">画笔</param>
    /// <param name="_controlNum">组件序号</param>
    private static void Draw_Text(Graphics gString, int _controlNum)
    {
        if (DraggableObjects[_controlNum].isContent)
        {
            RbControls_DrawTextMethod DrawText = new RbControls_DrawTextMethod();

            string _txt = DraggableObjects[_controlNum].Field_Text;
            if (DraggableObjects[_controlNum].ControlType == 4) _txt = "[\"" + _txt + "\"]";

            Font fnt_Text = new Font(DraggableObjects[_controlNum].Field_TextFont, DraggableObjects[_controlNum].Field_TextFontSize, DraggableObjects[_controlNum].Field_TextFontStyle, GraphicsUnit.Pixel);
            Bitmap bmp_Text = new Bitmap(DraggableObjects[_controlNum].Region.Width - 7, DraggableObjects[_controlNum].Region.Height - 7);
            Graphics gText = Graphics.FromImage(bmp_Text);
            Point point_text = RBuild_Info.TextPoint(_controlNum, bmp_Text.Width, bmp_Text.Height, gText, _txt, fnt_Text);
            DrawText.DrawString(bmp_Text, _txt, fnt_Text, DraggableObjects[_controlNum].Field_ControlColor, new Rectangle(point_text.X, point_text.Y, bmp_Text.Width, bmp_Text.Height));
            DraggableObjects[_controlNum].Field_Img = bmp_Text;
            gString.DrawImage(DraggableObjects[_controlNum].Field_Img, 3, 3, bmp_Text.Width, bmp_Text.Height);
        }
    }

    /// <summary>
    /// 绘制图像
    /// </summary>
    /// <param name="gImage">画笔</param>
    /// <param name="_controlNum">组件序号</param>
    private static void Draw_Image(Graphics gImage, int _controlNum)
    {

        if (DraggableObjects[_controlNum].isContent)
        {
            if (DraggableObjects[_controlNum].Field_ImgZoom == 1)
            {
                gImage.DrawImage(DraggableObjects[_controlNum].Field_Img, 3, 3, DraggableObjects[_controlNum].Region.Width - 7, DraggableObjects[_controlNum].Region.Height - 7);
            }
            else
            {
                Point point_img = RBuild_Info.ImagePoint(_controlNum, DraggableObjects[_controlNum].Region.Width - 7, DraggableObjects[_controlNum].Region.Height - 7);
                var desRect = new Rectangle(point_img.X + 3, point_img.Y + 3, DraggableObjects[_controlNum].Region.Width - 7, DraggableObjects[_controlNum].Region.Height - 7);
                var srcRect = new Rectangle(0, 0, DraggableObjects[_controlNum].Region.Width - 7, DraggableObjects[_controlNum].Region.Height - 7);
                gImage.DrawImage(DraggableObjects[_controlNum].Field_Img, desRect, srcRect, GraphicsUnit.Pixel);
            }
        }
    }

    /// <summary>
    /// 绘制形状
    /// </summary>
    /// <param name="gShap">画笔</param>
    /// <param name="_controlNum">组件序号</param>
    private static void Draw_Shape(Graphics gShap, int _controlNum)
    {
        if (DraggableObjects[_controlNum].isContent)
        {
            RbControls_DrawTextMethod ds = new RbControls_DrawTextMethod();

            Bitmap _bmp = new Bitmap(DraggableObjects[_controlNum].Region.Width, DraggableObjects[_controlNum].Region.Height);

            int _size = 1;
            int _sizeX = 0;
            int _sizeY = 0;
            if (DraggableObjects[_controlNum].Region.Width > DraggableObjects[_controlNum].Region.Height)
            {
                _size = DraggableObjects[_controlNum].Region.Height;
                _sizeX = (DraggableObjects[_controlNum].Region.Width - _size) / 2;
                _sizeY = 0;
            }
            else
            {
                _size = DraggableObjects[_controlNum].Region.Width;
                _sizeX = 0;
                _sizeY = (DraggableObjects[_controlNum].Region.Height - _size) / 2;
            }

            ds.DrawFontAwesome(
                _bmp,
                shaps_type[DraggableObjects[_controlNum].Field_Shape],
                _size - 6,
                DraggableObjects[_controlNum].Field_ControlColor,
                new Point(_sizeX, _sizeY),
                false
                );
            gShap.DrawImage(_bmp, new PointF(6, 6));
        }
    }
}

