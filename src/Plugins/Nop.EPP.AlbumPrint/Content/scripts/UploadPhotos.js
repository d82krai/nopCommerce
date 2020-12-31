
$(window).on('load', function () {

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

  $('#photoUploadId').val(makeid(15));

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

function uploadFiles() {
  debugger
  var rootFolderName = 'AlbumPrints';
  var randomFolderName = $('#photoUploadId').val();//makeid(10);
  // console.log("handle file - " + JSON.stringify(event, null, 2));
  var files = document.getElementById('photoUpload').files;
  if (!files.length) {
    return alert('Please choose a file to upload first.');
  }
  var f = files[0];
  var fileName = f.name;

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

  for (var i = 0; i < files.length; i++) {
    //alert(files[i].name);

    completeFilePath = rootFolderName + '/' + randomFolderName + '/' + files[i].name;
    f = files[i];

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

}

function addalbumproducttocart_details(urladd, formselector) {
  debugger
  if (AjaxCart.loadWaiting !== false) {
    return;
  }
  AjaxCart.setLoadWaiting(true);

  uploadFiles();

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

function makeid(length) {
  var result = '';
  var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  var charactersLength = characters.length;
  for (var i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }
  return result;
}