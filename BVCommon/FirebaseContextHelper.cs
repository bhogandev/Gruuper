using FireSharp.Response;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BVCommon
{
    public class FirebaseContextHelper
    {

         public static Type GetType(string type)
        {
            string s = type.ToUpper();

            Dictionary<string, Type> dbTypes = new Dictionary<string, Type>()
            {
                {"EVENT", typeof(BibleVerse.DTO.Event)},
                {"NOTIFICATIONS", typeof(BibleVerse.DTO.Notifications)},
                {"ELOG", typeof(BibleVerse.DTO.ELog)},
                {"COURSES", typeof(BibleVerse.DTO.Courses)},
                {"COMMENTS",  typeof(BibleVerse.DTO.Comments)},
                {"LIKES",  typeof(BibleVerse.DTO.Likes)},
                {"MESSAGES",  typeof(BibleVerse.DTO.Messages)},
                {"ASSIGNMENTS",  typeof(BibleVerse.DTO.Assignments)},
                {"ORGANIZATION",  typeof(BibleVerse.DTO.Organization)},
                {"ORGPROFILE",  typeof(BibleVerse.DTO.OrgProfile)},
                {"ORGSETTINGS",  typeof(BibleVerse.DTO.OrgSettings)},
                {"PHOTOS",  typeof(BibleVerse.DTO.Photos)},
                {"POSTS",  typeof(BibleVerse.DTO.Posts)},
                {"PROFILES",  typeof(BibleVerse.DTO.Profiles)},
                {"REFCODELOGS",  typeof(BibleVerse.DTO.RefCodeLogs)},
                {"POSTRELATIONS",  typeof(BibleVerse.DTO.PostsRelations)},
                {"SITECONFIGS",  typeof(BibleVerse.DTO.SiteConfigs)},
                {"SUBSCRIPTIONS",  typeof(BibleVerse.DTO.Subscriptions)},
                {"SUBSCRIPTIONSHISTORY",  typeof(BibleVerse.DTO.SubscriptionsHistory)},
                {"USERASSIGNMENTS",  typeof(BibleVerse.DTO.UserAssignments)},
                {"USERAWS",  typeof(BibleVerse.DTO.UserAWS)},
                {"USERCOURSES",  typeof(BibleVerse.DTO.UserCourses)},
                {"USERHISTORY",  typeof(BibleVerse.DTO.UserHistory)},
                {"VIDEOS",  typeof(BibleVerse.DTO.Videos)},
                {"USERRELATIONSHIPS",  typeof(BibleVerse.DTO.UserRelationships)},
                {"TRANSACTIONS", typeof(BibleVerse.DTO.Transactions)},
                { "GROUPS", typeof(BibleVerse.DTO.Models.Groups)},
                { "GROUPMEMBERS", typeof(BibleVerse.DTO.Models.GroupMembers)}
            };

            return dbTypes[s] != null ? dbTypes[s] : typeof(Exception);
        }

        public static bool WriteObject(FireSharp.FirebaseClient context, Type entType, string entObject)
        {
            switch (entType.FullName)
            {
                case "BibleVerse.DTO.Event":
                    BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                    context.Push("Events/" + newEvent.EventUID, newEvent);
                    return context.Get("Events/" + newEvent.EventUID).Body != null ? true : false;

                case "BibleVerse.DTO.Notifications":
                    BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                    context.Push("Notifications/" + newNotification.NotificationID, newNotification);
                    return context.Get("Notifications/" + newNotification.NotificationID).Body != null ? true : false;

                case "BibleVerse.DTO.ELog":
                    BibleVerse.DTO.ELog newELog = JsonConvert.DeserializeObject<BibleVerse.DTO.ELog>(entObject);
                    context.Push("Elogs/" + newELog.ElogID, newELog);
                    return context.Get("ELogs/" + newELog.ElogID).Body != null ? true : false;
                
                case "BibleVerse.DTO.Courses":
                    BibleVerse.DTO.Courses newCourse = JsonConvert.DeserializeObject<BibleVerse.DTO.Courses>(entObject);
                    context.Push("Courses/" + newCourse.CourseId, newCourse);
                    return context.Get("Courses/" + newCourse.CourseId).Body != null ? true : false;

                case "BibleVerse.DTO.Comments":
                    BibleVerse.DTO.Comments newComment = JsonConvert.DeserializeObject<BibleVerse.DTO.Comments>(entObject);
                    context.Push("Comments/" + newComment.Id, newComment);
                    return context.Get("Comments/" + newComment.Id).Body != null ? true : false;

                case "BibleVerse.DTO.Likes":
                    BibleVerse.DTO.Likes newLike = JsonConvert.DeserializeObject<BibleVerse.DTO.Likes>(entObject);
                    context.Push("Likes/" + newLike.Id, newLike);
                    return context.Get("Likes/" + newLike.Id).Body != null ? true : false;

                case "BibleVerse.DTO.Photos":
                    BibleVerse.DTO.Photos newPhoto = JsonConvert.DeserializeObject<BibleVerse.DTO.Photos>(entObject);
                    context.Push("Photos/" + newPhoto.PhotoId, newPhoto);
                    FirebaseResponse photoResponse = context.Get("Photos/" + newPhoto.PhotoId);
                    return photoResponse.Body != "null";

                case "BibleVerse.DTO.Videos":
                    BibleVerse.DTO.Videos newVideo = JsonConvert.DeserializeObject<BibleVerse.DTO.Videos>(entObject);
                    context.Push("Videos/" + newVideo.VideoId, newVideo);
                    FirebaseResponse videoResponse = context.Get("Videos/" + newVideo.VideoId);
                    return videoResponse.Body != "null";

                case "BibleVerse.DTO.Posts":
                    BibleVerse.DTO.Posts newPosts = JsonConvert.DeserializeObject<BibleVerse.DTO.Posts>(entObject);
                    context.Push("Posts/" + newPosts.PostId, newPosts);
                    FirebaseResponse postResponse = context.Get("Posts/" + newPosts.PostId);
                    return postResponse.Body != "null";

                case "BibleVerse.DTO.Messages":
                    BibleVerse.DTO.Messages newMessage = JsonConvert.DeserializeObject<BibleVerse.DTO.Messages>(entObject);
                    context.Push("Messages/" + newMessage.MessageId ,newMessage);
                    return context.Get("Messages/" + newMessage.MessageId).Body != null ? true : false;

                case "BibleVerse.DTO.Assignments":
                    BibleVerse.DTO.Assignments newAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.Assignments>(entObject);
                    context.Push("Assignments/" + newAssignment.AssignmentId, newAssignment);
                    FirebaseResponse assignmentResponse = context.Get("Assignments/" + newAssignment.AssignmentId);
                    return assignmentResponse.Body != "null";

                case "BibleVerse.DTO.Organization":
                    BibleVerse.DTO.Organization newOrganization = JsonConvert.DeserializeObject<BibleVerse.DTO.Organization>(entObject);
                    context.Push("Organization/" + newOrganization.OrganizationId, newOrganization);
                    FirebaseResponse organizationResponse = context.Get("Organization/" + newOrganization.OrganizationId);
                    return organizationResponse.Body != "null";

                case "BibleVerse.DTO.OrgSettings":
                    BibleVerse.DTO.OrgSettings newOrgSettings = JsonConvert.DeserializeObject<BibleVerse.DTO.OrgSettings>(entObject);
                    context.Push("OrgSettings/" + newOrgSettings.OrgSettingsId, newOrgSettings);
                    FirebaseResponse orgSettingsResponse = context.Get("OrgSettings/" + newOrgSettings.OrgSettingsId);
                    return orgSettingsResponse.Body != "null";

                case "BibleVerse.DTO.Profiles":
                    BibleVerse.DTO.Profiles newProfile = JsonConvert.DeserializeObject<BibleVerse.DTO.Profiles>(entObject);
                    context.Push("Profiles/" + newProfile.ProfileId, newProfile);
                    FirebaseResponse profileResponse = context.Get("Profiles/" + newProfile.ProfileId);
                    return profileResponse.Body != "null";

                case "BibleVerse.DTO.RefCodeLogs":
                    BibleVerse.DTO.RefCodeLogs newRefCodeLog = JsonConvert.DeserializeObject<BibleVerse.DTO.RefCodeLogs>(entObject);
                    context.Push("RefCodeLogs/" + newRefCodeLog.Key, newRefCodeLog);
                    FirebaseResponse refCodeLogResponse = context.Get("RefCodeLogs/" + newRefCodeLog.Key);
                    return refCodeLogResponse.Body != "null";

                case "BibleVerse.DTO.SiteConfig":
                    BibleVerse.DTO.SiteConfigs newSiteConfig = JsonConvert.DeserializeObject<BibleVerse.DTO.SiteConfigs>(entObject);
                    context.Set("SiteConfigs/" + newSiteConfig.Key, newSiteConfig);
                    FirebaseResponse siteConfigResponse = context.Get("SiteConfigs/" + newSiteConfig.Key);
                    return siteConfigResponse.Body != "null";

                case "BibleVerse.DTO.Subscriptions":
                    BibleVerse.DTO.Subscriptions newSubscription = JsonConvert.DeserializeObject<BibleVerse.DTO.Subscriptions>(entObject);
                    context.Push("Subscriptions/" + newSubscription.SubscriptionID, newSubscription);
                    FirebaseResponse subscriptionResponse = context.Get("Subscriptions/" + newSubscription.SubscriptionID);
                    return subscriptionResponse.Body != "null";

                case "BibleVerse.DTO.SubscriptionsHistory":
                    BibleVerse.DTO.SubscriptionsHistory newSubscriptionHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.SubscriptionsHistory>(entObject);
                    context.Push("SubscriptionsHistory/" + newSubscriptionHistory.RecordID, newSubscriptionHistory);
                    FirebaseResponse subscriptionHistoryResponse = context.Get("SubscriptionsHistory/" + newSubscriptionHistory.RecordID);
                    return subscriptionHistoryResponse.Body != "null";

                case "BibleVerse.DTO.UserAssignments":
                    BibleVerse.DTO.UserAssignments newUserAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAssignments>(entObject);
                    context.Push("UserAssignments/" + newUserAssignment.AssignmentId, newUserAssignment);
                    FirebaseResponse userAssignmentResponse = context.Get("UserAssignments/" + newUserAssignment.AssignmentId);
                    return userAssignmentResponse.Body != "null";

                case "BibleVerse.DTO.UserAWS":
                    BibleVerse.DTO.UserAWS newUserAWS = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAWS>(entObject);
                    context.Push("UserAWS/" + newUserAWS.ID, newUserAWS);
                    FirebaseResponse userAWSResponse = context.Get("UserAWS/" + newUserAWS.ID);
                    return userAWSResponse.Body != "null";

                case "BibleVerse.DTO.UserHistory":
                    BibleVerse.DTO.UserHistory newUserHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.UserHistory>(entObject);
                    context.Push("UserHistory/" + newUserHistory.ActionID, newUserHistory);
                    FirebaseResponse userHistoryResponse = context.Get("UserHistory/" + newUserHistory.ActionID);
                    return userHistoryResponse.Body != "null";

                case "BibleVerse.DTO.UserRelationships":
                    BibleVerse.DTO.UserRelationships newUserRelationship = JsonConvert.DeserializeObject<BibleVerse.DTO.UserRelationships>(entObject);
                    context.Push("UserRelationships/" + newUserRelationship.RelationshipID, newUserRelationship);
                    FirebaseResponse userRelationshipResponse = context.Get("UserRelationships/" + newUserRelationship.RelationshipID);
                    return userRelationshipResponse.Body != "null";

                case "BibleVerse.DTO.Transactions":
                    BibleVerse.DTO.Transactions newTransaction = JsonConvert.DeserializeObject<BibleVerse.DTO.Transactions>(entObject);
                    context.Push("Transactions/" + newTransaction.TransactionUID, newTransaction);
                    FirebaseResponse transactionResponse = context.Get("Transactions/" + newTransaction.TransactionUID);
                    return transactionResponse.Body != "null";

                case "BibleVerse.DTO.Models.Groups":
                    BibleVerse.DTO.Models.Groups newGroup = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.Groups>(entObject);
                    context.Push("Groups/" + newGroup.GroupID, newGroup);
                    FirebaseResponse groupResponse = context.Get("Groups/" + newGroup.GroupID);
                    return groupResponse.Body != "null";

                case "BibleVerse.DTO.Models.GroupMembers":
                    BibleVerse.DTO.Models.GroupMembers newGroupMember = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.GroupMembers>(entObject);
                    context.Push("GroupMembers/" + newGroupMember.GroupID + "/" + newGroupMember.UserName, newGroupMember);
                    FirebaseResponse groupMemberResponse = context.Get("GroupMembers/" + newGroupMember.GroupID + "/" + newGroupMember.UserName);
                    return groupMemberResponse.Body != "null";

            default:
                    return false;
            }
        }

        public static bool DeleteObject(FireSharp.FirebaseClient context, Type entType, string entObject)
        {
            switch (entType.FullName)
            {
                
                case "BibleVerse.DTO.Event":
                    BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                    context.Delete("Events/" + newEvent.EventUID);

                    var eventResponse = context.Get("Events/" + newEvent.EventUID);
                    return eventResponse.Body == "null";

                
                case "BibleVerse.DTO.Notifications":
                    BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                    context.Delete("Notifications/" + newNotification.NotificationID);

                    var notificationResponse = context.Get("Notifications/" + newNotification.NotificationID);
                    return notificationResponse.Body == "null";

                
                case "BibleVerse.DTO.ELog":
                    BibleVerse.DTO.ELog newELog = JsonConvert.DeserializeObject<BibleVerse.DTO.ELog>(entObject);
                    context.Delete("ELogs/" + newELog.ElogID);

                    var eLogResponse = context.Get("ELogs/" + newELog.ElogID);
                    return eLogResponse.Body == "null";

                
                case "BibleVerse.DTO.Courses":
                    BibleVerse.DTO.Courses newCourse = JsonConvert.DeserializeObject<BibleVerse.DTO.Courses>(entObject);
                    context.Delete("Courses/" + newCourse.CourseId);

                    var courseResponse = context.Get("Courses/" + newCourse.CourseId);
                    return courseResponse.Body == "null";

                
                case "BibleVerse.DTO.Comments":
                    BibleVerse.DTO.Comments newComment = JsonConvert.DeserializeObject<BibleVerse.DTO.Comments>(entObject);
                    context.Delete("Comments/" + newComment.Id);

                    var commentResponse = context.Get("Comments/" + newComment.Id);
                    return commentResponse.Body == "null";

                
                case "BibleVerse.DTO.Likes":
                    BibleVerse.DTO.Likes newLike = JsonConvert.DeserializeObject<BibleVerse.DTO.Likes>(entObject);
                    context.Delete("Likes/" + newLike.Id);
                    var likeResponse = context.Get("Likes/" + newLike.Id);
                    return likeResponse.Body == "null";

                case "BibleVerse.DTO.Videos":
                    BibleVerse.DTO.Videos newVideo = JsonConvert.DeserializeObject<BibleVerse.DTO.Videos>(entObject);
                    context.Delete("Videos/" + newVideo.VideoId);

                    var videoResponse = context.Get("Videos/" + newVideo.VideoId);
                    return videoResponse.Body == "null";

                case "BibleVerse.DTO.Posts":
                    BibleVerse.DTO.Posts newPosts = JsonConvert.DeserializeObject<BibleVerse.DTO.Posts>(entObject);
                    context.Delete("Posts/" + newPosts.PostId);

                    var postResponse = context.Get("Posts/" + newPosts.PostId);
                    return postResponse.Body == "null";

                case "BibleVerse.DTO.Messages":
                    BibleVerse.DTO.Messages newMessage = JsonConvert.DeserializeObject<BibleVerse.DTO.Messages>(entObject);
                    context.Delete("Messages/" + newMessage.MessageId);

                    var messageResponse = context.Get("Messages/" + newMessage.MessageId);
                    return messageResponse.Body == "null";

                case "BibleVerse.DTO.Assignments":
                    BibleVerse.DTO.Assignments newAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.Assignments>(entObject);
                    context.Delete("Assignments/" + newAssignment.AssignmentId);

                    var assignmentResponse = context.Get("Assignments/" + newAssignment.AssignmentId);
                    return assignmentResponse.Body == "null";

                case "BibleVerse.DTO.Organization":
                    BibleVerse.DTO.Organization newOrganization = JsonConvert.DeserializeObject<BibleVerse.DTO.Organization>(entObject);
                    context.Delete("Organization/" + newOrganization.OrganizationId);

                    var orgResponse = context.Get("Organization/" + newOrganization.OrganizationId);
                    return orgResponse.Body == "null";

                case "BibleVerse.DTO.OrgSettings":
                    BibleVerse.DTO.OrgSettings newOrgSettings = JsonConvert.DeserializeObject<BibleVerse.DTO.OrgSettings>(entObject);
                    context.Delete("OrgSettings/" + newOrgSettings.OrgSettingsId);

                    var orgSettingsResponse = context.Get("OrgSettings/" + newOrgSettings.OrgSettingsId);
                    return orgSettingsResponse.Body == "null";

                case "BibleVerse.DTO.Profiles":
                    BibleVerse.DTO.Profiles newProfile = JsonConvert.DeserializeObject<BibleVerse.DTO.Profiles>(entObject);
                    context.Delete("Profiles/" + newProfile.ProfileId);

                    var profileResponse = context.Get("Profiles/" + newProfile.ProfileId);
                    return profileResponse.Body == "null";

                case "BibleVerse.DTO.RefCodeLogs":
                    BibleVerse.DTO.RefCodeLogs newRefCodeLog = JsonConvert.DeserializeObject<BibleVerse.DTO.RefCodeLogs>(entObject);
                    context.Delete("RefCodeLogs/" + newRefCodeLog.Key);

                    var refCodeLogResponse = context.Get("RefCodeLogs/" + newRefCodeLog.Key);
                    return refCodeLogResponse.Body == "null";

                case "BibleVerse.DTO.SiteConfigs":
                    BibleVerse.DTO.SiteConfigs newSiteConfig = JsonConvert.DeserializeObject<BibleVerse.DTO.SiteConfigs>(entObject);
                    context.Delete("SiteConfigs/" + newSiteConfig.Key);

                    var siteConfigResponse = context.Get("SiteConfigs/" + newSiteConfig.Key);
                    return siteConfigResponse.Body == "null";

                case "BibleVerse.DTO.Subscriptions":
                    BibleVerse.DTO.Subscriptions newSubscription = JsonConvert.DeserializeObject<BibleVerse.DTO.Subscriptions>(entObject);
                    context.Delete("Subscriptions/" + newSubscription.SubscriptionID);

                    var subscriptionResponse = context.Get("Subscriptions/" + newSubscription.SubscriptionID);
                    return subscriptionResponse.Body == "null";

                case "BibleVerse.DTO.SubscriptionsHistory":
                    BibleVerse.DTO.SubscriptionsHistory newSubscriptionHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.SubscriptionsHistory>(entObject);
                    context.Delete("SubscriptionsHistory/" + newSubscriptionHistory.RecordID);
                    var subscriptionHistoryResponse = context.Get("SubscriptionsHistory/" + newSubscriptionHistory.RecordID);
                    return subscriptionHistoryResponse.Body == "null";

                case "BibleVerse.DTO.UserAssignments":
                    BibleVerse.DTO.UserAssignments newUserAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAssignments>(entObject);
                    context.Delete("UserAssignments/" + newUserAssignment.AssignmentId);

                    var userAssignmentResponse = context.Get("UserAssignments/" + newUserAssignment.AssignmentId);
                    return userAssignmentResponse.Body == "null";

                case "BibleVerse.DTO.UserAWS":
                    BibleVerse.DTO.UserAWS newUserAWS = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAWS>(entObject);
                    context.Delete("UserAWS/" + newUserAWS.ID);

                    var userAWSResponse = context.Get("UserAWS/" + newUserAWS.ID);
                    return userAWSResponse.Body == "null";

                case "BibleVerse.DTO.UserHistory":
                    BibleVerse.DTO.UserHistory newUserHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.UserHistory>(entObject);
                    context.Delete("UserHistory/" + newUserHistory.ActionID);

                    var userHistoryResponse = context.Get("UserHistory/" + newUserHistory.ActionID);
                    return userHistoryResponse.Body == "null";

                case "BibleVerse.DTO.UserRelationships":
                    BibleVerse.DTO.UserRelationships newUserRelationship = JsonConvert.DeserializeObject<BibleVerse.DTO.UserRelationships>(entObject);
                    context.Delete("UserRelationships/" + newUserRelationship.RelationshipID);

                    var userRelationshipResponse = context.Get("UserRelationships/" + newUserRelationship.RelationshipID);
                    return userRelationshipResponse.Body == "null";

                case "BibleVerse.DTO.Models.Groups":
                    BibleVerse.DTO.Models.Groups newGroup = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.Groups>(entObject);
                    context.Delete("Groups/" + newGroup.GroupID);

                    var groupResponse = context.Get("Groups/" + newGroup.GroupID);
                    return groupResponse.Body == "null";

                case "BibleVerse.DTO.Models.GroupMembers":
                    BibleVerse.DTO.Models.GroupMembers newGroupMember = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.GroupMembers>(entObject);
                    context.Delete("GroupMembers/" + newGroupMember.ID);
                    
                    return context.Get("GroupMembers/" + newGroupMember.ID) != null ? false : true;

                default:
                    return false;
            }
        }

        public static bool UpdateObject(FireSharp.FirebaseClient context, Type entType, string entObject)
        {
            try
            {
                switch (entType.FullName)
                {
                    case "BibleVerse.DTO.Event":
                        BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                        BibleVerse.DTO.Event oldEvent = context.Get("Events", FireSharp.QueryBuilder.New("Events").OrderBy("EventUID").LimitToFirst(1)).ResultAs<BibleVerse.DTO.Event>();
                        context.Update("Events/" + newEvent.EventUID, newEvent);

                        var updatedEvent = context.Get("Events/" + newEvent.EventUID).ResultAs<BibleVerse.DTO.Event>();
                        return updatedEvent != oldEvent && updatedEvent != null ? true : false;

                    case "BibleVerse.DTO.Notifications":
                        BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                        BibleVerse.DTO.Notifications oldNotification = context.Get("Notifications/" + newNotification.NotificationID).ResultAs<BibleVerse.DTO.Notifications>();
                        context.Update("Notifications/" + newNotification.NotificationID, newNotification);

                        var updatedNotification = context.Get("Notifications/" + newNotification.NotificationID).ResultAs<BibleVerse.DTO.Notifications>();
                        return updatedNotification != oldNotification && updatedNotification != null ? true : false;

                    case "BibleVerse.DTO.ELog":
                        BibleVerse.DTO.ELog newELog = JsonConvert.DeserializeObject<BibleVerse.DTO.ELog>(entObject);
                        BibleVerse.DTO.ELog oldELog = context.Get("ELogs/" + newELog.ElogID).ResultAs<BibleVerse.DTO.ELog>();
                        context.Update("ELogs/" + newELog.ElogID, newELog);

                        var updatedELog = context.Get("ELogs/" + newELog.ElogID).ResultAs<BibleVerse.DTO.ELog>();
                        return updatedELog != oldELog && updatedELog != null ? true : false;


                    case "BibleVerse.DTO.Courses":
                        BibleVerse.DTO.Courses newCourse = JsonConvert.DeserializeObject<BibleVerse.DTO.Courses>(entObject);
                        BibleVerse.DTO.Courses oldCourse = context.Get("Courses", FireSharp.QueryBuilder.New("Courses").OrderBy("CourseId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.Courses>();
                        context.Update("Courses/" + newCourse.CourseId, newCourse);

                        return context.Get("Courses/" + newCourse.CourseId).ResultAs<BibleVerse.DTO.Courses>() != oldCourse && context.Get("Courses/" + newCourse.CourseId).ResultAs<BibleVerse.DTO.Courses>() != null ? true : false;


                    case "BibleVerse.DTO.Comments":
                        BibleVerse.DTO.Comments newComment = JsonConvert.DeserializeObject<BibleVerse.DTO.Comments>(entObject);
                        BibleVerse.DTO.Comments oldComment = context.Get("Comments/" + newComment.Id).ResultAs<BibleVerse.DTO.Comments>();
                        context.Update("Comments/" + newComment.Id, newComment);

                        return context.Get("Comments/" + newComment.Id).ResultAs<BibleVerse.DTO.Comments>() != oldComment && context.Get("Comments/" + newComment.Id).ResultAs<BibleVerse.DTO.Comments>() != null ? true : false;


                    case "BibleVerse.DTO.Likes":
                        BibleVerse.DTO.Likes newLike = JsonConvert.DeserializeObject<BibleVerse.DTO.Likes>(entObject);
                        BibleVerse.DTO.Likes oldLike = context.Get("Likes", FireSharp.QueryBuilder.New("Likes")
                            .OrderBy("LikeId")
                            .LimitToFirst(1)).ResultAs<BibleVerse.DTO.Likes>();
                        context.Update("Likes/" + newLike.Id, newLike);

                        return context.Get("Likes/" + newLike.Id).ResultAs<BibleVerse.DTO.Likes>() != oldLike && context.Get("Likes/" + newLike.Id).ResultAs<BibleVerse.DTO.Likes>() != null ? true : false;

                    case "BibleVerse.DTO.Photos":
                        BibleVerse.DTO.Photos newPhoto = JsonConvert.DeserializeObject<BibleVerse.DTO.Photos>(entObject);
                        BibleVerse.DTO.Photos oldPhoto = context.Get("Photos", FireSharp.QueryBuilder.New("Photos")
                            .OrderBy("PhotoId")
                            .LimitToFirst(1)).ResultAs<BibleVerse.DTO.Photos>();
                        context.Update("Photos/" + newPhoto.PhotoId, newPhoto);

                        return context.Get("Photos/" + newPhoto.PhotoId).ResultAs<BibleVerse.DTO.Photos>() != oldPhoto && context.Get("Photos/" + newPhoto.PhotoId).ResultAs<BibleVerse.DTO.Photos>() != null ? true : false;

                    case "BibleVerse.DTO.Videos":
                        BibleVerse.DTO.Videos newVideo = JsonConvert.DeserializeObject<BibleVerse.DTO.Videos>(entObject);
                        BibleVerse.DTO.Videos oldVideo = context.Get("Videos", FireSharp.QueryBuilder.New("Videos")
                            .OrderBy("VideoId")
                            .LimitToFirst(1)).ResultAs<BibleVerse.DTO.Videos>();
                        context.Update("Videos/" + newVideo.VideoId, newVideo);

                        return context.Get("Videos/" + newVideo.VideoId).ResultAs<BibleVerse.DTO.Videos>() != oldVideo && context.Get("Videos/" + newVideo.VideoId).ResultAs<BibleVerse.DTO.Videos>() != null ? true : false;

                    case "BibleVerse.DTO.Posts":
                        BibleVerse.DTO.Posts newPost = JsonConvert.DeserializeObject<BibleVerse.DTO.Posts>(entObject);
                        BibleVerse.DTO.Posts oldPost = context.Get("Posts", FireSharp.QueryBuilder.New("Posts")
                            .OrderBy("PostId")
                            .LimitToFirst(1)).ResultAs<BibleVerse.DTO.Posts>();
                        context.Update("Posts/" + newPost.PostId, newPost);

                        return context.Get("Posts/" + newPost.PostId).ResultAs<BibleVerse.DTO.Posts>() != oldPost && context.Get("Posts/" + newPost.PostId).ResultAs<BibleVerse.DTO.Posts>() != null ? true : false;

                    case "BibleVerse.DTO.Messages":
                        BibleVerse.DTO.Messages newMessage = JsonConvert.DeserializeObject<BibleVerse.DTO.Messages>(entObject);
                        BibleVerse.DTO.Messages oldMessage = context.Get("Messages", FireSharp.QueryBuilder.New("Messages")
                            .OrderBy("MessageId")
                            .LimitToFirst(1)).ResultAs<BibleVerse.DTO.Messages>();
                        context.Update("Messages/" + newMessage.MessageId, newMessage);

                        return context.Get("Messages/" + newMessage.MessageId).ResultAs<BibleVerse.DTO.Messages>() != oldMessage && context.Get("Messages/" + newMessage.MessageId).ResultAs<BibleVerse.DTO.Messages>() != null ? true : false;


                    case "BibleVerse.DTO.Assignments":
                        BibleVerse.DTO.Assignments newAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.Assignments>(entObject);
                        BibleVerse.DTO.Assignments oldAssignment = context.Get("Assignments", FireSharp.QueryBuilder.New("Assignments").OrderBy("AssignmentId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.Assignments>();
                        context.Update("Assignments/" + newAssignment.AssignmentId, newAssignment);

                        return context.Get("Assignments/" + newAssignment.AssignmentId).ResultAs<BibleVerse.DTO.Assignments>() != oldAssignment && context.Get("Assignments/" + newAssignment.AssignmentId).ResultAs<BibleVerse.DTO.Assignments>() != null ? true : false;

                    case "BibleVerse.DTO.Organization":
                        BibleVerse.DTO.Organization newOrganization = JsonConvert.DeserializeObject<BibleVerse.DTO.Organization>(entObject);
                        BibleVerse.DTO.Organization oldOrganization = context.Get("Organization", FireSharp.QueryBuilder.New("Organization").OrderBy("OrganizationId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.Organization>();
                        context.Update("Organization/" + newOrganization.OrganizationId, newOrganization);

                        return context.Get("Organization/" + newOrganization.OrganizationId).ResultAs<BibleVerse.DTO.Organization>() != oldOrganization && context.Get("Organization/" + newOrganization.OrganizationId).ResultAs<BibleVerse.DTO.Organization>() != null ? true : false;

                    case "BibleVerse.DTO.OrgSettings":
                        BibleVerse.DTO.OrgSettings newOrgSettings = JsonConvert.DeserializeObject<BibleVerse.DTO.OrgSettings>(entObject);
                        BibleVerse.DTO.OrgSettings oldOrgSettings = context.Get("OrgSettings", FireSharp.QueryBuilder.New("OrgSettings").OrderBy("OrgSettingsId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.OrgSettings>();
                        context.Update("OrgSettings/" + newOrgSettings.OrgSettingsId, newOrgSettings);

                        return context.Get("OrgSettings/" + newOrgSettings.OrgSettingsId).ResultAs<BibleVerse.DTO.OrgSettings>() != oldOrgSettings && context.Get("OrgSettings/" + newOrgSettings.OrgSettingsId).ResultAs<BibleVerse.DTO.OrgSettings>() != null ? true : false;

                    case "BibleVerse.DTO.Profiles":
                        BibleVerse.DTO.Profiles newProfile = JsonConvert.DeserializeObject<BibleVerse.DTO.Profiles>(entObject);
                        BibleVerse.DTO.Profiles oldProfile = context.Get("Profiles", FireSharp.QueryBuilder.New("Profiles").OrderBy("ProfileId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.Profiles>();
                        context.Update("Profiles/" + newProfile.ProfileId, newProfile);

                        return context.Get("Profiles/" + newProfile.ProfileId).ResultAs<BibleVerse.DTO.Profiles>() != oldProfile && context.Get("Profiles/" + newProfile.ProfileId).ResultAs<BibleVerse.DTO.Profiles>() != null ? true : false;

                    case "BibleVerse.DTO.RefCodeLogs":
                        BibleVerse.DTO.RefCodeLogs newRefCodeLog = JsonConvert.DeserializeObject<BibleVerse.DTO.RefCodeLogs>(entObject);
                        BibleVerse.DTO.RefCodeLogs oldRefCodeLog = context.Get("RefCodeLogs", FireSharp.QueryBuilder.New("RefCodeLogs").OrderBy("RefCodeLogId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.RefCodeLogs>();
                        context.Update("RefCodeLogs/" + newRefCodeLog.Key, newRefCodeLog);

                        return context.Get("RefCodeLogs/" + newRefCodeLog.Key).ResultAs<BibleVerse.DTO.RefCodeLogs>() != oldRefCodeLog && context.Get("RefCodeLogs/" + newRefCodeLog.Key).ResultAs<BibleVerse.DTO.RefCodeLogs>() != null ? true : false;

                    case "BibleVerse.DTO.SiteConfigs":
                        BibleVerse.DTO.SiteConfigs newSiteConfig = JsonConvert.DeserializeObject<BibleVerse.DTO.SiteConfigs>(entObject);
                        BibleVerse.DTO.SiteConfigs oldSiteConfig = context.Get("SiteConfigs", FireSharp.QueryBuilder.New("SiteConfigs").OrderBy("SiteConfigId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.SiteConfigs>();
                        context.Update("SiteConfigs/" + newSiteConfig.Key, newSiteConfig);

                        return context.Get("SiteConfigs/" + newSiteConfig.Key).ResultAs<BibleVerse.DTO.SiteConfigs>() != oldSiteConfig && context.Get("SiteConfigs/" + newSiteConfig.Key).ResultAs<BibleVerse.DTO.SiteConfigs>() != null ? true : false;

                    case "BibleVerse.DTO.Subscriptions":
                        BibleVerse.DTO.Subscriptions newSubscription = JsonConvert.DeserializeObject<BibleVerse.DTO.Subscriptions>(entObject);
                        BibleVerse.DTO.Subscriptions oldSubscription = context.Get("Subscriptions", FireSharp.QueryBuilder.New("Subscriptions").OrderBy("SubscriptionId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.Subscriptions>();
                        context.Update("Subscriptions/" + newSubscription.SubscriptionID, newSubscription);

                        return context.Get("Subscriptions/" + newSubscription.SubscriptionID).ResultAs<BibleVerse.DTO.Subscriptions>() != oldSubscription && context.Get("Subscriptions/" + newSubscription.SubscriptionID).ResultAs<BibleVerse.DTO.Subscriptions>() != null ? true : false;

                    case "BibleVerse.DTO.SubscriptionsHistory":
                        BibleVerse.DTO.SubscriptionsHistory newSubscriptionHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.SubscriptionsHistory>(entObject);
                        BibleVerse.DTO.SubscriptionsHistory oldSubscriptionHistory = context.Get("SubscriptionsHistory", FireSharp.QueryBuilder.New("SubscriptionsHistory").OrderBy("SubscriptionHistoryId").LimitToFirst(1)).ResultAs<BibleVerse.DTO.SubscriptionsHistory>();
                        context.Update("SubscriptionsHistory/" + newSubscriptionHistory.RecordID, newSubscriptionHistory);

                        return context.Get("SubscriptionsHistory/" + newSubscriptionHistory.RecordID).ResultAs<BibleVerse.DTO.SubscriptionsHistory>() != oldSubscriptionHistory && context.Get("SubscriptionsHistory/" + newSubscriptionHistory.RecordID).ResultAs<BibleVerse.DTO.SubscriptionsHistory>() != null ? true : false;


                    case "BibleVerse.DTO.UserAssignments":
                        BibleVerse.DTO.UserAssignments newUserAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAssignments>(entObject);
                        BibleVerse.DTO.UserAssignments oldUserAssignment = context.Get("UserAssignments", FireSharp.QueryBuilder.New("UserAssignments").OrderBy("AssignmentID").LimitToFirst(1)).ResultAs<BibleVerse.DTO.UserAssignments>();
                        context.Update("UserAssignments/" + newUserAssignment.AssignmentId, newUserAssignment);

                        return context.Get("UserAssignments/" + newUserAssignment.AssignmentId).ResultAs<BibleVerse.DTO.UserAssignments>() != oldUserAssignment && context.Get("UserAssignments/" + newUserAssignment.AssignmentId).ResultAs<BibleVerse.DTO.UserAssignments>() != null ? true : false;


                    case "BibleVerse.DTO.UserAWS":
                        BibleVerse.DTO.UserAWS newUserAWS = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAWS>(entObject);
                        BibleVerse.DTO.UserAWS oldUserAWS = context.Get("UserAWS/" + newUserAWS.ID).ResultAs<BibleVerse.DTO.UserAWS>();
                        context.Update("UserAWS/" + newUserAWS.ID, newUserAWS);
                        return context.Get("UserAWS/" + newUserAWS.ID).ResultAs<BibleVerse.DTO.UserAWS>() != oldUserAWS && context.Get("UserAWS/" + newUserAWS.ID).ResultAs<BibleVerse.DTO.UserAWS>() != null;

                    case "BibleVerse.DTO.UserHistory":
                        BibleVerse.DTO.UserHistory newUserHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.UserHistory>(entObject);
                        BibleVerse.DTO.UserHistory oldUserHistory = context.Get("UserHistory/" + newUserHistory.ActionID).ResultAs<BibleVerse.DTO.UserHistory>();
                        context.Update("UserHistory/" + newUserHistory.ActionID, newUserHistory);
                        return context.Get("UserHistory/" + newUserHistory.ActionID).ResultAs<BibleVerse.DTO.UserHistory>() != oldUserHistory && context.Get("UserHistory/" + newUserHistory.ActionID).ResultAs<BibleVerse.DTO.UserHistory>() != null;


                    case "BibleVerse.DTO.UserRelationships":
                        BibleVerse.DTO.UserRelationships newUserRelationship = JsonConvert.DeserializeObject<BibleVerse.DTO.UserRelationships>(entObject);
                        BibleVerse.DTO.UserRelationships oldUserRelationship = context.Get("UserRelationships/" + newUserRelationship.RelationshipID).ResultAs<BibleVerse.DTO.UserRelationships>();
                        context.Update("UserRelationships/" + newUserRelationship.RelationshipID, newUserRelationship);
                        return context.Get("UserRelationships/" + newUserRelationship.RelationshipID).ResultAs<BibleVerse.DTO.UserRelationships>() != oldUserRelationship && context.Get("UserRelationships/" + newUserRelationship.RelationshipID).ResultAs<BibleVerse.DTO.UserRelationships>() != null;

                    case "BibleVerse.DTO.Models.Groups":
                        BibleVerse.DTO.Models.Groups newGroup = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.Groups>(entObject);
                        BibleVerse.DTO.Models.Groups oldGroup = context.Get("Groups/" + newGroup.GroupID).ResultAs<BibleVerse.DTO.Models.Groups>();
                        context.Update("Groups/" + newGroup.GroupID, newGroup);

                        return context.Get("Groups/" + newGroup.GroupID).ResultAs<BibleVerse.DTO.Models.Groups>() != oldGroup && context.Get("Groups/" + newGroup.GroupID).ResultAs<BibleVerse.DTO.Models.Groups>() != null ? true : false;

                    case "BibleVerse.DTO.Models.GroupMembers":
                        BibleVerse.DTO.Models.GroupMembers newGroupMember = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.GroupMembers>(entObject);
                        BibleVerse.DTO.Models.GroupMembers oldGroupMember = context.Get("GroupMembers/" + newGroupMember.GroupID).ResultAs<BibleVerse.DTO.Models.GroupMembers>();
                        context.Update("GroupMembers/" + newGroupMember.GroupID + "/" + newGroupMember.UserName, newGroupMember);

                        return context.Get("GroupMembers/" + newGroupMember.GroupID + "/" + newGroupMember.UserName).ResultAs<BibleVerse.DTO.Models.GroupMembers>() != oldGroupMember && context.Get("GroupMembers/" + newGroupMember.GroupID + "/" + newGroupMember.UserName).ResultAs<BibleVerse.DTO.Models.GroupMembers>() != null ? true : false;

                    default:
                        return false;
                }
            }catch(Exception ex)
            {
                BibleVerse.DTO.ELog exception = new BibleVerse.DTO.ELog()
                {
                    Message = String.Format("{0}: {1}, {2}", ex.Source, ex.StackTrace, ex.Message),
                    Service = "BVCommon.BVContextHelper",
                    Severity = 3,
                    CreateDateTime = DateTime.Now
                };

                context.Push("Elogs", exception);
                

                return false;
            }
        }
    }
}
