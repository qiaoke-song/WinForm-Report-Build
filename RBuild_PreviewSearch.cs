using System;
using System.Drawing;
using System.Windows.Forms;
using static Define_System;
using static Define_ReportFunction;

public class RBuild_PreviewSearch
{
    private RbControls_DialogCreate search_Form = new RbControls_DialogCreate();
    private RbControls_TextBox search_input = new RbControls_TextBox();
    private RbControls_TextLabel search_Result = new RbControls_TextLabel();

    public void Search()
    {
        new RbControls_SpearLine().Spear_line(search_Form.form_panel, new Size(25, 25), new Point(5, 5), EzRBuild.EzResource.search);
        new RbControls_TextLabel().Text_Label(search_Form.form_panel, new Point(30, 7), system_Font,system_FontColor, "查找文字");
        search_input.input_Box(search_Form.form_panel, 190, new Point(10, 32), system_Font,Color.Salmon, Color.White, "", 1024, false, input_Normal, null, null);

        new RbControls_ButtonLabel().Button_Label(search_Form.form_panel, new Point(205, 32), Color.Salmon, Color.Salmon, Color.Salmon, "[×]", system_Font,0, SearchClear_Click);
        new RbControls_ButtonLabel().Button_Label(search_Form.form_panel, new Point(228, 32), Color.Salmon, Color.Salmon, Color.Salmon, "[◢ 确定]", system_Font,1, SearchAccept_Click);

        new RbControls_TextLabel().Text_Label(search_Form.form_panel, new Point(10, 57), system_Font,Color.Black, "查询结果：");

        search_Result.Text_Label(search_Form.form_panel, new Point(75, 57), system_Font,Color.Salmon, "");

        search_Form.Create_Dialog(
               "search_Form", FormStartPosition.Manual, new Size(285, 80), new Point(RBuild_Preview.preview_Form.LocationEX.X + 10, RBuild_Preview.preview_Form.LocationEX.Y + 130),
               Color.White, form_Show, true, null);
    }

    private void SearchAccept_Click(object sender, EventArgs e)
    {
        if (search_input.textBox.Text != "")
        {
            for (int i = 0; i < Total_Page; i++)
            {
                if (RBuild_Preview.panel_Page[i].HasChildren) RBuild_Preview.panel_Page[i].Controls.Clear();
            }
            setSearchs.Clear();

            search_Text(search_input.textBox.Text);
            search_Result.labelText.Text = setSearchs.Count + " 个 .";
        }
    }

    private void SearchClear_Click(object sender, EventArgs e)
    {
        search_input.textBox.Text = "";
    }
}

