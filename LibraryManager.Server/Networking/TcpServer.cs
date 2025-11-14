using System.Net;
using System.Net.Sockets;
using LibraryManager.Server.Handlers;

namespace LibraryManager.Server.Networking;

public class TcpServer
{
    private readonly int _port;
    private readonly RequestDispatcher _dispatcher;
    private TcpListener? _listener;

    public TcpServer(int port, RequestDispatcher dispatcher)
    {
        _port = port;
        _dispatcher = dispatcher;
    }

    public async Task StartAsync()
    {
        _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
        _listener.Start();

        Console.WriteLine($"LibraryManager Server is running on 127.0.0.1:{_port}");
        Console.WriteLine("Waiting for client connections...");

        while (true)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"Client connected from {client.Client.RemoteEndPoint}");
                
                // Handle each client in a separate task
                _ = Task.Run(() => HandleClientAsync(client));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        var clientConnection = new ClientConnection(client, _dispatcher);
        await clientConnection.HandleAsync();
    }

    public void Stop()
    {
        _listener?.Stop();
        Console.WriteLine("Server stopped.");
    }
}
