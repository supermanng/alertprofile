using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.ComponentModel;
using Applications.ExtentionMethods;

namespace App.Commands
{
    public class CreateJwtTokenCommand : IRequest<string>
    {


        public long UserId { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public string UserEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CIF { get; set; }
        public JwtUserType JwtUserType { get; set; } = JwtUserType.AnonynmousUser;
    }

    public enum JwtUserType
    {
        [Description("Registered")]
        RegisteredUser,
        [Description("Anonymous")]
        AnonynmousUser
    }

    public class TokenData
    {
        public long UserId { get; set; }
        public string UserType { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerName { get; set; }
        public string UserEmail { get; set; }

    }


    public class CreateJwtTokenCommandHandler : IRequestHandler<CreateJwtTokenCommand, string>
    {

        private readonly ILogger<CreateJwtTokenCommandHandler> _logger;
        private readonly IConfiguration _config;

        public CreateJwtTokenCommandHandler(
               IConfiguration config,
             ILogger<CreateJwtTokenCommandHandler> logger)
        {

            _config = config;
            _logger = logger;
        }

        public async Task<string> Handle(CreateJwtTokenCommand request, CancellationToken cancellationToken)
        {

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:SecurityKey")));

            var claims = new Claim[] {

                new Claim(ClaimTypes.Actor, $"{request.CIF}"),
                new Claim(ClaimTypes.NameIdentifier, $"{request.UserId.ToString()}"),
                new Claim(ClaimTypes.Name, $"{request.FirstName} {request.LastName}"),
                new Claim(JwtRegisteredClaimNames.Email, $"{request.UserEmail}"),
                new Claim(ClaimTypes.MobilePhone, $"{request.MobileNumber}"),
                new Claim(ClaimTypes.NameIdentifier, $"{request.MobileNumber}"),
                new Claim(ClaimTypes.Role, $"{request.JwtUserType.GetDescription()}"),
                new Claim(ClaimTypes.UserData,  JsonConvert.SerializeObject(new TokenData(){
                 CustomerName=$"{request.FirstName} {request.LastName}",
                 MobileNumber=request.MobileNumber,
                 UserEmail=request.UserEmail,
                 UserId=request.UserId,
                 UserType="Customer"

             }))

            };


            var token = new JwtSecurityToken(
                issuer: _config.GetValue<string>("Jwt:Issuer"),
                audience: _config.GetValue<string>("Jwt:Audience"),
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:TimeoutMinutes")),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }


    }



}
