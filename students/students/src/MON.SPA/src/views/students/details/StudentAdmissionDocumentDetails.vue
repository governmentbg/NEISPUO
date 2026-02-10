<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <form-layout>
      <template #title>
        <h3>{{ $t('admissionDocument.reviewTitle') }}</h3>
      </template>

      <template #default>
        <admission-document-form
          :ref="'admissionDocumentEdit' + _uid"
          :person-id="personId"
          :document="document"
          is-details-view
          disabled
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
import AdmissionDocumentForm from '@/components/tabs/studentMovement/AdmissionDocumentForm';
import { StudentAdmissionDocumentModel } from "@/models/studentMovement/studentAdmissionDocumentModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentAdmissionDocumentEdit',
  components: {
    AdmissionDocumentForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    docId: {
      type: Number,
      required: true
    }
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
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
    
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.admissionDocument
        .getById(this.docId)
        .then((response) => {
          if(response.data) {
            this.document = new StudentAdmissionDocumentModel(response.data, this.$moment);
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('documents.documentLoadErrorMessage'));
          console.log(error.response.data.message);
        })
        .then(() => { this.loading = false; });
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>