<template>
  <div>
    <v-simple-table
      v-if="absencesModel"
    >
      <template #default>
        <thead>
          <tr>
            <th />
            <th>{{ $t('studentEvaluations.firstTermEvaluation') }}</th>
            <th>{{ $t('studentEvaluations.secondTermEvaluation') }}</th>
            <th>{{ $t('absence.total') }}</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>
              {{ $t('absence.excused') }}
            </td>
            <td>
              {{ absencesModel.absencesForValidReasonsFirstTerm }}
            </td>
            <td>
              {{ absencesModel.absencesForValidReasonsSecondTerm }}
            </td>
            <td>
              {{ absencesModel.annualAbsencesForValidReasons }}
            </td>
          </tr>
          <tr>
            <td>
              {{ $t('absence.unexcused') }}
            </td>
            <td>
              {{ absencesModel.absencesForInvalidReasonsFirstTerm }}
            </td>
            <td>
              {{ absencesModel.absencesForInvalidReasonsSecondTerm }}
            </td>
            <td>
              {{ absencesModel.annualAbsencesForInvalidReasons }}
            </td>
          </tr>
        </tbody>
      </template>
    </v-simple-table>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>


<script>
export default {
  name: 'RelocationDocumentAbsencesComponent',
  props: {
    docId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: false,
      absencesModel: null,
    };
  },
  mounted() {
    this.loadAbsences();
  },
  methods: {
    loadAbsences() {
      this.loading = true;
      this.$api.relocationDocument
        .getAbsences(this.docId)
        .then((result) => {
          if(result.data) {
            this.absencesModel = result.data;
          }
        }).catch((err) => {
          this.$notifier.error('', this.$t('common.loadError'));
          console.log(err.response);
        })
        .finally(() => {
          this.loading = false;
        });
    }
  }
};
</script>
