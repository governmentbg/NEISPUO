class BissService {
  constructor() {
    this.baseUrl = "https://localhost:53952";
  }

  async version() {

    let response = await fetch(`${this.baseUrl}/version`);
    let version = (await response.json());
    console.log(version);
    return version;

  }

  async getSigner(){
    const model = {
    };

    const response = await fetch(`${this.baseUrl}/getsigner`, {
      method: 'POST', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors',
      headers: {
        'Content-Type': 'application/json'
        // 'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: JSON.stringify(model)
    });
    let signer =  await response.json();
    console.log(signer);
    return signer;
   }

  async signXml(xml, signer, signData){
    const model = {
      "version":"1.0",
      "signatureType":"signature",
      "contentType":"data",
      "hashAlgorithm":"SHA256",
      "contents":[
        this.$helper.utf8ToBase64(xml)
        //"PGRzOlNpZ25lZEluZm8geG1sbnM6ZHM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvMDkveG1sZHNpZyMiPjxkczpDYW5vbmljYWxpemF0aW9uTWV0aG9kIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS8xMC94bWwtZXhjLWMxNG4jIj48L2RzOkNhbm9uaWNhbGl6YXRpb25NZXRob2Q+PGRzOlNpZ25hdHVyZU1ldGhvZCBBbGdvcml0aG09Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvMDQveG1sZHNpZy1tb3JlI3JzYS1zaGEyNTYiPjwvZHM6U2lnbmF0dXJlTWV0aG9kPjxkczpSZWZlcmVuY2UgSWQ9InItaWQtNTFlYmIwY2IzYjdlNmE0ZTE4NjlkMjU2MzI4NDEwNTItMSIgVVJJPSIiPjxkczpUcmFuc2Zvcm1zPjxkczpUcmFuc2Zvcm0gQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy9UUi8xOTk5L1JFQy14cGF0aC0xOTk5MTExNiI+PGRzOlhQYXRoPm5vdChhbmNlc3Rvci1vci1zZWxmOjpkczpTaWduYXR1cmUpPC9kczpYUGF0aD48L2RzOlRyYW5zZm9ybT48ZHM6VHJhbnNmb3JtIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS8xMC94bWwtZXhjLWMxNG4jIj48L2RzOlRyYW5zZm9ybT48L2RzOlRyYW5zZm9ybXM+PGRzOkRpZ2VzdE1ldGhvZCBBbGdvcml0aG09Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvMDQveG1sZW5jI3NoYTI1NiI+PC9kczpEaWdlc3RNZXRob2Q+PGRzOkRpZ2VzdFZhbHVlPjk2MERydFN5TXhnUHdEWmVVbGJYRjFVVWMzOFU2M0NtYzZhRkV1d01qYnM9PC9kczpEaWdlc3RWYWx1ZT48L2RzOlJlZmVyZW5jZT48ZHM6UmVmZXJlbmNlIFR5cGU9Imh0dHA6Ly91cmkuZXRzaS5vcmcvMDE5MDMjU2lnbmVkUHJvcGVydGllcyIgVVJJPSIjeGFkZXMtaWQtNTFlYmIwY2IzYjdlNmE0ZTE4NjlkMjU2MzI4NDEwNTIiPjxkczpUcmFuc2Zvcm1zPjxkczpUcmFuc2Zvcm0gQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzEwL3htbC1leGMtYzE0biMiPjwvZHM6VHJhbnNmb3JtPjwvZHM6VHJhbnNmb3Jtcz48ZHM6RGlnZXN0TWV0aG9kIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS8wNC94bWxlbmMjc2hhMjU2Ij48L2RzOkRpZ2VzdE1ldGhvZD48ZHM6RGlnZXN0VmFsdWU+cmVHOE9Ob2p1blVwSDNZLzJJc0RucDYwSndMb2Z1TkZQWG51QjJqQ1Z3az08L2RzOkRpZ2VzdFZhbHVlPjwvZHM6UmVmZXJlbmNlPjwvZHM6U2lnbmVkSW5mbz4="
      ],
        // "signedContents":["IC6gNuE2NJaIRhBz7X1Wstz2vnVuq7y95m3PzRgbOrBGo/etYRucnIPcpQHGwFM6+eJRCois6oF3HEIkabbOqOcNqypvgbF1ttp7ZWiG26rAfaCzPHrLz9ul2OOCaBduoxCjYrCFOo2fonQMpmZ16Aw5ZW8QQDg5uWGTEQ9MwLmrULWbXcI3PbcJMfa4AqxlymEuRMY3SUo3qvm3JImh3aqB6AvfH4IjzCJgacfEArCWLZvp9nyirslewANJCS89NLGNfrDrSBVIvDcBFLX1pQD9bYkfmG5m5T9ZuPK/8AUTpF4wzzYYn7Dx77qhrf0MxV4T7+sxOeimj5Cbc/TagQ=="],
        // "signedContentsCert":["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"],
        "signedContents":[signData.signedContents],
        "signedContentsCert":[signData.signedContentsCert],

        "signerCertificateB64":signer.chain[0],
        "confirmText":["hash"]
        // contents: ["VGVzdERhdGE=" ],
        // contentType: 'data',
        // hashAlgorithm: "SHA-256",
        // signatureType: "signature"
    };

    console.log(xml);
    const response = await fetch(`${this.baseUrl}/sign`, {
      method: 'POST', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors',
      headers: {
        'Content-Type': 'application/json'
        // 'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: JSON.stringify(model)
    });
    let result =  await response.json();
    console.log(result);
    return result;
  }


}

export default new BissService();
