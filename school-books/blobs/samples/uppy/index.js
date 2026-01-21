"use strict";

function computeHMAC(key, message) {
  const enc = new TextEncoder();
  const algorithm = { name: "HMAC", hash: "SHA-512" };

  return crypto.subtle
    .importKey("raw", enc.encode(key), algorithm, false, ["sign", "verify"])
    .then((cryptoKey) =>
      crypto.subtle.sign(algorithm.name, cryptoKey, enc.encode(message))
    )
    .then((signature) => {
      // convert bytes to base64 string
      const base64Str = btoa(String.fromCharCode(...new Uint8Array(signature)));

      const urlSafeBase64HMAC = base64Str
        // Url-safe Base64 / RFC 4648
        // https://tools.ietf.org/html/rfc4648
        .replace("+", "-")
        .replace("/", "_")
        .replace(/=+$/, "");

      return urlSafeBase64HMAC;
    });
}

const key = "pBLzUmoEHTcoEva6UELH";
const unixTimeSeconds = Math.floor(new Date().getTime() / 1000);
const message = `${unixTimeSeconds}`;

computeHMAC(key, message).then((hmac) => {
  const uppy = new Uppy.Core({ debug: true, autoProceed: true });
  uppy.use(Uppy.FileInput, {
    target: ".UppyForm",
    replaceTargetContent: true,
  });
  uppy.use(Uppy.ProgressBar, {
    target: ".UppyProgressBar",
    hideAfterFinish: false,
  });
  uppy.use(Uppy.XHRUpload, {
    endpoint: `http://localhost:5189?t=${unixTimeSeconds}&h=${hmac}`,
    formData: true,
    fieldName: "files[]",
  });

  // And display uploaded files
  uppy.on("upload-success", (file, response) => {
    const url = response.body.location;
    const fileName = response.body.name;

    const li = document.createElement("li");
    const a = document.createElement("a");
    a.href = url;
    a.target = "_blank";
    a.appendChild(document.createTextNode(fileName));
    li.appendChild(a);

    document.querySelector(".uploaded-files ol").appendChild(li);
  });
});
