using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibleVerse.Repositories.HubRepositories
{
    public class HubHelper
    {
        //Send Private Message To User
        public static bool SendPrivateMessage(BibleVerse.DALV2.BVIdentityContext context, BibleVerse.DTO.Messages msg, string conID, string groupID)
        {
            bool result = false;

            result = LogMessage(context, msg);

            //Write Logic to send message to user

            return result;
        }

        public static List<BibleVerse.DTO.Models.Groups> RetrieveUserGroups(BibleVerse.DALV2.BVIdentityContext context, string userName)
        {
            List<BibleVerse.DTO.Models.Groups> msgGroups = new List<BibleVerse.DTO.Models.Groups>();

            //Grab lsit of groups

            //Parameters
            var uParam = userName;

            var qGroups = context
                .Groups
                .FromSqlRaw("EXECUTE  dbo.sp_GetGroupIDsForUser {0}", uParam).ToListAsync();

            msgGroups = qGroups.Result != null ? qGroups.Result : new List<DTO.Models.Groups>();

            return msgGroups;
        }

        public static bool CreateNewGroup(BibleVerse.DALV2.BVIdentityContext context, BibleVerse.DTO.Models.Groups group, List<BibleVerse.DTO.Models.GroupMembers> gMembers)
        {
            bool result = false;
            string _entType = String.Empty;
            string _entObject = String.Empty;

            _entType = group.GetType().Name;
            _entObject = JsonConvert.SerializeObject(group);

            result = BVCommon.BVContextFunctions.WriteToDb(_entType, _entObject, context);

            foreach(BibleVerse.DTO.Models.GroupMembers gMember in gMembers)
            {
                _entType = gMember.GetType().Name;
                _entObject = JsonConvert.SerializeObject(gMember);

                result = BVCommon.BVContextFunctions.WriteToDb(_entType, _entObject, context);
            }

            return result;
        }

        public static List<BibleVerse.DTO.ClientModels.ResponseMessage> RetrievePreviousConversation(BibleVerse.DALV2.BVIdentityContext context, BibleVerse.DTO.Models.Groups group)
         {
            List<BibleVerse.DTO.ClientModels.ResponseMessage> prevMessages = new List<DTO.ClientModels.ResponseMessage>();

            var qMessages = (from c in context.Messages
                             where (c.Recipient == @group.GroupUID) && (c.IsDeleted == false) 
                             orderby c.CreateDateTime ascending
                             select c);
            if(qMessages.FirstOrDefault() != null)
            {
                 foreach(BibleVerse.DTO.Messages message in qMessages.ToList())
                {
                    BibleVerse.DTO.ClientModels.ResponseMessage response = new DTO.ClientModels.ResponseMessage()
                    {
                        user = message.Username,
                        message = message.Body,
                        time = message.CreateDateTime.ToString("hh:mm tt")
                    };

                    prevMessages.Add(response);
                }

                return prevMessages;
            }
            else
            {
                throw new BibleVerse.Exceptions.BVException(context, string.Format("No messages able to be retrieved for group with id {0}", group.GroupUID), 777);
            } 
        }

        public static bool RemoveUserFromGroup(BibleVerse.DALV2.BVIdentityContext context, BibleVerse.DTO.Models.Groups group, BibleVerse.DTO.Models.GroupMembers member)
        {
            bool result = false;

            //Get list of usernames of current users in group
            var groupUserNames = (from c in context.GroupMembers
                                  where (c.GroupID == member.GroupID) && (c.UserName != member.UserName)
                                  select c.UserName).ToList();

            string newGroupName = GenGroupName(groupUserNames);

            string newGroupID = GenGroupID(groupUserNames);

            result = ReconfigureGroup(context, group, member, newGroupName, newGroupID, "REMOVE");

            return result;
        }

        public static bool AddUserToGroup(BibleVerse.DALV2.BVIdentityContext context, BibleVerse.DTO.Models.Groups group, BibleVerse.DTO.Models.GroupMembers member)
        {
            bool result = false;

            //Get list of usernames of current users in group
            var groupUserNames = (from c in context.GroupMembers
                                  where (c.GroupID == member.GroupID) && (c.UserName != member.UserName)
                                  select c.UserName).ToList();

            string newGroupName = GenGroupName(groupUserNames);

            string newGroupID = GenGroupID(groupUserNames);

            result = ReconfigureGroup(context, group, member, newGroupName, newGroupID, "ADD");

            return result;
        }

        public static bool InviteUserToGroup(BibleVerse.DALV2.BVIdentityContext context, BibleVerse.DTO.Models.Groups group, BibleVerse.DTO.Users user, string appRoot)
        {
            /*
             * Create Emamil Invite For User To Join Group Chat
             * Use RefCodeLogs to Store Invite Code
             * Send Invite Code in link along with route to confirm/add user to group
             */

            string members = string.Empty;
            string refCode = string.Empty;
            List<BibleVerse.DTO.Models.GroupMembers> groupMembers = (from c in context.GroupMembers
                                                                     where c.GroupID == @group.GroupUID
                                                                     select c).ToList();

            foreach(BibleVerse.DTO.Models.GroupMembers member in groupMembers)
            {
                members += member.UserName;

                if(groupMembers.IndexOf(member) != (groupMembers.Count - 1))
                {
                    members += " ,";
                }
            }

            string key = (from x in context.SiteConfigs
                          where x.Service == "Email" && x.Name == "AccountPass"
                          select x.Value).FirstOrDefault();

            //Generate RefCode + Verify It Is not being used
            bool refCodeGen = false;
            while (!refCodeGen)
            {
                refCode = BVCommon.BVFunctions.CreateRefCode();

                var refCodeCheck = (from c in context.RefCodeLogs
                                    where c.RefCode == refCode
                                    select c).FirstOrDefault();

                if(refCodeCheck == null)
                {
                    refCodeGen = true;
                }
            }

            BibleVerse.DTO.RefCodeLogs refCodeLog = new DTO.RefCodeLogs()
            {
                OrganizationID = group.GroupUID,
                RefCodeType = "Group Chat Invite",
                RefCode = refCode,
                isUsed = false,
                isExpired = false,
                ChangeDateTime = DateTime.Now,
                CreateDateTime = DateTime.Now
            };

            string entType = refCodeLog.GetType().Name;
            string entObject = JsonConvert.SerializeObject(refCodeLog);

            bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObject, context);

            if (result)
            {

                //Send GroupChat Invite To User
                BibleVerse.Events.Services.EmailService.SendGroupChatInvite(user.Email, refCode, user.UserName, user.Id, members, key, appRoot);
                return true;
            }
            else
            {
                BibleVerse.Exceptions.BVException exception = new Exceptions.BVException(context, "Group Chat Invite", 777);
                return false;
            }
        }

        #region Helper Methods
        private static bool LogMessage(BibleVerse.DALV2.BVIdentityContext context, BibleVerse.DTO.Messages msg)
        {
            
            bool result = false;
            string _entType = String.Empty;
            string _entObject = String.Empty;

            _entType = msg.GetType().Name;
            _entObject = JsonConvert.SerializeObject(msg);
            result = BVCommon.BVContextFunctions.WriteToDb(_entType, _entObject, context);

            return result;
        }

        public static string GenGroupID(List<string> groupMembers)
        {
            string gid = string.Empty;
            string newgid = string.Empty;
            char[] conversionArray = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

            groupMembers.Sort();

            foreach(string g in groupMembers)
            {
                gid += g;
            }

            gid = gid.ToUpper();

            foreach(char c in gid)
            {
                 
                newgid += Array.IndexOf(conversionArray, c).ToString();
                
            }

            return newgid;
        }

        public static string GenGroupName(List<string> groupMembers)
        {
            string gName = string.Empty;

            groupMembers.Sort();

            foreach (string g in groupMembers)
            {
                if (groupMembers.IndexOf(g) < groupMembers.Count - 1)
                    gName += g + ", ";
                else
                    gName += g;
            }

            return gName;
        }

        private static bool ReconfigureGroup(BibleVerse.DALV2.BVIdentityContext context, BibleVerse.DTO.Models.Groups group, BibleVerse.DTO.Models.GroupMembers member ,string newGroupName, string newGroupUID, string configurationType)
        {
            /*
             * - update groupUID
             * - update groupmembersID
             * - update groupName
             * - update group member count                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
             */
            bool result = false;
            string _entType = string.Empty;
            string _entObject = string.Empty;
            BibleVerse.DTO.Models.Groups oldGroup = new DTO.Models.Groups();
            List<BibleVerse.DTO.Models.GroupMembers> oldGroupMembers = new List<DTO.Models.GroupMembers>();

            switch (configurationType)
            {
                case "REMOVE":
         
                    //Grab old group from db
                     oldGroup = (from c in context.Groups
                                    where c.GroupUID == @group.GroupUID
                                    select c).FirstOrDefault();

                    //Grab old group members from db
                     oldGroupMembers = (from x in context.GroupMembers
                                           where x.GroupID == oldGroup.GroupUID
                                           select x).ToList();

                    _entType = member.GetType().Name;
                    _entObject = JsonConvert.SerializeObject(member);


                    //Remove member from group
                    result = BVCommon.BVContextFunctions.DeleteFromDb(_entType, _entObject, context);

                    //Update GroupUID in Group Table + GroupID in GroupMembers Table as well as GroupName
                    group.GroupUID = newGroupUID;
                    group.GroupName = newGroupName;

                    _entType = group.GetType().Name;
                    _entObject = JsonConvert.SerializeObject(group);

                    result = BVCommon.BVContextFunctions.UpdateToDb(_entType, _entObject, context);

                    //Remove member from list
                    oldGroupMembers.Remove(member);

                    //Update each memeber of group with new groupID
                    foreach(BibleVerse.DTO.Models.GroupMembers m in oldGroupMembers)
                    {
                        m.GroupID = newGroupUID;

                        _entType = m.GetType().Name;
                        _entObject = JsonConvert.SerializeObject(m);

                        result = BVCommon.BVContextFunctions.UpdateToDb(_entType, _entObject, context);
                    }

                    return result;

                case "ADD":

                    //Grab old group from db
                    oldGroup = (from c in context.Groups
                                    where c.GroupUID == @group.GroupUID
                                    select c).FirstOrDefault();

                    //Grab old group members from db
                    oldGroupMembers = (from x in context.GroupMembers
                                           where x.GroupID == oldGroup.GroupUID
                                           select x).ToList();

                    //Add new member to group
                    oldGroupMembers.Add(member);

                    _entType = member.GetType().Name;
                    _entObject = JsonConvert.SerializeObject(member);


                    //Add  member to group
                    result = BVCommon.BVContextFunctions.WriteToDb(_entType, _entObject, context);
                     
                    //Update GroupUID in Group Table + GroupID in GroupMembers Table as well as GroupName
                    group.GroupUID = newGroupUID;
                    group.GroupName = newGroupName; 

                    _entType = group.GetType().Name;     
                    _entObject = JsonConvert.SerializeObject(group);

                    result = BVCommon.BVContextFunctions.UpdateToDb(_entType, _entObject, context); 

                    //Update each memeber of group with new groupID
                    foreach (BibleVerse.DTO.Models.GroupMembers m in oldGroupMembers)
                    {
                        m.GroupID = newGroupUID;

                        _entType = m.GetType().Name;
                        _entObject = JsonConvert.SerializeObject(m);

                        result = BVCommon.BVContextFunctions.UpdateToDb(_entType, _entObject, context);
                    }


                    return result;

                default:
                    return result;
            }

        }

        #endregion
    }
}
