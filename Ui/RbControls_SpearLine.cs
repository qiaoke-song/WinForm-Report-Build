using System.Drawing;
using System.Windows.Forms;
using static RbControls;

public class RbControls_SpearLine
{
    public PictureBoxEx spear_Img;
    /// <summary>
    /// 分割线图像
    /// </summary>
    /// <param name="obj">主控件</param>
    /// <param name="_size">大小</param>
    /// <param name="_point">位置</param>
    /// <param name="_bmp">图像</param>
    public void Spear_line(Control obj, Size _size, Point _point, Bitmap _bmp)
    {
        spear_Img = new PictureBoxEx()
        {
            Size = _size,
            Location = _point,
            Image = _bmp,
            BackColor = Color.Transparent
        };
        obj.Controls.Add(spear_Img);
    }
}
