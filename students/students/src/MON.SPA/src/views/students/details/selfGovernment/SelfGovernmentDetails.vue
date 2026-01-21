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
        <h3>{{ $t('lod.selfGovernment.reviewTitle') }}</h3>
      </template>
      <template #default>
        <self-government-form
          v-if="form !== null"
          :ref="'selfGovernmentForm' + _uid"
          :value="form"
          is-details
          disabled
        >
          <template #schoolYear>
            <v-text-field
              v-if="form"
              :value="form.schoolYearName"
              :label="$t('common.schoolYear')"
              disabled
              outlined
              persistent-placeholder
            />
          </template>
        </self-government-form>
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

import SelfGovernmentForm from '@/components/tabs/selfGovernment/SelfGovernmentForm.vue';
import { StudentSelfGovernmentModel } from '@/models/studentSelfGovernmentModel.js';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'SelfGovernmentDetails',
  components: {
    SelfGovernmentForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    selfGovernmentdId: {
      type: Number,
      required: true
    }
  },
  data()
  {
    return {
      loading: true,
      form: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission'])
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSelfGovernmentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.selfGovernment.getById(this.selfGovernmentdId)
      .then(response => {
        if (response.data) {
          this.form = new StudentSelfGovernmentModel(response.data);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.studentInternationalMobilityLoad'));
        console.log(error);
      })
      .then(() => { this.loading = false; });
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
