using System;
namespace BibleVerse.Exceptions
{
    public class UserLogOutException : BVException
    {
        string userName = string.Empty;
        private static string exceptionType = "UserLogOutException";

        public UserLogOutException(BibleVerse.DALV2.BVIdentityContext context, string logOutErrContext)
            : base(context, logOutErrContext, exceptionType)
        {

        }

        public UserLogOutException(BibleVerse.DALV2.BVIdentityContext context, string logOutErrContext, int errCode)
            : base(context, logOutErrContext, exceptionType, errCode)
        {

        }

        public UserLogOutException(BibleVerse.DALV2.BVIdentityContext context, string logOutErrContext, string loginErrType, int errCode)
            : base(context, logOutErrContext, loginErrType, errCode)
        {

        }
    }
}
