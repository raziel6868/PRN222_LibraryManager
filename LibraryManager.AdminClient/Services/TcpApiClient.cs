using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace LibraryManager.AdminClient.Services;

public class ApiRequest
{
    public string Action { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public JsonElement Data { get; set; }
}

public class TcpApiClient
{
    private readonly string _serverHost;
    private readonly int _serverPort;

    public TcpApiClient(string serverHost = "127.0.0.1", int serverPort = 5000)
    {
        _serverHost = serverHost;
        _serverPort = serverPort;
    }

    public async Task<ApiResponse> SendRequestAsync(string action, object? data = null)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_serverHost, _serverPort);

            using var stream = client.GetStream();

            // Prepare request
            var request = new ApiRequest
            {
                Action = action,
                Data = data
            };

            // Serialize and send
            string requestJson = JsonSerializer.Serialize(request);
            byte[] requestBytes = Encoding.UTF8.GetBytes(requestJson);
            await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

            // Receive response
            byte[] buffer = new byte[65536]; // 64KB
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Deserialize
            var response = JsonSerializer.Deserialize<ApiResponse>(responseJson);
            return response ?? new ApiResponse { Success = false, Message = "Invalid response" };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Success = false,
                Message = $"Connection error: {ex.Message}"
            };
        }
    }

    public async Task<T?> SendRequestAsync<T>(string action, object? data = null)
    {
        var response = await SendRequestAsync(action, data);
        
        if (!response.Success)
        {
            throw new Exception(response.Message ?? "Request failed");
        }

        if (response.Data.ValueKind == JsonValueKind.Null || 
            response.Data.ValueKind == JsonValueKind.Undefined)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(response.Data.GetRawText());
    }
}
