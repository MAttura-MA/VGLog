using System.ComponentModel.DataAnnotations;

namespace VGLog.Models
{
    public enum GameStatus
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
