using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext context;

        public UsersService(AppDbContext dbcontext) 
        {
            this.context = dbcontext;
        }

        public IEnumerable<User> GetAll()
            => context.Users.AsEnumerable();

        public async Task<User> GetById(int id)
        {
            var e = await context.Users.Where(a => a.Id == id).FirstOrDefaultAsync();

            if (e == null)
                throw new Exception("User Not Found");

            return e;
        }

        public IEnumerable<User> GetAllSuppliers()
        {
            return context.Users.Where(u => u.Role == (int)UserRole.Supplier).AsEnumerable();
        }

        public async Task<User> Register(RegisterRequest request)
        {
            var existingUser = await context.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email || u.Id == request.Id);
            if (existingUser != null)
            {
                throw new Exception("A user with this email or user id already exists.");
            }

            var imagePath = await SaveImage(request.Image);

            var newUser = new User
            {
                Id = request.Id,
                DepartmentId = request.DepartmentId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                Gender = request.Gender,
                Image = imagePath,
                IsActive = true,
                Role = (int)UserRole.Customer
            };

            await context.Users.AddAsync(newUser);
            await this.Save();

            return newUser;
        }

        public async Task<string?> SaveImage(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return Path.Combine("Images", fileName);
        }

        public async Task<(User user, UserRole role)> Login(LoginRequest request)
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.Id == request.Id);

            if (user == null)
            {
                throw new AuthenticationException("Invalid user id.");
            }

            if (!user.IsActive) 
            {
                throw new AuthenticationException("Account is deactivated.");
            }

            if (user.Password != request.Password)
            {
                throw new AuthenticationException("Invalid password.");
            }

            return (user, (UserRole)user.Role);
        }

        public async Task<User> Save(User request)
        {
            var e = await context.Users.AddAsync(request);
            await this.Save();
            return e.Entity;
        }

        public async Task<User> Update(User request)
        {
            var e = context.Users.Update(request);
            await this.Save();
            return e.Entity;
        }

        public async Task<User> Delete(int id)
        {
            var e = await this.GetById(id);
            var result = context.Remove(e);
            await Save();
            return result.Entity;
        }

        async Task<int> Save()
            => await context.SaveChangesAsync();
    }
}
