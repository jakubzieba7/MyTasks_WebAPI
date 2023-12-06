using MyTasks_WebAPI.Models.Domains;
using MyTasks_WebAPI.Models.DTOs;

namespace MyTasks_WebAPI.Models.Converters
{
    public static class CategoryConverter
    {
        public static CategoryDto ToDto(this Category model)
        {
            return new CategoryDto()
            {
                Id = model.Id,
                Name = model.Name,
                UserId = model.UserId,
            };
        }

        public static IEnumerable<CategoryDto> ToDtos(this IEnumerable<Category> model)
        {
            if (model == null)
                return Enumerable.Empty<CategoryDto>();

            return model.Select(x => x.ToDto());
        }

        public static Category ToDao(this CategoryDto model)
        {
            return new Category()
            {
                Id = model.Id,
                Name = model.Name,
                UserId = model.UserId,
            };
        }
    }
}
