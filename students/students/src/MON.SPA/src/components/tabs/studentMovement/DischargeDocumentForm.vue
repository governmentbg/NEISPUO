<template>
  <v-form
    v-if="model"
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <!-- {{ model }} -->
    <v-row
      dense
    >
      <v-col
        cols="12"
        md="3"
        sm="6"
      >
        <c-info
          uid="dischargeDocument.noteNumber"
        >
          <template>
            <v-text-field
              v-model="model.noteNumber"
              :label="$t('documents.noteNumberLabel')"
              :rules="[$validator.required(), $validator.maxLength(20)]"
              clearable
              :class="!disabled ? 'required' : ''"
            />
          </template>
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="3"
        sm="6"
      >
        <c-info
          uid="dischargeDocument.noteDate"
        >
          <date-picker
            ref="noteDate"
            v-model="model.noteDate"
            :show-buttons="false"
            :scrollable="false"
            :no-title="true"
            :show-debug-data="false"
            :label="$t('documents.noteDate')"
            :rules="[$validator.required()]"
            :class="!disabled ? 'required' : ''"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="3"
        sm="6"
      >
        <c-info
          uid="dischargeDocument.dischargeDate"
        >
          <date-picker
            ref="dischargeDate"
            v-model="model.dischargeDate"
            :show-buttons="false"
            :scrollable="false"
            :no-title="true"
            :show-debug-data="false"
            :label="$t('documents.dischargeDateLabel')"
            :rules="[$validator.required()]"
            :class="!disabled ? 'required' : ''"
          />
        </c-info>
      </v-col>
      <v-col
        v-if="!hideStatus"
        cols="12"
        md="3"
        sm="6"
      >
        <c-info
          uid="dischargeDocument.status"
        >
          <v-select
            id="dischargeDocumentStatus"
            v-model="model.status"
            :items="statusOptions"
            :label="$t('documents.statusLabel')"
            :rules="[$validator.required()]"
            :class="!disabled ? 'required' : ''"
            :disabled="editConfirmedDocument"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="3"
        sm="6"
      >
        <c-info
          uid="dischargeDocument.reason"
        >
          <v-select
            ref="dischargeReasonDropdown"
            v-model="model.dischargeReasonTypeId"
            :items="dischargeReasonOptions"
            :label="$t('documents.dischargeReasonTypeLabel')"
            :rules="[$validator.required()]"
            clearable
            :class="!disabled ? 'required' : ''"
          />
        </c-info>
      </v-col>
    </v-row>
    <v-row
      v-if="!hideFiles"
    >
      <v-col
        cols="12"
      >
        <file-manager
          v-model="model.documents"
          :disabled="disabled"
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>

import DocumentStatuses from '@/common/documentStatuses.js';
import FileManager from '@/components/common/FileManager.vue';
import { NewStudentDischargeDocumentModel } from '@/models/studentMovement/newStudentDischargeDocumentModel.js';

export default {
  name: 'DischargeDocument',
  components: {
    FileManager
  },
  props: {
    document: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hideFiles: {
      type: Boolean,
      default: false
    },
    hideStatus: {
      type: Boolean,
      default: false
    },
    // При редакция да потвърден документ не е позволено да се редактират
    // полетата Институция, Позиция и Статус
    editConfirmedDocument: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: new NewStudentDischargeDocumentModel({status: 1}),
      dischargeReasonOptions: [],
      statusOptions: DocumentStatuses,
    };
  },
  watch: {
    document() {
      this.model = this.document ?? new NewStudentDischargeDocumentModel({status: 1});
    }
  },
  beforeMount() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getDischargeReasonTypeOptions({ isForDischarge: true })
      .then((response) => {
        if(response.data) {
          this.dischargeReasonOptions = response.data;
        }
      })
      .catch((error) => {
        console.log(error);
      });
    },
    deleteFile(index) {
      this.model.previousFiles.splice(index, 1);
      this.model.documents = this.model.previousFiles;
    },
    filesInputChange(value) {
      if(event.type === 'change') {
        this.model.documents = this.model.previousFiles.concat(value);
        this.model.previousFiles = this.model.documents;
      }
    },
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    }
  }
};
</script>
