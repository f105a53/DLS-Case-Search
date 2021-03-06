﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using EasyNetQ;
using EasyNetQ.Logging;
using Index = Common.Index;

namespace SearchNode
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main";
            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);
            using var index = new Index("./lucene-index");
            using var bus =
                RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITMQ_CSTRING") ?? "host=localhost");
            bus.RespondAsync<SearchRequest, SearchResults<Email>>(request =>
                Task.Factory.StartNew(() => index.Search(request.Text)));
            Console.WriteLine("Running...\nPress Ctrl+C to exit");
            exitEvent.WaitOne();
        }
    }
}