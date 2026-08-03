var USER_ID = 0;
var USER_KEY = '';

var MESSAGE_NONE = 0;
var MESSAGE_ERROR = 1;
var MESSAGE_SUCCESS = 2;
var MESSAGE_INFO = 4;
var MESSAGE_WARNING = 5;

var MESSAGE_TYPE_SUCCESS = "success";
var MESSAGE_TYPE_INFO = "info";
var MESSAGE_TYPE_WARNING = "warning";
var MESSAGE_TYPE_ERROR = "error";
var REQUEST_TYPE_GET = "GET";
var REQUEST_TYPE_POST = "POST";

function createMessage(type, msg) {
    debugger;

    var title = "";
    if (type === "error") {
        title = "خطا";
    } else if (type === "success") {
        title = "موفقیت آمیز";
    } else if (type === "warning") {
        title = "هشدار";
    } else if (type === "info") {
        title = "اطلاع رسانی";
    }
    
    Command: toastr[type](msg, title)
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}

function createRequest() {
    var response = {
        type: "GET",
        headers: { userID: USER_ID, key: USER_KEY },
        contentType: "application/json; charset=utf-8",
        error: function (a) { console.log(a); }
    };
    return response;
}

function createRequest(entity) {
    var url = $("[codeprocess-ajax-form]").attr("codeprocess-ajax-form");
    if (url != undefined) {
        if (url.startsWith("/panel") == false) {
            url = base_admin_url + url;
        }
    }
    var response = {
        type: "POST",
        data: JSON.stringify(entity),
        url: url,
        headers: { userID: USER_ID, key: USER_KEY },
        contentType: "application/json; charset=utf-8",
        error: function (a) {
            createMessage("error", "خطا", a.responseText);
        },
        success: function (result) {
            if (result.Type === MESSAGE_ERROR) {
                createMessage(MESSAGE_TYPE_ERROR, result.Body);
            } else if (result.Type === MESSAGE_SUCCESS) {
                createMessage(MESSAGE_TYPE_SUCCESS, result.Body);
                var backUrl = getCodeprocessBackUrl();
                if (backUrl !== undefined) {
                    if (backUrl.toUpperCase().startsWith("/PANEL") == false) {
                        backUrl = base_admin_url + backUrl;
                    }
                    document.location = backUrl;
                }
            }
        }
    };
    return response;
}