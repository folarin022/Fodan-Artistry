using FodanArtistry.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.Services
{
    public static class EmailSenderExtensions
    {
        public static async Task SendConfirmationEmailAsync(
            this IEmailSender emailSender,
            string email,
            string firstName,
            string confirmationLink)
        {
            var subject = "Confirm your email - Fodan Artistry";

            var htmlMessage = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f8f9fa;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background: white;
                        border-radius: 20px;
                        overflow: hidden;
                        box-shadow: 0 10px 30px rgba(0,0,0,0.1);
                    }}
                    .header {{
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white;
                        padding: 40px 30px;
                        text-align: center;
                    }}
                    .header h1 {{
                        margin: 0;
                        font-size: 2rem;
                    }}
                    .content {{
                        padding: 40px 30px;
                    }}
                    .button {{
                        display: inline-block;
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white;
                        text-decoration: none;
                        padding: 15px 40px;
                        border-radius: 50px;
                        margin: 20px 0;
                        font-weight: 600;
                    }}
                    .footer {{
                        padding: 20px 30px;
                        background: #f8f9fa;
                        text-align: center;
                        color: #6c757d;
                        font-size: 0.9rem;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Welcome to Fodan Artistry! 🎨</h1>
                    </div>
                    <div class='content'>
                        <h2>Hello {firstName},</h2>
                        <p>Thank you for registering with Fodan Artistry. Please confirm your email address by clicking the button below:</p>
                        
                        <div style='text-align: center;'>
                            <a href='{confirmationLink}' class='button'>Confirm Email Address</a>
                        </div>
                        
                        <p>If the button doesn't work, copy and paste this link into your browser:</p>
                        <p style='word-break: break-all; background: #f8f9fa; padding: 10px; border-radius: 5px;'>{confirmationLink}</p>
                        
                        <p>This link will expire in 24 hours.</p>
                        <p>If you didn't create an account, please ignore this email.</p>
                    </div>
                    <div class='footer'>
                        <p>&copy; {DateTime.Now.Year} Fodan Artistry. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";

            await emailSender.SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
