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
        <h3>{{ $t('lod.sanctions.reviewTitle') }}</h3>
      </template>
      <template #default>
        <sanction-form
          v-if="form !== null"
          :ref="'form' + _uid"
          :value="form"
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
          <template #institution>
            <v-text-field
              v-if="form"
              :value="form.institutionName"
              :label="$t('common.institution')"
              disabled
              outlined
              persistent-placeholder
            />
          </template>
        </sanction-form>
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
import SanctionForm from "@/components/tabs/sanctions/SanctionForm";
import { StudentSanctionModel } from "@/models/studentSanctionModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'SanctionDetails',
  components: {
    SanctionForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    sanctionId: {
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
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSanctionRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.studentSanctions.getById(this.sanctionId)
      .then(response => {
        if (response.data) {
          this.form = new StudentSanctionModel(response.data, this.$moment);
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
