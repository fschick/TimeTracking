using FS.TimeTracking.Abstractions.Interfaces.Repository.Services;
using FS.TimeTracking.Abstractions.Models.Repository;
using FS.TimeTracking.Abstractions.Models.REST;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data.Common;

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
                context.Result = new ConflictObjectResult(new ErrorInformation { DatabaseErrorCode = DatabaseErrorCode.ForeignKeyViolation });
                break;
        }
    }

    private bool IsForeignKeyViolation(DbException dbException)
        => _dbExceptionService.TranslateDbException(dbException) == DatabaseErrorCode.ForeignKeyViolation;
}