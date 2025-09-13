using System.ComponentModel.DataAnnotations;

namespace ElderCare.Api.Modules.Activities
{
    public sealed class RegisterDto
    {
        [Required] public int activity_id { get; set; }
        [Required] public int elderly_id { get; set; }
    }

    public sealed class SignInDto
    {
        [Required] public int activity_id { get; set; }
        [Required] public int elderly_id { get; set; }
        public bool attended { get; set; } = true;   // true=已参加, false=缺席
        public string? feedback { get; set; }        // 选填备注
    }
}
