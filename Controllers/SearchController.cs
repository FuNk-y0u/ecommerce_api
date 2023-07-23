using ecommerce.Interfaces;
using ecommerce.Models;
using ecommerce.Models.Dto;
using ecommerce.Repository;
using ecommerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly dbcontext _context;
        public SearchController(dbcontext context)
        {
            this._context = context;
        }
        [HttpPost]
        public async Task<ActionResult<List<SearchResDto>>> searchProducts(SearchDto searchData)
        {
            var productQuery = await _context.Products.Include(x => x.CategoryModel).Include(x => x.UserModel).Where(x => x.Name.ToUpper().Contains(searchData.Text.ToUpper())).ToListAsync();

            switch (searchData.sort)
            {
                case "name":
                    productQuery = productQuery.OrderBy(s => s.Name).ToList();
                    break;
                case "date":
                    productQuery = productQuery.OrderBy(s => s.DateAdded).ToList();
                    break;
                case "price":
                    productQuery = productQuery.OrderBy(s => s.PriceAmount).ToList();
                    break;
                default:
                    NotFound("Your Order Method Doesnot Exists!");
                    break;

            }


            PaginateService pg = new PaginateService();
            var pages = pg.GetPage(productQuery, searchData.page, searchData.pagesize);
            var resProduct = new List<SearchResDto>();
            foreach(var page in pages) {
                resProduct.Add(
                    new SearchResDto
                    {
                        id = page.Id,
                        name = page.Name,
                        category = page.CategoryModel.Name,
                        price = page.PriceAmount.ToString(),

                    });
            }
            return resProduct;
            
            
        }
    }
}
