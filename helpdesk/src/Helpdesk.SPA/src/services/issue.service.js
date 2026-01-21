import http from './http.service';

class IssueService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/issue";
  }

  getById(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  create(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/Update`, model);
  }

  resolve(model) {
    if (!model) {
      throw new Error('model is required');
    }

    return this.$http.put(`${this.baseUrl}/Resolve`, model);
  }

  reopen(model) {
    if (!model) {
      throw new Error('model is required');
    }

    return this.$http.put(`${this.baseUrl}/Reopen`, model);
  }

  assignToMyself(issueId) {
    if (!issueId) {
      throw new Error('issueId is required');
    }

    return this.$http.put(`${this.baseUrl}/AssignToMyself`, { issueId: issueId});
  }

  assignTo(issueId, userId) {
    if (!issueId) {
      throw new Error('issueId is required');
    }

    return this.$http.put(`${this.baseUrl}/AssignTo`, { issueId: issueId, userId: userId });
  }

  comment(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.put(`${this.baseUrl}/Comment`, model);
  }

  logIssueReadActivity(id) {
    return this.$http.post(`${this.baseUrl}/LogReadActivity?id=${id}`);
  }

}

export default new IssueService();
