<template>
  <div>
    <v-form
      ref="form"
      @submit.prevent="validate"
    >
      <BarcodeYearForm
        ref="barcodeYearForm"
        :basic-document-id="basicDocumentId"
        :is-edit-form-mode="true"
        :form.sync="form"
        :saving="saving"
      />
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>

import BarcodeYearForm from "@/components/diplomas/barcodes/BarcodeYearForm";
import { BarcodeYearModel } from "@/models/diploma/barcodeYearModel.js";

export default {

     name: 'BarcodeYearEdit',
    components: {
       BarcodeYearForm
    },
      props: {
        basicDocumentId: {
            type: Number,
            required: true
        },
        barcodeYearId: {
            type: Number,
            required: true
        },
    },
    data()
    {
        return {
            saving: false,
            form: new BarcodeYearModel(),
            isEditMode: false
        };
    },
    mounted() {
       this.loadData();
    },
    methods: {
        loadData() {
            this.saving = true;
            this.$api.barcodeYear.getBarcodeYear(this.barcodeYearId)
            .catch(error => {
                this.$notifier.error('', this.$t('errors.diplomaBarcodesLoad'));
                console.log(error);
            })
            .then(response => {
                if (response.data) {
                    this.form = new BarcodeYearModel(response.data, this);
                }
            })
           .finally(() => { this.saving = false; });
        },
        async validate() {
            let hasErrors = this.$validator.validate(this);

            if(hasErrors) {
                this.$notifier.error('', this.$t('validation.hasErrors'));
                return;
            }

            if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))) {
              
                this.saving = true;

                const payload = {
                    id: this.form.id,
                    edition: this.form.edition,
                    schoolYear: this.form.schoolYear,
                    headerPage: this.form.headerPage,
                    internalPage: this.form.internalPage,
                    basicDocumentId: this.basicDocumentId
                };

                this.$api.barcodeYear.updateBarcodeYear(payload)
                .then(() => {
                   this.$refs.barcodeYearForm.redirectToList();
                })
                .catch((error) => {
                    this.$notifier.error('',this.$t("errors.diplomaBarcodesAdd"));
                    console.log(error.response.data.message);
                })
                .finally(() => { this.saving = false; });
            }
        },
    }
};
</script>
