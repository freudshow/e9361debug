using System;

namespace E9361App.Log
{
    public enum SRMsgType
    {
        发送报文,
        接收报文,
        报文说明
    }

    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object syslock = new object();

        public static T getInstance()
        {
            if (_instance == null)
            {
                lock (syslock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// 事件参数定义
    /// </summary>
    public class SRMsgEventArgs : EventArgs
    {
        private string m_msg;
        private SRMsgType m_MsgType;

        public SRMsgEventArgs(SRMsgType type, string msg)//当输入内容为字符串
        {
            m_msg = msg;
            m_MsgType = type;
        }

        public string getMsg()
        {
            return m_msg;
        }

        public SRMsgType getMsgType()
        {
            return m_MsgType;
        }
    }

    public class SRMessageSingleton : Singleton<SRMessageSingleton>
    {
        private log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        //OnSRMsg自定义事件（添加收发报文时触发）  SRMsgEventArgs自定义参数
        private event EventHandler<SRMsgEventArgs> OnSRMsg;

        /// <summary>
        /// 添加收发报文
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="info">报文内容</param>
        public void AddSRMsg(SRMsgType type, string info)
        {
            //触发事件
            if (OnSRMsg != null)
            {
                OnSRMsg(this, new SRMsgEventArgs(type, info)); //演示不同的参数类型
            }

            loginfo.Debug("Signaleton " + info);
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="_OnSRMsg">事件处理函数</param>
        public void setOnSRMsgHandle(EventHandler<SRMsgEventArgs> _OnSRMsg)
        {
            OnSRMsg += _OnSRMsg;
        }
    }
}