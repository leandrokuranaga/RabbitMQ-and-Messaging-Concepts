using System;
using System.Collections.Generic;
using System.Text;
using Demo.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Reply
{
    class Program
    {
        static void Main(string[] args)
        {
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string requestData = Encoding.UTF8.GetString(e.Body.ToArray());
                CalculationRequest request = JsonConvert.DeserializeObject<CalculationRequest>(requestData);

                Console.WriteLine("Request received: "+request.ToString());

                CalculationResponse response = new CalculationResponse();

                if(request.Operation == OperationType.Add)
                {
                    response.Result = request.Number1 + request.Number2;

                }
                else if (request.Operation == OperationType.Subtract)
                {
                    response.Result = request.Number1 - request.Number2;
                }

                string responseData = JsonConvert.SerializeObject(response);
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Headers = new Dictionary<string, object>();
                basicProperties.Headers.Add(Demo.Common.Constants.RequestIdHeaderKey, e.BasicProperties.Headers[Demo.Common.Constants.RequestIdHeaderKey]);
                //Console.WriteLine("Request received: " + request);

                string responseQueueName = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers[Demo.Common.Constants.ResponseQueueHeaderKey]);

                channel.BasicPublish("", responseQueueName, basicProperties, Encoding.UTF8.GetBytes(responseData));
            };

            channel.BasicConsume("requests", true, consumer);

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.Close();
            conn.Close();

        }
    }
}
