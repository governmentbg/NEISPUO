
import localServer from '../services/localServer.service';
import helper from '../components/helper';

export async function checkForLocalServer() {
  let hasLocalServerError = false;
  await localServer
    .version()
    .then(() => {
      hasLocalServerError = false;
    })
    .catch(() => {
      hasLocalServerError = true;
    });

  return hasLocalServerError;
}


export async function requireMinLocalServerVersion(minVersion) {
      let needUpgrade = false;
      let currentVersion = null;
      let requestedVersion = helper.getVersion(minVersion);
      await localServer
        .version()
        .then((version) => {
          currentVersion = helper.getVersion(version.data);
          if (currentVersion != null && requestedVersion && currentVersion < requestedVersion) {
            needUpgrade = true;
          }
        })
        .catch(() => {
          needUpgrade = true;
        });

      return needUpgrade;
}
