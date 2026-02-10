/* eslint-disable import/no-extraneous-dependencies */
const winston = require('winston');
const udpWinston = require('udp-transport-winston');

class SIEMLogger {
  constructor() {
    if (process.env?.SIEM_ENABLED === 'true') {
      try {
        this.logger = winston.createLogger({
          format: winston.format.printf(({ message }) => {
            if (typeof message === 'object') {
              return JSON.stringify(message);
            }
            return message;
          }),
          transports: [
            new udpWinston.UDPTransport({
              host: process.env.SIEM_HOST,
              port: process.env.SIEM_PORT,
            }),
          ],
        });
        console.log('SIEM integration started successfully.');
      } catch (error) {
        console.error('Error initializing SIEM Logger:', error);
        this.logger = null;
      }
    } else {
      console.log(`SIEM integration disabled.`);
    }
  }

  sendLoginSuccessLog(auditLogin) {
    if (this.logger) {
      this.logger.info(this.buildLoginSuccessLog(auditLogin));
    }
  }

  buildLoginSuccessLog(auditRecord) {
    return {
      timestamp: new Date().toISOString(),
      appid: process.env.SIEM_APP_ID,
      source_ip: auditRecord?.IPSource || 'unknown',
      event_type: 'auth_login_success',
      user: {
        username: auditRecord?.Username,
        role: auditRecord?.SysRoleName,
        role_id: auditRecord?.SysRoleID,
        user_id: auditRecord?.SysUserID,
      },
      institution: {
        id: auditRecord?.InstitutionID,
        name: auditRecord?.InstitutionName,
      },
      location: {
        region: {
          id: auditRecord?.RegionID,
          name: auditRecord?.RegionName,
        },
        municipality: {
          id: auditRecord?.MunicipalityID,
          name: auditRecord?.MunicipalityName,
        },
      },
      audit: {
        login_audit_id: auditRecord?.LoginAuditID,
        status: 'success',
        description: `User ${auditRecord?.Username} login successfully`,
      },
      metadata: {
        level: 'INFO',
        event: `authn_login_success:${auditRecord?.Username}`,
      },
    };
  }
}

module.exports = new SIEMLogger();
