const GAMEOBJECT = "Bridge";
const FUNCTION_LOG = "Log";
const FUNCTION_FILE = "FileSelected";
const FUNCTION_HAS_CAMERA = "HasCameraCallback";

var max_size = -1;

function HasCamera() {
    var result = true;

    // Prefer camera resolution nearest to 1280x720.
    var constraints = {
        audio: false,
        video: {
            width: {
                ideal: 324,
                max: 324
            },
            height: {
                ideal: 492,
                max: 492
            }
        }
    };

    if (navigator.mediaDevices === undefined) {
        navigator.mediaDevices = {};
    }

    if (navigator.mediaDevices.getUserMedia === undefined) {
        navigator.mediaDevices.getUserMedia = function(constraints) {

            // First get ahold of the legacy getUserMedia, if present
            var getUserMedia = navigator.webkitGetUserMedia || navigator.mozGetUserMedia;

            // Some browsers just don't implement it - return a rejected promise with an error
            // to keep a consistent interface
            if (!getUserMedia) {
                result = false;
            }
        }
    }

    sendMessageToUnity(FUNCTION_HAS_CAMERA, result.toString());
}

function SelectFile(maxSize) {

    // Prefer camera resolution nearest to 1280x720.
    var constraints = {
        audio: false,
        video: {
            width: 1280,
            height: 720
        }
    };

    // Older browsers might not implement mediaDevices at all, so we set an empty object first
    if (navigator.mediaDevices === undefined) {
        navigator.mediaDevices = {};
    }

    // Some browsers partially implement mediaDevices. We can't just assign an object
    // with getUserMedia as it would overwrite existing properties.
    // Here, we will just add the getUserMedia property if it's missing.
    if (navigator.mediaDevices.getUserMedia === undefined) {
        navigator.mediaDevices.getUserMedia = function(constraints) {

            // First get ahold of the legacy getUserMedia, if present
            var getUserMedia = navigator.webkitGetUserMedia || navigator.mozGetUserMedia;

            // Some browsers just don't implement it - return a rejected promise with an error
            // to keep a consistent interface
            if (!getUserMedia) {
                alert('What are you doing here? Did you forgot to user the HasCamera Method? getUserMedia is not implemented in this browser');
            }

            // Otherwise, wrap the call to the old navigator.getUserMedia with a Promise
            return new Promise(function(resolve, reject) {
                getUserMedia.call(navigator, constraints, resolve, reject);
            });
        }
    }

    navigator.mediaDevices.getUserMedia({
            audio: false,
            video: true
        })
        .then(function(stream) {
            // var video = document.querySelector('video');
            var video = document.getElementById('CameraStream');

            // Older browsers may not have srcObject
            if ("srcObject" in video) {
                video.srcObject = stream;
            } else {
                // Avoid using this in new browsers, as it is going away.
                video.src = window.URL.createObjectURL(stream);
            }
            video.onloadedmetadata = function(e) {
                video.play();
                video.style.objectFit = "cover";

                document.getElementById("unity").style.display = "none";
                document.getElementById("video").style.display = "";
            };
        })
        .catch(function(err) {
            console.log(err.name + ": " + err.message);
        });

    var buttonPause = document.getElementById('btnCapture');
    var buttonResume = document.getElementById('btnReset');
    var buttonSave = document.getElementById('btnSave');

    buttonResume.style.display = "none";
    buttonSave.style.display = "none";

    buttonPause.addEventListener("click", function(e) {
        e.preventDefault();

        buttonPause.style.display = "none";

        buttonResume.style.display = "";
        buttonSave.style.display = "";

        // var snap = takeSnapshot();
        //
        // var image = document.getElementById("snap");
        //
        // // Show image.
        // image.setAttribute('src', snap);
        // image.classList.add("visible");
        //
        var video = document.getElementById('CameraStream');

        // Pause video playback of stream.
        video.pause();
    });

    buttonResume.addEventListener("click", function(e) {
        e.preventDefault();

        buttonResume.style.display = "none";
        buttonSave.style.display = "none";

        buttonPause.style.display = "";

        var video = document.getElementById('CameraStream');

        // Pause video playback of stream.
        video.play();
    });

    buttonSave.addEventListener("click", function(e) {
        e.preventDefault();

        sendMessageToUnity(FUNCTION_FILE, takeSnapshot());

        document.getElementById("unity").style.display = "";
        document.getElementById("video").style.display = "none";
    });
}

function takeSnapshot() {
    // Here we're using a trick that involves a hidden canvas element.

    var hidden_canvas = document.getElementById('cameraCanvas'),
        context = hidden_canvas.getContext('2d');

    var video = document.getElementById('CameraStream');
    var width = video.videoWidth,
        height = video.videoHeight;

    if (width && height) {

        // Setup a canvas with the same dimensions as the video.
        hidden_canvas.width = width;
        hidden_canvas.height = height;

        // Make a copy of the current frame in the video on the canvas.
        context.drawImage(video, 0, 0, width, height);

        // Turn the canvas image into a dataURL that can be used as a src for our photo.
        return hidden_canvas.toDataURL('image/png');
    }
}

function SelectFile_(maxSize) {

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

function hasGetUserMedia() {
    // Note: Opera builds are unprefixed.
    return !!(navigator.getUserMedia || navigator.webkitGetUserMedia ||
        navigator.mozGetUserMedia || navigator.msGetUserMedia);
}
