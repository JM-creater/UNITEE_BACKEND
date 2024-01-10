using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Email;
using UNITEE_BACKEND.Models.ImageDirectory;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Models.Security;
using UNITEE_BACKEND.Models.Token;

namespace UNITEE_BACKEND.Services
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public UsersService(AppDbContext dbcontext, IConfiguration _configuration) 
        {
            context = dbcontext;
            configuration = _configuration; 
        }

        public async Task<User?> GetTopSellingSeller()
        {
            var topSellerData = await context.OrderItems
                                             .Include(oi => oi.Order)
                                                .ThenInclude(o => o.User)
                                             .Include(oi => oi.Product)
                                             .Where(oi => oi.Order.IsDeleted == false)
                                             .GroupBy(oi => oi.Product.SupplierId)
                                             .Select(group => new {
                                                 ShopId = group.Key,
                                                 TotalSales = group.Sum(oi => oi.Product.Price * oi.Quantity)
                                             })
                                             .OrderByDescending(x => x.TotalSales)
                                             .FirstOrDefaultAsync();

            if (topSellerData == null)
            {
                throw new InvalidOperationException("Supplier not found");
            }

            var seller = context.Users.Find(topSellerData.ShopId);
            return seller;
        }


        public async Task<IEnumerable<User>> GetAll()
            => await context.Users.ToListAsync();

        public async Task<User> SupplierById(int id)
        {
            var supplier = await context.Users
                                        .Include(u => u.Ratings)
                                        .Where(u => u.Id == id)
                                        .FirstOrDefaultAsync();

            if (supplier == null || supplier.Role != (int)UserRole.Supplier)
            {
                throw new InvalidOperationException("Supplier not found");
            }

            return supplier;
        }

        public async Task<User> GetById(int id)
            => await context.Users
                            .Include(a => a.Department)
                            .Where(a => a.Id == id)
                            .FirstOrDefaultAsync();

        public async Task<IEnumerable<User>> GetAllSuppliers()
        {
            return await context.Users
                          .Include(u => u.Products)
                            .ThenInclude(u => u.Sizes)
                          .Include(u => u.Ratings)
                          .Where(u => u.Role == (int)UserRole.Supplier)
                          .OrderByDescending(u => u.DateCreated)
                          .ToListAsync();
        }


        public async Task<IEnumerable<User>> GetAllSuppliersProducts(int departmentId)
        {
            var supplierIdsProductDepartment = await context.ProductDepartments
                                                            .Where(pd => pd.DepartmentId == departmentId)
                                                            .Select(pd => pd.Product.SupplierId)
                                                            .Distinct()
                                                            .ToListAsync();

            return await context.Users
                          .Where(u => u.Role == (int)UserRole.Supplier && supplierIdsProductDepartment.Contains(u.Id))
                          .Include(u => u.Products)
                             .ThenInclude(u => u.Sizes)
                          .Include(u => u.Products)
                             .ThenInclude(u => u.Ratings)
                          .Include(u => u.Products)
                             .ThenInclude(u => u.ProductDepartments)
                                .ThenInclude(u => u.Department)
                         .ToListAsync();
        }

        public async Task<List<User>> GetSupplierById(int id)
            => await context.Users
                            .Where(u => u.Id == id && u.Role == (int)UserRole.Supplier)
                            .ToListAsync();

        public async Task<IEnumerable<Product>> GetProductsBySupplierShop(int supplierId)
            => await context.Products
                            .Where(p => p.SupplierId == supplierId)
                            .ToListAsync();


        public async Task<IEnumerable<User>> GetAllCustomers()
            => await context.Users
                            .Where(c => c.Role == (int)UserRole.Customer)
                            .OrderByDescending(u => u.DateCreated)
                            .ToListAsync();

        public async Task<User> RegisterCustomer(RegisterRequest request)
        {
            try
            {
                var existingUserId = await context.Users
                                                  .Where(u => u.Id == request.Id)
                                                  .FirstOrDefaultAsync();

                if (existingUserId != null)
                {
                    throw new AuthenticationException("A user with ID already exists.");
                }

                var existingUserEmail = await context.Users
                                                     .Where(u => u.Email == request.Email)
                                                     .FirstOrDefaultAsync();

                if (existingUserEmail != null)
                {
                    throw new AuthenticationException("A user with email already exists.");
                }

                var existingUserFirstname = await context.Users
                                                         .Where(u => u.FirstName == request.FirstName)
                                                         .FirstOrDefaultAsync();

                if (existingUserFirstname != null)
                {
                    throw new AuthenticationException("A user with the same first name already exists.");
                }

                var existingUserLastname = await context.Users
                                                        .Where(u => u.LastName == request.LastName)
                                                        .FirstOrDefaultAsync();

                if (existingUserLastname != null)
                {
                    throw new AuthenticationException("A user with the same last name already exists.");
                }

                var existingUserPhonenumber = await context.Users
                                                           .Where(u => u.PhoneNumber == request.PhoneNumber)
                                                           .FirstOrDefaultAsync();

                if (existingUserPhonenumber != null)
                {
                    throw new AuthenticationException("A user with phone number already exists.");
                }

                var imagePath = await new ImagePathConfig().SaveImage(request.Image);
                var studyLoadPath = await new ImagePathConfig().SaveStudyLoad(request.StudyLoad);
                var encryptedPassword = PasswordEncryptionService.EncryptPassword(request.Password);
                var confirmationToken = Tokens.CreateRandomToken();
                var confirmationCode = Tokens.GenerateConfirmationCode();

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
                    DateCreated = DateTime.Now,
                    EmailConfirmationToken = confirmationToken,
                    ConfirmationCode = confirmationCode,
                    IsValidate = false,
                    EmailVerificationStatus = EmailStatus.Pending,
                    EmailVerificationSentTime = DateTime.Now

                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                var emailConfig = new EmailConfig(configuration);

                await emailConfig.SendConfirmationEmail(newUser.Email, newUser.ConfirmationCode);

                return newUser;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> ConfirmEmail(string confirmationCode)
        {
            try
            {
                var user = await context.Users
                                        .Where(u => u.ConfirmationCode == confirmationCode)
                                        .FirstOrDefaultAsync();

                if (user == null)
                    throw new ArgumentException("User not found.");

                if (user.EmailVerificationStatus == EmailStatus.Pending && DateTime.Now > user.EmailVerificationSentTime.AddHours(24))
                {
                    user.EmailVerificationStatus = EmailStatus.Expired;
                    var emailConfig = new EmailConfig(configuration);
                    await emailConfig.SendConfirmationEmail(user.Email, confirmationCode);

                    context.Users.Update(user);
                    await context.SaveChangesAsync();

                    return user;
                }
                else if (user.EmailVerificationStatus == EmailStatus.Pending)
                {
                    user.EmailVerificationStatus = EmailStatus.Verified;
                    user.EmailConfirmationToken = null;
                    user.IsEmailConfirmed = true;
                    user.ConfirmationCode = null; 
                }
                else if (user.EmailVerificationStatus == EmailStatus.Deferred)
                {
                    user.EmailVerificationStatus = EmailStatus.Verified;
                    user.EmailConfirmationToken = null;
                    user.IsEmailConfirmed = true;
                    user.ConfirmationCode = null;
                }
                else
                {
                    throw new InvalidOperationException("Invalid confirmation code or status.");
                }

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> VerifyLater(int userId)
        {
            try
            {
                var user = await context.Users
                                        .Where(u => u.Id == userId)
                                        .FirstOrDefaultAsync();

                if (user != null)
                {
                    user.EmailVerificationStatus = EmailStatus.Deferred; 
                }

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> VerifyEmail(int userId)
        {
            try
            {
                var user = await context.Users 
                                        .Where(u => u.Id == userId)
                                        .FirstOrDefaultAsync();

                var confirmationCode = Tokens.GenerateConfirmationCode();

                if (user.EmailVerificationStatus == EmailStatus.Deferred || user.EmailVerificationStatus == EmailStatus.Pending)
                {
                    user.ConfirmationCode = confirmationCode;
                    var emailConfig = new EmailConfig(configuration);
                    await emailConfig.SendConfirmationEmail(user.Email, user.ConfirmationCode);
                }

                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
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
                    throw new InvalidOperationException("A user with ID already exists.");
                }

                var existingUserEmail = await context.Users
                                                     .Where(u => u.Email == request.Email)
                                                     .FirstOrDefaultAsync();

                if (existingUserEmail != null)
                {
                    throw new InvalidOperationException("A user with email already exists.");
                }

                var existingUserShopName = await context.Users
                                                        .Where(u => u.ShopName == request.ShopName)
                                                        .FirstOrDefaultAsync();

                if (existingUserShopName != null)
                {
                    throw new InvalidOperationException("A user with shop name already exists.");
                }

                var existingUserAddress = await context.Users
                                                       .Where(u => u.Address == request.Address)
                                                       .FirstOrDefaultAsync();

                if (existingUserAddress != null)
                {
                    throw new InvalidOperationException("A user with address already exists.");
                }

                var imagePath = await new ImagePathConfig().SaveSupplierImage(request.Image);
                var imageBir = await new ImagePathConfig().SaveBIR(request.BIR);
                var imageCityPermit = await new ImagePathConfig().SaveCityPermit(request.CityPermit);
                var imageSchoolPermit = await new ImagePathConfig().SaveSchoolPermit(request.SchoolPermit);
                var imageBarangayClearance = await new ImagePathConfig().SaveBarangayClearance(request.BarangayClearance);
                var imageValidIdFrontImage = await new ImagePathConfig().SaveValidIdFrontImage(request.ValidIdFrontImage);
                var imageValidIdBackImage = await new ImagePathConfig().SaveValidIdBackImage(request.ValidIdBackImage);
                var encryptedPassword = PasswordEncryptionService.EncryptPassword(request.Password);
                var confirmationToken = Tokens.CreateRandomToken();
                var confirmationCode = Tokens.GenerateConfirmationCode();

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
                    BarangayClearance = imageBarangayClearance,
                    ValidIdFrontImage = imageValidIdFrontImage,
                    ValidIdBackImage = imageValidIdBackImage,
                    Role = (int)UserRole.Supplier,
                    IsActive = false,
                    DateCreated = DateTime.Now,
                    EmailConfirmationToken = confirmationToken,
                    ConfirmationCode = confirmationCode,
                    IsValidate = false
                };

                await context.Users.AddAsync(newSupplier);
                await context.SaveChangesAsync();

                var emailConfig = new EmailConfig(configuration);
                await emailConfig.SendConfirmationEmail(newSupplier.Email, newSupplier.ConfirmationCode);

                return newSupplier;
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
                                        .FirstOrDefaultAsync();
                }
                else if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    user = await context.Users
                                        .Where(u => u.Email == request.Email)
                                        .FirstOrDefaultAsync();
                }

                if (user == null)
                {
                    throw new AuthenticationException("User not registered");
                }

                if (!user.IsValidate)
                {
                    throw new AuthenticationException("Waiting for validation");
                }

                if (!user.IsActive)
                {
                    throw new AuthenticationException("Account is deactivated");
                }

                if (!PasswordEncryptionService.VerifyPassword(request.Password, user.Password))
                {
                    throw new AuthenticationException("Incorrect Password");
                }

                return (user, (UserRole)user.Role);
            }
            catch (Exception e)
            {
                throw new AuthenticationException(e.Message);
            }
        }

        public async Task<User> ValidateCustomer(int id, ValidateUserRequest request)
        {
            try
            {
                var userExist = await context.Users
                                             .Where(a => a.Id == id)
                                             .FirstOrDefaultAsync();

                if (userExist == null)
                    throw new InvalidOperationException("User not Found");

                if (userExist.Role != (int)UserRole.Customer)
                    throw new InvalidOperationException("The provided ID does not correspond to a customer");

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
                var userExist = await context.Users
                                             .Where(a => a.Id == id)
                                             .FirstOrDefaultAsync();

                if (userExist == null)
                    throw new InvalidOperationException("User not Found");

                if (userExist.Role != (int)UserRole.Supplier)
                    throw new InvalidOperationException("The provided ID does not correspond to a supplier");

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

        public async Task<User> UpdateCustomerProfile(int id, UpdateCustomerRequest request)
        {
            try
            {
                var user = await context.Users
                                        .Where(e => e.Id == id)
                                        .FirstOrDefaultAsync();

                if (user == null)
                    throw new InvalidOperationException("No User Found");

                var confirmationCode = Tokens.GenerateConfirmationCode();

                if (request.Image != null)
                {
                    var imageUser = await new ImagePathConfig().SaveImage(request.Image);
                    user.Image = imageUser;
                }

                if (!string.IsNullOrEmpty(request.firstName))
                {
                    user.FirstName = request.firstName;
                }

                if (!string.IsNullOrEmpty(request.lastName))
                {
                    user.LastName = request.lastName;
                }

                if (!string.IsNullOrEmpty(request.email))
                {
                    var emailExists = await context.Users
                                                   .AnyAsync(u => u.Id != id &&
                                                             u.Email == request.email);

                    if (emailExists)
                    {
                        throw new InvalidOperationException("Email already in use by another user.");
                    }

                    if (user.Email != request.email)
                    {
                        user.Email = request.email;
                        user.EmailVerificationStatus = EmailStatus.Pending;
                    }
                }

                if (!string.IsNullOrEmpty(request.phoneNumber))
                {
                    user.PhoneNumber = request.phoneNumber;
                }

                if (!string.IsNullOrEmpty(request.gender))
                {
                    user.Gender = request.gender;
                }

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> UpdateAdminProfile(int id, UpdateAdminRequest request)
        {
            try
            {
                var user = await context.Users
                                        .Where(e => e.Id == id)
                                        .FirstOrDefaultAsync();

                if (user == null)
                    throw new InvalidOperationException("No User Found");

                if (request.Image != null)
                {
                    var imageAdmin = await new ImagePathConfig().SaveImage(request.Image);
                    user.Image = imageAdmin;
                }

                if (!string.IsNullOrEmpty(request.firstName))
                {
                    user.FirstName = request.firstName;
                }

                if (!string.IsNullOrEmpty(request.lastName))
                {
                    user.LastName = request.lastName;
                }

                if (!string.IsNullOrEmpty(request.email))
                {
                    var emailExists = await context.Users
                                                   .AnyAsync(u => u.Id != id &&
                                                             u.Email == request.email);

                    if (emailExists)
                    {
                        throw new InvalidOperationException("Email already in use by another user.");
                    }

                    if (user.Email != request.email)
                    {
                        user.Email = request.email;
                        user.EmailVerificationStatus = EmailStatus.Pending;
                    }
                }

                if (!string.IsNullOrEmpty(request.phoneNumber))
                {
                    user.PhoneNumber = request.phoneNumber;
                }

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> UpdateProfileSupplier(int id, UpdateSupplierRequest request)
        {
            try
            {
                var supplier = await context.Users
                                            .Where(e => e.Id == id)
                                            .FirstOrDefaultAsync();

                if (supplier == null)
                {
                    throw new InvalidOperationException("No User Found");
                }

                if (request.Image != null)
                {
                    var imageSupplier = await new ImagePathConfig().SaveSupplierImage(request.Image);
                    supplier.Image = imageSupplier;
                }

                if (!string.IsNullOrEmpty(request.shopName))
                {
                    supplier.ShopName = request.shopName;
                }

                if (!string.IsNullOrEmpty(request.address))
                {
                    supplier.Address = request.address;
                }

                if (!string.IsNullOrEmpty(request.phoneNumber))
                {
                    supplier.PhoneNumber = request.phoneNumber;
                }

                if (!string.IsNullOrEmpty(request.email))
                {
                    var emailExists = await context.Users
                                                   .AnyAsync(u => u.Id != id && 
                                                             u.Email == request.email);

                    if (emailExists)
                    {
                        throw new InvalidOperationException("Email already in use by another user.");
                    }

                    if (supplier.Email != request.email)
                    {
                        supplier.Email = request.email;
                        supplier.EmailVerificationStatus = EmailStatus.Pending;
                    }
                }

                context.Users.Update(supplier);
                await context.SaveChangesAsync();

                return supplier;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> UpdateCustomerPassword(int id, UpdatePasswordRequest request)
        {
            try
            {
                var user = await context.Users
                                            .Where(u => u.Id == id)
                                            .FirstOrDefaultAsync();

                if (user == null)
                    throw new InvalidOperationException("Customer not found");

                var updatedPassword = PasswordEncryptionService.EncryptPassword(request.Password);

                user.Password = updatedPassword;

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> UpdateSupplierPassword(int id, UpdatePasswordRequest request)
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

        public async Task<User> UpdateAdminPassword(int id, UpdatePasswordRequest request)
        {
            try
            {
                var supplier = await context.Users
                                            .Where(u => u.Id == id)
                                            .FirstOrDefaultAsync();

                if (supplier == null)
                    throw new InvalidOperationException("Admin not found");

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

        public async Task<User> ForgotPassword(string email)
        {
            try
            {
                var user = await context.Users
                                    .Where(u => u.Email == email)
                                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new InvalidOperationException("Email not yet registered");
                }

                user.PasswordResetToken = Tokens.CreateRandomToken();
                user.ResetTokenExpires = DateTime.Now.AddDays(1);

                context.Users.Update(user);
                await context.SaveChangesAsync();

                var emailConfig = new EmailConfig(configuration);
                await emailConfig.SendPasswordResetEmail(user.Email, user.PasswordResetToken);

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<User> ResetPassword(ResetPasswordDto dto)
        {
            try
            {
                var user = await context.Users
                                    .Where(u => u.PasswordResetToken == dto.Token)
                                    .FirstOrDefaultAsync();

                if (user == null || user.ResetTokenExpires < DateTime.Now)
                {
                    throw new InvalidOperationException("Invalid Token.");
                }

                user.Password = PasswordEncryptionService.EncryptPassword(dto.NewPassword);
                user.PasswordResetToken = null;
                user.ResetTokenExpires = null;

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<bool> IsResetTokenValid(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return false;

                var user = await context.Users
                                        .Where(u => u.PasswordResetToken == token
                                                && u.ResetTokenExpires > DateTime.UtcNow)
                                        .FirstOrDefaultAsync();

                return user != null;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    }
}
