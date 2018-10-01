using CSI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ComponentModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectResults<T>
    {
        public bool IsSucceed
        {
            get
            {
                return this.Results.Where(t => t.Error != null).ToList().Count == 0;
            }
        }

        public IList<ObjectResultDataItem<T>> Results { get; private set; } = new List<ObjectResultDataItem<T>>();

        public void AddResult(T data)
        {
            this.Results.Add(new ObjectResultDataItem<T>(data));
        }

        public void AddResult(T data, Exception error)
        {
            this.Results.Add(new ObjectResultDataItem<T>(data,error));
        }

        public string[] GetErrorMessages()
        {
            return this.Results.Where(t => t.Error != null)
                .Select(t => ExceptionUtility.GetLastExceptionMessage(t.Error))
                .ToArray();
        }

        public T[] GetErrors()
        {
            return this.Results.Where(t => t.Error != null).Select(t=>t.Data).ToArray();
        }

        public T[] GetSuccesses()
        {
            return this.Results.Where(t => t.Error == null).Select(t=>t.Data).ToArray();
        }

        
    }

    public class ObjectResultDataItem<T> : ObjectResultDataItem
    {
        public ObjectResultDataItem(T data)
        {
            this.Data = data;
        }

        public ObjectResultDataItem(T data, Exception ex) : base(ex)
        {
            this.Data = data;
        }

        public T Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ObjectResultDataItem
    {
        public ObjectResultDataItem()
        {

        }

        public ObjectResultDataItem(Exception ex)
        {
            this.Error = ex;
        }
        
        public Exception Error { get; set; }
    }

    public static class ObjectResultsExtensions
    {
        /// <summary>
        /// Convert ObjectResults to Result with property IsSucceed and Results as List of Result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="results"></param>
        /// <returns></returns>
        public static object ToResult<T>(this ObjectResults<T> results) 
        {
            return new
            {
                IsSucceed = results.IsSucceed,
                Results = results.Results.Select(t => new { Data = t.Data, Error = t.Error==null?null: ExceptionUtility.GetLastExceptionMessage(t.Error) }).ToArray()
            };
        }

        /// <summary>
        /// Return list of results
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="results"></param>
        /// <returns></returns>
        public static object ToListResult<T>(this ObjectResults<T> results)
        {
            return results.Results.Select(t => new { Data = t.Data, Error = t.Error == null ? null : ExceptionUtility.GetLastExceptionMessage(t.Error) }).ToArray();
        }

        /// <summary>
        /// Convert ObjectResults to View Result with property IsSucceed ,Data and Errors
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="results"></param>
        /// <returns></returns>
        public static object ToViewResult<T>(this ObjectResults<T> results)
        {
            return new
            {
                IsSucceed = results.IsSucceed,
                Errors = results.Results.Where(t => t.Error != null).Select(t => new { Data = t.Data, Error = t.Error == null ? null : ExceptionUtility.GetLastExceptionMessage(t.Error) }).ToArray(),
                Data = results.Results.Where(t => t.Error == null).Select(t => t.Data).ToArray()
            };
        }
    }
}