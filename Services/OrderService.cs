using UNITEE_BACKEND.DatabaseContext;

namespace UNITEE_BACKEND.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext context;

        public OrderService(AppDbContext dbcontext)
        {
            context = dbcontext;
        }


    }
}
