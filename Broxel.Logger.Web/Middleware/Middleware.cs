using Microsoft.AspNetCore.Http;

namespace Broxel.Logger.Web.Middleware
{
    public  class Middleware
    {
        private readonly RequestDelegate _next;

        public Middleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        { 

            await _next(context);
        }
    }
}