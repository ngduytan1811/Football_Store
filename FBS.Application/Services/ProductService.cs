using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using FBS.API.Responses.Base;
using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using FBS.Shared.DataTranferObjects.Base;
using FBS.Shared.Enums;
using FBS.Shared.Helpers;

namespace FBS.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ======================
        // RANDOM PRODUCTS
        // ======================
        public async Task<List<ProductDto>> GetRandomProducts()
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Product>();
            var data = await repo.QueryAll();

            data = data.Include(x => x.ProductSizes)
                       .Include(x => x.ProductColor)
                       .Include(x => x.Category);

            var list = await data.ToListAsync();
            list = list.OrderBy(x => Guid.NewGuid()).Take(6).ToList();

            return list.Select(product => new ProductDto
            {
                Id = product.Id,
                Status = product.Status,
                CategoryName = product.Category?.Name,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Name = product.Name,
                Color = product.ProductColor?.Color,
                Sizes = product.ProductSizes.Select(ps => ps.Size).ToList(),
                Image = !string.IsNullOrEmpty(product.Image)
                        ? "/theme/client/img/product/" + product.Image
                        : string.Empty,
                Description = product.Description,
                CreatedAt = product.CreatedAt
            }).ToList();
        }

        // ======================
        // LIST PRODUCTS
        // ======================
        public async Task<BaseTableResponse<ProductDto>> GetProducts(BaseSearchDto<ProductSearchDto> dto)
        {
            var result = new BaseTableResponse<ProductDto>();

            var queryProduct = await _unitOfWork.GetRepositoryReadOnlyAsync<Product>().QueryAll();
            var queryReview = await _unitOfWork.GetRepositoryReadOnlyAsync<ProductReview>().QueryAll();

            queryProduct = queryProduct.Include(x => x.ProductSizes)
                                       .Include(x => x.ProductColor)
                                       .Include(x => x.Category);

            var search = dto.SearchParams ?? new ProductSearchDto();

            if (!string.IsNullOrEmpty(search.SearchName))
            {
                var key = search.SearchName.ToLower().Trim();
                queryProduct = queryProduct.Where(x =>
                    x.Name.ToLower().Contains(key) ||
                    x.Description.ToLower().Contains(key)
                );
            }

            if (search.Sizes?.Count > 0)
                queryProduct = queryProduct.Where(x =>
                    x.ProductSizes.Any(s => search.Sizes.Contains(s.Size)));

            if (search.Brands?.Count > 0)
                queryProduct = queryProduct.Where(x => search.Brands.Contains(x.Brand));

            if (search.FromPrice.HasValue)
                queryProduct = queryProduct.Where(x => x.Price >= search.FromPrice);

            if (search.ToPrice.HasValue)
                queryProduct = queryProduct.Where(x => x.Price <= search.ToPrice);

            if (search.CategoryId.HasValue)
            {
                // Lấy tất cả danh mục con trực tiếp
                var queryCategories = await _unitOfWork
                    .GetRepositoryReadOnlyAsync<Category>()
                    .QueryAll();

                // Danh sách ID danh mục con
                var childIds = queryCategories
                    .Where(c => c.ParentId == search.CategoryId)
                    .Select(c => c.Id)
                    .ToList();

                // Thêm ID của danh mục CHA vào luôn
                childIds.Add(search.CategoryId.Value);

                // Lọc sản phẩm theo CHA hoặc CON
                queryProduct = queryProduct.Where(x => childIds.Contains(x.CategoryId.Value));
            }



            result.Total = queryProduct.Count();

            var query = queryProduct.Select(product => new ProductDto
            {
                Id = product.Id,
                Status = product.Status,
                CategoryName = product.Category != null ? product.Category.Name : null,

                CategoryId = product.CategoryId,
                Price = product.Price,
                Name = product.Name,
                Color = product.ProductColor.Color,
                Brand = product.Brand,
                Image = product.Image != null
                        ? "/theme/client/img/product/" + product.Image
                        : string.Empty,
                Sizes = product.ProductSizes.Select(ps => ps.Size).ToList(),
                Description = product.Description,
                CreatedAt = product.CreatedAt,
            });

            var (items, totalPage) = TableResponseHelper.MakeToList(query, result.Total, dto.Start, dto.PageSize);

            result.Items = items;
            result.TotalPage = totalPage;

            return result;
        }

        // ======================
        // FIND PRODUCT BY ID
        // ======================
        public async Task<BaseResponse<ProductDto>> FindById(Guid productId)
        {
            var result = new BaseResponse<ProductDto>();

            var repo = await _unitOfWork.GetRepositoryReadOnlyAsync<Product>().QueryAll();

            repo = repo
                .Include(x => x.ProductSizes)
                .Include(x => x.ProductColor)
                .Include(x => x.Category)
                .Include(x => x.ProductImages); 

            var product = repo.FirstOrDefault(i => i.Id == productId);
            if (product == null)
                return result;

            result.Data = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Detail = product.Detail,
                Status = product.Status,
                Color = product.ProductColor?.Color,
                Image = product.Image,

                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Price = product.Price,
                Sizes = product.ProductSizes.Select(ps => ps.Size).ToList(),
                SubImages = product.ProductImages.Select(i => i.ImagePath).ToList()
            };

            return result;
        }

        // ======================
        // CREATE PRODUCT REVIEW
        // ======================
        public async Task<BaseResponse<string>> CreateProductReview(ProductReviewSaveDto dto)
        {
            var result = new BaseResponse<string>();

            var productReviewRepo = _unitOfWork.GetRepositoryAsync<ProductReview>();

            await productReviewRepo.Add(new ProductReview
            {
                ProductId = dto.ProductId,
                FullName = dto.FullName,
                Message = dto.Message,
                Status = StatusEnum.Active
            });

            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        // ======================
        // CREATE PRODUCT
        // ======================
        public async Task<BaseResponse<string>> CreateProduct(ProductSaveDto dto)
        {
            var result = new BaseResponse<string>();

            var productRep = _unitOfWork.GetRepositoryAsync<Product>();
            var productColorRep = _unitOfWork.GetRepositoryAsync<ProductColor>();
            var productSizeRep = _unitOfWork.GetRepositoryAsync<ProductSize>();
            var imageRep = _unitOfWork.GetRepositoryAsync<ProductImage>();

            // ===== Tạo product =====
            var product = new Product
            {
                Name = dto.Name?.Trim(),
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                
                Brand = dto.Brand,
                Price = dto.Price,
                Status = StatusEnum.Active,
                Image = dto.Image,
                Detail = string.Join(
    "\n\n",
    new[] { dto.DetailPart1, dto.DetailPart2 }
        .Where(s => !string.IsNullOrWhiteSpace(s))
),


            };

            await productRep.Add(product);

            // ===== Màu =====
            await productColorRep.Add(new ProductColor
            {
                Product = product,
                Color = dto.Color
            });

            // ===== Sizes =====
            var sizes = dto.Sizes.Distinct().Select(s => new ProductSize
            {
                Product = product,
                Size = s
            }).ToList();

            await productSizeRep.Add(sizes);

            // ===== Lưu product trước =====
            await _unitOfWork.SaveChangesAsync();

            // ===== Lưu ảnh phụ =====
            if (dto.SubImages != null && dto.SubImages.Count > 0)
            {
                foreach (var img in dto.SubImages)
                {
                    await imageRep.Add(new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        ImagePath = img
                    });
                }

                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }


        // ======================
        // ADD MULTIPLE IMAGES
        // ======================
        public async Task AddProductImages(Guid productId, List<string> images)
        {
            var repo = _unitOfWork.GetRepositoryAsync<ProductImage>();

            foreach (var img in images)
            {
                await repo.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    ImagePath = img
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        // ======================
        // UPDATE PRODUCT
        // ======================
        public async Task<BaseResponse<string>> UpdateProduct(Guid id, ProductSaveDto dto, List<string> newImages)
        {
            var result = new BaseResponse<string>();

            var productRep = _unitOfWork.GetRepositoryAsync<Product>();
            var productColorRep = _unitOfWork.GetRepositoryAsync<ProductColor>();
            var productSizeRep = _unitOfWork.GetRepositoryAsync<ProductSize>();
            var productImageRep = _unitOfWork.GetRepositoryAsync<ProductImage>();

            var product = await productRep.Single(x => x.Id == id);
            if (product == null)
                return result;

            var productColor = await productColorRep.Single(x => x.ProductId == id);

            // =============== UPDATE PRODUCT =======================
            product.Name = dto.Name?.Trim();
            product.CategoryId = dto.CategoryId;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Brand = dto.Brand;
            product.Status = dto.Status;
            product.Image = dto.Image;
            productColor.Color = dto.Color;
            product.Detail = string.Join(
    "\n\n",
    new[] { dto.DetailPart1, dto.DetailPart2 }
        .Where(s => !string.IsNullOrWhiteSpace(s))
);



            // =============== UPDATE SIZE ===========================
            var oldSizes = await (await productSizeRep.QueryCondition(x => x.ProductId == id)).ToListAsync();

            foreach (var s in oldSizes)
                await productSizeRep.Delete(s);

            var newSizes = dto.Sizes.Select(size => new ProductSize
            {
                ProductId = product.Id,
                Size = size
            }).ToList();

            await productSizeRep.Add(newSizes);

            // =============== UPDATE PRODUCT & COLOR =================
            await productRep.Update(product);
            await productColorRep.Update(productColor);

            // =============== UPDATE IMAGES =========================
            var oldImages = await (await productImageRep.QueryCondition(x => x.ProductId == id)).ToListAsync();

            var keepImages = dto.OldSubImages ?? new List<string>();

            // XOÁ ảnh phụ bị user xoá
            foreach (var img in oldImages)
            {
                if (!keepImages.Contains(img.ImagePath))
                {
                    await productImageRep.Delete(img);   // chỉ xoá ảnh user xoá
                }
            }

            // THÊM ảnh phụ mới
            if (newImages != null && newImages.Count > 0)
            {
                foreach (var img in newImages)
                {
                    await productImageRep.Add(new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = id,
                        ImagePath = img
                    });
                }
            }

            // Lưu thay đổi
            await _unitOfWork.SaveChangesAsync();

            return result;
        }



        // ======================
        // DELETE SUB IMAGES
        // ======================
        public async Task RemoveProductImages(Guid productId)
        {
            var repo = _unitOfWork.GetRepositoryAsync<ProductImage>();

            var query = await repo.QueryCondition(x => x.ProductId == productId);
            var list = await query.ToListAsync();

            foreach (var img in list)
            {
                await repo.Delete(img);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        // ======================
        // DELETE PRODUCT
        // ======================
        public async Task<BaseResponse<string>> DeleteProduct(Guid id)
        {
            var result = new BaseResponse<string>();

            var repo = _unitOfWork.GetRepositoryAsync<Product>();
            var product = await repo.Single(x => x.Id == id);

            if (product == null)
                return result;

            

            await repo.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }
    }
}
