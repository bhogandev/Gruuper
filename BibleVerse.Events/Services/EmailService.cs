using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using BibleVerse.DTO;
using Microsoft.AspNetCore.WebUtilities;

namespace BibleVerse.Events.Services
{
    public class EmailService
    {
        
        public static void SendConfirmationEmail(string ToEmail, string id, string confirmationToken, string key, string appRoot)
        {
            Dictionary<string, string> confirmationData = new Dictionary<string, string>() { { "id", id }, { "token", HttpUtility.UrlEncode(confirmationToken) } };
            var confirmationLink = new Uri(QueryHelpers.AddQueryString(string.Format("{0}/{1}", appRoot, "ConfirmEmail"), confirmationData));
            Send(ToEmail, "Confirm Your Account", "Thank you for registering for Gruuper. \n Please click the confirmation link to confirm your account and get started: " + confirmationLink, key);
        }

        public static void SendGroupChatInvite(string ToEmail, string refCode, string userName, string id ,string members, string key, string appRoot)
        {
            Dictionary<string, string> confirmationData = new Dictionary<string, string>() { { "id", id }, { "token", HttpUtility.UrlEncode(refCode) } };
            var confirmationLink = new Uri(QueryHelpers.AddQueryString(string.Format("{0}/{1}", appRoot, "RefCode/Group/Join"), confirmationData));
            Send(ToEmail, "You've Been Invited To Join A Group Chat!", string.Format("Hi {0}, \n  You've been invited to join a group chat on gruup messenger by {1}. \n Click the link below to join: ", userName, members) + confirmationLink, key);
        }

        public static void SendEmailNotification(string ToEmail, BibleVerse.DTO.Notifications notification)
        {

        }

        public static void SendPasswordResetEmail(string ToEmail)
        {

        }

        private static void Send(string ToAddress, string Subject, string Body, string key)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);

            /*
            //Grab Email Pass from DB
            IQueryable<string> pass = from c in _context.SiteConfigs
                                           where (c.Service == "Email" && c.Name == "AccountPass")
                                           select c.Value;
            */   

            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("GruuperApp@gmail.com", key);
            client.EnableSsl = true
            ;
            client.Send("info@Gruuper.com", ToAddress, Subject, Body);
        }
    }
}
