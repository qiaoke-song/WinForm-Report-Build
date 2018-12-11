using System;
using System.Drawing;
using System.Windows.Forms;

public class RbControls_ButtonLabel
{
    private Label Btn_labelText;
    private Color Btn_enterColor, Btn_leaveColor;
    /// <summary>
    /// 文字按钮
    /// </summary>
    /// <param name="obj">主控件</param>
    /// <param name="_point">位置</param>
    /// <param name="_textColor">文字颜色</param>
    /// <param name="_enterColor">选中颜色</param>
    /// <param name="_leaveColor">离开颜色</param>
    /// <param name="_text">文字</param>
    /// <param name="_fnt">字体</param>
    /// <param name="_tag">标识</param>
    /// <param name="_click">点击事件</param>
    public void Button_Label(Control obj, Point _point, Color _textColor, Color _enterColor, Color _leaveColor, string _text, Font _fnt, int _tag, MouseEventHandler _click)
    {
        Btn_enterColor = _enterColor;
        Btn_leaveColor = _leaveColor;
        Btn_labelText = new Label()
        {
            AutoSize = true,
            Location = _point,
            Font = _fnt,
            ForeColor = _textColor,
            Text = _text,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            Tag = _tag
        };
        Btn_labelText.MouseEnter += BtnLabelText_MouseEnter;
        Btn_labelText.MouseLeave += BtnLabelText_MouseLeave;
        Btn_labelText.MouseDown += _click;
        obj.Controls.Add(Btn_labelText);
    }

    private void BtnLabelText_MouseLeave(object sender, EventArgs e)
    {
        Btn_labelText.ForeColor = Btn_leaveColor;
    }

    private void BtnLabelText_MouseEnter(object sender, EventArgs e)
    {
        Btn_labelText.ForeColor = Btn_enterColor;
    }

}

