using System.Text;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;


namespace Broxel.Logger.Web.Filter
{
    public class FilterLogger : IAsyncActionFilter, IExceptionFilter
    {
        private static IHttpClientFactory _httpClientFactory;

        public FilterLogger(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        string methodName = string.Empty;
        string displayName = string.Empty;
        string statusCode = string.Empty;
        string message = string.Empty;
        string stackTrace = string.Empty;

        string messageLogInf = string.Empty;
        string messageLogWar = string.Empty;
        string messageLogErr = string.Empty;


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            methodName = context.HttpContext.Request.Method.ToString();
            displayName = context.ActionDescriptor.DisplayName.ToString();
            statusCode = context.HttpContext.Response.StatusCode.ToString();

            //Antes del método

            await next();

            messageLogInf = $"INFORMATION=======> {statusCode} {displayName} {methodName}";
            //Llamar endpoint Audit
            var resulInf = InvokeBroxelLoggerApi("Information", messageLogInf);
        }



        public void OnException(ExceptionContext context)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Parameters :[");
            foreach (var item in context.ModelState.Keys)
            {
                stringBuilder.Append($"{item}:{context.ModelState[item]?.RawValue} ,");
            }
            stringBuilder.Append("]");
            string parameters = stringBuilder.ToString();


            methodName = context.HttpContext.Request.Method.ToString();
            displayName = context.ActionDescriptor.DisplayName.ToString();
            message = context.Exception.Message.ToString();
            stackTrace = context.Exception.StackTrace.ToString();


            var error = new Error
            {
                StatusCode = 500,
                Message = context.Exception.Message,

            };
            context.Result = new JsonResult(error) { StatusCode = 500 };



            if (parameters == "Parameters :[]")
            {
                messageLogWar = $"WARNING==========>  {error.StatusCode} {displayName} {methodName} {message}";
                var resultWar = InvokeBroxelLoggerApi("Warning", messageLogWar);
                messageLogErr = $"ERROR============>  {error.StatusCode} {displayName} {methodName} {message} {stackTrace}";
                var resultErr = InvokeBroxelLoggerApi("Error", messageLogErr);
            }
            else
            {
                messageLogWar = $"WARNING==========>  {error.StatusCode} {displayName} {methodName} {parameters} {message}";
                var resultWar = InvokeBroxelLoggerApi("Warning", messageLogWar);
                messageLogErr = $"ERROR============>  {error.StatusCode} {displayName} {methodName} {parameters} {message} {stackTrace}";
                var resultErr = InvokeBroxelLoggerApi("Error", messageLogErr);
            }
        }

        
        public class Error
        {
            public int StatusCode { get; set; }
            public string Message { get; set; } = string.Empty;
        }



        public async Task InvokeBroxelLoggerApi(string typeLog, string messageLog)
        {
            try
            {
                //_httpClientFactory.CreateClient("http://localhost:5070/BroxelLogging");

                using (var httpClient = new HttpClient())
                {
                    var url = "http://localhost:5070/BroxelLogging";
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var messageInf = new MessageLog() { Message = messageLog };
                    var messageInfSerializada = JsonSerializer.Serialize(messageInf);
                    var content = new StringContent(messageInfSerializada, Encoding.UTF8, "application/json");
                    var result = new HttpResponseMessage();

                    switch (typeLog)
                    {
                        case "Information":
                            result = await httpClient.PostAsync(url + $"/PostLogAudit?messageLog={messageLog}", content);
                            break;
                        case "Warning":
                            result = await httpClient.PostAsJsonAsync(url + $"/PostLogApplication?messageLog={messageLog}", content);
                            break;
                        case "Error":
                            result = await httpClient.PostAsJsonAsync(url + $"/PostLogOperational?messageLog={messageLog}", content);
                            break;
                    }

                    if (result.IsSuccessStatusCode)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
