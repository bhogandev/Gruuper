using System;
using Microsoft.AspNetCore.Builder;
using BibleVerse.DTO;
using Amazon.S3;
using Amazon.Runtime;
using Amazon.S3.Util;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.Net;
using System.IO;
using BibleVerse.Repositories.AWSRepositories;
using Amazon;
using FireSharp.Interfaces;
using FireSharp.Config;
using Microsoft.Extensions.Configuration;
using Firebase.Storage;
using FireSharp;

namespace BibleVerse.Repositories
{
    public class FirebaseRepository
    {
        private readonly BibleVerse.DALV2.BVIdentityContext _context;
        protected string StackTraceRoot = "BibleVerse.DTO -> Repository -> FirebaseRepository: ";
        private readonly IConfiguration _configuration;
        private string accessKey = string.Empty;
        private string secretKey = string.Empty;
        private string fireStorageURL = string.Empty;
        private FirebaseClient client;
        private FirebaseConfig config = new FirebaseConfig{ };
        

        public FirebaseRepository(FirebaseClient client, IConfiguration configuration, BibleVerse.DALV2.BVIdentityContext context)
        {
            this._context = context;
            this._configuration = configuration;
            Initialize();
            RegionEndpoint region = RegionEndpoint.USEast1;

        }

        private void Initialize()
        {

            var authSecret = _configuration.GetValue<string>("Keys:RealtimeDatabaseKey");

            var basePath = _configuration.GetValue<string>("ConnectionStrings:RealtimeDatabaseConnection");

            var firebasestorage = _configuration.GetValue<string>("ConnectionStrings:FireStorage");

            config.AuthSecret = authSecret;
            config.BasePath = basePath;
            fireStorageURL = firebasestorage;
            client = new FirebaseClient(config);
        }

        public async Task<BibleVerse.DTO.PostsRelations> uploadObject(string uploadType, byte[] base64Object, string objectUrl, string genObjectID ,BibleVerse.DTO.UserUpload uUp, BibleVerse.DTO.PostModel _newPost)
        {
            FirebaseStorage fbStorage = new FirebaseStorage(fireStorageURL, new FirebaseStorageOptions
            {
                ThrowOnCancel = true
            });
            FirebaseStorageTask userUploadResponse;
            BibleVerse.DTO.PostsRelations uploadsSuccessful = new PostsRelations();
           
            

            if (!String.IsNullOrEmpty(uploadType))
            {
                if (uploadType.ToUpper() == "PHOTO")
                {
                    string uploadStatus = string.Empty;

                    using (var stream = new MemoryStream(base64Object))
                    {
                        var fileUploadRequest = new PutObjectRequest
                        {
                            BucketName = _newPost.OrganizationId.ToLower(),
                            Key = _newPost.UserId.ToLower() + "/" + _newPost.UserId.ToLower() + "_pub" + "/" + "Images/Photos/" + uUp.FileNames[0],
                            InputStream = stream,
                            ContentType = uUp.FileTypes[0],
                            CannedACL = S3CannedACL.PublicRead
                        };

                         userUploadResponse = new FirebaseStorage(fireStorageURL).Child(fileUploadRequest.Key).PutAsync(stream);

                        userUploadResponse.Progress.ProgressChanged += (s, args) =>
                        {
                            while(args.Percentage != 100)
                            {
                                //Upload file
                            }

                            uploadStatus = HttpStatusCode.OK.ToString();
                        };
                    };
                   
                    if (uploadStatus == HttpStatusCode.OK.ToString())
                    {
                        //Update Tables in DB
                        Photos newPhoto = new Photos()
                        {
                            PhotoId = genObjectID,
                            URL = objectUrl,
                            Caption = "",//Add functionality in the future to allow user to pass caption
                            IsDeleted = false,
                            Title = "", //Add functionality in the future to allow user to pass title
                            ChangeDateTime = DateTime.Now,
                            CreateDateTime = DateTime.Now
                        };

                        

                        // Invoke FirebaseHelper Here
                        bool photoUploaded = FirebaseHelper.UploadPhotoToDb(newPhoto, client);

                        PostsRelations newRelation = new PostsRelations()
                        {
                            AttachmentID = genObjectID,
                            ContentType = "Photo",
                            FileName = uUp.FileNames[0],
                            Link = objectUrl
                        };

                        //AWSHelper here
                        bool relationUploaded = FirebaseHelper.CreateNewPostRelationship(newRelation, client);

                        uploadsSuccessful = photoUploaded && relationUploaded ? newRelation : null;

                    }
                }
                else if (uploadType.ToUpper() == "VIDEO")
                {
                    using (var stream = new MemoryStream(base64Object))
                    {
                        var fileUploadRequest = new PutObjectRequest
                        {
                            BucketName = _newPost.OrganizationId.ToLower(),
                            Key = _newPost.UserId.ToLower() + "/" + _newPost.UserId.ToLower() + "_pub" + "/" + "Videos/PostVideos/" + uUp.FileNames[0],
                            InputStream = stream,
                            ContentType = uUp.FileTypes[0],
                            CannedACL = S3CannedACL.PublicRead
                        };

                        userUploadResponse = new FirebaseStorage(fireStorageURL).Child(fileUploadRequest.Key).PutAsync(stream);

                        userUploadResponse.Progress.ProgressChanged += (s, args) =>
                        {
                            while (args.Percentage != 100)
                            {
                                //Upload file
                            }

                            var uploadStatus = HttpStatusCode.OK.ToString();
                        };
                    };

                    if (userUploadResponse != null)
                    {

                        //Update Tables in DB
                        Videos newVideo = new Videos()
                        {
                            VideoId = genObjectID,
                            URL = objectUrl,
                            Caption = "",
                            IsDeleted = false,
                            Title = "",
                            ChangeDateTime = DateTime.Now,
                            CreateDateTime = DateTime.Now
                        };

                        // Invoke AWSHelper Here
                        bool videoUploaded = FirebaseHelper.UploadVideoToDb(newVideo, client);

                        PostsRelations newRelation = new PostsRelations()
                        {
                            AttachmentID = genObjectID,
                            ContentType = "Video",
                            FileName = uUp.FileNames[0],
                            Link = objectUrl
                        };

                        //AWSHelper here
                        bool relationUploaded = FirebaseHelper.CreateNewPostRelationship(newRelation, client);

                        uploadsSuccessful = videoUploaded && relationUploaded ? newRelation : null;
                    }
                }
                }
            return uploadsSuccessful;
            }

