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
        <h3>{{ $t('studentScholarships.editTitle') }}</h3>
        <v-spacer />
        <v-chip
          v-if="form"
          color="light"
        >
          {{ form.institutionName }}
        </v-chip>
      </template>

      <template #default>
        <scholarship-form
          :ref="'scholarshipForm' + _uid"
          :document="form"
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
import ScholarshipForm from "@/components/tabs/studentScholarships/ScholarshipForm";
import { StudentScholarshipModel } from "@/models/studentScholarship/studentScholarshipModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "ScholarshipEdit",
  components: {
    ScholarshipForm,
  },
  props: {
    personId: {
      type: Number,
      required: false,
      default: 0,
    },
    scholarshipId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: false,
      saving: false,
      form: null,
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission"]),
    disabled() {
      return this.saving;
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentScholarshipManage)) {
      return this.$router.push("/errors/AccessDenied");
    }

    this.loadData();
  },
  methods: {
    loadData() {
      this.loading = true;
      this.$api.scholarship
        .getById(this.scholarshipId)
        .then((response) => {
          if (response.data) {
            this.form = new StudentScholarshipModel(response.data, this.$moment);
          }
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.studentAwardsLoad"));
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    onSave() {
      const form = this.$refs['scholarshipForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.form.orderDate = this.$helper.parseDateToIso(this.form.orderDate, '');
      this.form.commissionDate = this.$helper.parseDateToIso(this.form.commissionDate, '');
      this.form.startingDateOfReceiving = this.$helper.parseDateToIso(this.form.startingDateOfReceiving, '');
      this.form.endDateOfReceiving = this.$helper.parseDateToIso(this.form.endDateOfReceiving, '');

      this.saving = true;
      this.$api.scholarship
        .update(this.form)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"));
          this.onCancel();
        })
        .catch((error) => {
           this.$notifier.error(this.$t('common.save'), this.$t('common.error'), 5000);
          console.log(error.response);
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
