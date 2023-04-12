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
        private static readonly IMqttClient mqttClient = new MqttFactory().CreateMqttClient();
        private static readonly log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");

        public static void Reconnect_Using_Timer(string svr, int? port = 1883)
        {
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(svr, port).Build();

            _ = Task.Run(
                async () =>
                {
                    while (true)
                    {
                        try
                        {
                            if (!await mqttClient.TryPingAsync())
                            {
                                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
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
    }
}