        public async Task<ApiResponseModel> CreateUserDir(Users user)
        {
            string userDir = user.UserId.ToLower();
            ApiResponseModel apiResponse = APIHelperV1.InitializeAPIResponse();

            try
            {
                // Check if user folder already exists
                var listbucketRequst = new ListObjectsV2Request()
                {
                    BucketName = user.OrganizationId.ToLower()
                    , Prefix = user.UserId.ToLower()
                };




                var bucketList = new FirebaseStorage(fireStorageURL).Child(listbucketRequst.BucketName);


                if(bucketList == null)
                {
                    //create user folder
                    var userBucketRequest = new PutObjectRequest()
                    {
                        BucketName = user.OrganizationId.ToLower(),
                        Key = user.UserId.ToLower() + "/"
                    };

                    var userBucketResponse = new FirebaseStorage(fireStorageURL).Child(userBucketRequest.Key);

                    if(userBucketResponse != null)
                    {
                        //create sub folders
                        var userPubBucketRequest = new PutObjectRequest()
                        {
                            BucketName = user.OrganizationId.ToLower(),
                            Key = user.UserId.ToLower() + "/" + user.UserId.ToLower() + "_pub/"
                        };

                        var userPrivBucketRequest = new PutObjectRequest()
                        {
                            BucketName = user.OrganizationId.ToLower(),
                            Key = user.UserId.ToLower() + "/" + user.UserId.ToLower() + "_priv/"
                        };

                        var pubResponse = new FirebaseStorage(fireStorageURL).Child(userPubBucketRequest.Key); 
                        var privResponse = new FirebaseStorage(fireStorageURL).Child(userPrivBucketRequest.Key);

                        if((pubResponse != null) && (privResponse != null))
                        {
                            apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                            UserAWS useraws = new UserAWS()
                            {
                                ID = user.UserId,
                                Bucket = user.OrganizationId.ToLower(),
                                PublicDir = user.UserId.ToLower() + "/" + user.Id.ToLower() + "_pub/",
                                PrivateDir = user.UserId.ToLower() + "/" + user.Id.ToLower() + "_priv/",
                                ChangeDateTime = DateTime.Now,
                                CreateDateTime = DateTime.Now
                            };

                            // Store information in UserAWS
                            _context.UserAWS.Add(useraws);
                            _context.SaveChanges();
                        }
                    }

                    return apiResponse;
                }

            }catch(Exception ex)
            {
                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseErrors.Add(ex.InnerException.ToString());
            }

            return apiResponse;
        }

        public async Task<ApiResponseModel> CreateOrgBucket(Organization org)
        {
            string orgBucket = org.OrganizationId.ToLower();
            ApiResponseModel apiResponse = APIHelperV1.InitializeAPIResponse();

            try
            {
                if (new FirebaseStorage(fireStorageURL).Child(org.OrganizationId) == null)
                {
                    var putBucketRequest = new PutBucketRequest()
                    {
                        BucketName = orgBucket,
                        //BucketRegion = "us-east-1" // Eventually take out this hardcoded region and base region on user location
                    };

                    var response = new FirebaseStorage(fireStorageURL).Child(putBucketRequest.BucketName);

                    if (response != null)
                    {
                        string orgInit = BVCommon.BVFunctions.CreateInit(org.Name);

                        //Create org Dir
                        var putdirRequest = new PutObjectRequest()
                        {
                            BucketName = orgBucket,
                            Key = org.OrganizationId + "_init/",
                            ContentBody = orgInit
                        };

                        var initResponse = new FirebaseStorage(fireStorageURL).Child(putdirRequest.Key).PutAsync(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(putdirRequest.ContentBody)));

                        apiResponse.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                        //Add bucket information to tables

                        var dbOrg = from c in _context.Organization
                                    where c.OrganizationId == org.OrganizationId
                                    select c;

                        Organization realOrg = dbOrg.First();

                        realOrg.Bucket = orgBucket;
                        realOrg.ChangeDateTime = DateTime.Now;
                        _context.Organization.Update(realOrg);
                        _context.SaveChanges();
                    }
                    else
                    {
                        apiResponse.ResponseMessage = "Failure";
                        apiResponse.ResponseErrors.Add("Error on storage creation");
                        apiResponse.ResponseErrors.Add(response.ToString());
                    }


                }
                else
                {
                    apiResponse.ResponseMessage = "Failure";
                    apiResponse.ResponseErrors.Add("Org Bucket Already Exists With These Credentials");
                }
            }
            catch (Exception ex)
            {
                //Create ELog Error
                ELog e = new ELog()
                {
                    Message = ex.Message.ToString(),
                    Service = "FireBase",
                    Severity = 3,
                    CreateDateTime = DateTime.Now
                };

                _context.ELogs.Add(e);
                _context.SaveChanges();

                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseBody.Add(ex.ToString());
            }
                return apiResponse;
           
        }
        
    }
}
