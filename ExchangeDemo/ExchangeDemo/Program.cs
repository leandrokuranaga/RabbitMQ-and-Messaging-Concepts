using System;
using System.Text;
using RabbitMQ.Client;

namespace ExchangeDemo
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

            //Criando as exchanges
            channel.ExchangeDeclare(
                "exchange1",
                "direct",
                true,
                false,
                null);

            channel.ExchangeDeclare(
                "exchange2",
                "direct",
                true,
                false,
                null);

            //criando as queues
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

            //Bindings
            channel.QueueBind("queue1", "exchange1", "key1", null);
            channel.QueueBind("queue2", "exchange2", "key2", null);

            channel.ExchangeBind("exchange2", "exchange1", "key2", null);

            channel.BasicPublish(
                "exchange1",
                "key1",
                null,
                Encoding.UTF8.GetBytes("Key 1 message"));

            channel.BasicPublish(
                "exchange1",
                "key2",
                null,
                Encoding.UTF8.GetBytes("Key 2 message"));

        }
    }
}
