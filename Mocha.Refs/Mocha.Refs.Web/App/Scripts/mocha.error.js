/*!
 * Mocha Error
 *
 * 必須変数:
 *   $mc.env.basePath
 *   $mc.env.initialized
 */
var mc = mc || {};

(function ($mc) {
    "use strict";

    function showErrorModal(message) {
        var modal = $("#error-modal");

        $("#error-modal-message", modal).text(message);

        var ok = $(".btn-primary", modal);
        ok.off("click");
        ok.on("click", function () {
            location.reload(true);
        });
        modal.modal("show");
    }

    $mc.error = {
        sendClientErrorLog: function (kind, message, url, lineNumber, stackTrace) {
            var data = {
                kind: kind,
                message: message,
                url: url,
                lineNumber: lineNumber,
                stackTrace: stackTrace
            };
            $.ajax({
                url: $mc.env.basePath + "System/LogClientError",
                data: JSON.stringify(data),
                type: "POST",
                contentType: "application/json",
                error: null
            });

            var modalMsg=
                "画面の処理中にエラーが発生しました。" +
                "ご迷惑をおかけしております。";

            if ($mc.env.initialized) {
                showErrorModal(modalMsg);
            }
        },

        handleAjaxError: function (xhr) {
            var data = JSON.parse(xhr.responseText);
            var message = $mc.env.isDebug || data.IsBusinessError ?
                data.Message :
                "要求の処理中にエラーが発生しました。" +
                "しばらく時間をおいてからのアクセスをお願いします。" +
                "ご迷惑をおかけしております。";

            showErrorModal(message);
        }
    };

    window.onerror = function (message, url, lineNumber) {
        $mc.error.sendClientErrorLog("window.onerror", message, url, lineNumber, null);
        return true;
    };
})(mc);
