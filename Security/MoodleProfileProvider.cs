using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Profile;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Data;
using Telerik.Sitefinity.Security.Model;
using timw255.Sitefinity.Moodle.Models;

namespace timw255.Sitefinity.Moodle.Security
{
    public class MoodleProfileProvider : UserProfileDataProvider
    {
        #region Privates variables
        private string _moodleUrl = "http://localhost:8080/";
        private string _wsToken = "814d0c56f0d38c822351a126fb4dd78e";
        private byte[] _key = (Guid.Parse("30708451-06a4-4352-89ae-a3541246bc1f")).ToByteArray();
        #endregion

        private string _providerName = "MoodleMembershipProvider";

        private ManagerInfo _managerInfo;
        private ManagerInfo ManagerInfo
        {
            get
            {
                return _managerInfo ?? (_managerInfo = new ManagerInfo()
                {
                    ProviderName = ProviderName,
                    ManagerType = "Telerik.Sitefinity.Security.UserManager",
                    ApplicationName = ApplicationName
                });
            }
        }

        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        public override string RootKey
        {
            get
            {
                return "MoodleProfileProvider";
            }
        }

        public override string ApplicationName
        {
            get
            {
                return "/MoodleUserProfiles";
            }
        }

        public MoodleProfileProvider()
        {
        }

        public override UserProfile CreateProfile(Guid profileId, string profileTypeName)
        {
            throw new NotImplementedException();
        }

        public override UserProfile CreateProfile(User user, Guid profileId, string profileTypeName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "Can't create profile for a user that is not set to an instance of an object.");
            }
            if (profileId == Guid.Empty)
            {
                throw new ArgumentNullException("profileId", "Can't create a profile instance with an empty guid as identity.");
            }
            
            MoodleProfile profile = new MoodleProfile();
            profile.Id = profileId;
            profile.ApplicationName = this.ApplicationName;
            profile.Owner = user.Id;
            profile.DateCreated = DateTime.Now;
            profile.LastModified = profile.DateCreated;
            ((IDataItem)profile).Provider = this;

            return profile;
        }

        public override UserProfile CreateProfile(User user, string profileTypeName)
        {
            return this.CreateProfile(user, this.GetNewGuid(), profileTypeName);
        }

        public override UserProfile CreateProfile(User user, Guid profileId, Type profileType)
        {
            return this.CreateProfile(user, profileId, profileType.FullName);
        }

        public override UserProfile CreateProfile(User user, Guid profileId)
        {
            return this.CreateProfile(user, profileId, typeof(UserProfile));
        }

        public override UserProfile CreateProfile(User user)
        {
            return this.CreateProfile(user, this.GetNewGuid(), typeof(UserProfile));
        }

        public override UserProfileLink CreateUserProfileLink(Guid id)
        {
            throw new NotImplementedException();
        }

        public override UserProfileLink CreateUserProfileLink()
        {
            throw new NotImplementedException();
        }

        public override void Delete(UserProfileLink item)
        {
            return;
        }

        public override void Delete(UserProfile item)
        {
            return;
        }

        public override UserProfile GetProfile(Type profileType, Guid id)
        {
            throw new NotImplementedException();
        }

        public override UserProfileLink GetUserProfileLink(Guid id)
        {
            return new UserProfileLink();
        }

        public override IQueryable<UserProfileLink> GetUserProfileLinks()
        {
            List<UserProfileLink> userProfileLinks = new List<UserProfileLink>();

            return userProfileLinks.AsQueryable();
        }

        public override IQueryable<UserProfile> GetUserProfiles()
        {
            throw new NotImplementedException();
        }

        public override IQueryable<UserProfile> GetUserProfiles(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException("userId", "id cannot be empty GUID.");
            }

            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users_by_field");

            byte[] result = Helpers.xorIt(_key, userId.ToByteArray());
            string sId = (BitConverter.ToInt64(result, 0)).ToString();

            request.AddParameter("field", "id");
            request.AddParameter("values[0]", sId);

            IRestResponse<List<MoodleUser>> response = client.Execute<List<MoodleUser>>(request);

            var mUser = response.Data.FirstOrDefault();

            List<MoodleProfile> profiles = new List<MoodleProfile>();

            MoodleProfile profile = new MoodleProfile();
            profile.Id = Guid.NewGuid();
            profile.ApplicationName = this.ApplicationName;
            profile.Owner = userId;
            profile.DateCreated = DateTime.Now;
            profile.LastModified = profile.DateCreated;
            profile.FirstName = mUser.FirstName;
            profile.LastName = mUser.LastName;
            ((IDataItem)profile).Provider = this;

            profiles.Add(profile);

            return profiles.AsQueryable();
        }

        public override IQueryable<UserProfile> GetUserProfiles(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "Can't get profiles for a user that is not set to an instance of an object.");
            }
            return this.GetUserProfiles(user.Id);
        }

        public override System.Collections.IList GetDirtyItems()
        {
            return base.GetDirtyItems();
        }
    }
}