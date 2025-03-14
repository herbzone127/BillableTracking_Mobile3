namespace BillableTracking.Shared.Dto
{
    public class ResponseData<T> where T : class
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}
