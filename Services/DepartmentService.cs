using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext context;

        public DepartmentService(AppDbContext dbcontext) 
        {
            this.context = dbcontext;
        }

        public IEnumerable<Department> GetAll()
            => context.Departments.AsEnumerable();

        public async Task<Department> GetById(int id)
        {
            var e = await context.Departments.Where(a => a.DepartmentId == id).FirstOrDefaultAsync();

            if (e == null)
                throw new Exception("User Not Found");

            return e;
        }
    }
}
