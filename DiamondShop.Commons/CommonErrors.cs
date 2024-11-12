using FluentResults;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Commons
{
    /// <summary>
    /// Dua vao tung loai nay de controller check type roi tra ve error
    /// Cac loi nay dua vao thu vien ErrorOr.Type, day la nhu extension
    /// co  2 cai error dung nhieu nhat
    /// 1. FailureError () [ gan nhu coveer moi th ]
    /// 2. ValidationError() [ khi validate fail de controller tra ve response cho dung ]
    /// </summary>

    public class FailureError : Error
    {
        public FailureError(string message) : base(message)
        {

        }
        public FailureError(string message, Dictionary<string, object>  errorsMetadata) : base(message)
        {
            WithMetadata(errorsMetadata);
        }
    }
    public class ValidationError : Error 
    {
        public ValidationError(string message, Dictionary<string, object> validationErrors) : base(message)
        {
            WithMetadata(validationErrors);
        }
        public ValidationError(string message) : base(message)
        {
        }
    }
    /// <summary>
    /// Top 2
    /// </summary>

    public class UnexpectedError: Error { }
    public class ConflictError : Error
    {
        public ConflictError(string message) : base(message)
        {

        }


        protected ConflictError()
        {
            base.Message = "Conflict";
        }
    }
    public class NotFoundError : Error
    {
        public NotFoundError(string message) : base(message)
        {
        }


        public NotFoundError()
        {
            base.Message = "Not Found";
        }
    }
    public class UnauthorizedError : Error 
    {
        public UnauthorizedError(string message) : base(message)
        {
        }


        public UnauthorizedError()
        {
            base.Message = "Unauthorized";
        }
    }
    public class ForbiddenError : Error 
    {
        public ForbiddenError(string message) : base(message)
        {
        }


        public ForbiddenError()
        {
            base.Message = "Forbidden";
        }
    }


}
