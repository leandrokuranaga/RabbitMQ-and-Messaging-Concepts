using System;
using System.Text;
using RabbitMQ.Client;

namespace DirectDemo
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

            //Criando a exchange
            channel.ExchangeDeclare(
                "ex.direct",
                "direct",
                true,
                false,
                null);
            
            //Criando as queues
            channel.QueueDeclare(
                "my.infos",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.warnings",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.errors",
                true,
                false,
                false,
                null);

            //Linkando as queues com a exchange passando o routing key
            channel.QueueBind(
                "my.infos",
                "ex.direct",
                "info",
                null);
            channel.QueueBind(
                "my.warnings",
                "ex.direct",
                "warning",
                null);
            channel.QueueBind(
                "my.errors",
                "ex.direct",
                "error",
                null);

            //Publicando a mensagem
            channel.BasicPublish(
                "ex.direct",
                "info", 
                null,
                Encoding.UTF8.GetBytes("Message with routing key info."));

            channel.BasicPublish(
                "ex.direct",
                "warning",
                null,
                Encoding.UTF8.GetBytes("Message with routing key warning."));

            channel.BasicPublish(
                "ex.direct",
                "error",
                null,
                Encoding.UTF8.GetBytes("Message with routing key error."));

        }
    }
}
