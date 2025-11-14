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
    using FBS.Application.DataTranferObjects.Cart;
    using FBS.Application.DataTranferObjects.Orders;
    using FBS.Application.Services.Interfaces;
    using FBS.Infrastructure.Entities;
    using FBS.Infrastructure.Repositories.Interfaces;
    using FBS.Shared.Constants;
    using FBS.Shared.DataTranferObjects.Base;
    using FBS.Shared.Enums;
    using FBS.Shared.Helpers;

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseTableResponse<OrderDto>> GetOrders(BaseSearchDto<OrderSearchDto> dto)
        {
            var result = new BaseTableResponse<OrderDto>();
            var queryOrder = await _unitOfWork.GetRepositoryReadOnlyAsync<Order>().QueryAll();
            var searchData = dto.SearchParams ?? new OrderSearchDto();

            result.Total = queryOrder.Count();

            var query = queryOrder.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerAddress = order.CustomerAddress,
                CustomerPhone = order.CustomerPhone,
                Status = order.Status,
                CustomerName = order.CustomerName,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
            });

            query = dto.ColumnSort switch
            {
                ColumnNames.CreatedAt => dto.Asc ? query.OrderBy(i => i.CreatedAt) : query.OrderByDescending(i => i.CreatedAt),
                _ => query,
            };

            var (items, totalPage) = TableResponseHelper.MakeToList(query, result.Total, dto.Start, dto.PageSize);

            result.Items = items;
            result.TotalPage = totalPage;

            return result;
        }

        public async Task<BaseResponse<OrderDto>> FindById(Guid orderId)
        {
            var result = new BaseResponse<OrderDto>();

            var queryOrder = await _unitOfWork.GetRepositoryReadOnlyAsync<Order>().QueryAll();
            queryOrder = queryOrder.Include(x => x.OrderItems);
            var order = queryOrder.FirstOrDefault(i => i.Id == orderId);

            if (order == null)
            {
                return result;
            }

            result.Data = new OrderDto
            {
                Id = order.Id,
                CustomerAddress = order.CustomerAddress,
                CustomerPhone = order.CustomerPhone,
                Status = order.Status,
                CustomerName = order.CustomerName,
                Note = order.Note,
                OrderItems = order.OrderItems.Select(x => new OrderItemDto
                {
                    Price = x.Price,
                    OrderId = x.OrderId,
                    ProductColor = x.ProductColor,
                    ProductSize = x.ProductSize,
                    Quantity = x.Quantity,
                }).ToList(),
            };

            return result;
        }

        public async Task<BaseResponse<string>> CreateOrder(CheckoutDto dto)
        {
            var result = new BaseResponse<string>();
            var orderItemRep = _unitOfWork.GetRepositoryAsync<OrderItem>();

            var newOrder = new Order
            {
                CustomerAddress = dto.Address,
                CustomerPhone = dto.PhoneNumber,
                CustomerName = dto.FullName,
                Note = dto.Note,
                Status = StatusEnum.WaitingApproval,
            };

            if (!dto.CartItems.Any()) return result;

            var orderItems = dto.CartItems.Select(x => new OrderItem
            {
                Price = x.Price,
                Order = newOrder,
                ProductId = x.ProductId,
                ProductColor = x.Color,
                ProductSize = x.Size,
                Quantity = x.Quantity,
            }).ToList();

            await orderItemRep.Add(orderItems);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<BaseResponse<string>> DeleteOrder(Guid id)
        {
            var result = new BaseResponse<string>();

            var orderRep = _unitOfWork.GetRepositoryAsync<Order>();

            var order = await orderRep.Single(x => x.Id == id);

            if (order == null)
            {
                return result;
            }

            await orderRep.Delete(order);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }
    }
}
