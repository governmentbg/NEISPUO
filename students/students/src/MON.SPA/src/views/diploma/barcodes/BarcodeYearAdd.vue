<template>
  <div>
    <v-form
      ref="form"
      @submit.prevent="validate"
    >
      <BarcodeYearForm
        ref="barcodeYearForm"
        :basic-document-id="basicDocumentId"
        :is-edit-form-mode="false"
        :form.sync="form"
        :saving="saving"
        @ResetForm="onReset"
      />
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import BarcodeYearForm from "@/components/diplomas/barcodes/BarcodeYearForm";
import { BarcodeYearModel } from "@/models/diploma/barcodeYearModel.js";

export default {
    name: 'BarcodeYearAdd',
    components: {
       BarcodeYearForm
    },
      props: {
        basicDocumentId: {
            type: Number,
            required: true
        },
    },
    data()
    {
        return {
            saving: false,
            form: new BarcodeYearModel()
        };
    },
     methods: {
        async validate() {
            let hasErrors = this.$validator.validate(this);

            if(hasErrors) {
                this.$notifier.error('', this.$t('validation.hasErrors'));
                return;
            }

            const payload = {
                edition: this.form.edition,
                schoolYear: this.form.schoolYear,
                headerPage: this.form.headerPage,
                internalPage: this.form.internalPage,
                basicDocumentId: this.basicDocumentId
            };

            if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))) {
                this.saving = true;

                this.$api.barcodeYear.addBarcodeYear(payload).then(() => {
                    this.$refs.barcodeYearForm.redirectToList();
                })
                .catch((error) => {
                    if (error.response && error.response.status === 400) {
                        this.$notifier.error('', this.$t('error.diplomaBarcodesSave'));
                    }
                    else{
                        this.$notifier.error('', this.$t('error.diplomaBarcodeEditionSchoolYearNotUniqueMessage'));
                    }
                    console.log(error.response.data.message);
                })
                .finally(() => { this.saving = false; });
            }
        },
        async onReset() {
            this.form = new BarcodeYearModel();
        }
    }
};
</script>


