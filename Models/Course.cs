using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timw255.Sitefinity.Moodle.Models
{
    class Course
    {
        public string FullName { get; set; } //full name
        public string ShortName { get; set; } //course short name
        public int CategoryId { get; set; } //category id
        public string IdNumber { get; set; } //Optional //id number
        public string Summary { get; set; } //Optional //summary
        public int SummaryFormat { get; set; } //Default to "1" //summary format (1 = HTML, 0 = MOODLE, 2 = PLAIN or 4 = MARKDOWN)
        public string Format { get; set; } //Default to "weeks" //course format: weeks, topics, social, site,..
        public int ShowGrades { get; set; } //Default to "1" //1 if grades are shown, otherwise 0
        public int NewsItems { get; set; } //Default to "5" //number of recent items appearing on the course page
        public int StartDate { get; set; } //Optional //timestamp when the course start
        public int NumSections { get; set; } //Optional //(deprecated, use courseformatoptions) number of weeks/topics
        public int MaxBytes { get; set; } //Default to "0" //largest size of file that can be uploaded into the course
        public int ShowReports { get; set; } //Default to "0" //are activity report shown (yes = 1, no =0)
        public int Visible { get; set; } //Optional //1: available to student, 0:not available
        public int HiddenSections { get; set; } //Optional //(deprecated, use courseformatoptions) How the hidden sections in the course are displayed to students
        public int GroupMode { get; set; } //Default to "0" //no group, separate, visible
        public int GroupModeForce { get; set; } //Default to "0" //1: yes, 0: no
        public int DefaultGroupingId { get; set; } //Default to "0" //default grouping id
        public int EnableCompletion { get; set; } //Optional //Enabled, control via completion and activity settings. Disabled, not shown in activity settings.
        public int CompletionNotify { get; set; } //Optional //1: yes 0: no
        public string Lang { get; set; } //Optional //forced course language
        public string ForceTheme { get; set; } //Optional //name of the force theme
        public Dictionary<string, string> CourseFormatOptions { get; set; } //Optional //additional options for particular course format
    }
}
