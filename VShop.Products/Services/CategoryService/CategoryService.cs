using AutoMapper;
using Microsoft.EntityFrameworkCore;

using VShop.Products.Dto;
using VShop.Products.Models;
using VShop.Products.Repositorys;

namespace VShop.Products.Services.CategoryService
{
    public class CategoryService : ICategoryInterface
    {
        private ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategories()
        {
            
            var categoriesEntity = await _categoryRepository.GetAll();

            
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categoriesEntity);

            return categoriesDto;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesProducts()
        {
            var categoriesEntity = await _categoryRepository.GetCategoryProducts();
            return _mapper.Map<IEnumerable<CategoryDto>>(categoriesEntity);
        }


        public async Task<CategoryDto> GetCategoryById(int id)
        {
            var categoryEntity = await _categoryRepository.GetById(id);
            return _mapper.Map<CategoryDto>(categoryEntity);
        }

        public async Task AddCategory(CategoryDto categoryDto)
        {
            var categoryEntity = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.Create(categoryEntity);
            categoryDto.CategoryId = categoryEntity.CategoryId;
        }

        public async Task UpdateCategory(CategoryDto categoryDto)
        {
            var categoryEntity = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.Update(categoryEntity);
        }

        public async Task RemoveCategory(int id)
        {
            var categoryEntity = _categoryRepository.GetById(id).Result;
            await _categoryRepository.DeleteById(categoryEntity.CategoryId);
        }

    }
}
