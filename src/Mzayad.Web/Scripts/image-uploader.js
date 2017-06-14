(function ($) {
    
    $.fn.fileUpload = function (options) {

        var settings = $.extend({
            itemId: "",
            parentId:"",
            imageUrl: "",
            uploadUrl: "",
            deleteUrl: "",
            acceptedFiles: "image/*",
            onUploadSuccessCallback: function () { },
            onDeleteSuccessCallback: function () { }
        }, options);

        var container = this;

        var itemId = settings.itemId;
        var parentId = settings.parentId;
        var imageUrl = settings.imageUrl;
        var uploadUrl = settings.uploadUrl;
        var deleteUrl = settings.deleteUrl;
        var acceptedFiles = settings.acceptedFiles;
        var onUploadSuccessCallback = settings.onUploadSuccessCallback;
        var onDeleteSuccessCallback = settings.onDeleteSuccessCallback;
        var elements = {
            uploader: $(".image-uploader-zone", container),
            instruction: $(".image-uploader-instruction", container),
            footer: $(".image-uploader-footer", container),
            progressSpinner: $(".image-progress-spinner", container),
            progressMessage: $(".image-progress-message", container),
            errorMessage: $(".image-error-message", container),
            deleteButton: $(".image-delete-button", container)
        };

        var getToken = function() {
            return $('input[name=__RequestVerificationToken]').val();
        };
        
        var onUploadSuccess = function (file, response) {
            elements.progressSpinner.hide();
            elements.deleteButton.show();
            updateBackgroundUrl(response.url);
            itemId = response.itemId;
            onUploadSuccessCallback(file, response);
        };
           
        var updateBackgroundUrl = function(url) {
            if (url) {
                elements.instruction.hide();
                elements.uploader.css("background-image", "url('" + url + "')");
            } else {
                elements.instruction.show();
                elements.uploader.css("background-image", "none");
            }         
        };
        
        var deletePhoto = function(url) {
            elements.progressSpinner.show();
            elements.progressMessage.text("Deleting...");
            elements.errorMessage.hide();
            elements.deleteButton.hide();

            $.ajax({
                type: 'POST',
                url: url,
                data: { __RequestVerificationToken: getToken(), itemId: itemId },
                dataType: 'json',
                success: onDeleteSuccess,
                error: onError
            });
        };
        
        var onDeleteSuccess = function() {
            updateBackgroundUrl();
            elements.errorMessage.hide();
            elements.deleteButton.hide();
            elements.progressSpinner.hide();
            onDeleteSuccessCallback();
        };
       

        var onError = function(error) {
            elements.progressSpinner.hide();
            elements.errorMessage.text(error.responseJSON.error);
            elements.errorMessage.show();
        }

        elements.uploader.dropzone({
            url: uploadUrl,
            acceptedFiles: acceptedFiles,
            sending: function(file, xhr, formData) {

                elements.errorMessage.hide();
                elements.progressSpinner.show();
                elements.progressMessage.text("Uploading...");

                formData.append("__RequestVerificationToken", getToken());
                formData.append("itemId", itemId);
                formData.append("parentId", parentId);       
            },
            success: onUploadSuccess,
            error: onError
        });

        
        if (imageUrl.length > 0) {
            updateBackgroundUrl(imageUrl);
            elements.deleteButton.show();
        }

        elements.deleteButton.click(function () {
             deletePhoto(deleteUrl);
        });

        elements.instruction.click(function () {
            elements.uploader.click();
        });

        return this;
    };
   
}(jQuery));