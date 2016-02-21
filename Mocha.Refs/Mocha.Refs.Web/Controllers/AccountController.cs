using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Mocha.Refs.Web.Models;
using Mocha.Refs.Core.Handlers;
using System.Web.Helpers;
using System.IO;
using System.Security.AccessControl;
using Mocha.Refs.Core.Repositories;
using Mocha.Refs.Core.Entities;
using IdentityUser = Mocha.Refs.Core.Auth.IdentityUser;
using Mocha.Refs.Core.Auth;
using ImageResizer;
using System.Configuration;

namespace Mocha.Refs.Web.Controllers
{
    [Authorize]
    public class AccountController : AbstractController
    {
        private IRefListHandler _refListHandler;

        public AccountController(IUserHandler handler, IRefListHandler refListHandler, IRefsContext refsContext): base(handler)
        {
            var store = new UserStore(refsContext);
            UserManager = new UserManager<IdentityUser>(store);

            _refListHandler = refListHandler;
        }

        public UserManager<IdentityUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "ユーザー名またはパスワードが正しくありません");
                }
            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //
        // POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (await _UserHandler.ExistsMochaUserByUserNameAsync(model.UserName))
        //    {
        //        ModelState.AddModelError("", "登録済みのユーザー名です。");
        //        return View(model);
        //    }
        //    if (await _UserHandler.ExistsMochaUserByEmailAsync(model.Email))
        //    {
        //        ModelState.AddModelError("", "登録済みのメールアドレスです。");
        //        return View(model);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var dispName = string.IsNullOrWhiteSpace(model.DisplayName) ? model.UserName : model.DisplayName;
        //        var user = new MochaUser()
        //        {
        //            UserName = model.UserName,
        //            Email = model.Email,
        //            DisplayName = dispName,
        //        };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInAsync(user, isPersistent: false);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            AddErrors(result);
        //        }
        //    }

        //    // ここで問題が発生した場合はフォームを再表示します
        //    return View(model);
        //}

        [HttpGet]
        public ActionResult Edit()
        {
            var user = GetUser();
            var vm = new EditPageViewModel()
            {
                //Email = user.Email,
                DisplayName = user.DisplayName,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditPageViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = GetUser();
                //if (vm.Email != user.Email)
                //{
                //    if (await _UserHandler.ExistsMochaUserByEmailAsync(vm.Email))
                //    {
                //        ModelState.AddModelError("", "登録済みのメールアドレスです。");
                //        return View(vm);
                //    }
                //}

                var idUser = await UserManager.FindByIdAsync(user.Id.ToString());

                //mochaUser.Email = vm.Email;
                idUser.User.DisplayName = vm.DisplayName;
                await UserManager.UpdateAsync(idUser);

                //user.Email = vm.Email;
                user.DisplayName = vm.DisplayName;
                await _UserHandler.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult UploadProfileImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadProfileImage(HttpPostedFileBase profileImageFile)
        {
            if (profileImageFile == null || !profileImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("", "画像ファイルを指定してください。");
                return View();
            }
            if (profileImageFile.ContentLength > 200 * 1024)
            {
                ModelState.AddModelError("", "ファイルサイズが200KB以下の画像ファイルを指定してください。");
                return View();
            }

            var user = GetUser();
            var userName = user.UserName;

            /// パスとフォルダの準備
            var path = Server.MapPath("~/App_Data/ProfileImages/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, user.UserName.Substring(0, 2));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            /// 128x128サイズの保存
            var imagePath = Path.Combine(path, userName + ".png");
            var job = new ImageJob(profileImageFile, imagePath, new Instructions("format=png&width=128&height=128&scale=both"));
            ImageBuilder.Current.Build(job);

            /// 48x48サイズの保存
            imagePath = Path.Combine(path, userName + "_small.png");
            job = new ImageJob(profileImageFile, imagePath, new Instructions("format=png&width=48&height=48&scale=both"));
            ImageBuilder.Current.Build(job);

            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Disassociate
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message = null;
        //    IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("Manage", new { Message = message });
        //}

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "パスワードが変更されました。"
                : message == ManageMessageId.SetPasswordSuccess ? "パスワードが設定されました。"
                : message == ManageMessageId.RemoveLoginSuccess ? "外部ログインが削除されました。"
                : message == ManageMessageId.Error ? "エラーが発生しました。"
                : "";
            //ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Manage(ManageUserViewModel model)
        //{
        //    bool hasPassword = HasPassword();
        //    ViewBag.HasLocalPassword = hasPassword;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasPassword)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a password so remove any validation errors caused by a missing OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }

        //    // ここで問題が発生した場合はフォームを再表示します
        //    return View(model);
        //}

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 外部ログイン プロバイダーへのリダイレクトを要求します
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                var exIdentity = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                var email = exIdentity.FindFirstValue(ClaimTypes.Email);

                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View(
                    "ExternalLoginConfirmation",
                    new ExternalLoginConfirmationViewModel {
                        UserName = loginInfo.DefaultUserName,
                        //Email = email,
                    }
                );
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (await _UserHandler.ExistsUserByUserNameAsync(model.UserName))
            {
                ModelState.AddModelError("", "登録済みのユーザー名です。");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            //if (await _UserHandler.ExistsMochaUserByEmailAsync(model.Email))
            //{
            //    ModelState.AddModelError("", "登録済みのメールアドレスです。");
            //    ViewBag.ReturnUrl = returnUrl;
            //    return View(model);
            //}

            if (ModelState.IsValid)
            {
                // 外部ログイン プロバイダーからユーザーに関する情報を取得します
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var exIdentity = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                var email = exIdentity.FindFirstValue(ClaimTypes.Email); /// twitterの場合は取れない

                var dispName = string.IsNullOrWhiteSpace(model.DisplayName) ? model.UserName : model.DisplayName;
                var user = new User()
                {
                    UserName = model.UserName,
                    Email = email,
                    DisplayName = dispName,
                };
                var idUser = new IdentityUser(user);
                var result = await UserManager.CreateAsync(idUser);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(idUser.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(idUser, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        ////
        //// GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            else
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            //ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            ViewBag.ShowRemoveButton = linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region ヘルパー
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

            //var mochaUserId = new Guid(user.Id);
            //var exists = await _UserHandler.ExistsUserByMochaUserIdAsync(mochaUserId);
            //if (!exists)
            //{
            //    /// initialize user
            //    await _UserHandler.CreateUserByMochaUserIdAsync(mochaUserId);
            //    //var createdUser = await _UserHandler.CreateUserByMochaUserIdAsync(mochaUserId);
            //    //await _refListHandler.CreateUnfiledRefListAsync(createdUser.Id);
            //}
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //private bool HasPassword()
        //{
        //    var user = UserManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PasswordHash != null;
        //    }
        //    return false;
        //}

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}