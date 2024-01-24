using System;
namespace BibleVerse.Exceptions
{
    public class UserLoginException: BVException
    {

        string userName = string.Empty;
        private static string exceptionType = "UserLoginException";

        public UserLoginException(BibleVerse.DALV2.BVIdentityContext context, string loginErrContext)
            :base(context, loginErrContext, exceptionType)
        {
           
        }

        public UserLoginException(BibleVerse.DALV2.BVIdentityContext context, string loginErrContext, int errCode)
            : base(context, loginErrContext, exceptionType ,errCode)
        {

        }

        public UserLoginException(BibleVerse.DALV2.BVIdentityContext context, string loginErrContext, string loginErrType, int errCode)
            :base(context, loginErrContext, loginErrType, errCode)
        {

        }

    }
}
