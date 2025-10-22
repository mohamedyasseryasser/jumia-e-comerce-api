using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace e_comerce_api.DTO
{
    internal class ApiErrorResponse : ModelStateDictionary
    {
        public string Message { get; set; }
        public string[] ErrorMessages { get; set; }
    }
    public class ApiResponse<T>
    {
        public string message {  get; set; }
        public T data { get; set; }
        public bool statue {  get; set; }
    }
}