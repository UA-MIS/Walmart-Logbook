using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace SecurityAssetManager.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        //[EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }

    public class RegisterViewModel
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "Username")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public List<Domain> Domains { get; set; }
        //public Guid DomainID { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class AdRegisterViewModel
    {
        [Required]
        [Display(Name = "AD Username")]
        public string Username { get; set; }

        [Display(Name = "Current Domain")]
        public string Domain { get; } = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name.ToUpper();

        public List<Domain> Domains { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "Username")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "Username")]
        public string Email { get; set; }
    }

    public class AdLoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }

    public static class DomainHelper
    {
        public static string CurrentDomain()
        {
            string st = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name.ToUpper();
            StringBuilder sb = new StringBuilder(st.Substring(0, st.IndexOf('.')));
            sb.Append(" Account");
            return sb.ToString();
        }
    }
}
