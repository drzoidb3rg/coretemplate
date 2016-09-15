using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.CustomErrorHandling
{
    public class TestErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public TestErrorHandlingMiddleware(ILoggerFactory loggerFactory, RequestDelegate next)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<TestErrorHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                _logger.LogError(ex.Message);

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error page middleware will not be executed.");
                }
                throw;
            }
            finally
            {
                var statusCode = context.Response.StatusCode;

                if (statusCode == 404)
                {
                    _logger.LogError(string.Format("Page not found {0}", context.Request.Path));
                    context.Request.Path = "/notfound";
                    await _next(context);
                }
            }
        }
    }
}
