using Application.Common.Models;
using Application.DTOs.Categories.GetCategories;
using Application.DTOs.Categories;
using Application.Services.Interfaces;
using Domain.Entities.Categories;
using Domain.Shared.Constants;
using Infrastructure.Persistence.Interfaces;
using System.Text.RegularExpressions;

namespace Application.Services;

public class CategoryService : BaseService, ICategoryService
{
    public CategoryService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public async Task<Response<List<GetCategoryResponse>>> GetAllAsync()
    {
        var categoryRepository = UnitOfWork.AsyncRepository<Category>();

        var categories =  (await categoryRepository.ListAsync(c => !c.IsDeleted))
                            .Select(c => new GetCategoryResponse(c))
                            .ToList();

        return new Response<List<GetCategoryResponse>>(true, categories);
    }
    public async Task<Response<CreateCategoryResponse>> CreateCategoryAsync(CreateCategoryRequest requestModel)
    {
        var categoryRepository = UnitOfWork.AsyncRepository<Category>();

        var existPrefix = await categoryRepository.GetAsync(cat => cat.Prefix == requestModel.Prefix);

        if (existPrefix != null)
        {
            return new Response<CreateCategoryResponse>(false, ErrorMessages.DuplicateCategoryPrefix);
        }

        var existCategoryName = await categoryRepository.GetAsync(cat => cat.Name == requestModel.Name);

        if (existCategoryName != null)
        {
            return new Response<CreateCategoryResponse>(false, ErrorMessages.DuplicateCategoryName);
        }

        if (!Regex.IsMatch(requestModel.Prefix, @"[A-Z]{2,8}"))
        {
            return new Response<CreateCategoryResponse>(false, ErrorMessages.InvalidCategoryPrefix);
        }

        var newCategory = new Category
        {
            Prefix = requestModel.Prefix,
            Name = requestModel.Name
        };

        var responseModel = new CreateCategoryResponse(newCategory);

        await categoryRepository.AddAsync(newCategory);
        await UnitOfWork.SaveChangesAsync();

        return new Response<CreateCategoryResponse>(true, Messages.ActionSuccess, responseModel);
    }
}