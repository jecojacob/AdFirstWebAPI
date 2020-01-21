using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdviceFirstApi.Domain.Models.Dto;

namespace AdviceFirstApi.Domain.Models
{
    [Table("AF_USER")]
    public class AfUser
    {
        [Display(Description ="Autogeneratd field for user id")]
        [Column("user_id")]
        public int Id { get; set; }
        [Column("user_code")]
        [Display(Description ="User Code without domain name")]
        public string UserCode { get; set; }
        [Column("user_name")]
        [Display(Description ="Users Full Name")]
        public string UserName { get; set; }
        [Column("user_code_name")]
        [Display(Description ="Combination of user code and name for drop down in the application")]
        public string  UserCodeName { get; set; }
        [Column("user_password")]
        [Display(Description ="Password the code is encrypted")]
        
        public string UserPassword { get; set; }
        [Column("user_role")]
        [Display(Description ="User Role , Admin or User")]
        
        public string  UserRole { get; set; }
        [Column("user_status")]
        [Display(Description ="User Status - Active or Inacitive")]
        
        public AfUserStatus UserStatus { get; set; }
        
    }
}