namespace Helpdesk.Models
{
    public class ResultModel<T>
    {
        public ResultModel(T data)
        {
            Data = data;
        }
        public ResultModel(T data, string message)
        {
            Data = data;
            Message = message;
        }

        public T Data { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }

    public class OKResultModel<T> : ResultModel<T>
    {
        public OKResultModel(T data) : base(data)
        {
            IsError = false;
        }

        public OKResultModel(T data, string message) : base(data, message)
        {
            IsError = false;
        }
    }

    public class ErrorResultModel<T> : ResultModel<T>
    {
        public ErrorResultModel(T data) : base(data)
        {
            IsError = true;
        }

        public ErrorResultModel(T data, string message) : base(data, message)
        {
            IsError = true;
        }
    }

}
