namespace userPortalBackend.presentation
{
    public static class EmailBody
    {
        public static string EmailStringbody(string email,string emailtoken)
        {
            return $@"<!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Password Reset Email</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                }}
                .container {{
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                    max-width: 600px;
                    width: 100%;
                    text-align: center;
                }}
                h2 {{
                    color: #333333;
                    margin-bottom: 20px;
                }}
                p {{
                    color: #666666;
                    font-size: 16px;
                    margin-bottom: 30px;
                }}
                .btn {{
                    display: inline-block;
                    background-color: #a2bad5;
                    color: #ffffff;
                    text-decoration: none;
                    padding: 10px 20px;
                    border-radius: 5px;
                    transition: background-color 0.3s ease;
                }}
                .btn:hover {{
                    background-color: #0056b3;
                    color:white
                }}
            </style>
       </head>
        <body>
            <div class=""container"">
                <h2>Password Reset</h2>
                <p>You have requested to reset your password. Click the button below to reset it:</p>
                <a class=""btn"" href=""http://localhost:4200/reset?email={email}&code={emailtoken}"" target=""_blank"">Reset Password</a>
            </div>
        </body>
        </html>
         ";

        }
    }
}
