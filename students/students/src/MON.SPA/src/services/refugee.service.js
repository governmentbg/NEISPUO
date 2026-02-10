import http from './http.service';

class RefugeeService {
    constructor() {
        this.$http = http;
        this.baseUrl = "/api/refugee";
    }

    getList() {
        return this.$http.get(`${this.baseUrl}/GetList`);
    }

    create(model) {
        if (!model) {
            throw new Error('Model is required!');
        }

        return this.$http.post(`${this.baseUrl}/Create`, model);
    }

    getById(id) {
        if (!id) {
            throw new Error('Id is required!');
        }

        return this.$http.get(`${this.baseUrl}/GetById/?id=${id}`);
    }

    getDetailsById(id) {
      if (!id) {
        throw new Error('Id is required!');
      }

      return this.$http.get(`${this.baseUrl}/GetDetailsById/?id=${id}`);
    }

    update(model){
        if(!model){
            throw new Error('Model is required!');
        }

        return this.$http.put(`${this.baseUrl}/Update`, model);
    }

    deleteApplication(id) {
      if(!id){
          throw new Error('Id is required!');
      }
      return this.$http.delete(`${this.baseUrl}/DeleteApplication?id=${id}`);
    }

    deleteApplicationChild(id) {
      if (!id) {
        throw new Error('Id is required!');
      }
      return this.$http.delete(`${this.baseUrl}/DeleteApplicationChild?id=${id}`);
    }

    cancelApplication(model) {
        if(!model){
            throw new Error('Model is required!');
        }
        return this.$http.put(`${this.baseUrl}/CancelApplication`, model);
    }

    cancelApplicationChild(model) {
        if(!model){
            throw new Error('Model is required!');
        }
        return this.$http.put(`${this.baseUrl}/CancelApplicationChild`, model);
    }

    completeApplication(id) {
      if(!id){
          throw new Error('Id is required!');
      }
      return this.$http.put(`${this.baseUrl}/CompleteApplication?id=${id}`);
    }

    completeApplicationChild(id) {
      if(!id){
          throw new Error('Id is required!');
      }
      return this.$http.put(`${this.baseUrl}/CompleteApplicationChild?id=${id}`);
    }

    unlockApplication(id) {
      if(!id){
          throw new Error('Id is required!');
      }
    return this.$http.put(`${this.baseUrl}/UnlockApplication?id=${id}`);
    }

    unlockApplicationChild(id) {
      if(!id){
          throw new Error('Id is required!');
      }
      return this.$http.put(`${this.baseUrl}/UnlockApplicationChild?id=${id}`);
    }

    countPendingAdmissions() {
      return this.$http.get(`${this.baseUrl}/CountPendingAdmissions`);
    }
}

export default new RefugeeService();
