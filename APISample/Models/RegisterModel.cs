using System.ComponentModel.DataAnnotations;
using System;
namespace APISample.Models
{
    public class RegisterModel
    {
        [Key]
        [Display(Name = "User ID")]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First name")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last name")]
        public required string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        [Display(Name = "Gender")]
        public required string Gender { get; set; }

        [StringLength(20)]
        [Display(Name = "Phone number")]
        public required string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email address")]
        public required string EmailAddress { get; set; }

        [StringLength(200)]
        [Display(Name = "Address")]
        public required string Address { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Username")]
        public required string Username { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Password")]
        public required string Password { get; set; }
    }
}


