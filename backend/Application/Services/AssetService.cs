using Application.Common.Models;
using Application.DTOs.Assets.GetAsset;
using Application.DTOs.Assets.GetListAssets;
using Application.Helpers;
using Application.Queries;
using Application.Services.Interfaces;
using Domain.Entities.Assets;
using Domain.Shared.Constants;
using Domain.Shared.Enums;
using Infrastructure.Persistence.Interfaces;

namespace Application.Services;

public class AssetService : BaseService, IAssetService
{
    private readonly IAssetRepository _assetRepository;

    public AssetService(
        IUnitOfWork unitOfWork,
        IAssetRepository assetRepository) : base(unitOfWork)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Response<GetAssetResponse>> GetAsync(GetAssetRequest request)
    {
        var asset = await _assetRepository
            .GetAsync(a => !a.IsDeleted &&
                            a.Id == request.Id &&
                            a.Location == request.Location);

        if (asset == null)
        {
            return new Response<GetAssetResponse>(false, ErrorMessages.NotFound);
        }

        var responseModel = new GetAssetResponse(asset);

        return new Response<GetAssetResponse>(true, Messages.ActionSuccess, responseModel);
    }

    public async Task<Response<GetListAssetsResponse>> GetListAsync(GetListAssetsRequest request)
    {
        var assets = (await _assetRepository.ListAsync(a => !a.IsDeleted && 
                                                            a.Location == request.Location))
                    .Select(a => new GetAssetResponse(a))
                    .AsQueryable();

        var validSortFields = new[]
        {
            ModelField.Name,
            ModelField.AssetCode,
            ModelField.Category,
            ModelField.State
        };

        var validSearchFields = new[]
        {
            ModelField.Name,
            ModelField.AssetCode
        };

        var validFilterFields = new []
        {
            ModelField.Category,
            ModelField.State
        };

        var filterQueries = new List<FilterQuery>();

        if (!string.IsNullOrEmpty(request.AssetFilter.AssetState))
        {
            filterQueries.Add(new()
            {
                FilterField = ModelField.State,
                FilterValue = request.AssetFilter.AssetState
            });
        }

        if (!string.IsNullOrEmpty(request.AssetFilter.Category))
        {
            filterQueries.Add(new()
            {
                FilterField = ModelField.Category,
                FilterValue = request.AssetFilter.Category
            });
        }

        var processedList = assets
            .MultipleFiltersByField(validFilterFields, filterQueries)
            .SearchByField(validSearchFields, request.SearchQuery.SearchValue)
            .SortByField(validSortFields, request.SortQuery.SortField, request.SortQuery.SortDirection);

        var pagedList = new PagedList<GetAssetResponse>(
                                processedList, 
                                request.PagingQuery.PageIndex, 
                                request.PagingQuery.PageSize);
        
        var responseData = new GetListAssetsResponse(pagedList); 

        return new Response<GetListAssetsResponse>(true, responseData);
    }
}