namespace FBS.Shared.Constants
{
    using FBS.Shared.Enums;

    public static class ConfigurationConstants
    {
        public static List<string> GetConfigurationsByTab(string tab)
        {
            switch (tab)
            {
                case Tab.Website: return Key.Website;
                case Tab.Account: return Key.Account;
                case Tab.Project: return Key.Project;
            }

            return new List<string>();
        }

        public static ConfigurationInputTypeEnum GetConfigurationInputType(string configurationKey)
        {
            var numberFields = new List<string>
            {
            };

            var fileFields = new List<string>
             {
                 nameof(Key.DefaultBgLogin),
                 nameof(Key.DefaultImage),
             };

            if (numberFields.Contains(configurationKey))
            {
                return ConfigurationInputTypeEnum.Number;
            }
            else if (fileFields.Contains(configurationKey))
            {
                return ConfigurationInputTypeEnum.File;
            }

            return ConfigurationInputTypeEnum.Text;
        }

        public static class Tab
        {
            public const string Website = "Website";
            public const string Account = "Account";
            public const string Project = "Project";

            public static readonly List<string> List = new List<string>
            {
                nameof(Website),
                nameof(Account),
                nameof(Project),
            };
        }

        public static class Key
        {
            public const string AppName = "AppName";
            public const string Phone = "Phone";
            public const string Email = "Email";
            public const string Address = "Address";
            public const string DefaultPassword = "DefaultPassword";
            public const string DefaultImage = "DefaultImage";
            public const string DefaultBgLogin = "DefaultBgLogin";

            public static readonly List<string> Website = new List<string>
            {
                nameof(AppName),
                nameof(Phone),
                nameof(Email),
                nameof(Address),
            };

            public static readonly List<string> Account = new List<string>
            {
                nameof(DefaultPassword),
            };

            public static readonly List<string> Project = new List<string>
            {
                nameof(DefaultImage),
            };
        }
    }
}
