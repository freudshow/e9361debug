using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using E9361App.Log;

namespace E9361App.Mqtt
{
    public class MqttHelper
    {
        private readonly IMqttClient m_MqttClient = new MqttFactory().CreateMqttClient();
        private readonly log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");
        private string m_Server;
        private int m_Port;
        private string m_PublishTopic;//向终端发送命令的主题
        private string m_ResponseTopic;//终端应答的主题

        public MqttHelper(string server, int port, string pub, string response)
        {
            m_Server = server;
            m_Port = port;
            m_PublishTopic = pub;
            m_ResponseTopic = response;
        }

        public void ConnectAndSubscribe(Func<MqttApplicationMessageReceivedEventArgs, Task> arrived)
        {
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(m_Server, m_Port).Build();

            _ = Task.Run(
                async () =>
                {
                    while (true)
                    {
                        try
                        {
                            if (!await m_MqttClient.TryPingAsync())
                            {
                                await m_MqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                                m_MqttClient.ApplicationMessageReceivedAsync += arrived;//收到消息后的处理函数
                                var mqttSubscribeOptions = new MqttFactory().CreateSubscribeOptionsBuilder()
                                                            .WithTopicFilter(
                                                                f =>
                                                                {
                                                                    f.WithTopic(m_ResponseTopic);
                                                                })
                                                            .Build();

                                await m_MqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
                            }
                        }
                        catch (Exception ex)
                        {
                            m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                        }
                        finally
                        {
                            await Task.Delay(TimeSpan.FromSeconds(2));
                        }
                    }
                });
        }

        public void PublishMessage(string msg)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                                .WithTopic(m_PublishTopic)
                                .WithPayload(msg)
                                .Build();

            m_MqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        }
    }
}