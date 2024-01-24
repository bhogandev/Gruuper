﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Security.Policy;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Web;
using BibleVerse.DTO;
using BibleVerse.Events.Services;
using BibleVerse.DTO.Models;

namespace BibleVerse.Repositories
{
    public class RegistrationRepository
    {
        private readonly BibleVerse.DALV2.BVIdentityContext _context;
        
        UserManager<Users> userManager;
        SignInManager<Users> signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly JWTRepository _jwtrepository;
        private readonly ELogRepository _eLogRepository;
        protected string StackTraceRoot = "BibleVerse.DTO -> Repository -> RegistrationRepository: ";

        public RegistrationRepository(UserManager<Users> _userManager , SignInManager<Users> _signInManager , BibleVerse.DALV2.BVIdentityContext context, IOptions<JWTSettings> jwtSettings, JWTRepository jwtrepository, ELogRepository eLogRepository)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            _jwtSettings = jwtSettings.Value;
            _jwtrepository = jwtrepository;
            _eLogRepository = eLogRepository;
            this._context = context;

        }

        #region Methods

        //Get a list of all users in system
        private List<Users> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        //Validate Organization
        public bool ValidateOrg(string OrgId)
        {
            IQueryable<Organization> org;

            org = from o in _context.Organization
                  where o.OrganizationId == OrgId
                  select o;

            if(org.FirstOrDefault() != null)
            {
                return true;
            } else
            {
                return false;
            }
        }

        //Generate Refresh Token
        public RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(6);

