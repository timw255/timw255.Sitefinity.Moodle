using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timw255.Sitefinity.Moodle.Models
{
    public class UsersList
    {
        [JsonProperty(PropertyName = "users")]
        public List<MoodleUser> Users { get; set; }
    }
}
