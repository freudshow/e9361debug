using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using E9361App.DBHelper;

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
    }

    public static class ExtendEnum
    {
        public static int ToInt(this Enum e)
        {
            return e.GetHashCode();
        }
    }

    internal static class DataBaseLogical
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
    }
}