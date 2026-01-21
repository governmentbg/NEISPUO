<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row>
        <v-col
          cols="12"
        >
          <c-info
            uid="studentAdmissionDocumentPermissionRequest.requestingInstitution"
          >
            <v-text-field
              :value="requestingInstitution"
              :label="$t('studentAdmissionDocumentPermissionRequest.requestingInstitution')"
              disabled
              class="required"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
        >
          <c-info
            uid="studentAdmissionDocumentPermissionRequest.authorizingInstitution"
          >
            <v-text-field
              :value="authorizingInstitution"
              :label="$t('studentAdmissionDocumentPermissionRequest.authorizingInstitution')"
              disabled
              class="required"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
        >
          <c-info
            uid="studentAdmissionDocumentPermissionRequest.note"
          >
            <v-text-field
              v-model="model.note"
              :label="$t('studentAdmissionDocumentPermissionRequest.note')"
              clearable
            />
          </c-info>
        </v-col>
      </v-row>
      <file-manager
        v-if="!disabled || (model.documents && model.documents.length > 0)"
        v-model="model.documents"
        :disabled="disabled"
        class="mt-3"
      />
    </v-form>
  </div>
</template>

<script>
import { StudentAdmissionDocumentPermissionRequestModel } from "@/models/studentMovement/studentAdmissionDocumentPermissionRequestModel.js";
import FileManager from '@/components/common/FileManager.vue';

export default {
  name: "AdmissionDocument",
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
      default: false
    },
  },
  data() {
    return {
      model: new StudentAdmissionDocumentPermissionRequestModel(),
      requestingInstitution: null,
      authorizingInstitution: null
    };
  },
  watch: {
    document() {
      this.model = this.document ?? new StudentAdmissionDocumentPermissionRequestModel();

      this.loadRequestingInstitution(this.model.requestingInstitutionId);
      this.loadAuthorizingInstitution(this.model.authorizingInstitutionId);
    }
  },
  methods: {
    loadRequestingInstitution(institutionId) {
      this.$api.institution.getDropdownModelById(institutionId)
      .then((response) => {
        if(response.data) {
          this.requestingInstitution = response.data.details;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    },
    loadAuthorizingInstitution(institutionId) {
      this.$api.institution.getDropdownModelById(institutionId)
      .then((response) => {
        if(response.data) {
          this.authorizingInstitution = response.data.details;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    },
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
  }
};
</script>
