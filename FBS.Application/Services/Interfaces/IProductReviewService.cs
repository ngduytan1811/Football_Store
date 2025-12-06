using FBS.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.Services.Interfaces
{
    public interface IProductReviewService
    {
        Task AddReview(ProductReview review);
        Task<List<ProductReview>> GetReviews(Guid productId);
    }
}
