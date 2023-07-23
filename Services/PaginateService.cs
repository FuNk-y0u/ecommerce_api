using ecommerce.Interfaces;
using ecommerce.Models.DbModel;
using System.Collections.Generic;
using System.Linq;

namespace ecommerce.Services
{
    public class PaginateService
    {
        public IList<ProductModel> GetPage(IList<ProductModel> products, int page, int pagesize)
        {
            return products.Skip(page * pagesize).Take(pagesize).ToList();
        }
    }
}
