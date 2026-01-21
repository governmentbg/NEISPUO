import http from './http.service';

class AdministrationService {
    constructor() {
      this.$http = http;
      this.baseUrl = "/api/administration";
    }

    getContextualInformation() {
      return this.$http.get(`${this.baseUrl}/ContextualInformation`);
    }

    getContextualInformationByKey(key) {
      return this.$http.get(`${this.baseUrl}/GetContextualInformationByKey?key=${key}`);
    }

    updateContextualInformation(model){
      if (!model) {
        throw new Error('Model is required');
      }

      return this.$http.put(`${this.baseUrl}/UpdateContextualInformation`, model);
    }

    updatePermissionDocumentation(model){
      if (!model) {
        throw new Error('Model is required');
      }

      return this.$http.put(`${this.baseUrl}/UpdatePermissionDocumentation`, model);
    }

    getTenantAppSetting(key) {
      if (!key) {
        throw new Error('Key is required');
      }

      return this.$http.get(`${this.baseUrl}/GetTenantAppSetting?key=${key}`);
    }

    setTenantAppSetting(model) {
      if (!model) {
        throw new Error('Model is required');
      }

      return this.$http.post(`${this.baseUrl}/SetTenantAppSetting`, model);
    }

    getCacheKeys() {
      return this.$http.get(`${this.baseUrl}/GetCacheKeys`);
    }


    getCacheServerInfo() {
      return this.$http.get(`${this.baseUrl}/GetCacheServerInfo`);
    }

    getCacheKeyValue(cacheKey) {
      return this.$http.get(`${this.baseUrl}/GetCacheKeyValue?cacheKey=${cacheKey}`);
    }

    getCacheKeyFull(cacheKey) {
      return this.$http.get(`${this.baseUrl}/GetCacheKeyFull?cacheKey=${cacheKey}`);
    }

    clearCache() {
      return this.$http.post(`${this.baseUrl}/ClearCache`);
    }

    clearCacheKey(cacheKey) {
      return this.$http.post(`${this.baseUrl}/ClearCacheKey?cacheKey=${cacheKey}`);
    }

    getDataReferences(schemaName, tableName, entityId, top, onlyWithDependecies) {
      if (!schemaName) {
        throw 'SchemaName argument is required.';
      }

      if (!tableName) {
        throw 'TableName argument is required.';
      }

      if (!entityId) {
        throw 'EntityId argument is required.';
      }

      let query = `${this.baseUrl}/GetDataReferences?schemaName=${schemaName}&tableName=${tableName}&entityId=${entityId}`;
      if (top) {
        query += `&top=${top}`;
      }

      if (onlyWithDependecies) {
        query += `&onlyWithDependecies=${onlyWithDependecies}`;
      }

      return this.$http.get(query);
    }

    finalizeLods(model) {
      if (!model) {
        throw new Error('Model is required');
      }

      return this.$http.post(`${this.baseUrl}/FinalizeLods`, model);
    }
}

export default new AdministrationService();
