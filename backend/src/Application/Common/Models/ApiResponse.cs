using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Common.Models;

public class ApiResponse<T>
{
    public T? Data { get; init; }
    public bool IsSuccessful { get; init; }
    public int StatusCode { get; init; }
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = new();

    public static ApiResponse<T> Success(T data, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Data = data,
            IsSuccessful = true,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> Fail(string message, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            IsSuccessful = false,
            StatusCode = statusCode,
            Message = message,
            Errors = string.IsNullOrWhiteSpace(message) ? new List<string>() : new List<string> { message }
        };
    }

    public static ApiResponse<T> ValidationFail(IEnumerable<string> errors)
    {
        return new ApiResponse<T>
        {
            IsSuccessful = false,
            StatusCode = 422,
            Errors = errors?.ToList() ?? new List<string>()
        };
    }
}

public class ApiResponse : ApiResponse<object?>
{
    public static ApiResponse Success(int statusCode = 200)
    {
        return new ApiResponse
        {
            Data = null,
            IsSuccessful = true,
            StatusCode = statusCode
        };
    }

    public static new ApiResponse Fail(string message, int statusCode = 400)
    {
        return new ApiResponse
        {
            IsSuccessful = false,
            StatusCode = statusCode,
            Message = message,
            Errors = string.IsNullOrWhiteSpace(message) ? new List<string>() : new List<string> { message }
        };
    }

    public static new ApiResponse ValidationFail(IEnumerable<string> errors)
    {
        return new ApiResponse
        {
            IsSuccessful = false,
            StatusCode = 422,
            Errors = errors?.ToList() ?? new List<string>()
        };
    }
}
