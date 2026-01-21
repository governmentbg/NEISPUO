<template>
  <div>
    <form-layout
      :disabled="saving"
      :hide-save-btn="!model.items.some(x => !x.isSuspended)"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('additionalPersonalDevelopment.createTitle') }}</h3>
      </template>
      <template #default>
        <additional-personal-development-support-form
          v-if="model != null"
          :ref="'additionalPersonalDevelopmentSupportForm' + _uid"
          v-model="model"
          :disabled="saving"
          is-creation
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

export default {
  name: 'StudentAdditionalPersonalDevelopmentCreateView',
  components: {
    AdditionalPersonalDevelopmentSupportForm,
    ApiErrorDetails
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      saving: false,
      validationError: null,
      dialog: false,
      model: {
        personId: this.personId,
        schoolYear: null,
        items: [],
        orders: [],
        scorecards: [],
        plans: [],
        documents: [],
        sop: [],
      }
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userInstitutionId', 'userDetails']),
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentManage
      );
    },
  },
  async mounted() {
    if(!this.hasManagePermission) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.model.schoolYear = (await this.$api.institution.getCurrentYear(this.userInstitutionId)).data;
  },
  methods: {
    onSave() {
      const form = this.$refs['additionalPersonalDevelopmentSupportForm' + this._uid];
      const isValid = form.validate();

        if(!isValid) {
          return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
        }

        this.model.orderDate = this.$helper.parseDateToIso(this.model.orderDate, '');

        this.validationError = '';
        this.saving = true;
        this.$api.studentAdditionalPDS.create(this.model)
          .then(() => {
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
            this.onCancel();
          })
          .catch((error) => {
            const { message, errors } = this.$helper.parseError(error.response);
            this.validationError = { date: new Date(), ...error.response.data };
            this.$notifier.modalError(message, errors);
            this.$helper.logError({ action: "AdditionalPersonalDevelopmentCreate", message: message }, errors, this.userDetails);
          })
          .then(() => { this.saving = false; });
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
