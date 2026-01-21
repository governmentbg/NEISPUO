<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3> {{ $t('certificate.addTitle') }}</h3>
      </template>

      <template #default>
        <CertificateForm 
          :ref="'certificateForm' + _uid"
          :model="model"
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
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import CertificateForm from "@/components/certificate/CertificateForm.vue";
import { Permissions } from '@/enums/enums';
import { mapGetters } from "vuex";

export default {
name: "CertificateCreateView",
components: {
CertificateForm
},
data() {
    return {
      saving: false,
      model: { fileToAdd: { description: null,file: null}, isValid: false,  description: null, certificateType: null}
    };
},
computed:{
    ...mapGetters(['hasPermission']),
},
async mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForCertificatesManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
},
methods:{
    async onCancel() {
       this.$router.push({ name: 'CertificatesList' });
    },
    async onSave() {
      let hasErrors = this.$validator.validate(this);

      if(hasErrors) {
        this.$notifier.error('', this.$t('validation.hasErrors'));
        return;
      }

      if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))) {
        const fileModel = await this.$helper.getFileContent(this.model.fileToAdd.file);

        const payload = {
          name: fileModel.noteFileName,
          isValid: this.model.isValid,
          certificateType:  this.model.certificateType,
          contents: fileModel.noteContents,
          description: this.model.description
        };

        this.saving = true;

        this.$api.certificate.create(payload).then(() => {
           this.$router.push({ name: 'CertificatesList' });
        }).catch((error) => {
            console.log(error.response);
            this.$notifier.error('', this.$t('certificate.error'));
        }).finally(() => {
          this.saving = false;
        });
      }
    }
}  
};
</script>
