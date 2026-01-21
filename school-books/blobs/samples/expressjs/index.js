const crypto = require('crypto');
const express = require('express');
const app = express();
const port = 5001;

app.get('/myblobs/:blobId', (req, res) => {
  // Important!!!
  // Validate that the user has the right to access that blobId

  const blobId = req.params.blobId;
  const HMACKey = 'BGrFkf9yQ9JJoA47oNBE';
  const blobServiceUrl = 'http://blobs.neispuo.mon.bg';

  const unixTimeSeconds = Math.floor(new Date().getTime() / 1000);
  const message = `${blobId}/${unixTimeSeconds}`;

  const hmac = crypto.createHmac('sha256', HMACKey).update(message);
  const urlSafeBase64HMAC =
    hmac.digest('base64')
    // Url-safe Base64 / RFC 4648
    // https://tools.ietf.org/html/rfc4648
    .replace('+', '-')
    .replace('/', '_')
    .replace(/=+$/, '');

  const location = `${blobServiceUrl}/${blobId}?t=${unixTimeSeconds}&h=${urlSafeBase64HMAC}`;
  res.redirect(location);
})

app.listen(port, () => {
  console.log(`listening at http://localhost:${port}`)
})
