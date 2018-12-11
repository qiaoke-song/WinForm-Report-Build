using System.Drawing;
using System.Windows.Forms;

public class RbControls_TextLabel
{
    public Label labelText;
    /// <summary>
    /// Label 文字
    /// </summary>
    /// <param name="obj">主控件</param>
    /// <param name="_point">位置</param>
    /// <param name="_fnt">字体</param>
    /// <param name="_fontColor">文字颜色</param>
    /// <param name="_text">文字</param>
    public void Text_Label(Control obj, Point _point, Font _fnt, Color _fontColor, string _text)
    {
        labelText = new Label()
        {
            AutoSize = true,
            Location = _point,
            Font = _fnt,
            ForeColor = _fontColor,
            Text = _text,
            BackColor = Color.Transparent
        };
        obj.Controls.Add(labelText);
    }
}
