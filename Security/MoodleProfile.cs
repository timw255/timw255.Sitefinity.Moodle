using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security.Model;

namespace timw255.Sitefinity.Moodle.Security
{
    [Persistent]
    public class MoodleProfile : SitefinityProfile
    {
        [DataMember]
        [FieldAlias("full_name")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string FullName { get; set; }

        [DataMember]
        [FieldAlias("address")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string Address { get; set; }

        [DataMember]
        [FieldAlias("city")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string City { get; set; }

        [DataMember]
        [FieldAlias("country")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string Country { get; set; }

        [DataMember]
        [FieldAlias("phone_1")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string Phone1 { get; set; }

        [DataMember]
        [FieldAlias("phone_2")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string Phone2 { get; set; }

        [DataMember]
        [FieldAlias("department")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string Department { get; set; }

        [DataMember]
        [FieldAlias("institution")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string Institution { get; set; }

        [DataMember]
        [FieldAlias("id_number")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string IdNumber { get; set; }

        [DataMember]
        [FieldAlias("interests")]
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string Interests { get; set; }
    }
}
