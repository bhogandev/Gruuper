using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Exceptions
{
    public class UserDoesNotExist : BVException
    {
        string userName = string.Empty;
        private static string exceptionType = "UserDoesNotExist";

        public UserDoesNotExist(BibleVerse.DALV2.BVIdentityContext context, string ErrContext)
            : base(context, ErrContext, exceptionType)
        {

        }

        public UserDoesNotExist(BibleVerse.DALV2.BVIdentityContext context, string ErrContext, int errCode)
            : base(context, ErrContext, exceptionType, errCode)
        {

        }

        public UserDoesNotExist(BibleVerse.DALV2.BVIdentityContext context, string ErrContext, string ErrType, int errCode)
            : base(context, ErrContext, ErrType, errCode)
        {

        }
    }
}
