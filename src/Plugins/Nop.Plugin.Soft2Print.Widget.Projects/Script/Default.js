var soft2print = soft2print || {};
soft2print.project = soft2print.project || {};
soft2print.project.settings = soft2print.project.settings || {};
soft2print.project.actions = (function () {

    var openProject = function (projectID) {
        window.location.href = soft2print.project.settings.openProjectUrl.split(soft2print.project.settings.projectIDPrefix).join(projectID);
        return false;
    };
    var copyProject = function (projectID) {
        window.location.href = soft2print.project.settings.copyProjectUrl.split(soft2print.project.settings.projectIDPrefix).join(projectID);
        return false;
    };
    var deleteProject = function (projectID) {
        window.location.href = soft2print.project.settings.deleteProjectUrl.split(soft2print.project.settings.projectIDPrefix).join(projectID);
        return false;
    };

    var setPreview = function (container, url) {
        container.onload = null;
        // here
        var parentContainer = $(container).parent();
        var largestContainerSize = Math.max(parentContainer.width(), parentContainer.height())
        var newUrl = url + "&size=" + largestContainerSize;

        var image = new Image()
        image.src = newUrl;
        image.onload = function () {
            $(container).attr("src", newUrl);

            var largestImageSize = Math.max(image.width, image.height);
            if (largestContainerSize > largestImageSize) {

                // Load a larger image
                var image2 = new Image()
                image2.src = newUrl;
                image2.onload = function () {
                    $(container).attr("src", newUrl + "&clear=true");
                }

                return false;

            }

            return false;
        };

        return false;
    };

    return {
        openProject: openProject,
        copyProject: copyProject,
        deleteProject: deleteProject,

        setPreview: setPreview
    };
})();