            return refreshToken;
        }

        #endregion

        #region Tasks

        public async Task<ApiResponseModel> FUFAT(string token)
        {
            ApiResponseModel apiResponse = APIHelperV1.InitializeAPIResponse();

            //Get user based on access token
            var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken securityToken;

                var principle = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userID = principle.FindFirst(ClaimTypes.Name)?.Value;

                    var u = from x in userManager.Users
                            where x.UserId == userID
                            select x;

                    if (u.FirstOrDefault() != null)
                    {
                        var user = u.FirstOrDefault();

                    apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                    apiResponse.ResponseBody.Add(JsonConvert.SerializeObject(user));
                    }else
                {
                    apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                    apiResponse.ResponseErrors.Add("Invalid Token");
                }

                }


                return apiResponse;
        }

        //Get Users From Search
        public async Task<ApiResponseModel> FindUser(string username, string user)
        {
            IQueryable<Users> foundUsers;
            ApiResponseModel apiResponse = APIHelperV1.InitializeAPIResponse();
            List<SearchViewModel> searchResults = new List<SearchViewModel>();
            List<Profiles> searchProfiles = new List<Profiles>();

           
            foundUsers = from c in userManager.Users
                         where (c.UserName.Contains(username) && c.UserId != user)
                         orderby c.UserName descending
                         select c;


            if(foundUsers.FirstOrDefault() != null)
            {

                foreach(Users u in foundUsers)
                {
                    var uProfile = from c in _context.Profiles
                                   where c.ProfileId == u.UserId
                                   select c;

                    var uOrgName = from c in _context.Organization
                                   where c.OrganizationId == u.OrganizationId
                                   select c;

                    SearchViewModel uviewmodel = new SearchViewModel()
                    {
                        UserName = u.UserName,
                        ProfileURL = string.Format("/profile/{0}", u.UserName) //Create anchor link for user profile
                    };

                    if(uProfile.FirstOrDefault() != null)
                    {
                        uviewmodel.PictureURL = uProfile.FirstOrDefault().Picture;
                    }

                    if(uOrgName.FirstOrDefault() != null)
                    {
                        uviewmodel.OrgName = uOrgName.FirstOrDefault().Name;
                    }

                    searchResults.Add(uviewmodel);
                }

                apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                apiResponse.ResponseBody.Add(JsonConvert.SerializeObject(searchResults));
            }
            else
            {
                apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                apiResponse.ResponseErrors.Add("No users found");
            }


            return apiResponse;
            
        }

        //Create an organization
        public async Task<ApiResponseModel> CreateOrganization(Organization newOrg)
        {
            IQueryable<string> newOrgID;
            bool idCreated = false;
            bool orgExistsAlready = false;
            int retryTimes = 0;
            ApiResponseModel apiResponse = APIHelperV1.InitializeAPIResponse();

            //See if Organization Exists already
            var org = from c in _context.Organization
                      where c.Email == newOrg.Email
                      select c;

            if(org.FirstOrDefault() != null)
            {
                orgExistsAlready = true;
            }

            if(!orgExistsAlready) //If org doesn't exist, create org
            {
                // Create OrganizationID
                while (idCreated == false && retryTimes < 3)
                {
                    var genOrgID = BVCommon.BVFunctions.CreateUserID();
                    newOrgID = from c in _context.Organization
                               where c.OrganizationId == genOrgID
                               select c.OrganizationId;

                    if (newOrgID.FirstOrDefault() == null) //If generated ID isn't being used, assign it to org entity
                    {
                        IQueryable<string> newGenID;
                        bool subIdCreated = false;
                        int subRetryTimes = 0;

                        //Create SubscriptionID
                        while (!subIdCreated && retryTimes < 3)
                        {
                            var genSubID = BVCommon.BVFunctions.CreateUserID();
                            newGenID = from c in _context.Subscriptions
                                       where c.SubscriptionID == genSubID
                                       select c.SubscriptionID;

                            if (newGenID.FirstOrDefault() == null) //If subscription ID isn't being used, assign it to sub entity
                            {
                                newOrg.OrganizationId = genOrgID;

                                Subscriptions userSubscription = new Subscriptions()
                                {
                                    SubscriptionID = genSubID,
                                    OrganizationID = newOrg.OrganizationId,
                                    SubscriptionType = newOrg.Misc,
                                    ChangeDateTime = DateTime.Now,
                                    CreateDateTime = DateTime.Now
                                };

                                OrgSettings defaultOrgSettings = new OrgSettings()
                                {
                                    OrgSettingsId = newOrg.OrganizationId,
                                    OrganizationName = newOrg.Name,
                                    SharingEnabled = true,
                                    CalendarSharing = true,
                                    FollowersEnabled = true,
                                    MemMsgEnabled = true,
                                    OrgMsgEnabled = true,
                                    ChangeDateTime = DateTime.Now,
                                    CreateDateTime = DateTime.Now
                                };

                                newOrg.SubsciberId = userSubscription.SubscriptionID;
                                newOrg.Misc = "";
                                newOrg.CreateDateTime = DateTime.Now;
                                newOrg.ChangeDateTime = DateTime.Now;
                                newOrg.OrgSettingsId = newOrg.OrganizationId;

                                _context.OrgSettings.Add(defaultOrgSettings);
                                _context.Organization.Add(newOrg);
                                _context.Subscriptions.Add(userSubscription);
                                _context.SaveChanges();

                                var createdOrg = from c in _context.Organization
                                                 where c.OrganizationId == newOrg.OrganizationId
                                                 select c;

                                if (createdOrg.FirstOrDefault() != null) // If organization is actually created
                                {
                                    var createdSub = from c in _context.Subscriptions
                                                     where c.SubscriptionID == userSubscription.SubscriptionID
                                                     select c;

                                    if (createdSub.FirstOrDefault() != null) //if sub is actually created
                                    {
                                        bool refCodeCreated = false;
                                        int refRetryTimes = 0;

                                        while (!refCodeCreated && refRetryTimes < 3) // Create Ref Log +  Code
                                        {
                                            string refCode = BVCommon.BVFunctions.CreateRefCode();

                                            var createdRefCode = from c in _context.RefCodeLogs
                                                                 where c.RefCode == refCode
                                                                 select c;

                                            if (createdRefCode.FirstOrDefault() == null)  // If ref code isn't being used
                                            {
                                                RefCodeLogs newRefLog = new RefCodeLogs()
                                                {
                                                    OrganizationID = newOrg.OrganizationId,
                                                    RefCode = refCode,
                                                    RefCodeType = "Owner Referral Code",
                                                    isUsed = false,
                                                    isExpired = false,
                                                    ChangeDateTime = DateTime.Now,
                                                    CreateDateTime = DateTime.Now
                                                };

                                                _context.RefCodeLogs.Add(newRefLog); //Add  ref code to table
                                                _context.SaveChanges();

                                                //Exit Loop
                                                idCreated = true;
                                                subIdCreated = true;
                                                refCodeCreated = true;

                                                //Set api response to success and add data to pass
                                                apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                                                apiResponse.ResponseBody.Add(genOrgID);
                                                apiResponse.ResponseBody.Add(refCode);
                                                return apiResponse;
                                            }
                                            retryTimes++;
                                        }
                                    }
                                }

                            }
                            subRetryTimes++;
                        }
                    }
                    retryTimes++;
                }
            } else
            {
                apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                apiResponse.ResponseErrors.Add("An organization with this email already exists.");
                return apiResponse;
            }

            apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
            apiResponse.ResponseErrors.Add("Organization Creation Failed. Please try again.");
            return apiResponse;
        }

        //Create a User
        public async Task<ApiResponseModel> CreateUser(UserSignUpRequest nUser)
        {
            IQueryable<string> newUID;
            bool idCreated = false;
            bool userExistsAlready = false;
            int retryTimes = 0;
            ApiResponseModel apiResponse = APIHelperV1.InitializeAPIResponse();

            var userWEmail = from c in userManager.Users
                             where c.Email == nUser.Email
                             select c;

            if(userWEmail.FirstOrDefault() != null)
            {
                userExistsAlready = true;
            }

            if (!userExistsAlready)// If user doesn't exist already
            {
                //Setup New User With Default Values
                Users newUser = new Users()
                {
                    UserId = "",
                    UserName = nUser.UserName,
                    PasswordHash = nUser.Password,
                    Email = nUser.Email,
                    Level = 1,
                    ExpPoints = 0,
                    RwdPoints = 0,
                    Status = !string.IsNullOrEmpty(nUser.RefCode) ? nUser.RefCode : "Member",
                    OnlineStatus = "Offline",
                    Friends = 0,
                    PhoneNumber = nUser.PhoneNumber,
                    DOB = nUser.DOB,
                    Age = BVCommon.BVFunctions.GetUserAge(nUser.DOB),
                    OrganizationId = "BPLBUB-325522-413758-972947",
                    isDeleted = false,
                    isSuspended = false,
                    ChangeDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                };

                bool OrgExists = ValidateOrg(newUser.OrganizationId);

                if (OrgExists)
                {

                    while (idCreated == false && retryTimes < 3)
                    {
                        var genUID = BVCommon.BVFunctions.CreateUserID();
                        newUID = from c in userManager.Users
                                 where c.UserId == genUID 
                                 select c.UserId;

                        if (newUID.FirstOrDefault() == null) // If userID is not already in DB
                        {
                            newUser.UserId = genUID;

                            //Check if referal code changes  user status
                            if(newUser.Status != "Member") //If actual reference code is passed
                            {
                                var newUserStatus = from c in _context.RefCodeLogs
                                                    where c.RefCode == newUser.Status
                                                    select c;

                                if(newUserStatus.FirstOrDefault() != null) //If ref code is found
                                {
                                    var userStatus = BVCommon.BVFunctions.RetreiveStatusFromRefCode(newUserStatus.FirstOrDefault().RefCodeType);
                                    RefCodeLogs genRefCode = newUserStatus.First();

                                    if(!userStatus.Contains("Error") && genRefCode.isUsed != true)
                                    {
                                        newUser.Status = userStatus;
                                        genRefCode.isUsed = true;
                                        genRefCode.ChangeDateTime = DateTime.Now;
                                        _context.RefCodeLogs.Update(genRefCode);
                                        _context.SaveChanges();

                                    } else if(genRefCode.isUsed == true)
                                    {
                                        apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                                        apiResponse.ResponseErrors.Add("Referral Code Has Already Been Used!");
                                    }else if (genRefCode.isExpired == true) //Compare to Expire Date?
                                    {
                                        apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                                        apiResponse.ResponseErrors.Add("Referral Code Has Expired. Please Request Another Or Customer Support");
                                    }
                                    else
                                    {
                                        apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                                        apiResponse.ResponseErrors.Add("Referral Code is Invalid");
                                        return apiResponse;
                                    }
                                } else
                                {
                                    apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                                    apiResponse.ResponseErrors.Add("Referral Code is Invalid");
                                    return apiResponse;
                                }
                            }

                            var res = await userManager.CreateAsync(newUser, newUser.PasswordHash);

                            if (res.Succeeded)
                            {
                                
                                var org = from o in _context.Organization
                                      where o.OrganizationId == newUser.OrganizationId
                                      select o;

                                org.FirstOrDefault().Members++; // Uneccessary?
                                _context.Organization.Update(org.FirstOrDefault());
                                _context.SaveChanges();

                                //Retrieve default Profile to generate new user profile
                                var defaultProfile = from c in _context.Profiles
                                                     where c.ProfileId == "default"
                                                     select c;

                                Profiles dp = defaultProfile.First();

                                //Create a profile for user
                                Profiles newUserProfile = new Profiles()
                                {
                                    ProfileId = newUser.UserId,
                                    Picture = dp.Picture,
                                    Theme = dp.Theme,
                                    Followers = dp.Followers,
                                    Following = dp.Following,
                                    Description = dp.Description,
                                    IsDeleted = dp.IsDeleted,
                                    ChangeDateTime = DateTime.Now,
                                    CreateDateTime = DateTime.Now
                                };

                                _context.Profiles.Add(newUserProfile);
                                _context.SaveChanges();

                                //Grab Email Key
                                var eKey = from c in _context.SiteConfigs
                                           where c.Service == "Email" && c.Name == "AccountPass"
                                           select c;

                                idCreated = true;
                                apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                                apiResponse.Misc = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                                apiResponse.User = newUser;
                                apiResponse.ResponseBody.Add(eKey.First().Value);
                            }
                            else
                            {
                                //Check against error codes. If normal error, return to user, otherwise Log error in Elog and return generic error
                                apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                                foreach (IdentityError e in res.Errors.ToList())
                                {
                                    apiResponse.ResponseErrors.Add(e.Description);
                                }

                            }

                            return apiResponse;
                        }

                        retryTimes++;
                    }
                    apiResponse.ResponseMessage = "Retry Timeout Failure";
                    return apiResponse;
                }
                else
                {
                    apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                    apiResponse.ResponseErrors.Add("Organization Not Found");
                }

                return apiResponse;
            } else //If user exits already
            {
                apiResponse.ResponseMessage = "Email already exists";
                apiResponse.ResponseErrors.Add("Email Already Exists");
                return apiResponse;
            }
        }

        //Resend Confirmation Email
        public async Task<RegistrationResponseModel> ResendConfirmation(UserViewModel requestUser)
        {
            RegistrationResponseModel apiResponse = new RegistrationResponseModel();
            apiResponse.ResponseErrors = new List<string>();

            //Check if user actually exists
            var user = await userManager.FindByEmailAsync(requestUser.Email);

            if(user != null)
            {
                if (!user.EmailConfirmed)
                {
                    var eKey = from c in _context.SiteConfigs
                               where c.Service == "Email" && c.Name == "AccountPass"
                               select c;

                    //resend user confirmation link
                    apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                    apiResponse.ConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    apiResponse.UserId = user.Id;
                    apiResponse.Misc = eKey.FirstOrDefault().Value;
                } else
                {
                    apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                    apiResponse.ResponseErrors.Add("User Already Confirmed");
                }
            } else
            {
                apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                apiResponse.ResponseErrors.Add("User Not Found!");
            }

            return apiResponse;
        }

        //Confirm User Email
        public async Task<EComResponseModel> ConfirmEmail(EmailConfirmationModel ecom)
        {
            var user = await userManager.FindByIdAsync(ecom.userID);
            EComResponseModel eComResponse = new EComResponseModel();
            eComResponse.ResponseErrors = new List<IdentityError>();


            if (user != null)
            {
                if(user.EmailConfirmed == false)
                { 
                    var emailConfirmation = await userManager.ConfirmEmailAsync(user, ecom.token);

                    if (emailConfirmation.Succeeded)
                    {
                        eComResponse.ResponseStatus = "Email Confirmed";
                    }
                    else
                    {
                        eComResponse.ResponseStatus = "Email Confirmation Failed";
                        foreach (IdentityError e in emailConfirmation.Errors)
                        {
                            eComResponse.ResponseErrors.Add(e);
                        }
                    }
                } else
                {
                    eComResponse.ResponseStatus = "Email Confirmation Failed";
                    IdentityError error = new IdentityError()
                    {
                        Code = "EMAILALREADYCONFIRMED",
                        Description = "Email is already confirmed!"
                    };
                    eComResponse.ResponseErrors.Add(error);
                }
                return eComResponse;
            } else
            {
                eComResponse.ResponseStatus = "An Error Occurred";
                IdentityError error = new IdentityError()
                {
                    Code = "GENERROR",
                    Description = "User Not Found On Request For Confirmation"
                };
                eComResponse.ResponseErrors.Add(error);
                return eComResponse;
            }
        }

        //Log User In
        public async Task<LoginResponseModel> LoginUser(LoginRequestModel loginRequest)
        {
            // Handle if user is suspended currently and handle if user is deleted or if user has not confirmed  email yet
            bool userFound = false;
            int retryTimes = 0;
            LoginResponseModel loginResponse = new LoginResponseModel();
            IQueryable<Users> currUser;
            IQueryable<Posts> iPosts;

            while (userFound == false && retryTimes < 3)
            {
                currUser = from u in userManager.Users
                           where u.Email == loginRequest.Email
                           select u;
                if (currUser.FirstOrDefault() != null)
                {
                   userFound = true;
                   var res = await signInManager.PasswordSignInAsync(currUser.FirstOrDefault().UserName, loginRequest.Password, true, true);

                    if(res.Succeeded)
                    {
                        if (currUser.FirstOrDefault().EmailConfirmed == true)
                        {
                            Users cu = currUser.First();
                            var rt = _jwtrepository.GenerateRefreshToken();
                            rt.UserId = cu.UserId;
                            cu.OnlineStatus = "Online"; // Set user status to online
                            cu.ChangeDateTime = DateTime.Now;
                            cu.RefreshTokens.Add(rt);
                            //Log action in user actions table
                            UserHistory loginLog = new UserHistory()
                            {
                                UserID = cu.UserId,
                                ActionType = "UserLogin",
                                ActionMessage = cu.UserName + " has just logged in.",
                                Prev_Value = "Offline",
                                Curr_Value = "Online",
                                ChangeDateTime = DateTime.Now,
                                CreateDateTime = DateTime.Now
                            };

                            _context.UserHistory.Add(loginLog);
                            _context.SaveChanges();

                            //retrieve user profile
                            var userProfile = from c in _context.Profiles
                                              where c.ProfileId == cu.UserId
                                              select c;

                            if(userProfile.FirstOrDefault() != null)
                            {
                                //loginResponse.UserProfile = userProfile.First();
                            } else
                            {
                                var defaultProfile = from c in _context.Profiles
                                                     where c.ProfileId == "default"
                                                     select c;
                                //loginResponse.UserProfile = defaultProfile.FirstOrDefault();
                            }

                            var userOrg = from x in _context.Organization
                                          where x.OrganizationId == cu.OrganizationId
                                          select x;

                            if(userOrg.FirstOrDefault() != null)
                            {
                                //loginResponse.Misc = userOrg.First().Name;
                            }

                            var userUpdate = await userManager.UpdateAsync(cu);

                            if (userUpdate.Succeeded)
                            {
                                loginResponse.ResponseStatus = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                                cu.PasswordHash = "";
                                cu.AccessToken = _jwtrepository.GenerateAccessToken(cu.UserId);
                                cu.RefreshToken = rt.Token;
                                //loginResponse.ResponseUser = cu;
                                loginResponse.Token = cu.AccessToken;
                                loginResponse.RefreshToken = rt.Token;
                                loginResponse.User = JsonConvert.SerializeObject(BVCommon.HelperFunctions.ConvertUserToUserView(cu));
                            }
                            else
                            {
                                Error error = new Error()
                                {
                                    Code = "ERRORONUSERUPDATE",
                                    Description = "Error updating user after successful login"
                                };
                                loginResponse.ResponseStatus = BibleVerse.Repositories.APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                                loginResponse.ResponseErrors = new List<Error>();
                                loginResponse.ResponseErrors.Add(error);
                            }
                        } else
                        {
                            //Return Login Failure and redirect user to confirmation screen
                            loginResponse.ResponseStatus = BibleVerse.Repositories.APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                            loginResponse.ResponseErrors = new List<Error>();
                            Error e = new Error()
                            {
                                Code = "EMAILNOTCONFIRMED",
                                Description = "Email not confirmed. Please confirm email."
                            };
                            loginResponse.ResponseErrors.Add(e);
                        }
                    } else
                    {
                        List<string> identityError = BVCommon.ELogFunctions.GetSignInError(res);
                        Error error = new Error()
                        {
                            Code = identityError[0],
                            Description = identityError[1]
                        };
                        loginResponse.ResponseStatus = BibleVerse.Repositories.APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
                        loginResponse.ResponseErrors = new List<Error>();
                        loginResponse.ResponseErrors.Add(error);
                    }
                }
                else
                {
                    retryTimes++;
                }
            }
                return loginResponse;
        }

        //Log User Out
        public async Task<string> LogoutUser(RefreshRequest request)
        {
            Users u = _jwtrepository.FindUserFromAccessToken(request);

            UserViewModel uvm = new UserViewModel()
            {
                UserID = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                Age = u.Age,
                OrganizationId = u.OrganizationId,
                OnlineStatus = u.OnlineStatus,
                Status =  u.Status,
                ExpPoints =   u.ExpPoints,
                RwdPoints = u.RwdPoints,
                Friends = u.Friends
            };



            //Sign user out
            var user = await userManager.FindByEmailAsync(uvm.Email);

            if (user != null)
            {
                user.OnlineStatus = uvm.OnlineStatus;
                user.Level = uvm.Level;
                user.RwdPoints = uvm.RwdPoints;
                user.ExpPoints = uvm.ExpPoints;
                user.Friends = uvm.Friends;
                user.Status = uvm.Status;
                user.ChangeDateTime = DateTime.Now;

                var result = await userManager.UpdateAsync(user); //Update user with final view model at time of signout

                if (result.Succeeded)
                {
                    UserHistory logoutLog = new UserHistory()
                    {
                        UserID = user.UserId,
                        ActionType = "UserLogout",
                        ActionMessage = user.UserName + " has just logged out.",
                        Prev_Value = "Online",
                        Curr_Value = "Offline",
                        ChangeDateTime = DateTime.Now,
                        CreateDateTime = DateTime.Now
                    };

                    _context.UserHistory.Add(logoutLog);
                    _context.SaveChanges();

                    return "User Successfully Updated";
                }
                else
                {
                    //Log in ELog
                    BibleVerse.Exceptions.UserLogOutException logOutException = new BibleVerse.Exceptions.UserLogOutException(_context, String.Format("Error At Application LogOut: {0}, StackTrace: {1}", "User Final Update Not Completed Upon Logout", StackTraceRoot + "User LogOut Update", 00003));
                    //var exceptionLog = _eLogRepository.StoreELog(BibleVerse.DTO.Transfers.TransferFunctions.TempELogToELog(logOutException.LoggedException));
                    return "Error: User Final Update Not Completed Upon Logout";
                }
            } else
            {
                //Log in ELog
                BibleVerse.Exceptions.UserLogOutException logOutException = new BibleVerse.Exceptions.UserLogOutException(_context, String.Format("Error At Application LogOut: {0}, StackTrace: {1}", "User turned up NULL", StackTraceRoot + "User LogOut Update", 00001));
                return "Error: User turned up NULL";
            }
        }

        #endregion
    }
}