using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Security;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Data;
using System.Configuration.Provider;
using Telerik.Sitefinity.Localization;
using timw255.Sitefinity.Moodle.Models;
using RestSharp;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Converters;
using Telerik.Sitefinity.Security.Model;

namespace timw255.Sitefinity.Moodle.Security
{
    public class MoodleMembershipProvider : MembershipDataProvider
    {
        #region Privates variables
        private string _moodleUrl = "http://localhost:8080/";
        private string _wsToken = "814d0c56f0d38c822351a126fb4dd78e";
        private byte[] _key = (Guid.Parse("30708451-06a4-4352-89ae-a3541246bc1f")).ToByteArray();

        private ManagerInfo _managerInfo;
        private string _providerName = "MoodleMembershipProvider";
        #endregion

        #region Public variables
        public const string ServiceEndpointKey = "serviceEndpoint";
        #endregion

        #region Properties
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        public override string ApplicationName
        {
            get
            {
                return "/MoodleMembershipProvider";
            }
        }

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

        public override ProviderAbilities Abilities
        {
            get
            {
                var abilities = new ProviderAbilities { ProviderName = Name, ProviderType = GetType().FullName };
                abilities.AddAbility("GetUser", true, true);
                //abilities.AddAbility("CreateUser", true, true);
                //abilities.AddAbility("DeleteUser", true, true);
                //abilities.AddAbility("UpdateUser", true, true);
                abilities.AddAbility("ValidateUser", true, true);
                //abilities.AddAbility("ResetPassword", true, true);
                //abilities.AddAbility("GetPassword", true, true);
                return abilities;
            }
        }

        #endregion

        #region Methods
        private bool CheckValidPassword(Telerik.Sitefinity.Security.Model.User user, string password)
        {
            if (user == null || !user.IsApproved || string.IsNullOrWhiteSpace(user.Password))
                return false;

            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/login/token.php?username={username}&password={password}&service=", Method.POST);

            request.AddUrlSegment("username", user.UserName);
            request.AddUrlSegment("password", password);

            IRestResponse response = client.Execute(request);
            dynamic deserialized = JsonConvert.DeserializeObject<ExpandoObject>(response.Content, new ExpandoObjectConverter());

            try
            {
                var token = deserialized.token;
                return true;
                
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
        }

        public override bool EmailExists(string email)
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users");

            request.AddParameter("criteria[0][key]", "email");
            request.AddParameter("criteria[0][value]", email);

            IRestResponse<UsersList> response = client.Execute<UsersList>(request);

            return response.Data.Users.Count > 0;
        }

        public override bool UserExists(string username)
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users");

            request.AddParameter("criteria[0][key]", "username");
            request.AddParameter("criteria[0][value]", username);

            IRestResponse<UsersList> response = client.Execute<UsersList>(request);

            return response.Data.Users.Count > 0;
        }

        private User GetSitefinityUser(MoodleUser mUser)
        {
            var user = new User();

            user.ApplicationName = ApplicationName;
            user.IsBackendUser = false;

            user.Id = new Guid(Helpers.xorIt(_key, BitConverter.GetBytes(mUser.Id)));

            user.Email = mUser.Email;
            user.Comment = mUser.Description;
            user.LastActivityDate = DateTime.UtcNow.AddDays(-1);
            user.LastModified = DateTime.MinValue;
            user.LastLoginDate = DateTime.MinValue;
            user.FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue;
            user.FailedPasswordAttemptWindowStart = DateTime.MinValue;
            user.Password = Guid.NewGuid().ToString();
            user.ManagerInfo = ManagerInfo;
            user.IsApproved = true; // Convert.ToBoolean(mUser.Confirmed);
            user.PasswordFormat = 1;

            user.SetUserName(mUser.Username);
            user.SetCreationDate(DateTime.Now);
            user.SetIsLockedOut(false);
            user.SetLastLockoutDate(DateTime.Now);
            user.SetLastPasswordChangedDate(DateTime.Now);
            user.SetPasswordQuestion("question");

            return user;
        }

        protected override void Initialize(string providerName, NameValueCollection config, Type managerType)
        {
            base.Initialize(providerName, config, managerType);
        }

