namespace BookingService.Utils
{
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }

        protected ServiceResult(bool success, string? message = null)
        {
            Success = success;
            ErrorMessage = message;
        }

        public static ServiceResult SuccessResult()
        {
            return new ServiceResult(true);
        }

        public static ServiceResult Failure(string errorMessage)
        {
            return new ServiceResult(false, errorMessage);
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; private set; }

        private ServiceResult(bool success, T data = default, string? errorMessage = null) : base(success, errorMessage)
        {
            Data = data;
        }

        public static ServiceResult<T> SuccessResult(T data)
        {
            return new ServiceResult<T>(true, data);
        }

        public static new ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>(false, default, errorMessage);
        }
    }
}
