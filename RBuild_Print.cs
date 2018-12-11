using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Define_System;
using static Define_PrintPage;
using static Define_ReportFunction;

public class RBuild_Print
{
    private RbControls_DialogCreate print_Form = new RbControls_DialogCreate();
    private static ContextMenuStrip printer_Menu;
    private static ToolStripMenuItem[] printer_Item;

    private RbControls_TextBox Input_Printer, Input_Scope, Input_Copies;
    private RbControls_CheckBox[] select_Cope = new RbControls_CheckBox[3];
    private RbControls_CheckBox[] select_ptype = new RbControls_CheckBox[2];
    private TextBox Input_focuse;
    private RbControls_ButtonLabel[] acceptPrint = new RbControls_ButtonLabel[2];

    private List<int> page_TmpScope = new List<int>();

    public void Print()
    {
        Input_focuse = new TextBox()
        {
            Size = new Size(0, 0),
            Location = new Point(0, 0),
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.White
        };
        print_Form._formObject.Controls.Add(Input_focuse);

        printer_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };
        printer_Item = new ToolStripMenuItem[printers.Count];
        for (int i = 0; i < printers.Count; i++)
        {
            printer_Item[i] = new ToolStripMenuItem()
            {
                AutoSize = true,
                Text = printers[i].printerName,
                Tag = i
            };
            printer_Item[i].Click += printerMenu_Click;
            printer_Menu.Items.Add(printer_Item[i]);
        }

        Bitmap[] printer_Img = new Bitmap[3] { EzRBuild.EzResource.print, EzRBuild.EzResource.prview, EzRBuild.EzResource.copy };
        string[] printer_text = new string[] { "- 打印机选择：", "- 打印范围选择：", "- 打印方式：" };
        string[] printer_scope = new string[] { "全部", "单数页", "双数页" };
        int[] _sety = new int[] { 15, 86, 171 };
        int[] _setx = new int[] { 35, 90, 155 };
        for (int i = 0; i < 3; i++)
        {
            new RbControls_SpearLine().Spear_line(print_Form.form_panel, new Size(25, 25), new Point(10, _sety[i] - 2), printer_Img[i]);
            new RbControls_TextLabel().Text_Label(print_Form.form_panel, new Point(35, _sety[i]), system_Font,system_FontColor, printer_text[i]);

            select_Cope[i] = new RbControls_CheckBox();
            if (i == 0) select_Cope[i].check_Box(print_Form.form_panel, true, i, new Point(_setx[i], 108), Color.Black, system_Font,printer_scope[i], check_AlwaysTrue, cope_Click);
            else select_Cope[i].check_Box(print_Form.form_panel, false, i, new Point(_setx[i], 108), Color.Black, system_Font,printer_scope[i], check_AlwaysTrue, cope_Click);
        }
        new RbControls_SpearLine().Spear_line(print_Form.form_panel, new Size(380, 5), new Point(5, 70), EzRBuild.EzResource.hspear);
        new RbControls_SpearLine().Spear_line(print_Form.form_panel, new Size(380, 5), new Point(5, 158), EzRBuild.EzResource.hspear);

        Input_Printer = new RbControls_TextBox();
        Input_Printer.input_Box(print_Form.form_panel, 230, new Point(20, 40), system_Font,Color.Salmon, Color.White, DefaultPrinter, 1024, true, input_Normal, printer_Click, null);
        new RbControls_ButtonLabel().Button_Label(print_Form.form_panel, new Point(255, 40), Color.Black, system_btnEnter, Color.Black, "[◢ 选择设备 ]", system_Font,0, SelectPrinter_Click);
        new RbControls_ButtonLabel().Button_Label(print_Form.form_panel, new Point(335, 40), Color.Black, system_btnEnter, Color.Black, "[ 属性 ]", system_Font,0, PrinterAttrib_Click);

        Input_Scope = new RbControls_TextBox();
        Input_Scope.input_Box(print_Form.form_panel, 80, new Point(302, 133), system_Font,Color.Salmon, Color.White, "", 1024, false, input_Normal, null, Scope_KeyPress);
        new RbControls_TextLabel().Text_Label(print_Form.form_panel, new Point(35, 133), system_Font,Color.Black, "输入页码或范围(','或'-'分隔。例如1-5或1,3,4 ...)：");

        Input_Copies = new RbControls_TextBox();
        Input_Copies.input_Box(print_Form.form_panel, 40, new Point(70, 195), system_Font,Color.Salmon, Color.White, "1", 3, false, input_Number, null, null);
        new RbControls_TextLabel().Text_Label(print_Form.form_panel, new Point(35, 195), system_Font,Color.Black, "份数：");

        string[] _ptype = new string[] { "逐份打印", "逐页打印" };
        string[] acceptPrintMenu = new string[2] { "[✔ 确定 ]", "[✘ 取消 ]" };
        for (int i = 0; i < 2; i++)
        {
            select_ptype[i] = new RbControls_CheckBox();
            if (i == 0) select_ptype[i].check_Box(print_Form.form_panel, true, i, new Point(135 + i * 75, 195), Color.Black, system_Font,_ptype[i], check_AlwaysTrue, ptype_Click);
            else select_ptype[i].check_Box(print_Form.form_panel, false, i, new Point(135 + i * 75, 195), Color.Black, system_Font,_ptype[i], check_AlwaysTrue, ptype_Click);

            acceptPrint[i] = new RbControls_ButtonLabel();
            acceptPrint[i].Button_Label(print_Form.form_panel, new Point(260 + i * 65, 225), system_buttonColor, system_btnEnter, system_buttonColor, acceptPrintMenu[i], system_Font,i, AcceptPrinter_Click);
        }

