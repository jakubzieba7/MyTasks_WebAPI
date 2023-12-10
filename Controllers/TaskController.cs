using Microsoft.AspNetCore.Mvc;
using MyTasks_WebAPI.Models.Response;
using MyTasks_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using MyTasks_WebAPI.Models.DTOs;
using MyTasks_WebAPI.Models.Converters;
using System.Security.Claims;

namespace MyTasks_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public TaskController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all tasks by userId, page size and page number
        /// </summary>
        /// <param name="paginationFilter">Page size and page number</param>
        /// <returns>DataResponse - IEnumerable TaskDto</returns>
        [HttpGet]
        public DataResponse<IEnumerable<TaskDto>> Get([FromQuery] PaginationFilter paginationFilter)
        {
            var response = new DataResponse<IEnumerable<TaskDto>>();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
        /// <returns>DataResponse - TaskDto</returns>
        [HttpGet("{id}")]
        public DataResponse<TaskDto> Get(int id)
        {
            var response = new DataResponse<TaskDto>();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
                var task = taskDto.ToDao();
                task.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
                var task = taskDto.ToDao();
                task.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        /// <summary>
        /// Delete task by task id and userId
        /// </summary>
        /// <param name="id">Task id</param>
        /// <returns>Response</returns>
        [HttpDelete]
        public Response Delete(int id)
        {
            var response = new Response();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
