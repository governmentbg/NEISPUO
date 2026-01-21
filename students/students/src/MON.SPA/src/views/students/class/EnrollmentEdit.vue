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
          {{ $t('enroll.titleEdit', { institution: loggedUserInstitutionName }) }}
        </template>

        <template #subtitle>
          {{ subtitle }}
        </template>

        <template #default>
          <enrollment-form
            v-if="document !== null"
            :ref="'enrollmentForm' + _uid"
            :document="document"
            :disabled="saving"
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
// Редакция на група/паралелка

import EnrollmentForm from '@/components/students/class/EnrollmentForm.vue';
import { StudentAdditionalClassChangeModel } from '@/models/studentClass/studentAdditionalClassChangeModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "EnrollmentEditView",
  components: {
    EnrollmentForm
  },
  props: {
    pid: {
      type: Number,
      required: true
    },
    studentClassId: {
      type: Number,
      required: false,
      default() {
        return undefined;
      }
    },
    classId: {
      type: Number,
      required: false,
      default() {
        return undefined;
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
    ...mapGetters(['userDetails', 'hasStudentPermission', 'selectedStudentClass']),
    loggedUserInstitutionName() {
      return this.userDetails?.institution;
    },
    subtitle() {
      return this.selectedStudentClass ? `${this.selectedStudentClass.classGroup.className} - ${this.selectedStudentClass.classGroup.classTypeName}` : '';
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentToClassEnrollment)) {
      return this.$router.push('/errors/AccessDenied');
    }

    const studentClass = (await this.$api.studentClass.getById(this.studentClassId))?.data;
    this.document = new StudentAdditionalClassChangeModel({
      personId: studentClass.personId,
      currentStudentClassId: studentClass.classId,
      classId: studentClass.classId,
      id: studentClass.id,
      enrollmentDate: studentClass.enrollmentDate,
      dischargeDate: studentClass.dischargeDate,
      positionId: studentClass.positionId,
      profession: studentClass.studentProfession,
      eduForm: studentClass.studentEduFormName,
      classGroup: studentClass.classGroup,
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

      this.saving = true;
      this.$api.studentClass
        .updateAdditionalClass(this.document)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.onCancel();
        })
        .catch((error) => {
          console.log(error.response);
          this.$notifier.error(this.$t("common.save"),this.$t("common.saveError"), 5000);
        })
        .then(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
