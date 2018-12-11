using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Drawing.Printing;
using static Define_Design;
using static Define_Draggable;
using static Define_DataLink;
using static Define_PrintPage;

public static class Define_ReportFunction
{
    public static int Total_Page = 0; // 总页数
    private static int listRecord = 0; // 每页记录数
    private static int bandHeight; // 内容栏目高度
    private static int drawHeight; // 总高度

    private static int _pTotal_Page = 0; // 打印总页数
    public static int _pnum = 0; // 数据库打印页计数
    public static int _pnumNodata = 0; // 单页表打印计数

    /// <summary>
    /// 预览页面数组
    /// </summary>
    public class Report_Page
    {
        public int pageNum;
        public Bitmap pageImage;

        public Report_Page(int _num, Bitmap _img)
        {
            pageNum = _num;
            pageImage = _img;
        }
    }
    public static List<Report_Page> report_Pages = new List<Report_Page>();

    public static RbControls_DrawTextMethod DrawText = new RbControls_DrawTextMethod();

    // 查找文字结果
    public class Searchs
    {
        public RbControls_TransparentRect _searchRect = new RbControls_TransparentRect();
        public Searchs(RbControls_TransparentRect _search)
        {
            _searchRect = _search;
        }
    }
    public static List<Searchs> setSearchs = new List<Searchs>();

    /// <summary>
    /// 生成报表
    /// </summary>
    /// <param name="_pageType">页面类型</param>
    public static void Report_generation(int _pageType)
    {
        Bitmap pgImage;

        report_Pages.Clear();

        if (DraggableObjects.Count > 0)
        {
            Total_Page = 1;
            report_Pages.Clear();

            var hasData = DraggableObjects.FindAll(data => data.ControlType == 4).Count();
            if ((hasData > 0) && (data_Pool.data_Type != 0))
            {
                OpenLink(data_Pool.data_Type);
                Set_DataTable(data_Pool.data_Type);
                int recordCount = dt.Rows.Count;

                bandHeight = DraggableBandObjects[1].Region.Height - 42 - 5;
                drawHeight = (DraggableBandObjects[2].Region.Bottom + 20 - 1 - DraggableBandObjects[2].Region.Height - 126) - (DraggableBandObjects[0].Region.Bottom - 60);
                listRecord = drawHeight / bandHeight;

                Total_Page = (int)Math.Ceiling((double)recordCount / listRecord);
                _pTotal_Page = Total_Page;
            }

            for (int i = 0; i < Total_Page; i++)
            {
                pgImage = new Bitmap((int)(PreViewPage_Area.Width * zoomScale), (int)(PreViewPage_Area.Height * zoomScale));
                pgImage.SetResolution(96, 96); // 设置图像dpi为96

                // 页头
                var ReportHead = DraggableObjects.FindAll(head => head.Belong_Band == 0).ToList();
                Draw_Control(pgImage, 0, i, ReportHead);

                //内容
                var ReportContent = DraggableObjects.FindAll(content => content.Belong_Band == 1).ToList();
                var HasData = ReportContent.FindAll(hd => hd.ControlType == 4).Count();
                if (HasData > 0) Draw_DataControl(pgImage, i, ReportContent);
                else Draw_Control(pgImage, 1, i, ReportContent);

                // 页脚
                var ReportFooter = DraggableObjects.FindAll(footer => footer.Belong_Band == 2).ToList();
                Draw_Control(pgImage, 2, i, ReportFooter);

                report_Pages.Add(new Report_Page(i, pgImage));
            }
            if (data_Pool.data_Type != -1) CloseLink(data_Pool.data_Type);
        }
    }

