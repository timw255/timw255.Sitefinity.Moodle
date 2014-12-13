using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timw255.Sitefinity.Moodle.Models
{
    public class MoodleException
    {
        public string Exception { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string DebugInfo { get; set; }
    }
}
