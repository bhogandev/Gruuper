using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories.AWSRepositories
{
    public class FirebaseHelper
    {
        private static string StackTrace = "BibleVerse.Repositories -> FirebaseRepositories -> FirebaseHelper";


        public static bool UploadPhotoToDb(BibleVerse.DTO.Photos _photo, FireSharp.FirebaseClient _context)
        {
            try
            {
                if (_photo != null)
                {
                    
                    string entType = _photo.GetType().Name;

                    string entObj = JsonConvert.SerializeObject(_photo);

                    bool result = BVCommon.FirebaseContextFunctions.WriteToDb(entType, entObj, _context);

                    return result;
                }
                else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().Name;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.FirebaseContextFunctions.WriteToDb(entType, entObj, _context);

                return false;
            }
        }

        public static bool UploadVideoToDb(BibleVerse.DTO.Videos _video, FireSharp.FirebaseClient _context)
        {
            try
            {
                if (_video != null)
                {

                    string entType = _video.GetType().Name;

                    string entObj = JsonConvert.SerializeObject(_video);

                    bool result = BVCommon.FirebaseContextFunctions.WriteToDb(entType, entObj, _context);

                    return result;
                }
                else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().Name;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.FirebaseContextFunctions.WriteToDb(entType, entObj, _context);

                return false;
            }
        }

        public static bool CreateNewPostRelationship(BibleVerse.DTO.PostsRelations _newPostRelationship, FireSharp.FirebaseClient _context)
        {
            try
            {
                if (_newPostRelationship != null)
                {

                    string entType = _newPostRelationship.GetType().Name;

                    string entObj = JsonConvert.SerializeObject(_newPostRelationship);

                    bool result = BVCommon.FirebaseContextFunctions.WriteToDb(entType, entObj, _context);

                    return result;
                }
                else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().Name;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.FirebaseContextFunctions.WriteToDb(entType, entObj, _context);

                return false;
            }
        }
    }
}
