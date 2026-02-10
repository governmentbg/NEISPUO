<template>
  <div>
    <file-manager
      v-model="files"
    />
    <v-card-actions>
      <v-spacer />
      <v-btn
        ref="submit"
        raised
        color="primary"
        @click="onTestSave"
      >
        <v-icon left>
          fas fa-save
        </v-icon>          
        {{ $t('buttons.save') }}
      </v-btn>
    </v-card-actions> 
  </div>
</template>

<script>
import FileManager from '@/components/common/FileManager.vue';
import { DocumentModel } from '@/models/documentModel.js';

export default {
  name: 'FileManagerView',
  components: {
    FileManager
  },
  data() {
    return {
      files: []
    };
  },
  mounted() {
    this.$api.document.getTestDocuments()
      .then(response => {
        if(response.data) {
          response.data.forEach(el => {
            this.files.push(new DocumentModel(el));
          });
        }
      })
      .catch(error => {
        console.log(error);
      });
  },
  methods: {
    onTestSave() {
      this.$api.document.postTestDocuments(this.files)
      .then(response => {
        //Todo 
        console.log(response);
      })
      .catch(error => {
        console.log(error);
      });
    }
  }
};
</script>