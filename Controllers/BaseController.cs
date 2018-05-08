using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vytals.Repository;
using Microsoft.AspNetCore.Identity;
using Vytals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vytals.Controllers
{
    /// <summary>
    /// Base Controller
    /// </summary>
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        public StringValues token;
        /// <summary>
        /// Unit of work instance
        /// </summary>
        public IUnitOfWork unitOfWork;
        /// <summary>
        /// usermanager instance
        /// </summary>
        public UserManager<ApplicationUser> userManager;
        /// <summary>
        /// httpContext instance
        /// </summary>
        public IHttpContextAccessor httpContext;

        /// <summary>
        /// Contructor of base controller
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="userManager"></param>
        public BaseController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpCotext)
        {
            this.httpContext = httpCotext;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        [NonAction]
        public string HeaderLanguage()
        {
            var language = this.httpContext.HttpContext.Request?.Headers["Accept-Language"];
            return language.ToString();
        }


    }
}
