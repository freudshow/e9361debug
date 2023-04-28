using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using E9361App.Common;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E9361Debug.Logical
{
    public enum CommandType
    {
        Cmd_Type_Invalid = -1,
        Cmd_Type_MaintainFrame = 0,
        Cmd_Type_Shell,
        Cmd_Type_Mqtt,
        Cmd_Type_MaintainReadRealDataBase,
    }

    public enum ResultTypeEnum
    {
        Result_Type_Invalid = -1,
        Result_Type_Int32 = 0,
        Result_Type_Double,
        Result_Type_Boolean,
        Result_Type_Positive_Infinity,
        Result_Type_Negtive_Infinity,
        Result_Type_Byte_Array,
        Result_Type_String,
    }

    public enum ResultSignEnum
    {
        Result_Sign_Invalid = -1,
        Result_Sign_Equal = 0,
        Result_Sign_Greater_Than,
        Result_Sign_Less_Than,
        Result_Sign_Interval,
        Result_Sign_Regex,
        Result_Sign_Lambda
    }

    internal class PropertyChangedClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    internal class CheckItems : PropertyChangedClass
    {
        private int m_Seq;
        private CommandType m_CmdType;
        private string m_CmdParam;
        private ResultTypeEnum m_ResultType;
        private string m_ResultValue;
        private ResultSignEnum m_ResultSign;
        private string m_Description;
        private bool m_IsEnable;
        private int m_TimeOut;
        private string m_ChildTableName;
        private ObservableCollection<CheckItems> m_Children;

        public int Seq
        {
            get => m_Seq;
            set
            {
                m_Seq = value; OnPropertyChanged(nameof(Seq));
            }
        }

        public CommandType CmdType
        {
            get => m_CmdType;
            set
            {
                m_CmdType = value; OnPropertyChanged(nameof(CmdType));
            }
        }

        public string CmdParam
        {
            get => m_CmdParam;
            set
            {
                m_CmdParam = value; OnPropertyChanged(nameof(CmdParam));
            }
        }

        public ResultTypeEnum ResultType
        {
            get => m_ResultType;
            set
            {
                m_ResultType = value; OnPropertyChanged(nameof(ResultType));
            }
        }

        public string ResultValue
        {
            get => m_ResultValue;
            set
            {
                m_ResultValue = value; OnPropertyChanged(nameof(ResultValue));
            }
        }

        public ResultSignEnum ResultSign
        {
            get => m_ResultSign;
            set
            {
                m_ResultSign = value; OnPropertyChanged(nameof(ResultSign));
            }
        }

        public string Description
        {
            get => m_Description;
            set
            {
                m_Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public bool IsEnable
        {
            get => m_IsEnable;
            set
            {
                m_IsEnable = value;
                if (Children != null && Children.Count > 0)
                {
                    foreach (CheckItems item in Children)
                    {
                        item.IsEnable = value;
                    }
                }

                OnPropertyChanged(nameof(IsEnable));
            }
        }

        public int TimeOut
        {
            get => m_TimeOut;
            set
            {
                m_TimeOut = value;
                OnPropertyChanged(nameof(TimeOut));
            }
        }

        public string ChildTableName
        {
            get => m_ChildTableName;
            set
            {
                m_ChildTableName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    DataTable dt = DataBaseLogical.GetCheckItemsByTableName(value);

                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                        Children = new ObservableCollection<CheckItems>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            CheckItems item = new CheckItems
                            {
                                Seq = Convert.ToInt32(dr["seq"]),
                                CmdType = dr["cmdType"] == DBNull.Value ? CommandType.Cmd_Type_Invalid : (CommandType)Convert.ToInt32(dr["cmdType"]),
                                CmdParam = dr["cmdParam"] == DBNull.Value ? "" : dr["cmdParam"].ToString(),
                                ResultType = dr["resultType"] == DBNull.Value ? ResultTypeEnum.Result_Type_Invalid : (ResultTypeEnum)Convert.ToInt32(dr["resultType"]),
                                ResultValue = dr["resultValue"] == DBNull.Value ? "" : dr["resultValue"].ToString(),
                                ResultSign = dr["resultSign"] == DBNull.Value ? ResultSignEnum.Result_Sign_Invalid : (ResultSignEnum)Convert.ToInt32(dr["resultSign"]),
                                Description = dr["description"] == DBNull.Value ? "" : dr["description"].ToString(),
                                IsEnable = dr["isEnable"] == DBNull.Value ? false : Convert.ToInt32(dr["isEnable"]) == 1 ? true : false,
                                ChildTableName = dr["childTableName"].ToString(),
                            };

                            Children.Add(item);
                        }
                    }
                }

                OnPropertyChanged(nameof(ChildTableName));
            }
        }

        public ObservableCollection<CheckItems> Children
        {
            get => m_Children;
            set
            {
                m_Children = value;
                OnPropertyChanged(nameof(Children));
            }
        }
    }

    public class CheckProcess
    {
        public static async Task<bool> JudgeResultBySign<S, T>(S result, T target, ResultSignEnum sign)
        {
            bool testResult = false;

            switch (sign)
            {
                case ResultSignEnum.Result_Sign_Equal://相等
                    Func<S, bool> equalEvaluate = await Common.GetLambdaAsync<S>($"(x)=>x=={target}");
                    testResult = equalEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Greater_Than://大于等于
                    Func<S, bool> greaterEvaluate = await Common.GetLambdaAsync<S>($"(x)=>x>={target}");
                    testResult = greaterEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Less_Than://小于等于
                    Func<S, bool> lessEvaluate = await Common.GetLambdaAsync<S>($"(x)=>x<={target}");
                    testResult = lessEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Interval://区间
                    string[] bounds = target.ToString().Split(',');
                    Func<S, bool> intervalEvaluate = await Common.GetLambdaAsync<S>($"(x)=>x>={bounds[0]}&&x<={bounds[1]}");
                    testResult = intervalEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Regex://正则表达式
                    Regex re = new Regex(target.ToString(), RegexOptions.Compiled);
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        testResult = re.IsMatch(result.ToString());
                    }

                    break;

                case ResultSignEnum.Result_Sign_Lambda://lambda表达式
                    Func<S, bool> lambdaEvaluate = await Common.GetLambdaAsync<S>(target.ToString());
                    testResult = lambdaEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Invalid://无效
                default:
                    testResult &= false;
                    break;
            }

            return testResult;
        }
    }
}