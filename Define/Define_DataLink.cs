using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

public static class Define_DataLink
{
    /// <summary>
    /// 数据库连接
    /// </summary>
    public static SqlConnection SQLSERVER_Conn; // sql 连接
    public static OleDbConnection ACCESS_Conn; // access 连接

    public static SqlDataAdapter Sql_Cont;
    public static OleDbDataAdapter Access_Cont;

    public static SqlCommand SqlCom;
    public static OleDbCommand AccessCom;

    public static DataSet ds;
    public static DataTable dt;

    public static string Sql_Select = "";

    /// <summary>
    /// 数据库全部表名
    /// </summary>
    public class Data_TableName
    {
        public string tableName;
        public Data_TableName(string _name)
        {
            tableName = _name;
        }
    }
    public static List<Data_TableName> data_TableNames = new List<Data_TableName>();

    /// <summary>
    /// 数据表全部字段
    /// </summary>
    public class Data_Field
    {
        public string Field_Name;
        public string Field_Type;
        public Data_Field(string _name, string _type)
        {
            Field_Name = _name;
            Field_Type = _type;
        }
    }
    public static List<Data_Field> data_Fields = new List<Data_Field>();

    [Serializable]
    public class Data_Pool
    {
        public int data_Type = 0; // 连接标志，0.none,1.sql,2.Access
        public string data_UserName = ""; // 用户名
        public string data_Pasword = ""; // 密码
        public string data_connectionIP = ""; // 连接地址
        public string data_DataName = ""; // 数据库名
        public string data_TableName = ""; // 数据表名
        public string data_SQL = ""; // sql 语句
    }
    public static Data_Pool data_Pool = new Data_Pool();

    /// <summary>
    /// 初始化数据库连接
    /// </summary>
    /// <param name="_type">连接数据库类型</param>
    /// <param name="_userName">用户名</param>
    /// <param name="_passWord">密码</param>
    /// <param name="_connectionIP">地址</param>
    /// <param name="_dataName">数据表</param>
    /// <returns>连接标志</returns>
    public static void Initialize_DataLink(int _type, string _userName, string _passWord, string _connectionIP, string _dataName)
    {
        //data_Pool.data_Type = -1;
        switch (_type)
        {
            case 1:
                try
                {
                    SQLSERVER_Conn = new SqlConnection("user id=" + _userName + ";data source=" + _connectionIP + ";persist security info=False;initial catalog=" + _dataName + ";password=" + _passWord + ";");
                    SQLSERVER_Conn.Open();
                    data_Pool.data_Type = 1;
                }
                catch { }
                break;
            case 2:
                try
                {
                    ACCESS_Conn = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0;Data Source=" + _dataName + ";Jet OLEDB:Database Password=" + _passWord + ";");
                    ACCESS_Conn.Open();
                    data_Pool.data_Type = 2;
                }
                catch { }
                break;
            default:
                break;
        }
        CloseLink(data_Pool.data_Type);
    }

    /// <summary>
    /// 关闭数据库
    /// </summary>
    /// <param name="_type">数据库类型</param>
    public static void OpenLink(int _type)
    {
        try
        {
            switch (_type)
            {
                case 1:
                    SQLSERVER_Conn.Open();
                    break;
                case 2:
                    ACCESS_Conn.Open();
                    break;
                default:
                    break;
            }
        }
        catch { }
    }

    /// <summary>
    /// 关闭数据库
    /// </summary>
    /// <param name="_type">数据库类型</param>
    public static void CloseLink(int _type)
    {
        try
        {
            switch (_type)
            {
                case 1:
                    SQLSERVER_Conn.Close();
                    break;
                case 2:
                    ACCESS_Conn.Close();
                    break;
                default:
                    break;
            }
        } catch { }
    }

    /// <summary>
    /// 打开数据表
    /// </summary>
    /// <param name="_type">数据库类型</param>
    public static void Set_DataTable(int _type)
    {
        try
        {
            switch (_type)
            {
                case 1:
                    if (data_Pool.data_Type != -1)
                    {
                        try
                        {
                            Sql_Cont = new SqlDataAdapter(Sql_Select, SQLSERVER_Conn);
                        }
                        catch
                        {
                            Sql_Cont = new SqlDataAdapter("SELECT * FROM " + data_Pool.data_TableName, SQLSERVER_Conn);
                        }
                        ds = new DataSet();
                        Sql_Cont.Fill(ds, data_Pool.data_TableName);
                        dt = ds.Tables[data_Pool.data_TableName];
                    }
                    break;
                case 2:
                    if (data_Pool.data_Type != -1)
                    {
                        try
                        {
                            Access_Cont = new OleDbDataAdapter(Sql_Select, ACCESS_Conn);
                        } catch
                        {
                            Access_Cont = new OleDbDataAdapter("SELECT * FROM " + data_Pool.data_TableName, ACCESS_Conn);
                        }
                        ds = new DataSet();
                        Access_Cont.Fill(ds, data_Pool.data_TableName);
                        dt = ds.Tables[data_Pool.data_TableName];
                    }
                    break;
                default:
                    break;
            }
        }
        catch { }
    }


