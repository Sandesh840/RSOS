
using Application.DTOs.Base;
using Application.DTOs.Student;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using System.Net;

namespace RSOS.Helper
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerActionDescriptor != null)
                {
                    var actionName = controllerActionDescriptor.ActionName;
                    //var controllerName = controllerActionDescriptor.ControllerName;
                    //var routeTemplate = controllerActionDescriptor.AttributeRouteInfo.Template;

                    if (actionName == "AuthenticateNew")
                    {
                        context.Items["RequestEntryTime"] = DateTime.UtcNow;
                        _logger.LogError($"Request received and queued at {context.Items["RequestEntryTime"]}");
                    }
                    else if (actionName == "GetStudentResponsesResultAuthorize")
                    {
                        //var response = new ResponseDTO<StudentResponseDTO>
                        //{
                        //    Status = "Success",
                        //    Message = "Successfully Retrieved",
                        //    StatusCode = HttpStatusCode.OK,
                        //    Result = new StudentResponseDTO()
                        //};

                        //var jsonResponse = JsonConvert.SerializeObject(response);
                        var response = "{\r\n\"statusCode\": 200,\r\n    \"status\": \"Success\",\r\n    \"message\": \"Successfully Retrieved\",\r\n    \"result\": {\r\n        \"studentResponses\": null,\r\n        \"studentScores\": null\r\n    }\r\n}";

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 200;

                        await context.Response.WriteAsync(response);



                        return;
                    }
                }
            }
            
            await _next(context);
        }
    }

}
