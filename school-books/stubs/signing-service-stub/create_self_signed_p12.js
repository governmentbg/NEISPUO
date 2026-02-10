const forge = require('node-forge');
const fs = require('fs');
const path = require('path');

function generateKeyPair() {
  return new Promise((resolve, reject) => {
    forge.pki.rsa.generateKeyPair({ bits: 2048, workers: -1 }, (err, keypair) => {
      if (err) {
        reject(err);
      } else {
        resolve(keypair);
      }
    });
  });
}

function createSelfSignedCert(keypair) {
  const cert = forge.pki.createCertificate();
  cert.publicKey = keypair.publicKey;
  cert.serialNumber = '01';
  cert.validity.notBefore = new Date();
  cert.validity.notAfter = new Date();
  cert.validity.notAfter.setFullYear(cert.validity.notBefore.getFullYear() + 20);

  const attrs = [{
    name: 'commonName',
    value: 'Test Director'
  }];

  cert.setSubject(attrs);
  cert.setIssuer(attrs);
  cert.setExtensions([{
    name: 'basicConstraints',
    cA: true
  }, {
    name: 'keyUsage',
    keyCertSign: true,
    digitalSignature: true,
    nonRepudiation: true,
    keyEncipherment: true,
    dataEncipherment: true
  }, {
    name: 'extKeyUsage',
    serverAuth: true,
    clientAuth: true,
    codeSigning: true,
    emailProtection: true,
    timeStamping: true
  }, {
    name: 'nsCertType',
    client: true,
    server: true,
    email: true,
    objsign: true,
    sslCA: true,
    emailCA: true,
    objCA: true
  }]);

  cert.sign(keypair.privateKey, forge.md.sha256.create());

  return cert;
}

function createPKCS12Bundle(cert, keypair, password) {
  const p12Asn1 = forge.pkcs12.toPkcs12Asn1(
    keypair.privateKey,
    cert,
    password,
    { algorithm: '3des', generateLocalKeyId: true, friendlyName: 'test_director' }
  );
  const p12Der = forge.asn1.toDer(p12Asn1).getBytes();
  return Buffer.from(p12Der, 'binary');
}

(async () => {
  const keypair = await generateKeyPair();
  const cert = createSelfSignedCert(keypair);
  const password = 'password';
  const p12 = createPKCS12Bundle(cert, keypair, password);

  fs.writeFileSync(path.join(__dirname, 'certificate.p12'), p12);
})();
