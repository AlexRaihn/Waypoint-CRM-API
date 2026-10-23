using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Formatters;
using HotChocolate.Execution;
using System.Net;

namespace waypoint_server.middlewares
{
    public class ErrorMiddleware : DefaultHttpResponseFormatter
    {
        protected override HttpStatusCode OnDetermineStatusCode(OperationResult result, FormatInfo format, HttpStatusCode? proposedStatusCode)
        {
            var error = result.Errors?.FirstOrDefault();

            return error?.Code switch
            {
                "UNAUTHORIZED" => HttpStatusCode.Unauthorized,
                "FORBIDDEN" => HttpStatusCode.Forbidden,
                "NOT_FOUND" => HttpStatusCode.NotFound,
                "BAD_REQUEST" => HttpStatusCode.BadRequest,
                "INTERNAL_SERVER_ERROR" => HttpStatusCode.InternalServerError,

                _ => base.OnDetermineStatusCode(
                    result,
                    format,
                    proposedStatusCode)
            };
        }
    }
}