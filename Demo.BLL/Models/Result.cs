using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.BLL.Models
{
    public sealed class Result<T>
    {

        public bool ? Succeeded { get; }
        public T ? Data { get; }
        public string ? Error { get; }

        private Result(bool succeeded, T? data, string? error )
        {
            Succeeded = succeeded;
            Data = data;
            Error = error;
        }

        public static Result<T> Success(T data)
        => new Result<T>(true, data, null);

        public static Result<T> Failure(string error)
     => new Result<T>(false,default, error);




    }
}
