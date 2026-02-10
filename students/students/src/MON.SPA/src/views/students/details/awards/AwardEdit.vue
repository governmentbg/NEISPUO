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
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('lod.awards.editTitle') }}</h3>
      </template>
      <template #default>
        <award-form
          v-if="form !== null"
          :ref="'form' + _uid"
          :value="form"
          :disabled="disabled"
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

import AwardForm from "@/components/tabs/awards/AwardForm";
import { StudentAwardModel } from "@/models/studentAwardModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'AwardEdit',
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
      saving: false,
      form: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    disabled() {
      return this.saving;
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentAwardManage)) {
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
    onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.form.date = this.$helper.parseDateToIso(this.form.date, '');

      this.saving = true;
      this.$api.studentAwards.update(this.form)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.$router.go(-1);
      })
      .catch((error) => {
        this.$notifier.error('',this.$t("errors.studentAwardsAdd"));
        console.log(error.response);
      })
      .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>