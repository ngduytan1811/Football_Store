using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FBS.API.Responses.Base;
using FBS.Application.DataTranferObjects.Products;
using FBS.Shared.DataTranferObjects.Base;

namespace FBS.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetRandomProducts();

        Task<BaseTableResponse<ProductDto>> GetProducts(BaseSearchDto<ProductSearchDto> dto);

        Task<BaseResponse<ProductDto>> FindById(Guid id);

        Task<BaseResponse<string>> CreateProductReview(ProductReviewSaveDto dto);

        Task<BaseResponse<string>> CreateProduct(ProductSaveDto dto);

        Task<BaseResponse<string>> UpdateProduct(Guid id, ProductSaveDto dto, List<string> newImages);


        Task<BaseResponse<string>> DeleteProduct(Guid id);

        Task AddProductImages(Guid productId, List<string> images);

        Task RemoveProductImages(Guid productId);
        
    }
}
