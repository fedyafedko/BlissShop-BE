using BlissShop.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace BlissShop.Middlewares;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            UserManagerException => new BadRequestObjectResult(context.Exception.Message),
            NotFoundException => new NotFoundObjectResult(context.Exception.Message),
            InvalidCredentialsException => new UnauthorizedObjectResult(context.Exception.Message),
            ExpiredException => new BadRequestObjectResult(context.Exception.Message),
            IncorrectParametersException => new BadRequestObjectResult(context.Exception.Message),
            AlreadyExistsException => new BadRequestObjectResult(context.Exception.Message),
            ConfirmedEmailException => new BadRequestObjectResult(context.Exception.Message),
            InvalidTokenException => new BadRequestObjectResult(context.Exception.Message),
            InvalidSecurityAlgorithmException => new BadRequestObjectResult(context.Exception.Message),
            _ => new ObjectResult(new { error = $"An unexpected error occurred: {context.Exception.Message}" })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            }
        };
    }
}
