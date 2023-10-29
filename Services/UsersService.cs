using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Models.Security;

namespace UNITEE_BACKEND.Services
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersService(AppDbContext dbcontext, IHttpContextAccessor httpContext) 
        {
            this.context = dbcontext;
            this._httpContextAccessor = httpContext;
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

        public IEnumerable<User> GetAllSuppliersProducts(int departmentId)
        {
            var supplierIdsWithProductsInDepartment = context.Products
                                                             .Where(p => p.DepartmentId == departmentId)
                                                             .Select(p => p.SupplierId)
                                                             .Distinct()
                                                             .ToList();

            return context.Users
                         .Where(u => u.Role == (int)UserRole.Supplier && supplierIdsWithProductsInDepartment.Contains(u.Id))
                         .AsEnumerable();
        }


        public User GetSupplierById(int id)
        {
            return context.Users.FirstOrDefault(u => u.Id == id && u.Role == (int)UserRole.Supplier);
        }

        public IEnumerable<Product> GetProductsBySupplierShop(int supplierId)
        {
            return context.Products.Where(p => p.SupplierId == supplierId).ToList();
        }


        public IEnumerable<User> GetAllCustomers()
        {
            return context.Users.Where(c => c.Role == (int)UserRole.Customer).AsEnumerable();
        }

        public async Task<User> GetCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return null;

            return await userManager.FindByIdAsync(userId);
        }

        public async Task<User> Register(RegisterRequest request)
        {
            var existingUser = await context.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email || u.Id == request.Id);

            if (existingUser != null)
                throw new Exception("A user with this email or user id already exists.");

            var imagePath = await SaveImage(request.Image);
            var studyLoadPath = await SaveStudyLoad(request.StudyLoad);
            var encryptedPassword = PasswordEncryptionService.EncryptPassword(request.Password);

            var newUser = new User
            {
                Id = request.Id,
                DepartmentId = request.DepartmentId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = encryptedPassword,
                PhoneNumber = request.PhoneNumber,
                Gender = request.Gender,
                Image = imagePath,
                StudyLoad = studyLoadPath,
                Role = (int)UserRole.Customer,
                IsActive = false
            };

            await context.Users.AddAsync(newUser);
            await this.Save();

            return newUser;
        }
           
        // Profile Picture
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

        public async Task<string?> SaveStudyLoad(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "StudyLoad");
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

            return Path.Combine("StudyLoad", fileName);
        }

        public async Task<(User user, UserRole role)> Login(LoginRequest request)
        {
            try
            {
                User user = null;

                if (request.Id.HasValue) 
                {
                    user = await context.Users
                                        .Where(u => u.Id == request.Id)
                                        .SingleOrDefaultAsync();
                }
                else if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    user = await context.Users
                                        .Where(u => u.Email == request.Email)
                                        .SingleOrDefaultAsync();
                }

                if (user == null)
                    throw new AuthenticationException("Invalid user Id or Email");

                if (!user.IsValidate)
                    throw new AuthenticationException("Waiting for validation");

                if (!user.IsActive)
                    throw new AuthenticationException("Account is deactivated");

                if (!PasswordEncryptionService.VerifyPassword(request.Password, PasswordEncryptionService.EncryptPassword(request.Password)))
                    throw new AuthenticationException("Invalid Password");

                return (user, (UserRole)user.Role);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<User> ValidateUser(int id, ValidateUserRequest request)
        {
            try
            {
                var userExist = await context.Users.Where(a => a.Id == id).FirstOrDefaultAsync();

                if (userExist == null)
                    throw new Exception("User not Found");

                if (userExist.Role != (int)UserRole.Supplier && userExist.Role != (int)UserRole.Customer)
                    throw new Exception("The provided ID does not correspond to a supplier or a customer");

                userExist.IsValidate = request.IsValidate;

                await this.Save();

                return userExist;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
