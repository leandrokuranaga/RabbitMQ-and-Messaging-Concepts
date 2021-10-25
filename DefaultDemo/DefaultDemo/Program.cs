using System;
using System.Text;
using RabbitMQ.Client;

namespace DefaultDemo
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

            //declare queues
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

            //publicando a mensagem
            channel.BasicPublish(
                "",
                "my.queue1",
                null,
                Encoding.UTF8.GetBytes("Message with routing key my.queue1"));

            channel.BasicPublish(
                "",
                "my.queue2",
                null,
                Encoding.UTF8.GetBytes("Message with routing key my.queue2"));

        }
    }
}
