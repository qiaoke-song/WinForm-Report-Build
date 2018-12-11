using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RbControls_PixelTextShadow
{
    private int radius = 5;
    private int distance = 10;
    private double angle = 60;
    private byte alpha = 192;

    private int[] gaussMatrix;
    private int nuclear = 0;
    public int Radius
    {
        get
        {
            return radius;
        }
        set
        {
            if (radius != value)
            {
                radius = value;
                MakeGaussMatrix();
            }
        }
    }

    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
        }
    }

    public double Angle
    {
        get
        {
            return angle;
        }
        set
        {
            angle = value;
        }
    }

    public byte Alpha
    {
        get
        {
            return alpha;
        }
        set
        {

            alpha = value;
        }
    }
    private unsafe void MaskShadow(Bitmap bmp)
    {
        if (nuclear == 0)
            MakeGaussMatrix();
        Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);
        Bitmap tmp = (Bitmap)bmp.Clone();
        BitmapData dest = bmp.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        BitmapData source = tmp.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            byte* ps = (byte*)source.Scan0;
            ps += 3;
            byte* pd = (byte*)dest.Scan0;
            pd += (radius * (dest.Stride + 4) + 3);
            int width = dest.Width - radius * 2;
            int height = dest.Height - radius * 2;
            int matrixSize = radius * 2 + 1;
            int mOffset = dest.Stride - matrixSize * 4;
            int rOffset = radius * 8;
            int count = matrixSize * matrixSize;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    byte* s = ps - mOffset;
                    int v = 0;
                    for (int i = 0; i < count; i++, s += 4)
                    {
                        if ((i % matrixSize) == 0)
                            s += mOffset;
                        v += gaussMatrix[i] * *s;
                    }
                    *pd = (byte)(v / nuclear);
                    pd += 4;
                    ps += 4;
                }
                pd += rOffset;
                ps += rOffset;
            }
        }
        finally
        {
            tmp.UnlockBits(source);
            bmp.UnlockBits(dest);
            tmp.Dispose();
        }
    }

    protected virtual void MakeGaussMatrix()
    {
        double Q = (double)radius / 2.0;
        if (Q == 0.0)
            Q = 0.1;
        int n = radius * 2 + 1;
        int index = 0;
        nuclear = 0;
        gaussMatrix = new int[n * n];

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                gaussMatrix[index] = (int)Math.Round(Math.Exp(-((double)x * x + y * y) / (2.0 * Q * Q)) /
                                                     (2.0 * Math.PI * Q * Q) * 1000.0);
                nuclear += gaussMatrix[index];
                index++;
            }
        }
    }

    public RbControls_PixelTextShadow()
    {
    }

    public void Draw(Graphics g, string text, Font font, RectangleF layoutRect, StringFormat format)
    {
        RectangleF sr = new RectangleF((float)(radius * 2), (float)(radius * 2), layoutRect.Width, layoutRect.Height);
        Bitmap bmp = new Bitmap((int)sr.Width + radius * 4, (int)sr.Height + radius * 4, PixelFormat.Format32bppArgb);
        Brush brush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
        Graphics bg = Graphics.FromImage(bmp);
        try
        {
            bg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            bg.DrawString(text, font, brush, sr, format);
            MaskShadow(bmp);
            RectangleF dr = layoutRect;
            dr.Offset((float)(Math.Cos(Math.PI * angle / 180.0) * distance),
                      (float)(Math.Sin(Math.PI * angle / 180.0) * distance));
            sr.Inflate((float)radius, (float)radius);
            dr.Inflate((float)radius, (float)radius);
            g.DrawImage(bmp, dr, sr, GraphicsUnit.Pixel);
        }
        finally
        {
            bg.Dispose();
            brush.Dispose();
            bmp.Dispose();
        }
    }

    public void Draw(Graphics g, string text, Font font, RectangleF layoutRect)
    {
        Draw(g, text, font, layoutRect, null);
    }

    public void Draw(Graphics g, string text, Font font, PointF origin, StringFormat format)
    {
        RectangleF rect = new RectangleF(origin, g.MeasureString(text, font, origin, format));
        Draw(g, text, font, rect, format);
    }

    public void Draw(Graphics g, string text, Font font, PointF origin)
    {
        Draw(g, text, font, origin, null);
    }

}
