using FBS.Application.DataTranferObjects.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.Services.Interfaces
{
    public interface IProductSizeService
    {
        Task UpsertAsync(UpsertProductSizeDto dto);
        Task<List<ProductSizeStockDto>> GetByProductColorAsync(Guid productColorId);
    }

}
