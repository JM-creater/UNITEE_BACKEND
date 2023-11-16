using Microsoft.EntityFrameworkCore;
using System.Net;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Models.Security;

namespace UNITEE_BACKEND.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext context;
        private readonly IUsersService usersService;

        public SupplierService(AppDbContext dbcontext, IUsersService _usersService) 
        {
            context = dbcontext;
            usersService = _usersService;
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

        public async Task<User> RegisterSupplier(SupplierRequest request)
        {
            try
            {
                var existingUserId = await context.Users
                                                  .Where(u => u.Id == request.Id)
                                                  .FirstOrDefaultAsync();

                if (existingUserId != null)
                {
                    throw new InvalidOperationException("A supplier with ID already exists.");
                }

                var existingUserEmail = await context.Users
                                                     .Where(u => u.Email == request.Email)
                                                     .FirstOrDefaultAsync();

                if (existingUserEmail != null)
                {
                    throw new InvalidOperationException("A supplier with email already exists.");
                }

                var existingUserShopName = await context.Users
                                                        .Where(u => u.ShopName == request.ShopName) 
                                                        .FirstOrDefaultAsync();

                if (existingUserShopName != null) 
                {
                    throw new InvalidOperationException("A supplier with shop name already exists.");
                }

                var existingUserAddress = await context.Users
                                                       .Where(u => u.Address == request.Address)
                                                       .FirstOrDefaultAsync();

                if (existingUserAddress != null) 
                {
                    throw new InvalidOperationException("A supplier with address already exists.");
                }

                var imagePath = await SaveImage(request.Image);
                var imageBir = await SaveBIR(request.BIR);
                var imageCityPermit = await SaveCityPermit(request.CityPermit);
                var imageSchoolPermit = await SaveSchoolPermit(request.SchoolPermit);
                var encryptedPassword = PasswordEncryptionService.EncryptPassword(request.Password);

                var newSupplier = new User
                {
                    Id = request.Id,
                    Email = request.Email,
                    Password = encryptedPassword,
                    PhoneNumber = request.PhoneNumber,
                    ShopName = request.ShopName,
                    Address = request.Address,
                    Image = imagePath,
                    BIR = imageBir,
                    CityPermit = imageCityPermit,
                    SchoolPermit = imageSchoolPermit,
                    Role = (int)UserRole.Supplier,
                    IsActive = false,
                    DateCreated = DateTime.UtcNow
                };

                await context.Users.AddAsync(newSupplier);
                await context.SaveChangesAsync();

                return newSupplier;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
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

        public async Task<User> UpdatePassword(int id, UpdatePasswordRequest request)
        {
            try
            {
                var supplier = await context.Users
                                            .Where(u => u.Id == id)
                                            .FirstOrDefaultAsync();
                if (supplier == null)
                    throw new InvalidOperationException("Supplier not found");

                var updatedPassword = PasswordEncryptionService.EncryptPassword(request.Password);

                supplier.Password = updatedPassword;

                context.Users.Update(supplier);
                await context.SaveChangesAsync();

                return supplier;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> UpdateSupplier(int id, SupplierRequest request)
        {
            try
            {
                var existingSupplier = await context.Users
                                                    .Where(a => a.Id == id)
                                                    .FirstOrDefaultAsync();

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

                context.Users.Update(existingSupplier);
                await context.SaveChangesAsync();

                return existingSupplier;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }        
        }
    }
}
