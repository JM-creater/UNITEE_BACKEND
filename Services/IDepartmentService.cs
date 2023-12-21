using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Services
{
    public interface IDepartmentService
    {
        public Task<IEnumerable<Department>> GetAll();
        public Task<Department> GetById(int id);

    }
}
