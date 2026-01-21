<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <form-layout>
      <template #title>
        <h3>{{ $t('documents.relocationDocumentViewTitle') }}</h3>
      </template>

      <template #default>
        <relocation-document-form
          :ref="'admissionDocumentEdit' + _uid"
          :person-id="personId"
          :document="document"
          disabled
        >
          <template #hostingInstitution>
            <v-text-field
              v-if="document"
              :value="document.institutionName"
              :label="$t('documents.recipientInstitution')"
              disabled
            />
          </template>
        </relocation-document-form>
        <admission-documents-for-relocation
          v-if="document"
          :doc-id="document.id"
          class="mt-4"
        />
      </template>

      <template #actions>
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="backClick"
        >
          <v-icon left>
            fas fa-chevron-left
          </v-icon>
          {{ $t('buttons.back') }}
        </v-btn>
      </template>
    </form-layout>
  </div>
</template>

<script>
import RelocationDocumentForm from '@/components/tabs/studentMovement/RelocationDocumentForm';
import AdmissionDocumentsForRelocation from '@/components/tabs/studentMovement/AdmissionDocumentsForRelocation';
import { StudentRelocationDocumentModel } from "@/models/studentMovement/studentRelocationDocumentModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentRelocationDocumentDetails',
  components: {
    RelocationDocumentForm,
    AdmissionDocumentsForRelocation
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    docId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: false,
      document: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.relocationDocument
        .getById(this.docId)
        .then((response) => {
          if(response.data) {
            this.document = new StudentRelocationDocumentModel(response.data, this.$moment);
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('documents.documentLoadErrorMessage', 5000));
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
