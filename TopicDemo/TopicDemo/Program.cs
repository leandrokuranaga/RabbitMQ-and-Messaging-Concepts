using System;
using System.Text;
using RabbitMQ.Client;

namespace TopicDemo
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

            //creating topic exchange
            channel.ExchangeDeclare(
                "ex.topic",
                "topic",
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

            channel.QueueDeclare(
                "my.queue3",
                true,
                false,
                false,
                null);

            //binding queues to exchanges

            channel.QueueBind(
                "my.queue1",
                "ex.topic",
                "*.image.*"
                );

            channel.QueueBind(
                "my.queue2",
                "ex.topic",
                "#.image"
                );

            channel.QueueBind(
                "my.queue3",
                "ex.topic",
                "image.#"
                );

            //publicando mensages
            channel.BasicPublish(
                "ex.topic",
                "convert.image.bmp",
                null,
                Encoding.UTF8.GetBytes("Routing key is convert.image.bmp"));

            channel.BasicPublish(
                "ex.topic",
                "convert.bitmap.image",
                null,
                Encoding.UTF8.GetBytes("Routing key is convert.bitmap.image"));

            channel.BasicPublish(
                "ex.topic",
                "image.bitmap.32bit",
                null,
                Encoding.UTF8.GetBytes("Routing key is image.bitmap.32bit"));

        }
    }
}
