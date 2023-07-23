using Azure.Core;
using ecommerce.Interfaces;
using ecommerce.Models.DbModel;
using ecommerce.Models.Dto;
using ecommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly dbcontext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;

        public CategoryController(dbcontext context, ITokenService tokenService, ILogger<AccountController> logger)
        {
            this._context = context;
            this._tokenService = tokenService;
            this._logger = logger;

        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryModel>>> GetCategorys()
        {
            var CatList = new List<CategoryModel>();
            CatList = await _context.Categories.ToListAsync();
            return Ok(CatList);
        }

        [HttpPost]
        public async Task<ActionResult<List<CategoryModel>>> AddCategory(CategoryAddDto category)
        {
            int usrid = _tokenService.ValidateToken(category.Token);
            var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
            if (UsrQuery == null)
            {
                return Forbid("Woops Account Doesnot Exists!");
            }
            if (UsrQuery.RoleModel.Role == "Admin")
            {
                _context.Add(new CategoryModel
                {
                    Name = category.Name,
                });
                await _context.SaveChangesAsync();
                _logger.LogWarning($"{UsrQuery.Email} added a category");
                return Ok("Sucessfully added new category");
            }
            return Forbid("Only Admin Can Add And Edit Category");
        }

        [HttpPut]
        public async Task<ActionResult<List<CategoryModel>>> EditCategory(CategoryEditDto category)
        {
            int usrid = _tokenService.ValidateToken(category.Token);
            var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
            if (UsrQuery.RoleModel.Role == "Admin")
            {
                var catquery = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                if (catquery == null)
                    return NotFound("Category Not Found!");
                catquery.Name = category.Name;
                await _context.SaveChangesAsync();
                _logger.LogWarning($"{UsrQuery.Email} edited category");
                return Ok("Sucessfully added new category");
            }
            _logger.LogWarning($"{UsrQuery.Email} tried to edit category");
            return Forbid("Only Admin Can Add And Edit Category");
        }

        [HttpDelete]
        public async Task<ActionResult<List<CategoryModel>>> DeleteCategory(CategoryDeleteDto category)
        {
            int usrid = _tokenService.ValidateToken(category.Token);
            var UsrQuery = await _context.Users.Include(x => x.RoleModel).SingleOrDefaultAsync(x => x.Id == usrid);
            if (UsrQuery.RoleModel.Role == "Admin")
            {
                var catquery = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                if (catquery == null)
                    return NotFound("Category Not Found!");
                _context.Categories.Remove(catquery);
                await _context.SaveChangesAsync();
                _logger.LogWarning($"{UsrQuery.Email} deleted category");
                return Ok("Sucessfully removed category");
            }
            _logger.LogWarning($"{UsrQuery.Email} tried to edit category");
            return Forbid("Only Admin Can Add And Edit Category");
        }


    }
}
