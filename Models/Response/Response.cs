
namespace MyTasks_WebAPI.Models.Response
{
    public class Response
    {
        public Response()
        {
            Errors = new List<Error>();
        }
        public bool IsSuccess => Errors is null || !Errors.Any();
        public List<Error> Errors { get; set; }
    }
}
