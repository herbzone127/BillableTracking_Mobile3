namespace BillableTracking.Shared.Models
{
    public class ServiceResponseMessage<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = "";
        public int StatusCode { get; set; }
    }
}
