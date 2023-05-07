using E9361Debug.Maintain;

namespace E9361Debug.logical
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
}