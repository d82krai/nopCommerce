
$(window).on('load', function () {

});

$(function () {
  var id = makeid(15);
  $('#coverpadUploadId').val(id);
  $('#photoUploadId').val(id);
});

function openCity(evt, cityName) {
  var i, tabcontent, tablinks;
  tabcontent = document.getElementsByClassName("tabcontent");
  for (i = 0; i < tabcontent.length; i++) {
    tabcontent[i].style.display = "none";
  }
  tablinks = document.getElementsByClassName("tablinks");
  for (i = 0; i < tablinks.length; i++) {
    tablinks[i].className = tablinks[i].className.replace(" active", "");
  }
  document.getElementById(cityName).style.display = "block";
  evt.currentTarget.className += " active";
}

// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();

Dropzone.autoDiscover = false;

$(document).ready(function () {

  $("#dZUpload").dropzone({ url: "/file/post" });

  //$('#photoUploadId').val(makeid(15));

  //$("#dZUpload").dropzone({
  //  url: "/hn_SimpeFileUploader.ashx",
  //  addRemoveLinks: true,
  //  success: function (file, response) {
  //    var imgName = response;
  //    file.previewElement.classList.add("dz-success");
  //    console.log("Successfully uploaded :" + imgName);
  //  },
  //  error: function (file, response) {
  //    file.previewElement.classList.add("dz-error");
  //  }
  //});
});

function uploadPhotos() {
  debugger
  var rootFolderName = 'AlbumPrints';
  var randomFolderName = $('#photoUploadId').val();//makeid(10);
  // console.log("handle file - " + JSON.stringify(event, null, 2));
  //var files = document.getElementById('photoUpload').files;
  if ($('.photoPhotoCard').length == 0) {
    return alert('Please choose photo for album.');
  }
  var f = null;//files[0];
  var fileName = '';

  var completeFilePath = rootFolderName + '/' + randomFolderName + '/' + fileName;

  const s3 = new AWS.S3({
    correctClockSkew: true,
    endpoint: 'https://s3.us-east-1.wasabisys.com', //use appropriate endpoint as per region of the bucket
    accessKeyId: 'HY8Z6VILJOXJRK7YH8B6',
    secretAccessKey: 'JUzVcLXvWCOIpb41z79b32htZuA20dJXzvvRkZMe',
    region: 'us-east-1'
    , logger: console
  });

  console.log('Loaded');

  for (var i = 0; i < $('.photoPhotoCard').length; i++) {
    //alert(files[i].name);
    var fileCard = $('.photoPhotoCard')[i];
    var img = $(fileCard).find('img')[0];
    var fileName = $(img).data('filename');
    var size = $(img).data('size');
    var type = $(img).data('type');

    var file = dataURLtoFile($(img).attr('src'), fileName);

    var fileExtension = '.' + fileName.replace(/^.*\./, '');

    var ddlPhotoSheetType = $(fileCard).find('.ddlPhotoSheetType')[0];
    var sheetType = $(ddlPhotoSheetType).find('option:selected').text();
    var sheetTypeFixed = sheetType.replace(/ /g, "_").replace(/\./g, "_");

    completeFilePath = rootFolderName + '/' + randomFolderName + '/' + 'Photo' + i.toString() + ' ' + sheetTypeFixed + fileExtension;
    f = file;

    const uploadRequest = new AWS.S3.ManagedUpload({
      params: { Bucket: 'epp', Key: completeFilePath, Body: f },
      service: s3
    });

    uploadRequest.on('httpUploadProgress', function (event) {
      const progressPercentage = Math.floor(event.loaded * 100 / event.total);
      console.log('Upload progress ' + progressPercentage);
    });

    console.log('Configed and sending');

    uploadRequest.send(function (err) {
      if (err) {
        console.log('UPLOAD ERROR: ' + JSON.stringify(err, null, 2));
      } else {
        console.log('Good upload');
      }
    });

  }
  return true;
}

