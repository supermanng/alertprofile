//using App.Commands;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Applications.ExtentionMethods;
//using Applications.Interfaces;
//using MobilityOnboarding.Common.OnboardingModels;
//using MobilityOnboarding.Domain.Entities;
//using MobilityOnboarding.Domain.Enum;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Applications.Helper
//{
//    public class VerificationHelper : IVerificationHelper
//    {
//        private readonly IMobilityOnboardingDbContext _dbContext;
//        private readonly IConfiguration _configuration;
//        private readonly IMediator _mediatr;
//        public VerificationHelper(IMobilityOnboardingDbContext dbContext, IConfiguration configuration, IMediator mediatr)
//        {
//            _dbContext = dbContext;
//            _configuration = configuration;
//            _mediatr = mediatr;
//        }
//        public bool IsOnboardingPurpose(int purpose)
//        {
//            return (int)VerificationPurpose.Onboarding == purpose;
//        }
//        private string GenerateCode()
//        {
//            int _min = 1;
//            int _max = 9999;
//            Random _rdm = new Random();
//            var number = _rdm.Next(_min, _max);
//            return $"{number}".PadLeft(4, '0');
//        }


//        //TODO: Resend OTP
//        //Use trackingId to fetch existing one.  Check to ensure  it has not expired

//        public async Task<MessageResponse<VerificationResponse>> VerifyOtp(string mobileNumber, string trackingId, string otp)
//        {

//            //TestCases:(5); Tracking Id Valid, OTP expired, Has already been verified, OTP is not valid, Otp is valid


//            var verification = await _dbContext.Verifications.FirstOrDefaultAsync(c => c.MobileNumber.Equals(mobileNumber) && c.OtpStatus != OTPStatus.DeActivated &&

//             //  c.Otp.Equals(otp) &&
//             c.TrackingId.Equals(trackingId));


//            if (verification == null)
//            {
//                return new MessageResponse<VerificationResponse>()
//                {

//                    ResponseCode = (int)MobilityResponseCode.Failed,
//                    IsSuccessResponse = false,
//                    Message = "The tracking Id is not valid.",
//                    Result = null
//                };
//            }

//            if (verification.OtpStatus == OTPStatus.Verified)
//            {
//                return new MessageResponse<VerificationResponse>() { IsSuccessResponse = false, Message = "This request has already beeen verified." };
//            }

//            if (verification.OtpStatus == OTPStatus.TrialLimitReached)
//            {
//                return new MessageResponse<VerificationResponse>() { IsSuccessResponse = false, Message = "You have exceeded the maximum number of trials for OTP validation. Kindly resend OTP. " };
//            }


//            if (verification.ExpiryDate < DateTime.Now)
//            {


//                return new MessageResponse<VerificationResponse>() { IsSuccessResponse = false, Message = "The generated OTP has expired. Kindly resend OTP. " };

//            }

//            var messageResponse = new MessageResponse<VerificationResponse>()
//            {
//            };


//            string testOtp = "2244";//TODO: Remove the test otp

//            if (verification.Otp != otp && otp != testOtp)
//            {
//                messageResponse.Message = "The entered OTP is not valid.";
//                verification.OtpTrials += 1;
//                if (verification.OtpTrials >= 5)
//                {
//                    verification.OtpStatus = OTPStatus.TrialLimitReached;
//                    messageResponse.Message = $"The entered OTP is not valid and attempt no longer allowed. Please resend an OTP.";

//                }
//                else if (verification.OtpTrials >= 3)
//                {
//                    messageResponse.Message = $"The entered OTP is not valid. You have ${5 - verification.OtpTrials} attempt(s) left.";
//                }


//                await _dbContext.SaveChangesAsync(CancellationToken.None);
//                return messageResponse;
//            }

//            verification.DateUsed = DateTime.Now;
//            verification.OtpStatus = OTPStatus.Verified;
//            messageResponse.Message = "OK";
//            await _dbContext.SaveChangesAsync(CancellationToken.None);



//            if (verification.Purpose == VerificationPurpose.Login)
//            {
//                return await GetUserAuthTokenAsync(verification);
//            }
//            else
//            {
//                var token = await _mediatr.Send(new CreateJwtTokenCommand()
//                {
//                    JwtUserType = JwtUserType.AnonynmousUser,
//                    MobileNumber = verification.MobileNumber,
//                    UserId = verification.Id,
//                    CIF = verification.TrackingId,
//                    FirstName = "Anonymous",
//                    LastName = "Unknown",
//                    UserEmail = "anonymous@9mobility.com",
//                    UserName = verification.MobileNumber

//                });
//                return new MessageResponse<VerificationResponse>()
//                {
//                    IsSuccessResponse = true,
//                    ResponseCode = (int)MobilityResponseCode.Registration_Required,
//                    Message = $"{MobilityResponseCode.Registration_Required.GetDescription()}",
//                    Result = new VerificationResponse()
//                    {

//                        AccesssToken = $"{token}",
//                        ExpiresIn = verification.ExpiryDate,
//                        FirstName = "",
//                        LastName = ""
//                    }

