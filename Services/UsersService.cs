using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.ImageDirectory;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Models.Security;

namespace UNITEE_BACKEND.Services
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext context;

        public UsersService(AppDbContext dbcontext) 
        {
            context = dbcontext;
        }

        public IEnumerable<User> GetAll()
            => context.Users.AsEnumerable();

        public async Task<User> GetById(int id)
            => await context.Users
                            .Include(a => a.Department)
                            .Where(a => a.Id == id)
                            .FirstOrDefaultAsync();

        public IEnumerable<User> GetAllSuppliers()
        {
            return context.Users
                          .Include(u => u.Products)
                            .ThenInclude(u => u.Sizes)
                          .Where(u => u.Role == (int)UserRole.Supplier)
                          .OrderByDescending(u => u.DateCreated)
                          .AsEnumerable();
        }


        public IEnumerable<User> GetAllSuppliersProducts(int departmentId)
        {
            var supplierIdsWithProductsInDepartment = context.ProductDepartments
                                                            .Where(pd => pd.DepartmentId == departmentId)
                                                            .Select(pd => pd.Product.SupplierId)
                                                            .Distinct()
                                                            .ToList();

            return context.Users
                         .Where(u => u.Role == (int)UserRole.Supplier && supplierIdsWithProductsInDepartment.Contains(u.Id))
                         .AsEnumerable();
        }


        public async Task<List<User>> GetSupplierById(int id)
            => await context.Users
                            .Where(u => u.Id == id && u.Role == (int)UserRole.Supplier)
                            .ToListAsync();

        public IEnumerable<Product> GetProductsBySupplierShop(int supplierId)
            => context.Products
                      .Where(p => p.SupplierId == supplierId)
                      .ToList();


        public IEnumerable<User> GetAllCustomers()
            => context.Users
                          .Where(c => c.Role == (int)UserRole.Customer)
                          .OrderByDescending(u => u.DateCreated)
                          .AsEnumerable();

        public async Task<User> Register(RegisterRequest request)
        {
            try
            {
                var existingUserId = await context.Users
                                                  .Where(u => u.Id == request.Id)
                                                  .FirstOrDefaultAsync();

                if (existingUserId != null)
                {
                    throw new InvalidOperationException("A user with ID already exists.");
                }

                var existingUserEmail = await context.Users
                                                     .Where(u => u.Email == request.Email)
                                                     .FirstOrDefaultAsync();

                if (existingUserEmail != null)
                {
                    throw new InvalidOperationException("A user with email already exists.");
                }

                var existingUserFirstname = await context.Users
                                                         .Where(u => u.FirstName == request.FirstName)
                                                         .FirstOrDefaultAsync();

                if (existingUserFirstname != null)
                {
                    throw new InvalidOperationException("A user with the same first name already exists.");
                }

                var existingUserLastname = await context.Users
                                                        .Where(u => u.LastName == request.LastName)
                                                        .FirstOrDefaultAsync();

                if (existingUserLastname != null)
                {
                    throw new InvalidOperationException("A user with the same last name already exists.");
                }

                var existingUserPhonenumber = await context.Users
                                                           .Where(u => u.PhoneNumber == request.PhoneNumber)
                                                           .FirstOrDefaultAsync();

                if (existingUserPhonenumber != null)
                {
                    throw new InvalidOperationException("A user with phone number already exists.");
                }

                var imagePath = await new ImagePathConfig().SaveImage(request.Image);
                var studyLoadPath = await new ImagePathConfig().SaveStudyLoad(request.StudyLoad);
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
                    IsActive = false,
                    DateCreated = DateTime.UtcNow
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                return newUser;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
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

                if (!PasswordEncryptionService.VerifyPassword(request.Password, user.Password))
                    throw new AuthenticationException("Invalid Password");

                return (user, (UserRole)user.Role);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> ValidateUser(int id, ValidateUserRequest request)
        {
            try
            {
                var userExist = await context.Users.Where(a => a.Id == id).FirstOrDefaultAsync();

                if (userExist == null)
                    throw new Exception("User not Found");

                if (userExist.Role != (int)UserRole.Customer)
                    throw new Exception("The provided ID does not correspond to a customer");

                userExist.IsValidate = request.IsValidate;
                userExist.IsActive = request.IsActive;

                context.Users.Update(userExist);
                await context.SaveChangesAsync();

                return userExist;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> ValidateSupplier(int id, ValidateUserRequest request)
        {
            try
            {
                var userExist = await context.Users.Where(a => a.Id == id).FirstOrDefaultAsync();

                if (userExist == null)
                    throw new Exception("User not Found");

                if (userExist.Role != (int)UserRole.Supplier)
                    throw new Exception("The provided ID does not correspond to a supplier");

                userExist.IsValidate = request.IsValidate;
                userExist.IsActive = request.IsActive;

                context.Users.Update(userExist);
                await context.SaveChangesAsync();

                return userExist;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> UpdatePassword(int id, UpdatePasswordRequest request)
        {
            try
            {
                var user = await context.Users
                                        .Where(u => u.Id == id)
                                        .FirstOrDefaultAsync();
                if (user == null)
                    throw new InvalidOperationException("User not found");

                var updatePassword = PasswordEncryptionService.EncryptPassword(request.Password);

                user.Password = updatePassword;

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> Update(int id, UpdateCustomerRequest request)
        {
            try
            {
                var user = await context.Users
                                    .Where(e => e.Id == id)
                                    .FirstOrDefaultAsync();

                if (user == null)
                    throw new InvalidOperationException("No User Found");

                user.FirstName = request.firstName;
                user.LastName = request.lastName;
                user.Email = request.email;
                user.DepartmentId = request.departmentId;
                user.PhoneNumber = request.phoneNumber;
                user.Gender = request.gender;

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> UpdateSupplier(int id, UpdateSupplierRequest request)
        {
            try
            {
                var supplier = await context.Users
                                        .Where(e => e.Id == id)
                                        .FirstOrDefaultAsync();

                if (supplier == null)
                {
                    throw new Exception("No User Found");
                }

                supplier.ShopName = request.shopName;
                supplier.Address = request.address;
                supplier.Email = request.email;
                supplier.PhoneNumber = request.phoneNumber;

                context.Users.Update(supplier);
                await context.SaveChangesAsync();

                return supplier;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    }
}
