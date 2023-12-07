using Microsoft.AspNetCore.Mvc;
using MyTasks_WebAPI.Models.Response;
using MyTasks_WebAPI.Models;
using Task = MyTasks_WebAPI.Models.Domains.Task;
using Microsoft.AspNetCore.Authorization;
using MyTasks_WebAPI.Models.DTOs;
using MyTasks_WebAPI.Models.Converters;

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
        public DataResponse<IEnumerable<TaskDto>> Get(string userId)
        {
            var response = new DataResponse<IEnumerable<TaskDto>>();

            try
            {
                response.Data = _unitOfWork.TaskRepository.Get(userId)?.ToDtos();
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpGet("{id}")]
        public DataResponse<TaskDto> Get(int id, string userId)
        {
            var response = new DataResponse<TaskDto>();

            try
            {
                response.Data = _unitOfWork.TaskRepository.Get(id, userId)?.ToDto();
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpPost]
        public DataResponse<int> Add(TaskDto taskDto)
        {
            var response = new DataResponse<int>();

            try
            {
                var task=taskDto.ToDao();
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
        public Response Update(TaskDto taskDto)
        {
            var response = new Response();

            try
            {
                _unitOfWork.TaskRepository.Update(taskDto.ToDao());
                _unitOfWork.Complete();
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpDelete]
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
