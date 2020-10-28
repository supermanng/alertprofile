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
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applications.Handlers.Commands
{
   public class InvokeTransactionCommand:IRequest<BaseMessageResponse>
    {
        public long UserId { get; set; }
        public int TransactionType { get; set; }
        
    }
   
    public class InvokeTransactionCommandValidator:AbstractValidator<InvokeTransactionCommand>
    {
        public InvokeTransactionCommandValidator()
        {
            RuleFor(c => c.UserId).NotEmpty();
            RuleFor(c => c.TransactionType).NotEmpty();
        }
    }
    public class InvokeTransactionCommandHanlder : IRequestHandler<InvokeTransactionCommand, BaseMessageResponse>
    {
        private bool SendEmail(string to, string msg, string title)
        {
            var smtp = "smtp.gmail.com";
            var username = "delighttechnosoft@gmail.com";
            var password = "Actionman@1234";
            MailMessage email = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(smtp);

            email.From = new MailAddress(username);
            email.To.Add(to);
            email.Subject = title;
            email.Body = msg;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
            SmtpServer.EnableSsl = true;
            // await SmtpServer.SendMailAsync(email);
            SmtpServer.Send(email);
            return true;

        }
        private readonly IAlertDbContext _dbContext;
        public InvokeTransactionCommandHanlder(IAlertDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<BaseMessageResponse> Handle(InvokeTransactionCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(c => c.Id == request.UserId);
            if(user==null)
                return Util.GetSimpleResponse(false, "User not found");
            var transactionType = (TransactionType)request.TransactionType;
            var userTranactionSettings = user.TransactionType;
            if (userTranactionSettings == TransactionType.Both)
            {
                if ((CommMedium.Email == user.CommunitactionType) || (CommMedium.Both == user.CommunitactionType))
                     SendEmail(user.Email, $"A transaction of type {transactionType.ToString()} has occured on your account", "Notification");
            }
            else
            {
                if(transactionType==user.TransactionType)
                    if ((CommMedium.Email == user.CommunitactionType) || (CommMedium.Both == user.CommunitactionType))
                        SendEmail(user.Email, $"A transaction of type {transactionType.ToString()} has occured on your account", "Notification");
            }
            return Util.GetSimpleResponse(true, $"Task completed");
        }
    }
}
