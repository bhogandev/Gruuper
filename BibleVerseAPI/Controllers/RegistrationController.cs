using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BVCommon;
using BibleVerse.Repositories;
using BibleVerse.DTO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web;
using System.Text;
using BibleVerse.Events.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using BibleVerse.DTO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationRepository _repository;
        private readonly AWSRepository _awsrepository;
        private readonly APIHelperV1 _apihelper;
        private IConfiguration _configuration;

        public RegistrationController(RegistrationRepository repository, AWSRepository aWSRepository, APIHelperV1 apihelper, IConfiguration configuration)
        {
            this._repository = repository;
            this._awsrepository = aWSRepository;
            _apihelper = apihelper;
            this._configuration = configuration;
        }

        //GET 
        [HttpGet]
        [ActionName("ConfirmEmail")]
        public IActionResult Get(string id, string token)
        {
            EmailConfirmationModel ecom = new EmailConfirmationModel()
            {
                userID =  id,
                token =  HttpUtility.UrlDecode(token)
            };

            var eComResponse =  _repository.ConfirmEmail(ecom);

            if (eComResponse.Result.ResponseStatus == "Email Confirmed")
            {
                return Ok(eComResponse.Result.ResponseStatus);
            }
            else
            {
                return Conflict(JsonConvert.SerializeObject(eComResponse.Result.ResponseErrors));
            }
        }

        [HttpPost]
        [ActionName("ResendConfirmation")]
        public IActionResult ResendConfirmation([FromBody] object ru)
        {
            UserViewModel requestUser = JsonConvert.DeserializeObject<UserViewModel>(ru.ToString());

            var response = _repository.ResendConfirmation(requestUser);

            if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
            {
                string appRoot = _configuration.GetConnectionString("AppRootProd");

                //If API is running in test mode, send user root to stage UI
                if (_configuration.GetValue<string>("RunInTestMode") == "Y")
                {
                    appRoot = _configuration.GetConnectionString("AppRootStage");
                }

                //Send ConfirmationEmail Token
                EmailService.SendConfirmationEmail(requestUser.Email, response.Result.UserId ,HttpUtility.UrlEncode(response.Result.ConfirmationToken), response.Result.Misc, appRoot);

                return Ok();
            }
            else if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
            {
                return Conflict(JsonConvert.SerializeObject(response.Result.ResponseErrors));
            }
            else
            {
                RegistrationResponseModel genModel = new RegistrationResponseModel()
                {
                    ResponseMessage = "An error occurred during reconfirmation"
                };
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Authorize]
        [ActionName("UserProfile")]
        public async Task<ObjectResult> GetUserProfile()
        {
            try
            {
                _apihelper.InitializeApiCall(HttpContext);

                //Get JWT from Request Header
                var token = Request.Headers["Token"];

                //Validate Token, If not valid, send conflict with ExpiredTokenMessage


                //Pass valid token here
                ApiResponseModel response = await _repository.FUFAT(token);

                if (response.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    return Ok(response);

                }
                else if (response.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                {
                    return Conflict(response);
                }
                else
                {
                    // Create an ELog and Log error
                    return BadRequest("An Unexpected Error Occured");
                }
            } catch(Exception ex)
            {
                return BadRequest("");
            }
        }


        /*
         * DEPRECATED 03.26.2021
         * REPLACED W/ Query Action In UserController
         * 
        [HttpGet]
        [ActionName("Search")]
        public IActionResult Search(string username, string user)
        {
            try
            {
                _apihelper.InitializeApiCall(HttpContext);

                var response = _repository.FindUser(username, user);

                if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    return Ok(response.Result);

                }
                else if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                {
                    return Conflict(response.Result);
                }
                else
                {
                    // Create an ELog and Log error
                    return BadRequest("An Unexpected Error Occured");
                }
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }
        */

        [HttpPost]
        [ActionName("CreateUser")]
        public IActionResult CreateUser([FromBody] object userRequest)
        {
            try
            {
                _apihelper.InitializeApiCall(HttpContext);

                //Users newU = JsonConvert.DeserializeObject<Users>(userRequest.ToString());

                var apiResponse =  _repository.CreateUser(JsonConvert.DeserializeObject<UserSignUpRequest>(userRequest.ToString()));

                if (apiResponse.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    string appRoot = _configuration.GetConnectionString("AppRootProd");

                    //If API is running in test mode, send user root to stage UI
                    if (_configuration.GetValue<string>("RunInTestMode") == "Y")
                    {
                        appRoot = _configuration.GetConnectionString("AppRootStage");
                    }

                    //Send ConfirmationEmail Token
                    EmailService.SendConfirmationEmail(apiResponse.Result.User.Email, apiResponse.Result.User.Id, HttpUtility.UrlEncode(apiResponse.Result.Misc), apiResponse.Result.ResponseBody[0], appRoot);

                    apiResponse.Result.ResponseBody[0] = String.Empty; //Clear out email pass
                    apiResponse.Result.Misc = String.Empty; //Clear out email confirmation token
                    

                    var requestBody = new StringContent(JsonConvert.SerializeObject(apiResponse.Result.User), Encoding.UTF8, "application/json");

                    var awsresult = _awsrepository.CreateUserDir(apiResponse.Result.User);
                    apiResponse.Result.User = null;

                    if (awsresult.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        //Do something here
                        
                    }
                    else
                    {
                        //Log in ELog and create task
                    }


                    return Ok(apiResponse.Result);

                }
                else if (apiResponse.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                {
                    return Conflict(apiResponse.Result);
                }
                else if (apiResponse.Result.ResponseMessage == "Email already exists")
                {
                    return Conflict(apiResponse.Result);
                }
                else
                {
                    // Create an ELog and Log error
                    return BadRequest("An Unexpected Error Occured");
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost]
        [ActionName("CreateOrg")]
        public IActionResult CreateOrg([FromBody] object userOrg)
        {
            try
            {
                _apihelper.InitializeApiCall(HttpContext);

                Organization newOrg = JsonConvert.DeserializeObject<Organization>(userOrg.ToString());

                var apiResponse = _repository.CreateOrganization(newOrg);

                if (apiResponse.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    return Ok(apiResponse);
                }
                else if (apiResponse.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                {
                    return Conflict(apiResponse);
                }
                else
                {
                    apiResponse.Result.ResponseMessage = "BadRequest";
                    apiResponse.Result.ResponseErrors = new List<string>();
                    apiResponse.Result.ResponseErrors.Add("An Unexpected Error Occured. Please try again");
                    return BadRequest(apiResponse);
                }
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }

    }
}