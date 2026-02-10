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
    <div
      v-else
    >
      <form-layout>
        <template #title>
          <h3>{{ $t('lod.internationalMobility.reviewTitle') }}</h3>
        </template>
        <template #default>
          <international-mobility-form
            v-if="form !== null"
            :ref="'internationalMobilityForm' + _uid"
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
          </international-mobility-form>
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
  </div>
</template>

<script>
import InternationalMobilityForm from "@/components/tabs/internationalMobility/InternationalMobilityForm";
import { InternationalMobilityModel } from "@/models/internationalMobilityModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'InternationalMobilityDetails',
  components: {
    InternationalMobilityForm,
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    internationalMobilityId: {
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
    ...mapGetters(['hasStudentPermission'])
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentInternationalMobilityRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.studentInternationalMobility.getById(this.internationalMobilityId)
      .then(response => {
        if (response.data) {
          this.form = new InternationalMobilityModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.studentInternationalMobilityLoad'));
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
