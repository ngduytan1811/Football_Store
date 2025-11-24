// <copyright file= CatgoryService.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;

namespace FBS.Application.Services
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using FBS.API.Responses.Base;
    using FBS.Application.DataTranferObjects.Products;
    using FBS.Application.Services.Interfaces;
    using FBS.Infrastructure.Entities;
    using FBS.Infrastructure.Repositories.Interfaces;
    using FBS.Shared.Constants;
    using FBS.Shared.DataTranferObjects.Base;
    using FBS.Shared.Enums;
    using FBS.Shared.Helpers;

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductDto>> GetRandomProducts()
        {
            var queryProduct = await _unitOfWork.GetRepositoryReadOnlyAsync<Product>().QueryAll();
            queryProduct = queryProduct.Include(x => x.ProductSizes).Include(x => x.ProductColor).Include(x => x.Category);

            var query = queryProduct.OrderBy(x => Guid.NewGuid())
                .Take(6).Select(product => new ProductDto
                {
                    Id = product.Id,
                    Status = product.Status,
                    CategoryName = product.CategoryId.HasValue ? product.Category.Name : string.Empty,
                    CategoryId = product.CategoryId,
                    Price = product.Price,
                    Name = product.Name,
                    Color = product.ProductColor.Color,
                    Sizes = product.ProductSizes.Select(ps => ps.Size).ToList(),

                    Description = product.Description,
                    CreatedAt = product.CreatedAt,
                });

            return query.ToList();
        }

        public async Task<BaseTableResponse<ProductDto>> GetProducts(BaseSearchDto<ProductSearchDto> dto)
        {
            var result = new BaseTableResponse<ProductDto>();
            var queryProduct = await _unitOfWork.GetRepositoryReadOnlyAsync<Product>().QueryAll();
            var queryProductReview = await _unitOfWork.GetRepositoryReadOnlyAsync<ProductReview>().QueryAll();
            queryProduct = queryProduct.Include(x => x.ProductSizes).Include(x => x.ProductColor).Include(x => x.Category);
            var searchData = dto.SearchParams ?? new ProductSearchDto();

            if (searchData.CategoryId.HasValue)
            {
                queryProduct = queryProduct.Where(x => x.CategoryId == searchData.CategoryId);
            }

            result.Total = queryProduct.Count();

            var query = queryProduct.Select(product => new ProductDto
            {
                Id = product.Id,
                Status = product.Status,
                CategoryName = product.CategoryId.HasValue ? product.Category.Name : string.Empty,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Name = product.Name,
                Color = product.ProductColor.Color,
                Sizes = product.ProductSizes.Select(ps => ps.Size).ToList(),

                Description = product.Description,
                CreatedAt = product.CreatedAt,
            });

            query = dto.ColumnSort switch
            {
                ColumnNames.CreatedAt => dto.Asc ? query.OrderBy(i => i.CreatedAt) : query.OrderByDescending(i => i.CreatedAt),
                _ => query,
            };

            var (items, totalPage) = TableResponseHelper.MakeToList(query, result.Total, dto.Start, dto.PageSize);

            var productIds = items.Select(x => x.Id).ToList();

            var reivews = queryProductReview.Where(x => productIds.Contains(x.ProductId.Value)).ToList();

            foreach (var item in items)
            {
                var reviewArr = reivews.Where(x => x.ProductId == item.Id);
                item.Reviews = reviewArr.Select(x => new ProductReivewDto
                {
                    FullName = x.FullName,
                    Message = x.Message
                }).ToList();
            }

            result.Items = items;
            result.TotalPage = totalPage;

            return result;
        }

        public async Task<BaseResponse<ProductDto>> FindById(Guid productId)
        {
            var result = new BaseResponse<ProductDto>();

            var queryProduct = await _unitOfWork.GetRepositoryReadOnlyAsync<Product>().QueryAll();
            queryProduct = queryProduct
                .Include(x => x.ProductSizes)
                .Include(x => x.ProductColor)
                .Include(x => x.Category);

            var product = queryProduct.FirstOrDefault(i => i.Id == productId);

            if (product == null)
                return result;

            result.Data = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Detail = product.Detail,
                Status = product.Status,
                Color = product.ProductColor.Color,
                Image = product.Image,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Price = product.Price,
                Sizes = product.ProductSizes.Select(ps => ps.Size).ToList()
            };

            return result;
        }


        public async Task<BaseResponse<string>> CreateProductReview(ProductReviewSaveDto dto)
        {
            var result = new BaseResponse<string>();
            var productReviewRep = _unitOfWork.GetRepositoryAsync<ProductReview>();

            await productReviewRep.Add(new ProductReview
            {
                ProductId = dto.ProductId,
                Message = dto.Message,
                FullName = dto.FullName,
                Status = StatusEnum.Active,
            });

            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<BaseResponse<string>> CreateProduct(ProductSaveDto dto)
        {
            var result = new BaseResponse<string>();
            var productColorRep = _unitOfWork.GetRepositoryAsync<ProductColor>();
            var productSizeRep = _unitOfWork.GetRepositoryAsync<ProductSize>();

            var newProduct = new Product
            {
                Name = dto.Name.Trim(),
                CategoryId = dto.CategoryId,
                Description = dto.Description?.Trim(),
                Price = dto.Price,
                Detail = dto.Detail,
                Status = StatusEnum.Active,
            };

            await productColorRep.Add(new ProductColor
            {
                Product = newProduct,
                Color = dto.Color,
            });

            var productSizesinsert = dto.Sizes.Select(x=>new ProductSize
            {
                Product = newProduct,
                Size = x,
            }).ToList();
            await productSizeRep.Add(productSizesinsert);
           

            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<BaseResponse<string>> UpdateProduct(Guid id, ProductSaveDto dto)
        {
            var result = new BaseResponse<string>();

            var productRep = _unitOfWork.GetRepositoryAsync<Product>();
            var productSizeRep = _unitOfWork.GetRepositoryAsync<ProductSize>();
            var productColorRep = _unitOfWork.GetRepositoryAsync<ProductColor>();

            var product = await productRep.Single(x => x.Id == id);

            if (product == null)
            {
                return result;
            }

            // GET COLOR
            var productColor = await productColorRep.Single(x => x.ProductId == product.Id);

            // GET ALL SIZES (dùng QueryCondition)
            var query = await productSizeRep.QueryCondition(x => x.ProductId == product.Id);
            var productSizes = await query.ToListAsync();

            // UPDATE PRODUCT INFO
            product.Name = dto.Name.Trim();
            product.CategoryId = dto.CategoryId;
            product.Status = dto.Status;
            product.Price = dto.Price;
            product.Description = dto.Description;

            // UPDATE COLOR
            productColor.Color = dto.Color;

            // ❗ DELETE OLD SIZES
            foreach (var item in productSizes)
            {
                await productSizeRep.Delete(item);
            }

            // ❗ ADD NEW SIZES (distinct tránh trùng)
            var newSizes = dto.Sizes.Distinct().Select(size => new ProductSize
            {
                ProductId = product.Id,
                Size = size
            }).ToList();

            await productSizeRep.Add(newSizes);

            // SAVE ALL
            await productRep.Update(product);
            await productColorRep.Update(productColor);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }


        public async Task<BaseResponse<string>> DeleteProduct(Guid id)
        {
            var result = new BaseResponse<string>();

            var productRep = _unitOfWork.GetRepositoryAsync<Product>();

            var product = await productRep.Single(x => x.Id == id);

            if (product == null)
            {
                return result;
            }

            await productRep.Delete(product);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }
    }
}
