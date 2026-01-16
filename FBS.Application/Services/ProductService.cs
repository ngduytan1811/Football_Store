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

        public async Task<List<ProductDto>> GetRandomProducts()
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Product>();
            var data = await repo.QueryAll();

            data = data.Include(x => x.Category);

            var colorRepo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductColor>();
            var colors = await (await colorRepo.QueryAll())
                .Include(c => c.ProductSizes)
                .ToListAsync();

            var list = await data.ToListAsync();
            list = list.OrderBy(x => Guid.NewGuid()).Take(6).ToList();

            return list.Select(product =>
            {
               
                var productColors = colors
                    .Where(c => c.ProductId == product.Id)
                    .ToList();

                var sizes = productColors
                    .SelectMany(c => c.ProductSizes)
                    .Select(ps => ps.Size);

                return new ProductDto
                {
                    Id = product.Id,
                    Status = product.Status,
                    CategoryName = product.Category?.Name,
                    CategoryId = product.CategoryId,
                    Price = product.Price,
                    Discount = product.Discount,
                    Name = product.Name,       
                    Color = productColors.FirstOrDefault()?.Color,
                    Sizes = SortSizes(sizes),
                    Image = !string.IsNullOrEmpty(product.Image)
                        ? "/theme/client/img/product/" + product.Image
                        : string.Empty,
                    Description = product.Description,
                    CreatedAt = product.CreatedAt
                };
            }).ToList();
        }


        private static readonly List<string> SizeOrder = new()
{
    "XS", "S", "M", "L", "XL", "2XL", "3XL"
};
        private List<string> SortSizes(IEnumerable<string> sizes)
        {
            return sizes
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .OrderBy(s =>
                {
                    
                    if (int.TryParse(s, out int number))
                        return number;

                    
                    var index = SizeOrder.IndexOf(s.ToUpper());
                    return index >= 0 ? 1000 + index : 2000;
                })
                .ToList();
        }
        public async Task<BaseTableResponse<ProductDto>> GetProducts(BaseSearchDto<ProductSearchDto> dto)
        {
            var result = new BaseTableResponse<ProductDto>();

            var queryProduct = await _unitOfWork
                .GetRepositoryReadOnlyAsync<Product>()
                .QueryAll();
            queryProduct = queryProduct
                .Include(p => p.Category)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.ProductSizes);

            var search = dto.SearchParams ?? new ProductSearchDto();

            if (!string.IsNullOrWhiteSpace(search.SearchName))
            {
                var key = search.SearchName.Trim().ToLower();
                queryProduct = queryProduct.Where(p =>
                    p.Name.ToLower().Contains(key) ||
                    p.Description.ToLower().Contains(key));
            }
            if (search.Sizes?.Any() == true)
            {
                queryProduct = queryProduct.Where(p =>
                    p.ProductColors.Any(pc =>
                        pc.ProductSizes.Any(ps =>
                            search.Sizes.Contains(ps.Size))));
            }
          
            if (search.Brands?.Any() == true)
            {
                queryProduct = queryProduct.Where(p =>
                    search.Brands.Contains(p.Brand));
            }
            if (search.FromPrice.HasValue)
                queryProduct = queryProduct.Where(p =>
                    p.Price >= search.FromPrice);

            if (search.ToPrice.HasValue)
                queryProduct = queryProduct.Where(p =>
                    p.Price <= search.ToPrice);

            if (search.CategoryId.HasValue)
            {
                var parentId = search.CategoryId.Value;

                var categoryRepo = _unitOfWork.GetRepositoryReadOnlyAsync<Category>();
                var categoryIds = await (await categoryRepo.QueryAll())
                    .Where(c => c.Id == parentId || c.ParentId == parentId)
                    .Select(c => c.Id)
                    .ToListAsync();

                queryProduct = queryProduct.Where(p =>
                    p.CategoryId.HasValue &&
                    categoryIds.Contains(p.CategoryId.Value));
            }

            
            switch (search.Sort)
            {
                case "Price_Asc":
                    queryProduct = queryProduct.OrderBy(p => p.Price);
                    break;
                case "Price_Desc":
                    queryProduct = queryProduct.OrderByDescending(p => p.Price);
                    break;
                case "Name_Asc":
                    queryProduct = queryProduct.OrderBy(p => p.Name);
                    break;
                case "Name_Desc":
                    queryProduct = queryProduct.OrderByDescending(p => p.Name);
                    break;
                default:
                    queryProduct = queryProduct.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            result.Total = await queryProduct.CountAsync();

            var products = await queryProduct.ToListAsync();

           
            var mappedItems = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Status = p.Status,
                CategoryName = p.Category?.Name,
                CategoryId = p.CategoryId,
                Name = p.Name,
                Brand = p.Brand,
                Price = p.Price,
                Discount = p.Discount??0,
                Color = p.ProductColors.FirstOrDefault()?.Color,

                Sizes = SortSizes(
                    p.ProductColors
                        .SelectMany(pc => pc.ProductSizes)
                        .Select(ps => ps.Size)
                        .Distinct()
                ),

                Image = !string.IsNullOrEmpty(p.Image)
                    ? "/theme/client/img/product/" + p.Image
                    : string.Empty,

                CreatedAt = p.CreatedAt
            }).ToList();

            var (pagedItems, totalPage) =
                TableResponseHelper.MakeToList<ProductDto>(
                    mappedItems.AsQueryable(),
                    result.Total,
                    dto.Start,
                    dto.PageSize
                );

            result.Items = pagedItems;
            result.TotalPage = totalPage;

            return result;
        }
        public async Task<BaseResponse<ProductDto>> FindById(Guid productId)
        {
            var result = new BaseResponse<ProductDto>();

            var productRepo = await _unitOfWork
                .GetRepositoryReadOnlyAsync<Product>()
                .QueryAll();

            var product = await productRepo
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
                return result;

           
            var colorRepo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductColor>();
            var colors = await (await colorRepo.QueryAll())
                .Where(c => c.ProductId == productId)
                .Include(c => c.ProductSizes)
                .ToListAsync();

        
            var allSizes = colors
                .SelectMany(c => c.ProductSizes)
                .Select(ps => ps.Size);

            result.Data = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Detail = product.Detail,
                Status = product.Status,
                Image = product.Image,
                Discount = product.Discount,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Price = product.Price,

            
                Color = colors.FirstOrDefault()?.Color,

                
                Sizes = SortSizes(allSizes),

                SubImages = product.ProductImages
                    .Select(i => i.ImagePath)
                    .ToList()
            };

            return result;
        }
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
        public async Task<BaseResponse<Guid>> CreateProduct(ProductSaveDto dto)
        {
            var result = new BaseResponse<Guid>();

            var productRep = _unitOfWork.GetRepositoryAsync<Product>();
            var productColorRep = _unitOfWork.GetRepositoryAsync<ProductColor>();
            var productSizeRep = _unitOfWork.GetRepositoryAsync<ProductSize>();
            var imageRep = _unitOfWork.GetRepositoryAsync<ProductImage>();

           
            var product = new Product
            {
                Name = dto.Name?.Trim(),
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                Discount = dto.Discount??0,
                Brand = dto.Brand,
                Price = dto.Price,
                Status = StatusEnum.Active,
                Image = dto.Image,
                Detail = string.Join( "\n\n", new[] { dto.DetailPart1, dto.DetailPart2 }.Where(s => !string.IsNullOrWhiteSpace(s))),

            };

            await productRep.Add(product);

        
          

            var productColor = new ProductColor
            {
                Product = product,
                Color = dto.Color
            };

            await productColorRep.Add(productColor);

            var sizes = dto.Sizes
                .Distinct()
                .Select(s => new ProductSize
                {
                    ProductColor = productColor,
                    Size = s,
                    Quantity = 0
                })
                .ToList();

            await productSizeRep.Add(sizes);

           



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
            await _unitOfWork.SaveChangesAsync();
            result.Data = product.Id;
            result.Type = GlobalConstants.ResponseType.Success;
            return result;
        }
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

            
            product.Name = dto.Name?.Trim();
            product.CategoryId = dto.CategoryId;
            product.Description = dto.Description;
            product.Discount = dto.Discount??0;
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

            //  update size

            if (productColor != null)
            {
                productColor.Color = dto.Color;
                await productColorRep.Update(productColor);
            }

            await productRep.Update(product);


            // update iamge
            var oldImages = await (await productImageRep.QueryCondition(x => x.ProductId == id)).ToListAsync();
            var keepImages = dto.OldSubImages ?? new List<string>();
            // xóa ảnh phụ
            foreach (var img in oldImages)
            {
                if (!keepImages.Contains(img.ImagePath))
                {
                    await productImageRep.Delete(img);   
                }
            }
            // thêm ảnh phụ
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

            
            await _unitOfWork.SaveChangesAsync();

            return result;
        }
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
