using Vytals.Enums;
using Vytals.Infrastructures;
using Vytals.Models;
using Vytals.Repository;
using Vytals.Services.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Vytals.Services.QuestionStores;
using Vytals.Services.Math;
using AutoMapper;
using Vytals.Models.ReutrnModels;
using Vytals.Resources;
using Microsoft.Extensions.Localization;
using Vytals.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;

namespace Vytals.Hubs
{
    [HubName("GameHub")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class GameHub : Hub
    {
        #region Private Fields

        private readonly IMapper _mapper;
        private IHttpContextAccessor _contextAccessor;
        private static List<UserConnection> users = new List<UserConnection>();
        private static List<LevelPoint> LevelPoints = new List<LevelPoint>();
        private VfDbContext _dataContext = null;
        private readonly IStringLocalizer<Account> _localizerAccount;
        // private readonly ILogger _logger;
        ILogger _logger;

        private TelemetryClient telemetry = new TelemetryClient();

        protected VfDbContext DataContext
        {
            get
            {
                if (_dataContext != null)
                {
                    return _dataContext;
                }
                else
                {
                    var options = new DbContextOptionsBuilder<VfDbContext>();
                    options.UseSqlServer("Server=tcp:beauty-advisor.database.windows.net,1433;Initial Catalog=LocalSkills;Persist Security Info=False;User ID=beautyadvisor;Password=Sofus71204;MultipleActiveResultSets=False;TrustServerCertificate=False;Connection Timeout=30;");
                    _dataContext = new VfDbContext(options.Options);
                    return _dataContext;
                }

            }
        }

        private List<IQuestionStores> _questionStoreService;
        // private List<ICalculation> _calculationService;

        #endregion

        #region Contructors
        public GameHub(IHttpContextAccessor contextAccessor, IMapper mapper, IStringLocalizer<Account> localizerAccount, ILogger<GameHub> logger)
        {
            this._contextAccessor = contextAccessor;
            this._localizerAccount = localizerAccount;
            _mapper = mapper;
            _logger = logger;

            GenLevel();

            // Init question services
            if (_questionStoreService == null)
            {
                _questionStoreService = new List<IQuestionStores>();

                _questionStoreService.Add(new AdditionThreeNumberLv1_12345QuestionService());
                _questionStoreService.Add(new AdditionThreeNumberLv2_678910QuestionService());
                _questionStoreService.Add(new AdditionThreeNumberLv3_1112131415QuestionService());
                _questionStoreService.Add(new AdditionThreeNumberLv4_1617181920QuestionService());
                _questionStoreService.Add(new AdditionThreeNumberLv5_2122232425QuestionService());
                _questionStoreService.Add(new AdditionThreeNumberLv6_2627282930QuestionService());

                _questionStoreService.Add(new MultiplicationThreeNumberLv1_12345QuestionService());
                _questionStoreService.Add(new MultiplicationThreeNumberLv2_678910QuestionService());
                _questionStoreService.Add(new MultiplicationThreeNumberLv3_1112131415QuestionService());
                _questionStoreService.Add(new MultiplicationThreeNumberLv4_1617181920QuestionService());
                _questionStoreService.Add(new MultiplicationThreeNumberLv5_2122232425QuestionService());
                _questionStoreService.Add(new MultiplicationThreeNumberLv6_2627282930QuestionService());


                _questionStoreService.Add(new SubtractionThreeNumberLv1_12345QuestionService());
                _questionStoreService.Add(new SubtractionThreeNumberLv2_678910QuestionService());
                _questionStoreService.Add(new SubtractionThreeNumberLv3_112131415QuestionService());
                _questionStoreService.Add(new SubtractionThreeNumberLv4_1617181920QuestionService());
                _questionStoreService.Add(new SubtractionThreeNumberLv5_212232425QuestionService());
                _questionStoreService.Add(new SubtractionThreeNumberLv6_2627282930QuestionService());


                // Division services
                _questionStoreService.Add(new DivisionThreeNumberLv1_12345QuestionService());
                _questionStoreService.Add(new DivisionThreeNumberLv2_678910QuestionService());
                _questionStoreService.Add(new DivisionThreeNumberLv3_1112131415QuestionService());
                _questionStoreService.Add(new DivisionThreeNumberLv4_1617181920QuestionService());
                _questionStoreService.Add(new DivisionThreeNumberLv5_2122232425QuestionService());
                _questionStoreService.Add(new DivisionThreeNumberLv5_2627282930QuestionService());

            
            }

        }
        #endregion

        #region Setup

        public override Task OnConnected()
        {
            try
            {
                var userId = Convert.ToInt32(_contextAccessor.HttpContext.Request.Headers["Authorization"]);
                var connectId = Context.ConnectionId;
                var userInfo = DataContext.Users.Where(s => s.Id == userId).FirstOrDefault();

                Console.WriteLine("Connected: " + userId);

                var userConnect = new UserConnection()
                {
                    Id = userId,
                    Level = userInfo.Level,
                    ConnectionId = connectId,
                    State = UserState.Online,
                    Point = userInfo.Point,
                };

                var enemyUser = users.Find(s => s.Id == userInfo.EnemyId);

                // get current point and question after re-connect.
                if (userInfo.EnemyId != 0 && enemyUser != null && enemyUser.State == UserState.Playing)
                {
                    userConnect.CurrentQuestionNumber = userInfo.CurrentQuestionNumber;
                    userConnect.CurrentPointInGame = userInfo.CurrentPointInGame;
                    userConnect.MathType = userInfo.MathType;
                    userConnect.EnemyId = userInfo.EnemyId;
                    userConnect.Point = userInfo.Point;
                }
                else 
                {
                    userConnect.CurrentQuestionNumber = 0;
                    userConnect.CurrentPointInGame = 0;
                    userConnect.MathType = null;
                    userConnect.EnemyId = 0;
                    userConnect.Point = userInfo.Point;
                }

                var itemUser = users.Find(s => s.ConnectionId == connectId);
                if (itemUser == null)
                {
                    users.Add(userConnect);
                }
                return base.OnConnected();
            }
            catch (Exception ex)
            {
                return base.OnConnected();
            }
        }


        public override Task OnReconnected()
        {
            var userId = Convert.ToInt32(_contextAccessor.HttpContext.Request.Headers["Authorization"]);
            var connectId = Context.ConnectionId;
            var userInfo = DataContext.Users.Where(s => s.Id == userId).FirstOrDefault();


            Console.WriteLine("Connected: " + userId);

            var userConnect = new UserConnection()
            {
                Id = userId,
                Level = userInfo.Level,
                ConnectionId = connectId,
                CurrentQuestionNumber = userInfo.CurrentQuestionNumber,
                State = userInfo.State,
                Point = userInfo.Point,

            };

            var enemyUser = users.Find(s => s.Id == userInfo.EnemyId);

            // get current point and question after re-connect.
            if (userInfo.EnemyId != 0 && enemyUser != null && enemyUser.State == UserState.Playing)
            {
                userConnect.CurrentQuestionNumber = userInfo.CurrentQuestionNumber;
                userConnect.CurrentPointInGame = userInfo.CurrentPointInGame;
                userConnect.MathType = userInfo.MathType;
                userConnect.EnemyId = userInfo.EnemyId;
                userConnect.Point = userInfo.Point;
            }
            else
            {
                userConnect.CurrentQuestionNumber = 0;
                userConnect.CurrentPointInGame = 0;
                userConnect.MathType = null;
                userConnect.EnemyId = 0;
                userConnect.Point = userInfo.Point;
            }

            var itemUser = users.Find(s => s.ConnectionId == connectId);
            if (itemUser == null)
            {
                users.Add(userConnect);
            }
            return base.OnReconnected();

        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = _contextAccessor.HttpContext.Request.Headers["Authorization"];
            var connectId = Context.ConnectionId;

            var myUser = users.Find(s => s.ConnectionId == connectId);
            Console.WriteLine("Disconnect: " + userId);

            // save last info to database.

            var userInfo = DataContext.Users.Where(s => s.Id == myUser.Id).FirstOrDefault();

            if (userInfo.EnemyId != 0)
            {
                userInfo.EnemyId = myUser.EnemyId;
                userInfo.CurrentQuestionNumber = myUser.CurrentQuestionNumber;
                userInfo.CurrentPointInGame = myUser.CurrentPointInGame;
                userInfo.MathType = myUser.MathType;

                DataContext.Users.Update(userInfo);
                DataContext.SaveChanges();
            }


            //Remove from list are activing.
            if (myUser != null)
            {
                users.Remove(myUser);
            }
            return base.OnDisconnected(stopCalled);
        }
        #endregion

        #region StartGame

        public async Task<ApiResponder> StartGame(string mathType)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                var connectId = Context.ConnectionId;
                var myUser = users.Find(s => s.ConnectionId == connectId);
                var indexOfMyUser = users.IndexOf(myUser);
                users[indexOfMyUser].State = UserState.Ready;
                users[indexOfMyUser].MathType = mathType;


                var enemyUser = users.Where(s => ((s.Level - myUser.Level >= 0 && s.Level - myUser.Level <= 3) || (myUser.Level - s.Level >= 0 && myUser.Level - s.Level <= 3)) && s.Id != myUser.Id && s.State == UserState.Ready && s.MathType.Equals(mathType)).FirstOrDefault();

                if (enemyUser == null)
                {
                    await Clients.Client(myUser.ConnectionId).waitingForFindingEnemy(true);

                    Console.WriteLine(myUser.Id + " is finding enemy");
                    //var logger = _loggerFactory.CreateLogger("LoggerCategory");
                    _logger.LogInformation(myUser.Id + " is finding enemy");
                }
                else
                {
                    // random question

                    var questions = _questionStoreService.Find(s => s.Level(myUser.Level) && s.MathType(mathType))?.GennerateAnwersQuestion();

                    users[indexOfMyUser].State = UserState.Playing;
                    //users[indexOfMyUser].QuestionAndAwsers = questions;


                    var indexOfEnemyUser = users.IndexOf(enemyUser);
                    users[indexOfEnemyUser].State = UserState.Playing;
                    //users[indexOfEnemyUser].QuestionAndAwsers = questions;

                    await Clients.Clients(new[] { myUser.ConnectionId, enemyUser.ConnectionId }).waitingForFindingEnemy(false);

                    _logger.LogInformation(myUser.Id, "Start game for userId " + myUser.Id + " and " + enemyUser.Id);

                    var myUserInfo = DataContext.Users.Where(s => s.Id == myUser.Id).FirstOrDefault();
                    var enemyUserInfo = DataContext.Users.Where(s => s.Id == enemyUser.Id).FirstOrDefault();

                    var myUserIndex = users.IndexOf(myUser);
                    users[myUserIndex].EnemyId = enemyUser.Id;
                    users[myUserIndex].State = UserState.Playing;
                    users[myUserIndex].CurrentQuestionNumber = 1;

                    var enemyIndex = users.IndexOf(enemyUser);
                    users[enemyIndex].EnemyId = myUser.Id;
                    users[enemyIndex].State = UserState.Playing;
                    users[enemyIndex].CurrentQuestionNumber = 1;

                    await Clients.Client(myUser.ConnectionId).getEnemyInfo(_mapper.Map<UserProfileReturnModel>(enemyUserInfo));
                    await Clients.Client(enemyUser.ConnectionId).getEnemyInfo(_mapper.Map<UserProfileReturnModel>(myUserInfo));

                    //var questionForCurrenyLevel = _questionStoreService.Find(s => s.Level(enemyUser.Level) && s.MathType(mathType));

                    await Clients.Clients(new[] { myUser.ConnectionId, enemyUser.ConnectionId }).getQuestion(questions);
                }


                stopwatch.Stop();

                var metrics = new Dictionary<string, double>
                        {{"processingTime", stopwatch.Elapsed.TotalMilliseconds}};

                // Set up some properties:
                var properties = new Dictionary<string, string>
                        {{"signalSource", "GameHub"}};

                // Send the event:
                telemetry.TrackEvent("StartGame", properties, metrics);

                return new ApiResponder(true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponder(null, new Error() { errorCode = ErrorDefine.START_GAME_FAIL, errorMessage = ex.StackTrace });
            }
        }
        #endregion

        #region NextQuestion

        public async Task<ApiResponder> NextQuestion(string mathType)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                var connectId = Context.ConnectionId;
                var myUser = users.Find(s => s.ConnectionId == connectId);
                var indexOfMyUser = users.IndexOf(myUser);
                users[indexOfMyUser].CurrentQuestionNumber += 1;

                _logger.LogInformation("Select next question for userId: " + myUser.Id);

                var questionForCurrenyLevel = _questionStoreService.Find(s => s.Level(myUser.Level) && s.MathType(mathType));
                //var item = questionForCurrenyLevel.GennerateAnwersQuestion().BinarySearch()
                //await Clients.Client(myUser.ConnectionId).getQuestion(users[indexOfMyUser].QuestionAndAwsers[myUser.CurrentQuestionNumber]);

                _logger.LogInformation("Send next question for userId: " + myUser.Id);

                stopwatch.Stop();

                var metrics = new Dictionary<string, double>
                        {{"processingTime", stopwatch.Elapsed.TotalMilliseconds}};

                // Set up some properties:
                var properties = new Dictionary<string, string>
                        {{"signalSource", "GameHub"}};

                // Send the event:
                telemetry.TrackEvent("NextQuestion", properties, metrics);

                return new ApiResponder(true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponder(null, new Error() { errorCode = ErrorDefine.START_GAME_FAIL, errorMessage = ex.StackTrace });
            }
        }