    /// <summary>
    /// 列出全部表名
    /// </summary>
    /// <param name="_type">数据库类型</param>
    public static void GetTableName(int _type)
    {
        data_TableNames.Clear();
        switch (_type)
        {
            case 1: // SQL Server
                try
                {
                    SQLSERVER_Conn.Open();
                    DataTable dts = SQLSERVER_Conn.GetSchema("Tables", null);
                    for (int i = 0; i < dts.Rows.Count; i++)
                    {
                        data_TableNames.Add(new Data_TableName(dts.Rows[i][2].ToString()));
                    }
                    SQLSERVER_Conn.Close();
                } catch { }
                break;
            case 2: // Access
                try
                {
                    ACCESS_Conn.Open();
                    DataTable dta = ACCESS_Conn.GetSchema("Tables");
                    for (int i = 0; i < dta.Rows.Count; i++)
                    {
                        if (dta.Rows[i][3].ToString() == "TABLE")
                        {
                            data_TableNames.Add(new Data_TableName(dta.Rows[i][2].ToString()));
                        }
                    }
                    ACCESS_Conn.Close();
                }
                catch { }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 取得全部字段
    /// </summary>
    /// <param name="_type">数据库类型</param>
    public static void GetField(int _type, string Data_Table)
    {
        data_Fields.Clear();
        string sql = "SELECT TOP 1 * FROM [" + Data_Table + "]";

        switch (_type)
        {
            case 1:
                SQLSERVER_Conn.Open();
                SqlCom = new SqlCommand(sql, SQLSERVER_Conn);

                SqlDataReader drs = SqlCom.ExecuteReader();
                for (int i = 0; i < drs.FieldCount; i++)
                {
                    data_Fields.Add(new Data_Field(drs.GetName(i), ""));
                }
                SQLSERVER_Conn.Close();
                break;
            case 2:
                data_Fields.Clear();
                ACCESS_Conn.Open();
                AccessCom = new OleDbCommand(sql, ACCESS_Conn);
                OleDbDataReader dra = AccessCom.ExecuteReader();
                for (int i = 0; i < dra.FieldCount; i++)
                {
                    data_Fields.Add(new Data_Field(dra.GetName(i), ""));
                }
                ACCESS_Conn.Close();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 字段统计
    /// </summary>
    /// <param name="_cmd">统计命令</param>
    /// <param name="_field">字段</param>
    /// <returns></returns>
    public static string Field_Calculate (string _cmd, string _field)
    {
        string result = "";
        string sql = "";
        switch (_cmd)
        {
            case "SUM":
                sql = Sql_Select.Replace("*", "SUM("+ _field + ")");
                break;
            case "NUMBER":
                sql = Sql_Select.Replace("*", "COUNT(*)");
                break;
            case "AVERAGE":
                sql = Sql_Select.Replace("*", "AVG(" + _field + ")");
                break;
            case "MAX":
                sql = Sql_Select.Replace("*", "MAX("+_field+")");
                break;
            case "MIN":
                sql = Sql_Select.Replace("*", "MIN(" + _field + ")");
                break;
            default:
                break;
        }

        OpenLink(data_Pool.data_Type);

        if (data_Pool.data_Type == 1)
        {
            try
            {
                SqlCom = new SqlCommand(sql, SQLSERVER_Conn);
                var _calc = SqlCom.ExecuteScalar();
                result = _calc.ToString();
            }
            catch { result = "类型错误！"; }
        }
        if (data_Pool.data_Type == 2)
        {
            try
            {
                AccessCom = new OleDbCommand(sql, ACCESS_Conn);
                var _calc = AccessCom.ExecuteScalar();
                result = _calc.ToString();
            }
            catch { result = "类型错误！"; }
        }


        CloseLink(data_Pool.data_Type);

        return result;
    }
}
