<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('resourceSupport.createTitle') }}</h3>
      </template>
      <template #default>
        <resource-support-form
          v-if="form !== null"
          :ref="'form' + _uid"
          :value="form"
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
import ResourceSupportForm from "@/components/tabs/resourceSupport/ResourceSupportForm";
import { StudentResourceSupportModel } from "@/models/studentResourceSupportModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'ResourceSupportCreate',
  components: {
    ResourceSupportForm,
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      default() {
        return null;
      }
    },
  },
  data()
  {
    return {
      saving: false,
      form: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userInstitutionId']),
    disabled() {
      return this.saving;
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    const form = new StudentResourceSupportModel();
    form.schoolYear = isNaN(this.schoolYear)
      ? (await this.$api.institution.getCurrentYear(this.userInstitutionId)).data
      : this.schoolYear;
    this.form = form;
  },
  methods: {
    onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      if(!Array.isArray(this.form.resourceSupports) || this.form.resourceSupports.length === 0) {
        return this.$notifier.error(this.$t('validation.hasErrors'), this.$t('resourceSupport.emptyResourceSupportsError'));
      }

      this.form.personId = this.personId;
      this.form.reportDate = this.$helper.parseDateToIso(this.form.reportDate, '');

      this.saving = true;
      this.$api.resourceSupport.create(this.form)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'));
        this.$router.go(-1);
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('common.saveError'));
        console.log(error.response);
      })
      .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
