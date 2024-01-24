using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BibleVerse.DTO;

namespace BVCommon
{
    public class HelperFunctions
    {
        public static UserViewModel ConvertUserToUserView(Users user)
        {
            UserViewModel newUVM = new UserViewModel()
            {
                UserID = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Level = user.Level,
                ExpPoints = user.ExpPoints,
                RwdPoints = user.RwdPoints,
                Status = user.Status,
                OnlineStatus = user.OnlineStatus,
                Age = user.Age,
                Friends = user.Friends,
                OrganizationId = user.OrganizationId,
                OrgName = String.Empty
            };

            return newUVM;

        }

        public static ApiResponseModel ConvertToValidApiResponse(string message, List<string> errors)
        {
            ApiResponseModel newARM = new ApiResponseModel()
            {
                ResponseMessage = message,
                ResponseErrors = errors
            };

            return newARM;
        }
    }
}