function uploadCoverpads() {
  debugger
  var rootFolderName = 'AlbumPrints';
  var randomFolderName = $('#coverpadUploadId').val();//makeid(10);
  // console.log("handle file - " + JSON.stringify(event, null, 2));
  //var files = document.getElementById('coverpadUpload').files;
  if ($('.coverPadPhotoCard').length == 0) {
    return alert('Please choose photo for coverpad.');
  }
  var f = null;//files[0];
  var fileName = '';

  var completeFilePath = rootFolderName + '/' + randomFolderName + '/' + fileName;

  const s3 = new AWS.S3({
    correctClockSkew: true,
    endpoint: 'https://s3.us-east-1.wasabisys.com', //use appropriate endpoint as per region of the bucket
    accessKeyId: 'HY8Z6VILJOXJRK7YH8B6',
    secretAccessKey: 'JUzVcLXvWCOIpb41z79b32htZuA20dJXzvvRkZMe',
    region: 'us-east-1'
    , logger: console
  });

  console.log('Loaded');

  for (var i = 0; i < $('.coverPadPhotoCard').length; i++) {
    //alert(files[i].name);
    var fileCard = $('.coverPadPhotoCard')[i];
    var img = $(fileCard).find('img')[0];
    var fileName = $(img).data('filename');
    var size = $(img).data('size');
    var type = $(img).data('type');

    var file = dataURLtoFile($(img).attr('src'), fileName);

    var fileExtension = '.' + fileName.replace(/^.*\./, '');

    completeFilePath = rootFolderName + '/' + randomFolderName + '/CoverPad/' + 'CoverPad' + i.toString() + fileExtension;
    f = file;

    const uploadRequest = new AWS.S3.ManagedUpload({
      params: { Bucket: 'epp', Key: completeFilePath, Body: f },
      service: s3
    });

    uploadRequest.on('httpUploadProgress', function (event) {
      const progressPercentage = Math.floor(event.loaded * 100 / event.total);
      console.log('Upload progress ' + progressPercentage);
    });

    console.log('Configed and sending');

    uploadRequest.send(function (err) {
      if (err) {
        console.log('UPLOAD ERROR: ' + JSON.stringify(err, null, 2));
      } else {
        console.log('Good upload');
      }
    });

  }
  return true;
}

function addalbumproducttocart_details(urladd, formselector) {
  debugger
  if (AjaxCart.loadWaiting !== false) {
    return;
  }
  AjaxCart.setLoadWaiting(true);

  if (uploadPhotos()) {
    uploadCoverpads();
  }

  //uploadCoverpads();

  $.ajax({
    cache: false,
    url: urladd,
    data: $(formselector).serialize(),
    type: "POST",
    success: AjaxCart.success_process,
    complete: AjaxCart.resetLoadWaiting,
    error: AjaxCart.ajaxFailure
  });
}

function UpdateCoverpadPrice(inputFileCtrl) {
  debugger

  var fileCount = inputFileCtrl == null ? 0 : $(inputFileCtrl)[0].files.length;
  var existingFileCount = $('.coverPadPhotoCard').length;

  var totalFileCount = fileCount + existingFileCount;

  if (totalFileCount > 0) {
    $('#product_attribute_14_48_qty').val(totalFileCount);
    $('#product_attribute_14_48_qty').trigger("paste");
  }
}

function makeid(length) {
  var result = '';
  var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  var charactersLength = characters.length;
  for (var i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }
  return result;
}

function dataURLtoFile(dataurl, filename) {

  var arr = dataurl.split(','),
    mime = arr[0].match(/:(.*?);/)[1],
    bstr = atob(arr[1]),
    n = bstr.length,
    u8arr = new Uint8Array(n);

  while (n--) {
    u8arr[n] = bstr.charCodeAt(n);
  }

  return new File([u8arr], filename, { type: mime });
}

