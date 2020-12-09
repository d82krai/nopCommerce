
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

function handleFile() {
  debugger
  // console.log("handle file - " + JSON.stringify(event, null, 2));
  var files = document.getElementById('photoUpload').files;
  if (!files.length) {
    return alert('Please choose a file to upload first.');
  }
  var f = files[0];
  var fileName = f.name;

  const s3 = new AWS.S3({
    correctClockSkew: true,
    endpoint: 'https://s3.us-east-1.wasabisys.com', //use appropriate endpoint as per region of the bucket
    accessKeyId: 'HY8Z6VILJOXJRK7YH8B6',
    secretAccessKey: 'JUzVcLXvWCOIpb41z79b32htZuA20dJXzvvRkZMe',
    region: 'us-east-1'
    , logger: console
  });

  console.log('Loaded');
  const uploadRequest = new AWS.S3.ManagedUpload({
    params: { Bucket: 'epp', Key: fileName, Body: f },
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