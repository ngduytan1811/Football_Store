// <copyright file= IProductService.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Application.Services.Interfaces
{
    using System.Threading.Tasks;
    using FBS.API.Responses.Base;
    using FBS.Application.DataTranferObjects.Products;
    using FBS.Shared.DataTranferObjects.Base;

    public interface IProductService
    {
        Task<List<ProductDto>> GetRandomProducts();

        Task<BaseTableResponse<ProductDto>> GetProducts(BaseSearchDto<ProductSearchDto> dto);

        Task<BaseResponse<ProductDto>> FindById(Guid id);

        Task<BaseResponse<string>> CreateProductReview(ProductReviewSaveDto dto);

        Task<BaseResponse<string>> CreateProduct(ProductSaveDto dto);

        Task<BaseResponse<string>> UpdateProduct(Guid id, ProductSaveDto dto);

        Task<BaseResponse<string>> DeleteProduct(Guid id);
    }
}
