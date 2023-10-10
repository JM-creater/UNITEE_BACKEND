using Microsoft.EntityFrameworkCore;
using System.Net;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext context;
        private readonly IUsersService usersService;

        public SupplierService(AppDbContext dbcontext, IUsersService usersService) 
        {
            this.context = dbcontext;
            this.usersService = usersService;
        }

        public async Task<User> GetSupplierById(int id)
        {
            var supplier = await context.Users.FindAsync(id);

            if (supplier == null || supplier.Role != (int)UserRole.Supplier)
            {
                throw new Exception("Supplier not found");
            }

            return supplier;
        }

        public async Task<User> AddSupplier(SupplierRequest request)
        {
            try
            {
                var existingUser = await context.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email || u.Id == request.Id);
                if (existingUser != null)
                {
                    throw new Exception("A supplier with this email or user id already exists.");
                }

                var imagePath = await SaveImage(request.Image);
                var imageBir = await SaveBIR(request.BIR);
                var imageCityPermit = await SaveCityPermit(request.CityPermit);
                var imageSchoolPermit = await SaveSchoolPermit(request.SchoolPermit);

                var newSupplier = new User
                {
                    Id = request.Id,
                    Email = request.Email,
                    Password = request.Password,
                    PhoneNumber = request.PhoneNumber,
                    ShopName = request.ShopName,
                    Address = request.Address,
                    Image = imagePath,
                    BIR = imageBir,
                    CityPermit = imageCityPermit,
                    SchoolPermit = imageSchoolPermit,
                    Role = (int)UserRole.Supplier
                };

                await context.Users.AddAsync(newSupplier);
                await context.SaveChangesAsync();

                return newSupplier;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        // Profile Picture
        public async Task<string?> SaveImage(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "SupplierImage");
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

            return Path.Combine("SupplierImage", fileName);
        }

        // BIR
        public async Task<string?> SaveBIR(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "BIR");
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

            return Path.Combine("BIR", fileName);
        }

        // City Permit
        public async Task<string?> SaveCityPermit(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "CityPermit");
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

            return Path.Combine("CityPermit", fileName);
        }

        // School Permit
        public async Task<string?> SaveSchoolPermit(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "SchoolPermit");
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

            return Path.Combine("SchoolPermit", fileName);
        }

        public async Task<User> UpdateSupplier(int id, SupplierRequest request)
        {
            var existingSupplier = await context.Users.FirstOrDefaultAsync(a => a.Id == id);

            if (existingSupplier == null)
            {
                throw new Exception("Supplier Not Found");
            }

            if (existingSupplier.Role != (int)UserRole.Supplier)
            {
                throw new Exception("The provided ID does not correspond to a supplier");
            }

            if (request.Image != null)
            {
                existingSupplier.Image = await SaveImage(request.Image);
            }

            existingSupplier.Email = request.Email;
            existingSupplier.Password = request.Password;
            existingSupplier.PhoneNumber = request.PhoneNumber;
            existingSupplier.ShopName = request.ShopName;
            existingSupplier.Address = request.Address;

            await this.Save();

            return existingSupplier;
        }

        public async Task<User> ActivateSupplier(int id, bool isActive)
        {
            var existingSupplier = await context.Users.FindAsync(id);

            if (existingSupplier == null)
            {
                throw new Exception("Supplier not found");
            }

            if (existingSupplier.Role != (int)UserRole.Supplier)
            {
                throw new Exception("The provided ID does not correspond to a supplier");
            }

            existingSupplier.IsActive = isActive;
            await Save();

            return existingSupplier;
        }

        public async Task<User> Save(User request)
        {
            var e = await context.Users.AddAsync(request);
            await this.Save();
            return e.Entity;
        }

        async Task<int> Save()
            => await context.SaveChangesAsync();
    }
}
