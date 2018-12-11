using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static Define_PrintPage;
using static Define_Design;
using static Define_DrawObject;

public static class Define_Draggable
{
    public static int control_Num = -1; // 组件序号
    public static int band_Num = 0; // 栏目序号
    public static int control_Type = -1; // 组件类型

    /// <summary>
    /// 栏目移动
    /// </summary>
    public abstract class DraggableBandObject
    {
        public abstract int Id { get; set; }
        public abstract Rectangle Region { get; set; } // 大小范围
        public abstract bool IsDragging { get; set; } // 能否拖动
        public abstract bool IsSize { get; set; } // 能否拖动
        public abstract Point DraggingPoint { get; set; } // band拖动位置
        public abstract Bitmap Setimage { get; set; } // band图像
        public abstract void OnPaint(PaintEventArgs e); // 重绘
    }

    public class DraggableBand : DraggableBandObject
    {
        private int m_Id;
        private Rectangle m_Region;
        private bool m_IsDragging;
        private bool m_IsSize;
        private Point m_DraggingPoint;
        private Bitmap image;

        public DraggableBand(int startX, int startY, int _width, int _height, int _flag)
        {
            image = LinBand(_width, _height, _flag, startY, band_Num);

            m_Region = new Rectangle(startX, startY, _width, _height);
            m_IsDragging = false;

        }

