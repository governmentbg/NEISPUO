<template>
  <div>
    <v-dialog
      v-model="sopDialog"
      persistent
      max-width="1100"
    >
      <v-card>
        <v-card-title>
          {{ $t('sop.history.title') }}
          <v-spacer />
          <v-text-field
            v-model="search"
            append-icon="mdi-magnify"
            :label="$t('buttons.search')"
            clearable
            single-line
            hide-details
          />
        </v-card-title>
        <v-card-text>
          <v-data-table
            ref="studentSopHistoryTable"
            :loading="loading"
            :items="studentSopHistory" 
            :headers="headers"
            :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
            :search="search"
            class="elevation-1"
          />
        </v-card-text>

        <v-card-actions class="justify-center">
          <v-btn
            color="primary"
            text
            @click="sopDialog = false"
          >
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>


<script>
import { mapGetters } from 'vuex';

export default {
name: "StudentSopHistory",
  props: {
    personId: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      loading: false,
      sopDialog: false,
      search: '',
      studentSopHistory: [],
      headers: [
        {
          text: this.$t('sop.history.type'),
          value: 'specialNeedsType'
        },
        {
          text: this.$t('sop.history.description'),
          value: 'specialNeedsSubType'
        },
        {
          text: this.$t('sop.history.supportiveEnvironment'),
          value: 'supportiveEnvironment'
        },
        {
          text: this.$t('sop.history.schoolYear'),
          value: 'schoolYearName'
        }
      ]
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions'])
  },
  methods: {
    getStudentSopHistory() {
      this.loading = true;
      this.$api.student.getStudentSopHistory(this.personId)
        .then((response) => {
          this.studentSopHistory = response.data;
          this.sopDialog = true;
        })
        .catch((error) => {
          this.studentSopHistory = [];
          this.$notifier.error('', this.$t('errors.studentSopHistoryLoad'));
          console.log(error);
        })
        .then(() => { this.loading = false; });
    }
  }
};
</script>