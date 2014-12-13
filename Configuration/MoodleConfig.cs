using System;
using System.Configuration;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Localization;

namespace timw255.Sitefinity.Moodle.Configuration
{
    /// <summary>
    /// Sitefinity configuration section.
    /// </summary>
    /// <remarks>
    /// If this is a Sitefinity module's configuration,
    /// you need to add this to the module's Initialize method:
    /// App.WorkWith()
    ///     .Module(ModuleName)
    ///     .Initialize()
    ///         .Configuration<MoodleConfig>();
    /// 
    /// You also need to add this to the module:
    /// protected override ConfigSection GetModuleConfig()
    /// {
    ///     return Config.Get<MoodleConfig>();
    /// }
    /// </remarks>
    /// <see cref="http://www.sitefinity.com/documentation/documentationarticles/developers-guide/deep-dive/configuration/creating-configuration-classes"/>
    [ObjectInfo(Title = "MoodleConfig Title", Description = "MoodleConfig Description")]
    public class MoodleConfig : ConfigSection
    {
        [ObjectInfo(Title = "Text", Description = "This is a sample string field.")]
        [ConfigurationProperty("Text", DefaultValue = "Hello, World!")]
        public string ExampleConfigProperty
        {
            get
            {
                return (string)this["Text"];
            }
            set
            {
                this["Text"] = value;
            }
        }
    }
}