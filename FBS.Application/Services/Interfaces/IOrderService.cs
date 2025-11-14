namespace FBS.Application.Services.Interfaces
{
    using System.Threading.Tasks;
    using FBS.API.Responses.Base;
    using FBS.Application.DataTranferObjects.Cart;
    using FBS.Application.DataTranferObjects.Orders;
    using FBS.Shared.DataTranferObjects.Base;

    public interface IOrderService
    {

        Task<BaseTableResponse<OrderDto>> GetOrders(BaseSearchDto<OrderSearchDto> dto);

        Task<BaseResponse<OrderDto>> FindById(Guid id);

        Task<BaseResponse<string>> CreateOrder(CheckoutDto dto);

        Task<BaseResponse<string>> DeleteOrder(Guid id);
    }
}
