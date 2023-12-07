using Microsoft.AspNetCore.Mvc;
using MyTasks_WebAPI.Models.Response;
using MyTasks_WebAPI.Models;
using MyTasks_WebAPI.Models.Domains;

namespace MyTasks_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public CategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public DataResponse<IEnumerable<Category>> Get(string userId)
        {
            var response = new DataResponse<IEnumerable<Category>>();

            try
            {
                response.Data = _unitOfWork.CategoryRepository.GetCategories(userId);
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpGet("{id}")]
        public DataResponse<Category> Get(int id, string userId)
        {
            var response = new DataResponse<Category>();

            try
            {
                response.Data = _unitOfWork.CategoryRepository.Get(id, userId);
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpPost]
        public DataResponse<int> Add(Category category)
        {
            var response = new DataResponse<int>();

            try
            {
                _unitOfWork.CategoryRepository.Add(category);
                _unitOfWork.Complete();
                response.Data = category.Id;
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        [HttpPut]
        public Response Update(Category category)
        {
            var response = new Response();

            try
            {
                _unitOfWork.CategoryRepository.Update(category);
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
                _unitOfWork.CategoryRepository.Delete(id, userId);
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
