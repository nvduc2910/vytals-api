using System;
namespace Vytals
{
    public static class ErrorDefine
    {
        public const int GENERAL_CODE = 5000;
        public const int UNAUTHORIZE = 4001;
      
        //Validation 
        public const int INVALID_MODEL = 1111;
        public const int SIGNALR_ERROR = 5001;

        // account 
        public const int USER_NOT_FOUND = 1004;
        public const int REGISTER_FAIL = 1003;
        public const int INCORECT_PASSWORD = 1005;
        public const int INVALID_PIN_CODE = 1006;
        public const int PIN_CODE_EXPIRED = 1007;

        //Game
        public const int START_GAME_FAIL = 3000;
        public const int FINISH_GAME_FAIL = 3001;
        public const int CAN_NOT_FIND_GRANDE_QUESTION = 3002;
    }

    public static class SucceedCodeDefine
    {
        public const int START_GAME_OK = 2000;
    }



}
