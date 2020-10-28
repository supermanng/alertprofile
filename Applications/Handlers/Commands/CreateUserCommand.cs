using Applications.Interfaces;
using Common;
using Common.Models;
using Domain.Entities;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applications.Handlers.Commands
{
   public class CreateUserCommand:IRequest<BaseMessageResponse>
    {
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int AlertSetting { get; set; }
        public int CommunicationTypeId { get; set; }
    }
    public class CreateUserCommandValidator:AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(c => c.AlertSetting).NotEmpty();
            RuleFor(c => c.CommunicationTypeId).NotEmpty();
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.CommunicationTypeId).NotEmpty();
            RuleFor(c => c.FirstName).NotEmpty();
            RuleFor(c => c.LastName).NotEmpty();
            RuleFor(c => c.MobileNumber).NotEmpty().Length(11);
        }
    }
    public class CreateUserCommandHanlder : IRequestHandler<CreateUserCommand, BaseMessageResponse>
    {
        private readonly IAlertDbContext _dbContext;
        public CreateUserCommandHanlder(IAlertDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<BaseMessageResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userWithEmailFound = await _dbContext.User.AnyAsync(c => c.Email == request.Email);
            if (userWithEmailFound)
                return Util.GetSimpleResponse(false, "User with similar email already exists");
            var userWithPhoneFound = await _dbContext.User.AnyAsync(c => c.MobileNumber == request.MobileNumber);
            if (userWithPhoneFound)
                return Util.GetSimpleResponse(false, "User with similar Phone number already exists");

            var usr = new User()
            {
                CommunitactionType = (CommMedium)request.CommunicationTypeId,
                Email = request.Email,
                FirstName = request.FirstName,
                MobileNumber = request.MobileNumber,
                RegistrationDate = DateTime.Now,
                LastName = request.LastName
            };
            _dbContext.User.Add(usr);
            await _dbContext.SaveChangesAsync();
            return Util.GetSimpleResponse(true, $"Your registration was successful..");
        }
    }
}
