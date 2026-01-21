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
          <h3>{{ $t('enroll.titleEdit', { institution: loggedUserInstitutionName }) }}</h3>
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
// Местене от една в друга група паралелка в рамките на инстируцията

import EnrollmentForm from '@/components/students/class/EnrollmentForm.vue';
import { StudentAdditionalClassChangeModel } from '@/models/studentClass/studentAdditionalClassChangeModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "EnrollmentChangeView",
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
      document: null
    };
  },
  computed: {
    ...mapGetters(['userDetails', 'hasStudentPermission', 'selectedStudentClass']),
    loggedUserInstitutionName() {
      return this.userDetails?.institution;
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentPermissionNameForStudentToClassEnrollmentToClassEntrollment)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.document = new StudentAdditionalClassChangeModel({
      personId: this.pid ,
      currentStudentClassId: this.classId ?? this.selectedStudentClass?.classId,
      classId: this.classId ?? this.selectedStudentClass?.classId,
      id: this.studentClassId ?? this.selectedStudentClass?.id
    });
  },
  methods: {
    onSave() {
        const form = this.$refs["enrollmentForm" + this._uid];
        const isValid = form.validate();

        if (!isValid) {
          return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
        }

      this.saving = true;
      this.$api.studentClass
        .changeAdditionalClass(this.document)
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
    },
  }
};
</script>
