using ecommerce.Interfaces;
using ecommerce.Models;
using ecommerce.Models.DbModel;
using ecommerce.Models.Dto;
using ecommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly dbcontext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;

        public OrderController(dbcontext context, ITokenService tokenService, ILogger<AccountController> logger)
        {
            this._context = context;
            this._tokenService = tokenService;
            this._logger = logger;


        }
        [HttpPost("add")]
        public async Task<ActionResult<string>> AddToOrder(OrderDto order)
        {

            int usrid = _tokenService.ValidateToken(order.Token);
            var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
            if (UsrQuery == null)
            {
                return Forbid("Woops Account Doesnot Exists!");
            }
            var catQuery = await _context.Carts.Include(x => x.UserModel).FirstOrDefaultAsync(x => x.UserModel.Id  == usrid);
            if (catQuery == null)
            {
                return NotFound("Your Cart Is Empty ");
            }
            var catQuerys = await _context.Carts.Include(x => x.UserModel).Include(x => x.ProductModel).Where(x => x.UserModel.Id == usrid).ToListAsync();
            double total = 0.0;
            foreach (var cat in catQuerys)
            {
                var producerQuery = await _context.Users.FirstOrDefaultAsync(x => x.Id == cat.ProductModel.UserModel.Id);
                if (producerQuery != null)
                {
                    total = total +  Convert.ToDouble( cat.Qnty) * Convert.ToDouble(cat.ProductModel.PriceAmount);
                    _context.Add(new OrderModel
                    {
                        From = UsrQuery,
                        To = producerQuery,
                        ProductModel = cat.ProductModel,
                        Qnty = cat.Qnty
                    });
                    _context.Remove(cat);
                    await _context.SaveChangesAsync();
                }
            }
            _logger.LogInformation($"{UsrQuery.Email} generated an order");
            return Ok($"Sucessfully Added Order your total cost will be {total}");


        }
        [HttpGet("orders")]
        public async Task<ActionResult<List<OrderGetDto>>> ListOrders(OrderDto ordr)
        {
            int usrid = _tokenService.ValidateToken(ordr.Token);
            var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
            if (UsrQuery == null)
            {
                return Forbid("Woops Account Doesnot Exists!");
            }
            if(UsrQuery.RoleModel.Role == "Admin" | UsrQuery.RoleModel.Role == "Consumer")
            {
                var orderQuerys = await _context.Orders.Include(x => x.From).Include(y => y.To).Include(y => y.ProductModel).Where(x => x.To.Id == UsrQuery.Id).ToListAsync();
                if (orderQuerys.Count == 0) {
                    return NotFound("Your OrderList is empty");
                }
                var OrdersDto = new List<OrderGetDto>();
                foreach(var orderQuery in orderQuerys)
                {

                    OrdersDto.Add(new OrderGetDto
                    {
                        orderId = orderQuery.Id,
                        from = orderQuery.From.Username,
                        productName = orderQuery.ProductModel.Name,
                        quantity = orderQuery.Qnty
                    });

                }
                return Ok(OrdersDto);
            }
            return Forbid("Access Denied");
        }
    }
}
