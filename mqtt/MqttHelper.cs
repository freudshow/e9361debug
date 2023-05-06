using E9361Debug.Log;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace E9361Debug.Mqtt
{
    public class MqttHelper
    {
        private readonly IMqttClient m_MqttClient = new MqttFactory().CreateMqttClient();
        private readonly log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");
        private string m_Server;
        private int m_Port;

        public MqttHelper(string server, int port)
        {
            m_Server = server;
            m_Port = port;
        }

        public async Task ConnectAndSubscribeAsync(List<string> topics, Func<MqttApplicationMessageReceivedEventArgs, Task> arrived)
        {
            if (topics == null || topics.Count <= 0)
            {
                return;
            }

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(m_Server, m_Port).Build();

            await Task.Run(
                async () =>
                {
                    while (true)
                    {
                        try
                        {
                            if (!await m_MqttClient.TryPingAsync())
                            {
                                await m_MqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                                m_MqttClient.ApplicationMessageReceivedAsync += arrived;
                                MqttClientSubscribeOptionsBuilder subscribeOptionsBuilder = new MqttFactory().CreateSubscribeOptionsBuilder();

                                for (int i = 0; i < topics.Count; i++)
                                {
                                    _ = subscribeOptionsBuilder.WithTopicFilter(topics[i]);
                                }

                                await m_MqttClient.SubscribeAsync(subscribeOptionsBuilder.Build(), CancellationToken.None);
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

        public void PublishMessage(string publishTopic, string msg)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                                .WithTopic(publishTopic)
                                .WithPayload(msg)
                                .Build();

            m_MqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        }
    }
}