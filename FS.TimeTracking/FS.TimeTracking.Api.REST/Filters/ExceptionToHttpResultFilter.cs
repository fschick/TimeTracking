using FS.TimeTracking.Api.REST.Models;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.Core;
using FS.TimeTracking.Core.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Net;
using System.Net.Http;

namespace FS.TimeTracking.Api.REST.Filters;

internal class ExceptionToHttpResultFilter : IExceptionFilter
{
    private readonly IDbExceptionService _dbExceptionService;
    private readonly EnvironmentConfiguration _environment;
    private readonly ILogger<ExceptionToHttpResultFilter> _logger;

    public ExceptionToHttpResultFilter(IDbExceptionService dbExceptionService, EnvironmentConfiguration environment, ILogger<ExceptionToHttpResultFilter> logger)
    {
        _dbExceptionService = dbExceptionService;
        _environment = environment;
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case DbException dbException when IsForeignKeyViolation(dbException):
                _logger.LogInformation(context.Exception, "Entity could not be modified.");
                var isDeleteOperation = context.HttpContext.Request.Method == HttpMethod.Delete.Method;
                var errorCode = isDeleteOperation
                    ? ApplicationErrorCode.ForeignKeyViolationOnDelete
                    : ApplicationErrorCode.ForeignKeyViolation;
                var dbErrorMessages = _environment.IsDevelopment ? new[] { dbException.Message } : null;
                var foreignKeyViolationError = new ApplicationError { Code = errorCode, Messages = dbErrorMessages };
                context.Result = new ConflictObjectResult(foreignKeyViolationError);
                break;
            case ConformityException conformityException:
                _logger.LogInformation(context.Exception, "Entity could not be modified.");
                var conformityError = new ApplicationError
                {
                    Code = conformityException.ErrorCode ?? ApplicationErrorCode.ConformityViolation,
                    Messages = conformityException.Errors
                };
                context.Result = new ConflictObjectResult(conformityError);
                break;
            default:
                _logger.LogError(context.Exception, "An unhandled exception has occurred while executing the request.");
                var serverErrorMessages = _environment.IsDevelopment ? new[] { context.Exception.ToString() } : null;
                var internalServerError = new ApplicationError { Code = ApplicationErrorCode.InternalServerError, Messages = serverErrorMessages };
                context.Result = new ObjectResult(internalServerError) { StatusCode = (int)HttpStatusCode.InternalServerError };
                break;
        }
    }

    private bool IsForeignKeyViolation(DbException dbException)
        => _dbExceptionService.TranslateDbException(dbException) == ApplicationErrorCode.ForeignKeyViolation;
}