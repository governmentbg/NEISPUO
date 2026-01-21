<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <form-layout>
      <template #title>
        <h3>{{ $t("refugee.applicationDetails") }} № {{ appId }}</h3>
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
          is-details
          disabled
        />
      </template>

      <template #actions>
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="backClick"
        >
          <v-icon left>
            fas fa-chevron-left
          </v-icon>
          {{ $t('buttons.back') }}
        </v-btn>
      </template>
    </form-layout>
  </div>
</template>

<script>
import RefugeeApplicationForm from "@/components/refugee/RefugeeApplicationForm.vue";
import { RefugeeApplicationModel } from "@/models/refugee/refugeeApplicationModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "RefugeeApplicationDetailsView",
  components: {
    RefugeeApplicationForm,
  },
  props: {
    appId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      document: null,
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userDetails']),
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForRefugeeApplicationsRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.refugee
        .getDetailsById(this.appId)
      .then(response => {
        if(response.data) {
          this.document = new RefugeeApplicationModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('documents.documentLoadErrorMessage', 5000));
        console.log(error);
      })
      .then(()=> { this.loading = false; });
    },
    backClick() {
      this.$router.go(-1);
    }
  },
};
</script>
