using System.Collections.Generic;

namespace Pada.Infrastructure.Types
{
    public class ResultModel<T>
    {
        public ResultModel(T data, bool success = true, BaseError errors = default!)
        {
            Datas = data;
            IsSuccess = success;
            Errors = errors;
        }

        public ResultModel(BaseError errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public static ResultModel<T> Create(T data, bool isSuccess = true, BaseError errors = default!)
        {
            return new(data, isSuccess, errors);
        }

        public static ResultModel<T> Fail(BaseError errors = default!)
        {
            return new ResultModel<T>(default, false, errors);
        }

        public static ResultModel<T> Success(T data)
        {
            return new ResultModel<T>(data);
        }

        public T Datas { get; }
        public bool IsSuccess { get; }
        public BaseError Errors { get; }
    }

    public class ResultModel
    {
        public ResultModel(bool success, BaseError errors = default!)
        {
            IsSuccess = success;
            Errors = errors;
        }

        public ResultModel(BaseError errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public static ResultModel Create(bool isSuccess = true, BaseError errors = default!)
        {
            return new(isSuccess, errors);
        }

        public static ResultModel Fail(BaseError errors = default!)
        {
            return new ResultModel(false, errors);
        }

        public static ResultModel Success()
        {
            return new ResultModel(true);
        }

        public bool IsSuccess { get; }
        public BaseError Errors { get; }
    }
}