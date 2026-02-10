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
    <form-layout>
      <template #title>
        <h3>{{ $t('resourceSupport.reviewTitle') }}</h3>
      </template>
      <template #default>
        <resource-support-form
          v-if="form !== null"
          :ref="'form' + _uid"
          :value="form"
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
import ResourceSupportForm from "@/components/tabs/resourceSupport/ResourceSupportForm";
import { StudentResourceSupportModel } from "@/models/studentResourceSupportModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'ResourceSupportDetails',
  components: {
    ResourceSupportForm
  },
  props: {
    personId: {
      type: Number,
      required: false,
      default: 0
    },
    reportId: {
      type: Number,
      required: true
    },
  },
  data()
  {
    return {
      loading: true,
      form: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.resourceSupport.getById(this.reportId)
      .then(response => {
        if (response.data) {
          this.form = new StudentResourceSupportModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('common.loadError'));
        console.log(error.response);
      })
      .then(() => { this.loading = false; });
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
