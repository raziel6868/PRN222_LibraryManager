using System.Text.Json;

namespace LibraryManager.Server.Models;

public class ApiRequest
{
    public string Action { get; set; } = string.Empty;
    public JsonElement Data { get; set; }
}
