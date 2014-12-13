using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Security;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Data;
using Telerik.Sitefinity.Security.Model;
using System.Configuration.Provider;
using Telerik.Sitefinity.Localization;
using timw255.Sitefinity.Moodle.Models;
using RestSharp;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Converters;

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
                abilities.AddAbility("CreateUser", true, true);
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
        protected override Guid GetNewGuid()
        {
            return Guid.NewGuid();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                base.Dispose(false);
            }
        }

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

            //var passwordFormat = (MembershipPasswordFormat)user.PasswordFormat;
            //return CheckValidPassword(string.Concat(password, user.Salt), user.Password, passwordFormat);
        }

        //private bool CheckValidPassword(string enteredByUser, string original, MembershipPasswordFormat passwordFormat)
        //{

        //    if (passwordFormat != MembershipPasswordFormat.Encrypted)
        //        return original == EncodePassword(enteredByUser, null, passwordFormat);

        //    var str = DecodePassword(original, passwordFormat);
        //    return str == enteredByUser;
        //}

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

        private Telerik.Sitefinity.Security.Model.User GetSitefinityUser(timw255.Sitefinity.Moodle.Models.User mUser)
        {
            var user = new Telerik.Sitefinity.Security.Model.User();

            user.ApplicationName = ApplicationName;
            user.IsBackendUser = false;

            user.Id = new Guid(xorIt(_key, BitConverter.GetBytes(mUser.Id)));

            user.Email = mUser.Email;
            user.Comment = string.Empty;
            user.LastActivityDate = DateTime.UtcNow.AddDays(-1);
            user.LastModified = DateTime.MinValue;
            user.LastLoginDate = DateTime.MinValue;
            user.FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue;
            user.FailedPasswordAttemptWindowStart = DateTime.MinValue;
            user.Password = GetNewGuid().ToString();
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

        public override IQueryable<Telerik.Sitefinity.Security.Model.User> GetUsers()
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users");

            request.AddParameter("criteria[0][key]", "");
            request.AddParameter("criteria[0][value]", "");

            IRestResponse<UsersList> response = client.Execute<UsersList>(request);

            var mUsers = response.Data;

            var users = new List<Telerik.Sitefinity.Security.Model.User>();

            foreach (var mUser in mUsers.Users)
            {
                var user = GetSitefinityUser(mUser);
                users.Add(user);
            }

            return users.AsQueryable();
        }

        public override Telerik.Sitefinity.Security.Model.User GetUser(Guid id)
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users");

            byte[] result = xorIt(_key, id.ToByteArray());
            string sId = (BitConverter.ToInt64(result, 0)).ToString();

            request.AddParameter("criteria[0][key]", "id");
            request.AddParameter("criteria[0][value]", sId);

            IRestResponse<UsersList> response = client.Execute<UsersList>(request);

            var mUser = response.Data.Users.FirstOrDefault();

            return mUser == null ? null : GetSitefinityUser(mUser);
        }

        public override Telerik.Sitefinity.Security.Model.User GetUserByEmail(string email)
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users");

            request.AddParameter("criteria[0][key]", "email");
            request.AddParameter("criteria[0][value]", email);

            IRestResponse<UsersList> response = client.Execute<UsersList>(request);

            var mUser = response.Data.Users.FirstOrDefault();

            return mUser == null ? null : GetSitefinityUser(mUser);
        }

        public override Telerik.Sitefinity.Security.Model.User GetUser(string username)
        {
            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_get_users");

            request.AddParameter("criteria[0][key]", "username");
            request.AddParameter("criteria[0][value]", username);

            IRestResponse<UsersList> response = client.Execute<UsersList>(request);

            var mUser = response.Data.Users.FirstOrDefault();

            return mUser == null ? null : GetSitefinityUser(mUser);
        }

        public override Telerik.Sitefinity.Security.Model.User CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (!ValidateParameters(ref username, ref password, ref email, ref passwordQuestion, ref passwordAnswer, ref providerUserKey, out status))
            {
                return null;
            }

            var newUser = new timw255.Sitefinity.Moodle.Models.User
            {
                Username = username,
                Password = password,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = email
            };

            var client = new RestClient(_moodleUrl);

            var request = new RestRequest("moodle/webservice/rest/server.php?wstoken={wstoken}&wsfunction={wsfunction}&moodlewsrestformat=json", Method.POST);

            request.AddUrlSegment("wstoken", _wsToken);
            request.AddUrlSegment("wsfunction", "core_user_create_users");

            request.AddParameter("users", newUser);

            var str = GenerateSalt();
            var utcNow = DateTime.UtcNow;

            var empty = CreateUser((Guid)providerUserKey, username);

            empty.Password = EncodePassword(password, str, PasswordFormat);
            empty.PasswordAnswer = EncodePassword(passwordAnswer.ToUpperInvariant(), null, PasswordFormat);
            empty.Salt = str;
            empty.Email = email;
            empty.Comment = string.Empty;
            empty.IsApproved = isApproved;
            empty.FailedPasswordAttemptCount = 0;
            empty.FailedPasswordAttemptWindowStart = utcNow;
            empty.FailedPasswordAnswerAttemptCount = 0;
            empty.FailedPasswordAnswerAttemptWindowStart = utcNow;
            empty.PasswordFormat = (int)PasswordFormat;
            empty.PasswordAnswer = passwordAnswer;
            empty.SetPasswordQuestion(passwordQuestion);
            empty.SetCreationDate(DateTime.Now);

            return empty;
        }

        public override Telerik.Sitefinity.Security.Model.User CreateUser(string userName)
        {
            return CreateUser(GetNewGuid(), userName);
        }

        public override Telerik.Sitefinity.Security.Model.User CreateUser(Guid id, string userName)
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

            var user = new Telerik.Sitefinity.Security.Model.User { ApplicationName = ApplicationName, Id = id };
            user.SetUserName(userName);
            ((IDataItem)user).Provider = this;
            user.ManagerInfo = ManagerInfo;

            return user;
        }

        public override bool ValidateUser(string userName, string password)
        {
            return ValidateUser(GetUser(userName), password);
        }

        public override bool ValidateUser(Telerik.Sitefinity.Security.Model.User user, string password)
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

        public static Guid ToGuid(string src)
        {
            byte[] stringbytes = Encoding.UTF8.GetBytes(src);
            byte[] hashedBytes = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(stringbytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }

        public byte[] xorIt(byte[] key, byte[] data)
        {
            byte[] result = new byte[16];

            key.CopyTo(result, 0);

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(key[i] ^ data[i]);
            }

            return result;
        }
        #endregion

        #region Not Supported
        public override string GetPassword(Telerik.Sitefinity.Security.Model.User user, string answer)
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

        public override void Delete(Telerik.Sitefinity.Security.Model.User item)
        {
            throw new NotSupportedException();
        }

        public override string ResetPassword(Telerik.Sitefinity.Security.Model.User user, string answer)
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

        public override bool ChangePassword(Telerik.Sitefinity.Security.Model.User user, string oldPassword, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(Telerik.Sitefinity.Security.Model.User user, string password, string newPasswordQuestion, string newPasswordAnswer)
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

        public override bool UnlockUser(Telerik.Sitefinity.Security.Model.User user)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
