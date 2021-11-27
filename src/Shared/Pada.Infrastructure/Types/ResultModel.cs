using System.Collections.Generic;

namespace Pada.Infrastructure.Types
{
    public class ResultModel<T>
    {
        public ResultModel(T data, bool success = true, IDictionary<string, string[]> errors = default!)
        {
            Datas = data;
            IsSuccess = success;
            Errors = errors;
        }

        public ResultModel(IDictionary<string, string[]> errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public ResultModel(string code, string error)
        {
            IsSuccess = false;
            Errors.Add(code, new[] {error});;
        }
        
        public static ResultModel<T> Create(T data, bool isSuccess = true, IDictionary<string, string[]> errors = default!)
        {
            return new(data, isSuccess, errors);
        }

        public static ResultModel<T> Fail(IDictionary<string, string[]> errors = default!)
        {
            return new ResultModel<T>(default, false, errors);
        }

        public static ResultModel<T> Success(T data)
        {
            return new ResultModel<T>(data);
        }

        public T Datas { get; }
        public bool IsSuccess { get; }
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
    }

    public class ResultModel
    {
        public ResultModel(bool success, IDictionary<string, string[]> errors = default!)
        {
            IsSuccess = success;
            Errors = errors;
        }

        public ResultModel(IDictionary<string, string[]> errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public ResultModel(string code, string error)
        {
            IsSuccess = false;
            Errors.Add(code, new[] {error});;
        }
        
        public static ResultModel Create(bool isSuccess = true, IDictionary<string, string[]> errors = default!)
        {
            return new(isSuccess, errors);
        }

        public static ResultModel Fail(IDictionary<string, string[]> errors = default!)
        {
            return new ResultModel(false, errors);
        }

        public static ResultModel Success()
        {
            return new ResultModel(true);
        }

        public bool IsSuccess { get; }
        public IDictionary<string, string[]> Errors { get; }
    }
}