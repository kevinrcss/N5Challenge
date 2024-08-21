namespace N5Challenge.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Result()
        {
            Success = false;
        }
    }
}
