using E9361Debug.Common;
using E9361Debug.Communication;
using E9361Debug.DBHelper;
using System;
using System.Data;
using System.Collections.Generic;

namespace E9361Debug.Logical
{
    internal enum BaseParaEnum
    {
        Base_Para_Invalid = -1,
        Base_Para_IP_Address = 1,
        Base_Para_SSH_Port,
        Base_Para_User,
        Base_Para_Passwd,
        Base_Para_Mqtt_Publish_Topic,
        Base_Para_Mqtt_Response_Topic,
        Base_Para_Mqtt_Port,
        Base_Para_Maintain_Default_UDP_Port,
        Base_Para_Main_Check_Table,
        Base_Para_Maintain_Port_Type,
        Base_Para_Maintain_Default_TCP_Client_Port,
        Base_Para_Upload_Directory,
        Base_Para_Download_Directory,
    }

    public static class ExtendEnum
    {
        public static int ToInt(this Enum e)
        {
            return e.GetHashCode();
        }
    }

    public static class DataBaseLogical
    {
        public static DataTable GetBaseParams()
        {
            try
            {
                DataSet dt = SQLiteHelper.Query("select * from t_basePara", "t_basePara");
                if (dt == null || dt.Tables == null || dt.Tables.Count <= 0)
                {
                    return null;
                }

                return dt.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetBaseParamBySeq(int seq)
        {
            try
            {
                DataSet dt = SQLiteHelper.Query($"select * from t_basePara where seq={seq}", "t_basePara");
                if (dt == null || dt.Tables == null || dt.Tables.Count <= 0)
                {
                    return null;
                }

                return dt.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetBaseCheckTableName()
        {
            DataTable checkTable = GetBaseParamBySeq(BaseParaEnum.Base_Para_Main_Check_Table.ToInt());
            if (checkTable == null || checkTable.Rows == null || checkTable.Rows.Count <= 0)
            {
                return null;
            }

            return checkTable.Rows[0]["value"].ToString();
        }

        public static DataTable GetCheckItemsByTableName(string tablename)
        {
            DataSet dt = SQLiteHelper.Query($"select * from {tablename}", tablename);
            if (dt == null || dt.Tables == null || dt.Tables.Count <= 0)
            {
                return null;
            }

            return dt.Tables[0];
        }

        public static string GetTerminalIP()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_IP_Address.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            return dt.Rows[0]["value"].ToString();
        }

        public static int GetTerminalSSHPort()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_SSH_Port.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return -1;
            }

            return Convert.ToInt32(dt.Rows[0]["value"]);
        }

        public static string GetTerminalSSHUserName()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_User.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            return dt.Rows[0]["value"].ToString();
        }

        public static string GetTerminalSSHPasswd()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_Passwd.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            return dt.Rows[0]["value"].ToString();
        }

        public static int GetTerminalUDPPort()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_Maintain_Default_UDP_Port.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return -1;
            }

            return Convert.ToInt32(dt.Rows[0]["value"].ToString());
        }

        public static int GetTerminalTCPClientPort()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_Maintain_Default_TCP_Client_Port.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return -1;
            }

            return Convert.ToInt32(dt.Rows[0]["value"].ToString());
        }

        public static PortTypeEnum GetMaintainPortType()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_Maintain_Port_Type.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return PortTypeEnum.PortType_Error;
            }

            return CommonClass.GetEnumByString<PortTypeEnum>(dt.Rows[0]["value"].ToString());
        }

        public static int GetMqttPort()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_Mqtt_Port.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return -1;
            }

            return Convert.ToInt32(dt.Rows[0]["value"].ToString());
        }

        public static string GetConsoleComName()
        {
            DataSet ds = SQLiteHelper.Query("select value from t_runtimeVariable where name='Console_Port_name'", "t_runtimeVariable");
            if (ds == null || ds.Tables == null || ds.Tables[0].Rows == null || ds.Tables[0].Rows.Count <= 0)
            {
                return "";
            }

            return ds.Tables[0].Rows[0]["value"].ToString();
        }

        public static int GetConsoleComBaudRate()
        {
            DataSet ds = SQLiteHelper.Query("select value from t_runtimeVariable where name='Console_Port_Baud'", "t_runtimeVariable");
            if (ds == null || ds.Tables == null || ds.Tables[0].Rows == null || ds.Tables[0].Rows.Count <= 0)
            {
                return -1;
            }

            return Convert.ToInt32(ds.Tables[0].Rows[0]["value"]);
        }

        public static DataTable GetBaudrateList()
        {
            DataSet ds = SQLiteHelper.Query("select enum, enumName from t_portBaudrateEnum", "t_portBaudrateEnum");
            if (ds == null || ds.Tables == null || ds.Tables[0].Rows == null || ds.Tables[0].Rows.Count <= 0)
            {
                return null;
            }

            return ds.Tables[0];
        }

        public static List<string> GetMqttTopicList()
        {
            DataSet ds = SQLiteHelper.Query("select topics from t_mqttTopics", "t_mqttTopics");
            if (ds == null || ds.Tables == null || ds.Tables[0].Rows == null || ds.Tables[0].Rows.Count <= 0)
            {
                return null;
            }

            List<string> topics = new List<string>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                topics.Add(dr["topics"].ToString());
            }

            return topics;
        }

        public static string GetUploadDirectory()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_Upload_Directory.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            return dt.Rows[0]["value"].ToString();
        }

        public static string GetDownloadDirectory()
        {
            DataTable dt = GetBaseParamBySeq(BaseParaEnum.Base_Para_Download_Directory.ToInt());
            if (dt == null || dt.Rows == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            return dt.Rows[0]["value"].ToString();
        }

        public static bool SaveComName(string comname)
        {
            string selectsql = "select * from t_runtimeVariable where name='Console_Port_name';";
            DataSet ds = SQLiteHelper.Query(selectsql, "t_runtimeVariable");
            if (ds == null || ds.Tables == null || ds.Tables[0].Rows == null || ds.Tables[0].Rows.Count <= 0)
            {
                string insertsql = $"insert into t_runtimeVariable (name, value) values ('Console_Port_name','{comname}')";
                return SQLiteHelper.ExecuteSql(insertsql) > 0;
            }
            else
            {
                string updatesql = $"update t_runtimeVariable set value='{comname}' where name='Console_Port_name';";
                return SQLiteHelper.ExecuteSql(updatesql) > 0;
            }
        }

        public static bool SaveBaudRate(int baudrate)
        {
            string selectsql = "select * from t_runtimeVariable where name='Console_Port_Baud';";
            DataSet ds = SQLiteHelper.Query(selectsql, "t_runtimeVariable");
            if (ds == null || ds.Tables == null || ds.Tables[0].Rows == null || ds.Tables[0].Rows.Count <= 0)
            {
                string insertsql = $"insert into t_runtimeVariable (name, value) values ('Console_Port_Baud',{baudrate})";
                return SQLiteHelper.ExecuteSql(insertsql) > 0;
            }
            else
            {
                string updatesql = $"update t_runtimeVariable set value={baudrate} where name='Console_Port_Baud';";
                return SQLiteHelper.ExecuteSql(updatesql) > 0;
            }
        }
    }
}