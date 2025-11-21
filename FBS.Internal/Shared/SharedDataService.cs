using FBS.Application.DataTranferObjects.Categories;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;

public interface ISharedDataService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
}

public class SharedDataService : ISharedDataService
{
    private List<CategoryDto> _cache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public SharedDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        if (_cache != null)
            return _cache;

        await _lock.WaitAsync();
        try
        {
            if (_cache == null)
            {
                _cache = await LoadCategoriesFromDb();
            }
        }
        finally
        {
            _lock.Release();
        }

        return _cache;
    }

    private async Task<List<CategoryDto>> LoadCategoriesFromDb()
    {
        await Task.Delay(200);
        var result = new List<CategoryDto>();
        var query = await _unitOfWork.GetRepositoryReadOnlyAsync<Category>().QueryAll();

        result = query.Where(x => x.IsActive).Select(x => new CategoryDto
        {
            Name = x.Name,
        }).ToList();

        return result;
    }
}
