using Microsoft.EntityFrameworkCore;

namespace FBS.Application.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using FBS.API.Responses.Base;
    using FBS.Application.DataTranferObjects.Cart;
    using FBS.Application.DataTranferObjects.Orders;
    using FBS.Application.Services.Interfaces;
    using FBS.Infrastructure.Entities;
    using FBS.Infrastructure.Repositories.Interfaces;
    using FBS.Shared.DataTranferObjects.Base;

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ================================
        // ADMIN - Lấy tất cả đơn hàng
        // ================================
        public async Task<BaseTableResponse<OrderDto>> GetOrders(BaseSearchDto<OrderSearchDto> dto)
        {
            var result = new BaseTableResponse<OrderDto>();
            var query = await _unitOfWork.GetRepositoryReadOnlyAsync<Order>().QueryAll();

            result.Total = query.Count();

            result.Items = query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new OrderDto
                {
                    Id = x.Id,
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,
                    CustomerAddress = x.CustomerAddress,
                    CustomerEmail = x.CustomerEmail,
                    Note = x.Note,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            result.TotalPage = 1;
            return result;
        }

        // ================================
        // ADMIN / USER - Lấy chi tiết đơn hàng theo ID
        // ================================
        public async Task<BaseResponse<OrderDto>> FindById(Guid orderId)
        {
            var result = new BaseResponse<OrderDto>();

            var query = await _unitOfWork.GetRepositoryReadOnlyAsync<Order>().QueryAll();
            query = query.Include(x => x.OrderItems).ThenInclude(x => x.Product);

            var order = query.FirstOrDefault(x => x.Id == orderId);
            if (order == null) return result;

            result.Data = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CustomerEmail = order.CustomerEmail,
                CustomerAddress = order.CustomerAddress,
                Note = order.Note,

                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    ProductColor = i.ProductColor,
                    ProductSize = i.ProductSize,
                    Quantity = i.Quantity,
                   
                    Price = i.Price
                }).ToList()
            };

            return result;
        }

        // ================================
        // USER - Tạo đơn hàng mới
        // ================================
        public async Task<BaseResponse<string>> CreateOrder(CheckoutDto dto)
        {
            var result = new BaseResponse<string>();
            var orderItemRepo = _unitOfWork.GetRepositoryAsync<OrderItem>();

            var newOrder = new Order
            {
                CustomerId = dto.CustomerId,         // 🔥 BẮT BUỘC
                CustomerName = dto.FullName,
                CustomerPhone = dto.PhoneNumber,
                CustomerEmail = dto.Email,
                CustomerAddress = dto.Address,
                Note = dto.Note,
                CreatedAt = DateTime.Now
            };

            var items = dto.CartItems.Select(x => new OrderItem
            {
                Order = newOrder,
                ProductId = x.ProductId,
                ProductColor = x.Color,
                ProductSize = x.Size,
                Quantity = x.Quantity,
                Price = x.Price
            }).ToList();

            await orderItemRepo.Add(items);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }


        // ================================
        // USER - Lấy tất cả đơn hàng theo CustomerId
        // ================================
        public async Task<List<OrderDto>> GetOrdersByCustomer(Guid customerId)
        {
            var query = await _unitOfWork
                .GetRepositoryReadOnlyAsync<Order>()
                .QueryAll();

            var orders = query
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .ToList();

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CustomerEmail = order.CustomerEmail,
                CustomerAddress = order.CustomerAddress,
                Note = order.Note,
                CreatedAt = order.CreatedAt,

                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    ProductImage = i.Product?.Image ,
                    ProductColor = i.ProductColor,
                    ProductSize = i.ProductSize,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()

            }).ToList();
        }


        // ================================
        // USER - Lấy chi tiết đơn hàng & kiểm tra quyền sở hữu
        // ================================
        public async Task<OrderDto?> GetOrderDetail(Guid orderId, Guid customerId)
        {
            var repo = await _unitOfWork.GetRepositoryReadOnlyAsync<Order>().QueryAll();

            var order = repo
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == orderId && o.CustomerId == customerId);

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CustomerEmail = order.CustomerEmail,
                CustomerAddress = order.CustomerAddress,
                Note = order.Note,
                CreatedAt = order.CreatedAt,

                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    ProductColor = i.ProductColor,
                    ProductSize = i.ProductSize
                }).ToList()
            };
        }

        // ================================
        // ADMIN - Xóa đơn
        // ================================
        public async Task<BaseResponse<string>> DeleteOrder(Guid id)
        {
            var result = new BaseResponse<string>();
            var orderRepo = _unitOfWork.GetRepositoryAsync<Order>();

            var order = await orderRepo.Single(x => x.Id == id);
            if (order == null) return result;

            await orderRepo.Delete(order);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
