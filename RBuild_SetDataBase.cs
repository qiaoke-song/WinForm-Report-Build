using System;
using System.Drawing;
using System.Windows.Forms;
using static Define_System;
using static Define_DataLink;

public class RBuild_SetDataBase
{
    private RbControls_DialogCreate setData_Form = new RbControls_DialogCreate();
    private RbControls_CheckBox[] set_base = new RbControls_CheckBox[3];
    private RbControls_TextLabel[] data_info = new RbControls_TextLabel[5];
    private RbControls_TextBox[] input_info = new RbControls_TextBox[5];
    private RbControls_TextBox inputSQL = new RbControls_TextBox();
    private ContextMenuStrip DataTable_Menu;
    private string[] dataText = new string[3] { "None", "SQL Server", "Access" };
    private int _link;

    public void Set_Data()
    {
        new RbControls_SpearLine().Spear_line(setData_Form.form_panel, new Size(20, 20), new Point(15, 5), EzRBuild.EzResource.datapool);
        new RbControls_TextLabel().Text_Label(setData_Form.form_panel, new Point(35, 7), system_Font,system_FontColor, "设置数据库");

        new RbControls_SpearLine().Spear_line(setData_Form.form_panel, new Size(340, 5), new Point(5, 55), EzRBuild.EzResource.hspear);
        new RbControls_SpearLine().Spear_line(setData_Form.form_panel, new Size(340, 5), new Point(5, 225), EzRBuild.EzResource.hspear);

        new RbControls_SpearLine().Spear_line(setData_Form.form_panel, new Size(20, 20), new Point(15, 65), EzRBuild.EzResource.data_set);
        new RbControls_TextLabel().Text_Label(setData_Form.form_panel, new Point(35, 67), system_Font,system_FontColor, "数据库连接设置");

        new RbControls_SpearLine().Spear_line(setData_Form.form_panel, new Size(20, 20), new Point(15, 235), EzRBuild.EzResource.sql);
        new RbControls_TextLabel().Text_Label(setData_Form.form_panel, new Point(35, 237), system_Font,system_FontColor, "SQL 语句");
        inputSQL.input_Box(setData_Form.form_panel, 295, new Point(35, 257), system_Font,system_inputColor, Color.White, data_Pool.data_SQL, 4096, false, input_Normal, null, null);

        //string[] acceptSetMenu = new string[2] { "[✔ 应用设置 ]", "[✘ 取消 ]" };
        int[] _setx = new int[3] { 35, 100, 200 };

        for (int i = 0; i < 2; i++)
        {
            new RbControls_ButtonLabel().Button_Label(setData_Form.form_panel, new Point(195 + i * 85, 287), system_buttonColor, system_btnEnter, system_buttonColor, acceptSetMenu[i], system_Font,i, acceptSet_Click);
        }

        _link = data_Pool.data_Type;
        for (int i = 0; i < 3; i++)
        {
            set_base[i] = new RbControls_CheckBox();
            if (i == data_Pool.data_Type) set_base[i].check_Box(setData_Form.form_panel, true, i, new Point(_setx[i], 31), Color.Black, system_Font,dataText[i], check_AlwaysTrue, SetDataLink_Click);
            else set_base[i].check_Box(setData_Form.form_panel, false, i, new Point(_setx[i], 31), Color.Black, system_Font,dataText[i], check_AlwaysTrue, SetDataLink_Click);
        }

        string[] datas = new string[5] { "   用户名：", "      密码：", "链接地址：", "   数据库：", "   数据表：" };
        string _info = "";
        for (int i = 0; i < 5; i++)
        {
            input_info[i] = new RbControls_TextBox();

            if (i == 0) _info = data_Pool.data_UserName;
            else if (i == 1) _info = "";
            else if (i == 2) _info = data_Pool.data_connectionIP;
            else if (i == 3) _info = data_Pool.data_DataName;
            else if (i == 4) _info = data_Pool.data_TableName;

            if (i == 1) input_info[i].input_Box(setData_Form.form_panel, 230, new Point(95, 90 + i * 27), system_Font,system_inputColor, Color.White, _info, 4096, false, input_Pasword, null, null);
            else if (i == 4) input_info[i].input_Box(setData_Form.form_panel, 230, new Point(95, 90 + i * 27), system_Font,system_inputColor, Color.White, _info, 4096, true, input_Normal, null, null);
            else input_info[i].input_Box(setData_Form.form_panel, 230, new Point(95, 90 + i * 27), system_Font,system_inputColor, Color.White, _info, 4096, false, input_Normal, null, null);

            data_info[i] = new RbControls_TextLabel();
            data_info[i].Text_Label(setData_Form.form_panel, new Point(35, 90 + i * 27), system_Font,Color.Black, datas[i]);
        }

        new RbControls_ButtonLabel().Button_Label(setData_Form.form_panel, new Point(325, 199), system_btnEnter, system_btnEnter, system_btnEnter, "◥", system_Font,0, TableSelect_Click);

        if (data_Pool.data_Type == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                input_info[i].textBox.Text = "";
                input_info[i].textBox.Enabled = false;
                data_info[i].labelText.Enabled = false;
            }
            inputSQL.textBox.Text = "";
            inputSQL.textBox.Enabled = false;
        }
        else
        if (data_Pool.data_Type == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                input_info[i].textBox.Enabled = true;
                data_info[i].labelText.Enabled = true;
            }
            inputSQL.textBox.Enabled = true;
        }
        else
        if (data_Pool.data_Type == 2)
        {
            for (int i = 0; i < 5; i++)
            {
                if ((i == 0) || (i == 2))
                {
                    input_info[i].textBox.Text = "";
                    input_info[i].textBox.Enabled = false;
                    data_info[i].labelText.Enabled = false;
                }
                else
                {
                    input_info[i].textBox.Enabled = true;
                    data_info[i].labelText.Enabled = true;
                }
            }
            inputSQL.textBox.Enabled = true;
        }

        DataTable_Menu = new ContextMenuStrip() { ShowImageMargin = false, Font = system_Font };

        setData_Form.Create_Dialog(
            "setData_Form", FormStartPosition.Manual, new Size(350, 315), new Point(RBuild_Design.design_Form.LocationEX.X + 135, RBuild_Design.design_Form.LocationEX.Y + 90),
            Color.White, form_ShowDialog, true, null
        );
    }

    private void SetDataLink_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        if (_link != (int)pL.Tag)
        {
            for (int i = 0; i < 5; i++) input_info[i].textBox.Text = "";
            inputSQL.textBox.Text = "";
        }


        for (int i = 0; i < 3; i++)
        {
            if (i != (int)pL.Tag) set_base[i].set_Flag(false);
        }

        if ((int)pL.Tag == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                input_info[i].textBox.Text = "";
                input_info[i].textBox.Enabled = false;
                data_info[i].labelText.Enabled = false;
            }
            inputSQL.textBox.Text = "";
            inputSQL.textBox.Enabled = false;
        }
        else
        if ((int)pL.Tag == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                input_info[i].textBox.Enabled = true;
                data_info[i].labelText.Enabled = true;
            }
            inputSQL.textBox.Enabled = true;
        }
        else
        if ((int)pL.Tag == 2)
        {
            for (int i = 0; i < 5; i++)
            {
                if ((i == 0) || (i == 2))
                {
                    input_info[i].textBox.Text = "";
                    input_info[i].textBox.Enabled = false;
                    data_info[i].labelText.Enabled = false;
                }
                else
                {
                    input_info[i].textBox.Enabled = true;
                    data_info[i].labelText.Enabled = true;
                }
            }
            inputSQL.textBox.Enabled = true;
        }

        _link = (int)pL.Tag;
    }

    private void acceptSet_Click(object sender, EventArgs e)
    {
        Label pL = (Label)sender;

        if ((int)pL.Tag == 0)
        {
            if (input_info[4].textBox.Text == "")
            {
                CloseLink(data_Pool.data_Type);
                data_Pool.data_Type = 0;
            }
            else
            {
                data_Pool.data_Type = _link;
                data_Pool.data_UserName = input_info[0].textBox.Text;
                data_Pool.data_Pasword = input_info[1].textBox.Text;
                data_Pool.data_connectionIP = input_info[2].textBox.Text;
                data_Pool.data_DataName = input_info[3].textBox.Text;
                data_Pool.data_SQL = inputSQL.textBox.Text;

                Sql_Select = inputSQL.textBox.Text;
            }
            RBuild_Info.Set_CompositeLocation();
        }
        setData_Form._formObject.Close();
    }

    private void TableSelect_Click(object sender, MouseEventArgs e)
    {
        Label pL = (Label)sender;

        DataTable_Menu.Items.Clear();
        if ((_link != 0) && (input_info[3].textBox.Text != ""))
        {
            setData_Form._formObject.Cursor = Cursors.WaitCursor;
            Initialize_DataLink(_link, input_info[0].textBox.Text, input_info[1].textBox.Text, input_info[2].textBox.Text, input_info[3].textBox.Text);
            data_Pool.data_UserName = input_info[0].textBox.Text;
            data_Pool.data_Pasword = input_info[1].textBox.Text;
            data_Pool.data_connectionIP = input_info[2].textBox.Text;
            data_Pool.data_DataName = input_info[3].textBox.Text;
            setData_Form._formObject.Cursor = Cursors.Default;

            GetTableName(_link);
            ToolStripMenuItem[] menu_Item = new ToolStripMenuItem[data_TableNames.Count];
            for (int i = 0; i < data_TableNames.Count; i++)
            {
                menu_Item[i] = new ToolStripMenuItem()
                {
                    AutoSize = true,
                    Font = system_Font,
                    Text = data_TableNames[i].tableName,
                };
                menu_Item[i].Click += DataMenu_Click;
                DataTable_Menu.Items.Add(menu_Item[i]);
            }
        }
        else
        {
            if (_link != 0)
            {
                ToolStripMenuItem menu_Item = new ToolStripMenuItem()
                {
                    AutoSize = true,
                    Font = system_Font,
                    Text = "  数据库连接错误！",
                };
                DataTable_Menu.Items.Add(menu_Item);
            }
        }
        DataTable_Menu.Show(pL.PointToScreen(e.Location));
    }

    private void DataMenu_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem mL = (ToolStripMenuItem)sender;
        input_info[4].textBox.Text = mL.Text;

        if (data_Pool.data_TableName != input_info[4].textBox.Text)
        {
            inputSQL.textBox.Text = "SELECT * FROM " + input_info[4].textBox.Text;
        }
        data_Pool.data_TableName = mL.Text;
    }
}
