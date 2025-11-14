namespace LibraryManager.Server.Models;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }

    public static ApiResponse SuccessResponse(object? data = null, string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse FailResponse(string message)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null
        };
    }
}
