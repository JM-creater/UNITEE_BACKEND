using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Security.Authentication;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.GenerateToken;
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


        public IEnumerable<User> GetAll()
            => context.Users.AsEnumerable();

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

        public IEnumerable<User> GetAllSuppliers()
        {
            return context.Users
                          .Include(u => u.Products)
                            .ThenInclude(u => u.Sizes)
                          .Include(u => u.Ratings)
                          .Where(u => u.Role == (int)UserRole.Supplier)
                          .OrderByDescending(u => u.DateCreated)
                          .AsEnumerable();
        }


        public IEnumerable<User> GetAllSuppliersProducts(int departmentId)
        {
            var supplierIdsProductDepartment = context.ProductDepartments
                                                            .Where(pd => pd.DepartmentId == departmentId)
                                                            .Select(pd => pd.Product.SupplierId)
                                                            .Distinct()
                                                            .ToList();

            return context.Users
                          .Where(u => u.Role == (int)UserRole.Supplier && supplierIdsProductDepartment.Contains(u.Id))
                          .Include(u => u.Products)
                             .ThenInclude(u => u.Sizes)
                          .Include(u => u.Products)
                             .ThenInclude(u => u.Ratings)
                          .Include(u => u.Products)
                             .ThenInclude(u => u.ProductDepartments)
                                .ThenInclude(u => u.Department)
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
                var confirmationToken = RandomToken.CreateRandomToken();
                var confirmationCode = RandomToken.GenerateConfirmationCode();

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
                    ConfirmationCode = confirmationCode
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                await SendConfirmationEmail(newUser.Email, newUser.ConfirmationCode);

                return newUser;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task SendConfirmationEmail(string email, string confirmationCode)
        {
            string subject = "Verify Your Unitee Account Email";
            string message = $@"
                    <html>
                    <head>
                      <style>
                        body {{ font-family: 'Arial', sans-serif; background-color: #f6f6f6; padding: 20px; }}
                        .email-container {{ background-color: #ffffff; padding: 20px; border: 1px solid #dddddd; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }}
                        .email-header {{ color: #333333; font-size: 18px; font-weight: bold; margin-bottom: 30px; }}
                        .confirmation-code {{ font-size: 24px; font-weight: bold; color: #333333; padding: 10px 0; }}
                        .instructions {{ font-size: 14px; color: #555555; }}
                        .footer {{ font-size: 12px; color: #999999; margin-top: 30px; }}
                      </style>
                    </head>
                    <body>
                      <div class='email-container'>
                        <p class='email-header'>Verify Your Unitee Account Email</p>
                        <p>Unitee has received a request to use this email address as your account. Please use the following code to finish setting up this email verification:</p>
                        <p class='confirmation-code'>{confirmationCode}</p>
                        <p class='instructions'>This code will expire in 24 hours.</p>
                        <p class='instructions'>If you did not request this change or if you have any questions, please contact using this email unitee42@gmail.com.</p>
                        <p class='footer'>Thank you for using Unitee!</p>
                      </div>
                    </body>
                    </html>";

            await SendEmailAsync(email, subject, message);
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

                if (user.ConfirmationCode == confirmationCode)
                {
                    user.IsEmailConfirmed = true;
                    user.EmailConfirmationToken = null;
                }
                else
                {
                    throw new InvalidOperationException("Invalid confirmation code.");
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
                var encryptedPassword = PasswordEncryptionService.EncryptPassword(request.Password);
                var confirmationToken = RandomToken.CreateRandomToken();
                var confirmationCode = RandomToken.GenerateConfirmationCode();

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
                    DateCreated = DateTime.Now,
                    EmailConfirmationToken = confirmationToken,
                    ConfirmationCode = confirmationCode
                };

                await context.Users.AddAsync(newSupplier);
                await context.SaveChangesAsync();

                await SendConfirmationEmail(newSupplier.Email, newSupplier.ConfirmationCode);

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
                    throw new AuthenticationException("Invalid Password");
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
                var userExist = await context.Users
                                             .Where(a => a.Id == id)
                                             .FirstOrDefaultAsync();

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

        public async Task<User> UpdateCustomerProfile(int id, UpdateCustomerRequest request)
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

        public async Task<User> UpdateAdminProfile(int id, UpdateAdminRequest request)
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

        public async Task<User> UpdateProfileSupplier(int id, UpdateSupplierRequest request)
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

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["Sender"]));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = subject;

            mimeMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["MailServer"], int.Parse(emailSettings["MailPort"]), false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(emailSettings["Sender"], emailSettings["Password"]);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendPasswordResetEmail(string email, string token)
        {
            string resetLink = $"http://localhost:5173/forgot_password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
            string subject = "Password Reset Request";
            string message = $"Please click on the link to reset your password: <a href='{resetLink}'>Reset Password</a>";

            await SendEmailAsync(email, subject, message);
        }

        public async Task<User> ForgotPassword(string email)
        {
            var user = await context.Users
                                    .Where(u => u.Email == email)
                                    .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new InvalidOperationException("Email not yet registered");
            }

            user.PasswordResetToken = RandomToken.CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);

            context.Users.Update(user);
            await context.SaveChangesAsync();

            await SendPasswordResetEmail(user.Email, user.PasswordResetToken);

            return user;
        }

        public async Task<User> ResetPassword(ResetPasswordDto dto)
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

        public async Task<bool> IsResetTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var user = await context.Users
                                    .Where(u => u.PasswordResetToken == token && u.ResetTokenExpires > DateTime.UtcNow)
                                    .FirstOrDefaultAsync();

            return user != null;
        }
    }
}
