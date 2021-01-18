using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
namespace AgentClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string clientConnectionId = null;

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/MessageHub")
                .WithAutomaticReconnect()
                .Build();
            
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            }; 
            
            connection.Reconnecting += error =>
            {
                Debug.Assert(connection.State == HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.

                return Task.CompletedTask;
            }; 
            
            connection.Reconnected += connectionId =>
            {
                clientConnectionId = connectionId;
                Debug.Assert(connection.State == HubConnectionState.Connected);

                // Notify users the connection was reestablished.
                // Start dequeuing messages queued while reconnecting if any.

                return Task.CompletedTask;
            };

            await connection.StartAsync();

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {

            });

            await connection.InvokeAsync("SendMessage", clientConnectionId, "message");
        }
    }
}