    /// <summary>
    /// 查找文本
    /// </summary>
    /// <param name="_search">包含文字</param>
    public static void search_Text(string _search)
    {
        setSearchs.Clear();
        var objects = DraggableObjects.FindAll(head => head.Belong_Band == 1).ToList();
        var HasData = objects.FindAll(hd => hd.ControlType == 4).Count();
        if (HasData > 0)
        {
            for (int _num = 0; _num < Total_Page; _num++)  //////第几页？
            {
                for (int p = 0; p < listRecord; p++)
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i].isContent)
                        {
                            string _txt = "";
                            int _record = (_num * listRecord) + p;
                            if (_record > dt.Rows.Count - 1) break;

                            if ((objects[i].ControlType == 1) || (objects[i].ControlType == 4) || (objects[i].ControlType == 5))
                            {
                                if (objects[i].ControlType == 1) _txt = objects[i].Field_Text;
                                if (objects[i].ControlType == 4) _txt = dt.Rows[_record][objects[i].Field_Text].ToString();
                                if (objects[i].ControlType == 5)
                                {
                                    _txt = objects[i].Field_Text.Replace("xx", Total_Page.ToString());
                                    _txt = _txt.Replace("x", (_num + 1).ToString());
                                }
                            }
                            // 找到文字，颜色覆盖
                            if (_txt.IndexOf(_search) > -1)
                            {
                                RbControls_TransparentRect _SearchRect = new RbControls_TransparentRect();
                                _SearchRect.BackColor = Color.Red; //颜色
                                _SearchRect.Radius = 1; //圆角 角度
                                _SearchRect.ShapeBorderStyle = RbControls_TransparentRect.ShapeBorderStyles.ShapeBSNone; //边框
                                _SearchRect.Size = new Size((int)Math.Ceiling((objects[i].Region.Width - 6) * zoomScale), (int)Math.Ceiling((objects[i].Region.Height - 6) * zoomScale)); //大小
                                int _top = (DraggableBandObjects[0].Region.Bottom - 60) + (objects[i].Region.Top - DraggableBandObjects[1].Region.Top);
                                _SearchRect.Location = new Point((int)((objects[i].Region.Left + 3) * zoomScale), (int)((_top + p * bandHeight) * zoomScale)); //位置
                                _SearchRect.Opacity = 120; //透明度
                                RBuild_Preview.panel_Page[_num].Controls.Add(_SearchRect);
                                setSearchs.Add(new Searchs(_SearchRect));
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].isContent)
                {
                    string _txt = "";
                    if (objects[i].ControlType == 1)
                    {
                        if (objects[i].ControlType == 1) _txt = objects[i].Field_Text;
                    }
                    // 找到文字，颜色覆盖
                    if (_txt.IndexOf(_search) > -1)
                    {
                        RbControls_TransparentRect _SearchRect = new RbControls_TransparentRect();
                        _SearchRect.BackColor = Color.Red; //颜色
                        _SearchRect.Radius = 1; //圆角 角度
                        _SearchRect.ShapeBorderStyle = RbControls_TransparentRect.ShapeBorderStyles.ShapeBSNone; //边框
                        _SearchRect.Size = new Size((int)Math.Ceiling((objects[i].Region.Width - 6) * zoomScale), (int)Math.Ceiling((objects[i].Region.Height - 6) * zoomScale)); //大小
                        int _top = (DraggableBandObjects[0].Region.Bottom - 60) + (objects[i].Region.Top - DraggableBandObjects[1].Region.Top);
                        _SearchRect.Location = new Point((int)((objects[i].Region.Left + 3) * zoomScale), (int)(_top * zoomScale)); //位置
                        _SearchRect.Opacity = 120; //透明度
                        RBuild_Preview.panel_Page[0].Controls.Add(_SearchRect);
                        setSearchs.Add(new Searchs(_SearchRect));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 数据库写入预览
    /// </summary>
    /// <param name="_pgbmp">写入的图像</param>
    /// <param name="_num">序号</param>
    /// <param name="objects">数组</param>
    private static void Draw_DataControl(Bitmap _pgbmp, int _num, List<DraggableObject> objects)
    {
        for (int p = 0; p < listRecord; p++)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].isContent)
                {
                    string _txt = "";
                    int _record = (_num * listRecord) + p;
                    if (_record > dt.Rows.Count - 1) break;

                    Bitmap _bmp = new Bitmap((int)Math.Ceiling((objects[i].Region.Width - 6) * zoomScale), (int)Math.Ceiling((objects[i].Region.Height - 6) * zoomScale));
                    Graphics gControl = Graphics.FromImage(_bmp);

                    gControl.FillRectangle(new SolidBrush(objects[i].Field_BackColor), new Rectangle(0, 0, _bmp.Width, _bmp.Height));

                    if ((objects[i].ControlType == 1) || (objects[i].ControlType == 4) || (objects[i].ControlType == 5) || (objects[i].ControlType == 6))
                    {
                        if (objects[i].ControlType == 1) _txt = objects[i].Field_Text;
                        if (objects[i].ControlType == 4) _txt = dt.Rows[_record][objects[i].Field_Text].ToString();
                        if (objects[i].ControlType == 5)
                        {
                            _txt = objects[i].Field_Text.Replace("xx", Total_Page.ToString());
                            _txt = _txt.Replace("x", (_num + 1).ToString());
                        }
                        if (objects[i].ControlType == 6)
                        {
                            _txt = objects[i].Field_Calculate;
                            _txt = _txt.Replace("[", "").Replace("\"]", "");
                            string[] splitString = _txt.Split(new string[] { "\"" }, StringSplitOptions.None);
                            _txt = Field_Calculate(splitString[0], splitString[1]);
                        }
                        Font fnt_Text = new Font(objects[i].Field_TextFont, (int)Math.Ceiling(objects[i].Field_TextFontSize * zoomScale), objects[i].Field_TextFontStyle);
                        Graphics gText = Graphics.FromImage(_bmp);
                        Point point_text = Text_point(objects[i].Field_Align, _bmp.Width - 1, _bmp.Height - 1, gText, _txt, fnt_Text);
                        DrawText.DrawString(_bmp, _txt, fnt_Text, objects[i].Field_ControlColor, new Rectangle((int)(point_text.X * zoomScale), (int)(point_text.Y * zoomScale), _bmp.Width - 1, _bmp.Height - 1));
                    }
                    if (objects[i].ControlType == 2)
                    {
                        if (objects[i].Field_ImgZoom == 1)
                        {
                            gControl.DrawImage(objects[i].Field_Img, 0, 0, (int)((objects[i].Region.Width - 7) * zoomScale), (int)((objects[i].Region.Height - 7) * zoomScale));
                        }
                        else
                        {
                            Point point_img = Image_point(objects[i], objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                            var Img_desRect = new Rectangle((int)(point_img.X * zoomScale), (int)(point_img.Y * zoomScale), (int)((objects[i].Region.Width - 7) * zoomScale), (int)((objects[i].Region.Height - 7) * zoomScale));
                            var Img_srcRect = new Rectangle(0, 0, objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                            gControl.DrawImage(objects[i].Field_Img, Img_desRect, Img_srcRect, GraphicsUnit.Pixel);
                        }
                    }
                    if (objects[i].ControlType == 3)
                    {
                        int _size = 1, _sizeX = 0, _sizeY = 0;
                        if (objects[i].Region.Width > objects[i].Region.Height)
                        {
                            _size = objects[i].Region.Height;
                            _sizeX = (objects[i].Region.Width - _size) / 2;
                            _sizeY = 0;
                        }
                        else
                        {
                            _size = objects[i].Region.Width;
                            _sizeX = 0;
                            _sizeY = (objects[i].Region.Height - _size) / 2;
                        }
                        DrawText.DrawFontAwesome(_bmp, shaps_type[objects[i].Field_Shape], (int)Math.Ceiling((_size - 3) * zoomScale), objects[i].Field_ControlColor, new Point((int)(_sizeX * zoomScale), (int)(_sizeY * zoomScale)), false);
                        gControl.DrawImage(_bmp, new Point(0, 0));
                    }

                    Pen pen = new Pen(objects[i].Field_LineColor, objects[i].Field_LineThickness);
                    pen.DashStyle = objects[i].Field_LineType;
                    if (objects[i].Field_BoxLine[5]) for (int t = 0; t < 4; t++) objects[i].Field_BoxLine[t] = true;
                    if (objects[i].Field_BoxLine[0]) gControl.DrawLine(pen, 0, _bmp.Height - 1, _bmp.Width - 1, _bmp.Height - 1);
                    if (objects[i].Field_BoxLine[1]) gControl.DrawLine(pen, 0, 0, _bmp.Width - 1, 0);
                    if (objects[i].Field_BoxLine[2]) gControl.DrawLine(pen, 0, 0, 0, _bmp.Height - 1);
                    if (objects[i].Field_BoxLine[3]) gControl.DrawLine(pen, _bmp.Width - 1, 0, _bmp.Width - 1, _bmp.Height - 1);
                    if (objects[i].Field_BoxLine[6]) gControl.DrawLine(pen, 0, 0, _bmp.Width - 1, _bmp.Height - 1);
                    if (objects[i].Field_BoxLine[7]) gControl.DrawLine(pen, _bmp.Width - 1, 0, 0, _bmp.Height - 1);

                    Graphics pgImg = Graphics.FromImage(_pgbmp);
                    int _top = (DraggableBandObjects[0].Region.Bottom - 60) + (objects[i].Region.Top - DraggableBandObjects[1].Region.Top);
                    var desRect = new Rectangle((int)((objects[i].Region.Left + 3) * zoomScale), (int)((_top + p * bandHeight) * zoomScale), _bmp.Width, _bmp.Height);
                    var srcRect = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
                    pgImg.DrawImage(_bmp, desRect, srcRect, GraphicsUnit.Pixel);
                }
            }
        }
    }

    /// <summary>
    /// 非数据库写入预览
    /// </summary>
    /// <param name="_pgbmp">写入的图像</param>
    /// <param name="_type">band类型</param>
    /// <param name="_num">序号</param>
    /// <param name="objects">数组</param>
    private static void Draw_Control(Bitmap _pgbmp, int _type, int _num, List<DraggableObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i].isContent)
            {
                string _txt = "";

                Bitmap _bmp = new Bitmap((int)Math.Ceiling((objects[i].Region.Width - 6) * zoomScale), (int)Math.Ceiling((objects[i].Region.Height - 6) * zoomScale));
                Graphics gControl = Graphics.FromImage(_bmp);
                gControl.PageUnit = GraphicsUnit.Pixel;

                gControl.FillRectangle(new SolidBrush(objects[i].Field_BackColor), new Rectangle(0, 0, _bmp.Width, _bmp.Height));

                if ((objects[i].ControlType == 1) || (objects[i].ControlType == 4) || (objects[i].ControlType == 5) || (objects[i].ControlType == 6))
                {
                    if (objects[i].ControlType == 1) _txt = objects[i].Field_Text;
                    if (objects[i].ControlType == 4) _txt = dt.Rows[0][objects[i].Field_Text].ToString();
                    if (objects[i].ControlType == 5)
                    {
                        _txt = objects[i].Field_Text.Replace("xx", Total_Page.ToString());
                        _txt = _txt.Replace("x", (_num + 1).ToString());
                    }

                    if (objects[i].ControlType == 6)
                    {
                        _txt = objects[i].Field_Calculate;
                        _txt = _txt.Replace("[", "").Replace("\"]", "");
                        string[] splitString = _txt.Split(new string[] { "\"" }, StringSplitOptions.None);
                        _txt = Field_Calculate(splitString[0], splitString[1]);
                    }

                    Font fnt_Text = new Font(objects[i].Field_TextFont, (int)Math.Ceiling(objects[i].Field_TextFontSize * zoomScale), objects[i].Field_TextFontStyle, GraphicsUnit.Pixel);
                    Graphics gText = Graphics.FromImage(_bmp);
                    Point point_text = Text_point(objects[i].Field_Align, _bmp.Width - 1, _bmp.Height - 1, gText, _txt, fnt_Text);
                    DrawText.DrawString(_bmp, _txt, fnt_Text, objects[i].Field_ControlColor, new Rectangle((int)(point_text.X * zoomScale), (int)(point_text.Y * zoomScale), _bmp.Width - 1, _bmp.Height - 1));
                }
                if (objects[i].ControlType == 2)
                {
                    if (objects[i].Field_ImgZoom == 1)
                    {
                        gControl.DrawImage(objects[i].Field_Img, 0, 0, (int)((objects[i].Region.Width - 7) * zoomScale), (int)((objects[i].Region.Height - 7) * zoomScale));
                    }
                    else
                    {
                        Point point_img = Image_point(objects[i], objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                        var Img_desRect = new Rectangle((int)(point_img.X * zoomScale), (int)(point_img.Y * zoomScale), (int)((objects[i].Region.Width - 7) * zoomScale), (int)((objects[i].Region.Height - 7) * zoomScale));
                        var Img_srcRect = new Rectangle(0, 0, objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                        gControl.DrawImage(objects[i].Field_Img, Img_desRect, Img_srcRect, GraphicsUnit.Pixel);
                    }
                }
                if (objects[i].ControlType == 3)
                {
                    int _size = 1, _sizeX = 0, _sizeY = 0;
                    if (objects[i].Region.Width > objects[i].Region.Height)
                    {
                        _size = objects[i].Region.Height;
                        _sizeX = (objects[i].Region.Width - _size) / 2;
                        _sizeY = 0;
                    }
                    else
                    {
                        _size = objects[i].Region.Width;
                        _sizeX = 0;
                        _sizeY = (objects[i].Region.Height - _size) / 2;
                    }
                    DrawText.DrawFontAwesome(_bmp, shaps_type[objects[i].Field_Shape], (int)Math.Ceiling((_size - 3) * zoomScale), objects[i].Field_ControlColor, new Point((int)(_sizeX * zoomScale), (int)(_sizeY * zoomScale)), false);
                    gControl.DrawImage(_bmp, new Point(0, 0));
                }

                Pen pen = new Pen(objects[i].Field_LineColor, objects[i].Field_LineThickness);
                pen.DashStyle = objects[i].Field_LineType;
                if (objects[i].Field_BoxLine[5]) for (int t = 0; t < 4; t++) objects[i].Field_BoxLine[t] = true;
                if (objects[i].Field_BoxLine[0]) gControl.DrawLine(pen, 0, _bmp.Height - 1, _bmp.Width - 1, _bmp.Height - 1);
                if (objects[i].Field_BoxLine[1]) gControl.DrawLine(pen, 0, 0, _bmp.Width - 1, 0);
                if (objects[i].Field_BoxLine[2]) gControl.DrawLine(pen, 0, 0, 0, _bmp.Height - 1);
                if (objects[i].Field_BoxLine[3]) gControl.DrawLine(pen, _bmp.Width - 1, 0, _bmp.Width - 1, _bmp.Height - 1);
                if (objects[i].Field_BoxLine[6]) gControl.DrawLine(pen, 0, 0, _bmp.Width - 1, _bmp.Height - 1);
                if (objects[i].Field_BoxLine[7]) gControl.DrawLine(pen, _bmp.Width - 1, 0, 0, _bmp.Height - 1);

                Graphics pgImg = Graphics.FromImage(_pgbmp);

                int _top = 0;
                if (_type == 0) _top = objects[i].Region.Top - 20 + 7; //_top += 4;
                if (_type == 1) _top = (DraggableBandObjects[0].Region.Bottom - 60) + (objects[i].Region.Top - DraggableBandObjects[1].Region.Top);
                if (_type == 2) _top = (DraggableBandObjects[2].Region.Bottom + 20 - 1 - DraggableBandObjects[2].Region.Height - 126) + (objects[i].Region.Top - DraggableBandObjects[2].Region.Top);

                var desRect = new Rectangle((int)((objects[i].Region.Left + 3) * zoomScale), (int)(_top * zoomScale), _bmp.Width, _bmp.Height);
                var srcRect = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
                pgImg.DrawImage(_bmp, desRect, srcRect, GraphicsUnit.Pixel);
            }
        }
    }

