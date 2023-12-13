
namespace MyTasks_WebAPI.Models.Response
{
    public class Response
    {
        public Response()
        {
            Errors = new List<Error>();
        }
        public string Status { get; set; }
        public string Message { get; set; }
        public bool IsSuccess => Errors is null || !Errors.Any();
        public List<Error> Errors { get; set; }
    }
}
