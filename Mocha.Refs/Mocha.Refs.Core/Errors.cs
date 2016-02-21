using Mocha.Common.Exceptions;
using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core
{
    public static class Errors
    {
        public static Error UpdateConcurrency = new Error("E1000", "古いデータを使って更新しようとしました。");
        public static Error InvalidUserWrite = new Error("E1001", "編集権限のないデータを更新しようとしました。");
        public static Error TagUseNameAlreadyExists = new Error("E1002", "{0}はすでに使用されているタグ名です。");
        public static Error UserNotFound = new Error("E1003", "{0}は存在しないユーザー名です。");
        public static Error UrlTooLong = new Error("E1004", "URLが長すぎます。");
        public static Error InvalidUrlFormat = new Error("E1005", "URLの形式が正しくありません。");
        public static Error TitleForUrlNotFound = new Error("E1006", "タイトルが見つかりませんでした。");
        public static Error DescriptionForUrlNotFound = new Error("E1007", "説明が見つかりませんでした。");
        public static Error ImageForUrlNotFound = new Error("E1008", "画像が見つかりませんでした。");
        public static Error HtmlForUrlDownloadFailed = new Error("E1009", "HTMLのダウンロードに失敗しました。");
    }
}
