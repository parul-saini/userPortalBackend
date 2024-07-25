namespace userPortalBackend.presentation
{
    public static class EmailBody
    {
        public static string EmailStringbody(string email,string emailtoken)
        {
            return $@"<!doctype html>
       <html lang=""en"">
      <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>Bootstrap demo</title>
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH"" crossorigin=""anonymous"">
    <style>body {{
      font-family: Arial, sans-serif;
      background-color: #f2f2f2;
      margin: 0;
      padding: 0;
    }}
    
    .container {{
      max-width: 600px;
      margin: 0 auto;
      padding: 20px;
      background-color: #fff;
    }}
    
    .header {{
      text-align: center;
      padding: 20px 0;
    }}
    
    .header img {{
      max-width: 150px;
    }}
    
    .header-content {{
      background-color: #282C34;
      color: #fff;
      padding: 20px;
      border-radius: 5px;
    }}
    
    .body {{
      padding: 20px;
    }}
    a{{
     color : white; 
    }}
    .button {{
      background-color: #282C34;
      color: white;
      padding: 10px 20px;
      text-decoration: none;
      display: inline-block;
      border-radius: 5px;
    }}
    
    </style>
  </head>
  <body>
    <div class=""container"">
      <div class=""header"">
       
        <div class=""header-content"">
      
          <h2>Please reset your password</h2>
        </div>
      </div>
      <div class=""body"">
        <p>Hello,</p>
        <p>We have sent you this email in response to your request to reset your password.</p>
        <p>To reset your password, please follow the link below:</p>
        <a class=""btn btn-dark"" href=""http://localhost:4200/reset?email={email}&code={emailtoken}"" target=""_blank"">Reset Password</a>
        <p>Please ignore this email if you did not request a password change.</p>
      </div>
    </div>
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"" integrity=""sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz"" crossorigin=""anonymous""></script>
      </body>
    </html>";

        }
    }
}
