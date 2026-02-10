<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('diplomas.createRequest.createTitle') }}</h3>
      </template>

      <template #default>
        <diploma-create-request-editor
          :ref="'diplomaCreateRequestEditorForm_' + _uid"
          v-model="model"
          :disabled="disabled"
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
import DiplomaCreateRequestEditor from '@/components/diplomas/DiplomaCreateRequestEditor';
import { DiplomaCreateRequestModel } from "@/models/diploma/diplomaCreateRequestModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'DiplomaCreateRequestCreateView',
  components: {
    DiplomaCreateRequestEditor
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    requestingInstitutionId: {
      type: Number,
      required: true
    },
    currentInstitutionId: {
      type: Number,
      required: false,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      saving: false,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    disabled() {
      return this.saving;
    }
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForDiplomaCreateRequestManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.model = new DiplomaCreateRequestModel({
      personId: this.personId,
      requestingInstitutionId: this.requestingInstitutionId,
      currentInstitutionId: this.currentInstitutionId }, this.$moment);
  },
  methods: {
    async onSave() {
      const form = this.$refs['diplomaCreateRequestEditorForm_' + this._uid];
      const isValid = form ? form.validate() : false;

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.model.registrationDate = this.$helper.parseDateToIso(this.model.registrationDate, '');

      this.saving = true;
      this.$api.diplomaCreateRequest
        .create(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.navigateToStudentDiplomaMenu();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'));
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
    },
    navigateToStudentDiplomaMenu() {
      this.$router.push({ name: 'StudentDiplomasList' , params: { id: this.personId }});
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
