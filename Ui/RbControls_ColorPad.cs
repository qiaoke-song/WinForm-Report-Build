using System;
using System.Drawing;
using System.Windows.Forms;
using static RbControls;

public class RbControls_ColorPad
{
    public Color selectColor;
    private Color[] colors = new Color[]
    {
        Color.FromArgb(0,0,0), Color.FromArgb(64,64,64), Color.FromArgb(255,0,0),Color.FromArgb(255,106,0),
        Color.FromArgb(225,216,0), Color.FromArgb(182,255,0), Color.FromArgb(76,255,0),Color.FromArgb(0,255,33),
        Color.FromArgb(0,255,144), Color.FromArgb(0,255,255), Color.FromArgb(0,148,255),Color.FromArgb(0,38,255),
        Color.FromArgb(72,0,255), Color.FromArgb(178,0,255), Color.FromArgb(255,0,255),Color.FromArgb(255,0,110),
    };

    /// <summary>
    /// 选择颜色
    /// </summary>
    /// <param name="obj">在那个控件内</param>
    /// <param name="_point">位置</param>
    /// <param name="_size">大小</param>
    /// <param name="_step">间隔</param>
    /// <param name="_borderColor">边框颜色</param>
    /// <param name="_mouseClick">鼠标点击事件</param>
    public void colorPad(Control obj, Point _point, Size _size, int _step, Color _borderColor, MouseEventHandler _mouseClick)
    {
        selectColor = colors[0];

        for (int i = 0; i < 16; i++)
        {
            PanelEx panel_Bar = new PanelEx()
            {
                Size = _size,
                Location = new Point(_point.X + i * _step, _point.Y),
                BackColor = _borderColor,
                Tag = i
            };
            obj.Controls.Add(panel_Bar);

            PanelEx panel_color = new PanelEx()
            {
                Size = new Size(_size.Width - 2, _size.Height - 2),
                Location = new Point(1, 1),
                Cursor = Cursors.Hand,
                BackColor = colors[i],
                Tag = i
            };
            panel_color.Click += panelColor_Click;
            panel_color.MouseClick += _mouseClick;
            panel_Bar.Controls.Add(panel_color);
        }
    }

    private void panelColor_Click(object sender, EventArgs e)
    {
        PanelEx pL = (PanelEx)sender;
        selectColor = pL.BackColor;
    }
}

