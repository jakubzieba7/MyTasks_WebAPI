using Microsoft.AspNetCore.Mvc;
using MyTasks_WebAPI.Models.Response;
using MyTasks_WebAPI.Models;
using Task = MyTasks_WebAPI.Models.Domains.Task;
using Microsoft.AspNetCore.Authorization;

namespace MyTasks_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public TaskController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet(Name = "Get tasks"), Authorize]
        public DataResponse<IEnumerable<Task>> Get(string userId)
        {
            var response = new DataResponse<IEnumerable<Task>>();

            try
            {
                response.Data = _unitOfWork.TaskRepository.Get(userId);
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpGet("{id}")]
        public DataResponse<Task> Get(int id, string userId)
        {
            var response = new DataResponse<Task>();

            try
            {
                response.Data = _unitOfWork.TaskRepository.Get(id, userId);
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpPost]
        public DataResponse<int> Add(Task task)
        {
            var response = new DataResponse<int>();

            try
            {
                _unitOfWork.TaskRepository.Add(task);
                _unitOfWork.Complete();
                response.Data = task.Id;
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpPut]
        public Response Update(Task task)
        {
            var response = new Response();

            try
            {
                _unitOfWork.TaskRepository.Update(task);
                _unitOfWork.Complete();
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpDelete("{id}")]
        public Response Delete(int id, string userId)
        {
            var response = new Response();

            try
            {
                _unitOfWork.TaskRepository.Delete(id, userId);
                _unitOfWork.Complete();
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }
    }
}