//                };

//            }

//        }

//        private async Task<MessageResponse<VerificationResponse>> GetUserAuthTokenAsync(Verification verification)
//        {
//            //Check db for user and create toke n


//            var token = await _mediatr.Send(new CreateJwtTokenCommand()
//            {
//                JwtUserType = JwtUserType.RegisteredUser,
//                MobileNumber = verification.MobileNumber,
//                UserId = verification.Id,
//                CIF = verification.TrackingId,
//                FirstName = "First Name",
//                LastName = "Last Name",
//                UserEmail = "anonymous@9mobility.com",
//                UserName = verification.MobileNumber

//            });
//            return new MessageResponse<VerificationResponse>()
//            {
//                IsSuccessResponse = true,
//                ResponseCode = (int)MobilityResponseCode.Success,
//                Message = "",
//                Result = new VerificationResponse()
//                {

//                    AccesssToken = $"{token}",
//                    ExpiresIn = verification.ExpiryDate,
//                    FirstName = "",
//                    LastName = ""
//                }

//            };
//        }

//        private async Task<InitiateVerificationResponse> CreateNewVerification(string mobileNumber, VerificationPurpose verificationPurpose, string ttid = null)
//        {
//            var lease = _configuration.GetValue<int>("OTP:Lease");
//            var otp = GenerateCode();
//            var trackingId = ttid ?? $"{ Guid.NewGuid()}";
//            var verification = new Verification()
//            {
//                MobileNumber = mobileNumber,
//                DateSent = DateTime.Now,
//                ExpiryDate = DateTime.Now.AddMinutes(lease),
//                OtpStatus = OTPStatus.Initiated,
//                Purpose = verificationPurpose,
//                Otp = otp,
//                TrackingId = trackingId,
//            };
//            await _dbContext.Verifications.AddAsync(verification);
//            await _dbContext.SaveChangesAsync(CancellationToken.None);
//            return new InitiateVerificationResponse()
//            {
//                ExpiresIn = DateTime.Now.AddMinutes(lease),
//                //OTP = otp,
//                TrackingId = trackingId
//            };
//        }
//        public async Task<MessageResponse<InitiateVerificationResponse>> ResendVerificationOTP(string trackingId)
//        {

//            //TestCases:(2); Tracking Id Valid, OTP resent time reached


//            var lease = _configuration.GetValue<int>("OTP:Lease");
//            var maxRetry = _configuration.GetValue<int>("OTP:RetryMax");
//            var response = new MessageResponse<InitiateVerificationResponse>();
//            //var otp = GenerateCode();
//            var verification = await _dbContext.Verifications.SingleOrDefaultAsync(c => c.TrackingId.Equals(trackingId) && c.OtpStatus != OTPStatus.DeActivated);
//            if (verification == null)
//            {
//                response.Result = default;
//                response.Message = "Unable to retrieve the otp with the given tracking number";
//                response.ResponseCode = (int)MobilityResponseCode.NotFound;
//                return response;
//            }

//            //var timeDifferenceForRetrial= DateTime.Su. verification.DateSent.AddSeconds(60)
//            var diffInSeconds = (DateTime.Now - verification.DateSent).TotalSeconds;
//            if (diffInSeconds < 60)
//            {
//                return new MessageResponse<InitiateVerificationResponse>()
//                {
//                    IsSuccessResponse = false,
//                    Message = $"You can only resend OTP in the next {60 - diffInSeconds} second(s).",
//                    ResponseCode = (int)MobilityResponseCode.Failed

//                };
//            }



//            var createNewVerification = await CreateNewVerification(verification.MobileNumber, verification.Purpose, verification.TrackingId);

//            verification.OtpStatus = OTPStatus.DeActivated;//A new veriication will be created
//            await _dbContext.SaveChangesAsync(CancellationToken.None);


//            return new MessageResponse<InitiateVerificationResponse>()
//            {
//                IsSuccessResponse = true,
//                Message = "The One Time Password has been resent.",
//                ResponseCode = (int)MobilityResponseCode.Success,
//                Result = new InitiateVerificationResponse()
//                {
//                    ExpiresIn = verification.ExpiryDate,
//                    TrackingId = verification.TrackingId
//                }

//            };



//        }

//        public async Task<bool> CheckIfUserExists(string mobileNo)
//        {

//            return await _dbContext.Users.AnyAsync(c => c.MobileNumber.Equals(mobileNo));

//        }
//        public async Task<MessageResponse<InitiateVerificationResponse>> InitiateVerification(string mobileNo)
//        {
//            var lease = _configuration.GetValue<int>("OTP:Lease");
//            var userExists = await CheckIfUserExists(mobileNo);
//            var result = await CreateNewVerification(mobileNo, userExists ? VerificationPurpose.Login : VerificationPurpose.Onboarding);


//            return new MessageResponse<InitiateVerificationResponse>()
//            {
//                Message = $"A verification code has been sent to {mobileNo}",
//                IsSuccessResponse = true,
//                ResponseCode = (int)MobilityResponseCode.Success,
//                Result = result

//            };
//        }
//    }
//}
