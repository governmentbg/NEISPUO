<template>
  <div
    v-if="loading"
  >
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
  </div>
  <div
    v-else
  >
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('lod.sanctions.studentSanctionsEditTitle') }}</h3>
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
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import SanctionForm from "@/components/tabs/sanctions/SanctionForm";
import { StudentSanctionModel } from "@/models/studentSanctionModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
    name: 'SanctionEdit',
    components: {
       SanctionForm
    },
    props: {
        personId: {
            type: Number,
            required: false,
            default: 0
        },
        sanctionId: {
            type: Number,
            required: true
        },
    },
    data() {
        return {
            loading: true,
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
    mounted() {
        if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSanctionManage)) {
            return this.$router.push('/errors/AccessDenied');
        }

       this.load();
    },
    methods: {
        load() {
            this.loading = true;

            this.$api.studentSanctions.getById(this.sanctionId)
            .then(response => {
                if (response.data) {
                    this.form = new StudentSanctionModel(response.data, this.$moment);
                    if(!this.form.institutionId) {
                      this.form.institutionId = this.userInstitutionId;
                    }
                }
            })
            .catch(error => {
              this.$notifier.error('', this.$t('common.loadError'));
              console.log(error.response);
            })
           .then(() => { this.loading = false; });
        },
        onSave() {
            const form = this.$refs['form' + this._uid];
            const isValid = form.validate();

            if(!isValid) {
                return this.$notifier.error('', this.$t('validation.hasErrors'));
            }

            this.form.startDate = this.$helper.parseDateToIso(this.form.startDate, '');
            this.form.endDate = this.$helper.parseDateToIso(this.form.endDate, '');
            this.form.cancelOrderDate = this.$helper.parseDateToIso(this.form.cancelOrderDate, '');
            this.form.orderDate = this.$helper.parseDateToIso(this.form.orderDate, '');
            this.form.ruoOrderDate = this.$helper.parseDateToIso(this.form.ruoOrderDate, '');

            this.saving = true;


            this.$api.studentSanctions.update(this.form)
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
        },
    }
};
</script>
