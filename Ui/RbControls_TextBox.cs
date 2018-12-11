using System;
using System.Drawing;
using System.Windows.Forms;
using static RbControls;

public class RbControls_TextBox
{
    public TextBox textBox;

    /// <summary>
    /// inputBox 输入框
    /// </summary>
    /// <param name="obj">在那个控件内</param>
    /// <param name="_sizeWidth">宽度</param>
    /// <param name="_point">位置</param>
    /// <param name="_fnt">字体</param>
    /// <param name="_fontColor">字体颜色</param>
    /// <param name="_backColor">背景色</param>
    /// <param name="_text">文字</param>
    /// <param name="_maxlength">最大文字数</param>
    /// <param name="_readonly">是否只读</param>
    /// <param name="_mode">模式：0普通、1密码、2整数限制</param>
    /// <param name="_click">点击事件</param>
    public void input_Box(Control obj, int _sizeWidth, Point _point, Font _fnt, Color _fontColor, Color _backColor, string _text, int _maxlength, bool _readonly, int _mode, EventHandler _click, KeyEventHandler _keyup)
    {
        PanelEx input_Line = new PanelEx()
        {
            Size = new Size(_sizeWidth, 1),
            Location = new Point(_point.X, _point.Y + 16),
            BackgroundImage = EzRBuild.EzResource.line,
            BackgroundImageLayout = ImageLayout.Stretch
        };
        obj.Controls.Add(input_Line);

        textBox = new TextBox()
        {
            AutoSize = false,
            Multiline = false,
            MaxLength = _maxlength,
            Size = new Size(_sizeWidth, 16),
            Location = _point,
            BorderStyle = BorderStyle.None,
            Font = _fnt,
            Text = _text,
            ReadOnly = _readonly,
            ForeColor = _fontColor,
            BackColor = _backColor
        };
        if (_mode == 1) textBox.PasswordChar = '●';
        if (_mode == 2) textBox.KeyPress += TextBox_KeyPress;
        textBox.Select(0, 0);
        textBox.Click += _click;
        textBox.KeyUp += _keyup;
        obj.Controls.Add(textBox);

    }

    private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        char result = e.KeyChar;
        if (char.IsDigit(result) || result == 8)
        {
            e.Handled = false;
        }
        else
        {
            e.Handled = true;
        }
    }
}
