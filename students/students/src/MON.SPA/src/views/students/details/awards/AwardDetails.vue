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
        <h3>{{ $t('lod.awards.reviewTitle') }}</h3>
      </template>
      <template #default>
        <award-form
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
        </award-form>
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

import AwardForm from "@/components/tabs/awards/AwardForm";
import { StudentAwardModel } from "@/models/studentAwardModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'AwardDetails',
  components: {
    AwardForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    awardId: {
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
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentAwardRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.studentAwards.getById(this.awardId)
      .then(response => {
        if (response.data) {
          this.form = new StudentAwardModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.studentAwardsLoad'));
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
