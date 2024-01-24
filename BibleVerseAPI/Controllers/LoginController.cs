using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using BibleVerse.Repositories;
using BibleVerse.DTO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BibleVerseAPI.Controllers
{
    //Authorization for JWT token. (Need to figure out proper flow)
    //[Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : Controller
    {
        private readonly JWTSettings _jwtSettings;
        private readonly JWTRepository _jWTRepository;
        private readonly RegistrationRepository _repository;
        private readonly ELogRepository _elogRepository;
        private readonly APIHelperV1 _apihelper;
        private string serviceBase = "Login";
        private string context = String.Empty;
        private readonly BibleVerse.DALV2.BVIdentityContext dbContext;

        public LoginController(RegistrationRepository repository, ELogRepository eLogRepository, JWTRepository jWTRepository, APIHelperV1 apihelper, BibleVerse.DALV2.BVIdentityContext context)
        {
            _repository = repository;
            _elogRepository = eLogRepository;
            _jWTRepository = jWTRepository;
            _apihelper = apihelper;
            dbContext = context;
        }
        
        [HttpPost]
        [ActionName("LoginUser")]
        public IActionResult LoginUser([FromBody] object userRequest)
        {
            try
            {
                string lr = String.Empty;

                var loginResponse = _repository.LoginUser(JsonConvert.DeserializeObject<LoginRequestModel>(userRequest.ToString()));


                try
                {

                    lr = JsonConvert.SerializeObject(loginResponse.Result);
                }
                catch (Exception ex)
                {
                    //Create ELog Storing Exception
                    BibleVerse.Exceptions.UserLoginException loginException = new BibleVerse.Exceptions.UserLoginException(dbContext, string.Format("Error At Application Login: {0}, StackTrace: {1}", ex.ToString(), ex.StackTrace.ToString()), 00001);

                    //var exceptionResponse = _elogRepository.StoreELog(BibleVerse.DTO.Transfers.TransferFunctions.TempELogToELog(loginException.LoggedException));

                    loginResponse.Result.ResponseStatus = BibleVerse.Repositories.APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                }

                if (loginResponse.IsCompletedSuccessfully)
                {
                    if (loginResponse.Result.ResponseStatus == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok(lr);
                    }
                    else if (loginResponse.Result.ResponseStatus == BibleVerse.Repositories.APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                    {
                        return Conflict(lr);
                    }
                    else
                    {
                        return BadRequest(lr);
                    }
                }
                else
                {
                    //Create an Elog error
                    return BadRequest("An Error Occurred");
                }
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }

        //[EnableCors("DevPolicy")]
        [HttpPost]
        [ActionName("RefreshToken")]
        public IActionResult RefreshToken([FromBody] object refreshRequest)
        {
            try
            {
                var response = _jWTRepository.AuthorizeRefreshRequest(JsonConvert.DeserializeObject<RefreshRequest>(refreshRequest.ToString()));

                if (response.IsCompletedSuccessfully)
                {
                    if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok(response.Result.ResponseBody);
                    }
                    else if (response.Result.ResponseMessage == "Failed")
                    {
                        return Conflict("An Error Has Occurred");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    //Create an Elog error
                    return BadRequest("An Error Occurred");
                }
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
