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
            context = dbcontext;
        }

        public async Task<IEnumerable<Department>> GetAll()
            => await context.Departments
                            .Include(d => d.ProductDepartments)
                            .ToListAsync();

        public async Task<Department> GetById(int id)
            => await context.Departments
                            .Where(a => a.DepartmentId == id)
                            .FirstOrDefaultAsync();
    }
}
