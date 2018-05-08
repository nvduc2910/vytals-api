using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vytals.CustomAttribute;
using Vytals.Helpers;
using Vytals.Infrastructures;
using Vytals.Models;
using Vytals.Repository;
using Vytals.Services.Grandes;
using Vytals.Services.Grandes.AdditionMathGrandeQuestions;
using Vytals.Services.Grandes.DivisionMathGrandeQuestions;
using Vytals.Services.Grandes.MultiplicaitonMathGrandeQuestions;
using Vytals.Services.Grandes.SubtractionMathGrandeQuestions;
using Vytals.Services.QuestionStores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vytals.Controllers
{
    [Route("api/[controller]/[action]")]
    [ValidateModel]
    [HandleException]
    public class GradeController : BaseController
    {
        private static List<IGrandeQuestionStore> _grandeQuestionStore;

        public GradeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpCotext) : base(unitOfWork, userManager, httpCotext)
        {
            if(_grandeQuestionStore == null)
            {
                _grandeQuestionStore = new List<IGrandeQuestionStore>();
                _grandeQuestionStore.Add(new AdditionMathGrande1Questions());
                _grandeQuestionStore.Add(new DivisionMathGrande1Questions());
                _grandeQuestionStore.Add(new MultiplicationMathGrande1Questions());
                _grandeQuestionStore.Add(new SubtractionMathGrande1Questions());
            }

        }


        #region GetGradeQuestion
        [HttpGet]
        public IActionResult GetGradeQuestion(int grandNumber, string mathType)
        {
            var questions = _grandeQuestionStore.Find(s => s.Grande(grandNumber) && s.MathType(mathType))?.GennerateAnwersQuestion();

            if (questions == null)
                return ApiResponder.RespondFailureTo(HttpStatusCode.Ok, "Can not generate question", ErrorDefine.CAN_NOT_FIND_GRANDE_QUESTION);

            return ApiResponder.RespondSuccessTo(HttpStatusCode.Ok, questions);

        }

        #endregion
    }
}
