import { HttpTransportType, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

import { config } from '@/common/config';

export default {
  install(Vue) {
    // use a new Vue instance as the interface for Vue components to receive/send SignalR events
    // this way every component can listen to events or send new events using this.$questionHub
    const studentHub = new Vue();
    Vue.prototype.$studentHub = studentHub;

    // Provide methods to connect/disconnect from the SignalR hub
    let connection = null;
    let startedPromise = null;
    let manuallyClosed = false;

    const hubUrl = `${config.apiBaseUrl}student-hub`;
    Vue.prototype.startStudentSignalR = (jwtToken) => {

      connection = new HubConnectionBuilder()
        .withUrl(hubUrl, jwtToken ?
          {
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets,
            accessTokenFactory: () => jwtToken
          } : {
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets
          })
        .configureLogging(LogLevel.Error)
        .build();

      // Forward hub events through the event, so we can listen for them in the Vue components
      // connection.on('SendMessage', (msg) => {
      //   studentHub.$emit('message-received', { msg });
      // });

      connection.on('SendMessage', (msg) => {
        studentHub.$emit('change-person-messages', { msg });
      });

      connection.on('StudentFinalizedLodsReloaded', (finalizedLods) => {
        studentHub.$emit('student-finalized-lods-reloaded', finalizedLods);
      });

      connection.on('ContextualInformationReloaded', (contextualInformation) => {
        studentHub.$emit('contextual-information-reloaded', contextualInformation);
      });

      connection.on('AbsenceCampaignModified', (id) => {
        studentHub.$emit('absence-campaign-modified', id);
      });

      connection.on('AspCampaignModified', (id) => {
        studentHub.$emit('asp-campaign-modified', id);
      });

      connection.on('DocManagementCampaignModified', (id,) => {
        studentHub.$emit('doc-management-campaign-modified', id);
      });

      connection.on('DocManagementAdditionalCampaignModified', (id, parentId) => {
        studentHub.$emit('doc-management-additional-campaign-modified', id, parentId);
      });

      connection.on('ResourceSupportModified', (id) => {
        studentHub.$emit('resource-support-modified', id);
      });

      connection.on('PersonalDevelopmentModified', (id) => {
        studentHub.$emit('personal-development-modified', id);
      });

      connection.on('RefugeeEnrolledInInstitution', (personId, institutionId, regionId) => {
        studentHub.$emit('refugee-enrolled-in-institution', personId, institutionId, regionId);
      });

      connection.on('StudentClassEduStateChange', (personId) => {
        studentHub.$emit('student-class-edu-state-change', personId);
      });

      // You need to call connection.start() to establish the connection but the client wont handle reconnecting for you!
      // Docs recommend listening onclose and handling it there.
      // This is the simplest of the strategies
      function start() {
        startedPromise = connection.start()
          .then(() => {
            console.log('student-hub started');
          })
          .catch(err => {
            console.error('Failed to connect with hub', err);
            return new Promise((resolve, reject) => setTimeout(() => start().then(resolve).catch(reject), 20000));
          });
        return startedPromise;
      }
      connection.onclose(() => {
        if (!manuallyClosed) {
          start();
        }
      });

      // Start everything
      manuallyClosed = false;
      start();
    };

    Vue.prototype.stopStudentSignalR = () => {
      if (!startedPromise) {
        return;
      }

      manuallyClosed = true;
      return startedPromise
        .then(() => connection.stop())
        .then(() => { startedPromise = null; });
    };

    // Provide methods for components to send messages back to server
    // Make sure no invocation happens until the connection is established
    studentHub.joinStudentGroup = (personId) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke('JoinStudentGroup', personId))
        .catch(console.error);
    };

    studentHub.leaveStudentGroup = (personId) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke('LeaveStudentGroup', personId))
        .catch(console.error);
    };



    //tasksHub.joinToHub = () => {
    //  if (!startedPromise) return

    //  return startedPromise
    //    .then(() => connection.invoke('Join'))
    //    .catch(console.error)
    //}
  }
};
