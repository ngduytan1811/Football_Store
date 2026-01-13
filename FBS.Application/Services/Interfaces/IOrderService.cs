using System.Threading.Tasks;
using FBS.API.Responses.Base;
using FBS.Application.DataTranferObjects.Cart;
using FBS.Application.DataTranferObjects.Orders;
using FBS.Shared.DataTranferObjects.Base;
using FBS.Shared.Enums;

namespace FBS.Application.Services.Interfaces
{
    public interface IOrderService
    {
        
        Task<BaseTableResponse<OrderDto>> GetOrders(BaseSearchDto<OrderSearchDto> dto);
        Task<BaseResponse<OrderDto>> FindById(Guid id);
        Task<BaseResponse<string>> CreateOrder(CheckoutDto dto, Guid customerId);
        Task<OrderDto> CreatePendingOrder(CheckoutDto request, Guid customerId);

        Task<BaseResponse<string>> DeleteOrder(Guid id);
        Task MarkOrderAsPaid(Guid orderId);

        Task<List<OrderDto>> GetOrdersByCustomer(Guid customerId);
        Task<OrderDto?> GetOrderDetail(Guid orderId, Guid customerId);
    
        Task<BaseResponse<string>> CancelOrder(Guid orderId, Guid customerId);
        Task<BaseResponse<string>> UpdateOrderInfo(Guid orderId,Guid customerId,UpdateOrderInfoDto dto);
        Task<BaseResponse<string>> UpdateOrderStatus(Guid orderId,StatusEnum newStatus,string note);


    }
}
