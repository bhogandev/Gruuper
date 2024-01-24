using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories
{
    public class APIHelperV1
    {
        private readonly BibleVerse.DALV2.BVIdentityContext _context;

        public APIHelperV1(BibleVerse.DALV2.BVIdentityContext context)
        {
            this._context = context;
        }

        //Initialize API Response Model For API Call
        public static BibleVerse.DTO.ApiResponseModel InitializeAPIResponse()
        {
            BibleVerse.DTO.ApiResponseModel newApiReponse = new DTO.ApiResponseModel();
            newApiReponse.ResponseBody = new List<string>();
            newApiReponse.ResponseErrors = new List<string>();

            return newApiReponse;
        }

        public static string RetreieveResponseMessage(ResponseMessageEnum response)
        {
            switch(response){
                case ResponseMessageEnum.Success:
                    return "SUCCESS";

                case ResponseMessageEnum.Failure:
                    return "FAILURE";

                case ResponseMessageEnum.Conflict:
                    return "CONFLICT";

                default:
                    return "BADREQUEST";
            }
        }

        public enum ResponseMessageEnum
        {
            Success,
            Failure,
            Conflict
        }

        public void InitializeApiCall(HttpContext context)
        {
            try
            {
                VerifyToken(context);
                RecordTransaction(context);
            }
            catch (ArgumentNullException nullEx)
            {

            }
            catch (Exception ex)
            {
                //Log Exception In DB
            }
        }

        //Verify that call provided token and refresh token
        private void VerifyToken(HttpContext context )
        {
            HttpRequest Request = context.Request;

            if (Request != null)
            {
                string token = string.Empty;
                string refreshToken = string.Empty;

                if ((String.IsNullOrEmpty(Request.Headers["token"]) || Request.Headers["token"] == "undefined") || (String.IsNullOrEmpty(Request.Headers["refreshToken"]) || Request.Headers["RefreshToken"] == "undefined"))
                    throw new ArgumentNullException("Null Value Provided For Token");
            }
        }

        private void RecordTransaction(HttpContext context)
        {
            HttpRequest Request = context.Request;

            if (Request != null)
            {
                string device = string.Empty;
                string identity = string.Empty;
                string version = string.Empty;
                string requestBody = string.Empty;

                if (!string.IsNullOrEmpty(Request.Headers["device"]))
                    device = Request.Headers["device"];

                if (!string.IsNullOrEmpty(Request.Headers["identity"]))
                    identity = Request.Headers["identity"];

                if (!string.IsNullOrEmpty(Request.Headers["version"]))
                    version = Request.Headers["version"];

                if (Request.Body != null)
                    requestBody = Request.Body.ToString();

                BibleVerse.DTO.Transactions transaction = new BibleVerse.DTO.Transactions()
                {
                    ClientAddress = !string.IsNullOrEmpty(context.Connection.RemoteIpAddress.ToString()) ? context.Connection.RemoteIpAddress.ToString() : "",
                    ClientDevice = device,
                    ClientIdentity = identity,
                    ClientVersion = version,
                    TransactionController = Request.Path,
                    TransactionType = Request.Method,
                    TransactionPayload = requestBody,
                    TransactionTime = DateTime.Now,
                };


                this.RecordTransaction(transaction);
            }
        }

        public bool RecordTransaction(BibleVerse.DTO.Transactions transaction)
        {
            string entType = transaction.GetType().Name;

            string entObject = JsonConvert.SerializeObject(transaction);

            bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObject, _context);

            return result;
        }
    }
}