        print_Form.Create_Dialog("print_Form", FormStartPosition.CenterScreen, new Size(390, 250), new Point(100, 200), Color.White, form_ShowDialog, true, null);
    }

    private void cope_Click(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;

        Input_Scope.textBox.Text = "";
        Input_focuse.Focus();

        for (int i = 0; i < 3; i++)
        {
            if (i == (int)pL.Tag) select_Cope[i].set_Flag(true);
            else select_Cope[i].set_Flag(false);
        }
    }

    private void ptype_Click(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;
        for (int i = 0; i < 2; i++)
        {
            if (i == (int)pL.Tag) select_ptype[i].set_Flag(true);
            else select_ptype[i].set_Flag(false);
        }
    }

    private void Scope_KeyPress(object sender, KeyEventArgs e)
    {
        TextBox tL = (TextBox)sender;
        if ((tL.Text).Trim() == "")
        {
            Input_focuse.Focus();
            select_Cope[0].set_Flag(true);
        }
        else
        {
            for (int i = 0; i < 3; i++) select_Cope[i].set_Flag(false);
        }
    }

    private void AcceptPrinter_Click(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;

        if ((int)pL.Tag == 1)
        {
            print_Form._formObject.Close();
        }
        else
        {
            page_TmpScope.Clear();

            if (Input_Scope.textBox.Text == "") // 打印范围选项
            {
                if (select_Cope[0].select) // 全部页
                {
                    for (int i = 0; i < Total_Page; i++) page_TmpScope.Add(i);
                }
                else
                if (select_Cope[1].select) // 单数页
                {
                    for (int i = 0; i < Total_Page; i++)
                    {
                        if (i % 2 != 0) page_TmpScope.Add(i);
                    }
                }
                else
                if (select_Cope[2].select) // 双数页
                {
                    for (int i = 0; i < Total_Page; i++)
                    {
                        if (i % 2 == 0) page_TmpScope.Add(i);
                    }
                }
            }
            else // 指定打印范围
            {
                string[] splitString = Input_Scope.textBox.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splitString.Length; i++)
                {
                    int n;
                    if (int.TryParse(splitString[i], out n))
                    {
                        page_TmpScope.Add(int.Parse(splitString[i]));
                    }
                    else
                    {
                        string[] splitSpear = splitString[i].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                        if (splitSpear.Length == 2)
                        {
                            int n1 = -1, n2 = -1;
                            int.TryParse(splitSpear[0], out n1);
                            int.TryParse(splitSpear[1], out n2);

                            if ((n1 != -1) && (n2 != -1) && (n1 < n2))
                            {
                                for (int t = n1; t <= n2; t++)
                                {
                                    page_TmpScope.Add(t);
                                }
                            }
                        }
                    }
                }
            }


            if (page_TmpScope.Count > 0)
            {
                if ((Input_Copies.textBox.Text != "") && (int.Parse(Input_Copies.textBox.Text) > 0))
                {
                    page_TmpScope.Sort();
                    page_Scope.Clear();
                    if (select_ptype[0].select) // 逐份打印
                    {
                        for (int i = 0; i < int.Parse(Input_Copies.textBox.Text); i++)
                        {
                            for (int t = 0; t < page_TmpScope.Count; t++) page_Scope.Add(page_TmpScope[t]);
                        }
                    }

                    else
                    if (select_ptype[1].select) // 逐页打印
                    {
                        for (int t = 0; t < page_TmpScope.Count; t++)
                        {
                            for (int i = 0; i < int.Parse(Input_Copies.textBox.Text); i++)
                            {
                                page_Scope.Add(page_TmpScope[t]);
                            }
                        }
                    }
                }
            }

            printDocument.Print();
            print_Form._formObject.Close();
        }
    }

    private void SelectPrinter_Click(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;
        printer_Menu.Show(pL.PointToScreen(new Point(10, 22)));
    }

    private void printer_Click(object sender, EventArgs e)
    {
        Input_focuse.Focus();
    }

    private void printerMenu_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem mL = (ToolStripMenuItem)sender;
        Input_Printer.textBox.Text = mL.Text;
    }

    /// 取得打印机属性
    [System.Runtime.InteropServices.DllImport("winspool.drv", SetLastError = true)]
    public extern static int OpenPrinter(string pPrinterName, ref IntPtr hPrinter, ref IntPtr pDefault);

    [System.Runtime.InteropServices.DllImport("winspool.drv", SetLastError = true)]
    public extern static int DocumentProperties(IntPtr hWnd, IntPtr hPrinter, string pDeviceName, ref IntPtr pDevModeOutput, ref IntPtr pDevModeInput, int fMode);

    [System.Runtime.InteropServices.DllImport("winspool.drv", SetLastError = true)]
    public static extern int ClosePrinter(IntPtr phPrinter);

    private const int DM_PROMPT = 4;

    private void PrinterAttrib_Click(object sender, EventArgs e)
    {
        string printerName = Input_Printer.textBox.Text;

        if (printerName != null && printerName.Length > 0)
        {
            IntPtr pPrinter = IntPtr.Zero;
            IntPtr pDevModeOutput = IntPtr.Zero;
            IntPtr pDevModeInput = IntPtr.Zero;
            IntPtr nullPointer = IntPtr.Zero;

            OpenPrinter(printerName, ref pPrinter, ref nullPointer);

            int iNeeded = DocumentProperties(print_Form._formObject.Handle, pPrinter, printerName, ref pDevModeOutput, ref pDevModeInput, 0);
            pDevModeOutput = System.Runtime.InteropServices.Marshal.AllocHGlobal(iNeeded);
            DocumentProperties(print_Form._formObject.Handle, pPrinter, printerName, ref pDevModeOutput, ref pDevModeInput, DM_PROMPT);
            ClosePrinter(pPrinter);
        }
    }

}
