using System.ComponentModel.DataAnnotations;

namespace VGLog.Models.Enums
{
    public enum GameStatusEnum
    {
        [Display(Name = "To play")]
        Toplay,

        [Display(Name = "Playing")]
        Playing,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "Dropped")]
        Dropped
    }
}
