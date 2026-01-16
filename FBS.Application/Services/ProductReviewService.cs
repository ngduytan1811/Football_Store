using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddReview(ProductReview review)
        {
            var repo = _unitOfWork.GetRepositoryAsync<ProductReview>();
            await repo.Add(review);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<ProductReview>> GetReviews(Guid productId)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductReview>();
            var query = await repo.QueryAll();

            return query
                .Where(x => x.ProductId == productId)
               
                .ToList();
        }
    }

}
