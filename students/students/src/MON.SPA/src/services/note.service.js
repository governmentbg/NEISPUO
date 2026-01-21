import http from './http.service';

class NoteService {
    constructor() {
        this.$http = http;
        this.baseUrl = "/api/note";
    }

    getListForPerson(personId) {
        if (!personId) {
            throw new Error('PersonId is required!');
        }

        return this.$http.get(`${this.baseUrl}/GetStudentNotes/?id=${personId}`);
    }
    create(model) {
        if (!model) {
            throw new Error('Note model is required!');
        }

        return this.$http.post(`${this.baseUrl}/Create`, model);
    }
    getById(id) {
        if (!id) {
            throw new Error('Note Id is required!');
        }

        return this.$http.get(`${this.baseUrl}/GetById/?id=${id}`);
    }
    update(model){
        if(!model){
            throw new Error('Note model is required!');
        }

        return this.$http.put(`${this.baseUrl}/Update`, model);
    }
    delete(id) {
        if(!id){
            throw new Error('Note Id is required!');
        }
        return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
      }
}

export default new NoteService();
