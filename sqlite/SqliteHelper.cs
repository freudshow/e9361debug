using E9361Debug.Log;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace E9361Debug.DBHelper
{
    public class SQLiteHelper
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

        public static SQLiteDataReader ExecuteReader(string strSQL)
        {
            SQLiteDataReader sr;
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand(strSQL, connection);

            try
            {
                connection.Open();
                sr = command.ExecuteReader();
            }
            catch (SQLiteException ex)
            {
                throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
            }

            return sr;
        }

        public static SQLiteDataReader ExecuteReader(string SQLString, params SQLiteParameter[] cmdParms)
        {
            SQLiteDataReader sr;
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                PrepareCommand(cmd, conn, null, SQLString, cmdParms);
                SQLiteDataReader reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                sr = reader;
            }
            catch (SQLiteException ex)
            {
                throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
            }

            return sr;
        }

        public static int ExecuteSql(string SQLString)
        {
            int num;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(SQLString, connection);

                try
                {
                    connection.Open();
                    num = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    connection.Close();
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }

            return num;
        }

        public static int ExecuteSql(string SQLString, string content)
        {
            int num;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(SQLString, connection);
                SQLiteParameter parameter = new SQLiteParameter("@content", DbType.String)
                {
                    Value = content
                };
                command.Parameters.Add(parameter);

                try
                {
                    connection.Open();
                    num = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }

            return num;
        }

        public static int ExecuteSql(string SQLString, params SQLiteParameter[] cmdParms)
        {
            int num;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();

                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }

            return num;
        }

        public static int ExecuteSqlByTime(string SQLString, int Times)
        {
            int num;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(SQLString, connection);

                try
                {
                    connection.Open();
                    command.CommandTimeout = Times;
                    num = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    connection.Close();
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }

            return num;
        }

        public static object ExecuteSqlGet(string SQLString, string content)
        {
            object obj;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(SQLString, connection);
                SQLiteParameter parameter = new SQLiteParameter("@content", DbType.String)
                {
                    Value = content
                };
                command.Parameters.Add(parameter);

                try
                {
                    connection.Open();
                    object objA = command.ExecuteScalar();
                    obj = (Equals(objA, null) || Equals(objA, DBNull.Value)) ? null : objA;
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }

            return obj;
        }

        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            int num;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(strSQL, connection);
                SQLiteParameter parameter = new SQLiteParameter("@fs", DbType.Binary)
                {
                    Value = fs
                };
                command.Parameters.Add(parameter);

                try
                {
                    connection.Open();
                    num = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }

            return num;
        }

        public static void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection
                };
                SQLiteTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    int num = 0;
                    while (true)
                    {
                        if (num >= SQLStringList.Count)
                        {
                            transaction.Commit();
                            break;
                        }
                        string str = SQLStringList[num].ToString();
                        if (str.Trim().Length > 1)
                        {
                            command.CommandText = str;
                            command.ExecuteNonQuery();
                        }
                        num++;
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
            }
        }

        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    SQLiteCommand cmd = new SQLiteCommand();

                    try
                    {
                        foreach (DictionaryEntry entry in SQLStringList)
                        {
                            string cmdText = entry.Key.ToString();
                            SQLiteParameter[] cmdParms = (SQLiteParameter[])entry.Value;
                            PrepareCommand(cmd, connection, transaction, cmdText, cmdParms);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                    }
                }
            }
        }

        public static bool Exists(string strSql)
        {
            object single = GetSingle(strSql);
            int num = (Equals(single, null) || Equals(single, DBNull.Value)) ? 0 : int.Parse(single.ToString());

            return (num != 0);
        }

        public static bool Exists(string strSql, params SQLiteParameter[] cmdParms)
        {
            object single = GetSingle(strSql, cmdParms);
            int num = (Equals(single, null) || Equals(single, DBNull.Value)) ? 0 : int.Parse(single.ToString());

            return (num != 0);
        }

        public static int GetMaxID(string FieldName, string TableName)
        {
            object single = GetSingle("select max(" + FieldName + ")+1 from " + TableName);

            return ((single != null) ? int.Parse(single.ToString()) : 1);
        }

        public static object GetSingle(string SQLString)
        {
            object obj;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(SQLString, connection);

                try
                {
                    connection.Open();
                    object objA = command.ExecuteScalar();
                    obj = (Equals(objA, null) || Equals(objA, DBNull.Value)) ? null : objA;
                }
                catch (SQLiteException ex)
                {
                    connection.Close();
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }

            return obj;
        }

        public static object GetSingle(string SQLString, params SQLiteParameter[] cmdParms)
        {
            object obj;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();

                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    object objA = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    obj = (Equals(objA, null) || Equals(objA, DBNull.Value)) ? null : objA;
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }

            return obj;
        }

        public static SQLiteParameter MakeSQLiteParameter(string name, DbType type, object value)
        {
            return new SQLiteParameter(name, type) { Value = value };
        }

        public static SQLiteParameter MakeSQLiteParameter(string name, DbType type, int size, object value)
        {
            return new SQLiteParameter(name, type, size) { Value = value };
        }

        public static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, string cmdText, SQLiteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
            {
                cmd.Transaction = trans;
            }

            cmd.CommandType = CommandType.Text;

            if (cmdParms != null)
            {
                foreach (SQLiteParameter parameter in cmdParms)
                {
                    if (((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Input)) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        public static DataSet Query(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();

                try
                {
                    connection.Open();
                    new SQLiteDataAdapter(SQLString, connection).Fill(ds, "ds");
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }

                return ds;
            }
        }

        public static DataSet Query(string SQLString, int Times)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();

                try
                {
                    connection.Open();
                    new SQLiteDataAdapter(SQLString, connection) { SelectCommand = { CommandTimeout = Times } }.Fill(ds, "ds");
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }

                return ds;
            }
        }

        public static DataSet Query(string SQLString, string TableName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();

                try
                {
                    connection.Open();
                    new SQLiteDataAdapter(SQLString, connection).Fill(ds, TableName);
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                }

                return ds;
            }
        }

        public static DataSet Query(string SQLString, params SQLiteParameter[] cmdParms)
        {
            DataSet ds;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();

                    try
                    {
                        adapter.Fill(dataSet, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SQLiteException ex)
                    {
                        throw new Exception(FileFunctionLine.GetExceptionInfo(ex));
                    }

                    ds = dataSet;
                }
            }

            return ds;
        }
    }
}