        #endregion

        #region ChoooseAnwer

        public async Task<ApiResponder> ChooseAnwers(bool anwers)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();


                var connectId = Context.ConnectionId;
                var myUser = users.Find(s => s.ConnectionId == connectId);
                var indexOfMyUser = users.IndexOf(myUser);

                var enemyUser = users.Find(s => s.Id == myUser.EnemyId);

                if (anwers == true)
                    users[indexOfMyUser].CurrentPointInGame += 10;

                else if (anwers == false && users[indexOfMyUser].CurrentPointInGame > 0)
                    users[indexOfMyUser].CurrentPointInGame -= 5;

                if (enemyUser != null)

                    await Clients.Client(enemyUser.ConnectionId).getEnemyPoint(users[indexOfMyUser].CurrentPointInGame);

                var metrics = new Dictionary<string, double>
                        {{"processingTime", stopwatch.Elapsed.TotalMilliseconds}};

                // Set up some properties:
                var properties = new Dictionary<string, string>
                        {{"signalSource", "GameHub"}};

                // Send the event:
                telemetry.TrackEvent("ChooseAnwers", properties, metrics);

                return new ApiResponder(true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponder(null, new Error() { errorCode = ErrorDefine.START_GAME_FAIL, errorMessage = ex.StackTrace });
            }
        }


        #endregion

        #region FinishGame

        public async Task<ApiResponder> FinishGame()
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                var connectId = Context.ConnectionId;
                var myUser = users.Find(s => s.ConnectionId == connectId);

                ChangeUserState(myUser.Id, UserState.Finish);

                var enemyUser = users.Where(s => s.Id == myUser.EnemyId && s.State == UserState.Finish).FirstOrDefault();

                if (enemyUser == null)
                {
                    var levelMyUserData = new
                    {
                        EnemyPointInGame = -1,
                        IsUpLevel = false,
                        IsDownLevel = false,
                    };
                    await Clients.Client(myUser.ConnectionId).getFinishGame(levelMyUserData);
                }
                else
                {
                    // Check who is winer and cacual level.
                    // if enemy is the winer
                    if(enemyUser.CurrentPointInGame > myUser.CurrentPointInGame)
                    {
                        enemyUser.Point += ConstantDefine.POINT_FOR_WIN;
                        if(myUser.Point > 0)
                            myUser.Point -= ConstantDefine.POINT_FOR_LOSSS;


                        var enemyLevel = LevelPoints[enemyUser.Level];
                        var mylevel = LevelPoints[myUser.Level - 1];


                        var levelEnemyUserData = new
                        {
                            EnemyPointInGame = myUser.CurrentPointInGame,
                            Level = mylevel != null && myUser.Point < mylevel.Point ? myUser.Level - 1 : myUser.Level ,
                            IsUpLevel = enemyLevel != null && enemyUser.Point >= enemyLevel.Point && enemyUser.Level != ConstantDefine.MAX_LEVEL ? true : false,
                            IsDownLevel = false,
                      
                        };
                        await Clients.Client(enemyUser.ConnectionId).getFinishGame(levelEnemyUserData);

                        var levelMyUserData = new
                        {
                            EnemyPointInGame = enemyUser.CurrentPointInGame,
                            Level = enemyLevel != null && enemyUser.Point >= enemyLevel.Point && enemyUser.Level != ConstantDefine.MAX_LEVEL ? enemyUser.Level + 1 : enemyUser.Level,
                            IsUpLevel = false,
                            IsDownLevel = mylevel != null && myUser.Point < mylevel.Point ? true : false,
                        
                        };
                        await Clients.Client(myUser.ConnectionId).getFinishGame(levelMyUserData);


                        if (levelEnemyUserData.IsUpLevel == true)
                            enemyUser.Level += 1;

                        if (levelMyUserData.IsDownLevel == true && myUser.Level > 1)
                            myUser.Level -= 1;
                    }

                    //if my user is the winer
                    else if(myUser.CurrentPointInGame > enemyUser.CurrentPointInGame)
                    {
                        myUser.Point += ConstantDefine.POINT_FOR_WIN;

                        if(enemyUser.Point > 0)
                            enemyUser.Point -= ConstantDefine.POINT_FOR_LOSSS;

                        // check is uplevel for the winer
                        var mylevel = LevelPoints[myUser.Level];
                        var enemyLevel = LevelPoints[enemyUser.Level - 1];


                        var levelMyUserData = new
                        {
                            EnemyPointInGame = enemyUser.CurrentPointInGame,
                            Level = enemyLevel != null && enemyUser.Point < enemyLevel.Point ? enemyUser.Level - 1 : enemyUser.Level,
                            IsUpLevel = mylevel != null && myUser.Point >= mylevel.Point && myUser.Level != ConstantDefine.MAX_LEVEL ? true : false,
                            IsDownLevel = false,
                         
                        };
                        await Clients.Client(myUser.ConnectionId).getFinishGame(levelMyUserData);

                        // check up level for who loss

                        var levelEnemyUserData = new
                        {
                            EnemyPointInGame = myUser.CurrentPointInGame,
                            Level = mylevel != null && myUser.Point >= mylevel.Point && myUser.Level != ConstantDefine.MAX_LEVEL ? myUser.Level + 1 : myUser.Level,
                            IsUpLevel = false,
                            IsDownLevel = enemyLevel != null && enemyUser.Point < enemyLevel.Point ? true : false,

                        };
                        await Clients.Client(enemyUser.ConnectionId).getFinishGame(levelEnemyUserData);


                        if (levelMyUserData.IsUpLevel == true)
                            myUser.Level += 1;

                        if (levelEnemyUserData.IsDownLevel == true && enemyUser.Level > 1)
                            enemyUser.Level -= 1;


                    }
                    else if(enemyUser.CurrentPointInGame == myUser.CurrentPointInGame)
                    {
                        myUser.Point += ConstantDefine.POINT_FOR_DRAW;
                        enemyUser.Point += ConstantDefine.POINT_FOR_DRAW;

                        var mylevel = LevelPoints[myUser.Level];
                        var enemyLevel = LevelPoints[enemyUser.Level];


                        var levelMyUserData = new
                        {
                            EnemyPointInGame = enemyUser.CurrentPointInGame,
                            Level = enemyLevel != null && enemyUser.Point >= enemyLevel.Point && enemyUser.Level != ConstantDefine.MAX_LEVEL ? enemyUser.Level + 1 : enemyUser.Level ,
                            IsUpLevel = mylevel != null && myUser.Point >= mylevel.Point && myUser.Level != ConstantDefine.MAX_LEVEL ? true : false,
                            IsDownLevel = false,

                        };
                        await Clients.Client(myUser.ConnectionId).getFinishGame(levelMyUserData);



                        var levelEnemyUserData = new
                        {
                            EnemyPointInGame = myUser.CurrentPointInGame,
                            Level = mylevel != null && myUser.Point >= mylevel.Point && myUser.Level != ConstantDefine.MAX_LEVEL ? myUser.Level + 1 : myUser.Level,
                            IsUpLevel = enemyLevel != null && enemyUser.Point >= enemyLevel.Point && enemyUser.Level != ConstantDefine.MAX_LEVEL ? true : false,
                            IsDownLevel = false,

                        };
                        await Clients.Client(enemyUser.ConnectionId).getFinishGame(levelEnemyUserData);

                        if (levelMyUserData.IsUpLevel == true)
                            myUser.Level += 1;

                        if (levelEnemyUserData.IsUpLevel == true)
                            enemyUser.Level += 1;
                    }


                    UpdatePointAndLevelToDB(myUser.Id, myUser.Point, myUser.Level);
                    UpdatePointAndLevelToDB(enemyUser.Id, enemyUser.Point, enemyUser.Level);

                    ResetUserDataWhenFinishGameOrGiveUp(myUser.Id);
                    ResetUserDataWhenFinishGameOrGiveUp(enemyUser.Id);

                }

                var metrics = new Dictionary<string, double>
                        {{"processingTime", stopwatch.Elapsed.TotalMilliseconds}};

                // Set up some properties:
                var properties = new Dictionary<string, string>
                        {{"signalSource", "GameHub"}};

                // Send the event:
                telemetry.TrackEvent("FinishGame", properties, metrics);

                return new ApiResponder(true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponder(null, new Error() { errorCode = ErrorDefine.FINISH_GAME_FAIL, errorMessage = ex.StackTrace });
            }

        }
        #endregion

        #region ReceivedQuestion

        public async Task<ApiResponder> ReceivedQuestion()
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                var connectId = Context.ConnectionId;
                var myUser = users.Find(s => s.ConnectionId == connectId);
                ChangeUserState(myUser.Id, UserState.ReceivedQuestion);

                var enemyUser = users.Where(s => s.Id == myUser.EnemyId && s.State == UserState.ReceivedQuestion).FirstOrDefault();

                if (enemyUser == null)

                    await Clients.Client(myUser.ConnectionId).getReceivedQuestion(false);

                else
                {

                    await Clients.Client(myUser.ConnectionId).getReceivedQuestion(true);
                    await Clients.Client(enemyUser.ConnectionId).getReceivedQuestion(true);

                    ChangeUserState(myUser.Id, UserState.Playing);
                    ChangeUserState(enemyUser.Id, UserState.Playing);
                }

                var metrics = new Dictionary<string, double>
                        {{"processingTime", stopwatch.Elapsed.TotalMilliseconds}};

                // Set up some properties:
                var properties = new Dictionary<string, string>
                        {{"signalSource", "GameHub"}};

                // Send the event:
                telemetry.TrackEvent("FinishGame", properties, metrics);

                return new ApiResponder(true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponder(null, new Error() { errorCode = ErrorDefine.FINISH_GAME_FAIL, errorMessage = ex.StackTrace });
            }

        }
        #endregion

        #region GiveUp

        public async Task<ApiResponder> GiveUp()
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                var connectId = Context.ConnectionId;

                var myUser = users.Find(s => s.ConnectionId == connectId);
                var enemyUser = users.Find(s => s.Id == myUser.EnemyId);

                if(myUser.Point > 0)
                    myUser.Point -= ConstantDefine.POINT_FOR_LOSSS;
                
                enemyUser.Point += ConstantDefine.POINT_FOR_WIN;
               

                // return enemy info for myUser and return level point .

                //var mylevel = LevelPoints.Find(s => s.Point == myUser.Point || (s.Point - myUser.Point) == 500);

                var mylevel = LevelPoints[myUser.Level - 1];
                var enemyLevel = LevelPoints[enemyUser.Level];


                var levelMyUserData = new
                {
                    EnemyPointInGame = enemyUser.CurrentPointInGame,
                    Level = enemyLevel != null && enemyUser.Point >= enemyLevel.Point && enemyUser.Level != ConstantDefine.MAX_LEVEL ? enemyUser.Level + 1 : enemyUser.Level,
                    IsUpLevel = false,
                    IsDownLevel = mylevel != null && myUser.Point < mylevel.Point ? true : false,
                    UserIdGiveUp = myUser.Id,
                };


                await Clients.Client(myUser.ConnectionId).getEnemyGiveUp(levelMyUserData);

                // return myuser info for enemy and return level point 



                var levelEnemyUserData = new
                {
                    EnemyPointInGame = myUser.CurrentPointInGame,
                    Level = mylevel != null && myUser.Point < mylevel.Point ? myUser.Level - 1 : myUser.Level,
                    IsUpLevel = enemyLevel != null && enemyUser.Point >= enemyLevel.Point && enemyUser.Level != ConstantDefine.MAX_LEVEL ? true : false,
                    IsDownLevel = false,
                    UserIdGiveUp = myUser.Id,
                };



                await Clients.Client(enemyUser.ConnectionId).getEnemyGiveUp(levelEnemyUserData);

                if (levelMyUserData.IsDownLevel == true && myUser.Level > 1)
                    myUser.Level -= 1;

                if (levelEnemyUserData.IsUpLevel == true)
                    enemyUser.Level += 1;
             

                UpdatePointAndLevelToDB(myUser.Id, myUser.Point, myUser.Level);
                UpdatePointAndLevelToDB(enemyUser.Id, enemyUser.Point, enemyUser.Level);

                if (myUser != null)
                {
                    ResetUserDataWhenFinishGameOrGiveUp(enemyUser.Id);
                }
                ResetUserDataWhenFinishGameOrGiveUp(myUser.Id);


                var metrics = new Dictionary<string, double>
                        {{"processingTime", stopwatch.Elapsed.TotalMilliseconds}};

                // Set up some properties:
                var properties = new Dictionary<string, string>
                        {{"signalSource", "GameHub"}};

                // Send the event:
                telemetry.TrackEvent("GiveUp", properties, metrics);

                return new ApiResponder(true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponder(null, new Error() { errorCode = ErrorDefine.FINISH_GAME_FAIL, errorMessage = ex.StackTrace });
            }
        }
        #endregion

        #region CancelFindingEnemy

        public async Task<bool> CancelFindingEnemy()
        {
            try
            {
                var connectId = Context.ConnectionId;
                var myUser = users.Find(s => s.ConnectionId == connectId);

                ResetUserDataWhenFinishGameOrGiveUp(myUser.Id);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        #endregion

        #region ResetUserDateWhenFinishGame

        public void ResetUserDataWhenFinishGameOrGiveUp(int id)
        {
            // reset in memory

            users.Find(s => s.Id == id).State = UserState.Online;
            users.Find(s => s.Id == id).CurrentPointInGame = 0;
            users.Find(s => s.Id == id).CurrentQuestionNumber = 0;
            users.Find(s => s.Id == id).EnemyId = 0;
            users.Find(s => s.Id == id).MathType = string.Empty;


            //reset in database 

            var userInfo = DataContext.Users.Where(s => s.Id == id).FirstOrDefault();
            userInfo.EnemyId = 0;
            userInfo.CurrentQuestionNumber = 0;
            userInfo.CurrentPointInGame = 0;
            userInfo.MathType = string.Empty;

            DataContext.Users.Update(userInfo);
            DataContext.SaveChanges();

        }
        #endregion

        #region ChangeUserState

        public void ChangeUserState(int id, UserState userState)
        {
            users.Find(s => s.Id == id).State = userState;
        }

        #endregion

        #region UpdatePointAndLevelToDB

        public void UpdatePointAndLevelToDB(int userId, int point, int level)
        {
            var userInfo = DataContext.Users.Where(s => s.Id == userId).FirstOrDefault();

            if (point > 0)
                userInfo.Point = point;
            else
                userInfo.Point = 0;
            
            userInfo.Level = level;

            DataContext.Users.Update(userInfo);
            DataContext.SaveChanges();
        }

        #endregion

        #region GetMoreQuestion

        public async Task GetMoreQuestion(string mathType)
        {
            var connectId = Context.ConnectionId;
            var myUser = users.Find(s => s.ConnectionId == connectId);
            var questions = _questionStoreService.Find(s => s.Level(myUser.Level) && s.MathType(mathType)).GennerateAnwersQuestion();
            await Clients.Client(connectId).getQuestion(questions);
        }

        #endregion

        #region Methods

        public void GenLevel()
        {
            for (int i = 0; i < 31; i++)
            {
                var point = i * ConstantDefine.POINT_LEVEL;
                LevelPoints.Add(new LevelPoint() { Level = i + 1, Point = point});
            }
        }

        #endregion
    }

    public class UserConnection
    {
        public int Id { set; get; }
        public int EnemyId { get; set; }
        public string ConnectionId { set; get; }
        public int Level { get; set; }
        public UserState State { get; set; }
        public int CurrentQuestionNumber { get; set; }
        public int Point { get; set; }
        public int CurrentPointInGame { get; set; }
        public string MathType;
        //public  List<object> QuestionAndAwsers = new List<object>();
    }

    public class LevelPoint
    {
        public int Point { get; set; }
        public int Level { get; set; }
    }
}
