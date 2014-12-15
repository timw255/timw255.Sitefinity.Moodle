using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security.Data;
using Telerik.Sitefinity.Security.Model;

namespace timw255.Sitefinity.Moodle.Security
{
    public class MoodleRoleProvider : SitefinityRoleProvider
    {
        private RoleDataProvider dataProvider;

        public override string ApplicationName
        {
            get
            {
                return this.DataProvider.ApplicationName;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public MoodleRoleProvider()
        {
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            
        }

        public override void CreateRole(string roleName)
        {
            
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            
        }

        public override string[] GetAllRoles()
        {
            
        }

        public override string[] GetRolesForUser(string username)
        {
            
        }

        public override string[] GetUsersInRole(string roleName)
        {
            
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            
        }

        public override bool RoleExists(string roleName)
        {
            
        }
    }
}
