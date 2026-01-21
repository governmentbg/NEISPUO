<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <!-- {{ document }} -->

      <form-layout
        :disabled="saving"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t('otherDocuments.editTitle') }}</h3>
        </template>

        <template #default>
          <other-document-form
            v-if="document !== null"
            :ref="'otherDocumentForm' + _uid"
            :document="document"
            :disabled="saving"
            :disabled-institution="isSchoolDirector"
            issue-date-required
            document-type-required
          />
        </template>
      </form-layout>
    </div>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import OtherDocumentForm from '@/components/tabs/otherDocuments/OtherDocumentForm.vue';
import { StudentOtherDocumentModel } from '@/models/studentOtherDocumentModel.js';
import { UserRole } from '@/enums/enums';
import { mapGetters } from 'vuex';
import { Permissions } from "@/enums/enums";

export default {
  name: 'StudentOtherDocumentEdit',
  components: {
    OtherDocumentForm,
  },
  props: {
    pid: {
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
      loading: true,
      saving: false,
      document: null,
      roleSchool: UserRole.School,
      currentSchoolYear: null
    };
  },
  computed: {
    ...mapGetters(['userSelectedRole', 'isInRole', 'hasStudentPermission', 'hasPermission']),
    isSchoolDirector() {
      return this.isInRole(this.roleSchool);
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherDocumentManage)
      && !this.hasPermission(Permissions.PermissionNameForStudentOtherDocumentManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    await this.init();
    this.load();
  },
  methods: {
    async init() {
      this.currentSchoolYear = (await this.$api.institution.getCurrentYear(this.userSelectedRole.InstitutionID)).data;
    },
    load() {
      this.loading = true;
      this.$api.otherDocument.getById(this.docId)
      .then(response => {
        if(response.data) {
          this.document = new StudentOtherDocumentModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('common.loadError', 5000));
        console.log(error.response);
      })
      .then(()=> { this.loading = false; });
    },
    onSave() {
      const form = this.$refs['otherDocumentForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.document.issueDate = this.$helper.parseDateToIso(this.document.issueDate, '');
      this.document.deliveryDate = this.$helper.parseDateToIso(this.document.deliveryDate, '');

      this.saving = true;
      this.$api.otherDocument
        .update(this.document)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), this.$t('errors.otherDocumentsSaveError'), 5000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
