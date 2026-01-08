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
    using FBS.Shared.Constants;
    using FBS.Shared.DataTranferObjects.Base;
    using FBS.Shared.Enums;

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
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

       
        public async Task<BaseResponse<string>> CreateOrder(CheckoutDto dto)
        {
            var result = new BaseResponse<string>();
            var orderItemRepo = _unitOfWork.GetRepositoryAsync<OrderItem>();
            var orderRepo = _unitOfWork.GetRepositoryAsync<Order>();

            var paymentStatus = dto.PaymentMethod == "VietQR"
                          ? PaymentStatusEnum.Paid
                          : PaymentStatusEnum.Unpaid;

            var status = dto.PaymentMethod == "VietQR"
                ? StatusEnum.Active  
                : StatusEnum.Inactive;


            var newOrder = new Order
            {
                CustomerId = dto.CustomerId,        
                CustomerName = dto.FullName,
                CustomerPhone = dto.PhoneNumber,
                CustomerEmail = dto.Email,
                CustomerAddress = dto.Address,
                Note = dto.Note,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = paymentStatus,
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
            await orderRepo.Add(newOrder);
            await _unitOfWork.SaveChangesAsync();

            result.Data = newOrder.Id.ToString();
            result.Type = GlobalConstants.ResponseType.Success;

            result.Message = "Đặt hàng thành công!";

            return result;
        }

        public async Task<OrderDto> CreatePendingOrder(CheckoutDto request)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,

                CustomerName = request.FullName,
                CustomerPhone = request.PhoneNumber,
                CustomerEmail = request.Email,
                CustomerAddress = request.Address,

                Note = request.Note,

                PaymentMethod = "COD",
                PaymentStatus = PaymentStatusEnum.Unpaid,
                Status = StatusEnum.Active
            };

            var orderRepository = _unitOfWork.GetRepositoryAsync<Order>();

            await orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return new OrderDto
            {
                Id = order.Id
            };
        }

        public async Task MarkOrderAsPaid(Guid orderId)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Order>();

            var order = await repo.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order không tồn tại");

            order.PaymentStatus = PaymentStatusEnum.Paid;

            await _unitOfWork.SaveChangesAsync();
        }



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
                Status = order.Status,
                

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


        
        public async Task<OrderDto?> GetOrderDetail(Guid orderId, Guid customerId)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await repo.QueryAll();

            var order = await query
                .AsNoTracking() 
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId && o.CustomerId == customerId);

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
                Status = order.Status,

                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    ProductColor = i.ProductColor,
                    ProductSize = i.ProductSize,
                    ProductImage = i.Product.Image
                }).ToList()
            };
        }

        
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
        public async Task<BaseResponse<string>> CancelOrder(Guid orderId, Guid customerId)
        {
          
            var orderReadRepo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await orderReadRepo.QueryAll();

            var order = await query.FirstOrDefaultAsync(o =>
                o.Id == orderId && o.CustomerId == customerId);

            if (order == null)
            {
                return new BaseResponse<string>
                {
                    Type = GlobalConstants.ResponseType.Error,
                    Message = "Không tìm thấy đơn hàng"
                };
            }

           
            if (order.Status == StatusEnum.InHandler ||
                order.Status == StatusEnum.WaitingApproval ||
                order.Status == StatusEnum.Cancel)
            {
                return new BaseResponse<string>
                {
                    Type = GlobalConstants.ResponseType.Error,
                    Message = "Không thể hủy đơn hàng ở trạng thái này"
                };
            }

            
            var orderWriteRepo = _unitOfWork.GetRepositoryAsync<Order>();

            order.Status = StatusEnum.Cancel;
            order.UpdatedAt = DateTime.Now;

            orderWriteRepo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<string>
            {
                Type = GlobalConstants.ResponseType.Success,
                Message = "Hủy đơn hàng thành công"
            };
        }

        public async Task<BaseResponse<string>> UpdateOrderInfo(Guid orderId,Guid customerId,UpdateOrderInfoDto dto)
        {
            var readRepo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await readRepo.QueryAll();

            var order = await query.FirstOrDefaultAsync(o =>
                o.Id == orderId && o.CustomerId == customerId);

            if (order == null)
            {
                return new BaseResponse<string>
                {
                    Type = GlobalConstants.ResponseType.Error,
                    Message = "Không tìm thấy đơn hàng"
                };
            }

            
            if (order.Status == StatusEnum.InHandler ||
                order.Status == StatusEnum.WaitingApproval ||
                order.Status == StatusEnum.Cancel)
            {
                return new BaseResponse<string>
                {
                    Type = GlobalConstants.ResponseType.Error,
                    Message = "Không thể chỉnh sửa đơn hàng ở trạng thái này"
                };
            }

          
            order.CustomerName = dto.CustomerName;
            order.CustomerPhone = dto.CustomerPhone;
            order.CustomerAddress = dto.CustomerAddress;
            order.Note = dto.Note;
            order.UpdatedAt = DateTime.Now;

            var writeRepo = _unitOfWork.GetRepositoryAsync<Order>();
            writeRepo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<string>
            {
                Type = GlobalConstants.ResponseType.Success,
                Message = "Cập nhật thông tin đơn hàng thành công"
            };
        }


    }
}
