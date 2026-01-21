const { JwksClient } = require('jwks-rsa');
const jwt = require('jsonwebtoken');
const https = require('https');
const http = require('http');
const RoleEnum = require('./role.enum');

const jwksClient = new JwksClient({
  jwksUri: process.env.CUBEJS_JWK_URL,
  timeout: 60000, // Defaults to 30s,
  strictSsl: process.env.STRICT_SSL === 'true',
  requestAgent:
    process.env.NODE_ENV !== 'dev'
      ? new https.Agent({
          rejectUnauthorized: false,
        })
      : new http.Agent(),
});

// from example: https://www.npmjs.com/package/jsonwebtoken#jwtverifytoken-secretorpublickey-options-callback

function getKey(header, callback) {
  jwksClient.getSigningKey(header.kid, (err, key) => {
    if (err) {
      return callback(err, undefined);
    }

    const signingKey = key.getPublicKey();
    return callback(null, signingKey);
  });
}

async function verifyToken(token, options = {}) {
  return new Promise((resolve, reject) => {
    jwt.verify(token, getKey.bind(this), options, (err, decoded) => {
      if (err) {
        return reject(err);
      }
      return resolve(decoded);
    });
  });
}

async function getJwtFromHttp(bearerToken) {
  return verifyToken(bearerToken);
}

function generateAuthObject(decodedJwt) {
  if (!decodedJwt) {
    return {
      selectedRole: null,
      sessionID: null,
      isMonAdmin: false,
      isMon: false,
      isRuo: false,
      isSchool: false,
      isTeacher: false,
      isLeadTeacher: false,
      isMunicipality: false,
      isHelpDesk: false,
      isBudgetingInstitution: false,
    };
  }
  return {
    selectedRole: decodedJwt.selected_role,
    sessionID: decodedJwt.sessionID,
    isMonAdmin: decodedJwt.selected_role.SysRoleID === RoleEnum.MON_ADMIN,
    isMon:
      decodedJwt.selected_role.SysRoleID === RoleEnum.MON_ADMIN ||
      decodedJwt.selected_role.SysRoleID === RoleEnum.CIOO ||
      decodedJwt.selected_role.SysRoleID === RoleEnum.MON_OBGUM ||
      decodedJwt.selected_role.SysRoleID === RoleEnum.MON_OBGUM_FINANCES ||
      decodedJwt.selected_role.SysRoleID === RoleEnum.MON_CHRAO ||
      decodedJwt.selected_role.SysRoleID === RoleEnum.MON_EXPERT,
    isRuo:
      decodedJwt.selected_role.SysRoleID === RoleEnum.RUO ||
      decodedJwt.selected_role.SysRoleID === RoleEnum.RUO_EXPERT,
    isSchool: decodedJwt.selected_role.SysRoleID === RoleEnum.INSTITUTION,
    isTeacher: decodedJwt.selected_role.SysRoleID === RoleEnum.TEACHER,
    isLeadTeacher:
      decodedJwt.selected_role.SysRoleID === RoleEnum.TEACHER &&
      decodedJwt.selected_role.IsLeadTeacher === true,
    isMunicipality:
      decodedJwt.selected_role.SysRoleID === RoleEnum.MUNICIPALITY,
    isHelpDesk:
      decodedJwt.selected_role.SysRoleID === RoleEnum.CONSORTIUM_HELPDESK,
    isBudgetingInstitution:
      decodedJwt.selected_role.SysRoleID === RoleEnum.BUDGETING_INSTITUTION,
  };
}

module.exports = { getJwtFromHttp, generateAuthObject };