    /// <summary>
    /// 文字对齐方式位置确定
    /// </summary>
    /// <param name="_align">对齐方式字符串</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="g">画笔</param>
    /// <param name="text">文字</param>
    /// <param name="font_family">文字样式</param>
    /// <returns></returns>
    private static Point Text_point(string _align, int width, int height, Graphics g, string text, Font font_family)
    {
        int _lx = 0, _ly = 0;
        SizeF sizeF = g.MeasureString(text, font_family);

        string[] splitString = _align.Split(new string[] { "," }, StringSplitOptions.None);
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

    /// <summary>
    /// 图像对齐方式位置确定
    /// </summary>
    /// <param name="obj">数组</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns></returns>
    public static Point Image_point(DraggableObject obj, int width, int height)
    {
        int _lx = 0, _ly = 0;

        string[] splitString = obj.Field_Align.Split(new string[] { "," }, StringSplitOptions.None);
        for (int i = 0; i < splitString.Length; i++)
        {
            if (splitString[i] == "Vertical") _ly = (height - obj.Field_Img.Height) / 2;
            if (splitString[i] == "Middle") _lx = (width - obj.Field_Img.Width) / 2;
            if (splitString[i] == "Left") _lx = 0;
            if (splitString[i] == "Right") _lx = width - obj.Field_Img.Width;
            if (splitString[i] == "Top") _ly = 0;
            if (splitString[i] == "Bottom") _ly = height - obj.Field_Img.Height;
        }
        if (_lx < 0) _lx = 0;
        if (_ly < 0) _ly = 0;
        Point point = new Point(_lx, _ly);
        return point;
    }

    /// <summary>
    /// 打印功能
    /// </summary>
    public static void print_File()
    {
        /////////printDocument = new PrintDocument();
        _pnum = 0;
        _pnumNodata = 0;

        PaperSize pSize;
        if (_pgselect != -1)
        {
            pSize = new PaperSize(page_types[_pgselect] + " page", (int)(PreViewPage_Area.Width / 25.4f * 100f), (int)(PreViewPage_Area.Height / 25.4f * 100f));

            if (page_TypeFace.Page_Direction == 0)
            {
                printDocument.DefaultPageSettings.Landscape = false; // false纵向
            }
            else
            {
                printDocument.DefaultPageSettings.Landscape = true;
            }
        }
        else
        {
            if (PreViewPage_Area.Width > PreViewPage_Area.Height) printDocument.DefaultPageSettings.Landscape = true;
            else printDocument.DefaultPageSettings.Landscape = false; // false纵向

            pSize = new PaperSize("Custom page", (int)(PreViewPage_Area.Width / 25.4f * 100f), (int)(PreViewPage_Area.Height / 25.4f * 100f));
        }

        printDocument.DefaultPageSettings.PaperSize = pSize;
        printDocument.PrintPage += PrintDocument_PrintPage;

        new RBuild_Print().Print();
        printDocument.Dispose();

    }


    /// <summary>
    /// 输出到打印机
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
        var hasData = DraggableObjects.FindAll(content => content.Belong_Band == 1 && content.ControlType == 4).Count();

        if (hasData < 1) // 没有数据库打印
        {
            noData_Pring(e);
        }
        else // 包含数据库打印
        {
            hasData_Pring(e);
        }
    }

