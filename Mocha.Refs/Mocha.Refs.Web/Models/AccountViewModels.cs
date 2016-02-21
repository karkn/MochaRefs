using System;
using System.ComponentModel.DataAnnotations;

namespace Mocha.Refs.Web.Models
{
    [Serializable]
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "ユーザー名")]
        public string UserName { get; set; }

        //[Required]
        //[Display(Name = "メールアドレス")]
        //[EmailAddress(ErrorMessage = "メールアドレスの形式が正しくありません。")]
        //public string Email { get; set; }

        [Display(Name = "表示名")]
        [StringLength(32, ErrorMessage = "{0} の長さは、{2} 文字以上である必要があります。", MinimumLength = 4)]
        public string DisplayName { get; set; }
    }

    [Serializable]
    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "現在のパスワード")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} の長さは、{2} 文字以上である必要があります。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新しいパスワード")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "新しいパスワードの確認入力")]
        [Compare("NewPassword", ErrorMessage = "新しいパスワードと確認のパスワードが一致しません。")]
        public string ConfirmPassword { get; set; }
    }

    [Serializable]
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "ユーザー名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }

        [Display(Name = "このアカウントを記憶する")]
        public bool RememberMe { get; set; }
    }

    [Serializable]
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "ユーザー名")]
        [StringLength(32, ErrorMessage = "{0} の長さは、{2} 文字以上である必要があります。", MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} の長さは、{2} 文字以上である必要があります。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "メールアドレス")]
        [EmailAddress(ErrorMessage = "メールアドレスの形式が正しくありません。")]
        public string Email { get; set; }

        [Display(Name = "表示名")]
        [StringLength(32, ErrorMessage = "{0} の長さは、{2} 文字以上である必要があります。", MinimumLength = 4)]
        public string DisplayName { get; set; }
    }

    [Serializable]
    public class EditPageViewModel
    {
        //[Required]
        //[Display(Name = "メールアドレス")]
        //[EmailAddress(ErrorMessage = "メールアドレスの形式が正しくありません。")]
        //public string Email { get; set; }

        [Required]
        [Display(Name = "表示名")]
        [StringLength(32, ErrorMessage = "{0} の長さは、{2} 文字以上である必要があります。", MinimumLength = 4)]
        public string DisplayName { get; set; }
    }
}
