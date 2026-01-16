using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.Services
{
    public class ProductSizeService : IProductSizeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductSizeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task UpsertAsync(UpsertProductSizeDto dto)
        {
            
            if (string.IsNullOrWhiteSpace(dto.Size))
                throw new Exception("Size không hợp lệ");

            if (dto.Quantity < 0)
                throw new Exception("Quantity không được âm");

        
            var colorRepo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductColor>();
            var productColor = await (await colorRepo.QueryAll())
                .FirstOrDefaultAsync(x => x.Id == dto.ProductColorId);

            if (productColor == null)
                throw new Exception("ProductColor không tồn tại");

            if (productColor.ProductId != dto.ProductId)
                throw new Exception("ProductColor không thuộc Product");

           
            var sizeRepo = _unitOfWork.GetRepositoryAsync<ProductSize>();
            var sizeReadRepo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductSize>();

            var existing = await (await sizeReadRepo.QueryAll())
                .FirstOrDefaultAsync(x =>
                    x.ProductColorId == dto.ProductColorId &&
                    x.Size == dto.Size);

            if (existing == null)
            {
                await sizeRepo.Add(new ProductSize
                {
                    Id = Guid.NewGuid(),         
                    ProductColorId = dto.ProductColorId,
                    Size = dto.Size,
                    Quantity = dto.Quantity
                });
            }
            else
            {
                existing.Quantity = dto.Quantity;
                await sizeRepo.Update(existing);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<ProductSizeStockDto>> GetByProductColorAsync(Guid productColorId)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductSize>();

            var sizes = await (await repo.QueryAll())
                .Where(x => x.ProductColorId == productColorId)
                .OrderBy(x => x.Size)
                .Select(x => new ProductSizeStockDto
                {
                    Id = x.Id,
                    Size = x.Size,
                    Quantity = x.Quantity
                })
                .ToListAsync();

            return sizes;
        }
    }

}
