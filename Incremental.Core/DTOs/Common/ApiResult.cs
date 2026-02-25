using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.DTOs.Common
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResult<T> Ok(T? data = default, string? message = null)
        {
            return new ApiResult<T>
            {
                Success = true,
                Data = data,
                Message = message ?? "Success"
            };
        }

        public static ApiResult<T> Fail(string message, List<string>? errors = null)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    // Для случаев, когда не нужны данные (например, DELETE)
    public class ApiResult : ApiResult<object>
    {
        public static new ApiResult Ok(object? data = null, string? message = null)
        {
            return new ApiResult
            {
                Success = true,
                Data = data,
                Message = message ?? "Success"
            };
        }

        public static new ApiResult Fail(string message, List<string>? errors = null)
        {
            return new ApiResult
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}