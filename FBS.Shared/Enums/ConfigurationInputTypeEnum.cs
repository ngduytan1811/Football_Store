namespace FBS.Shared.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum ConfigurationInputTypeEnum
    {
        /// <summary>
        /// Text
        /// </summary>
        [Display(Name = "Text", Order = 1)]
        Text = 1,

        /// <summary>
        /// Number
        /// </summary>
        [Display(Name = "Number", Order = 1)]
        Number = 2,

        /// <summary>
        /// File
        /// </summary>
        [Display(Name = "File", Order = 3)]
        File = 3,
    }
}
