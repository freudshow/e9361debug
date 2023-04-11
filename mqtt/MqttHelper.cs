using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;

namespace E9361App.Mqtt
{
    public class MqttHelper
    {
        private static readonly IMqttClient mqttClient = new MqttFactory().CreateMqttClient();

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
                                Console.WriteLine($"{GetFilePath()}{GetFunctionName()}{GetLineNumber()}The MQTT client is connected.");
                            }
                            else
                            {
                                Console.WriteLine($"{GetFilePath()}{GetFunctionName()}{GetLineNumber()}already connected.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{GetFilePath()}{GetFunctionName()}{GetLineNumber()}Exception: {ex.Message}");
                        }
                        finally
                        {
                            Console.WriteLine($"{GetFilePath()}{GetFunctionName()}{GetLineNumber()}checking...");
                            await Task.Delay(TimeSpan.FromSeconds(2));
                        }
                    }
                });
        }

        public static string GetLineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return $"[{lineNumber}]";
        }

        public static string GetFunctionName([System.Runtime.CompilerServices.CallerMemberName] string func = "")
        {
            return $"[{func}()]";
        }

        public static string GetFilePath([System.Runtime.CompilerServices.CallerFilePath] string fileName = "")
        {
            return $"[{fileName}]";
        }
    }
}