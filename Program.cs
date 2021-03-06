using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace az204svbusRec
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://az204sbnamespace01.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=7HBbceI6AIeRj2+4CggxXIZ3swUGOdabW+MWdYBJOJ4=";
        const string QueueName = "az204queue";
        static IQueueClient queueClient;
        public static async Task Main(string[] args)
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
            Console.WriteLine("======================================================");
            // Register the queue message handler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();
            Console.ReadKey();
            await queueClient.CloseAsync();
        }

        static async Task MainAsync()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
            Console.WriteLine("======================================================");
            // Register the queue message handler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();
            Console.ReadKey();
            await queueClient.CloseAsync();
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
        // Configure the message handler options in terms of exception handling, number of concurrent 
        
        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
        // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
        // Set it according to how many messages the application wants to process in parallel.
        MaxConcurrentCalls = 1,
        // Indicates whether the message pump should automatically complete the messages after 
        
        // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
        AutoComplete = false
        };

        // Register the function that processes messages.
        queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
        // Process the message.
        Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
        // Complete the message so that it is not received again.
        // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the 
        
        await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already 
       
        // If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
        // to avoid unnecessary exceptions.
        }

        // Use this handler to examine the exceptions received on the message pump.
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
        Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
        var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
        Console.WriteLine("Exception context for troubleshooting:");
        Console.WriteLine($"- Endpoint: {context.Endpoint}");
        Console.WriteLine($"- Entity Path: {context.EntityPath}");
        Console.WriteLine($"- Executing Action: {context.Action}");
        return Task.CompletedTask;
        }

    }
}
