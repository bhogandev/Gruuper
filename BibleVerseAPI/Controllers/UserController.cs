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
using Microsoft.AspNetCore.Authorization;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserController : BVController
    {
        private readonly BibleVerse.Repositories.UserRepositories.UserActionRepository _repository;
        private readonly BibleVerse.Repositories.APIHelperV1 _apiHelper;
        

        public UserController(BibleVerse.Repositories.UserRepositories.UserActionRepository repository, BibleVerse.Repositories.APIHelperV1 apiHelper)
        {
            this._repository = repository;
            this._apiHelper = apiHelper;
        }

        /*
         * Query System for search result (Object, post, person etc)
         */
        [HttpGet]
        [ActionName("Query")]
        public IActionResult Index(string query, string qFilter)
        {
            try
            {
                _apiHelper.InitializeApiCall(HttpContext);

                //Pass valid token here
                var response = _repository.Query(qFilter, Request.Headers["Token"], query);


                if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    return Ok(JsonConvert.SerializeObject(response.Result.ResponseBody));

                }
                else if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                {
                    return Conflict(JsonConvert.SerializeObject(response.Result.ResponseErrors));
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

        /*
         * Retrieve Friends For User
         */
        [HttpGet]
        [ActionName("GetFriends")]
        public IActionResult GetFriends(string username)
        {
            try
            {
                _apiHelper.InitializeApiCall(HttpContext);

                //Pass valid token here
                var response = _repository.GetFriends(username, Request.Headers["Token"]);


                if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    return Ok(JsonConvert.SerializeObject(response.Result));

                }
                else if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                {
                    return Conflict(JsonConvert.SerializeObject(response.Result));
                }
                else
                {
                    // Create an ELog and Log error
                    return BadRequest("An Unexpected Error Occured");
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        /*
         * Get User Profile
         */
        [HttpGet]
        [ActionName("GetProfile")]
        public IActionResult GetProfile(string username)
        {
            _apiHelper.InitializeApiCall(HttpContext);

            try
            {

                Task<ApiResponseModel> response = _repository.GetUserProfile(Request.Headers["Token"], Request.Headers["RefreshToken"], username);

                if (response.IsCompleted && response.Result != null)
                {
                    if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok(JsonConvert.SerializeObject(response.Result.ResponseBody));
                    }
                    else
                    {
                        return Conflict(JsonConvert.SerializeObject(response.Result.ResponseErrors));
                    }
                }
                else
                {
                    return BadRequest("An Error Occured");
                }
            }catch(ArgumentNullException nullEx)
            {
                //Record Exception
                return BadRequest(BibleVerse.Exceptions.BVExErrorCodes.ExShortCut(BibleVerse.Exceptions.BVExErrorCodes.ShortCodes.KillCode));
            }catch(Exception ex)
            {
                //Log Exception
                return BadRequest("An Error Occured" );
            }
        }

        /*
         * Get user info
         */
        [HttpGet]
        [ActionName("GetUserInfo")]
        public IActionResult GetUserInfo()
        {
            _apiHelper.InitializeApiCall(HttpContext);

            try
            {

                Task<ApiResponseModel> response = _repository.GetUserInfo(Request.Headers["Token"], Request.Headers["RefreshToken"]);

                if (response.IsCompleted && response.Result != null)
                {
                    if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok(response.Result);
                    }
                    else
                    {
                        return Conflict(response.Result);
                    }
                }
                else
                {
                    return BadRequest("An Error Occured");
                }
            }
            catch (ArgumentNullException nullEx)
            {
                //Record Exception
                return BadRequest(BibleVerse.Exceptions.BVExErrorCodes.ExShortCut(BibleVerse.Exceptions.BVExErrorCodes.ShortCodes.KillCode));
            }
            catch (Exception ex)
            {
                string message = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                List<string> errors = new List<string> { "An Error Occured" };

                //Log Exception
                return BadRequest(BVCommon.HelperFunctions.ConvertToValidApiResponse(message, errors));
            }
        }
    }
}
