using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InfoTrackSEO.Domain.Exceptions;

namespace InfoTrackSEO.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error")]
        public ErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = context?.Error;
            var code = 500;

            if (exception is NotFoundException) code = 404; // Not Found
            else if (exception is DomainServiceException) code = 400; // Bad Request

            Response.StatusCode = code;

            return new ErrorResponse(exception);
        }
    }

    public class ErrorResponse
    {
        public ErrorResponse(Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        public string ErrorMessage { get; set; }
    }
}
