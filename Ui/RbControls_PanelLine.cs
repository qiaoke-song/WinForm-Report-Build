using System.Drawing;
using System.Windows.Forms;
using static RbControls;

public class RbControls_PanelLine
{
    /// <summary>
    /// panel分割线
    /// </summary>
    /// <param name="obj">主控件</param>
    /// <param name="_size">大小</param>
    /// <param name="_point">位置</param>
    public void Panel_Line(Control obj, Size _size, Point _point)
    {
        PanelEx _Line = new PanelEx()
        {
            Size = _size,
            Location = _point,
            BackgroundImage = EzRBuild.EzResource.line,
            BackgroundImageLayout = ImageLayout.Stretch
        };
        obj.Controls.Add(_Line);
    }
}
