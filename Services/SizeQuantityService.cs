using UNITEE_BACKEND.DatabaseContext;

namespace UNITEE_BACKEND.Services
{
    public class SizeQuantityService : ISizeQuantityService
    {
        private readonly AppDbContext context;

        public SizeQuantityService(AppDbContext dbcontext) 
        {
            this.context = dbcontext;
        }

    }
}
