<template>
  <div>
    <div v-if="saving">
      <v-progress-linear
        indeterminate
        color="primary"
      />
    </div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t("printTemplate.addTitle") }}</h3>
      </template>
      <template #default>
        <print-template-form
          :ref="'printTemplateForm' + _uid"
          :form="form"
          :disabled="saving"
        />
      </template>
    </form-layout>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { PrintTemplateModel } from "@/models/printTemplateModel.js";
import PrintTemplateForm from "@/components/tabs/printTemplate/PrintTemplateForm.vue";

export default {
  name: "PrintTemplateAdd",
  components: {
    PrintTemplateForm,
  },
  props: {
  },
  data() {
    return {
      saving: false,
      form: new PrintTemplateModel(),
    };
  },
  methods: {
    async onSave() {
      const form = this.$refs["printTemplateForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      if (
        await this.$refs.confirm.open(
          this.$t("buttons.save"),
          this.$t("common.confirm")
        )
      ) {
        this.saving = true;

        this.$api.printTemplate
          .create(this.form)
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
            this.onCancel();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("common.saveError"));
            console.log(error.response);
          })
          .finally(() => {
            this.saving = false;
          });
      }
    },
      onCancel() {
        console.log("back");
      this.$router.go(-1);
    },
  },
};
</script>
