using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Mocha.Refs.Web.Helpers
{
    public class EmailHelper
    {
        public void SendRegistrationConfirmation(string name, string to, string token)
        {
            using (var smtp = new SmtpClient("mail.mochaware.jp"))
            using (var mail = new MailMessage())
            {
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("mkamo@mochaware.jp", "Ise.meoto18");

                mail.From = new MailAddress("do-not-reply@refs.mochaware.jp");
                mail.To.Add(to);
                mail.Subject = "[Mochaware Refs] 登録の確認";
                var message =
@"{0}さん

このたびはMochaware Refsにご登録いただき、誠にありがとうございます。
下記URLにアクセスしていただくとユーザー登録が完了します。

{1}
";
                var url = string.Format("http://refs.mochaware.jp/reg_confirm/{0}", token);
                mail.Body = string.Format(message, name, url);

                smtp.Send(mail);
            }
        }
    }
}
