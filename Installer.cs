using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Metadata;
using Telerik.Sitefinity.Metadata.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security.Model;
using timw255.Sitefinity.Moodle.Security;
using Telerik.Sitefinity.Modules.UserProfiles;
using Telerik.Sitefinity.Modules.UserProfiles.Web.Services;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Modules.UserProfiles.Web.Services.Model;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Modules.UserProfiles.Configuration;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Web.UI.ContentUI.Config;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Fluent.Definitions;
using Telerik.Sitefinity.Web.UI.Fields.Enums;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Web.UI.ContentUI.Views.Backend.Master.Config;
using Telerik.Sitefinity.ModuleEditor.Web.Services.Model;
using Telerik.Sitefinity.Fluent.DynamicData;

namespace timw255.Sitefinity.Moodle
{
    public class Installer
    {
        public static void PreApplicationStart()
        {
            Bootstrapper.Initialized += (new EventHandler<ExecutedEventArgs>(Installer.Bootstrapper_Initialized));
        }

        private static void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName != "RegisterRoutes" || !Bootstrapper.IsDataInitialized)
            {
                return;
            }

            RegisterConfigurations();
            InitializeUserProfiles(false);
        }

        private static void RegisterConfigurations()
        {

        }

        private static void InitializeUserProfiles(bool destroyEverything)
        {
            var metaDataConfig = Config.Get<MetadataConfig>();
            string metaDataProviderName = metaDataConfig.DefaultProvider;

            var userProfileTypes = UserProfilesHelper.GetUserProfileTypes(metaDataProviderName);
            var moodleUserProfileType = userProfileTypes.Where(t => t.DynamicTypeName == typeof(MoodleProfile).FullName).FirstOrDefault();

            if (moodleUserProfileType == null && !destroyEverything)
            {
                var profileTypeData = UserProfileTypeViewModel.GetBlankItem(typeof(MoodleProfile)) as UserProfileTypeViewModel;

                profileTypeData.AppliedTo = "MoodleMembershipProvider";
                profileTypeData.DynamicTypeName = typeof(MoodleProfile).FullName;
                profileTypeData.Id = Guid.Empty;
                profileTypeData.MembershipProvidersUsage = MembershipProvidersUsage.SpecifiedProviders;
                profileTypeData.Name = "MoodleProfile";
                profileTypeData.ProfileProviderName = "MoodleProfileProvider";

                var providerViewModel = new ProviderViewModel[] {
                    new ProviderViewModel() {
                        ProviderFriendlyName = "Moodle",
                        ProviderName = "MoodleMembershipProvider"
                    }
                };

                profileTypeData.SelectedMembershipProviders = providerViewModel;
                profileTypeData.Title = "Moodle profile";


                ConfigManager manager = ConfigManager.GetManager();
                UserProfilesConfig section = manager.GetSection<UserProfilesConfig>();
                string str = string.Concat(typeof(MoodleProfile).Namespace, ".", profileTypeData.Name);
                if (section.ProfileTypesSettings.ContainsKey(str))
                {
                    throw new ArgumentException(Res.Get<UserProfilesResources>().ErrorProfileTypeAlreadyExists);
                }
                string title = profileTypeData.Title;
                MetadataManager metadataManager = MetadataManager.GetManager();
                if ((
                    from td in metadataManager.GetMetaTypeDescriptions()
                    where td.UserFriendlyName == title
                    select td).Count<MetaTypeDescription>() > 0)
                {
                    throw new ArgumentException(Res.Get<UserProfilesResources>().ErrorProfileTypeTitleAlreadyExists);
                }
                string name = profileTypeData.Name;
                MetaType fullName = metadataManager.CreateMetaType(typeof(MoodleProfile).Namespace, profileTypeData.Name);
                fullName.BaseClassName = typeof(SitefinityProfile).FullName;
                fullName.IsDynamic = true;
                fullName.DatabaseInheritance = DatabaseInheritanceType.vertical;
                MetaTypeDescription metaTypeDescription = metadataManager.CreateMetaTypeDescription(fullName.Id);
                UpdateUserProfileType(fullName, metaTypeDescription, section, profileTypeData);
                metadataManager.SaveChanges(true);
                manager.SaveSection(section);
                string fullTypeName = fullName.FullTypeName;
                profileTypeData.DynamicTypeName = fullTypeName;
                string contentViewDefinitionName = UserProfilesHelper.GetContentViewDefinitionName(profileTypeData.Name);

                ContentViewConfig contentViewConfig = manager.GetSection<ContentViewConfig>();
                ConfigElementDictionary<string, ContentViewControlElement> contentViewControls = contentViewConfig.ContentViewControls;
                ContentViewControlDefinitionFacade contentViewControlDefinitionFacade = App.WorkWith().Module().DefineContainer(contentViewControls, contentViewDefinitionName).SetContentTypeName(fullTypeName);
                ContentViewControlElement contentViewControlElement = contentViewControlDefinitionFacade.Get();

                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.BackendCreate, FieldDisplayMode.Write);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.BackendEdit, FieldDisplayMode.Write);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.BackendView, FieldDisplayMode.Read);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.FrontendCreate, FieldDisplayMode.Write);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.FrontendEdit, FieldDisplayMode.Write);
                InsertDetailView(contentViewControlDefinitionFacade, ProfileTypeViewKind.FrontendView, FieldDisplayMode.Read);

                contentViewControls.Add(contentViewControlElement);
                manager.SaveSection(contentViewConfig);
                UserProfileManager.GetManager().GetUserProfiles().Count<UserProfile>();
            }
            else if (moodleUserProfileType != null && destroyEverything)
            {
                Telerik.Sitefinity.Fluent.AppSettings appSetting = App.Prepare();
                if (!string.IsNullOrEmpty(metaDataProviderName))
                {
                    appSetting.MetadataProviderName = metaDataProviderName;
                }
                DynamicTypeDescriptionFacade dynamicTypeDescriptionFacade = appSetting.WorkWith().DynamicData().TypeDescription(moodleUserProfileType.Id);
                DynamicTypeFacade dynamicTypeFacade = dynamicTypeDescriptionFacade.DynamicType();
                Type clrType = dynamicTypeFacade.Get().ClrType;
                if (clrType == typeof(SitefinityProfile))
                {
                    throw new InvalidOperationException(Res.Get<UserProfilesResources>().ErrorDeleteBuiltInProfileType);
                }
                UserProfileManager userProfileManager = UserProfilesHelper.GetUserProfileManager(clrType, null);
                userProfileManager.DeleteProfilesForProfileType(clrType);
                userProfileManager.SaveChanges();
                dynamicTypeDescriptionFacade.Delete();
                dynamicTypeFacade.Delete();
                ConfigManager manager = ConfigManager.GetManager();
                UserProfilesConfig section = manager.GetSection<UserProfilesConfig>();
                section.ProfileTypesSettings.Remove(dynamicTypeFacade.Get().FullTypeName);
                ContentViewConfig contentViewConfig = manager.GetSection<ContentViewConfig>();
                string contentViewDefinitionName = UserProfilesHelper.GetContentViewDefinitionName(clrType);
                contentViewConfig.ContentViewControls.Remove(contentViewDefinitionName);
                dynamicTypeFacade.SaveChanges(true);
                manager.SaveSection(section);
                manager.SaveSection(contentViewConfig);
            }
        }

        internal static void UpdateUserProfileType(MetaType metaType, MetaTypeDescription typeDescription, UserProfilesConfig profilesConfig, UserProfileTypeViewModel profileTypeData)
        {
            string fullTypeName = metaType.FullTypeName;
            typeDescription.UserFriendlyName = profileTypeData.Title;
            UpdateConfiguration(profilesConfig, fullTypeName, profileTypeData);
        }

        internal static void UpdateConfiguration(UserProfilesConfig profilesConfig, string metaTypeFullName, UserProfileTypeViewModel profileTypeData)
        {
            ProfileTypeSettings profileTypeSettings = UserProfilesHelper.GetProfileTypeSettings(profilesConfig, metaTypeFullName, true);
            profileTypeSettings.ProfileProvider = profileTypeData.ProfileProviderName;
            profileTypeSettings.UseAllMembershipProviders = new bool?(profileTypeData.MembershipProvidersUsage == MembershipProvidersUsage.AllProviders);
            profileTypeSettings.MembershipProviders.Clear();
            if (profileTypeData.MembershipProvidersUsage == MembershipProvidersUsage.SpecifiedProviders)
            {
                ProviderViewModel[] selectedMembershipProviders = profileTypeData.SelectedMembershipProviders;
                for (int i = 0; i < (int)selectedMembershipProviders.Length; i++)
                {
                    ProviderViewModel providerViewModel = selectedMembershipProviders[i];
                    ConfigElementList<MembershipProviderElement> membershipProviders = profileTypeSettings.MembershipProviders;
                    MembershipProviderElement membershipProviderElement = new MembershipProviderElement(profileTypeSettings.MembershipProviders)
                    {
                        ProviderName = providerViewModel.ProviderName
                    };
                    membershipProviders.Add(membershipProviderElement);
                }
            }
        }

        private static void InsertDetailView(ContentViewControlDefinitionFacade fluentContentView, ProfileTypeViewKind viewKind, FieldDisplayMode displayMode)
        {
            ContentViewSectionElement contentViewSectionElement;
            DetailViewDefinitionFacade detailViewDefinitionFacade = fluentContentView.AddDetailView(UserProfilesHelper.GetContentViewName(viewKind)).SetTitle(UserProfilesHelper.GetContentViewTitle(viewKind)).HideTopToolbar().SetDisplayMode(displayMode).LocalizeUsing<UserProfilesResources>().DoNotRenderTranslationView().DoNotUseWorkflow();
            DetailFormViewElement detailFormViewElement = detailViewDefinitionFacade.Get();
            string str = CustomFieldsContext.customFieldsSectionName;
            if (!detailFormViewElement.Sections.TryGetValue(str, out contentViewSectionElement))
            {
                SectionDefinitionFacade<DetailViewDefinitionFacade> sectionDefinitionFacade = detailViewDefinitionFacade.AddExpandableSection(str).SetDisplayMode(detailFormViewElement.DisplayMode);
                contentViewSectionElement = sectionDefinitionFacade.Get();
            }
            fluentContentView.Get().ViewsConfig.Add(detailFormViewElement);
        }
    }
}
