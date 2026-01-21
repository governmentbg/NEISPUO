<template>
  <div>
    <div v-if="saving">
      <v-progress-linear
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <form-layout
        :disabled="saving"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t("printTemplate.editTitle") }}</h3>
        </template>

        <template #default>
          <print-template-form
            v-if="form !== null"
            :ref="'printTemplateForm' + _uid"
            :form="form"
            :disabled="saving"
          />
        </template>
      </form-layout>
      <confirm-dlg ref="confirm" />
    </div>
  </div>
</template>


<script>
import PrintTemplateForm from "@/components/tabs/printTemplate/PrintTemplateForm.vue";
import { PrintTemplateModel } from "@/models/printTemplateModel.js";

export default {
  name: "PrintTemplateEdit",
  components: {
    PrintTemplateForm,
  },
  props: {
    id: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      saving: false,
      form: new PrintTemplateModel(),
      loading: true,
      isEditMode: false,
    };
  },
  mounted() {
    this.loadData();
  },
  methods: {
    loadData() {
      this.saving = true;

      this.$api.printTemplate
        .getPrintTemplate(this.id)
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.printTemplateLoad"));
          console.log(error);
        })
        .then((response) => {
          if (response.data) {
            this.form = new PrintTemplateModel(response.data, this);
            console.log(this.form);
          }
        })
        .finally(() => {
          this.saving = false;
        });
    },
    async onSave() {
      const form = this.$refs["printTemplateForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }
        console.log("aaaaa");
      if (
        await this.$refs.confirm.open(
          this.$t("buttons.save"),
          this.$t("common.confirm")
        )
      ) {
        this.saving = true;

        this.$api.printTemplate
          .update(this.form)
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
            this.onCancel();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("errors.printTemplateAdd"));
            console.log(error.response.data.message);
          })
          .finally(() => {
            this.saving = false;
          });
      }
    },
    onCancel() {
      this.$router.go(-1);
    },
  },
};
</script>
