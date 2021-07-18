using Eiffel.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eiffel.AspNetCore.WebApi
{
    public class ApiResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResponseWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var responseBody = context.Response.Body;

            var statusCode = context.Response.StatusCode;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                string errorBody = string.Empty;
                try
                {
                    await _next(context);
                }
                catch(DomainException ex)
                {
                    errorBody = HandleException(ex, context, 500, true);
                }
                catch(Exception ex)
                {
                    errorBody = HandleException(ex, context, 500);
                }

                memoryStream.Position = 0;

                string responseString = new StreamReader(memoryStream).ReadToEnd();

                string apiResponseBody = HandleResponse(responseString, context, statusCode);

                byte[] responseBytes = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(errorBody) ? apiResponseBody : errorBody);

                context.Response.Body = responseBody;

                await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }

        private string HandleResponse(string originalBody, HttpContext context, int statusCode)
        {
            ApiResponse<dynamic> apiResponse;

            if (IsJsonValid(originalBody, out dynamic objBody))
            {
                context.Response.Headers["Content-type"] = "application/vnd.api+json";

                if (statusCode >= 200 && statusCode < 299)
                {
                    apiResponse = new ApiResponse<dynamic>(statusCode, objBody, null);
                }
                else
                {
                    string errorMessage = context.Response.StatusCode != 500 ? StatusCodeMessage(context.Response.StatusCode) : originalBody;

                    apiResponse = new ApiResponse<dynamic>()
                    {
                        StatusCode = statusCode,
                        ErrorMessage = errorMessage,
                        IsSucceeded = false
                    };
                }

                string newBody = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                return newBody;
            }

            return originalBody;
        }

        private string HandleException(Exception ex, HttpContext context, int statusCode, bool isDomainException = false)
        {
            context.Response.Headers["Content-type"] = "application/vnd.api+json";
            context.Response.StatusCode = 200;

            var apiResponse = new ApiResponse<dynamic>()
            {
                StatusCode = statusCode,
                ErrorMessage = isDomainException ? $"{ex.Message}" : ex.ToString(),
                IsSucceeded = false
            };

            string errorBody = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            return errorBody;
        }

        private bool IsJsonValid<T>(string json, out T obj)
        {
            obj = default(T);

            if ((json.StartsWith("{") && json.EndsWith("}")) || (json.StartsWith("[") && json.EndsWith("]")))
            {
                try
                {
                    obj = JsonSerializer.Deserialize<T>(json);
                }
                catch
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private string StatusCodeMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "Bad request.";
                case 401:
                    return "Unauthorized access.";
                case 402:
                    return "Payment required.";
                case 403:
                    return "Forbidden access.";
                case 404:
                    return "Resource not found.";
                case 405:
                    return "Method not allowed.";
                case 406:
                    return "Not acceptable.";
                case 407:
                    return "Proxy authentication required.";
                case 408:
                    return "Request timeout.";
                case 409:
                    return "Conflict";
                case 410:
                    return "Resource is gone.";
                case 411:
                    return "Length is required.";
                case 500:
                    return "Internal server error.";
                case 501:
                    return "Not implemented.";
                case 502:
                    return "Bad gateway.";
                case 503:
                    return "Service unavailable.";
                case 504:
                    return "Gateway timeout.";
                case 505:
                    return "HTTP version not supported.";
            }
            return "";
        }
    }
}
