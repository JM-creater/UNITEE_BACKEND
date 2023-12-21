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

        public async Task<IEnumerable<Department>> GetAll()
            => await context.Departments
                            .Include(d => d.ProductDepartments)
                            .ToListAsync();

        public async Task<Department> GetById(int id)
        {
            var e = await context.Departments.Where(a => a.DepartmentId == id).FirstOrDefaultAsync();

            if (e == null)
                throw new Exception("User Not Found");

            return e;
        }
    }
}
