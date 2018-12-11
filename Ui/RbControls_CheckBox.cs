using System;
using System.Drawing;
using System.Windows.Forms;

public class RbControls_CheckBox
{
    public bool select = false;
    private Label check_Label;
    private int mode;
    private Point point;

    /// <summary>
    /// checkBox 组件
    /// </summary>
    /// <param name="obj">在那个控件内</param>
    /// <param name="_flag">是否选中</param>
    /// <param name="_tag">tag值</param>
    /// <param name="_point">位置</param>
    /// <param name="_fontColor">字体颜色</param>
    /// <param name="_text">文字</param>
    /// <param name="_mode">模式：普通、始终被选中</param>
    public void check_Box(Control obj, bool _flag, int _tag, Point _point, Color _fontColor, Font _fnt, string _text, int _mode, MouseEventHandler _click)
    {
        point = _point;
        mode = _mode;

        check_Label = new Label()
        {
            AutoSize = true,
            Location = _point,
            Font = _fnt,
            Text = "▄",
            ForeColor = Color.FromArgb(213, 213, 213),
            Cursor = Cursors.Hand,
            BackColor = Color.Transparent,
            Tag = _tag
        };
        check_Label.Click += CheckText_Click;
        check_Label.MouseDown += _click;
        if (_flag)
        {
            check_Label.Location = new Point(_point.X, _point.Y + 3);
            check_Label.ForeColor = Color.FromArgb(162, 37, 13);
            check_Label.Text = "✔";
        }
        obj.Controls.Add(check_Label);

        Label check_Text = new Label()
        {
            AutoSize = true,
            Location = new Point(_point.X + 15, _point.Y + 2),
            Font = _fnt,
            Text = _text,
            ForeColor = _fontColor,
            Cursor = Cursors.Hand,
            BackColor = Color.Transparent,
            Tag = _tag
        };
        check_Text.Click += CheckText_Click;
        check_Text.MouseDown += _click;
        obj.Controls.Add(check_Text);

        select = _flag;
    }

    public void CheckText_Click(object sender, EventArgs e)
    {
        if (mode == 0)
        {
            select = !select;
            if (select)
            {
                check_Label.Text = "✔";
                check_Label.Location = new Point(point.X, point.Y + 3);
                check_Label.ForeColor = Color.FromArgb(162, 37, 13);
            }
            else
            {
                check_Label.Text = "▄";
                check_Label.Location = point;
                check_Label.ForeColor = Color.FromArgb(213, 213, 213);
            }
        }
        else
        if (mode == 1)
        {
            select = true;
            check_Label.Text = "✔";
            check_Label.Location = new Point(point.X, point.Y + 3);
            check_Label.ForeColor = Color.FromArgb(162, 37, 13);
        }
    }

    public void set_Flag(bool _stat)
    {
        if (_stat)
        {
            check_Label.Text = "✔";
            check_Label.Location = new Point(point.X, point.Y + 3);
            check_Label.ForeColor = Color.FromArgb(162, 37, 13);
        }
        else
        {
            check_Label.Text = "▄";
            check_Label.Location = point;
            check_Label.ForeColor = Color.FromArgb(213, 213, 213);
        }
        select = _stat;
    }
}

