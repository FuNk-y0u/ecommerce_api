using ecommerce.Interfaces;
using ecommerce.Models;
using ecommerce.Models.Dto;
using ecommerce.Repository;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Controllers
{
    [Route("/")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly dbcontext _context;
        private readonly ITokenService _tokenService;
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(dbcontext context, ITokenService tokenService, IAccountService accountService, IEmailService emailService, ILogger<AccountController> logger)
        {
            this._context = context;
            this._tokenService = tokenService;
            this._accountService = accountService;
            this._emailService = emailService;
            this._logger = logger;

        }
        private async Task<bool> UserExist(string email)
        {
            return _context.Users.Any(u => u.Email== email);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResDto>> login(LoginDto user)
        {

            var UsrQuery = _context.Users.SingleOrDefault(x => x.Email == user.Email);
            if (UsrQuery == null)
            {
                return NotFound("User not found");
            }
            var Hmac = new HMACSHA512(UsrQuery.PasswordSalt);
            var ComputedHash = Hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            for (int i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != UsrQuery.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }

            }
            if(UsrQuery.EmailVerified == true)
            {
                _logger.LogInformation($"{user.Email} logged in");
                return new UserResDto
                {
                    Email = UsrQuery.Email,
                    Token = _tokenService.CreateToken(UsrQuery)
                };
            }
            return Unauthorized("Please check your email for verification link");
            

        }

        [HttpPost("signup")]
        public async Task<ActionResult<UserResDto>> Register(SignupDto user)
        {
            if(await UserExist(user.Email))
            {
                return Conflict("User already exists");
            }

            var Hmac = new HMACSHA512();
            var RoleQuery = _context.Roles.SingleOrDefault(x => x.Id == user.Role);

            if (RoleQuery == null)
            {
                return NotFound("Specified role doesnot exist ");
            }

            var res = _accountService.CreateAccount(_context, user.Email, user.Username, user.Password, RoleQuery, false);
            var verificationEmail = user.Email;
            var verificationToken = _tokenService.VerificationGenerateEmail(user);
            var baseUri = $"{Request.Scheme}://{Request.Host}/emailverify/";
            var verificationLink = baseUri + verificationToken;
            _emailService.send_mail(user.Email, "account verification", verificationLink);
            _logger.LogInformation($"{user.Email} account created");
            return Ok("Account sucessfully created please verify your account through ur provided email to login");

            
        }

        [HttpGet("emailverify/{token}")]
        public async Task<ActionResult> emailVerify(string token)
        {
            string usremail = _tokenService.ValidateVerifyToken(token);
            if (usremail == null)
            {
                return Forbid("Invalid verify token");
            }
            var userQuery = await _context.Users.SingleOrDefaultAsync(x=>x.Email == usremail);
            if (userQuery == null)
                return NotFound("Woops! looks like the cookie monster ate your account!");
            userQuery.EmailVerified = true;
            _context.SaveChanges();
            _logger.LogInformation($"{userQuery.Email} verified");
            return Ok("Sucessfully verified account");
        }

    }
}
