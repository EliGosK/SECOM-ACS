using CSI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ComponentModel
{
    public class ObjectResult
    {
        public ObjectResult() { }
        public ObjectResult(object data)
        {
            this.DataState = data;
        }

        public ObjectResult(Exception error)
        {
            this.Error = error;
        }

        public ObjectResult(object data, Exception error)
        {
            this.DataState = data;
            this.Error = error;
        }

        public string Message { get; set; }
        public object DataState { get; private set; }
        public Exception Error { get; private set; }
        public bool IsSucceed { get { return this.Error == null; } }

        public static ObjectResult Succeed() { return new ObjectResult(); }
        public static ObjectResult Succeed(object data) { return new ObjectResult(data); }

        public static ObjectResult Fail(string errorMessage) { return new ObjectResult(new Exception(errorMessage)); }
        public static ObjectResult Fail(Exception error) { return new ObjectResult(error); }
        public static ObjectResult Fail(object data, string errorMessage) { return new ObjectResult(data, new Exception(errorMessage)); }
        public static ObjectResult Fail(object data, Exception error) { return new ObjectResult(data, error); }

        public string GetErrorMessage()
        {
            if (this.Error == null) return null;
            return ExceptionUtility.GetLastExceptionMessage(this.Error);
        }
    }

    public class ObjectResult<T>
    {
        public ObjectResult(T data)
        {
            this.DataState = data;
        }

        public ObjectResult(T data, Exception error)
        {
            this.DataState = data;
            this.Error = error;
        }

        public string Message { get; set; }
        public T DataState { get; private set; }
        public Exception Error { get; private set; }
        public bool IsSucceed { get { return this.Error == null; } }

        public static ObjectResult<T> Succeed(T data) { return new ObjectResult<T>(data); }

        public static ObjectResult<T> Fail(T data, string errorMessage) { return new ObjectResult<T>(data, new Exception(errorMessage)); }
        public static ObjectResult<T> Fail(T data, Exception error) { return new ObjectResult<T>(data, error); }

        public string GetErrorMessage()
        {
            if (this.Error == null) return null;
            return ExceptionUtility.GetLastExceptionMessage(this.Error);
        }
    }
}
