<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('diplomas.createRequest.editTitle') }}</h3>
      </template>

      <template #default>
        <diploma-create-request-editor
          :ref="'diplomaCreateRequestEditorForm_' + _uid"
          :value="model"
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
  name: 'DiplomaCreateRequestEditView',
  components: {
    DiplomaCreateRequestEditor
  },
  props: {
    id: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: false,
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

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.diplomaCreateRequest.getById(this.id)
        .then((response) => {
          if(response.data) {
            this.model = new DiplomaCreateRequestModel(response.data, this.$moment);
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('documents.documentLoadErrorMessage'), 5000);
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    async onSave() {
      const form = this.$refs['diplomaCreateRequestEditorForm_' + this._uid];
      const isValid = form ? form.validate() : false;

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.model.registrationDate = this.$helper.parseDateToIso(this.model.registrationDate, '');

      this.saving = true;
      this.$api.diplomaCreateRequest
        .update(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'));
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
