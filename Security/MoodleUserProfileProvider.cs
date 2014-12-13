using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Profile;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Data;

namespace timw255.Sitefinity.Moodle.Security
{
    public class MoodleUserProfileProvider : UserProfileDataProvider
    {
        public override string RootKey
        {
            get
            {
                return "UserProfileDataProvider";
            }
        }

        protected UserProfileDataProvider()
        {
        }

        private void CollectEventsData()
        {
            object obj;
            List<ProfileEventBase> profileEventBases = new List<ProfileEventBase>();
            IList dirtyItems = this.GetDirtyItems();
            if (dirtyItems.Count == 0)
            {
                return;
            }
            foreach (object dirtyItem in dirtyItems)
            {
                UserProfile userProfile = dirtyItem as UserProfile;
                ProfileEventBase profileCreating = null;
                if (userProfile == null)
                {
                    continue;
                }
                base.TryGetExecutionStateData("EventOriginKey", out obj);
                switch (base.GetDirtyItemStatus(userProfile))
                {
                    case SecurityConstants.TransactionActionType.New:
                    {
                        profileCreating = new ProfileCreating()
                        {
                            Profile = userProfile
                        };
                        this.PopulateProfileEventBase(ref profileCreating, userProfile);
                        this.RaiseEvent(profileCreating, obj, true, true);
                        profileCreating = new ProfileCreated();
                        break;
                    }
                    case SecurityConstants.TransactionActionType.Updated:
                    {
                        profileCreating = new ProfileUpdating()
                        {
                            Profile = userProfile
                        };
                        this.PopulateProfileEventBase(ref profileCreating, userProfile);
                        this.RaiseEvent(profileCreating, obj, true, true);
                        profileCreating = new ProfileUpdated();
                        break;
                    }
                    case SecurityConstants.TransactionActionType.Deleted:
                    {
                        profileCreating = new ProfileDeleting()
                        {
                            Profile = userProfile
                        };
                        this.PopulateProfileEventBase(ref profileCreating, userProfile);
                        this.RaiseEvent(profileCreating, obj, true, true);
                        profileCreating = new ProfileDeleted();
                        break;
                    }
                }
                this.PopulateProfileEventBase(ref profileCreating, userProfile);
                profileEventBases.Add(profileCreating);
            }
            if (profileEventBases.Count > 0)
            {
                base.SetExecutionStateData("events", profileEventBases);
            }
        }

        public override void CommitTransaction()
        {
            this.CollectEventsData();
            base.CommitTransaction();
            this.RaiseEvents(false);
        }

        public override object CreateItem(Type profileType, Guid id)
        {
            if (!typeof(UserProfile).IsAssignableFrom(profileType))
            {
                throw DataProviderBase.GetInvalidItemTypeException(profileType, this.GetKnownTypes());
            }
            return this.CreateProfile(id, profileType.FullName);
        }

        public abstract UserProfile CreateProfile(User user);

        public abstract UserProfile CreateProfile(User user, Guid profileID);

        public abstract UserProfile CreateProfile(User user, Guid profileID, Type profileType);

        public abstract UserProfile CreateProfile(User user, string profileTypeName);

        public abstract UserProfile CreateProfile(User user, Guid profileId, string profileTypeName);

        public abstract UserProfile CreateProfile(Guid profileId, string profileTypeName);

        public abstract UserProfileLink CreateUserProfileLink();

        public abstract UserProfileLink CreateUserProfileLink(Guid id);

        public abstract void Delete(UserProfile item);

        public abstract void Delete(UserProfileLink item);

        public override void DeleteItem(object item)
        {
            if (item != null)
            {
                UserProfile userProfile = item as UserProfile;
                if (userProfile != null)
                {
                    this.Delete(userProfile);
                }
                this.providerDecorator.DeletePermissions(userProfile);
                throw DataProviderBase.GetInvalidItemTypeException(item.GetType(), new Type[] { typeof(UserProfile) });
            }
            throw new ArgumentNullException("item");
        }

        public virtual void DeleteProfilesForProfileType(Type profileType)
        {
            string fullName = profileType.FullName;
            IQueryable<UserProfileLink> userProfileLinks = 
                from upl in this.GetUserProfileLinks()
                where upl.UserProfileTypeName == fullName
                select upl;
            foreach (UserProfileLink userProfileLink in userProfileLinks)
            {
                this.Delete(userProfileLink);
            }
        }

        public override void FlushTransaction()
        {
            this.CollectEventsData();
            base.FlushTransaction();
        }

        public override object GetItem(Type profileType, Guid id)
        {
            if (profileType == null)
            {
                throw new ArgumentNullException("itemType");
            }
            if (!typeof(UserProfile).IsAssignableFrom(profileType))
            {
                throw DataProviderBase.GetInvalidItemTypeException(profileType, new Type[] { typeof(UserProfile) });
            }
            return this.GetProfile(profileType, id);
        }

