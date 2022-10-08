using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Configuration;
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
    private readonly EnvironmentConfiguration _environment;

    public ExceptionToHttpResultFilter(IDbExceptionService dbExceptionService, EnvironmentConfiguration environment)
    {
        _dbExceptionService = dbExceptionService;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case DbException dbException when IsForeignKeyViolation(dbException):
                var errorCode = context.HttpContext.Request.Method == HttpMethod.Delete.Method
                    ? RestErrorCode.ForeignKeyViolationOnDelete
                    : RestErrorCode.ForeignKeyViolation;
                var dbErrorMessages = _environment.IsDevelopment ? new[] { dbException.Message } : null;
                var foreignKeyViolationError = new RestError { Code = errorCode, Messages = dbErrorMessages };
                context.Result = new ConflictObjectResult(foreignKeyViolationError);
                break;
            case ConformityException conformityException:
                var conformityError = new RestError { Code = RestErrorCode.ConformityViolation, Messages = conformityException.Errors };
                context.Result = new ConflictObjectResult(conformityError);
                break;
            default:
                var serverErrorMessages = _environment.IsDevelopment ? new[] { context.Exception.ToString() } : null;
                var internalServerError = new RestError { Code = RestErrorCode.InternalServerError, Messages = serverErrorMessages };
                context.Result = new ObjectResult(internalServerError) { StatusCode = (int)HttpStatusCode.InternalServerError };
                break;
        }
    }

    private bool IsForeignKeyViolation(DbException dbException)
        => _dbExceptionService.TranslateDbException(dbException) == RestErrorCode.ForeignKeyViolation;
}