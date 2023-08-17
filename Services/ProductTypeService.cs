using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly AppDbContext context;

        public ProductTypeService(AppDbContext dbcontext)
        {
            this.context = dbcontext;
        }

        public IEnumerable<ProductType> GetAll()
            => context.ProductTypes.AsEnumerable();
    }
}
