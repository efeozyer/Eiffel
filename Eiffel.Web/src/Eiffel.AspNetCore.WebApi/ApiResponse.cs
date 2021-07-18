using System.Runtime.Serialization;

namespace Eiffel.AspNetCore.WebApi
{
    public class ApiResponse<T>
        where T : class
    {
        public ApiResponse()
        {

        }
        public ApiResponse(int statusCode, T obj, string errorMessage)
        {
            StatusCode = statusCode;
            Body = obj;
            ErrorMessage = errorMessage;
        }

        public int StatusCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public T Body { get; set; }

        public bool IsSucceeded { get; set; } = true;
    }
}
