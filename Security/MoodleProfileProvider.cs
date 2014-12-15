using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Profile;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Data;
using Telerik.Sitefinity.Security.Model;

namespace timw255.Sitefinity.Moodle.Security
{
    public class MoodleProfileProvider : SitefinityProfileProvider
    {
        private UserProfileManager _profileManager;

        public override string ApplicationName
        {
            get
            {
                return this.profileManager.Provider.ApplicationName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        internal UserProfileManager profileManager
        {
            get
            {
                if (this._profileManager == null)
                {
                    this._profileManager = UserProfileManager.GetManager();
                }
                return this._profileManager;
            }
            set
            {
                this._profileManager = value;
            }
        }

        public MoodleProfileProvider()
        {
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            return 0;
        }

        public override int DeleteProfiles(string[] usernames)
        {
            return 0;
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            return 0;
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profileInfoCollections = new ProfileInfoCollection();
            totalRecords = 0;
            return profileInfoCollections;
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profileInfoCollections = new ProfileInfoCollection();
            totalRecords = 0;
            return profileInfoCollections;
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profileInfoCollections = new ProfileInfoCollection();
            totalRecords = 0;
            return profileInfoCollections;
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            ProfileInfoCollection profileInfoCollections = new ProfileInfoCollection();
            int num = pageIndex * pageSize;
            IQueryable<UserProfile> userProfiles = this.profileManager.GetUserProfiles().Skip<UserProfile>(num).Take<UserProfile>(pageSize);
            totalRecords = userProfiles.Count<UserProfile>();
            foreach (UserProfile userProfile in userProfiles)
            {
                profileInfoCollections.Add(new ProfileInfo(userProfile.User.UserName, false, userProfile.User.LastActivityDate, userProfile.LastModified, 0));
            }
            return profileInfoCollections;
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            IQueryable<UserProfile> userProfiles =
                from p in this.profileManager.GetUserProfiles()
                where p.LastModified < userInactiveSinceDate
                select p;
            return userProfiles.Count<UserProfile>();
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            return new SettingsPropertyValueCollection();
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
        }
    }
}