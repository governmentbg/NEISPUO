import http from './http.service';

class UserService {
    constructor() {
      this.$http = http;
    }
    
    getUserInfo(){
        return this.$http.get(`/api/authentication/getUserInfo`);
    }
}

export default new UserService();