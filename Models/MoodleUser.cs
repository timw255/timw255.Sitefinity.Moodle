using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Security.Model;

namespace timw255.Sitefinity.Moodle.Models
{
    public class MoodleUser
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; } //ID of the user

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; } //Username policy is defined in Moodle security config.

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; } //Plain text password consisting of any characters

        [JsonProperty(PropertyName = "firstname")]
        public string FirstName { get; set; } //The first name(s) of the user

        [JsonProperty(PropertyName = "lastname")]
        public string LastName { get; set; } //The family name of the user

        [JsonProperty(PropertyName = "fullname")]
        public string FullName { get; set; } //The fullname of the user

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; } //A valid and unique email address

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; } //Optional //Postal address

        [JsonProperty(PropertyName = "phone1")]
        public string Phone1 { get; set; } //Optional //Phone 1

        [JsonProperty(PropertyName = "phone2")]
        public string Phone2 { get; set; } //Optional //Phone 2

        [JsonProperty(PropertyName = "auth")]
        public string Auth { get; set; } //Default to "manual" //Auth plugins include manual, ldap, imap, etc

        [JsonProperty(PropertyName = "idnumber")]
        public string IdNumber { get; set; } //Default to "" //An arbitrary ID code number perhaps from the institution

        [JsonProperty(PropertyName = "interests")]
        public string Interests { get; set; } //Optional //user interests (separated by commas)

        [JsonProperty(PropertyName = "firstaccess")]
        public int FirstAccess { get; set; } //Optional //first access to the site (0 if never)

        [JsonProperty(PropertyName = "lastaccess")]
        public int LastAccess { get; set; } //Optional //last access to the site (0 if never)

        [JsonProperty(PropertyName = "confirmed")]
        public int Confirmed { get; set; } //Optional //Active user: 1 if confirmed, 0 otherwise

        [JsonProperty(PropertyName = "lang")]
        public string Lang { get; set; } //Default to "en" //Language code such as "en", must exist on server

        [JsonProperty(PropertyName = "calendartype")]
        public string CalendarType { get; set; } //Default to "gregorian" //Calendar type such as "gregorian", must exist on server

        [JsonProperty(PropertyName = "theme")]
        public string Theme { get; set; } //Optional //Theme name such as "standard", must exist on server

        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; } //Optional //Timezone code such as Australia/Perth, or 99 for default

        [JsonProperty(PropertyName = "mailformat")]
        public int MailFormat { get; set; } //Optional //Mail format code is 0 for plain text, 1 for HTML etc

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } //Optional //User profile description, no HTML

        [JsonProperty(PropertyName = "descriptionformat")]
        public int DescriptionFormat { get; set; } // Optional //description format (1 = HTML, 0 = MOODLE, 2 = PLAIN or 4 = MARKDOWN)

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; } //Optional //Home city of the user

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; } //Optional //URL of the user

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; } //Optional //Home country code of the user, such as AU or CZ

        [JsonProperty(PropertyName = "profileimageurlsmall")]
        public string ProfileImageUrlSmall { get; set; } //User image profile URL - small version

        [JsonProperty(PropertyName = "profileimageurl")]
        public string ProfileImageUrl { get; set; } //User image profile URL - big version

        [JsonProperty(PropertyName = "firstnamephoenetic")]
        public string FirstNamePhonetic { get; set; } //Optional //The first name(s) phonetically of the user

        [JsonProperty(PropertyName = "lastnamephoenetic")]
        public string LastNamePhonetic { get; set; } //Optional //The family name phonetically of the user

        [JsonProperty(PropertyName = "middlename")]
        public string MiddleName { get; set; } //Optional //The middle name of the user

        [JsonProperty(PropertyName = "alternatename")]
        public string AlternateName { get; set; } //Optional //The alternate name of the user

        //public Dictionary<string, string> Preferences { get; set; } // Optional //User preferences
        //public Dictionary<string, string> CustomFields { get; set; } //Optional //User custom fields (also known as user profil fields)
    }
}
