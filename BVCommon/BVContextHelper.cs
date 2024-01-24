﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BVCommon
{
    public class BVContextHelper
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

        public static bool WriteObject(BibleVerse.DALV2.BVIdentityContext context, Type entType, string entObject)
        {
            switch (entType.FullName)
            {
                case "BibleVerse.DTO.Event":
                    BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                    context.Events.Add(newEvent);
                    context.SaveChanges();
                    return context.Events.Find(newEvent.EventUID) != null ? true : false;

                case "BibleVerse.DTO.Notifications":
                    BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                    context.Notifications.Add(newNotification);
                    context.SaveChanges();
                    return context.Notifications.Find(newNotification.NotificationID) != null ? true : false;

                case "BibleVerse.DTO.ELog":
                    BibleVerse.DTO.ELog newELog = JsonConvert.DeserializeObject<BibleVerse.DTO.ELog>(entObject);
                    context.ELogs.Add(newELog);
                    context.SaveChanges();
                    return context.ELogs.Find(newELog.ElogID) != null ? true : false;
                
                case "BibleVerse.DTO.Courses":
                    BibleVerse.DTO.Courses newCourse = JsonConvert.DeserializeObject<BibleVerse.DTO.Courses>(entObject);
                    context.Courses.Add(newCourse);
                    context.SaveChanges();
                    return context.Courses.Find(newCourse.CourseId) != null ? true : false;

                case "BibleVerse.DTO.Comments":
                    BibleVerse.DTO.Comments newComment = JsonConvert.DeserializeObject<BibleVerse.DTO.Comments>(entObject);
                    context.Comments.Add(newComment);
                    context.SaveChanges();
                    return context.Comments.Find(newComment.Id) != null ? true : false;

                case "BibleVerse.DTO.Likes":
                    BibleVerse.DTO.Likes newLike = JsonConvert.DeserializeObject<BibleVerse.DTO.Likes>(entObject);
                    context.Likes.Add(newLike);
                    context.SaveChanges();
                    return context.Likes.Find(newLike.Id) != null ? true : false;

                case "BibleVerse.DTO.Photos":
                    BibleVerse.DTO.Photos newPhoto = JsonConvert.DeserializeObject<BibleVerse.DTO.Photos>(entObject);
                    context.Photos.Add(newPhoto);
                    context.SaveChanges();
                    return context.Photos.Find(newPhoto.PhotoId) != null ? true : false;

                case "BibleVerse.DTO.Videos":
                    BibleVerse.DTO.Videos newVideo = JsonConvert.DeserializeObject<BibleVerse.DTO.Videos>(entObject);
                    context.Videos.Add(newVideo);
                    context.SaveChanges();
                    return context.Videos.Find(newVideo.VideoId) != null ? true : false;

                case "BibleVerse.DTO.Posts":
                    BibleVerse.DTO.Posts newPosts = JsonConvert.DeserializeObject<BibleVerse.DTO.Posts>(entObject);
                    context.Posts.Add(newPosts);
                    context.SaveChanges();
                    return context.Posts.Find(newPosts.PostId) != null ? true : false;

                case "BibleVerse.DTO.Messages":
                    BibleVerse.DTO.Messages newMessage = JsonConvert.DeserializeObject<BibleVerse.DTO.Messages>(entObject);
                    context.Messages.Add(newMessage);
                    context.SaveChanges();
                    return context.Messages.Find(newMessage.MessageId) != null ? true : false;

                case "BibleVerse.DTO.Assignments":
                    BibleVerse.DTO.Assignments newAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.Assignments>(entObject);
                    context.Assignments.Add(newAssignment);
                    context.SaveChanges();
                    return context.Assignments.Find(newAssignment.AssignmentId) != null ? true : false;

                case "BibleVerse.DTO.Organization":
                    BibleVerse.DTO.Organization newOrganization = JsonConvert.DeserializeObject<BibleVerse.DTO.Organization>(entObject);
                    context.Organization.Add(newOrganization);
                    context.SaveChanges();
                    return context.Organization.Find(newOrganization.OrganizationId) != null ? true : false;

                case "BibleVerse.DTO.OrgSettings":
                    BibleVerse.DTO.OrgSettings newOrgSettings = JsonConvert.DeserializeObject<BibleVerse.DTO.OrgSettings>(entObject);
                    context.OrgSettings.Add(newOrgSettings);
                    context.SaveChanges();
                    return context.OrgSettings.Find(newOrgSettings.OrgSettingsId) != null ? true : false;

                case "BibleVerse.DTO.Profiles":
                    BibleVerse.DTO.Profiles newProfile = JsonConvert.DeserializeObject<BibleVerse.DTO.Profiles>(entObject);
                    context.Profiles.Add(newProfile);
                    context.SaveChanges();
                    return context.Profiles.Find(newProfile.ProfileId) != null ? true : false;

                case "BibleVerse.DTO.RefCodeLogs":
                    BibleVerse.DTO.RefCodeLogs newRefCodeLog = JsonConvert.DeserializeObject<BibleVerse.DTO.RefCodeLogs>(entObject);
                    context.RefCodeLogs.Add(newRefCodeLog);
                    context.SaveChanges();
                    return context.RefCodeLogs.Find(newRefCodeLog.Key) != null ? true : false;

                case "BibleVerse.DTO.SiteConfig":
                    BibleVerse.DTO.SiteConfigs newSiteConfig = JsonConvert.DeserializeObject<BibleVerse.DTO.SiteConfigs>(entObject);
                    context.SiteConfigs.Add(newSiteConfig);
                    context.SaveChanges();
                    return context.SiteConfigs.Find(newSiteConfig.Key) != null ? true : false;

                case "BibleVerse.DTO.Subscriptions":
                    BibleVerse.DTO.Subscriptions newSubscription = JsonConvert.DeserializeObject<BibleVerse.DTO.Subscriptions>(entObject);
                    context.Subscriptions.Add(newSubscription);
                    context.SaveChanges();
                    return context.Subscriptions.Find(newSubscription.SubscriptionID) != null ? true : false;

                case "BibleVerse.DTO.SubscriptionsHistory":
                    BibleVerse.DTO.SubscriptionsHistory newSubscriptionHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.SubscriptionsHistory>(entObject);
                    context.SubscriptionsHistory.Add(newSubscriptionHistory);
                    context.SaveChanges();
                    return context.SubscriptionsHistory.Find(newSubscriptionHistory.RecordID) != null ? true : false;

                case "BibleVerse.DTO.UserAssignments":
                    BibleVerse.DTO.UserAssignments newUserAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAssignments>(entObject);
                    context.UserAssignments.Add(newUserAssignment);
                    context.SaveChanges();
                    return context.UserAssignments.Find(newUserAssignment.AssignmentId) != null ? true : false;

                case "BibleVerse.DTO.UserAWS":
                    BibleVerse.DTO.UserAWS newUserAWS = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAWS>(entObject);
                    context.UserAWS.Add(newUserAWS);
                    context.SaveChanges();
                    return context.UserAWS.Find(newUserAWS.ID) != null ? true : false;

                case "BibleVerse.DTO.UserHistory":
                    BibleVerse.DTO.UserHistory newUserHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.UserHistory>(entObject);
                    context.UserHistory.Add(newUserHistory);
                    context.SaveChanges();
                    return context.UserHistory.Find(newUserHistory.ActionID) != null ? true : false;

                case "BibleVerse.DTO.UserRelationships":
                    BibleVerse.DTO.UserRelationships newUserRelationship = JsonConvert.DeserializeObject<BibleVerse.DTO.UserRelationships>(entObject);
                    context.UserRelationships.Add(newUserRelationship);
                    context.SaveChanges();
                    return context.UserRelationships.Find(newUserRelationship.RelationshipID) != null ? true : false;

                case "BibleVerse.DTO.Transactions":
                    BibleVerse.DTO.Transactions newTransaction = JsonConvert.DeserializeObject<BibleVerse.DTO.Transactions>(entObject);
                    context.Transactions.Add(newTransaction);
                    context.SaveChanges();
                    return context.Transactions.Find(newTransaction.TransactionUID) != null ? true : false;

                case "BibleVerse.DTO.Models.Groups":
                    BibleVerse.DTO.Models.Groups newGroup = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.Groups>(entObject);
                    context.Groups.Add(newGroup);
                    context.SaveChanges();
                    return context.Groups.Find(newGroup.GroupID) != null ? true : false;

                case "BibleVerse.DTO.Models.GroupMembers":
                    BibleVerse.DTO.Models.GroupMembers newGroupMember = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.GroupMembers>(entObject);
                    context.GroupMembers.Add(newGroupMember);
                    context.SaveChanges();
                    return context.GroupMembers.FirstOrDefaultAsync(x => (x.GroupID == newGroupMember.GroupID) && (x.UserName == newGroupMember.UserName)) != null ? true : false;

                default:
                    return false;
            }
        }

        public static bool DeleteObject(BibleVerse.DALV2.BVIdentityContext context, Type entType, string entObject)
        {
            switch (entType.FullName)
            {
                case "BibleVerse.DTO.Event":
                    BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                    context.Events.Remove(newEvent);
                    context.SaveChanges();
                    return context.Events.Find(newEvent.EventUID) != null ? false : true;

                case "BibleVerse.DTO.Notifications":
                    BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                    context.Notifications.Remove(newNotification);
                    context.SaveChanges();
                    return context.Notifications.Find(newNotification.NotificationID) != null ? false : true;

                case "BibleVerse.DTO.ELog":
                    BibleVerse.DTO.ELog newELog = JsonConvert.DeserializeObject<BibleVerse.DTO.ELog>(entObject);
                    context.ELogs.Remove(newELog);
                    context.SaveChanges();
                    return context.ELogs.Find(newELog.ElogID) != null ? false : true;

                case "BibleVerse.DTO.Courses":
                    BibleVerse.DTO.Courses newCourse = JsonConvert.DeserializeObject<BibleVerse.DTO.Courses>(entObject);
                    context.Courses.Remove(newCourse);
                    context.SaveChanges();
                    return context.Courses.Find(newCourse.CourseId) != null ? false : true;

                case "BibleVerse.DTO.Comments":
                    BibleVerse.DTO.Comments newComment = JsonConvert.DeserializeObject<BibleVerse.DTO.Comments>(entObject);
                    context.Comments.Remove(newComment);
                    context.SaveChanges();
                    return context.Comments.Find(newComment.Id) != null ? false : true;

                case "BibleVerse.DTO.Likes":
                    BibleVerse.DTO.Likes newLike = JsonConvert.DeserializeObject<BibleVerse.DTO.Likes>(entObject);
                    context.Likes.Remove(newLike);
                    context.SaveChanges();
                    return context.Likes.Find(newLike.Id) != null ? false : true;

                case "BibleVerse.DTO.Photos":
                    BibleVerse.DTO.Photos newPhoto = JsonConvert.DeserializeObject<BibleVerse.DTO.Photos>(entObject);
                    context.Photos.Remove(newPhoto);
                    context.SaveChanges();
                    return context.Photos.Find(newPhoto.PhotoId) != null ? false : true;

                case "BibleVerse.DTO.Videos":
                    BibleVerse.DTO.Videos newVideo = JsonConvert.DeserializeObject<BibleVerse.DTO.Videos>(entObject);
                    context.Videos.Remove(newVideo);
                    context.SaveChanges();
                    return context.Videos.Find(newVideo.VideoId) != null ? false : true;

                case "BibleVerse.DTO.Posts":
                    BibleVerse.DTO.Posts newPosts = JsonConvert.DeserializeObject<BibleVerse.DTO.Posts>(entObject);
                    context.Posts.Remove(newPosts);
                    context.SaveChanges();
                    return context.Posts.Find(newPosts.PostId) != null ? false : true;

                case "BibleVerse.DTO.Messages":
                    BibleVerse.DTO.Messages newMessage = JsonConvert.DeserializeObject<BibleVerse.DTO.Messages>(entObject);
                    context.Messages.Remove(newMessage);
                    context.SaveChanges();
                    return context.Messages.Find(newMessage.MessageId) != null ? false : true;

                case "BibleVerse.DTO.Assignments":
                    BibleVerse.DTO.Assignments newAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.Assignments>(entObject);
                    context.Assignments.Remove(newAssignment);
                    context.SaveChanges();
                    return context.Assignments.Find(newAssignment.AssignmentId) != null ? false : true;

                case "BibleVerse.DTO.Organization":
                    BibleVerse.DTO.Organization newOrganization = JsonConvert.DeserializeObject<BibleVerse.DTO.Organization>(entObject);
                    context.Organization.Remove(newOrganization);
                    context.SaveChanges();
                    return context.Organization.Find(newOrganization.OrganizationId) != null ? false : true;

                case "BibleVerse.DTO.OrgSettings":
                    BibleVerse.DTO.OrgSettings newOrgSettings = JsonConvert.DeserializeObject<BibleVerse.DTO.OrgSettings>(entObject);
                    context.OrgSettings.Remove(newOrgSettings);
                    context.SaveChanges();
                    return context.OrgSettings.Find(newOrgSettings.OrgSettingsId) != null ? false : true;

                case "BibleVerse.DTO.Profiles":
                    BibleVerse.DTO.Profiles newProfile = JsonConvert.DeserializeObject<BibleVerse.DTO.Profiles>(entObject);
                    context.Profiles.Remove(newProfile);
                    context.SaveChanges();
                    return context.Profiles.Find(newProfile) != null ? false : true;

                case "BibleVerse.DTO.RefCodeLogs":
                    BibleVerse.DTO.RefCodeLogs newRefCodeLog = JsonConvert.DeserializeObject<BibleVerse.DTO.RefCodeLogs>(entObject);
                    context.RefCodeLogs.Remove(newRefCodeLog);
                    context.SaveChanges();
                    return context.RefCodeLogs.Find(newRefCodeLog.Key) != null ? false : true;

                case "BibleVerse.DTO.SiteConfigs":
                    BibleVerse.DTO.SiteConfigs newSiteConfig = JsonConvert.DeserializeObject<BibleVerse.DTO.SiteConfigs>(entObject);
                    context.SiteConfigs.Remove(newSiteConfig);
                    context.SaveChanges();
                    return context.SiteConfigs.Find(newSiteConfig.Key) != null ? false : true;

                case "BibleVerse.DTO.Subscriptions":
                    BibleVerse.DTO.Subscriptions newSubscription = JsonConvert.DeserializeObject<BibleVerse.DTO.Subscriptions>(entObject);
                    context.Subscriptions.Remove(newSubscription);
                    context.SaveChanges();
                    return context.Subscriptions.Find(newSubscription.SubscriptionID) != null ? false : true;

                case "BibleVerse.DTO.SubscriptionsHistory":
                    BibleVerse.DTO.SubscriptionsHistory newSubscriptionHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.SubscriptionsHistory>(entObject);
                    context.SubscriptionsHistory.Remove(newSubscriptionHistory);
                    context.SaveChanges();
                    return context.SubscriptionsHistory.Find(newSubscriptionHistory.RecordID) != null ? false : true;

                case "BibleVerse.DTO.UserAssignments":
                    BibleVerse.DTO.UserAssignments newUserAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAssignments>(entObject);
                    context.UserAssignments.Remove(newUserAssignment);
                    context.SaveChanges();
                    return context.UserAssignments.Find(newUserAssignment.AssignmentId) != null ? false : true;

                case "BibleVerse.DTO.UserAWS":
                    BibleVerse.DTO.UserAWS newUserAWS = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAWS>(entObject);
                    context.UserAWS.Remove(newUserAWS);
                    context.SaveChanges();
                    return context.UserAWS.Find(newUserAWS.ID) != null ? false : true;

                case "BibleVerse.DTO.UserHistory":
                    BibleVerse.DTO.UserHistory newUserHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.UserHistory>(entObject);
                    context.UserHistory.Remove(newUserHistory);
                    context.SaveChanges();
                    return context.UserHistory.Find(newUserHistory.ActionID) != null ? false : true;

                case "BibleVerse.DTO.UserRelationships":
                    BibleVerse.DTO.UserRelationships newUserRelationship = JsonConvert.DeserializeObject<BibleVerse.DTO.UserRelationships>(entObject);
                    context.UserRelationships.Remove(newUserRelationship);
                    context.SaveChanges();
                    return context.UserRelationships.Find(newUserRelationship.RelationshipID) != null ? false : true;

                case "BibleVerse.DTO.Models.Groups":
                    BibleVerse.DTO.Models.Groups newGroup = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.Groups>(entObject);
                    context.Groups.Remove(newGroup);
                    context.SaveChanges();
                    return context.Groups.Find(newGroup.GroupID) != null ? false : true;

                case "BibleVerse.DTO.Models.GroupMembers":
                    BibleVerse.DTO.Models.GroupMembers newGroupMember = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.GroupMembers>(entObject);
                    context.GroupMembers.Remove(newGroupMember);
                    context.SaveChanges();
                    return context.GroupMembers.FirstOrDefaultAsync(x => (x.GroupID == newGroupMember.GroupID) && (x.UserName == newGroupMember.UserName)) != null ? false : true;

                default:
                    return false;
            }
        }

        public static bool UpdateObject(BibleVerse.DALV2.BVIdentityContext context, Type entType, string entObject)
        {
            try
            {
                switch (entType.FullName)
                {
                    case "BibleVerse.DTO.Event":
                        BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                        BibleVerse.DTO.Event oldEvent = context.Events.Find(newEvent.EventUID);
                        context.Events.Update(newEvent);
                        context.SaveChanges();
                        return context.Events.Find(newEvent.EventUID) != oldEvent && context.Events.Find(newEvent.EventUID) != null ? true : false;

                    case "BibleVerse.DTO.Notifications":
                        BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                        BibleVerse.DTO.Notifications oldNotification = context.Notifications.Find(newNotification.NotificationID);
                        context.Notifications.Update(newNotification);
                        context.SaveChanges();
                        return context.Notifications.Find(newNotification.NotificationID) != oldNotification && context.Notifications.Find(newNotification.NotificationID) != null ? true : false;

                    case "BibleVerse.DTO.ELog":
                        BibleVerse.DTO.ELog newELog = JsonConvert.DeserializeObject<BibleVerse.DTO.ELog>(entObject);
                        BibleVerse.DTO.ELog oldELog = context.ELogs.Find(newELog.ElogID);
                        context.ELogs.Update(newELog);
                        context.SaveChanges();
                        return context.ELogs.Find(newELog) != oldELog && context.ELogs.Find(newELog) != null ? true : false;

                    case "BibleVerse.DTO.Courses":
                        BibleVerse.DTO.Courses newCourse = JsonConvert.DeserializeObject<BibleVerse.DTO.Courses>(entObject);
                        BibleVerse.DTO.Courses oldCourse = context.Courses.Find(newCourse.CourseId);
                        context.Courses.Update(newCourse);
                        context.SaveChanges();
                        return context.Courses.Find(newCourse.CourseId) != oldCourse && context.Events.Find(newCourse.CourseId) != null ? true : false;

                    case "BibleVerse.DTO.Comments":
                        BibleVerse.DTO.Comments newComment = JsonConvert.DeserializeObject<BibleVerse.DTO.Comments>(entObject);
                        context.Comments.Remove(newComment);
                        context.SaveChanges();
                        return context.Comments.Find(newComment) != null ? false : true;

                    case "BibleVerse.DTO.Likes":
                        BibleVerse.DTO.Likes newLike = JsonConvert.DeserializeObject<BibleVerse.DTO.Likes>(entObject);
                        context.Likes.Remove(newLike);
                        context.SaveChanges();
                        return context.Likes.Find(newLike) != null ? false : true;

                    case "BibleVerse.DTO.Photos":
                        BibleVerse.DTO.Photos newPhoto = JsonConvert.DeserializeObject<BibleVerse.DTO.Photos>(entObject);
                        context.Photos.Remove(newPhoto);
                        context.SaveChanges();
                        return context.Photos.Find(newPhoto) != null ? false : true;

                    case "BibleVerse.DTO.Videos":
                        BibleVerse.DTO.Videos newVideo = JsonConvert.DeserializeObject<BibleVerse.DTO.Videos>(entObject);
                        context.Videos.Remove(newVideo);
                        context.SaveChanges();
                        return context.Videos.Find(newVideo) != null ? false : true;

                    case "BibleVerse.DTO.Posts":
                        BibleVerse.DTO.Posts newPosts = JsonConvert.DeserializeObject<BibleVerse.DTO.Posts>(entObject);
                        BibleVerse.DTO.Posts oldPosts = context.Posts.Find(newPosts.PostId);
                        context.Entry(oldPosts).State = EntityState.Detached;
                        context.Posts.Update(newPosts);
                        context.SaveChanges();
                        return context.Posts.Find(newPosts.PostId) != oldPosts && context.Posts.Find(newPosts.PostId) != null ? true : false;

                    case "BibleVerse.DTO.Messages":
                        BibleVerse.DTO.Messages newMessage = JsonConvert.DeserializeObject<BibleVerse.DTO.Messages>(entObject);
                        context.Messages.Remove(newMessage);
                        context.SaveChanges();
                        return context.Messages.Find(newMessage) != null ? false : true;

                    case "BibleVerse.DTO.Assignments":
                        BibleVerse.DTO.Assignments newAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.Assignments>(entObject);
                        context.Assignments.Remove(newAssignment);
                        context.SaveChanges();
                        return context.Assignments.Find(newAssignment) != null ? false : true;

                    case "BibleVerse.DTO.Organization":
                        BibleVerse.DTO.Organization newOrganization = JsonConvert.DeserializeObject<BibleVerse.DTO.Organization>(entObject);
                        context.Organization.Remove(newOrganization);
                        context.SaveChanges();
                        return context.Organization.Find(newOrganization) != null ? false : true;

                    case "BibleVerse.DTO.OrgSettings":
                        BibleVerse.DTO.OrgSettings newOrgSettings = JsonConvert.DeserializeObject<BibleVerse.DTO.OrgSettings>(entObject);
                        context.OrgSettings.Remove(newOrgSettings);
                        context.SaveChanges();
                        return context.OrgSettings.Find(newOrgSettings) != null ? false : true;

                    case "BibleVerse.DTO.Profiles":
                        BibleVerse.DTO.Profiles newProfile = JsonConvert.DeserializeObject<BibleVerse.DTO.Profiles>(entObject);
                        context.Profiles.Remove(newProfile);
                        context.SaveChanges();
                        return context.Profiles.Find(newProfile) != null ? false : true;

                    case "BibleVerse.DTO.RefCodeLogs":
                        BibleVerse.DTO.RefCodeLogs newRefCodeLog = JsonConvert.DeserializeObject<BibleVerse.DTO.RefCodeLogs>(entObject);
                        context.RefCodeLogs.Remove(newRefCodeLog);
                        context.SaveChanges();
                        return context.RefCodeLogs.Find(newRefCodeLog) != null ? false : true;

                    case "BibleVerse.DTO.SiteConfigs":
                        BibleVerse.DTO.SiteConfigs newSiteConfig = JsonConvert.DeserializeObject<BibleVerse.DTO.SiteConfigs>(entObject);
                        context.SiteConfigs.Remove(newSiteConfig);
                        context.SaveChanges();
                        return context.SiteConfigs.Find(newSiteConfig) != null ? false : true;

                    case "BibleVerse.DTO.Subscriptions":
                        BibleVerse.DTO.Subscriptions newSubscription = JsonConvert.DeserializeObject<BibleVerse.DTO.Subscriptions>(entObject);
                        context.Subscriptions.Remove(newSubscription);
                        context.SaveChanges();
                        return context.Subscriptions.Find(newSubscription) != null ? false : true;

                    case "BibleVerse.DTO.SubscriptionsHistory":
                        BibleVerse.DTO.SubscriptionsHistory newSubscriptionHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.SubscriptionsHistory>(entObject);
                        context.SubscriptionsHistory.Remove(newSubscriptionHistory);
                        context.SaveChanges();
                        return context.SubscriptionsHistory.Find(newSubscriptionHistory) != null ? false : true;

                    case "BibleVerse.DTO.UserAssignments":
                        BibleVerse.DTO.UserAssignments newUserAssignment = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAssignments>(entObject);
                        context.UserAssignments.Remove(newUserAssignment);
                        context.SaveChanges();
                        return context.UserAssignments.Find(newUserAssignment) != null ? false : true;

                    case "BibleVerse.DTO.UserAWS":
                        BibleVerse.DTO.UserAWS newUserAWS = JsonConvert.DeserializeObject<BibleVerse.DTO.UserAWS>(entObject);
                        context.UserAWS.Remove(newUserAWS);
                        context.SaveChanges();
                        return context.UserAWS.Find(newUserAWS) != null ? false : true;

                    case "BibleVerse.DTO.UserHistory":
                        BibleVerse.DTO.UserHistory newUserHistory = JsonConvert.DeserializeObject<BibleVerse.DTO.UserHistory>(entObject);
                        context.UserHistory.Remove(newUserHistory);
                        context.SaveChanges();
                        return context.UserHistory.Find(newUserHistory) != null ? false : true;

                    case "BibleVerse.DTO.UserRelationships":
                        BibleVerse.DTO.UserRelationships newUserRelationship = JsonConvert.DeserializeObject<BibleVerse.DTO.UserRelationships>(entObject);
                        context.UserRelationships.Remove(newUserRelationship);
                        context.SaveChanges();
                        return context.UserRelationships.Find(newUserRelationship) != null ? false : true;

                    case "BibleVerse.DTO.Models.Groups":
                        BibleVerse.DTO.Models.Groups newGroup = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.Groups>(entObject);
                        BibleVerse.DTO.Models.Groups oldGroup = context.Groups.Find(newGroup.GroupID);
                        context.Groups.Update(newGroup);
                        context.SaveChanges();
                        return context.Groups.Find(newGroup.GroupID) != oldGroup && context.Groups.Find(newGroup.GroupID) != null ? true : false;

                    case "BibleVerse.DTO.Models.GroupMembers":
                        BibleVerse.DTO.Models.GroupMembers newGroupMember = JsonConvert.DeserializeObject<BibleVerse.DTO.Models.GroupMembers>(entObject);
                        BibleVerse.DTO.Models.GroupMembers oldGroupMember = context.GroupMembers.Find(newGroupMember.GroupID);
                        context.GroupMembers.Update(newGroupMember);
                        context.SaveChanges();
                        return context.GroupMembers.FirstOrDefaultAsync(x => (x.GroupID == newGroupMember.GroupID) && (x.UserName == newGroupMember.UserName)) != null ? true : false;

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

                context.ELogs.Add(exception);
                context.SaveChanges();

                return false;
            }
        }
    }
}
