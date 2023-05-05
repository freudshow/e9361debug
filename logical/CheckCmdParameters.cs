using E9361App.Maintain;

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
}