    // 无数据库打印
    private static void noData_Pring(PrintPageEventArgs e)
    {
        var objects = DraggableObjects.FindAll(head => head.isContent != false).ToList();

        for (int i = 0; i < objects.Count; i++)
        {
            string _txt = "";
            Size contrl_Size = new Size(objects[i].Region.Width - 6, objects[i].Region.Height - 6);

            int _top = 0;
            if (objects[i].Belong_Band == 0) _top = objects[i].Region.Top - 20 + 7; //_top += 4;
            if (objects[i].Belong_Band == 1) _top = (DraggableBandObjects[0].Region.Bottom - 60) + (objects[i].Region.Top - DraggableBandObjects[1].Region.Top);
            if (objects[i].Belong_Band == 2) _top = (DraggableBandObjects[2].Region.Bottom + 20 - 1 - DraggableBandObjects[2].Region.Height - 126) + (objects[i].Region.Top - DraggableBandObjects[2].Region.Top);

            e.Graphics.FillRectangle(new SolidBrush(objects[i].Field_BackColor), new Rectangle(objects[i].Region.Left + 3, _top, contrl_Size.Width, contrl_Size.Height));

            if ((objects[i].ControlType == 1) || (objects[i].ControlType == 4) || (objects[i].ControlType == 5) || (objects[i].ControlType == 6))
            {

                if (objects[i].ControlType == 1) _txt = objects[i].Field_Text;
                if (objects[i].ControlType == 4) _txt = dt.Rows[0][objects[i].Field_Text].ToString();
                if (objects[i].ControlType == 5)
                {
                    _txt = objects[i].Field_Text.Replace("xx", Total_Page.ToString());
                    _txt = _txt.Replace("x", "1");
                }
                if (objects[i].ControlType == 6)
                {
                    _txt = objects[i].Field_Calculate;
                    _txt = _txt.Replace("[", "").Replace("\"]", "");
                    string[] splitString = _txt.Split(new string[] { "\"" }, StringSplitOptions.None);
                    _txt = Field_Calculate(splitString[0], splitString[1]);
                }

                Font fnt_Text = new Font(objects[i].Field_TextFont, objects[i].Field_TextFontSize, objects[i].Field_TextFontStyle, GraphicsUnit.Pixel);
                Point point_text = Text_point(objects[i].Field_Align, contrl_Size.Width - 1, contrl_Size.Height - 1, e.Graphics, _txt, fnt_Text);
                DrawText.printDrawString(e.Graphics, _txt, fnt_Text, objects[i].Field_ControlColor, new Rectangle(point_text.X + objects[i].Region.Left + 3, point_text.Y + _top, contrl_Size.Width - 1, contrl_Size.Height - 1));
            }
            if (objects[i].ControlType == 2)
            {
                if (objects[i].Field_ImgZoom == 1)
                {
                    e.Graphics.DrawImage(objects[i].Field_Img, objects[i].Region.Left + 3, _top, objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                }
                else
                {
                    Point point_img = Image_point(objects[i], objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                    var Img_desRect = new Rectangle(point_img.X + objects[i].Region.Left + 3, point_img.Y + _top, objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                    var Img_srcRect = new Rectangle(0, 0, objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                    e.Graphics.DrawImage(objects[i].Field_Img, Img_desRect, Img_srcRect, GraphicsUnit.Pixel);
                }
            }
            if (objects[i].ControlType == 3)
            {
                int _size = 1, _sizeX = 0, _sizeY = 0;
                if (objects[i].Region.Width > objects[i].Region.Height)
                {
                    _size = objects[i].Region.Height;
                    _sizeX = (objects[i].Region.Width - _size) / 2;
                    _sizeY = 0;
                }
                else
                {
                    _size = objects[i].Region.Width;
                    _sizeX = 0;
                    _sizeY = (objects[i].Region.Height - _size) / 2;
                }
                Bitmap _bmp = new Bitmap(_size * 2, _size * 2);
                DrawText.DrawFontAwesome(_bmp, shaps_type[objects[i].Field_Shape], _size - 3, objects[i].Field_ControlColor, new Point(_sizeX, _sizeY), false);
                e.Graphics.DrawImage(_bmp, new Point(objects[i].Region.Left + 3, _top));
                _bmp.Dispose();
            }
            Pen pen = new Pen(objects[i].Field_LineColor, objects[i].Field_LineThickness);
            pen.DashStyle = objects[i].Field_LineType;
            if (objects[i].Field_BoxLine[5]) for (int t = 0; t < 4; t++) objects[i].Field_BoxLine[t] = true;
            if (objects[i].Field_BoxLine[0]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3, _top + contrl_Size.Height, objects[i].Region.Left + 3 + contrl_Size.Width, _top + contrl_Size.Height);
            if (objects[i].Field_BoxLine[1]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3, _top, objects[i].Region.Left + 3 + contrl_Size.Width, _top);
            if (objects[i].Field_BoxLine[2]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3, _top, objects[i].Region.Left + 3, _top + contrl_Size.Height);
            if (objects[i].Field_BoxLine[3]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3 + contrl_Size.Width, _top, objects[i].Region.Left + 3 + contrl_Size.Width, _top + contrl_Size.Height);
            if (objects[i].Field_BoxLine[6]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3, _top, objects[i].Region.Left + 3 + contrl_Size.Width - 1, _top + contrl_Size.Height - 1);
            if (objects[i].Field_BoxLine[7]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3 + contrl_Size.Width - 1, _top, objects[i].Region.Left + 3, _top + contrl_Size.Height - 1);
        }

        _pnumNodata += 1;
        if (_pnumNodata > page_Scope.Count - 1) e.HasMorePages = false;
        else e.HasMorePages = true;
    }

    // 数据库打印
    private static void hasData_Pring(PrintPageEventArgs e)
    {
        var objects = DraggableObjects.FindAll(head => head.isContent != false).ToList();

        for (int p = 0; p < listRecord; p++)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].isContent)
                {
                    string _txt = "";
                    int _record = ((page_Scope[_pnum] - 1) * listRecord) + p;
                    if (_record > dt.Rows.Count - 1) break;

                    Size contrl_Size = new Size(objects[i].Region.Width - 6, objects[i].Region.Height - 6);
                    int _top = 0;
                    if (objects[i].Belong_Band == 0) _top = objects[i].Region.Top - 20 + 7; //_top += 4;
                    if (objects[i].Belong_Band == 1) _top = (DraggableBandObjects[0].Region.Bottom - 60) + (objects[i].Region.Top - DraggableBandObjects[1].Region.Top);
                    if (objects[i].Belong_Band == 2) _top = (DraggableBandObjects[2].Region.Bottom + 20 - 1 - DraggableBandObjects[2].Region.Height - 126) + (objects[i].Region.Top - DraggableBandObjects[2].Region.Top);

                    _top += p * bandHeight;

                    e.Graphics.FillRectangle(new SolidBrush(objects[i].Field_BackColor), new Rectangle(objects[i].Region.Left + 3, _top, contrl_Size.Width, contrl_Size.Height));

                    if ((objects[i].ControlType == 1) || (objects[i].ControlType == 4) || (objects[i].ControlType == 5) || (objects[i].ControlType == 6))
                    {
                        if (objects[i].ControlType == 1) _txt = objects[i].Field_Text;
                        if (objects[i].ControlType == 4) _txt = dt.Rows[_record][objects[i].Field_Text].ToString();
                        if (objects[i].ControlType == 5)
                        {
                            _txt = objects[i].Field_Text.Replace("xx", Total_Page.ToString());
                            _txt = _txt.Replace("x", page_Scope[_pnum].ToString());
                        }
                        if (objects[i].ControlType == 6)
                        {
                            _txt = objects[i].Field_Calculate;
                            _txt = _txt.Replace("[", "").Replace("\"]", "");
                            string[] splitString = _txt.Split(new string[] { "\"" }, StringSplitOptions.None);
                            _txt = Field_Calculate(splitString[0], splitString[1]);
                        }

                        Font fnt_Text = new Font(objects[i].Field_TextFont, objects[i].Field_TextFontSize, objects[i].Field_TextFontStyle, GraphicsUnit.Pixel);
                        Point point_text = Text_point(objects[i].Field_Align, contrl_Size.Width - 1, contrl_Size.Height - 1, e.Graphics, _txt, fnt_Text);
                        DrawText.printDrawString(e.Graphics, _txt, fnt_Text, objects[i].Field_ControlColor, new Rectangle(point_text.X + objects[i].Region.Left + 3, point_text.Y + _top, contrl_Size.Width - 1, contrl_Size.Height - 1));
                    }
                    if (objects[i].ControlType == 2)
                    {
                        if (objects[i].Field_ImgZoom == 1)
                        {
                            e.Graphics.DrawImage(objects[i].Field_Img, objects[i].Region.Left + 3, _top, objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                        }
                        else
                        {
                            Point point_img = Image_point(objects[i], objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                            var Img_desRect = new Rectangle(point_img.X + objects[i].Region.Left + 3, point_img.Y + _top, objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                            var Img_srcRect = new Rectangle(0, 0, objects[i].Region.Width - 7, objects[i].Region.Height - 7);
                            e.Graphics.DrawImage(objects[i].Field_Img, Img_desRect, Img_srcRect, GraphicsUnit.Pixel);
                        }
                    }
                    if (objects[i].ControlType == 3)
                    {
                        int _size = 1, _sizeX = 0, _sizeY = 0;
                        if (objects[i].Region.Width > objects[i].Region.Height)
                        {
                            _size = objects[i].Region.Height;
                            _sizeX = (objects[i].Region.Width - _size) / 2;
                            _sizeY = 0;
                        }
                        else
                        {
                            _size = objects[i].Region.Width;
                            _sizeX = 0;
                            _sizeY = (objects[i].Region.Height - _size) / 2;
                        }
                        Bitmap _bmp = new Bitmap(_size * 2, _size * 2);
                        DrawText.DrawFontAwesome(_bmp, shaps_type[objects[i].Field_Shape], _size - 3, objects[i].Field_ControlColor, new Point(_sizeX, _sizeY), false);
                        e.Graphics.DrawImage(_bmp, new Point(objects[i].Region.Left + 3, _top));
                        _bmp.Dispose();
                    }
                    Pen pen = new Pen(objects[i].Field_LineColor, objects[i].Field_LineThickness);
                    pen.DashStyle = objects[i].Field_LineType;
                    if (objects[i].Field_BoxLine[5]) for (int t = 0; t < 4; t++) objects[i].Field_BoxLine[t] = true;
                    if (objects[i].Field_BoxLine[0]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3, _top + contrl_Size.Height, objects[i].Region.Left + 3 + contrl_Size.Width, _top + contrl_Size.Height);
                    if (objects[i].Field_BoxLine[1]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3, _top, objects[i].Region.Left + 3 + contrl_Size.Width, _top);
                    if (objects[i].Field_BoxLine[2]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3, _top, objects[i].Region.Left + 3, _top + contrl_Size.Height);
                    if (objects[i].Field_BoxLine[3]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3 + contrl_Size.Width, _top, objects[i].Region.Left + 3 + contrl_Size.Width, _top + contrl_Size.Height);
                    if (objects[i].Field_BoxLine[6]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3, _top, objects[i].Region.Left + 3 + contrl_Size.Width - 1, _top + contrl_Size.Height - 1);
                    if (objects[i].Field_BoxLine[7]) e.Graphics.DrawLine(pen, objects[i].Region.Left + 3 + contrl_Size.Width - 1, _top, objects[i].Region.Left + 3, _top + contrl_Size.Height - 1);
                }
            }
        }
        _pnum += 1;
        if (_pnum > page_Scope.Count - 1) e.HasMorePages = false;
        else e.HasMorePages = true;
    }
}

