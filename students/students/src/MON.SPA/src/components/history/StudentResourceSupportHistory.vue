<template>
  <div>
    <v-dialog
      v-model="resourceSupportDialog"
      persistent
      max-width="1100"
    >
      <v-card>
        <v-card-title>
          {{ $t('resourceSupport.history.title') }}
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
            ref="studentResourceSupportHistoryTable"
            :loading="loading"
            :items="studentResourceSupportHistory" 
            :headers="headers"
            :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
            :search="search"
            class="elevation-1"
          >
            <template v-slot:[`item.reportDate`]="{ item }">
              {{ item.reportDate ? $moment(item.reportDate).format(dateFormat) : '' }}
            </template>
          </v-data-table>
        </v-card-text>

        <v-card-actions class="justify-center">
          <v-btn
            color="primary"
            text
            @click="resourceSupportDialog = false"
          >
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>


<script>
import Constants from "@/common/constants.js";
import { mapGetters } from 'vuex';

export default {
 name: "StudentResourceSupportHistory",
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
      search: '',
      resourceSupportDialog: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      studentResourceSupportHistory: [],
      headers: [
        {
          text: this.$t('resourceSupport.history.reportNumber'),
          value: 'reportNumber'
        },
        {
          text: this.$t('resourceSupport.history.reportDate'),
          value: 'reportDate'
        },
        {
          text: 'Извършва се от',
          value: 'resourceSupportType'
        },        
      ]
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions'])
  },
  methods: {
    getStudentResourceSupportHistory() {
      this.$api.student.getStudentResourceSupportHistory(this.personId)
        .then((response) => {
          this.studentResourceSupportHistory = response.data;
          this.resourceSupportDialog = true;
        })
        .catch((error) => {
          this.studentResourceSupportHistory = [];
          this.$notifier.error('', this.$t('errors.studentResourceSupportHistoryLoad'));
          console.log(error);
        });
    }
  }
};


</script>