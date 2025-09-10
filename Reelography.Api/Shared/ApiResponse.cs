namespace Reelography.Api.Contents;


/// <summary>
/// Common API Response for all apis
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Success flag 
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Data that needs to be send in the response body
    /// </summary>
    public object? Data { get; set; }
    /// <summary>
    /// Nullable message. In case of error, error message can be set. 
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// Time Stamp of the request
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Configure Success response
    /// </summary>
    /// <param name="data"></param>
    /// <returns>API response of T type</returns>
    public static ApiResponse SuccessResponse(object data) =>
        new() { Success = true, Data = data };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ApiResponse FailureResponse(string message) =>
        new() { Success = false, Message = message, Data = null };
}