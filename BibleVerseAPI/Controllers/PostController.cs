using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibleVerse.Repositories;
using BibleVerse.DTO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PostController : BVController
    {
        private readonly BibleVerse.Repositories.UserRepositories.UserActionRepository _repository;
        private readonly JWTSettings _jwtSettings;
        private readonly JWTRepository _jWTRepository;
        private readonly APIHelperV1 _apihelper;
        private string serviceBase = "Post";
        private string context = String.Empty;

        public PostController(BibleVerse.Repositories.UserRepositories.UserActionRepository repository, APIHelperV1 apiHelper)
        { 
            _repository = repository;
            _apihelper = apiHelper;
            //Initialize();
        }

        //Get All Of User's Posts
        [HttpGet]
        [ActionName("GetUserPosts")]
        public IActionResult Get(string userName, int page)
        {
            try
            {
                _apihelper.InitializeApiCall(HttpContext);

                ApiResponseModel userPosts = _repository.GetUserPosts(userName, page).Result;

                if (userPosts != null)
                {
                    if (userPosts.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok(JsonConvert.SerializeObject(userPosts.ResponseBody));
                    }
                    else if (userPosts.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Conflict))
                    {
                        return Conflict("No Posts Found");
                    }
                    else
                    {
                        return BadRequest("Unable to Retrieve Posts");
                    }
                }
                else
                {
                    //create Elog Error
                    return BadRequest("An Error Occurred");
                }
            }catch (Exception ex)
            {
                //Log Exception Here

                return BadRequest();
            }
        }

        [HttpGet]
        [ActionName("GetPost")]
        public IActionResult GetPost()
        {
            try
            {

                //Initialize Call
                _apihelper.InitializeApiCall(HttpContext);

                //var token = Request.Headers["Token"];
                var postID = Request.Headers["PostId"];

                var response = _repository.GetPost(postID);

                if (response != null)
                {
                    return Ok(response);
                }

                return BadRequest("An Error Occxured");
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }

        //Get Timeline Posts
        //Get All Of User's Posts
        [HttpGet]
        //[Authorize]
        [ActionName("GetTimeline")]
        public IActionResult GetTimeline(int page)
        {
            try
            {
                //Initialize Call
                _apihelper.InitializeApiCall(HttpContext);

                var token = Request.Headers["Token"];
                var refreshToken = Request.Headers["RefreshToken"];

                List<Posts> userPosts = _repository.GenerateTimelinePosts(token, refreshToken, page).Result;

                if (userPosts != null)
                {
                    if (userPosts.Count > 0)
                    {
                        ApiResponseModel response = new ApiResponseModel();
                        response.ResponseBody = new List<string>();
                        response.ResponseErrors = new List<string>();
                        response.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                        response.ResponseBody.Add(JsonConvert.SerializeObject(userPosts));

                        return Ok(response);
                    }
                    else if (userPosts.Count == 0)
                    {
                        return Conflict("No Posts Found");
                    }
                    else
                    {
                        return Conflict("Unable to Retrieve Posts");
                    }
                }
                else
                {
                    //create Elog Error
                    return BadRequest("An Error Occurred");
                }
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [ActionName("Profile")]
        public IActionResult Profile(string userID, string currUserName)
        {
            try
            {
                //Initialize Call
                _apihelper.InitializeApiCall(HttpContext);

                var token = Request.Headers["Token"];
                var refreshToken = Request.Headers["RefreshToken"];
                var userName = Request.Headers["UserName"];

                ApiResponseModel userProfile = _repository.GetUserProfile(token, refreshToken, userName).Result;

                if (userProfile != null)
                {
                    if (userProfile.ResponseBody.Count > 0)
                    {
                        return Ok(userProfile);
                    }
                    else if (userProfile.ResponseBody.Count == 0)
                    {
                        return Conflict("No Profile Found");
                    }
                    else
                    {
                        return Conflict("Unable to Retrieve Profile");
                    }
                }
                else
                {
                    //create Elog Error
                    return BadRequest("An Error Occurred");
                }
            } catch (Exception ex)
            {
                return BadRequest();
            }
        }
        
        [HttpPost]
        [ActionName("RelationshipReq")]
        public IActionResult RelationshipRequest([FromBody] object rRequest)
        {
            try
            {
                //Initialize Call
                _apihelper.InitializeApiCall(HttpContext);

                RelationshipRequestModel relationshipRequest = JsonConvert.DeserializeObject<RelationshipRequestModel>(rRequest.ToString());

                var response = _repository.ProcessRelationshipRequest(relationshipRequest);
                var responseResult = response.Result;


                if (response != null)
                {
                    if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok(responseResult);
                    }
                    else if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                    {
                        return Conflict(responseResult);
                    }
                    else
                    {
                        return BadRequest(responseResult);
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

        // POST api/values
        [HttpPost]
        [ActionName("CreatePost")]
        public IActionResult CreatePost([FromBody] object userPost)
        {
            try
            {
                //Initialize Call
                _apihelper.InitializeApiCall(HttpContext);

                PostModel post = new PostModel();
                post = JsonConvert.DeserializeObject<PostModel>(userPost.ToString());
                post.UserId = Request.Headers["Token"];
                var postResponse = _repository.CreateUserPost(post);
                var pr = JsonConvert.SerializeObject(postResponse.ToString());

                if (postResponse != null)
                {
                    if (postResponse.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok(pr);
                    }
                    else if (postResponse.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                    {
                        return Conflict(pr);
                    }
                    else
                    {
                        return BadRequest(pr);
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

        [HttpPost]
        [ActionName("Interact")]
        public IActionResult PostInteract([FromBody] object request)
        {
            try
            {
                //Initialize Call
                _apihelper.InitializeApiCall(HttpContext);

                var IntType = Request.Headers["IntType"];
                var token = Request.Headers["Token"];
                if (!String.IsNullOrEmpty(IntType) && !String.IsNullOrEmpty(token))
                {
                    Task<string> response = null;
                    RefreshRequest r = new RefreshRequest() { AccessToken = token };

                    if (IntType == "Like" || IntType == "Unlike")
                    {
                        Likes like = JsonConvert.DeserializeObject<Likes>(request.ToString());
                        response = _repository.InteractWithPostLikes(like, r);
                    }
                    else if (IntType == "Comment")
                    {
                        Comments comment = JsonConvert.DeserializeObject<Comments>(request.ToString());
                        response = _repository.InteractWithPostComments(comment, r);
                    }

                    if (response != null)
                    {
                        if (response.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                        {
                            return Ok();
                        }
                        else if (response.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                        {
                            return Conflict();
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
                }
                else
                {
                    return BadRequest("Please Check Headers For Null Or Empty Value");
                }
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ActionName("DeletePost")]
        public IActionResult DeletePost([FromBody] object deleteRequest)
        {
            try
            {
                //Initialize Call
                _apihelper.InitializeApiCall(HttpContext);

                var token = Request.Headers["Token"];
                var postID = JsonConvert.DeserializeObject<Posts>(deleteRequest.ToString()).PostId;

                if (!String.IsNullOrEmpty(token) && !String.IsNullOrEmpty(postID))
                {
                    RefreshRequest r = new RefreshRequest()
                    {
                        AccessToken = token
                    };

                    var deleteResponse = _repository.DeleteUserPost(r, postID);

                    if (deleteResponse != null)
                    {
                        if (deleteResponse.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                        {
                            //return success
                            return Ok();

                        }
                        else
                        {
                            //return error msg
                            return Conflict(deleteResponse.Result.ToString());
                        }
                    }
                    else
                    {
                        return BadRequest(deleteResponse.Result.ToString());
                    }
                }
                else
                {
                    return BadRequest("An Error Occured");
                }
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ActionName("UploadProfilePic")]
        public IActionResult UploadProfilePic([FromBody] object userUpload)
        {
            try
            {
                //Initialize Call
                _apihelper.InitializeApiCall(HttpContext);

                UserUpload userUpload1 = new UserUpload();
                userUpload1 = JsonConvert.DeserializeObject<UserUpload>(userUpload.ToString());

                var uploadResponse = _repository.ChangeUserProfilePic(userUpload1);
                var ur = JsonConvert.SerializeObject(uploadResponse.ToString());

                if (uploadResponse != null)
                {
                    if (uploadResponse.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok();
                    }
                    else if (uploadResponse.Result.ToString() == "Failed")
                    {
                        return Conflict(ur);
                    }
                    else
                    {
                        return BadRequest(ur);
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
