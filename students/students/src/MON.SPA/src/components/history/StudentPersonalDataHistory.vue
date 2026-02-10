<template>
  <div>
    <v-dialog
      v-model="dialog"
      persistent
      max-width="1100"
    >
      <v-card>
        <v-card-title>
          {{ $t('studentTabs.history.title') }}
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
            ref="studentPersonalDataHistoryTable"
            :loading="loading"
            :items="studentPersonalDataHistory" 
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
            @click="dialog = false"
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
name: "StudentPersonalDataHistory",
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
      dialog: false,
      search: '',
      studentPersonalDataHistory: [],
      headers: [
        {
          text: this.$t('studentTabs.firstName'),
          value: 'firstName'
        },
        {
          text: this.$t('studentTabs.middleName'),
          value: 'middleName'
        },
        {
          text: this.$t('studentTabs.lastName'),
          value: 'lastName'
        },
        {
          text: this.$t('studentTabs.birthDate'),
          value: 'birthDate'
        },
        {
          text: this.$t('studentTabs.history.permanentResidenceAddress'),
          value: 'permanentAddress'
        },
        {
          text: this.$t('studentTabs.birthPlace'),
          value: 'birthPlace'
        },
        {
          text: this.$t('studentTabs.history.currentResidenceAddress'),
          value: 'currentAddress'
        },
        {
          text: this.$t('studentTabs.gender'),
          value: 'gender'
        },
        {
          text: this.$t('studentTabs.nationality'),
          value: 'nationality'
        },
        {
          text: this.$t('studentTabs.phoneNumber'),
          value: 'phoneNumber'
        }
      ]
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions'])
  },
  methods: {
    getStudentPersonalDataHistory() {
      this.search = '';
      this.loading = true;
      this.$api.student.getStudentPersonalDataHistory(this.personId)
        .then((response) => {
          this.studentPersonalDataHistory = response.data;
          this.dialog = true;
        })
        .catch((error) => {
          this.studentPersonalDataHistory = [];
          this.$notifier.error('', this.$t('errors.studentPersonalDataHistoryLoad'));
          console.log(error);
        })
        .then(() => { this.loading = false; });
    }
  }
};
</script>