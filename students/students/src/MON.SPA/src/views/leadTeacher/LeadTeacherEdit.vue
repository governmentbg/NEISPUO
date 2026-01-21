<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <form-layout
      v-else
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t("leadTeacher.editTitle") }} </h3>
      </template>

      <template #default>
        <lead-teacher-form
          v-if="document"
          :ref="'leadTeacherForm' + _uid"
          :document="document"
          :disabled="saving"
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
import LeadTeacherForm from "@/components/leadTeacher/LeadTeacherForm.vue";
import { LeadTeacherModel } from "@/models/leadTeacher/leadTeacherModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'LeadTeacherEditView',
  components: {
    LeadTeacherForm,
  },
  props: {
    classId: {
      type: Number,
      required: true
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
    ...mapGetters(['hasPermission', 'userDetails']),
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForLeadTeacherManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.leadTeacher.getByClassId(this.classId)
      .then(response => {
        if(response.data) {
          this.document = new LeadTeacherModel(response.data);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('documents.documentLoadErrorMessage', 5000));
        console.log(error);
      })
      .then(() => { this.loading = false; });
    },
    onSave() {
      const form = this.$refs["leadTeacherForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.saving = true;
      this.$api.leadTeacher
        .update(this.document)
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
  }
};
</script>
