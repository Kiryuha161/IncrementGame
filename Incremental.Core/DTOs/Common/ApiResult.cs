using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.DTOs.Common
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResult Ok(object? data = null, string? message = null)
        {
            return new ApiResult
            {
                Success = true,
                Data = data,
                Message = message ?? "Success"
            };
        }

        public static ApiResult Fail(string message, List<string>? errors = null)
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