        public override object GetItemOrDefault(Type itemType, Guid id)
        {
            if (itemType == null)
            {
                throw new ArgumentNullException("itemType");
            }
            if (itemType != typeof(UserProfile))
            {
                return base.GetItem(itemType, id);
            }
            return (
                from q in this.GetUserProfiles()
                where q.Id == id
                select q).FirstOrDefault<UserProfile>();
        }

        public override IEnumerable GetItems(Type itemType, string filterExpression, string orderExpression, int skip, int take, ref int? totalCount)
        {
            if (itemType == null)
            {
                throw new ArgumentNullException("itemType");
            }
            if (!typeof(UserProfile).IsAssignableFrom(itemType))
            {
                throw DataProviderBase.GetInvalidItemTypeException(itemType, new Type[] { typeof(UserProfile) });
            }
            return DataProviderBase.SetExpressions(SitefinityQuery.Get(itemType, this), filterExpression, orderExpression, new int?(skip), new int?(take), ref totalCount);
        }

        public override Type[] GetKnownTypes()
        {
            return new Type[] { typeof(UserProfile), typeof(UserProfileLink) };
        }

        public abstract UserProfile GetProfile(Type profileType, Guid id);

        public override Type GetUrlTypeFor(Type itemType)
        {
            if (!typeof(UserProfile).IsAssignableFrom(itemType))
            {
                throw new ArgumentException("Unknown type specified.");
            }
            return typeof(UserProfileUrlData);
        }

        public virtual TUserProfileType GetUserProfile<TUserProfileType>(User user)
        where TUserProfileType : UserProfile
        {
            return (TUserProfileType)(this.GetUserProfile(user, typeof(TUserProfileType)) as TUserProfileType);
        }

        public virtual UserProfile GetUserProfile(User user, Type profileType)
        {
            if (profileType == null)
            {
                throw new ArgumentNullException("profileType");
            }
            return this.GetUserProfile(user, profileType.FullName);
        }

        public virtual UserProfile GetUserProfile(Guid userId, string profileTypeName)
        {
            UserProfile userProfile;
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException("userId");
            }
            if (string.IsNullOrEmpty(profileTypeName))
            {
                throw new ArgumentNullException("profileTypeName", "The profile type full name should be specified.");
            }
            try
            {
                userProfile = (
                    from up in this.GetUserProfiles(userId)
                    where up.GetType().FullName == profileTypeName
                    select up).SingleOrDefault<UserProfile>();
            }
            catch (InvalidOperationException invalidOperationException1)
            {
                InvalidOperationException invalidOperationException = invalidOperationException1;
                string str = string.Format("There are more than one user profile of type: \"{0}\" for the user with Id: \"{1}\"", profileTypeName, userId);
                throw new InvalidOperationException(str, invalidOperationException);
            }
            return userProfile;
        }

        public virtual UserProfile GetUserProfile(User user, string profileTypeName)
        {
            return this.GetUserProfile(user.Id, profileTypeName);
        }

        public abstract UserProfileLink GetUserProfileLink(Guid id);

        public abstract IQueryable<UserProfileLink> GetUserProfileLinks();

        public abstract IQueryable<UserProfile> GetUserProfiles(User user);

        public abstract IQueryable<UserProfile> GetUserProfiles(Guid userId);

        public abstract IQueryable<UserProfile> GetUserProfiles();

        private void PopulateProfileEventBase(ref ProfileEventBase profileEvent, UserProfile profile)
        {
            UserProfileLink item = profile.UserLinks[0];
            profileEvent.UserId = item.UserId;
            profileEvent.MembershipProviderName = item.MembershipManagerInfo.ProviderName;
            profileEvent.ProfileId = profile.Id;
            profileEvent.ProfileType = profile.GetType();
        }

        private void RaiseEvent(IEvent eventData, object origin, bool rollbackTransaction, bool throwExceptions)
        {
            if (origin != null)
            {
                eventData.Origin = (string)origin;
            }
            try
            {
                EventHub.Raise(eventData, throwExceptions);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (rollbackTransaction)
                {
                    this.RollbackTransaction();
                }
                throw exception;
            }
        }

        private void RaiseEvents(bool throwExceptions)
        {
            object obj;
            List<ProfileEventBase> executionStateData = base.GetExecutionStateData("events") as List<ProfileEventBase>;
            if (executionStateData == null)
            {
                return;
            }
            base.TryGetExecutionStateData("EventOriginKey", out obj);
            foreach (ProfileEventBase executionStateDatum in executionStateData)
            {
                if (obj != null)
                {
                    executionStateDatum.Origin = (string)obj;
                }
                EventHub.Raise(executionStateDatum, throwExceptions);
            }
            base.SetExecutionStateData("events", null);
        }
    }
}
