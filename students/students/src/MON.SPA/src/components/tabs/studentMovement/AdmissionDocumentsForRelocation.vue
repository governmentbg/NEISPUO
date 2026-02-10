<template>
  <v-card
    v-show="showCard"
  >
    <v-card-title>
      {{ $t('relocationDocument.relatedAdmissionDocuments') }}
    </v-card-title>
    <v-card-text class="pa-0">
      <v-data-table
        ref="relatedAdmissionDocuments"
        :headers="headers"
        :items="admissionDocuments"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar
            flat
          >
            <v-spacer />
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              :label="$t('common.search')"
              single-line
              hide-details
            />
          </v-toolbar>
        </template>
        
        <template v-slot:[`item.noteDate`]="{ item }">
          {{ item.noteDate ? $moment(item.noteDate).format(dateFormat) : '' }}
        </template>
      </v-data-table>
    </v-card-text>
  </v-card>
</template>

<script>
import { mapGetters } from 'vuex';
import Constants from "@/common/constants.js"; 

export default {
  name: 'AdmissionDocumentsForRelocation',
  props: {
    docId: {
      type: Number,
      required: true
    },
    showCardIfEmpty: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      search: '',
      loading: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      admissionDocuments: [],
      headers: [
        {text: this.$t('documents.noteNumberLabel'), value: "noteNumber" },
        {text: this.$t('admissionDocument.noteDate'), value: "noteDate" },
        {text: this.$t('documents.institutionDropdownLabel'), value: "institutionName" },
      ]
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions']),
    showCard() {
      return this.showCardIfEmpty || this.admissionDocuments.length > 0;
    }
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.admissionDocument
        .getListForRelocationDocument(this.docId)
        .then((response) => {
          if(response.data) {
            this.admissionDocuments = response.data;
          }
        })
        .catch(error => {
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    }
  }
};
</script>