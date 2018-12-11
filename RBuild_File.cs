using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static Define_Draggable;
using static Define_Design;
using static Define_DrawObject;
using static Define_PrintPage;

public static class RBuild_File
{
    // 打开文件
    public static void Open_File()
    {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "报表文件（*.rpt）|*.rpt";
        ofd.FilterIndex = 1;
        ofd.RestoreDirectory = true;

        if (ofd.ShowDialog() == DialogResult.OK)
        {
            if (ReportChange_Flag)
            {
                DialogResult _save = MessageBox.Show("是否保存报表文件？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (_save.ToString().Equals("Yes")) Save_File();
                if (_save.ToString().Equals("Cancel")) return;
            }
            SerializerObject.Clear();
            string localFilePath = ofd.FileName.ToString();
            ReportFile_Name.Text = localFilePath;
            try
            {
                SerializerObject = Serializer.FileToObject<List<OperationObject>>(localFilePath);

                DraggableObjects.Clear();
                recordObjects.Clear();
                control_Num = -1;

                int _select = 0;
                for (int i = 0; i < SerializerObject.Count; i++)
                {
                    Draggable draggableBlock = new Draggable(SerializerObject[i].Region.Left, SerializerObject[i].Region.Top, SerializerObject[i].ControlType);
                    draggableBlock.Id = SerializerObject[i].Id;
                    draggableBlock.Belong_Band = SerializerObject[i].Belong_Band;
                    draggableBlock.Region = SerializerObject[i].Region;
                    draggableBlock.isContent = SerializerObject[i].isContent;
                    if (SerializerObject[i].ControlType == 2)
                    {
                        draggableBlock.Field_Img = Base64StringToImage(SerializerObject[i].Field_ImgBase64);
                    }
                    draggableBlock.Field_Text = SerializerObject[i].Field_Text;
                    draggableBlock.Field_Calculate = SerializerObject[i].Field_Calculate;
                    draggableBlock.Field_TextFont = SerializerObject[i].Field_TextFont;
                    draggableBlock.Field_TextFontSize = SerializerObject[i].Field_TextFontSize;
                    draggableBlock.Field_TextFontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), SerializerObject[i].Field_TextFontStyleString);
                    draggableBlock.Field_Align = SerializerObject[i].Field_Align;
                    draggableBlock.Field_ImgZoom = SerializerObject[i].Field_ImgZoom;
                    for (int t = 0; t < 8; t++) draggableBlock.Field_BoxLine[t] = SerializerObject[i].Field_BoxLine[t];
                    draggableBlock.Field_LineColor = ColorTranslator.FromHtml(SerializerObject[i].Field_LineColorString);
                    draggableBlock.Field_LineThickness = SerializerObject[i].Field_LineThickness;
                    draggableBlock.Field_LineType = SerializerObject[i].Field_LineType;
                    draggableBlock.Field_Shape = SerializerObject[i].Field_Shape;
                    draggableBlock.Field_ControlColor = ColorTranslator.FromHtml(SerializerObject[i].Field_ControlColorString);
                    draggableBlock.Field_BackColor = ColorTranslator.FromHtml(SerializerObject[i].Field_BackColorString);
                    DraggableObjects.Add(draggableBlock);

                    control_Num = i;
                    if (i == SerializerObject.Count - 1) _select = 1;
                    DraggableObjects[i].Setimage = LinBox(SerializerObject[i].Region.Width, SerializerObject[i].Region.Height, _select, SerializerObject[i].ControlType, i);
                }

                page_TypeFace = SerializerObject[0].page_Type;
                _pgselect = page_TypeFace.Page_Type;

                for (int i = 0; i < 3; i++)
                {
                    DraggableBandObjects[i].Region = SerializerObject[0].Band_Region[i];
                }

                Print_PageType.Size = page_TypeFace.Page_Area;

                int _iLeft = (page_Container.Width / 2) - (page_TypeFace.Page_Area.Width / 2);
                if (_iLeft < 0) _iLeft = 0;
                page_Install.Size = new Size(page_TypeFace.Page_Area.Width + 20, page_TypeFace.Page_Area.Height + 20);
                page_Install.Location = new Point(_iLeft, 0);

                // 设置预览页面大小
                PreViewPage_Area = new Size(page_TypeFace.Page_Area.Width, page_TypeFace.Page_Area.Height - 126);

                Print_PageType.Invalidate();
                RBuild_Info.set_Info(DraggableObjects[control_Num].ControlType);

                ReportChange_Flag = false;
            }

            catch
            {
                MessageBox.Show("报表打开错误！");
            }
        }
    }

    // 保存文件
    public static void New_File()
    {
        if (ReportChange_Flag)
        {
            DialogResult _save = MessageBox.Show("是否保存报表文件？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (_save.ToString().Equals("Yes")) Save_File();
            if (_save.ToString().Equals("Cancel")) return;
        }

        ReportFile_Name.Text = "MyReport.rpt";

        SerializerObject.Clear();
        DraggableObjects.Clear();
        recordObjects.Clear();
        control_Num = -1;

        page_Container.VerticalScroll.Value = 0;
        page_Container.HorizontalScroll.Value = 0;
        page_TypeFace.Page_Type = 4;
        page_TypeFace.Page_Direction = 0;
        Set_PrintPageType(4, page_TypeFace.Page_Direction);

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
        Print_PageType.Invalidate();
        ReportChange_Flag = false;
    }

    public static void Save_File()
    {
        SerializerObject.Clear();

        int _num = 0;

        for (int i = 0; i < DraggableObjects.Count; i++)
        {
            if (DraggableObjects[i].isContent)
            {
                OperationObject object_item = new OperationObject();
                object_item.Num = control_Num;
                object_item.Id = DraggableObjects[i].Id;
                object_item.Belong_Band = DraggableObjects[i].Belong_Band;
                object_item.Region = DraggableObjects[i].Region;
                object_item.ControlType = DraggableObjects[i].ControlType;
                object_item.isContent = DraggableObjects[i].isContent;
                if (DraggableObjects[i].ControlType == 2)
                {
                    Bitmap _bmp = new Bitmap(DraggableObjects[i].Field_Img.Width, DraggableObjects[i].Field_Img.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    Graphics draw = Graphics.FromImage(_bmp);
                    draw.DrawImage(DraggableObjects[i].Field_Img, 0, 0);
                    object_item.Field_ImgBase64 = ImgToBase64String(_bmp);
                    draw.Dispose();
                    _bmp.Dispose();
                }
                object_item.Field_Text = DraggableObjects[i].Field_Text;
                object_item.Field_Calculate = DraggableObjects[i].Field_Calculate;
                object_item.Field_TextFont = DraggableObjects[i].Field_TextFont;
                object_item.Field_TextFontSize = DraggableObjects[i].Field_TextFontSize;
                object_item.Field_TextFontStyleString = DraggableObjects[i].Field_TextFontStyle.ToString();
                object_item.Field_Align = DraggableObjects[i].Field_Align;
                object_item.Field_ImgZoom = DraggableObjects[i].Field_ImgZoom;

                for (int t = 0; t < 8; t++) object_item.Field_BoxLine[t] = DraggableObjects[i].Field_BoxLine[t];

                object_item.Field_LineColorString = ColorTranslator.ToHtml(DraggableObjects[i].Field_LineColor);
                object_item.Field_LineThickness = DraggableObjects[i].Field_LineThickness;
                object_item.Field_LineType = DraggableObjects[i].Field_LineType;
                object_item.Field_Shape = DraggableObjects[i].Field_Shape;
                object_item.Field_ControlColorString = ColorTranslator.ToHtml(DraggableObjects[i].Field_ControlColor);
                object_item.Field_BackColorString = ColorTranslator.ToHtml(DraggableObjects[i].Field_BackColor);
                object_item.page_Type = page_TypeFace;
                for (int t = 0; t < 3; t++) object_item.Band_Region[t] = DraggableBandObjects[t].Region;
                SerializerObject.Add(object_item);
                _num += 1;
            }
        }

        if (_num > 0)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "报表文件（*.rpt）|*.rpt";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            sfd.FileName = ReportFile_Name.Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                Serializer.ObjectToFile(SerializerObject, sfd.FileName);
                ReportChange_Flag = false;
            }
        }
        else
        {
            MessageBox.Show("报表组件内不包含内容，报表将不被保存。");
        }
    }

    //图片转为base64编码的字符串
    public static string ImgToBase64String(Bitmap bmp)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }
        catch
        {
            return null;
        }
    }

    //base64编码的字符串转为图片
    public static Bitmap Base64StringToImage(string strbase64)
    {
        try
        {
            byte[] arr = Convert.FromBase64String(strbase64);
            MemoryStream ms = new MemoryStream(arr);
            Bitmap bmp = new Bitmap(ms);
            ms.Close();
            return bmp;
        }
        catch
        {
            return null;
        }
    }
}

