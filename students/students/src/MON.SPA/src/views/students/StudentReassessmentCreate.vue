<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t("reassessment.title") }}</h3>
      </template>

      <template #default>
        <reassessment-form
          v-if="document"
          :ref="'reassessmentForm' + _uid"
          :document="document"
          :reason-id="reasonId"
          :disabled="saving"
        />
      </template>
    </form-layout>
  </div>
</template>
<script>
import ReassessmentForm from "@/components/students/ReassessmentForm.vue";
import { ReassessmentModel } from "@/models/reassessmentModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import Constants from "@/common/constants.js";

export default {
  name: "StudentReassessmentCreate",
  components: { ReassessmentForm },
  props: {
    pid: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      saving: false,
      document: null,
      reasonId: 0,
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission", "userInstitutionId"]),
  },
  async mounted() {
    if (
      !this.hasStudentPermission(
        Permissions.PermissionNameForStudentReassessmentManage
      )
    ) {
      return this.$router.push("/errors/AccessDenied");
    }
    await this.init();
  },
  methods: {
    async init() {
      this.document = new ReassessmentModel({
        personId: this.pid,
        schoolYear: await this.getCurrentSchoolYear(this.userInstitutionId)
      });
    },
    onSave() {
      const form = this.$refs["reassessmentForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.saving = true;

      if (this.document.reasonId !== Constants.ReassessmentEnableClassReasonId ) {
        this.document.InClass = null;
      }

      this.$api.reassessment
        .create(this.document)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(
            this.$t("common.save"),
            error?.response?.data?.message ?? this.$t("common.error"),
            7000
          );
          console.log(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    },
    async getCurrentSchoolYear() {
      try {
        const currentSchoolYear = Number((await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data);
        if (!isNaN(currentSchoolYear)) {
          return currentSchoolYear;
        }
      } catch (error) {
        console.log(error);
      }
    },
  },
};
</script>
