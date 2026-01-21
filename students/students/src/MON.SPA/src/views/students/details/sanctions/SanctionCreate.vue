<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('lod.sanctions.studentSanctionsAddTitle') }}</h3>
      </template>
      <template #default>
        <sanction-form
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
import SanctionForm from "@/components/tabs/sanctions/SanctionForm";
import { StudentSanctionModel } from "@/models/studentSanctionModel.js";
import { Permissions} from "@/enums/enums";
import { mapGetters } from 'vuex';


export default {
  name: 'SanctionCreate',
  components: {
    SanctionForm,
  },
  props: {
    personId: {
      type: Number,
      required: true
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
    ...mapGetters(['hasStudentPermission', 'isInRole', 'userSelectedRole', 'userInstitutionId']),
    disabled() {
      return this.saving;
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSanctionManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    const form = new StudentSanctionModel({
        institutionId: this.userInstitutionId,
        schoolYear: (await this.$api.institution.getCurrentYear(this.userInstitutionId)).data
      });

    this.form = form;
  },
  methods: {
    onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.form.personId = this.personId;
      this.form.startDate = this.$helper.parseDateToIso(this.form.startDate, '');
      this.form.endDate = this.$helper.parseDateToIso(this.form.endDate, '');
      this.form.cancelOrderDate = this.$helper.parseDateToIso(this.form.cancelOrderDate, '');
      this.form.orderDate = this.$helper.parseDateToIso(this.form.orderDate, '');
      this.form.ruoOrderDate = this.$helper.parseDateToIso(this.form.ruoOrderDate, '');

      this.saving = true;
      this.$api.studentSanctions.create(this.form)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
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
    },
  }
};
</script>
