using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Exceptions
{
    public class OperationFailedException : BVException
    {
        string userName = string.Empty;
        private static string exceptionType = "OperationFailedException";

        public OperationFailedException(BibleVerse.DALV2.BVIdentityContext context, string operationErrContext)
            : base(context, operationErrContext, exceptionType)
        {

        }

        public OperationFailedException(BibleVerse.DALV2.BVIdentityContext context, string operationErrContext, int errCode)
            : base(context, operationErrContext, exceptionType, errCode)
        {

        }

        public OperationFailedException(BibleVerse.DALV2.BVIdentityContext context, string operationErrContext, string operationErrType, int errCode)
            : base(context, operationErrContext, operationErrType, errCode)
        {

        } 
    }
}
