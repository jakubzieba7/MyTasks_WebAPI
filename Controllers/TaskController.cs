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

        /// <summary>
        /// Get all tasks by userId
        /// </summary>
        /// <param name="userId">Logged userId</param>
        /// <returns>DataResponse - IEnumerable TaskDto</returns>
        [HttpGet(Name = "Get tasks"), Authorize]
        public DataResponse<IEnumerable<TaskDto>> Get([FromQuery] PaginationFilter paginationFilter, string userId)
        {
            var response = new DataResponse<IEnumerable<TaskDto>>();

            try
            {
                response.Data = _unitOfWork.TaskRepository.Get(paginationFilter, userId)?.ToDtos();
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        /// <summary>
        /// Get task by id and userId
        /// </summary>
        /// <param name="id">Task id</param>
        /// <param name="userId">Logged userId</param>
        /// <returns>DataResponse - TaskDto</returns>
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

        /// <summary>
        /// Add task
        /// </summary>
        /// <param name="taskDto">TaskDto object</param>
        /// <returns>DataResponse - int</returns>
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

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="taskDto">TaskDto object</param>
        /// <returns>Response</returns>
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

        /// <summary>
        /// Delete task by task id and userId
        /// </summary>
        /// <param name="id">Task id</param>
        /// <param name="userId">Logged userId</param>
        /// <returns>Response</returns>
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
