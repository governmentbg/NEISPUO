import http from './http.service';

class DynamicFormService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/dynamicForm";
  }

  getJsonSchema() {
    return this.$http.get(`${this.baseUrl}/GetJsonSchema`);
  }

  getEntitiesJsonDescription() {
    return this.$http.get(`${this.baseUrl}/GetEntitiesJsonDescription`);
  }

  getEntityTypesDropdowns() {
    return this.$http.get(`${this.baseUrl}/GetEntityTypesDropdowns`);
  }

  getGridHeaders(entityTypeName) {
    return this.$http.get(`${this.baseUrl}/GetGridHeaders?entityTypeName=${entityTypeName}`);
  }

  getList(query) {
    return this.$http.get(`${this.baseUrl}/List${query}`, undefined, false);
  }

  getEntityModel(entityTypeName, entityId) {
    if(!entityTypeName) throw 'EntityTypeName is required!';
    if(!entityId) throw 'EntityId is required!';
    return this.$http.get(`${this.baseUrl}/getEntityModel?entityTypeName=${entityTypeName}&entityId=${entityId}`);
  }

  create(model) {
    if(!model) throw new Error("Model is required!");

    return this.$http.post(`${this.baseUrl}/create`, model);
  }

  update(model) {
    if(!model) throw new Error("Model is required!");

    return this.$http.put(`${this.baseUrl}/update`, model);
  }

  delete(model) {
    if(!model) throw new Error("Model is required!");

    // Използването на post за триене на е грешка. Трябва да подам body, а не се стравих с axios и 415 Status code.
    return this.$http.post(`${this.baseUrl}/delete`, model);
  }
}

export default new DynamicFormService();