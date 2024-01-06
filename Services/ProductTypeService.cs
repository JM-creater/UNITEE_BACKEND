using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly AppDbContext context;

        public ProductTypeService(AppDbContext dbcontext)
        {
            context = dbcontext;
        }

        public async Task<IEnumerable<ProductType>> GetAll()
            => await context.ProductTypes.ToListAsync();
    }
}
