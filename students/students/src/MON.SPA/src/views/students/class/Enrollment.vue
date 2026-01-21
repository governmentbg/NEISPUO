<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <form-layout
        :disabled="saving"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t('enroll.title', { institution: loggedUserInstitutionName }) }}</h3>
        </template>

        <template #default>
          <div
            v-if="document !== null"
          >
            <cplr-enrollment-form
              v-if="isCplrEnrollment"
              :ref="'enrollmentForm' + _uid"
              :document="document"
              :disabled="saving"
            />
            <enrollment-form
              v-else
              :ref="'enrollmentForm' + _uid"
              :document="document"
              :disabled="saving"
            />
          </div>
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
// Запис в допълнителна група/паралелка (не през документ за записване). Бутон в ученическите профилни данни.

import EnrollmentForm from '@/components/students/class/EnrollmentForm.vue';
import CplrEnrollmentForm from '@/components/students/class/CplrEnrollmentForm.vue';
import { StudentClassBaseModel } from '@/models/studentClass/studentClassBaseModel';
import { StudentClass } from '@/models/studentClass/studentClass';
import { Permissions, InstType } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "EnrollmentView",
  components: {
    EnrollmentForm,
    CplrEnrollmentForm
  },
  props: {
    pid: {
      type: Number,
      required: true
    },
    isCplrEnrollment: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      loading: false,
      saving: false,
      document: null,
    };
  },
  computed: {
    ...mapGetters(['userDetails', 'hasStudentPermission', 'isInstType']),
    loggedUserInstitutionName() {
      return this.userDetails?.institution;
    },
    isCplrInstitution() {
      return this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ);
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentToClassEnrollment)) {
      return this.$router.push('/errors/AccessDenied');
    }

    if(this.isCplrInstitution !== this.isCplrEnrollment) {
      this.$notifier.error('', this.$t('errors.authorizeError'), 5000);

      return this.$router.go(-1);
    }

    this.document = this.isCplrEnrollment
      ? new StudentClass({ personId: this.pid })
      : new StudentClassBaseModel({
        personId: this.pid,
        enrollmentDate: new Date(),
      }, this.$moment);
  },
  methods: {
    onSave() {
      const form = this.$refs["enrollmentForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.document.enrollmentDate = this.$helper.parseDateToIso(this.document.enrollmentDate, '');
      this.document.dischargeDate = this.$helper.parseDateToIso(this.document.dischargeDate, '');

      if(this.isCplrEnrollment) {
        this.saving = true;
        this.$api.studentClass
          .еnrollInCplrAdditionalClass(this.document)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.pid);
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
            this.onCancel();
          })
          .catch((error) => {
            this.$notifier.error(this.$t("common.save"), error.response?.data?.message ?? this.$t("common.saveError"), 5000);
            console.log(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      } else {
        this.saving = true;
        this.$api.studentClass
          .enrollInAdditionalClass(this.document)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.pid);
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
            this.onCancel();
          })
          .catch((error) => {
            this.$notifier.error(this.$t("common.save"), error.response?.data?.message ?? this.$t("common.saveError"), 5000);
            console.log(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }

    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>