$(function () {
  // Multiple images preview in browser
  var imagesPreview = function (input, placeToInsertImagePreview) {

    if (input.files) {
      var filesAmount = input.files.length;

      var maxCoverpadPhoto = $('#coverpadMaxPhoto').val();

      if (filesAmount != maxCoverpadPhoto) {
        alert('Please upload ' + maxCoverpadPhoto + ' photos for coverpad.');
        return;
      }

      for (i = 0; i < filesAmount; i++) {
        var reader = new FileReader();
        var file = input.files[i];
        reader.fileName = file.name;
        reader.size = file.size;
        reader.type = file.type;
        //reader.fileName = 
        //reader.onload = function (event) {
        //    $($.parseHTML('<img>')).attr('src', event.target.result).appendTo(placeToInsertImagePreview);
        //}

        reader.onload = function (event) {
          var html = '<div class="col-lg-3 col-md-4 col-6 coverPadPhotoCard">' +

            '<div class="input-group">' +
            //'<select title="Select Paper Type" class="custom-select input-sm ddlPhotoSheetType"></select>' +

            '<div class="input-group-append">' +
            '<button type="button" class="btn btn-danger input-sm btnRemovePhoto" title="Remove">x</button>' +
            '</div>' +
            '</div >' +


            '<a href="javascript:void(0)" class="d-block mb-4 h-100" >' +
            '<img class="img-fluid img-thumbnail" data-size="' + event.target.size + '" data-fileName="' + event.target.fileName + '" data-type="' + event.target.type + '" src="' + event.target.result + '" alt="">' +
            '</a>' +
            '</div >';

          $(placeToInsertImagePreview).append(html);
        }

        reader.readAsDataURL(input.files[i]);
      }
    }

  };

  $('#coverpadUpload').on('change', function () {
    imagesPreview(this, 'div.coverPadGallery');
    UpdateCoverpadPrice(this);
  });

  $('.ddlAllPhotoSheetType').on('change', function () {
    $('.ddlPhotoSheetType').each(function () {
      $(this).val($('.ddlAllPhotoSheetType').val());
    });
  });

  $(document).on('click', '.btnRemovePhoto', function () {
    $(this).closest('.coverPadPhotoCard').remove();
    setTimeout(
      UpdateCoverpadPrice(null)
      , 500);
  });

});


$(function () {
  // Multiple images preview in browser
  var photoPreview = function (input, placeToInsertImagePreview) {

    if (input.files) {
      var filesAmount = input.files.length;

      for (i = 0; i < filesAmount; i++) {
        var reader = new FileReader();
        var file = input.files[i];
        reader.fileName = file.name;
        reader.size = file.size;
        reader.type = file.type;
        //reader.fileName = 
        //reader.onload = function (event) {
        //    $($.parseHTML('<img>')).attr('src', event.target.result).appendTo(placeToInsertImagePreview);
        //}

        reader.onload = function (event) {
          var html = '<div class="col-lg-3 col-md-4 col-6 photoPhotoCard">' +

            '<div class="input-group">' +
            '<select title="Select Paper Type" class="custom-select input-sm ddlPhotoSheetType">';

          $(".ddlAllPhotoSheetType option").each(function () {
            html += '<option value="' + $(this).val() + '">' + $(this).text() + '</option>';
          });

          html += '</select>' +

            '<div class="input-group-append">' +
            '<button type="button" class="btn btn-danger input-sm btnRemovePhoto" title="Remove">x</button>' +
            '</div>' +
            '</div >' +


            '<a href="javascript:void(0)" class="d-block mb-4 h-100" >' +
            '<img class="img-fluid img-thumbnail" data-size="' + event.target.size + '" data-fileName="' + event.target.fileName + '" data-type="' + event.target.type + '" src="' + event.target.result + '" alt="">' +
            '</a>' +
            '</div >';

          $(placeToInsertImagePreview).append(html);
        }

        reader.readAsDataURL(input.files[i]);
      }
    }

  };

  $('#photoUpload').on('change', function () {
    photoPreview(this, 'div.photoGallery');
    UpdateCoverpadPrice(this);
  });

  $('.ddlAllPhotoSheetType').on('change', function () {
    $('.ddlPhotoSheetType').each(function () {
      $(this).val($('.ddlAllPhotoSheetType').val());
    });
  });

  $(document).on('click', '.btnRemovePhoto', function () {
    $(this).closest('.photoPhotoCard').remove();
    setTimeout(
      UpdateCoverpadPrice(null)
      , 500);
  });

});

$(function () {
  $(document).on('change', '.ddlAllPhotoSheetType, .ddlPhotoSheetType', function () {
    updatePhotoPrice();
  });
});

function updatePhotoPrice() {

  var ddlAllPhotoSheetTypes = $('.ddlAllPhotoSheetType option');
  var photoCards = $('.photoPhotoCard');

  for (var i = 0; i < ddlAllPhotoSheetTypes.length; i++) {
    var option = ddlAllPhotoSheetTypes[i];
    console.log($(option).val());
    var txtQtyClass = $(option).val().replace('ddlVal', '.qtyVal');
    var qty = 0;
    for (var j = 0; j < $('.ddlPhotoSheetType').length; j++) {
      var itemDdl = $('.ddlPhotoSheetType')[j];
      if ($(option).val() == $(itemDdl).val()) {
        qty++;
      }
    }


    $(txtQtyClass).val(qty);
    $(txtQtyClass).trigger("paste");
  }

  //for (var i = 0; i < photoCards.length - 1; i++) {
  //  var card = photoCards[i];



  //}

}
