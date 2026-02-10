<template>
  <div v-if="loading">
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
  </div>
  <div v-else>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('preSchool.editTitle') }}</h3>
      </template>
      <template>
        <pre-school-evaluation-form
          :value="form"
          :disabled="saving"
        />
      </template>
    </form-layout>
    <v-overlay
      :value="saving"
    >
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import PreSchoolEvaluationForm from '@/components/preSchoolEval/PreSchoolEvaluationForm';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentPreSchoolEvaluationEditView',
  components: {
    PreSchoolEvaluationForm
  },
  props: {
    evalId: {
      type: Number,
      required: true
    }
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
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentPreSchoolEvaluationManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.preSchool.getEvalById(this.evalId)
      .then(response => {
        if (response.data) {
          this.form = response.data;
        }
      })
      .catch(error => {
        console.log(error);
        this.$notifier.error('', this.$t('common.loadError'));
      })
      .then(() => { this.loading = false; });
    },
    onSave() {
      this.saving = true;
      this.$api.preSchool.update(this.form)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'), 5000);
          console.log(error);
        })
        .then(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>
