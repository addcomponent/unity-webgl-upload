const GAMEOBJECT = "Bridge";
const FUNCTION_LOG = "Log";
const FUNCTION_FILE = "FileSelected";

var max_size = -1;

function SelectFile(maxSize) {

    max_size = maxSize;

    if (!document.getElementById('ImageUploaderInput')) {

        var fileInput = document.createElement('input');
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('id', 'ImageUploaderInput');
        fileInput.style.visibility = 'hidden';

        fileInput.onclick = function(event) {
            this.value = null;
        }

        fileInput.onchange = function(event) {

            var nBytes = 0,
                oFiles = event.target.files,
                nFiles = oFiles.length;
            for (var nFileId = 0; nFileId < nFiles; nFileId++) {
                nBytes += oFiles[nFileId].size;
            }
            var sOutput = nBytes + " bytes";

            for (var aMultiples = ["KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"], nMultiple = 0, nApprox = nBytes / 1024; nApprox > 1; nApprox /= 1024, nMultiple++) {
                sOutput = nApprox.toFixed(3) + " " + aMultiples[nMultiple] + " (" + nBytes + " bytes)";
                sendMessageToUnity(FUNCTION_LOG, sOutput);
            }

            var file = oFiles[0];
            var imageType = /^image\//;

            if (imageType.test(file.type)) {

                var reader = new FileReader();
                reader.onload = function(event) {

                    var image = new Image();
                    image.onload = function(imageEvent) {

                        sendMessageToUnity(FUNCTION_LOG, "WEB Image Loaded | " + image.width + "x" + image.height);

                        var canvas = document.createElement('canvas'),
                            width = image.width,
                            height = image.height;

                        if (max_size !== -1) {
                            if (width > height) {
                                if (width > max_size) {
                                    height *= max_size / width;
                                    width = max_size;
                                }
                            } else {
                                if (height > max_size) {
                                    width *= max_size / height;
                                    height = max_size;
                                }
                            }
                        }

                        canvas.width = width;
                        canvas.height = height;
                        canvas.getContext('2d').drawImage(image, 0, 0, width, height);

                        sendMessageToUnity(FUNCTION_LOG, "WEB Image Resized | " + canvas.width + "x" + canvas.height);

                        var dataUrl = canvas.toDataURL('image/jpeg');

                        sendMessageToUnity(FUNCTION_FILE, dataUrl);
                    }
                    image.src = event.target.result;
                }
                reader.readAsDataURL(file);
            } else {
                sendMessageToUnity(FUNCTION_LOG, "Selected File is not an Image");
            }
        }
        document.body.appendChild(fileInput);
    }

    var OpenFileDialog = function() {
        document.getElementById('ImageUploaderInput').click();
        document.getElementById('canvas').removeEventListener('click', OpenFileDialog);
    };
    document.getElementById('canvas').addEventListener('click', OpenFileDialog, false);
}

function sendMessageToUnity(func, message) {
  SendMessage(GAMEOBJECT, func, message);
}
