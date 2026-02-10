<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('lod.internationalMobility.addTitle') }}</h3>
      </template>
      <template #default>
        <international-mobility-form
          v-if="form !== null"
          :ref="'internationalMobilityForm' + _uid"
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
import InternationalMobilityForm from "@/components/tabs/internationalMobility/InternationalMobilityForm";
import { InternationalMobilityModel } from "@/models/internationalMobilityModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
    name: 'InternationalMobilityCreate',
    components: {
        InternationalMobilityForm,
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
      if(!this.hasStudentPermission(Permissions.PermissionNameForStudentInternationalMobilityManage)) {
        return this.$router.push('/errors/AccessDenied');
      }

      this.form = new InternationalMobilityModel({
        institutionId: this.userInstitutionId,
        schoolYear: (await this.$api.institution.getCurrentYear(this.userInstitutionId)).data
      });
    },
    methods: {
        onSave() {
            const form = this.$refs['internationalMobilityForm' + this._uid];
            const isValid = form.validate();

            if(!isValid) {
                return this.$notifier.error('', this.$t('validation.hasErrors'));
            }

            this.form.personId = this.personId;
            this.form.fromDate = this.$helper.parseDateToIso(this.form.fromDate, '');
            this.form.toDate = this.$helper.parseDateToIso(this.form.toDate, '');

            this.saving = true;
            this.$api.studentInternationalMobility.create(this.form)
            .then(() => {
                this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
                this.$router.go(-1);
            })
            .catch((error) => {
                this.$notifier.error('',this.$t("errors.studentInternationalMobilityAdd"));
                console.log(error.response);
            })
            .then(() => { this.saving = false; });
        },
        onCancel() {
            this.$router.go(-1);
        },
    }
};
</script>
