﻿using Microsoft.AspNetCore.Mvc;
using MyTasks_WebAPI.Models.Response;
using MyTasks_WebAPI.Models;
using MyTasks_WebAPI.Models.DTOs;
using MyTasks_WebAPI.Models.Converters;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MyTasks_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public CategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all categories by logged userId
        /// </summary>
        /// <returns>DataResponse - CategoryDto</returns>
        [HttpGet]
        public DataResponse<IEnumerable<CategoryDto>> Get()
        {
            var response = new DataResponse<IEnumerable<CategoryDto>>();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                response.Data = _unitOfWork.CategoryRepository.GetCategories(userId)?.ToDtos();
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        /// <summary>
        /// Get category by category id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>DataResponse - CategoryDto</returns>
        [HttpGet("{id}")]
        public DataResponse<CategoryDto> Get(int id)
        {
            var response = new DataResponse<CategoryDto>();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                response.Data = _unitOfWork.CategoryRepository.Get(id, userId)?.ToDto();
            }
            catch (Exception exception)
            {
                //logowanie do pliku
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        /// <summary>
        /// Add category
        /// </summary>
        /// <param name="categoryDto">CategoryDto object</param>
        /// <returns>DataResponse - int</returns>
        [HttpPost]
        public DataResponse<int> Add(CategoryDto categoryDto)
        {
            var response = new DataResponse<int>();

            try
            {
                var category = categoryDto.ToDao();
                category.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        /// <summary>
        /// Update category 
        /// </summary>
        /// <param name="categoryDto">CategoryDto object</param>
        /// <returns>Response</returns>
        [HttpPut]
        public Response Update(CategoryDto categoryDto)
        {
            var response = new Response();

            try
            {
                var category = categoryDto.ToDao();
                category.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        /// <summary>
        /// Delete category by category id and userId
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>Response</returns>
        [HttpDelete]
        public Response Delete(int id)
        {
            var response = new Response();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
