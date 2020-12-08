
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