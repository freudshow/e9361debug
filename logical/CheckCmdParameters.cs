using E9361Debug.Maintain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.Hosting;
using System.Windows.Media;

namespace E9361Debug.Logical
{
    public class RealDatabaseCmdParameters
    {
        public ushort RealDataBaseNo { get; set; }
        public RealDataTeleTypeEnum TeleType { get; set; }
        public RealDataDataTypeEnum DataType { get; set; }
        public byte DataItemCount { get; set; }
    }

    public class YKOperateParameters
    {
        public YKOperateTypeEnum YKOperateType { get; set; }
        public byte YKNo { get; set; }
        public YKOnOffEnum YKOperation { get; set; }
        public int DelayTime { get; set; }
    }

    public class SftpFileTransferParameters
    {
        public bool IsUploadFileToTerminal = true;
        public string FullFileNameTerminal;
        public string FullFileNameComputer;
    }

    public class MqttPublishParameters
    {
        public string Topic { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// 交采芯片误差限的类型
    /// </summary>
    public enum MeterErrorType
    {
        Error_ABS_Value,    //绝对值
        Error_Percent,      //百分比
        Error_Permillage,   //千分比
    }

    public class ADEErrorParameter : PropertyChangedClass
    {
        private ushort m_RealDatabaseNo;
        private string m_ItemName;
        private double m_StandardValue;
        private double m_ActualValue;

        private MeterErrorType m_ErrorThresholdType;
        private double m_ErrorThreshold;
        private string m_ErrorThresholdString;
        private double m_Actual_Error;
        private string m_ActualErrorString;

        private bool m_Result;
        private string m_RessultString;
        private SolidColorBrush m_ForegroundColor;

        /// <summary>
        /// 实时库序号
        /// </summary>
        public ushort RealDatabaseNo
        {
            get => m_RealDatabaseNo;
            set
            {
                m_RealDatabaseNo = value;
                OnPropertyChanged(nameof(RealDatabaseNo));
            }
        }

        /// <summary>
        /// 数据项名称
        /// </summary>
        public string ItemName
        {
            get => m_ItemName;
            set
            {
                m_ItemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        /// <summary>
        /// 标准值
        /// </summary>
        public double StandardValue
        {
            get => m_StandardValue;
            set
            {
                m_StandardValue = value;
                OnPropertyChanged(nameof(StandardValue));
            }
        }

        /// <summary>
        /// 实际值
        /// </summary>
        [JsonIgnore]
        public double ActualValue
        {
            get => m_ActualValue;
            set
            {
                m_ActualValue = value;

                double abs = Math.Abs(value - StandardValue);
                switch (ErrorThresholdType)
                {
                    case MeterErrorType.Error_ABS_Value:
                        ActualError = abs;
                        break;

                    case MeterErrorType.Error_Percent:
                        ActualError = (abs / StandardValue) * 100;
                        break;

                    case MeterErrorType.Error_Permillage:
                        ActualError = (abs / StandardValue) * 1000;
                        break;

                    default:
                        ActualError = double.PositiveInfinity;
                        break;
                }

                OnPropertyChanged(nameof(ActualValue));
            }
        }

        /// <summary>
        /// 误差类型
        /// 0 - 绝对值
        /// 1 - 百分比
        /// 2 - 千分比
        /// </summary>
        public MeterErrorType ErrorThresholdType
        {
            get => m_ErrorThresholdType;
            set
            {
                m_ErrorThresholdType = value;
                OnPropertyChanged(nameof(ErrorThresholdType));
            }
        }

        /// <summary>
        /// 误差限, 1.2, %1, 5‰等
        /// </summary>
        public double ErrorThreshold
        {
            get => m_ErrorThreshold;
            set
            {
                m_ErrorThreshold = value;

                switch (ErrorThresholdType)
                {
                    case MeterErrorType.Error_ABS_Value:
                        ErrorThresholdString = $"|{value}|";
                        break;

                    case MeterErrorType.Error_Percent:
                        ErrorThresholdString = $"{value}%";
                        break;

                    case MeterErrorType.Error_Permillage:
                        ErrorThresholdString = $"{value}‰";
                        break;

                    default:
                        ErrorThresholdString = "未知误差类型";
                        break;
                }

                OnPropertyChanged(nameof(ErrorThreshold));
            }
        }

        /// <summary>
        /// 呈现给用户的误差限
        /// </summary>
        [JsonIgnore]
        public string ErrorThresholdString
        {
            get => m_ErrorThresholdString;
            set
            {
                m_ErrorThresholdString = value;
                OnPropertyChanged(nameof(ErrorThresholdString));
            }
        }

        /// <summary>
        /// 实际误差
        /// </summary>
        [JsonIgnore]
        public double ActualError
        {
            get => m_Actual_Error;
            set
            {
                m_Actual_Error = value;

                switch (ErrorThresholdType)
                {
                    case MeterErrorType.Error_ABS_Value:
                        ActualErrorString = $"|{value.ToString("0.000")}|";
                        break;

                    case MeterErrorType.Error_Percent:
                        ActualErrorString = $"{value.ToString("0.000")}%";
                        break;

                    case MeterErrorType.Error_Permillage:
                        ActualErrorString = $"{value.ToString("0.000")}‰";
                        break;

                    default:
                        break;
                }

                Result = value < ErrorThreshold;
                OnPropertyChanged(nameof(ActualError));
            }
        }

        /// <summary>
        /// 实际误差
        /// </summary>
        [JsonIgnore]
        public string ActualErrorString
        {
            get => m_ActualErrorString;
            set
            {
                m_ActualErrorString = value;
                OnPropertyChanged(nameof(ActualErrorString));
            }
        }

        /// <summary>
        /// 当前项是否合格
        /// </summary>
        [JsonIgnore]
        public bool Result
        {
            get => m_Result;
            set
            {
                m_Result = value;
                ResultString = value ? "合格" : "不合格";
                ForegroundColor = value ? Brushes.Green : Brushes.Red;
                OnPropertyChanged(nameof(Result));
            }
        }

        /// <summary>
        /// 呈现给用户的检测结果
        /// </summary>
        [JsonIgnore]
        public string ResultString
        {
            get => m_RessultString;
            set
            {
                m_RessultString = value;
                OnPropertyChanged(nameof(ResultString));
            }
        }

        /// <summary>
        /// 前景颜色
        /// </summary>
        [JsonIgnore]
        public SolidColorBrush ForegroundColor
        {
            get => m_ForegroundColor;
            set
            {
                m_ForegroundColor = value;
                OnPropertyChanged(nameof(ForegroundColor));
            }
        }
    }

    public class OneRouteADEError
    {
        /// <summary>
        /// 采样通道索引
        /// 从0开始, 连续增1.
        /// 取值范围0~5
        /// </summary>
        public int RouteNo;

        public bool IsResultPass
        {
            get
            {
                bool res = true;
                if (ItemList != null && ItemList.Count > 0)
                {
                    foreach (var item in ItemList)
                    {
                        if (!item.Result)
                        {
                            res = false;
                            break;
                        }
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// 参数列表
        /// </summary>
        public ObservableCollection<ADEErrorParameter> ItemList;
    }

    public class MultiRouteADEError
    {
        /// <summary>
        /// 采样通道列表
        /// </summary>
        public ObservableCollection<OneRouteADEError> RouteList;
    }
}