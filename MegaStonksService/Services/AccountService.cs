using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Accounts;
using MegaStonksService.Services.FinancialModellingPrepApi;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace MegaStonksService.Services
{
    public interface IAccountService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        void Register(RegisterRequest model, string origin);
        void OnBoardCompleted(int acountId);
        void VerifyEmail(string token);
        void ForgotPassword(ForgotPasswordRequest model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);
        void ResetPassword(ResetPasswordRequest model);
        IEnumerable<AccountResponse> GetAll();
        AccountResponse GetById(int id);
        AccountResponse Create(CreateRequest model);
        AccountResponse Update(int id, UpdateRequest model);
        void Delete(int id);
    }

    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly IFMPApiService _fmpApiService;
        private readonly IWatchListService _watchListService;

        public AccountService(
            DataContext context,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService, IFMPApiService fmpApiService, IWatchListService watchListService)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _fmpApiService = fmpApiService;
            _watchListService = watchListService;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            if (account == null || !account.IsVerified)
                throw new AppException("Email or password is incorrect");

            //Check if account is locked
            if (account.AccountLocked && account.AccountLockTimeout <= DateTime.UtcNow)
            {
                account.AccountLocked = false;
                account.LoginAttempt = 0;
                // save changes to db
                _context.Update(account);
                _context.SaveChanges();
            }
            else if (account.AccountLocked)
            {
                throw new AppException("Your account has been locked. You are Unable to Login at this time. Please wait 1 hour before trying again.");
            }


            //Check Login Attempt and set maximum of 5 attempts then lock
            if (!BC.Verify(model.Password, account.PasswordHash))
            {
                account.LoginAttempt++;
                // save changes to db
                _context.Update(account);
                _context.SaveChanges();
                if (account.LoginAttempt >= 5)
                {
                    account.AccountLocked = true;
                    account.AccountLockTimeout = DateTime.UtcNow.AddHours(1);
                    // save changes to db
                    _context.Update(account);
                    _context.SaveChanges();
                    throw new AppException($"Your account has been locked out for the next hour. Contact us at {_appSettings.EmailFrom} if you are having issues signing in ");
                }
                throw new AppException("Email or password is incorrect");
            }

            //Reset Login count to zero after successful login
            account.LoginAttempt = 0;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(account);
            var refreshToken = generateRefreshToken(ipAddress);


            // remove old refresh tokens from account
            removeOldRefreshTokens(account);

            account.RefreshTokens.Add(refreshToken);



            // save changes to db
            _context.Update(account);
            _context.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }



        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            removeOldRefreshTokens(account);

            account.RefreshTokens.Add(newRefreshToken);

            _context.Update(account);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(account);

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(account);
            _context.SaveChanges();
        }

        public void Register(RegisterRequest model, string origin)
        {
            // validate
            if (_context.Accounts.Any(x => x.Email == model.Email))
            {
                // send already registered error in email to prevent account enumeration
                sendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }

            // map model to new account object
            var account = _mapper.Map<Account>(model);

            // first registered account is an admin
            var isFirstAccount = _context.Accounts.Count() == 0;
            account.Role = isFirstAccount ? Role.Admin : Role.User;
            account.Created = DateTime.UtcNow;
            account.VerificationToken = randomTokenString();

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            // send email
            sendVerificationEmail(account, origin);
        }

        public void VerifyEmail(string token)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

            if (account == null) throw new AppException("Verification failed");

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void OnBoardCompleted(int accountId)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Id == accountId);
            if (account == null|| account.IsOnboarded) return;

            account.IsOnboarded = true;
            decimal startupWalletAmount = 10000M; //10,000CAD to start for everyone
            
            //Check If User already has a Wallet
            if(_context.Wallets.Where(x => x.Account.Id == accountId).Any())
            {
                throw new AppException("Account Wallet already Exists");
            }
            //Create Wallet for User
            if (account.Currency == Currency.USD.ToString("G"))
            {
                //Convert CAD to USD

                //Forex is too volatile for this because one minute, this will reult in $7000USD, another minute, it would be $8000 which makes it unfair 
               //var convertedAmount = _fmpApiService.ConvertCurrency(startupWalletAmount, "CADUSD").Result.Result;
               //startupWalletAmount = (Math.Floor(convertedAmount / 1000M) * 1000M);
               startupWalletAmount = 8000M;
            }
           
            var startupWallet = new Wallet
            {
                Account = account,
                InitialAmount = startupWalletAmount,
                CurrentAmount = startupWalletAmount,
                Currency = account.Currency,
                LastUpdated = DateTime.UtcNow
            };

            //Add Some Default Stocks to Watchlist 
            //int[] stocksToAdd = new int[] { 7365, 4400, 3225, 7191, 3775, 3892 };

            //foreach(var stockId in stocksToAdd)
            //{
            //    _watchListService.AddStockToWatchList(account, stockId);
            //}


            _context.Wallets.Add(startupWallet);
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Accounts.Update(account);
            _context.SaveChanges();

            // send email
            sendPasswordResetEmail(account, origin);
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");

            // update password and remove reset token
            account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _context.Accounts;
            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public AccountResponse GetById(int id)
        {
            var account = getAccount(id);
            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Create(CreateRequest model)
        {
            // validate
            if (_context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var account = _mapper.Map<Account>(model);
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Update(int id, UpdateRequest model)
        {
            var account = getAccount(id);

            // validate
            if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BC.HashPassword(model.Password);

            // copy model to account and save
            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public void Delete(int id)
        {
            try
            {
                var account = getAccount(id);
                _context.Accounts.Remove(account);
                _context.SaveChanges();
            }
            catch(Exception)
            {
                throw;
            }

        }

        // helper methods

        private Account getAccount(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return account;
        }

        private (RefreshToken, Account) getRefreshToken(string token)
        {
            var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException("Invalid token");
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) throw new AppException("Invalid token");
            return (refreshToken, account);
        }

        private string generateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = randomTokenString(),
                Expires = DateTime.UtcNow.AddDays(10),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void removeOldRefreshTokens(Account account)
        {
            //Remove all refresh tokens and not only inactive old ones. This should prevent the user from having two active sessions after the jwt tokens have expired
            account.RefreshTokens.RemoveAll(x =>
                x.IsActive &&
                x.Created <= DateTime.UtcNow);

                // account.RefreshTokens.RemoveAll(x =>
                //!x.IsActive &&
                //x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void sendVerificationEmail(Account account, string origin)
        {
            string message;
            var verifyUrl = $"https://www.megastonks.com/verify.html?{account.VerificationToken}";
            if (!string.IsNullOrEmpty(origin))
            {
                
                message = $@"<p>Please click the link below to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">Verify Here</a></p>";
            }
            else
            {
                message = $@"<p>Please click the link below to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">Verify Here</a></p>";
            }

            _emailService.SendAsync(
                to: account.Email,
                subject: "Sign-up Verification MegaStonks - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }

        private void sendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>If you don't know your password please visit the 'Forgot Password' page on the mobile app to reset it.</p>";
            else
                message = "<p>If you don't know your password you can reset it by requesting a reset password token from the mobile application.</p>";

            _emailService.SendAsync(
                to: email,
                subject: "Sign-up Verification MegaStonks - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private void sendPasswordResetEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password in the 'Reset Password' page on the mobile app:</p>
                             <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.SendAsync(
                to: account.Email,
                subject: "Sign-up Verification MegaStonks - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
    }
}