        public override IQueryable<User> GetUsers()
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users");

            request.AddParameter("criteria[0][key]", "");
            request.AddParameter("criteria[0][value]", "");

            IRestResponse<UsersList> response = client.Execute<UsersList>(request);

            var users = new List<User>();

            if (response.Data != null)
            {
                var mUsers = response.Data;

                foreach (var mUser in mUsers.Users)
                {
                    var user = GetSitefinityUser(mUser);
                    users.Add(user);
                }
            }
            
            return users.AsQueryable();
        }

        public override User GetUser(Guid id)
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users_by_field");

            byte[] result = Helpers.xorIt(_key, id.ToByteArray());
            string sId = (BitConverter.ToInt64(result, 0)).ToString();

            request.AddParameter("field", "id");
            request.AddParameter("values[0]", sId);

            IRestResponse<List<MoodleUser>> response = client.Execute<List<MoodleUser>>(request);

            var mUser = response.Data.FirstOrDefault();

            return mUser == null ? null : GetSitefinityUser(mUser);
        }

        public override User GetUserByEmail(string email)
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users_by_field");

            request.AddParameter("field", "email");
            request.AddParameter("values[0]", email);

            IRestResponse<List<MoodleUser>> response = client.Execute<List<MoodleUser>>(request);

            var mUser = response.Data.FirstOrDefault();

            return mUser == null ? null : GetSitefinityUser(mUser);
        }

        public override User GetUser(string username)
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users_by_field");

            request.AddParameter("field", "username");
            request.AddParameter("values[0]", username);

            IRestResponse<List<MoodleUser>> response = client.Execute<List<MoodleUser>>(request);

            var mUser = response.Data.FirstOrDefault();

            return mUser == null ? null : GetSitefinityUser(mUser);
        }

        public override User CreateUser(string userName)
        {
            return CreateUser(Guid.NewGuid(), userName);
        }

        public override User CreateUser(Guid id, string userName)
        {
            if (id == Guid.Empty) throw new ArgumentNullException("id");

            if (!string.IsNullOrEmpty(userName))
            {
                LoginUtils.CheckParameter(userName, true, true, true, 256, "userName");
                if (this.UserExists(userName))
                {
                    throw new ProviderException("Username already exists.");
                }
            }

            var user = new User { ApplicationName = ApplicationName, Id = id };
            user.SetUserName(userName);
            ((IDataItem)user).Provider = this;
            user.ManagerInfo = ManagerInfo;

            return user;
        }

        public override bool ValidateUser(string userName, string password)
        {
            return ValidateUser(GetUser(userName), password);
        }

        public override bool ValidateUser(User user, string password)
        {
            if (user == null) return false;

            var flag = CheckValidPassword(user, password);
            if (flag)
            {
                user.LastLoginDate = DateTime.UtcNow;
                user.FailedPasswordAttemptWindowStart = DateTime.UtcNow;
                user.FailedPasswordAttemptCount = 0;
            }
            else
            {
                UpdateFailureCount(user, "password");
            }
            return flag;
        }
        #endregion

        #region Not Supported
        public override User CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(User user, string answer)
        {
            throw new NotSupportedException();
        }

        public override string GetPassword(string userName, string answer)
        {
            throw new NotSupportedException();
        }

        public override string GetPassword(Guid userId, string answer)
        {
            throw new NotSupportedException();
        }

        public override void Delete(User item)
        {
            throw new NotSupportedException();
        }

        public override string ResetPassword(User user, string answer)
        {
            throw new NotSupportedException();
        }

        public override string ResetPassword(string userName, string answer)
        {
            throw new NotSupportedException();
        }

        public override string ResetPassword(Guid userId, string answer)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePassword(User user, string oldPassword, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(User user, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(Guid id, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string userName, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override System.Collections.IEnumerable GetItems(Type itemType, string filterExpression, string orderExpression, int skip, int take, ref int? totalCount)
        {
            throw new NotSupportedException();
        }

        public override bool UnlockUser(Guid userId)
        {
            throw new NotSupportedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotSupportedException();
        }

        public override bool UnlockUser(User user)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
