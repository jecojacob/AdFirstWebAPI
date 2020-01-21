using System.ComponentModel.DataAnnotations;

namespace AdviceFirstApi.Domain.Models.Dto
{
    public class AfUserModelCreate
    {
        [Required]
        [MaxLength(5)]
        public string UserCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }
        [Required]
        public string UserCodeName { get; set; }
        [Required]
        public string UserPassword { get; set; }
        [Required]
        public string UserRole { get; set; }
        public AfUserStatus UserStatus { get; set; }

    }
}