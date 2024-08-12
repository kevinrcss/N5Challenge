using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N5Challenge.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T Data { get; }

        private Result(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static Result<T> Ok(T data, string message = null) =>
            new Result<T>(true, message, data);

        public static Result<T> Fail(string message) =>
            new Result<T>(false, message, default);

    }
}