        public override int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public override Rectangle Region
        {
            get { return m_Region; }
            set { m_Region = value; }
        }
        public override bool IsDragging
        {
            get { return m_IsDragging; }
            set { m_IsDragging = value; }
        }
        public override bool IsSize
        {
            get { return m_IsSize; }
            set { m_IsSize = value; }
        }
        public override Point DraggingPoint
        {
            get { return m_DraggingPoint; }
            set { m_DraggingPoint = value; }
        }
        public override Bitmap Setimage
        {
            get { return image; }
            set { image = value; }
        }
        public override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(image, m_Region);
        }
    }
    public static List<DraggableBandObject> DraggableBandObjects = new List<DraggableBandObject>();

    /// <summary>
    /// 栏目信息序列化
    /// </summary>
    public class OperationBandObject
    {
        public int Id;
        public Rectangle Region; // 大小范围
    }
    public static List<OperationBandObject> SerializerBandObject = new List<OperationBandObject>();


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 组件移动
    /// </summary>
    public abstract class DraggableObject
    {
        public abstract int Primary { get; set; }
        public abstract string Id { get; set; }
        public abstract int Belong_Band { get; set; } // 属于哪个栏目，1.页头；2.页脚；3.数据栏
        public abstract Rectangle Region { get; set; } // 大小范围
        public abstract bool IsDragging { get; set; } // 能否拖动
        public abstract Point DraggingPoint { get; set; } // 组件拖动位置
        public abstract Bitmap Setimage { get; set; } // 组件图像
        public abstract int ControlType { get; set; } // 组件类型，1.文本；2.图像；3.形状；4.数据字段；5.日期；6.页码
        public abstract void OnPaint(PaintEventArgs e); // 重绘
        public abstract bool isContent { get; set; } // 组件内是否有内容
        public abstract Bitmap Field_Img { get; set; } // 图像组件的图像
        public abstract string Field_Text { get; set; } // 文字
        public abstract string Field_Calculate { get; set; } // 字段运算
        public abstract string Field_TextFont { get; set; } // 字体
        public abstract int Field_TextFontSize { get; set; } // 字体大小
        public abstract FontStyle Field_TextFontStyle { get; set; } // 文字样式
        public abstract string Field_Align { get; set; } // 对齐方式
        public abstract int Field_ImgZoom { get; set; } // 图像缩放
        public abstract bool[] Field_BoxLine { get; set; } // 边框和斜线
        public abstract Color Field_LineColor { get; set; } // 边框和斜线颜色
        public abstract int Field_LineThickness { get; set; } // 边框和斜线粗细
        public abstract DashStyle Field_LineType { get; set; } // 线型
        public abstract int Field_Shape { get; set; } // 形状的类型
        public abstract Color Field_ControlColor { get; set; } // 组件颜色
        public abstract Color Field_BackColor { get; set; } // 组件背景色
    }

    /// <summary>
    /// 组件移动对象定义
    /// </summary>
    public class Draggable : DraggableObject
    {
        private int m_Primary;
        private string m_Id;
        private int m_BelongBand; // 属于哪个栏目，1.页头；2.页脚；3.数据栏
        private int m_ControlType; // 组件类型，1.文本；2.图像；3.形状；4.数据字段；5.日期；6.页码
        private Rectangle m_Region; // 大小范围
        private bool m_IsDragging; // 能否拖动
        private Point m_DraggingPoint; // 组件拖动位置
        private Bitmap m_Setimage; // 组件图像
        private bool m_isContent; // 组件内是否有内容
        private Bitmap m_FieldImg; // 图像组件的图像
        private string m_FieldText; // 文字
        private string m_FieldCalculate; // 字段运算
        private string m_FieldTextFont; // 字体
        private int m_FieldTextFontSize; // 字体大小
        private FontStyle m_FieldTextFontStyle; // 文字样式
        private string m_FieldAlign; // 对齐方式
        private int m_FieldImgZoom; // 图像缩放
        private bool[] m_FieldBoxLine; // 边框和斜线
        private Color m_FieldLineColor; // 边框和斜线颜色
        private int m_FieldLineThickness; // 边框和斜线粗细
        private DashStyle m_FieldLineType; // 线型
        private int m_FieldShape; // 形状的类型
        private Color m_FieldControlColor; // 组件颜色
        private Color m_FieldBackColor; // 组件背景色

        public Draggable(int startX, int startY, int _controlType)
        {
            m_Region = new Rectangle(startX, startY, 51, 51);
            m_ControlType = _controlType;
            m_isContent = false;
            m_FieldBoxLine = new bool[8] { false, false, false, false, false, false, false, false };
            m_Primary = startY;
            m_Id = GetMd5Str16(DateTime.Now.ToString());
            m_FieldBackColor = Color.White;
            m_FieldLineColor = Color.Black;
            m_FieldLineThickness = 1;
            m_FieldLineType = DashStyle.Solid;
        }

        public override int Primary
        {
            get { return m_Primary; }
            set { m_Primary = value; }
        }

        public override string Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public override int Belong_Band
        {
            get { return m_BelongBand; }
            set { m_BelongBand = value; }
        }
        public override int ControlType
        {
            get { return m_ControlType; }
            set { m_ControlType = value; }
        }
        public override Rectangle Region
        {
            get { return m_Region; }
            set { m_Region = value; }
        }
        public override bool IsDragging
        {
            get { return m_IsDragging; }
            set { m_IsDragging = value; }
        }
        public override Point DraggingPoint
        {
            get { return m_DraggingPoint; }
            set { m_DraggingPoint = value; }
        }
        public override Bitmap Setimage
        {
            get { return m_Setimage; }
            set { m_Setimage = value; }
        }
        public override bool isContent
        {
            get { return m_isContent; }
            set { m_isContent = value; }
        }
        public override Bitmap Field_Img
        {
            get { return m_FieldImg; }
            set { m_FieldImg = value; }
        }
        public override string Field_Text
        {
            get { return m_FieldText; }
            set { m_FieldText = value; }
        }
        public override string Field_Calculate
        {
            get { return m_FieldCalculate; }
            set { m_FieldCalculate = value; }
        }
        public override string Field_TextFont
        {
            get { return m_FieldTextFont; }
            set { m_FieldTextFont = value; }
        }
        public override int Field_TextFontSize
        {
            get { return m_FieldTextFontSize; }
            set { m_FieldTextFontSize = value; }
        }
        public override FontStyle Field_TextFontStyle
        {
            get { return m_FieldTextFontStyle; }
            set { m_FieldTextFontStyle = value; }
        }
        public override string Field_Align
        {
            get { return m_FieldAlign; }
            set { m_FieldAlign = value; }
        }
        public override int Field_ImgZoom
        {
            get { return m_FieldImgZoom; }
            set { m_FieldImgZoom = value; }
        }
        public override bool[] Field_BoxLine
        {
            get { return m_FieldBoxLine; }
            set { m_FieldBoxLine = value; }
        }
        public override Color Field_LineColor
        {
            get { return m_FieldLineColor; }
            set { m_FieldLineColor = value; }
        }
        public override int Field_LineThickness
        {
            get { return m_FieldLineThickness; }
            set { m_FieldLineThickness = value; }
        }
        public override DashStyle Field_LineType
        {
            get { return m_FieldLineType; }
            set { m_FieldLineType = value; }
        }
        public override int Field_Shape
        {
            get { return m_FieldShape; }
            set { m_FieldShape = value; }
        }
        public override Color Field_ControlColor
        {
            get { return m_FieldControlColor; }
            set { m_FieldControlColor = value; }
        }
        public override Color Field_BackColor
        {
            get { return m_FieldBackColor; }
            set { m_FieldBackColor = value; }
        }
        public override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(m_Setimage, m_Region);
        }
    }
    public static List<DraggableObject> DraggableObjects = new List<DraggableObject>();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 剪切复制粘贴组件操作
    /// </summary>
    [Serializable]
    public class OperationObject
    {
        public Rectangle[] Band_Region = new Rectangle[3]; // 记录栏目位置
        public int Num; // 序号
        public string Id; // 标识
        public int Belong_Band; // 属于哪个栏目，1.页头；2.页脚；3.数据栏
        public Rectangle Region; // 大小范围
        [NonSerialized]
        public Bitmap Setimage; // 组件图像
        public string SetimageBase64; // 组件图像
        public int ControlType; // 组件类型，1.文本；2.图像；3.形状；4.数据字段；5.日期；6.页码
        public bool isContent; // 组件内是否有内容
        [NonSerialized]
        public Bitmap Field_Img; // 图像组件的图像
        public string Field_ImgBase64; // 图像组件的图像
        public string Field_Text; // 文字
        public string Field_Calculate; // 字段运算
        public string Field_TextFont; // 字体
        public int Field_TextFontSize; // 字体大小
        [NonSerialized]
        public FontStyle Field_TextFontStyle; // 文字样式
        public string Field_TextFontStyleString;
        public string Field_Align; // 对齐方式
        public int Field_ImgZoom; // 图像缩放
        public bool[] Field_BoxLine = new bool[8]; // 边框和斜线
        [NonSerialized]
        public Color Field_LineColor; // 边框和斜线颜色
        public string Field_LineColorString;
        public int Field_LineThickness; // 边框和斜线粗细
        public DashStyle Field_LineType; // 线型
        public int Field_Shape; // 形状的类型
        [NonSerialized]
        public Color Field_ControlColor; // 组件颜色
        public string Field_ControlColorString;
        [NonSerialized]
        public Color Field_BackColor; // 组件背景色
        public string Field_BackColorString;
        public Page_TypeFace page_Type;
    }
    public static OperationObject operationObject = new OperationObject();

    /// 组件操作记录，用于撤销、重做
    public static List<List<OperationObject>> recordObjects = new List<List<OperationObject>>();
    /// 保存文件
    public static List<OperationObject> SerializerObject = new List<OperationObject>();

    /// <summary>
    /// 16位md5值
    /// </summary>
    /// <param name="ConvertString"></param>
    /// <returns></returns>
    private static string GetMd5Str16(string ConvertString)
    {
        try
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string c16 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(ConvertString)), 4, 8);
                return c16.Replace("-", "").ToLower();
            }
        }
        catch { return null; }
    }

    /// <summary>
    /// 元素位移顶层，其他依次后移
    /// </summary>
    public static void ListSwap_Top()
    {
        var swap = DraggableObjects[control_Num];
        DraggableObjects.RemoveAt(control_Num);
        DraggableObjects.Add(swap);
        control_Num = DraggableObjects.Count - 1;
        Print_PageType.Invalidate();
        ReportChange_Flag = true;
    }

    /// <summary>
    /// 元素位移底层，其他依次后移
    /// </summary>
    public static void ListSwap_Bottom()
    {
        var swap = DraggableObjects[control_Num];
        DraggableObjects.RemoveAt(control_Num);
        DraggableObjects.Insert(0, swap);
        control_Num = 0;
        Print_PageType.Invalidate();
        ReportChange_Flag = true;
    }

    /// <summary>
    /// 复制组件
    /// </summary>
    public static void Object_OperationCopy()
    {
        if ((DraggableObjects.Count > 0) && (band_Num != -1))
        {
            operationObject.Id = GetMd5Str16(DateTime.Now.ToString());
            operationObject.Belong_Band = band_Num;
            operationObject.Region = DraggableObjects[control_Num].Region;
            operationObject.Setimage = DraggableObjects[control_Num].Setimage;
            operationObject.ControlType = DraggableObjects[control_Num].ControlType;
            operationObject.isContent = DraggableObjects[control_Num].isContent;
            operationObject.Field_Img = DraggableObjects[control_Num].Field_Img;
            operationObject.Field_Text = DraggableObjects[control_Num].Field_Text;
            operationObject.Field_Calculate = DraggableObjects[control_Num].Field_Calculate;
            operationObject.Field_TextFont = DraggableObjects[control_Num].Field_TextFont;
            operationObject.Field_TextFontSize = DraggableObjects[control_Num].Field_TextFontSize;
            operationObject.Field_TextFontStyle = DraggableObjects[control_Num].Field_TextFontStyle;
            operationObject.Field_Align = DraggableObjects[control_Num].Field_Align;
            operationObject.Field_ImgZoom = DraggableObjects[control_Num].Field_ImgZoom;
            for (int i = 0; i < 8; i++) operationObject.Field_BoxLine[i] = DraggableObjects[control_Num].Field_BoxLine[i];
            operationObject.Field_LineColor = DraggableObjects[control_Num].Field_LineColor;
            operationObject.Field_LineThickness = DraggableObjects[control_Num].Field_LineThickness;
            operationObject.Field_LineType = DraggableObjects[control_Num].Field_LineType;
            operationObject.Field_Shape = DraggableObjects[control_Num].Field_Shape;
            operationObject.Field_ControlColor = DraggableObjects[control_Num].Field_ControlColor;
            operationObject.Field_BackColor = DraggableObjects[control_Num].Field_BackColor;
        }
    }

    /// <summary>
    /// 粘贴组件
    /// </summary>
    /// <param name="lx">粘贴位置 x</param>
    /// <param name="ly">粘贴位置 y</param>
    public static void Object_OperationPast(int lx, int ly)
    {
        if ((operationObject.Id != null) && (band_Num != -1))
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
            operationObject.Region = new Rectangle(
                DraggableBandObjects[band_Num].Region.Left + lx,
                DraggableBandObjects[band_Num].Region.Top + ly,
                operationObject.Region.Width,
                operationObject.Region.Height
                );
            Draggable draggableBlock = new Draggable(operationObject.Region.Left, operationObject.Region.Top, operationObject.ControlType);

            draggableBlock.Id = GetMd5Str16(DateTime.Now.ToString());
            draggableBlock.Belong_Band = band_Num;
            draggableBlock.Region = operationObject.Region;
            draggableBlock.Setimage = operationObject.Setimage;
            draggableBlock.isContent = operationObject.isContent;
            draggableBlock.Field_Img = operationObject.Field_Img;
            draggableBlock.Field_Text = operationObject.Field_Text;
            draggableBlock.Field_Calculate = operationObject.Field_Calculate;
            draggableBlock.Field_TextFont = operationObject.Field_TextFont;
            draggableBlock.Field_TextFontSize = operationObject.Field_TextFontSize;
            draggableBlock.Field_TextFontStyle = operationObject.Field_TextFontStyle;
            draggableBlock.Field_Align = operationObject.Field_Align;
            draggableBlock.Field_ImgZoom = operationObject.Field_ImgZoom;
            for (int i = 0; i < 8; i++) draggableBlock.Field_BoxLine[i] = operationObject.Field_BoxLine[i];
            draggableBlock.Field_LineColor = operationObject.Field_LineColor;
            draggableBlock.Field_LineThickness = operationObject.Field_LineThickness;
            draggableBlock.Field_LineType = operationObject.Field_LineType;
            draggableBlock.Field_Shape = operationObject.Field_Shape;
            draggableBlock.Field_ControlColor = operationObject.Field_ControlColor;
            draggableBlock.Field_BackColor = operationObject.Field_BackColor;

            control_Num = DraggableObjects.Count;
            DraggableObjects.Add(draggableBlock);
            Print_PageType.Invalidate();
            Object_Record();
            RBuild_Info.set_Info(DraggableObjects[control_Num].ControlType);
            ReportChange_Flag = true;
        }
    }

    /// <summary>
    /// 记录组件操作
    /// </summary>
    public static void Object_Record()
    {
        List<OperationObject> r_obj = new List<OperationObject>();

        for (int i = 0; i < DraggableObjects.Count; i++)
        {
            OperationObject item = new OperationObject();
            item.Num = control_Num;
            item.Id = DraggableObjects[i].Id;
            item.Belong_Band = DraggableObjects[i].Belong_Band;
            item.Region = DraggableObjects[i].Region;
            item.Setimage = DraggableObjects[i].Setimage;
            item.ControlType = DraggableObjects[i].ControlType;
            item.isContent = DraggableObjects[i].isContent;
            item.Field_Img = DraggableObjects[i].Field_Img;
            item.Field_Text = DraggableObjects[i].Field_Text;
            item.Field_Calculate = DraggableObjects[i].Field_Calculate;
            item.Field_TextFont = DraggableObjects[i].Field_TextFont;
            item.Field_TextFontSize = DraggableObjects[i].Field_TextFontSize;
            item.Field_TextFontStyle = DraggableObjects[i].Field_TextFontStyle;
            item.Field_Align = DraggableObjects[i].Field_Align;
            item.Field_ImgZoom = DraggableObjects[i].Field_ImgZoom;
            for (int t = 0; t < 8; t++) item.Field_BoxLine[t] = DraggableObjects[i].Field_BoxLine[t];
            item.Field_LineColor = DraggableObjects[i].Field_LineColor;
            item.Field_LineThickness = DraggableObjects[i].Field_LineThickness;
            item.Field_LineType = DraggableObjects[i].Field_LineType;
            item.Field_Shape = DraggableObjects[i].Field_Shape;
            item.Field_ControlColor = DraggableObjects[i].Field_ControlColor;
            item.Field_BackColor = DraggableObjects[i].Field_BackColor;

            for (int t = 0; t < 3; t++) item.Band_Region[t] = DraggableBandObjects[t].Region;
            r_obj.Add(item);
        }
        oper_Record += 1;
        if (oper_Record >= recordObjects.Count)
        {
            recordObjects.Add(r_obj);
        }
        else
        {
            recordObjects.Insert(oper_Record, r_obj);
        }
    }

    /// <summary>
    /// 撤销、重做
    /// </summary>
    /// <param name="_mode">模式</param>
    public static void Object_Operation(int _mode)
    {
        bool Oper = false;

        if (_mode == 0) // 撤销
        {
            oper_Record -= 1;
            if (oper_Record < 0)
            {
                Oper = false;
                oper_Record = 0;
            }
            else Oper = true;
        }
        else
        if (_mode == 1) // 重做
        {
            oper_Record += 1;
            if (oper_Record >= recordObjects.Count)
            {
                Oper = false;
                oper_Record = recordObjects.Count - 1;
            }
            else Oper = true;
        }

        if (Oper)
        {
            DraggableObjects.Clear();
            for (int i = 0; i < recordObjects[oper_Record].Count; i++)
            {
                Draggable draggableBlock = new Draggable(recordObjects[oper_Record][i].Region.Left, recordObjects[oper_Record][i].Region.Top, recordObjects[oper_Record][i].ControlType);

                draggableBlock.Id = recordObjects[oper_Record][i].Id;
                draggableBlock.Belong_Band = recordObjects[oper_Record][i].Belong_Band;
                draggableBlock.Region = recordObjects[oper_Record][i].Region;
                draggableBlock.Setimage = recordObjects[oper_Record][i].Setimage;
                draggableBlock.isContent = recordObjects[oper_Record][i].isContent;
                draggableBlock.Field_Img = recordObjects[oper_Record][i].Field_Img;
                draggableBlock.Field_Text = recordObjects[oper_Record][i].Field_Text;
                draggableBlock.Field_Calculate = recordObjects[oper_Record][i].Field_Calculate;
                draggableBlock.Field_TextFont = recordObjects[oper_Record][i].Field_TextFont;
                draggableBlock.Field_TextFontSize = recordObjects[oper_Record][i].Field_TextFontSize;
                draggableBlock.Field_TextFontStyle = recordObjects[oper_Record][i].Field_TextFontStyle;
                draggableBlock.Field_Align = recordObjects[oper_Record][i].Field_Align;
                draggableBlock.Field_ImgZoom = recordObjects[oper_Record][i].Field_ImgZoom;
                for (int t = 0; t < 8; t++) draggableBlock.Field_BoxLine[t] = recordObjects[oper_Record][i].Field_BoxLine[t];
                draggableBlock.Field_LineColor = recordObjects[oper_Record][i].Field_LineColor;
                draggableBlock.Field_LineThickness = recordObjects[oper_Record][i].Field_LineThickness;
                draggableBlock.Field_LineType = recordObjects[oper_Record][i].Field_LineType;
                draggableBlock.Field_Shape = recordObjects[oper_Record][i].Field_Shape;
                draggableBlock.Field_ControlColor = recordObjects[oper_Record][i].Field_ControlColor;
                draggableBlock.Field_BackColor = recordObjects[oper_Record][i].Field_BackColor;

                DraggableObjects.Add(draggableBlock);
            }
            control_Num = recordObjects[oper_Record][0].Num;
            for (int i = 0; i < 3; i++)
            {
                DraggableBandObjects[i].Region = recordObjects[oper_Record][0].Band_Region[i];
            }

            Print_PageType.Invalidate();
            RBuild_Info.set_Info(DraggableObjects[control_Num].ControlType);
            ReportChange_Flag = true;
        }
    }

    /// <summary>
    /// 删除组件
    /// </summary>
    /// <param name="_mode">是否记录</param>
    public static void Delete_Control(bool _record)
    {
        if (DraggableObjects.Count > 0)
        {
            DraggableObjects.Remove(DraggableObjects[control_Num]);
            if (DraggableObjects.Count < 1)
            {
                control_Num = -1;
                RBuild_Info.Set_DefaultInfo();
            }
            else
            {
                control_Num -= 1;
                if (control_Num < 0) control_Num = 0;
                DraggableObjects[control_Num].Setimage = LinBox(DraggableObjects[control_Num].Region.Width, DraggableObjects[control_Num].Region.Height, 1, DraggableObjects[control_Num].ControlType, control_Num);
                RBuild_Info.set_Info(DraggableObjects[control_Num].ControlType);
                DraggableObjects[control_Num].IsDragging = false;
                if (_record) Object_Record();
            }
            page_Install.Invalidate();
            page_Container.Invalidate();
            Print_PageType.Invalidate();
            RBuild_MouseEvent.CursorFlag = -1;
            ReportChange_Flag = true;
        }
    }

    /// <summary>
    ///  初始化栏目
    /// </summary>
    public static void Initialize_Band()
    {
        int height = 120;
        int ly = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i == 0) { height = 120; ly = 0; }
            else if (i == 1) { height = 250; ly = 128; }
            else if (i == 2) { height = 120; ly = page_TypeFace.Page_Area.Height - height; }
            DraggableBand draggableBandBlock = new DraggableBand(0, ly, Print_PageType.Width, height, i);
            draggableBandBlock.Id = i;
            DraggableBandObjects.Add(draggableBandBlock);
        }
        Print_PageType.Invalidate();
    }

    // 设置位置
    public static void Set_BoxPoint(int _type)
    {
        if (control_Num > -1)
        {
            switch (_type)
            {
                case 0: // 左
                    DraggableObjects[control_Num].Region = new Rectangle(
                        0,
                        DraggableObjects[control_Num].Region.Top,
                        DraggableObjects[control_Num].Region.Width,
                        DraggableObjects[control_Num].Region.Height
                        );
                    break;
                case 1: // 右
                    DraggableObjects[control_Num].Region = new Rectangle(
                        Print_PageType.Width - DraggableObjects[control_Num].Region.Width,
                        DraggableObjects[control_Num].Region.Top,
                        DraggableObjects[control_Num].Region.Width,
                        DraggableObjects[control_Num].Region.Height
                        );
                    break;
                case 2: // 顶
                    DraggableObjects[control_Num].Region = new Rectangle(
                        DraggableObjects[control_Num].Region.Left,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Top + 20,
                        DraggableObjects[control_Num].Region.Width,
                        DraggableObjects[control_Num].Region.Height
                        );
                    break;
                case 3: // 底
                    DraggableObjects[control_Num].Region = new Rectangle(
                        DraggableObjects[control_Num].Region.Left,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Bottom - 20 - DraggableObjects[control_Num].Region.Height,
                        DraggableObjects[control_Num].Region.Width,
                        DraggableObjects[control_Num].Region.Height
                        );
                    break;
                default:
                    break;
            }
            Print_PageType.Invalidate();
            Object_Record();
            View_Info = true;
            ReportChange_Flag = true;
        }
    }

    // 设置对齐方式
    public static void Set_FillAlign(int _type)
    {
        if (control_Num > -1)
        {
            switch (_type)
            {
                case 0: // 左对齐
                    DraggableObjects[control_Num].Region = new Rectangle(
                        0,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Top + 20,
                        DraggableObjects[control_Num].Region.Width,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Height - 40
                        );
                    break;
                case 1: // 垂直对齐
                    DraggableObjects[control_Num].Region = new Rectangle(
                        0,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Top + 20 +
                        (DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Height - 40 - DraggableObjects[control_Num].Region.Height) / 2,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Width,
                        DraggableObjects[control_Num].Region.Height
                        );
                    break;
                case 2: // 右对齐
                    DraggableObjects[control_Num].Region = new Rectangle(
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Width - DraggableObjects[control_Num].Region.Width,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Top + 20,
                        DraggableObjects[control_Num].Region.Width,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Height - 40
                        );
                    break;
                case 3: // 顶对齐
                    DraggableObjects[control_Num].Region = new Rectangle(
                        0,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Top + 20,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Width,
                        DraggableObjects[control_Num].Region.Height
                        );
                    break;
                case 4: // 底对齐
                    DraggableObjects[control_Num].Region = new Rectangle(
                        0,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Bottom - 20 - DraggableObjects[control_Num].Region.Height,
                        DraggableBandObjects[DraggableObjects[control_Num].Belong_Band].Region.Width,
                        DraggableObjects[control_Num].Region.Height
                        );
                    break;

                default:
                    break;
            }

            DraggableObjects[control_Num].Setimage = LinBox(
                DraggableObjects[control_Num].Region.Width,
                DraggableObjects[control_Num].Region.Height,
                1,
                DraggableObjects[control_Num].ControlType,
                control_Num
                );
            Print_PageType.Invalidate();
            Object_Record();
            View_Info = true;
            ReportChange_Flag = true;
        }
    }

    // 画边框线
    public static void Set_LineBorder(int _type)
    {
        if (control_Num > -1)
        {
            DraggableObjects[control_Num].Field_BoxLine[_type] = !DraggableObjects[control_Num].Field_BoxLine[_type];

            if (_type == 5)
            {
                if (!DraggableObjects[control_Num].Field_BoxLine[_type]) for (int i = 0; i < 8; i++) DraggableObjects[control_Num].Field_BoxLine[i] = false;
            }

            DraggableObjects[control_Num].Setimage = LinBox(DraggableObjects[control_Num].Region.Width, DraggableObjects[control_Num].Region.Height, 1, DraggableObjects[control_Num].ControlType, control_Num);
            Print_PageType.Invalidate();
            Object_Record();
            ReportChange_Flag = true;
        }
    }
}
