using Azure.Core;
using ecommerce.Interfaces;
using ecommerce.Models.DbModel;
using ecommerce.Models.Dto;
using ecommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace ecommerce.Controllers
{
    [Route("/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly dbcontext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;
        public CartController(dbcontext context, ITokenService tokenService, ILogger<AccountController> logger)
        {
            this._context = context;
            this._tokenService = tokenService;
            this._logger = logger;
        }
        private async Task<bool> UserExist(string email)
        {
            return _context.Users.Any(u => u.Email== email);
        }

        // adds products to the cart
        [HttpPost("add")]
        public async Task<ActionResult<CartDto>> AddCart(CartDto cart)
        {
            int usrid = _tokenService.ValidateToken(cart.Token);
            var UsrQuery = await _context.Users.SingleOrDefaultAsync(x => x.Id== usrid);
            if (UsrQuery == null)
            {
                return NotFound("Account Unable To Be Found");
            }
            var ProdcutQuery = await _context.Products.SingleOrDefaultAsync(x => x.Id == cart.ProductId);
            if (ProdcutQuery == null)
            {
                return NotFound("Prodcut Unable To Be Found");
            }
            var Carts = await _context.Carts.Include(x => x.UserModel).Include(y => y.ProductModel).Where(x => x.UserModel.Id == UsrQuery.Id).ToListAsync();
            if(Carts.Count == 0)
            {
                _context.Add(new CartModel
                {
                    UserModel = UsrQuery,
                    ProductModel = ProdcutQuery,
                    Qnty = cart.Qnty
                });
                await _context.SaveChangesAsync();
                _logger.LogInformation($"{UsrQuery.Email} added a item to cart");
                return Ok("Sucessfully Added Item(s) To Your Cart");
            }
            foreach(var Cart in Carts)
            {
                if(Cart.ProductModel == ProdcutQuery)
                {
                    Cart.Qnty += cart.Qnty;
                }
                else
                {
                    _context.Add(new CartModel
                    {
                        UserModel = UsrQuery,
                        ProductModel = ProdcutQuery,
                        Qnty = cart.Qnty
                    });
                }
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation($"{UsrQuery.Email} added items to cart");
            return Ok("Sucessfully Added Item(s) To Your Cart");
        }

        // displays all items in the cart
        [HttpGet("mycart")]
        public async Task<ActionResult<List<CartGetDto>>> mycart(CartDto cart)
        {
            int usrid = _tokenService.ValidateToken(cart.Token);
            var UsrQuery = await _context.Users.SingleOrDefaultAsync(x => x.Id == usrid);
            if (UsrQuery == null)
            {
                return NotFound("Account Unable To Be Found");
            }
            
            var Carts = await _context.Carts.Include(x => x.UserModel).Include(y => y.ProductModel).Where(x => x.UserModel.Id == usrid).ToListAsync();
            if(Carts == null)
            {
                return NotFound("Your Cart Is Empty");
            }
            var DtoCarts = new List<CartGetDto>();
            foreach (var Cart in Carts)
            {
                DtoCarts.Add(new CartGetDto
                {
                    Id = Cart.Id,
                    ProductName = Cart.ProductModel.Name,
                    Qty = Cart.Qnty
                });
            }
            _logger.LogInformation($"{UsrQuery.Email} requested their cart");
            return DtoCarts;
        }

        [HttpPut("edit")]
        public async Task<ActionResult<CartGetDto>> EditCart(CartPutDto cartEdit)
        {
            int usrid = _tokenService.ValidateToken(cartEdit.Token);
            var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
            if (UsrQuery == null)
            {
                return NotFound("Account Not Found");
            }
            var CartQuerys = await _context.Carts.Include(x => x.ProductModel).Include(y => y.UserModel).Where(x => x.UserModel.Id == usrid).ToListAsync();
            foreach (var crt in CartQuerys)
            {
                if(crt.ProductModel.Id == cartEdit.Id)
                {
                    var CartQ = await _context.Carts.FirstOrDefaultAsync(x => x.Id == crt.Id);
                    CartQ.Qnty = cartEdit.Qty;
                    _context.SaveChangesAsync();
                    _logger.LogInformation($"{UsrQuery.Email} edited their cart");
                    return Ok("Sucessfully Edited Your Cart Item");
                }
            }
            return NotFound("Item Id you requested doesnot exist in your cart");

        }
        [HttpDelete("delete")]
        public async Task<ActionResult<CartGetDto>> DeleteCart(CartPutDto cartEdit)
        {
            int usrid = _tokenService.ValidateToken(cartEdit.Token);
            var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
            if (UsrQuery == null)
            {
                return NotFound("Account Not Found");
            }
            var CartQuerys = await _context.Carts.Include(x => x.ProductModel).Include(y => y.UserModel).Where(x => x.UserModel.Id == usrid).ToListAsync();
            foreach (var crt in CartQuerys)
            {
                if (crt.ProductModel.Id == cartEdit.Id)
                {
                    var CartQ = await _context.Carts.FirstOrDefaultAsync(x => x.Id == crt.Id);
                    _context.Carts.Remove(CartQ);
                    _context.SaveChangesAsync();
                    _logger.LogInformation($"{UsrQuery.Email} deleted their cart");
                    return Ok("Sucessfully Deleted Your Cart Item");
                }
            }
            return NotFound("Item Id you requested doesnot exist in your cart");

        }

    }
}
