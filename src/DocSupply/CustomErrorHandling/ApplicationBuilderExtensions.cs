using DocSupply.MiddleWare;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.CustomErrorHandling
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLiveErrorHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LiveErrorHandlingMiddleware>();
        }

        public static IApplicationBuilder UseTestErrorHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TestErrorHandlingMiddleware>();
        }
    }
}
