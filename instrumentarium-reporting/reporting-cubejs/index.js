const CubejsServer = require('@cubejs-backend/server');

const port = +process.env.PORT;
const cube = require('./cube');

const server = new CubejsServer({
  ...cube,
  jwt: {
    jwkUrl: process.env.CUBEJS_JWK_URL,
    algorithms: ['OKP', 'EC', 'RSA'],
  },
});

server.listen({ port }).then(({ version }) => {
  console.info(`🚀 Cube.js server (${version}) is listening on ${port}`);
});
