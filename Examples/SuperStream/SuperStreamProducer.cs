﻿// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 2.0.
// Copyright (c) 2007-2020 VMware, Inc.

using System.Text;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.AMQP;
using RabbitMQ.Stream.Client.Reliable;

namespace SuperStream;

public class SuperStreamProducer
{
    public static async Task Start()
    {
        Console.WriteLine("Starting SuperStream Producer");
        var config = new StreamSystemConfig();
        var system = await StreamSystem.Create(config);
        Console.WriteLine("Super Stream Producer connected to RabbitMQ");
        // We define a Producer with the SuperStream name (that is the Exchange name)
        var producer = await Producer.Create(new ProducerConfig(system, Costants.StreamName)
        {
            SuperStreamConfig = new SuperStreamConfig()
            {
                // The super stream is enable and we define the routing hashing algorithm
                Routing = msg => msg.Properties.MessageId.ToString()
            }
        });
        const int NumberOfMessages = 1_000_000;
        for (var i = 0; i < NumberOfMessages; i++)
        {
            var message = new Message(Encoding.Default.GetBytes($"hello{i}"))
            {
                Properties = new Properties() {MessageId = $"hello{i}"}
            };
            await producer.Send(message);
            Console.WriteLine("Super Stream Producer sent {0} messages to {1}", i, Costants.StreamName);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }
}
