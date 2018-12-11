using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using static RbControls;
using static Define_System;

public class RbControls_DialogCreate
{
    public winFormEX _formObject = new winFormEX();
    public PanelEx form_panel = new PanelEx();

    public void Create_Dialog(
        string _formName,
        FormStartPosition _startPosition,
        Size _formSize,
        Point _formPoint,
        Color _formBackColor,
        int _formOpenMode,
        bool _formTop,
        FormClosedEventHandler _fromClosed
        )
    {
        Use_HotKey = false;

        _formObject.Name = _formName;
        _formObject.FormBorderStyle = FormBorderStyle.None;
        _formObject.StartPosition = _startPosition;
        _formObject.Size = _formSize;
        _formObject.Location = _formPoint;
        _formObject.BackColor = _formBackColor;
        _formObject.TopMost = _formTop;

        _formObject.Sizeable = false;
        _formObject.FormClosed += _formObject_FormClosed;
        form_panel.Size = new Size(_formObject.Size.Width, _formObject.Size.Height - 1);
        form_panel.BackColor = Color.Transparent;
        form_panel.MouseDown += formMove;
        _formObject.Controls.Add(form_panel);

        PanelEx panel_Close = new PanelEx()
        {
            Size = new Size(18, 17),
            Location = new Point(_formObject.Width - 21, 3),
            Cursor = Cursors.Hand,
            BackColor = Color.FromArgb(247, 139, 94)
        };
        panel_Close.Click += Close_Click; ;
        form_panel.Controls.Add(panel_Close);

        Label label_Close = new Label()
        {
            AutoSize = true,
            Location = new Point(1, 1),
            Font = system_Font,
            Text = "✖",
            Cursor = Cursors.Hand,
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };
        label_Close.Click += Close_Click;
        panel_Close.Controls.Add(label_Close);


        _formObject.FormClosed += _fromClosed;

        if (_formOpenMode == 0) _formObject.Show();
        else _formObject.ShowDialog();

    }

    private void _formObject_FormClosed(object sender, FormClosedEventArgs e)
    {
        Use_HotKey = DialogUse_HotKey;
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
        if ((e.Button == MouseButtons.Left) && (_formObject.WindowState == FormWindowState.Normal)) // 拖动窗体
        {
            ReleaseCapture();
            SendMessage(_formObject.Handle, 0xA1, 0x02, 0);
        }
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

}
