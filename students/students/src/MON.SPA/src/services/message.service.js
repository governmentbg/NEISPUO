import http from './http.service';

class MessageService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/message";
  }

  getById(messageId) {
    if (!messageId) {
      throw new Error('MessageId is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById/?messageId=${messageId}`);
  }

  markRead(messageId) {
    if(!messageId) {
      throw new Error('MessageId is required');
    }

    return this.$http.put(
      `${this.baseUrl}/MarkAsRead/?messageId=${messageId}`);
  }

  archive(messageId) {
    if(!messageId) {
      throw new Error('MessageId is required');
    }

    return this.$http.put(`${this.baseUrl}/ArchiveMessage/?messageId=${messageId}`);
  }

  delete(messageId) {
    if(!messageId) {
      throw new Error('MessageId is required');
    }

    return this.$http.delete(`${this.baseUrl}/DeleteMessage/?messageId=${messageId}`);
  }

  deleteSelected(model) {
    if(!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/DeleteSelected`, model);
  }

  archiveSelected(model) {
    if(!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/ArchiveSelected`, model);
  }
}


export default new MessageService();
