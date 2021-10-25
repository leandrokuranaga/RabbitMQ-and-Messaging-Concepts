using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Demo.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RequestReplyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrentDictionary<string, CalculationRequest> waitingRequest = new();

            //Connection to RabbitMQ
            IConnection conn;

            //Channel to RabbitMQ
            IModel channel;

            //Connection factory to RabbitMQ
            ConnectionFactory factory = new();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            //Criando conexão
            conn = factory.CreateConnection();
            //Criando canal 
            channel = conn.CreateModel();

            string responseQueueName = "res." + Guid.NewGuid().ToString();
            channel.QueueDeclare(responseQueueName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string requestId = Encoding.UTF8
                .GetString((byte[])e.BasicProperties.Headers[Demo.Common.Constants.RequestIdHeaderKey]);

                CalculationRequest request;
                if(waitingRequest.TryGetValue(requestId, out request))
                {
                    string messageData = Encoding.UTF8.GetString(e.Body.ToArray());
                    CalculationResponse response = JsonConvert.DeserializeObject<CalculationResponse>(messageData);

                    Console.WriteLine("Calculation result: "+request.ToString()+ "=" +response.ToString());
                }

                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Response received: " + message);
            };

            channel.BasicConsume(responseQueueName, true, consumer);

            Console.WriteLine("Press a key to send requests");
            Console.ReadKey();

            sendRequest(waitingRequest, channel, new CalculationRequest(2, 4, OperationType.Add), responseQueueName);
            sendRequest(waitingRequest, channel, new CalculationRequest(14, 6, OperationType.Subtract), responseQueueName);
            sendRequest(waitingRequest, channel, new CalculationRequest(50, 2, OperationType.Add), responseQueueName);
            sendRequest(waitingRequest, channel, new CalculationRequest(30, 6, OperationType.Subtract), responseQueueName);

            Console.ReadKey();

            channel.Close();
            conn.Close();
        }

        private static void sendRequest(
            ConcurrentDictionary<string, CalculationRequest> waitingRequest, 
            IModel channel,
            CalculationRequest request,
            string responseQueueName
            )
        {
            string requestId = Guid.NewGuid().ToString();
            string requestData = JsonConvert.SerializeObject(request);

            waitingRequest[requestId] = request;

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>();
            basicProperties.Headers.Add(Demo.Common.Constants.RequestIdHeaderKey, Encoding.UTF8.GetBytes(requestId));
            basicProperties.Headers.Add(Demo.Common.Constants.ResponseQueueHeaderKey, Encoding.UTF8.GetBytes(responseQueueName));
            
            channel.BasicPublish(
                "",
                "requests",
                basicProperties,
                Encoding.UTF8.GetBytes(requestData));
        }
    }
}
