<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <form-layout
      v-else
      :disabled="saving"
      :hide-save-btn="!model.items.some(x => !x.isSuspended)"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('additionalPersonalDevelopment.editTitle') }}</h3>
      </template>
      <template #default>
        <additional-personal-development-support-form
          v-if="model != null"
          :ref="'additionalPersonalDevelopmentSupportForm' + _uid"
          v-model="model"
          :disabled="saving"
          is-edit
          @suspended="load"
        />
      </template>
      <template #left-actions>
        <v-btn
          v-if="validationError"
          color="red"
          outlined
          @click="dialog = true"
        >
          <v-icon
            large
            class="mr-2"
          >
            mdi-alert-circle-outline
          </v-icon>
          Виж детайли на последната грешка
        </v-btn>
        <v-alert
          v-if="!model.items.some(x => !x.isSuspended)"
          border="left"
          colored-border
          type="warning"
          elevation="2"
        >
          За да можете да запишете данните, трябва да посочите поне една дейност за ДПЛР в раздел "ДЕЙНОСТИ ЗА ДПЛР" на този екран.
        </v-alert>
      </template>
    </form-layout>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-toolbar
        color="red"
        outlined
      >
        <v-btn
          icon
          dark
          @click="dialog = false"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-spacer />
        <v-toolbar-items>
          <button-tip
            icon
            icon-name="fa-copy"
            tooltip="buttons.copy"
            bottom
            iclass=""
            small
            @click="copyError()"
          />
        </v-toolbar-items>
      </v-toolbar>

      <api-error-details :value="validationError" />
    </v-dialog>
  </div>
</template>

<script>
import AdditionalPersonalDevelopmentSupportForm from '@/components/students/AdditionalPersonalDevelopmentSupportForm.vue';
import ApiErrorDetails from "@/components/admin/ApiErrorDetails.vue";
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';
import Constants from '@/common/constants.js';

export default {
  name: 'StudentAdditionalPersonalDevelopmentEditView',
  components: {
    AdditionalPersonalDevelopmentSupportForm,
    ApiErrorDetails
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    id: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      saving: false,
      validationError: null,
      dialog: false,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentManage
      );
    },
  },
  mounted() {
    if(!this.hasManagePermission) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentAdditionalPDS.getById(this.id)
        .then((response) => {
          if (response.data) {
            this.model = response.data;
            this.model.orderDate = this.model.orderDate ? this.$moment(this.model.orderDate).format(Constants.DATEPICKER_FORMAT) : this.model.orderDate;
          }
        })
        .catch((error) => {
          this.$notifier.success('', this.$t('common.loadError'), 5000);
          console.log(error.response);
        })
        .finally(() => {
          this.loading = false;
        });
    },
    async onSave() {
      const form = this.$refs['additionalPersonalDevelopmentSupportForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.model.orderDate = this.$helper.parseDateToIso(this.model.orderDate, '');

      this.validationError = '';
      this.saving = true;
      this.$api.studentAdditionalPDS.update(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          const { message, errors } = this.$helper.parseError(error.response);
            this.validationError = { date: new Date(), ...error.response.data };
            this.$notifier.modalError(message, errors);
            this.$helper.logError({ action: "AdditionalPersonalDevelopmentEdit", message: message }, errors, this.userDetails);
        })
        .finally(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
    copyError() {
      let vm = this;
      navigator.clipboard.writeText(JSON.stringify(this.validationError)).then(
        function () {
          vm.$notifier.success("", vm.$t("common.clipboardSuccess"));
        },
        function () {
          vm.$notifier.error("", vm.$t("common.clipboardError"));
        }
      );
    },
  }
};
</script>
