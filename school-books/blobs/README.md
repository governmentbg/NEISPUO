# blobs

> A service for downloading and uploading files via HTTP

## Uploading

Uploads are performed using a standard `POST` request with `Content-Type: multipart/form-data` to `/` from which only the first file is uploaded and the rest is ignored.

In production a valid bearer token from the neispuo OIDC server will be required.

The response is a json with the following properties
  - __name: string__: the name captured during upload
  - __size: number__: the size of the uploaded file in bytes
  - __blobId: number__: the blobId of the uploaded file
  - __location: string__: a ready-made url from where the file can be uploaded, **which will expire in `<BlobUrlExpiration>` minutes**

#### request
```
POST /
Content-Type: multipart/form-data; boundary=abcde12345
Authorization: Bearer ....  // only required in production

--abcde12345
Content-Disposition: form-data; name="profileImage "; filename="image1.png"
Content-Type: application/octet-stream
{…file content…}
--abcde12345--
```

#### response
```
Content-Type: application/json; charset=utf-8

{
  "name":"image1.png",
  "size":319075,
  "blobId":1008,
  "location":"http://blobs.neispuo.mon.bg/1008?t=1608203243&h=rT3sXUI1ibHZ0Xrb-zkB7FaSpeynP3haYuQRy00WD_Y"
}
```

## Downloading

#### request
```
GET /<blobId>?t=<unixTimestampSeconds>&h=<hmac>
```
  - __blobId__
  - __unixTimestampSeconds__: timestamp of the curent moment in unix time seconds, see [unix time](https://en.wikipedia.org/wiki/Unix_time)  
    *To account for clock differences a +/- `<ClockSkew>` seconds will be tolerated.*
  - __hmac__:
    ```
    url_safe_base64(
      hmac_sha256(
        ascii_bytes('<HmacKey>'),
        ascii_bytes('<blobId>/<unixTimestampSeconds>')
      )
    )
    ```
    *Url-safe Base64([RFC 4648](https://tools.ietf.org/html/rfc4648))* is a simple extension over Base64 where `+` is replaced by `-`, `/` by `_` and any `=` at the end are trimmed.
    ```javascript
    // javascript implementation
    function url_safe_base64(base64string) {
      return base64string.replace('+', '-')
        .replace('/', '_')
        .replace(/=+$/, '');
    }
    ```

## Examples

  - [Uploading with Uppy](samples/uppy)
  - [Downloading with an expressjs proxy](samples/expressjs)
  - [Downloading with an aspnetcore proxy](samples/aspnetcore)

## Test deployment configuration

  - __location__: `test-blobs.mon.bg`
  - __authentication__: bearer token not required for uplaods
  - __BlobUrlExpiration__: 30 minutes
  - __ClockSkew__: 5 seconds
  - __HmacKey__: `BGrFkf9yQ9JJoA47oNBE`

## Building and runing the app with containers

  - Build and run the docker image
  ```sh
  docker build -t blobs --target blobs .
  
  docker run -it --rm -p 5100:80 \
      --env ASPNETCORE_ENVIRONMENT='Development' \
      --env SB__Data__DbIP='host.docker.internal' \
      --env SB__Data__DbPort='1433' \
      --env SB__Data__DbUser='sa' \
      --env SB__Data__DbPass='pass' \
      --env SB__Data__DbName='neispuo' \
      --env SB__Blobs__HMACKey='pBLzUmoEHTcoEva6UELH'
    blobs:latest
  ```
