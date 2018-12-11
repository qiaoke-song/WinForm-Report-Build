using System.Windows.Forms;

public class RbControls
{
    /// <summary>
    /// 双缓冲 panel
    /// </summary>
    public class PanelEx : Panel
    {
        public PanelEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            UpdateStyles();
        }
    }

    /// <summary>
    /// 双缓冲 PictureBox
    /// </summary>
    public class PictureBoxEx : PictureBox
    {
        public PictureBoxEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            UpdateStyles();
        }
    }
}
