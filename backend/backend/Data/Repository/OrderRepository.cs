using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace backend.Data.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

        public OrderRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders()
        {


            var orders = await context.OrderTable.ToListAsync();
            foreach (var order in orders)
            {
                var user = context.UserTable.FirstOrDefault(f => f.UserId == order.UserId);
                if (user != null)
                {
                    order.User = user;
                }
                order.ProductsInOrder = new List<ProductsInOrderDto>();
            }
            var productOrders = await context.ProductOrderTable.ToListAsync();
            foreach (var productOrder in productOrders)
            {
                foreach (var order in orders)
                {
                    if (productOrder.OrderId == order.OrderId)
                    {

                        ProductsInOrderDto productInOrder = new ProductsInOrderDto();
                        productInOrder.ProductId = productOrder.ProductId;
                        productInOrder.Amount = productOrder.Amount;
                        /*var product = context.ProductTable.FirstOrDefault(f => f.ProductId == productOrder.ProductId);
                         if (product != null)
                         {
                             order.Products.Add(mapper.Map<ProductDto>(product));
                         }
                        */
                        order.ProductsInOrder.Add(productInOrder);
                    }

                }
            }

            return mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderById(Guid orderId)
        {
            var order = await context.OrderTable.FindAsync(orderId);
            if (order != null)
            {
                var user = context.UserTable.FirstOrDefault(f => f.UserId == order.UserId);
                if (user != null)
                {
                    order.User = user;
                }
                order.ProductsInOrder = new List<ProductsInOrderDto>();

                var productOrders = await context.ProductOrderTable.ToListAsync();
                foreach (var productOrder in productOrders)
                {
                    if (productOrder.OrderId == order.OrderId)
                    {
                        ProductsInOrderDto productInOrder = new ProductsInOrderDto();
                        productInOrder.ProductId = productOrder.ProductId;
                        productInOrder.Amount = productOrder.Amount;
                        /*var product = context.ProductTable.FirstOrDefault(f => f.ProductId == productOrder.ProductId);
                        if (product != null)
                        {
                            order.Products.Add(mapper.Map<ProductDto>(product));
                        }*/
                        order.ProductsInOrder.Add(productInOrder);
                    }

                }
            }
            return mapper.Map<OrderDto>(order);
        }

        public async Task<Guid> CreateOrder(OrderCreateDto orderDto, Guid userId)
        {
            var order = mapper.Map<Order>(orderDto);
            order.OrderId = Guid.NewGuid();
            order.UserId = userId;
            order.OrderStatus = "Pending";
            order.OrderDate = DateTime.Now;
            order.NumberOfItems = 0;
            order.TotalPrice = 0;
            var today = DateTime.Today;
            var user = context.UserTable.FirstOrDefault(f => f.UserId == order.UserId);
            var userAge = today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth.Date > today.AddYears(-userAge))
            {
                userAge--;
            }
            foreach (var product in order.ProductsInOrder)
            {

                /*ProductOrderRepository repository = new ProductOrderRepository(this.context,this.mapper);
                ProductOrderCreateDto productOrderCreateDto = new ProductOrderCreateDto();
                productOrderCreateDto.OrderId = order.OrderId;
                productOrderCreateDto.ProductId = product.ProductId;
                productOrderCreateDto.Amount = product.Amount;
                repository.CreateProductOrder(productOrderCreateDto);*/
                ProductOrder productOrder = new ProductOrder();
                productOrder.OrderId = order.OrderId;
                productOrder.ProductId = product.ProductId;
                productOrder.Amount = product.Amount;
                order.NumberOfItems = order.NumberOfItems + product.Amount;
                context.ProductOrderTable.Add(productOrder);
                var specificProduct = context.ProductTable.FirstOrDefault(f => f.ProductId == product.ProductId);
                var discount = context.DiscountTable.FirstOrDefault(f => f.DiscountId == specificProduct.DiscountId);
                specificProduct.Quantity = specificProduct.Quantity - product.Amount;
                if (discount == null)
                {
                    order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount;
                }
                else
                {

                    var discountAmount = double.Parse(discount.Percentage.TrimEnd('%')) / 100;
                    if (discount.DiscountType == "13-18" && userAge >= 13 && userAge < 18)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "18-30" && userAge >= 18 && userAge < 30)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "30-45" && userAge >= 30 && userAge < 45)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "45-60" && userAge >= 45 && userAge < 60)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "60+" && userAge >= 60)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "All")
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount;
                    }
                }
            }

            context.OrderTable.Add(order);
            await context.SaveChangesAsync();
            return order.OrderId;
        }

        public async Task UpdateOrder(Guid orderId, OrderUpdateDto orderDto)
        {
            var order = await context.OrderTable.FindAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            var productOrders = await context.ProductOrderTable.ToListAsync();
            foreach (var productOrder in productOrders)
            {
                if (productOrder.OrderId == orderId)
                {
                    var specificProduct = context.ProductTable.FirstOrDefault(f => f.ProductId == productOrder.ProductId);
                    if (specificProduct != null)
                    {
                        specificProduct.Quantity = specificProduct.Quantity + productOrder.Amount;
                    }
                    context.ProductOrderTable.Remove(productOrder);
                }
            }

            mapper.Map(orderDto, order);


            order.NumberOfItems = 0;
            order.TotalPrice = 0;
            var today = DateTime.Today;
            var user = context.UserTable.FirstOrDefault(f => f.UserId == order.UserId);
            var userAge = today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth.Date > today.AddYears(-userAge))
            {
                userAge--;
            }
            foreach (var product in order.ProductsInOrder)
            {

                /*ProductOrderRepository repository = new ProductOrderRepository(this.context,this.mapper);
                ProductOrderCreateDto productOrderCreateDto = new ProductOrderCreateDto();
                productOrderCreateDto.OrderId = order.OrderId;
                productOrderCreateDto.ProductId = product.ProductId;
                productOrderCreateDto.Amount = product.Amount;
                repository.CreateProductOrder(productOrderCreateDto);*/
                ProductOrder productOrder = new ProductOrder();
                productOrder.OrderId = order.OrderId;
                productOrder.ProductId = product.ProductId;
                productOrder.Amount = product.Amount;
                order.NumberOfItems = order.NumberOfItems + product.Amount;
                context.ProductOrderTable.Add(productOrder);
                var specificProduct = context.ProductTable.FirstOrDefault(f => f.ProductId == product.ProductId);
                var discount = context.DiscountTable.FirstOrDefault(f => f.DiscountId == specificProduct.DiscountId);
                specificProduct.Quantity = specificProduct.Quantity - product.Amount;
                if (discount == null)
                {
                    order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount;
                }
                else
                {

                    var discountAmount = double.Parse(discount.Percentage.TrimEnd('%')) / 100;
                    if (discount.DiscountType == "13-18" && userAge >= 13 && userAge < 18)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "18-30" && userAge >= 18 && userAge < 30)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "30-45" && userAge >= 30 && userAge < 45)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "45-60" && userAge >= 45 && userAge < 60)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "60+" && userAge >= 60)
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else if (discount.DiscountType == "All")
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount * (1 - discountAmount);
                    }
                    else
                    {
                        order.TotalPrice = order.TotalPrice + specificProduct.Price * product.Amount;
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteOrder(Guid orderId)
        {
            var order = await context.OrderTable.FindAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }
            var productOrders = await context.ProductOrderTable.ToListAsync();
            foreach (var productOrder in productOrders)
            {
                if (productOrder.OrderId == orderId)
                {
                    var specificProduct = context.ProductTable.FirstOrDefault(f => f.ProductId == productOrder.ProductId);
                    if (specificProduct != null)
                    {
                        specificProduct.Quantity = specificProduct.Quantity + productOrder.Amount;
                    }
                    context.ProductOrderTable.Remove(productOrder);
                }
            }

            context.OrderTable.Remove(order);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserId(Guid userId)
        {

            var orders = await context.OrderTable.ToListAsync();
            foreach (var order in orders)
            {
                var user = context.UserTable.FirstOrDefault(f => f.UserId == order.UserId);
                if (user != null)
                {
                    order.User = user;
                }
                order.ProductsInOrder = new List<ProductsInOrderDto>();
            }
            var productOrders = await context.ProductOrderTable.ToListAsync();
            foreach (var productOrder in productOrders)
            {
                foreach (var order in orders)
                {
                    if (productOrder.OrderId == order.OrderId)
                    {

                        ProductsInOrderDto productInOrder = new ProductsInOrderDto();
                        productInOrder.ProductId = productOrder.ProductId;
                        productInOrder.Amount = productOrder.Amount;
                        
                        order.ProductsInOrder.Add(productInOrder);
                    }

                }
            }
            List<Order> returnOrders = new List<Order>();
            foreach (var order in orders)
            {
                if(order.UserId == userId)
                {
                    returnOrders.Add(order);
                }
            }
            return mapper.Map<IEnumerable<OrderDto>>(returnOrders);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = context.OrderTable.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "numberofitems":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(o => o.NumberOfItems) : query.OrderByDescending(o => o.NumberOfItems);
                        break;
                    case "totalprice":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(o => o.TotalPrice) : query.OrderByDescending(o => o.TotalPrice);
                        break;
                    case "orderstatus":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(o => o.OrderStatus) : query.OrderByDescending(o => o.OrderStatus);
                        break;
                    case "orderdate":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate);
                        break;
                    case "userid":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(o => o.UserId) : query.OrderByDescending(o => o.UserId);
                        break;
                    default:
                        break;
                }
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var orders = await query.ToListAsync();
            return mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }

}