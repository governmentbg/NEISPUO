const { Issuer } = require('openid-client');

module.exports = Issuer.discover(process.env.PARENT_CLIENT_DISCOVERY)
  .then((azureIssuer) => {
    // console.log('Discovered issuer %s %O', azureIssuer.issuer, azureIssuer.metadata);
    console.log('Azure parent integration started');
    return new azureIssuer.Client({
      client_id: process.env.PARENT_CLIENT_ID,
      client_secret: process.env.PARENT_CLIENT_SECRET,
      redirect_uris: [`${process.env.ROOT_URI}/azure-parent-integration-callback`],
      response_types: ['code']
    });
  })
  .catch((e) => {
    console.log(e);
    console.error('An error occurred in azure-parent-integration.js');
    return null;
  }); // => Client);
