using System.Threading;
using System.Windows.Forms;

public static class CrossThreadCall
{
    public static void CrossThreadCalls(this Control ctl, ThreadStart del)
    {
        if (del == null) return;
        if (ctl.InvokeRequired)  ctl.Invoke(del, null);
        else del();
    }
}
