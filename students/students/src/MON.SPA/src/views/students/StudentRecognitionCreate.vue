<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('recognition.createTitle') }}</h3>
      </template>

      <template #default>
        <recognition-form
          v-if="document !== null"
          :ref="'recognitionForm' + _uid"
          :document="document"
          :disabled="saving"
        />
      </template>
    </form-layout>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import RecognitionForm from '@/components/students/RecognitionForm.vue';
import { RecognitionModel } from '@/models/recognitionModel';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentRecognitionCreate',
  components: {
    RecognitionForm
  },
  props: {
    pid: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      saving: false,
      document: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userInstitutionId']),
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentRecognitionManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    await this.init();
  },
  methods: {
    async init() {
      this.document = new RecognitionModel({
        personId: this.pid,
        schoolYear: await this.getCurrentSchoolYear(this.userInstitutionId)
      }, this.$moment);
    },
    onSave() {
      const form = this.$refs['recognitionForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.document.ruoDocumentDate = this.$helper.parseDateToIso(this.document.ruoDocumentDate, '');
      this.document.diplomaDate = this.$helper.parseDateToIso(this.document.diplomaDate, '');

      this.saving = true;
      this.$api.recognition
        .create(this.document)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), error?.response?.data?.message ?? this.$t('common.error'), 7000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
    async getCurrentSchoolYear() {
      try {
        const currentSchoolYear = Number((await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data);
        if(!isNaN(currentSchoolYear)) {
          return currentSchoolYear;
        }
      } catch (error) {
        console.log(error);
      }
    }
  }
};
</script>
