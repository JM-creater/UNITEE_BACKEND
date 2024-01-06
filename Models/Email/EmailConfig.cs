using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using UNITEE_BACKEND.DatabaseContext;

namespace UNITEE_BACKEND.Models.Email
{
    public class EmailConfig
    {
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;

        public EmailConfig(IConfiguration _configuration)
        {
            configuration = _configuration;
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

        public async Task SendOrderCompletedEmailAsync(string email, int orderId)
        {
            var orderDetails = await context.Orders
                                            .Where(o => o.Id == orderId && o.User.Email == email)
                                            .Include(o => o.OrderItems)
                                                .ThenInclude(oi => oi.Product)
                                            .FirstOrDefaultAsync();

            if (orderDetails == null)
            {
                throw new ArgumentException("Order not found or email does not match the order's user email.");
            }

            var itemsList = orderDetails.OrderItems.Select(oi => {
                return $@"
                    <tr>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{oi.Product.ProductName}</td>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd; text-align: right;'>Qty - {oi.Quantity}</td>
                    </tr>";
            }).ToList();

            string subject = "Your UNITEE Order Has Been Claimed!";
            string message = $@"
                    <html>
                    <head>
                        <style>
                            .email-body {{
                                font-family: 'Arial', sans-serif;
                                color: #333;
                                margin: 0;
                                padding: 0;
                            }}
                            .header {{
                                background-color: #4CAF50;
                                padding: 20px;
                                text-align: center;
                                font-size: 24px;
                                color: white;
                            }}
                            .order-table {{
                                width: 100%;
                                border-collapse: collapse;
                                margin-top: 20px;
                            }}
                            .order-table th {{
                                background-color: #f2f2f2;
                                padding: 10px;
                                border-bottom: 1px solid #ddd;
                            }}
                            .order-table td {{
                                padding: 10px;
                                border-bottom: 1px solid #ddd;
                            }}
                            .product-image {{
                                max-width: 100px;
                                max-height: 100px;
                            }}
                            .total-cost {{
                                text-align: right;
                                margin-top: 20px;
                                font-size: 18px;
                            }}
                            .footer {{
                                margin-top: 20px;
                                text-align: center;
                                font-size: 14px;
                                color: #999;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-body'>
                             <div class='header'>Order Completed</div>
                             <p class=""greeting-user-name"">Hi {orderDetails.User.FirstName} {orderDetails.User.LastName},</p>
                             <p>We are happy to inform you that your order with the reference <strong>{orderDetails.OrderNumber}</strong> has been successfully completed!</p>
                             <table class='order-table'>
                                 <tr>
                                     <th style='text-align: left;'>Product Name</th>
                                     <th style='text-align: right;'>Quantity</th>
                                 </tr>
                                  {string.Join("", itemsList)}
                             </table>
                             <div class='total-cost'><strong>Total cost:</strong> {orderDetails.Total:C}</div>
                             <p>We hope you enjoy your purchase. Feel free to reach out for any further assistance.</p>
                             <footer>
                                Thank you for shopping with UNITEE!<br>Stay stylish!</footer>
                         </div>
                    </body>
                    </html>";


            await SendEmailAsync(email, subject, message);
        }

        public async Task SendPasswordResetEmail(string email, string token)
        {
            string resetLink = $"http://localhost:5173/forgot_password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
            string subject = "Password Reset Request";
            string message = $@"
                <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Arial', sans-serif;
                                color: #333;
                                background-color: #f4f4f4;
                                padding: 20px;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                background: #fff;
                                padding: 20px;
                                border-radius: 8px;
                                box-shadow: 0 0 10px rgba(0,0,0,0.1);
                            }}
                            .button {{
                                display: inline-block;
                                padding: 10px 20px;
                                background-color: #007bff;
                                border-radius: 5px;
                                text-decoration: none;
                                font-weight: bold;
                            }}
                            .button a {{
                                color: #fff; 
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h2>Password Reset Request</h2>
                            <p>You requested a password reset for your account. Please click the button below to set a new password:</p>
                            <a href='{resetLink}' class='button'>Reset Password</a>
                            <p>If you did not request a password reset, please ignore this email.</p>
                        </div>
                    </body>
                </html>";

            await SendEmailAsync(email, subject, message);
        }
    }
}
