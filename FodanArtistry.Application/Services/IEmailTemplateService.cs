public interface IEmailTemplateService
{
    string GetConfirmationEmail(string firstName, string confirmationLink);
    string GetPasswordResetEmail(string firstName, string resetLink);
    string GetWelcomeEmail(string firstName);
}

public class EmailTemplateService : IEmailTemplateService
{
    public string GetConfirmationEmail(string firstName, string confirmationLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: 'Segoe UI', sans-serif; }}
                .container {{ max-width: 600px; margin: 0 auto; }}
                .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }}
                .content {{ padding: 30px; background: #f8f9fa; }}
                .button {{ display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-decoration: none; padding: 12px 30px; border-radius: 25px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Welcome to Fodan Artistry! 🎨</h1>
                </div>
                <div class='content'>
                    <h2>Hello {firstName},</h2>
                    <p>Please confirm your email address by clicking the button below:</p>
                    <div style='text-align: center;'>
                        <a href='{confirmationLink}' class='button'>Confirm Email</a>
                    </div>
                    <p style='margin-top: 30px; font-size: 12px; color: #6c757d;'>
                        This link will expire in 24 hours.
                    </p>
                </div>
            </div>
        </body>
        </html>";
    }

    public string GetPasswordResetEmail(string firstName, string resetLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: 'Segoe UI', sans-serif; }}
                .container {{ max-width: 600px; margin: 0 auto; }}
                .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 30px; text-align: center; }}
                .content {{ padding: 30px; background: #f8f9fa; }}
                .button {{ display: inline-block; background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; text-decoration: none; padding: 12px 30px; border-radius: 25px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Reset Your Password 🔐</h1>
                </div>
                <div class='content'>
                    <h2>Hello {firstName},</h2>
                    <p>We received a request to reset your password. Click the button below to proceed:</p>
                    <div style='text-align: center;'>
                        <a href='{resetLink}' class='button'>Reset Password</a>
                    </div>
                    <p style='margin-top: 30px;'>If you didn't request this, please ignore this email.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    public string GetWelcomeEmail(string firstName)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: 'Segoe UI', sans-serif; }}
                .container {{ max-width: 600px; margin: 0 auto; }}
                .header {{ background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 30px; text-align: center; }}
                .content {{ padding: 30px; background: #f8f9fa; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Welcome to Fodan Artistry! 🎉</h1>
                </div>
                <div class='content'>
                    <h2>Hello {firstName},</h2>
                    <p>Thank you for joining Fodan Artistry! We're excited to have you.</p>
                    <p>Start exploring amazing artworks from talented artists around the world.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://localhost:44399/Artwork/Gallery' 
                           style='background: #10b981; color: white; padding: 12px 30px; text-decoration: none; border-radius: 25px;'>
                            Browse Gallery
                        </a>
                    </div>
                </div>
            </div>
        </body>
        </html>";
    }
}