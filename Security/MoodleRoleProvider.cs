using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Data;
using Telerik.Sitefinity.Security.Model;

namespace timw255.Sitefinity.Moodle.Security
{
    public class MoodleRoleProvider : RoleDataProvider
    {
        #region Privates variables
        private string _moodleUrl = "http://localhost:8080/";
        private string _wsToken = "814d0c56f0d38c822351a126fb4dd78e";
        #endregion

        public override ProviderAbilities Abilities
        {
            get
            {
                ProviderAbilities abilities = new ProviderAbilities();
                abilities.ProviderName = this.Name;
                abilities.ProviderType = this.GetType().FullName;
                abilities.AddAbility("GetRole", true, true);
                //abilities.AddAbility("AddRole", true, true);
                abilities.AddAbility("AssingUserToRole", false, false);
                abilities.AddAbility("UnAssingUserFromRole", false, false);
                //abilities.AddAbility("DeleteRole", false, false);
                return abilities;
            }
        }

        public MoodleRoleProvider()
        {
        }

        public virtual void AddUserToRole(User user, Role role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            if (SecurityManager.UnassignableRoles.Contains(role.Id))
            {
                string name = role.Name;
                Guid id = role.Id;
                throw new ArgumentException(string.Format("Users cannot be assigned to role {0} (Role ID: {1})", name, id.ToString()), "role");
            }
            Guid guid = role.Id;
            Guid id1 = user.Id;
            if (!(
                from l in this.GetUserLinks()
                where (l.Role.Id == guid) && (l.UserId == id1)
                select l).Any<UserLink>())
            {
                UserLink userLink = this.CreateUserLink();
                userLink.UserId = user.Id;
                userLink.Role = role;
                ManagerInfo managerInfo = this.GetManagerInfo(user.ManagerInfo.ManagerType, user.ManagerInfo.ProviderName);
                userLink.MembershipManagerInfo = managerInfo;
                role.Users.Add(userLink);
            }
        }

        public virtual void RemoveUserFromRole(User user, Role role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            this.RemoveUserFromRole(user.Id, role);
        }

        public virtual void RemoveUserFromRole(Guid userId, Role role)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException("userId");
            }
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            Guid id = role.Id;
            UserLink userLink = (
                from l in this.GetUserLinks()
                where (l.Role.Id == id) && (l.UserId == userId)
                select l).SingleOrDefault<UserLink>();
            if (userLink != null)
            {
                this.Delete(userLink);
            }
        }

        public override Telerik.Sitefinity.Security.Model.UserLink CreateUserLink(Guid id)
        {
            Telerik.Sitefinity.Security.Model.UserLink link = CreateUserLink();

            link.UserId = id;
            return link;
        }

        public override Telerik.Sitefinity.Security.Model.UserLink CreateUserLink()
        {
            Telerik.Sitefinity.Security.Model.UserLink link = new Telerik.Sitefinity.Security.Model.UserLink();

            link.ApplicationName = base.ApplicationName;
            return link;
        }

        public override Telerik.Sitefinity.Security.Model.Role GetRole(Guid id)
        {
            using (IDEAEntities entities = new IDEAEntities())
            {
                Telerik.Sitefinity.Security.Model.Role role = new Telerik.Sitefinity.Security.Model.Role();

                var profilo = entities.SF_PROFILI.Where(a1 => a1.SFID == id).FirstOrDefault();

                if (profilo != null)
                {
                    role.ApplicationName = base.ApplicationName;
                    role.Id = profilo.SFID;
                    role.Name = profilo.DESCR;
                    role.LastModified = DateTime.MinValue;
                }

                return role;
            }
        }

        public override IQueryable<Telerik.Sitefinity.Security.Model.Role> GetRoles()
        {
            List<Telerik.Sitefinity.Security.Model.Role> lst = new List<Telerik.Sitefinity.Security.Model.Role>();

            using (IDEAEntities entities = new IDEAEntities())
            {
                var profili = entities.SF_PROFILI;

                foreach (var p in profili)
                {
                    Telerik.Sitefinity.Security.Model.Role role = new Telerik.Sitefinity.Security.Model.Role();

                    role = GetRole(p.SFID);

                    lst.Add(role);
                }
            }

            return lst.AsQueryable();
        }

        public override Telerik.Sitefinity.Security.Model.UserLink GetUserLink(Guid id)
        {
            Telerik.Sitefinity.Security.Model.UserLink userLink = new Telerik.Sitefinity.Security.Model.UserLink();

            userLink.Id = id;
            userLink.UserId = id;
            userLink.ApplicationName = base.ApplicationName;

            // userLink.Role = GetRoleFromUserLink(id);

            var mi = new ManagerInfo();

            mi.ApplicationName = base.ApplicationName;
            mi.ManagerType = typeof(ManagerInfo).FullName;
            mi.ProviderName = "IFRoleProvider";

            userLink.MembershipManagerInfo = mi;

            return userLink;
        }

        public override IQueryable<Telerik.Sitefinity.Security.Model.UserLink> GetUserLinks()
        {
            List<Telerik.Sitefinity.Security.Model.UserLink> lst = new List<Telerik.Sitefinity.Security.Model.UserLink>();

            using (IDEAEntities entities = new IDEAEntities())
            {
                var linkProfili = entities.I_UTENTI.Where(o1 => o1.STATO == 0).Select(o1 => o1.SF_PROFILI_UTENTI);

                foreach (var i in linkProfili)
                {
                    Telerik.Sitefinity.Security.Model.UserLink userLink = GetUserLink(i.GUID);

                    lst.Add(userLink);
                }
            }

            return lst.AsQueryable();
        }

        private Telerik.Sitefinity.Security.Model.Role GetRoleFromUserLink(Guid userLinkID)
        {
            using (IDEAEntities entities = new IDEAEntities())
            {
                var userlink = entities.SF_PROFILI_UTENTI.Where(o => o.GUID == userLinkID).FirstOrDefault();

                if (userlink == null) return null;

                Telerik.Sitefinity.Security.Model.Role role = new Telerik.Sitefinity.Security.Model.Role();

                role.ApplicationName = base.ApplicationName;
                role.Id = userlink.PROFILO.Value;
                role.Name = userlink.DESCRIZIONE;
                return role;
            }
        }

        #region Not Supported
        public override Telerik.Sitefinity.Security.Model.Role CreateRole(Guid id, string roleName)
        {
            throw new NotImplementedException();
        }

        public override Telerik.Sitefinity.Security.Model.Role CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Telerik.Sitefinity.Security.Model.UserLink item)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Telerik.Sitefinity.Security.Model.Role item)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
