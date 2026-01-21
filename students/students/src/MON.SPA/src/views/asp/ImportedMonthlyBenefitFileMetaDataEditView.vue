<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('asp.ImportedFileMetaDataEdit') }}</h3>
      </template>
      <template #default>
        <ImportedMonthlyBenefitFileMetaDataForm
          :ref="'form' + _uid"
          :file-meta-data="fileMetaData"
          :disabled="disabled"
        />
      </template>
    </form-layout>

    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import ImportedMonthlyBenefitFileMetaDataForm from "@/components/asp/ImportedMonthlyBenefitFileMetaDataForm";
import { MonthlyBenefitsViewModel } from "@/models/asp/monthlyBenefitsViewModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "ImportedMonthlyBenefitFileMetaDataEditView",
  components: {
      ImportedMonthlyBenefitFileMetaDataForm
  },
  props: {
    importedFileId: {
      type: Number,
      required: true,
    }
  },
   data() {
    return {
        fileMetaData: null,
        loading: false
    };
  },
   computed: {
      ...mapGetters(['hasPermission']),
      disabled() {
        return this.loading;
        }
    },
  async mounted() {

    if(!this.hasPermission(Permissions.PermissionNameForASPImport)) {
        return this.$router.push('/errors/AccessDenied');
    }

    this.loading = true;

    this.$api.asp.getImportedBenefitsFileMetaData(this.importedFileId).then((response) => {
            if(response.data){
                this.fileMetaData = new MonthlyBenefitsViewModel(response.data, this.$moment);
            }
        }).catch((error) => {
            this.$notifier.error('', this.$t('errors.importedBenefitsDetailsLoad'));
            console.log(error);
        }).finally(() => {
            this.loading = false;
        });
  },
  methods: {
  onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.fileMetaData.id = this.importedFileId;
      this.fileMetaData.fromDate = this.$helper.parseDateToIso(this.fileMetaData.fromDate, '');
      this.fileMetaData.toDate = this.$helper.parseDateToIso(this.fileMetaData.toDate, '');

      this.loading = true;

      this.$api.asp.updateImportedBenefitsFileMetaData(this.fileMetaData)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.$router.go(-1);
      })
      .catch((error) => {
        this.$notifier.error('',this.$t("common.saveError"), 5000);
        console.log(error.response);
      })
      .then(() => { this.loading = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>
