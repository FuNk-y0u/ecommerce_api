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
    [Route("/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly dbcontext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;
        public ProductController(dbcontext context, ITokenService tokenService, ILogger<AccountController> logger)
        {
            this._context = context;
            this._tokenService = tokenService;
            this._logger = logger;

        }

        
        // returns all the products in database
        [HttpGet]
        public async Task<ActionResult<List<ProductGetDto>>> GetProducts(ProductDto request)
        {
            int usrid = _tokenService.ValidateToken(request.Token);
            if(usrid != -1) {
                var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
                if(UsrQuery.RoleModel.Role == "Admin")
                {
                    var products = _context.Products;
                    var ProductsDtoList = new List<ProductGetDto>();
                    foreach(var product in products)
                    {
                        ProductsDtoList.Add(new ProductGetDto
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            PriceAmmount = product.PriceAmount,
                            Size = product.Size,
                            Warrenty = product.Warrenty,
                            Brand = product.Brand,
                            DateAdded = product.DateAdded,
                            UsrId = product.UserModel.Id,
                            Username = product.UserModel.Username,
                            Email = product.UserModel.Email
                        });
                    }
                    return ProductsDtoList;
                }
                _logger.LogCritical($"{UsrQuery.Email} a non admin tried to access all products");
                return Unauthorized("Access denied");
            }
            return Forbid("Invalid Token");
        }

        // adds product to the database
        [HttpPost]
        public async Task<ActionResult<ProductDto>> AddProdcut(ProductDto product)
        {
            int usrid = _tokenService.ValidateToken(product.Token);
            if(usrid != -1)
            {
                var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
                var CategoryQuery = await _context.Categories.SingleOrDefaultAsync(x => x.Id == product.CategoryId);
                if (CategoryQuery == null)
                {
                    return NotFound("Category Couldnt Be Found");
                }
                if (UsrQuery.RoleModel.Role == "Admin" || UsrQuery.RoleModel.Role == "Producer")
                {
                    var dbprodcut = new ProductModel
                    {
                        Name = product.Name,
                        Description = product.Description,
                        PriceAmount = product.PriceAmount,
                        Size = product.Size,
                        Warrenty = product.Warrenty,
                        Brand = product.Brand,
                        DateAdded = DateTime.Now,
                        UserModel = UsrQuery,
                        CategoryModel = CategoryQuery
                    };
                    _context.Products.Add(dbprodcut);
                    await _context.SaveChangesAsync();
                    _logger.LogCritical($"{UsrQuery.Email} added a product");

                    return new ProductDto
                    {
                        Name = product.Name,
                        Description = product.Description,
                        PriceAmount = product.PriceAmount,
                        Size = product.Size,
                        Warrenty = product.Warrenty,
                        Brand = product.Brand,
                        Category = CategoryQuery.Name
                    };


                }
            }
            return Forbid("Invalid Token");
        }

        // Edits an existing product
        [HttpPut]
        public async Task<ActionResult<ProductDto>> EditProduct(ProductDto product)
        {
            int usrid = _tokenService.ValidateToken(product.Token);
            if(usrid != -1) {
                var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
                switch (UsrQuery.RoleModel.Role)
                {
                    case "Admin":
                        var ProductQuery = await _context.Products.SingleOrDefaultAsync(x => x.Id == product.Id);
                        ProductQuery.Name = product.Name;
                        ProductQuery.Description = product.Description;
                        ProductQuery.PriceAmount = product.PriceAmount;
                        ProductQuery.Size = product.Size;
                        ProductQuery.Brand = product.Brand;
                        var catquery = await _context.Categories.FirstOrDefaultAsync(x => x.Id == product.CategoryId);
                        if (catquery == null)
                            return NotFound("Category Specified Doesnot Exists");
                        ProductQuery.CategoryModel = catquery;
                        await _context.SaveChangesAsync();
                        return new ProductDto
                        {
                            Name = product.Name,
                            Description = product.Description,
                            PriceAmount = product.PriceAmount,
                            Size = product.Size,
                            Warrenty = product.Warrenty,
                            Brand = product.Brand,
                            Category = catquery.Name
                        };
                    case "Producer":
                        var ProductQuery1 = await _context.Products.Include(x => x.UserModel).Include(x=>x.CategoryModel).SingleOrDefaultAsync(x => x.Id == product.Id);
                        if(ProductQuery1.UserModel.Id != UsrQuery.Id)
                        {
                            return Unauthorized("Access denied");
                        }

                        ProductQuery1.Name = product.Name;
                        ProductQuery1.Description = product.Description;
                        ProductQuery1.PriceAmount = product.PriceAmount;
                        ProductQuery1.Size = product.Size;
                        ProductQuery1.Brand = product.Brand;
                        var catquery1 = await _context.Categories.FirstOrDefaultAsync(x => x.Id == product.CategoryId);
                        if(catquery1 == null)
                            return NotFound("Category Specified Doesnot Exists");
                        ProductQuery1.CategoryModel = catquery1;
                        await _context.SaveChangesAsync();
                        _logger.LogCritical($"{UsrQuery.Email} added a product");
                        return new ProductDto
                        {
                            Name = product.Name,
                            Description = product.Description,
                            PriceAmount = product.PriceAmount,
                            Size = product.Size,
                            Warrenty = product.Warrenty,
                            Brand = product.Brand,
                            Category = catquery1.Name
                        };
                    case "Consumer":
                        return Unauthorized("Access denied");

                }
            }
            return Forbid("Invalid token");
        }

        // deletes existing product
        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(ProductDto product)
        {
            int usrid = _tokenService.ValidateToken(product.Token);
            if (usrid != -1)
            {
                var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
                switch (UsrQuery.RoleModel.Role)
                {
                    case "Admin":
                        var ProductQuery = await _context.Products.SingleOrDefaultAsync(x => x.Id == product.Id);
                        _context.Products.Remove(ProductQuery);
                        await _context.SaveChangesAsync();
                        _logger.LogCritical($"{UsrQuery.Email} deleted a product");
                        return Ok("Sucess");
                    case "Producer":
                        var ProductQuery1 = await _context.Products.Include(x => x.UserModel).SingleOrDefaultAsync(x => x.Id == product.Id);
                        if (ProductQuery1.UserModel.Id != UsrQuery.Id)
                        {
                            return Unauthorized("Access denied");
                        }
                        _context.Products.Remove(ProductQuery1);
                        await _context.SaveChangesAsync();
                        _logger.LogCritical($"{UsrQuery.Email} deleted a prodcut");
                        return Ok("Sucess");
                    case "Consumer":
                        return Unauthorized("Access denied");

                }
            }
            return Forbid("Invalid Token");
        }
    }
}
