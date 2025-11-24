namespace ASM_C6.API.Helpers
{

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public static ApiResponse<T> SuccessResponse(T data, string msg = "") => new ApiResponse<T> { Success = true, Data = data, Message = msg };
        public static ApiResponse<T> ErrorResponse(string msg) => new ApiResponse<T> { Success = false, Message = msg };
    }
}
