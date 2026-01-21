<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t("refugee.newApplication") }}</h3>
        <v-spacer />
        <h3
          v-if="userDetails && userDetails.region"
        >
          РУО {{ userDetails.region }}
        </h3>
      </template>

      <template #default>
        <refugee-application-form
          v-if="document"
          :ref="'refugeeApplicationForm' + _uid"
          :document="document"
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
  </div>
</template>

<script>
import RefugeeApplicationForm from "@/components/refugee/RefugeeApplicationForm.vue";
import { RefugeeApplicationModel } from "@/models/refugee/refugeeApplicationModel";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: "RefugeeApplicationCreateView",
  components: {
    RefugeeApplicationForm,
  },
  data() {
    return {
      saving: false,
      document: null,
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userDetails'])
  },
  mounted() {
    if (!this.hasPermission(Permissions.PermissionNameForRefugeeApplicationsManage)) {
     return this.$router.push('/errors/AccessDenied');
    }

    this.init();
  },
  methods: {
    init() {
      this.document = new RefugeeApplicationModel({}, this.$moment);
    },
    onSave() {
      const form = this.$refs["refugeeApplicationForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.document.applicationDate = this.$helper.parseDateToIso(this.document.applicationDate, '');

      this.saving = true;
      this.$api.refugee
        .create(this.document)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(
            this.$t("common.save"),
            error?.response?.data?.message ?? this.$t("common.error"),
            7000
          );
          console.log(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    },
  },
};
</script>
