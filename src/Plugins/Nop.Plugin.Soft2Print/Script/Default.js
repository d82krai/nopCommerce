var soft2print = soft2print || {};
soft2print.openModule = soft2print.openModule || {};
soft2print.openModule.actions = (function () {

    var open = function (url, formContainer) {
        if (formContainer != null) {
            // as Post
            var form = $(formContainer);
            form.attr("action", url);
            form.submit();
        } else {
            // as Get
            window.location.href = url;
            return false;
        }
    };

    return {
        open: open
    };
})();

