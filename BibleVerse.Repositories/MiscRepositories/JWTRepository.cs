﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using BibleVerse.DTO;

namespace BibleVerse.Repositories
{
    public class JWTRepository
    {
        UserManager<Users> userManager;
        private readonly BibleVerse.DALV2.BVIdentityContext _context;
        private readonly JWTSettings _jwtSettings;
        protected string StackTraceRoot = "BibleVerse.DTO -> Repository -> JWTRepository: ";

        public JWTRepository(BibleVerse.DALV2.BVIdentityContext context, IOptions<JWTSettings> jwtSettings, UserManager<Users> _userManager)
        {
            userManager = _userManager;
            _jwtSettings = jwtSettings.Value;
            this._context = context;
        }

        //Validate Token Pre API call
        public bool ValidateJWTToken(string token)
        {
            return false;
        }

        //Generate JWT Token
        public string GenerateAccessToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
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

        //Find User From Access Token
        public Users FindUserFromAccessToken(RefreshRequest request)
        {
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

            var principle = tokenHandler.ValidateToken(request.AccessToken, tokenValidationParameters, out securityToken);

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

                    return user;
                }

            }

            return null;
        }

        //Validate the RefreshToken Of The User
        public bool ValidateRefreshToken(Users user, string refreshToken)
        {
            //Get list of logs matching refresh token
            var rTokenList = from x in _context.RefreshTokens
                             where x.Token == refreshToken
                             orderby x.ExpiryDate descending
                             select x;

            RefreshToken rt = rTokenList.FirstOrDefault();

            if (rt != null && (rt.UserId == user.UserId) && (rt.ExpiryDate > DateTime.UtcNow))
            {
                return true;
            }

            return false;
        }

        //Authorize Refresh Token
        public async Task<ApiResponseModel> AuthorizeRefreshRequest(RefreshRequest request)
        {
            ApiResponseModel apiResponse = new ApiResponseModel();
            apiResponse.ResponseBody = new List<string>();
            apiResponse.ResponseErrors = new List<string>();

            //Find user from access token
            Users user = FindUserFromAccessToken(request);

            //validate refresh token
            if (user != null && ValidateRefreshToken(user, request.RefreshToken))
            {
                user.AccessToken = GenerateAccessToken(user.UserId);
                apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                apiResponse.ResponseBody.Add(JsonConvert.SerializeObject(user));
                return apiResponse;
            }

            apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure);
            return apiResponse;
        }
    }
}
