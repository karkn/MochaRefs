using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Util.Web.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "*")] /// 10分
        public ActionResult ProfileImage(string userName)
        {
            var path = Server.MapPath("~/");
            path = Path.Combine(path, ConfigurationManager.AppSettings["ProfileImagesPath"]);
            path = Path.GetFullPath(path);
            path = Path.Combine(path, userName.Substring(0, 2));
            var imagePath = Path.Combine(path, userName + ".png");
            return File(imagePath, "image/png");
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "*")] /// 10分
        public ActionResult SmallProfileImage(string userName)
        {
            var path = Server.MapPath("~/");
            path = Path.Combine(path, ConfigurationManager.AppSettings["ProfileImagesPath"]);
            path = Path.GetFullPath(path);
            path = Path.Combine(path, userName.Substring(0, 2));
            var imagePath = Path.Combine(path, userName + "_small.png");
            return File(imagePath, "image/png");
        }
    }
}
