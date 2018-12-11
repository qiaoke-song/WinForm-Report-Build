using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static RbControls;

public class RbControls_FormCreate
{
    public winFormEX _formObject = new winFormEX();
    public PanelEx panel_Title = new PanelEx();
    public Size SizeEX;
    public Point LocationEX;
    private PanelEx panelScroll;

    private Bitmap[] Icon_formControl = new Bitmap[4] {
            EzRBuild.EzResource.screen_minimize,
            EzRBuild.EzResource.screen_maximize,
            EzRBuild.EzResource.screen_close,
            EzRBuild.EzResource.screen_rest,
        };
    private PictureBoxEx[] picture_formControl = new PictureBoxEx[3];
    private ToolTip toolTip = new ToolTip();
    private string[] tips_formControl = new string[4] { "最小化", "最大化", "关闭", "还原" };
    private int _type = 0;

    /// <summary>
    /// 建立窗体
    /// </summary>
    /// <param name="_panelScrollObj">窗体内设置带滚动条panel</param>
    /// <param name="_formName">窗体名称</param>
    /// <param name="_startPosition">初次打开位置</param>
    /// <param name="_formSize">大小</param>
    /// <param name="_formPoint">位置</param>
    /// <param name="_formBackColor">背景色</param>
    /// <param name="_formOpenMode">打开方式</param>
    /// <param name="_formTop">置顶</param>
    /// <param name="_fromCloseed">关闭事件</param>
    public void Create_Form(
        PanelEx _panelScrollObj,
        string _formName,
        FormStartPosition _startPosition,
        Size _formSize,
        Point _formPoint,
        Color _formBackColor,
        int _formOpenMode,
        bool _formTop,
        FormClosedEventHandler _fromClosed,
        EventHandler _formResize
        )
    {
        panelScroll = _panelScrollObj;
        _formObject.Name = _formName;
        _formObject.FormBorderStyle = FormBorderStyle.None;
        _formObject.StartPosition = _startPosition;
        _formObject.Size = _formSize;
        _formObject.Location = _formPoint;
        _formObject.BackColor = _formBackColor;
        _formObject.Resize += _formResize;
        _formObject.TopMost = _formTop;

        panel_Title.Dock = DockStyle.Top;
        panel_Title.Height = 60;
        panel_Title.BackgroundImage = EzRBuild.EzResource.background;
        panel_Title.Cursor = Cursors.Default;
        panel_Title.BackgroundImageLayout = ImageLayout.Stretch;
        panel_Title.MouseDown += formMove;
        _formObject.Controls.Add(panel_Title);

        PanelEx panel_formContro = new PanelEx()
        {
            Dock = DockStyle.Right,
            Width = 100,
            BackColor = Color.Transparent
        };
        panel_formContro.MouseDown += formMove;
        panel_Title.Controls.Add(panel_formContro);

        for (int i = 0; i < 3; i++)
        {
            picture_formControl[i] = new PictureBoxEx()
            {
                Size = new Size(25, 25),
                Location = new Point(i * 35, 4),
                Image = Icon_formControl[i],
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Tag = i
            };
            picture_formControl[i].MouseEnter += new EventHandler(formControl_MouseEnter);
            picture_formControl[i].MouseLeave += new EventHandler(formControl_MouseLeave);
            picture_formControl[i].Click += new EventHandler(formControl_Click);
            toolTip.SetToolTip(picture_formControl[i], tips_formControl[i]);
            panel_formContro.Controls.Add(picture_formControl[i]);

        }
        _formObject.FormClosed += _fromClosed;
        if (_formOpenMode == 0) _formObject.Show();
        else _formObject.ShowDialog();

    }

    private void Close_Click(object sender, EventArgs e)
    {
        _formObject.Close();
    }

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();
    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    /// <summary>
    /// 双击改变窗体状态，拖动窗体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void formMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && e.Clicks == 2) // 双击
        {
            if (_type == 0)
            {
                panelScroll.VerticalScroll.Value = 0;
                panelScroll.HorizontalScroll.Value = 0;
                if (_formObject.WindowState == FormWindowState.Normal)
                {
                    _formObject.WindowState = FormWindowState.Maximized;
                    picture_formControl[1].Image = Icon_formControl[3];
                    toolTip.SetToolTip(picture_formControl[1], tips_formControl[3]);
                }
                else
                if (_formObject.WindowState == FormWindowState.Maximized)
                {
                    _formObject.WindowState = FormWindowState.Normal;
                    picture_formControl[1].Image = Icon_formControl[1];
                    toolTip.SetToolTip(picture_formControl[1], tips_formControl[1]);
                }
            }
        }
        else
        if ((e.Button == MouseButtons.Left) && (_formObject.WindowState == FormWindowState.Normal)) // 拖动窗体
        {
            ReleaseCapture();
            SendMessage(_formObject.Handle, 0xA1, 0x02, 0);
        }

        LocationEX = _formObject.Location;
        SizeEX = _formObject.Size;
    }

    /// <summary>
    /// 鼠标在窗体控制按钮内
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void formControl_MouseEnter(object sender, EventArgs e)
    {
        PictureBoxEx pL = (PictureBoxEx)sender;
        pL.BackColor = Color.FromArgb(62, 109, 182);
    }

    /// <summary>
    /// 鼠标离开窗体控制按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void formControl_MouseLeave(object sender, EventArgs e)
    {
        PictureBoxEx pL = (PictureBoxEx)sender;
        pL.BackColor = Color.Transparent;
    }

    /// <summary>
    /// 点击窗体控制按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void formControl_Click(object sender, EventArgs e)
    {
        PictureBoxEx pL = (PictureBoxEx)sender;
        switch ((int)pL.Tag)
        {
            case 0: // 最小化到任务栏
                _formObject.WindowState = FormWindowState.Minimized;
                break;
            case 1: // 最大化
                if (_formObject.WindowState == FormWindowState.Normal)
                {
                    panelScroll.VerticalScroll.Value = 0;
                    panelScroll.HorizontalScroll.Value = 0;
                    _formObject.WindowState = FormWindowState.Maximized;
                    pL.Image = Icon_formControl[3];
                    toolTip.SetToolTip(pL, tips_formControl[3]);
                }
                else // 还原
                {
                    panelScroll.VerticalScroll.Value = 0;
                    panelScroll.HorizontalScroll.Value = 0;
                    _formObject.WindowState = FormWindowState.Normal;
                    pL.Image = Icon_formControl[1];
                    toolTip.SetToolTip(pL, tips_formControl[1]);
                }
                break;
            case 2: // 退出
                _formObject.Close();
                break;
            default:
                break;
        }
    }

}

