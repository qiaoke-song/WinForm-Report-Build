using System;
using System.Drawing;
using System.Windows.Forms;

public class RbControls_ConextMenu
{
    public Label[] text_label;

    private int total;
    private Color fontColor;
    private Color seletColor;
    private Color fontSelectColor;

    /// <summary>
    /// 菜单
    /// </summary>
    /// <param name="obj">在哪个控件内</param>
    /// <param name="_totalNum">总数量</param>
    /// <param name="_menutext">文字</param>
    /// <param name="_textPoint">文字位置</param>
    /// <param name="_height">高度</param>
    /// <param name="_width">宽度</param>
    /// <param name="_fnt">字体</param>
    /// <param name="_fontColor">字体颜色</param>
    /// <param name="_selectColor">选中背景色</param>
    /// <param name="_mouseMove">鼠标移动到项目上的事件</param>
    public void Menu(Control obj, int _totalNum, string[] _menutext, Point[] _textPoint, int _height, int[] _width, Font _fnt, Color _fontColor, Color _selectColor, Color _fontSelectColor, MouseEventHandler _mouseMove)
    {
        total = _totalNum;
        seletColor = _selectColor;
        fontColor = _fontColor;
        fontSelectColor = _fontSelectColor;

        text_label = new Label[_totalNum];

        for (int i = 0; i < _totalNum; i++)
        {
            text_label[i] = new Label()
            {
                Size = new Size(_width[i], _height),
                Location = _textPoint[i],
                Font = _fnt,
                ForeColor = _fontColor,
                Text = _menutext[i],
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = i
            };
            text_label[i].MouseEnter += Textlabel_MouseEnter;
            text_label[i].MouseMove += _mouseMove;
            obj.Controls.Add(text_label[i]);
        }
    }

    private void Textlabel_MouseEnter(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        for (int i = 0; i < total; i++)
        {
            if (i == (int)pL.Tag)
            {
                text_label[i].BackColor = seletColor;
                text_label[i].ForeColor = fontSelectColor;
            }
            else
            {
                text_label[i].BackColor = Color.Transparent;
                text_label[i].ForeColor = fontColor;
            }
        }
    }
}

