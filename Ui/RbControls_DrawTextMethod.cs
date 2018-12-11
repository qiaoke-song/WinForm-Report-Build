using System.Drawing;

public class RbControls_DrawTextMethod
{
    public void DrawString(Bitmap _bmp, string text, Font fnt, Color textColor, Rectangle rect)
    {
        DrawString(_bmp, text, fnt, textColor, rect, false, new Point(0, 0));
    }

    public void printDrawString(Graphics e, string text, Font fnt, Color textColor, Rectangle rect)
    {
        printDrawString(e, text, fnt, textColor, rect, false, new Point(0, 0));
    }

    public void DrawString(Bitmap _bmp, string text, Font fnt, Color textColor, Rectangle rect, bool shadow, Point distance)
    {
        Graphics txt_bmp = Graphics.FromImage(_bmp);
        if (shadow)
        {
            RbControls_PixelTextShadow _Txtshadow = new RbControls_PixelTextShadow();
            _Txtshadow.Draw(txt_bmp, text, fnt, new PointF(rect.X + distance.X, rect.Y + distance.Y));
        }
        txt_bmp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        SolidBrush _brush = new SolidBrush(textColor);
        txt_bmp.DrawString(text, fnt, _brush, rect);
    }

    public void printDrawString(Graphics e, string text, Font fnt, Color textColor, Rectangle rect, bool shadow, Point distance)
    {
        if (shadow)
        {
            RbControls_PixelTextShadow _Txtshadow = new RbControls_PixelTextShadow();
            _Txtshadow.Draw(e, text, fnt, new PointF(rect.X + distance.X, rect.Y + distance.Y));
        }
        e.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        SolidBrush _brush = new SolidBrush(textColor);
        e.DrawString(text, fnt, _brush, rect);
    }

    public void DrawFontAwesome(Bitmap _bmp, RbControls_FontAwesome.Type type, int size, Color color, Point location, bool Border)
    {
        Bitmap font_bmp = new Bitmap(size, size);
        font_bmp = new RbControls_FontAwesome.Properties(type) { ForeColor = color, Size = size - 4, ShowBorder = Border }.AsImage();

        Graphics add_bmp = Graphics.FromImage(_bmp);
        add_bmp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        add_bmp.DrawImage(font_bmp, location);
    }
}

