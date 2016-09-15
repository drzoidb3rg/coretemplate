using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.MiddleWare
{
    public class LiveErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        internal static IDictionary<int, string> ErrorPages { get; } = new Dictionary<int, string>
        {
            {404, "/NotFound" },
            {401, "/ServerError"},
            {402, "/ServerError"},
            {403, "/ServerError"},
            {500, "/ServerError"},
            {503, "/ServerError"}
        };

        public LiveErrorHandlingMiddleware(ILoggerFactory loggerFactory, RequestDelegate next)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<LiveErrorHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message);

                if(context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error page middleware will not be executed.");
                    throw;
                } try
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 500;
                    context.Request.Headers.Add("Message", ex.Message);
                    context.Request.Headers.Add("Stack", ex.StackTrace);
                    return;
                } catch (Exception ex2)
                {
                    _logger.LogError(ex2.Message);
                }
                throw;
            } finally
            {
                var statusCode = context.Response.StatusCode;
                if(statusCode == 404)
                {
                    _logger.LogError(string.Format("Page not found {0}", context.Request.Path));
                }

                var contentType = context.Request.Headers["Accept"].FirstOrDefault();

                if (ErrorPages.Keys.Contains(statusCode) && !string.IsNullOrEmpty(contentType) && contentType.Contains("text/html"))
                {
                    context.Request.Path = ErrorPages[statusCode];
                    await _next(context);
                }
            }
        }
    }
}
