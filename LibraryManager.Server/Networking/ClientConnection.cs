using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using LibraryManager.Server.Handlers;
using LibraryManager.Server.Models;

namespace LibraryManager.Server.Networking;

public class ClientConnection
{
    private readonly TcpClient _client;
    private readonly RequestDispatcher _dispatcher;

    public ClientConnection(TcpClient client, RequestDispatcher dispatcher)
    {
        _client = client;
        _dispatcher = dispatcher;
    }

    public async Task HandleAsync()
    {
        NetworkStream stream = _client.GetStream();

        try
        {
            // Read request
            byte[] buffer = new byte[65536]; // 64KB buffer
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string requestJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Received: {requestJson}");

            // Deserialize request
            ApiRequest? request = JsonSerializer.Deserialize<ApiRequest>(requestJson);
            
            if (request == null)
            {
                var errorResponse = ApiResponse.FailResponse("Invalid request format");
                await SendResponseAsync(stream, errorResponse);
                return;
            }

            // Dispatch request
            var response = await _dispatcher.DispatchAsync(request);

            // Send response
            await SendResponseAsync(stream, response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
            var errorResponse = ApiResponse.FailResponse($"Server error: {ex.Message}");
            await SendResponseAsync(stream, errorResponse);
        }
        finally
        {
            _client.Close();
            Console.WriteLine("Client disconnected");
        }
    }

    private async Task SendResponseAsync(NetworkStream stream, ApiResponse response)
    {
        string responseJson = JsonSerializer.Serialize(response);
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        Console.WriteLine($"Sent: {responseJson}");
    }
}
