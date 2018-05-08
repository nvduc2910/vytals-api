using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vytals.Models;
using Vytals.Infrastructures;
using Vytals.Exceptions;
using Vytals.Resources;
using Vytals.Helpers;
using Vytals.CustomAttribute;
using Vytals.Options;
using Vytals.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Vytals.Repository;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Vytals.Enums;
using Microsoft.AspNetCore.Identity;
using Vytals.Models.DTOModels;
using AutoMapper;
using Vytals.Models.ReutrnModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Vytals.Models.Entities;
using System.Collections.Generic;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vytals.Controllers
{
    /// <summary>
    /// This class is used as an api for the 
    /// requests 1.
    /// </summary>
    /// 

    [Route("api/[controller]/[action]")]
    [ValidateModel]
    [HandleException]

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly JwtIssuerOptions jwtIssuerOptions;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpCotext;
        private const int PinCodeExpirationTime = 24;
        private readonly IStringLocalizer<ValidationModel> localizerValidation;
        private readonly IStringLocalizer<Account> localizerAccount;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        #region Contructor

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration
                                 , IOptions<JwtIssuerOptions> jwtOptions, SignInManager<ApplicationUser> signInManager, IUnitOfWork unitOfWork, IHttpContextAccessor httpCotext, IStringLocalizer<ValidationModel> localizerValidation, IStringLocalizer<Account> localizerAccount, IMapper mapper, ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this._logger = logger;
            this.signInManager = signInManager;
            this.unitOfWork = unitOfWork;
            this.httpCotext = httpCotext;
            jwtIssuerOptions = jwtOptions.Value;
            this.localizerValidation = localizerValidation;
            this.localizerAccount = localizerAccount;
            _mapper = mapper;
        }

        #endregion

        #region Login

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
                throw new UserNotExistsException(localizerAccount["UserNotFound"]);

            bool result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
                throw new IncorrectPasswordException(localizerAccount["IncorrectPassword"]);

            user.DeviceToken = loginDTO.DeviceToken;

            await unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);

            _logger.LogInformation(user.Id, user.Name + " Have just login");

            return await RespondJwtTokenTo(user, false);
        }

        #endregion

        #region Register

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            ApplicationUser user = await RegisterPersonalUser(registerDTO);

            if (user == null)

                return ApiResponder.RespondFailureTo(HttpStatusCode.Ok, localizerAccount["RegisterFail"], ErrorDefine.REGISTER_FAIL);

            return await RespondJwtTokenTo(user, false);
        }

        #endregion

        #region GetProfile
        [HttpGet]
        public IActionResult GetProfile(int id)
        {
            var userId = Convert.ToInt32(userManager.GetUserId(User));

            var user = unitOfWork.GetRepository<ApplicationUser>().Get(s => s.Id == id).FirstOrDefault();

            if(user == null)
                throw new UserNotExistsException(localizerAccount["UserNotFound"]);
            
            return ApiResponder.RespondSuccessTo(HttpStatusCode.Ok, _mapper.Map<UserProfileReturnModel>(user));
        }

        #endregion


        [HttpPost]
        public async  Task<IActionResult> SetPointAndLevelUser(int level, int point, int userId)
        {
            var user = unitOfWork.GetRepository<ApplicationUser>().Get(s => s.Id == userId).FirstOrDefault();

            user.Level = level;
            user.Point = point;

            await unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);

            return ApiResponder.RespondSuccessTo(HttpStatusCode.Ok, "OK");

        }

        #region ForgotPassword
        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword(string email)
        //{
        //    ApplicationUser currentUser = await userManager.FindByEmailAsync(email);

        //    if (currentUser == null)
        //        throw new UserNotExistsException(localizerAccount["UserNotFound"]);

        //    int pinCode = new Random().Next(100000, 999999);
        //    currentUser.PinCode = pinCode;
        //    currentUser.PinCodeExpiration = DateTime.UtcNow.AddHours(PinCodeExpirationTime);

        //    IdentityResult updateUserResult = await userManager.UpdateAsync(currentUser);

        //    if (!updateUserResult.Succeeded)
        //        throw new IdentityException(updateUserResult.Errors.FirstOrDefault().Description);

        //    string message = System.IO.File.ReadAllText(@"./HtmlPages/Email.html");

        //    await emailSender.SendEmailAsync(email, "Pin Code Confirmation",
        //        String.Format(message, currentUser.UserName, pinCode.ToString()));

        //    return ApiResponder.RespondSuccessTo(HttpStatusCode.Ok, "Please check your email to setup new password");
        //}

        #endregion

        #region ResetPassword
        //[HttpPatch]
        //public async Task<IActionResult> ResetPassword(string email, string newPassword, int pinCode)
        //{
        //    ApplicationUser currentUser = await userManager.FindByEmailAsync(email);

        //    if (currentUser == null)
        //        throw new UserNotExistsException(localizerAccount["UserNotFound"]);

        //    if (currentUser.PinCode != pinCode)
        //        throw new InvalidPinCodeException(localizerAccount["InvalidPinCode"]);

        //    if (currentUser.PinCodeExpiration < DateTime.UtcNow)
        //        throw new PinCodeExpiredException(localizerAccount["PinCodeExpired"]);

        //    IdentityResult deletePasswordResult = await userManager.RemovePasswordAsync(currentUser);

        //    if (!deletePasswordResult.Succeeded)
        //        throw new IdentityException(deletePasswordResult.Errors.FirstOrDefault().Description);

        //    IdentityResult addPasswordResult = await userManager.AddPasswordAsync(currentUser, newPassword);

        //    if (!addPasswordResult.Succeeded)
        //        throw new IdentityException(addPasswordResult.Errors.FirstOrDefault().Description);

        //    return ApiResponder.RespondSuccessTo(HttpStatusCode.Ok, "Update new password sucessfully");
        //}

        #endregion

        #region Register
        [NonAction]
        private async Task<ApplicationUser> RegisterPersonalUser(RegisterDTO registerDTO)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                Name = registerDTO.Name,
                Level = 1,

            };

            IdentityResult userCreationResult = registerDTO.Password == null
                ? await userManager.CreateAsync(user)
            : await userManager.CreateAsync(user, registerDTO.Password);

            if (!userCreationResult.Succeeded)
                throw new IdentityException(userCreationResult.Errors.FirstOrDefault().Description);

            return user;
        }
        #endregion

        #region NewMedal
        [Authorize()]
        [HttpPost]
        public async Task<IActionResult> NewMedal([FromBody]Medal medals)
        {
            var userId = Convert.ToInt32(userManager.GetUserId(User));
            if (userId == 0)
                throw new UserNotExistsException(localizerAccount["UserNotFound"]);
            
            var newMadel = new UserMedal()
            {
                ApplicationUserId = userId,
                Madel = medals,
            };

            await unitOfWork.GetRepository<UserMedal>().InsertAsync(newMadel);
            return ApiResponder.RespondSuccessTo(HttpStatusCode.Ok, "Ok");
        }

        #endregion

        #region GetMedal
        [Authorize()]
        [HttpGet]
        public IActionResult GetMedal()
        {
            var userId = Convert.ToInt32(userManager.GetUserId(User));
            if(userId == 0)
                throw new UserNotExistsException(localizerAccount["UserNotFound"]);
            
            var medals = unitOfWork.GetRepository<UserMedal>().Get(s => s.ApplicationUserId == userId).ToList();
            List<UserMedalReturnModel> medalReturn = _mapper.Map<List<UserMedal>, List<UserMedalReturnModel>>(medals);
            return ApiResponder.RespondSuccessTo(HttpStatusCode.Ok, medalReturn);
        }

        #endregion

        #region GetJwtToken
        [NonAction]
        private async Task<ObjectResult> RespondJwtTokenTo(ApplicationUser user, bool isFirstTime)
        {
            
            ClaimsPrincipal principal = await signInManager.CreateUserPrincipalAsync(user);
            string encodedJwt = JwtEncoder.EncodeSecurityToken(jwtIssuerOptions, principal);



            return ApiResponder.RespondSuccessTo(HttpStatusCode.Ok, new
            {
                access_token = encodedJwt,
                user_id = user.Id,
                email = user.Email,
                level = user.Level,
            });
        }

        #endregion
    }
}
