using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace AlternateDemo
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

            //criar exchanges
            channel.ExchangeDeclare(
                "ex.fanout",
                "fanout",
                true,
                false,
                null);

            channel.ExchangeDeclare(
                "ex.direct",
                "direct",
                true,
                false,
                new Dictionary<string, object>()
                {
                    {"alternate-exchange", "ex.fanout"}
                }
                );

            //criando queues
            channel.QueueDeclare(
                "queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "queue2",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "queue3",
                true,
                false,
                false,
                null);

            //bindando as queues com as exchanges
            channel.QueueBind(
                "queue1",
                "ex.direct",
                "video",
                null);

            channel.QueueBind(
                "queue2",
                "ex.direct",
                "image",
                null);

            channel.QueueBind(
                "queue3",
                "ex.fanout",
                "",
                null);

            //publicando msgs
            channel.BasicPublish(
                "ex.direct",
                "video",
                null,
                Encoding.UTF8.GetBytes("This message is video"));

            channel.BasicPublish(
                "ex.direct",
                "image",
                null,
                Encoding.UTF8.GetBytes("This message is image"));

            channel.BasicPublish(
                "ex.direct",
                "text",
                null,
                Encoding.UTF8.GetBytes("This message is text"));

        }
    }
}
