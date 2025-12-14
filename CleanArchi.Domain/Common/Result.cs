namespace CleanArchi.Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public IReadOnlyList<Error> Errors { get; }


        private Result(bool isSuccess, T? value)
        {
            IsSuccess = isSuccess;
            Value = value;
            Errors = [];
        }

        private Result(bool isSuccess, List<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? [];
        }

        private Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Errors = error != null ? [error] : [];
        }

        public static Result<T> Success(T value) => new(true, value);
        public static Result<T> Failure(List<Error> errors) => new(false, errors);
        public static Result<T> Failure(Error error) => new(false, error);
    }
}
