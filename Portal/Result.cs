using System.Runtime.Serialization;

namespace Portal
{
    [DataContract]
    public class Result : IResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static Result Ok()
            => new Result {Success = true};

        public static Result Failed(string message)
            => new Result {Message = message};
    }

    [DataContract]
    public class Result<T> : IResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static Result<T> Ok(T data)
            => new() {Success = true, Data = data};

        public static Result<T> Failed(string message)
            => new() {Message = message};
    }
}