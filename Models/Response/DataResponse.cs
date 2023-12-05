namespace MyTasks_WebAPI.Models.Response
{
    public class DataResponse<T> : Response
    {
        public T Data { get; set; }
    }
}
