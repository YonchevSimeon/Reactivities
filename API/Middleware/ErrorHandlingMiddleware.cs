using System.Net;
namespace API.Middleware
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Application.Errors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandlingMiddleware> logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception e)
            {
                await this.HandleExceptionAsync(context, e);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            object errors = null;

            switch (e)
            {
                case RestException re: 
                    this.logger.LogError(e, "REST ERROR"); 
                    errors = re.Errors; 
                    context.Response.StatusCode = (int)re.Code;
                    break;
                
                default:
                    this.logger.LogError(e, "SERVER ERROR");
                    errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            
            if(errors != null) 
            {
                string result = JsonSerializer.Serialize(new
                {
                    errors
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}