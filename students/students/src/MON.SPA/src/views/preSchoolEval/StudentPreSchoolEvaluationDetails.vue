<template>
  <div v-if="loading">
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
  </div>
  <div v-else>
    <form-layout>
      <template #title>
        <h3>{{ $t('preSchool.reviewTitle') }}</h3>
      </template>
      <template>
        <pre-school-evaluation-form
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
import PreSchoolEvaluationForm from '@/components/preSchoolEval/PreSchoolEvaluationForm';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentPreSchoolEvaluationDetailsView',
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
      form: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentPreSchoolEvaluationRead)) {
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
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
