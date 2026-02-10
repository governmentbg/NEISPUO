<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t("equalization.createTitle") }}</h3>
      </template>

      <template #default>
        <equalization-form
          v-if="document"
          :ref="'equalizationForm' + _uid"
          :document="document"
          :disabled="saving"
        />
      </template>
    </form-layout>
  </div>
</template>

<script>
import EqualizationForm from "@/components/students/EqualizationForm.vue";
import { EqualizationModel } from "@/models/equalizationModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import Constants from "@/common/constants.js";

export default {
  name: "StudentEqualizationCreate",
  components: {
    EqualizationForm,
  },
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
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userInstitutionId']),
  },
  async mounted() {
    if (
      !this.hasStudentPermission(
        Permissions.PermissionNameForStudentEqualizationManage
      )
    ) {
      return this.$router.push("/errors/AccessDenied");
    }
    await this.init();
  },
  methods: {
    async init() {
      this.document = new EqualizationModel({
        personId: this.pid,
        schoolYear: await this.getCurrentSchoolYear(this.userInstitutionId)
      });
    },
    onSave() {
      const form = this.$refs["equalizationForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.saving = true;

      if (this.document.reasonId !== Constants.EqualizationEnableClassReasonId) {
        // InClassId се въвежда само при избрана причина "преместване на ученика от VIII до XII клас вкл. (чл. 32, ал. 1, т. 1 от Наредба №11"
        this.document.InClass = null;
      }

      this.$api.equalization
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
        if(!isNaN(currentSchoolYear)) {
          return currentSchoolYear;
        }
      } catch (error) {
        console.log(error);
      }
    }
  },
};
</script>
