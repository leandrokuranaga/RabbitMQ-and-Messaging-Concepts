using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace HeadersDemo
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

            //creating headers exchange
            channel.ExchangeDeclare(
                "ex.headers",
                "headers",
                true,
                false,
                null);

            //creating queues
            channel.QueueDeclare(
                "my.queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.queue2",
                true,
                false,
                false,
                null);

            //binding queues to exchange
            channel.QueueBind(
                "my.queue1",
                "ex.headers",
                "",
                new Dictionary<string, object>()
                {
                    {"x-match", "all"},
                    {"job", "convert"},
                    {"format", "jpeg"}
                });

            channel.QueueBind(
                "my.queue2",
                "ex.headers",
                "",
                new Dictionary<string, object>()
                {
                    {"x-match", "any"},
                    {"job", "convert"},
                    {"format", "jpeg"}
                });

            //criando propriedades basicas, para colocar headers values
            IBasicProperties props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("job", "convert");
            props.Headers.Add("format", "jpeg");


            //publicando msg
            channel.BasicPublish(
                "ex.headers",
                "",
                props,
                Encoding.UTF8.GetBytes("Message 1"));

            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("job", "convert");
            props.Headers.Add("format", "bmp");

            channel.BasicPublish(
                "ex.headers",
                "",
                props,
                Encoding.UTF8.GetBytes("Message 2"));
        }
    }
}
