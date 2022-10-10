using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.REST;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data.Common;
using System.Net;
using System.Net.Http;

namespace FS.TimeTracking.Api.REST.Filters;

internal class ExceptionToHttpResultFilter : IExceptionFilter
{
    private readonly IDbExceptionService _dbExceptionService;

    public ExceptionToHttpResultFilter(IDbExceptionService dbExceptionService)
        => _dbExceptionService = dbExceptionService;

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case DbException dbException when IsForeignKeyViolation(dbException):
                var errorCode = context.HttpContext.Request.Method == HttpMethod.Delete.Method
                    ? RestErrorCode.ForeignKeyViolationOnDelete
                    : RestErrorCode.ForeignKeyViolation;
                context.Result = new ConflictObjectResult(new RestError { ErrorCode = errorCode });
                break;
            default:
                context.Result = new ObjectResult(new RestError { ErrorCode = RestErrorCode.InternalServerError }) { StatusCode = (int)HttpStatusCode.InternalServerError };
                break;
        }
    }

    private bool IsForeignKeyViolation(DbException dbException)
        => _dbExceptionService.TranslateDbException(dbException) == RestErrorCode.ForeignKeyViolation;
}