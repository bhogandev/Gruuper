using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibleVerseAPI.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly BibleVerse.DALV2.BVIdentityContext context;
        private readonly BibleVerse.Repositories.JWTRepository jwtRepository;
        private IConfiguration _configuration;

        public ChatHub(BibleVerse.DALV2.BVIdentityContext _context, BibleVerse.Repositories.JWTRepository _jwtRepository, IConfiguration configuration)
        {
            this.context = _context;
            this.jwtRepository = _jwtRepository;
            this._configuration = configuration;
        }

        /*
         * [X] Log Connection Event
         * [X] Send Back ConnectionID to User
         * [X] Client Sends UserInfo
         * [X] Check if User Is In Any Groups (Group Members)
         * [X] Add ConnectionID to those groups
         * [X] Send Back List Of Groups User Is In via GroupNames/GroupIds
         * [X] User Is Able to Use Group Id To Send Messages To Prior Groups
         * [X] To Send Message To New Group User Sends UserName + Message
         * [X] Check against recipients to confirm there is no group that already exists
         * [X] If so use groupName instead of creating new group, otherwise create and log new group
         */

        public override Task OnConnectedAsync()
        {

            //Log Connection Event
            BibleVerse.Events.Event newConnectionEvent = new BibleVerse.Events.MessagingConnectEvent(context, Context.ConnectionId);

            //Send Connection ID back to Client
            Clients.Client(Context.ConnectionId).SendAsync("RecieveConnID", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public Task RetrieveUserGroups(string uCallPayload)
        {
            string userName = String.Empty;
            BibleVerse.DTO.RefreshRequest refreshRequest = new BibleVerse.DTO.RefreshRequest();

            List<BibleVerse.DTO.Models.Groups> groups = new List<BibleVerse.DTO.Models.Groups>();

            //Check if user is in any groups
            BibleVerse.DTO.ClientModels.InitialUCall uCall = JsonConvert.DeserializeObject<BibleVerse.DTO.ClientModels.InitialUCall>(uCallPayload);
            refreshRequest.AccessToken = uCall.token;

            userName = !String.IsNullOrEmpty(uCall.userName) ? uCall.userName : jwtRepository.FindUserFromAccessToken(refreshRequest).UserName;

            //Use Hub Helper to Get List Of User Groups
            groups = BibleVerse.Repositories.HubRepositories.HubHelper.RetrieveUserGroups(context, userName);

            foreach (BibleVerse.DTO.Models.Groups g in groups)
            {
                //Add user to each group in his session
                Groups.AddToGroupAsync(uCall.conId, g.GroupName);

                //Retrieve previous conversations for each group
                g.Conversation = BibleVerse.Repositories.HubRepositories.HubHelper.RetrievePreviousConversation(context, g);
            }

            return Clients.Client(uCall.conId).SendAsync("RetrieveUserGroups", groups);
        }

        public Task SendPrivateMessage(string UCallPayload) // Two parameters accepted
        {
            string groupID = string.Empty;
            string gName = string.Empty;

            try
            {
                //Deserialize Payload
                BibleVerse.DTO.ClientModels.ClientMessage message = JsonConvert.DeserializeObject<BibleVerse.DTO.ClientModels.ClientMessage>(UCallPayload);
                message.Message.MessageId = string.Empty;

                //Determine if groupID is included
                if (!string.IsNullOrEmpty(message.groupId))
                {
                    groupID = message.groupId;
                }
                else
                {
                    BibleVerse.DTO.RefreshRequest r = new BibleVerse.DTO.RefreshRequest() { AccessToken = message.token };

                    //Find user from token
                    message.Message.Username = jwtRepository.FindUserFromAccessToken(r).UserName;

                    //Add send user to group
                    message.Message.GroupRecipients.Add(message.Message.Username);

                    //Validate Recipient UserName
                    foreach (string groupMember in message.Message.GroupRecipients)
                    {
                        var qGMembers = from c in context.Users
                                        where c.UserName == groupMember
                                        select c;

                        if (qGMembers.FirstOrDefault() == null)
                        {
                            //throw err here how members do not exist
                            throw new BibleVerse.Exceptions.UserDoesNotExist(context, string.Format("User {0} Does Not Exist", groupMember));
                        }
                    }

                    //Create group Code to verify group doesnt exist
                    string groupCode = BibleVerse.Repositories.HubRepositories.HubHelper.GenGroupID(message.Message.GroupRecipients);


                    var qGroups = from c in context.Groups
                                  where c.GroupUID == groupCode
                                  select c;

                    if (qGroups.FirstOrDefault() == null)
                    {
                        BibleVerse.DTO.Models.Groups newGroup = new BibleVerse.DTO.Models.Groups()
                        {
                            GroupUID = groupCode,
                            GroupName = BibleVerse.Repositories.HubRepositories.HubHelper.GenGroupName(message.Message.GroupRecipients),
                            CreateDateTime = DateTime.Now,
                            ChangeDateTime = DateTime.Now
                        };

                        List<BibleVerse.DTO.Models.GroupMembers> newGroupMembers = new List<BibleVerse.DTO.Models.GroupMembers>();



                        foreach (string mem in message.Message.GroupRecipients)
                        {
                            BibleVerse.DTO.Models.GroupMembers newMember = new BibleVerse.DTO.Models.GroupMembers()
                            {
                                GroupID = groupCode,
                                UserName = mem,
                                CreateDateTime = DateTime.Now,
                                ChangeDateTime = DateTime.Now
                            };

                            newGroupMembers.Add(newMember);
                        }

                        //create the group
                        bool createGroupResult = BibleVerse.Repositories.HubRepositories.HubHelper.CreateNewGroup(context, newGroup, newGroupMembers);

                        groupID = groupCode.ToString();
                    }
                    else
                    {
                        groupID = groupCode.ToString();
                    }
                }

                //Get groupName from groupID
                gName = (from c in context.Groups
                         where c.GroupUID == groupID
                         select c).FirstOrDefault().GroupName;

                //Create Group In SignalR if User Isn't already
                Groups.AddToGroupAsync(message.conId, gName);
                Random ran = new Random();
                message.Message.MessageId = string.Format("{0}{1}{2}{3}", message.conId, groupID, ran.Next(1, 1000000000).ToString(), DateTime.Now.Millisecond);
                message.Message.Recipient = groupID;

                //Log message in db
                bool result = BibleVerse.Repositories.HubRepositories.HubHelper.SendPrivateMessage(context, message.Message, message.conId, groupID);

                if (result)
                {
                    BibleVerse.DTO.ClientModels.ResponseMessage responseMessage = new BibleVerse.DTO.ClientModels.ResponseMessage()
                    {
                        user = message.Message.Username,
                        message = message.Message.Body,
                        time = DateTime.Now.ToString("hh:mm tt")
                    };

                    return Clients.Group(gName).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(responseMessage));    // Note this 'ReceiveOne'
                }
                else
                {
                    //Let user know their message failed to send and allow them to retry
                    //To Do: Need to Create MessageFailedToSendException
                    return Clients.Client(message.conId).SendAsync("ReceiveMessage", message.Message.Username, message.Message.Body);
                }
            }
            catch (BibleVerse.Exceptions.UserDoesNotExist userdoesnotexist)
            {
                return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", "Recipient User Does Not Exist.");
            }
            catch (Exception ex)
            {
                //Create New Elog
                BibleVerse.Exceptions.BVException bvexception = new BibleVerse.Exceptions.BVException(context, string.Format("CahtHub: SendPrivateMessage \n {0} \n {1}", ex.Message, ex.InnerException), ex.StackTrace, 0);
                return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", bvexception.LoggedException.Message);
            }
        }

        public Task RemoveUserFromGroup(string uCallPayload)
        {
            bool result = false;

            //Deserialize Payload
            BibleVerse.DTO.ClientModels.GroupModificationRequest uCall = JsonConvert.DeserializeObject<BibleVerse.DTO.ClientModels.GroupModificationRequest>(uCallPayload);

            if(!string.IsNullOrEmpty(uCall.conID) || !string.IsNullOrEmpty(uCall.groupId) || !string.IsNullOrEmpty(uCall.modification))
            {
                //Verify modification is for removal
                if (uCall.modification == "REMOVAL") {
                    //CALL REMOVE FROM GROUP FUNC
                    
                    //Retrieve group that needs to be changed
                    var qGroup = (from c in context.Groups
                                 where c.GroupUID == uCall.groupId
                                 select c).FirstOrDefault();

                    //Grab group member to be removed from the call
                    var groupMember = (from x in context.GroupMembers
                                       where (x.GroupID == qGroup.GroupUID) && (x.UserName == uCall.userName)
                                       select x).FirstOrDefault();


                    result = BibleVerse.Repositories.HubRepositories.HubHelper.RemoveUserFromGroup(context, qGroup, groupMember);
                }
                else
                {
                    //return exception to user
                    BibleVerse.Exceptions.BVException exception = new BibleVerse.Exceptions.BVException(context, "Incorrect modiciation request per function called", 777);

                    return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", exception.LoggedException.Message);
                }
            }else
            {
                BibleVerse.Exceptions.BVException exception = new BibleVerse.Exceptions.BVException(context, "Null Value Provided For Group Modication Request", 777);

                //return null value exception
                return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", exception.LoggedException.Message);
            }

            if(result)
            {
                //return new group info to client for update
                return Clients.Client(Context.ConnectionId).SendAsync("UpdateGroupsInfo", string.Empty);
            }else
            {
                //return exception to alert user to try again or with error
                return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", "An Unepected Error Occured. Please try again");
            }
        }

        public Task InviteToGroup(string uCallPayload)
        {
            //Deserialize Payload
            BibleVerse.DTO.ClientModels.GroupModificationRequest uCall = JsonConvert.DeserializeObject<BibleVerse.DTO.ClientModels.GroupModificationRequest>(uCallPayload);

            if (!string.IsNullOrEmpty(uCall.conID) || !string.IsNullOrEmpty(uCall.groupId) || !string.IsNullOrEmpty(uCall.modification) || !string.IsNullOrEmpty(uCall.userName))
            {
                //Verify modification is for removal
                if (uCall.modification == "INVITE")
                {
                    var qGroup = (from c in context.Groups
                                  where c.GroupUID == uCall.groupId
                                  select c).FirstOrDefault();

                    var qUser = (from c in context.Users
                                 where c.UserName == uCall.userName
                                 select c).FirstOrDefault();

                    string appRoot = _configuration.GetConnectionString("AppRootProd");

                    //If API is running in test mode, send user root to stage UI
                    if (_configuration.GetValue<string>("RunInTestMode") == "Y")
                    {
                        appRoot = _configuration.GetConnectionString("AppRootStage");
                    }

                    //CALL INVITE TO GROUP FUNC
                    bool result = BibleVerse.Repositories.HubRepositories.HubHelper.InviteUserToGroup(context, qGroup, qUser, appRoot);

                    if (result)
                        return Clients.Client(Context.ConnectionId).SendAsync("InviteUserSuccessful", string.Format("{0} has been invited to the group!", qUser.UserName));
                    else
                        return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", "An unexpected error occured. Please try again.");
                }
                else
                {
                    //return exception to user
                    return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", "An unexpected error occured. Please try again.");
                }
            }
            else
            {
                //return null value exception
                return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", "Null value provided. Please try again.");
            }

        }

        public Task AddUserToGroup(string uCallPayload)
        {
            //Deserialize Payload
            BibleVerse.DTO.ClientModels.GroupModificationRequest uCall = JsonConvert.DeserializeObject<BibleVerse.DTO.ClientModels.GroupModificationRequest>(uCallPayload);

            if (!string.IsNullOrEmpty(uCall.conID) || !string.IsNullOrEmpty(uCall.groupId) || !string.IsNullOrEmpty(uCall.modification))
            {
                //Verify modification is for removal
                if (uCall.modification == "ADD")
                {
                    var qGroup = (from c in context.Groups
                                  where c.GroupUID == uCall.groupId
                                  select c).FirstOrDefault();

                    BibleVerse.DTO.Models.GroupMembers newMember = new BibleVerse.DTO.Models.GroupMembers()
                    {
                        GroupID = qGroup.GroupUID,
                        UserName = uCall.userName,
                        ChangeDateTime = DateTime.Now,
                        CreateDateTime = DateTime.Now
                    };

                    //CALL ADD to GROUP FUNC
                    bool result = BibleVerse.Repositories.HubRepositories.HubHelper.AddUserToGroup(context, qGroup, newMember);

                    if (result)
                        return Clients.Client(Context.ConnectionId).SendAsync("AddUserSuccessful", string.Format("{0} has been added to the group!", newMember.UserName));
                    else
                        return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", "An unexpected error occured. Please try again.");

                }
                else
                {
                    //return exception to user
                    return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", "An unexpected error occured. Please try again.");
                }
            }
            else
            {
                //return null value exception
                return Clients.Client(Context.ConnectionId).SendAsync("ReturnException", "Null value provided. Please try again.");
            }

        }
    }   
    }

