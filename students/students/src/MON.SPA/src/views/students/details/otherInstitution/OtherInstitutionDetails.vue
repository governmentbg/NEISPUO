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
        <h3>{{ $t('studentOtherInstitutions.reviewTitle') }}</h3>
      </template>
      <template #default>
        <OtherInstitutionForm
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

import OtherInstitutionForm from "@/components/tabs/studentOtherInstitutions/OtherInstitutionForm";
import { StudentOtherInstitutionModel } from '@/models/studentOtherInstitutionModel.js';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'OtherInstitutionDetails',
  components: {
    OtherInstitutionForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    institutionId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: true,
      form: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission'])
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherInstitutionRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.otherInstitution.getById(this.institutionId)
      .then(response => {
        if (response.data) {
          this.form = new StudentOtherInstitutionModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('studentOtherInstitutions.updateOtherInstitutionsErrorMessage'));
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