using System.Collections.Generic;

namespace Pada.Infrastructure.Types
{
    public class ResultModel<T>
    {
        public ResultModel(T data, bool success = true, IEnumerable<Error> errors = default!)
        {
            Datas = data;
            IsSuccess = success;
            Errors = errors;
        }

        public ResultModel(IEnumerable<Error> errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public static ResultModel<T> Create(T data, bool isSuccess = true, IEnumerable<Error> errors = default!)
        {
            return new(data, isSuccess, errors);
        }

        public static ResultModel<T> Fail(IEnumerable<Error> errors = default!)
        {
            return new ResultModel<T>(default, false, errors);
        }

        public static ResultModel<T> Success(T data)
        {
            return new ResultModel<T>(data);
        }

        public T Datas { get; }
        public bool IsSuccess { get; }
        public IEnumerable<Error> Errors { get; }
    }

    public class ResultModel
    {
        public ResultModel(bool success, IEnumerable<Error> errors = default!)
        {
            IsSuccess = success;
            Errors = errors;
        }

        public ResultModel(IEnumerable<Error> errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public static ResultModel Create(bool isSuccess = true, IEnumerable<Error> errors = default!)
        {
            return new(isSuccess, errors);
        }

        public static ResultModel Fail(IEnumerable<Error> errors = default!)
        {
            return new ResultModel(false, errors);
        }

        public static ResultModel Success()
        {
            return new ResultModel(true);
        }

        public bool IsSuccess { get; }
        public IEnumerable<Error> Errors { get; }
    }
}