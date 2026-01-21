<template>
  <v-row
    v-if="value"
    dense
  >
    <v-col
      cols="12"
      md="6"
    >
      <autocomplete
        v-model="value.basicDocumentId"
        api="/api/lookups/GetBasicDocumentTypes"
        :label="$t('diplomas.additionalDocument.basicDocument')"
        :placeholder="$t('common.choose')"
        clearable
        :defer-options-loading="false"
        class="required"
        :rules="[$validator.required()]"
        :filter="{
          mainBasicDocuments: mainBasicDocuments,
        }"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      <autocomplete
        v-model="value.institutionId"
        api="/api/lookups/GetInstitutionOptions"
        :label="$t('diplomas.additionalDocument.institution')"
        :placeholder="$t('common.choose')"
        clearable
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      xl="3"
    >
      <v-text-field
        v-model="value.series"
        :label="$t('diplomas.additionalDocument.series')"
      />
    </v-col>

    <v-col
      cols="12"
      md="6"
      xl="3"
    >
      <v-text-field
        v-model="value.factoryNumber"
        :label="$t('diplomas.additionalDocument.factoryNumber')"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      xl="3"
    >
      <v-text-field
        v-model="value.registrationNumber"
        :label="$t('diplomas.additionalDocument.registrationNumber')"
        :rules="[$validator.required()]"
        class="required"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      xl="3"
    >
      <v-text-field
        v-model="value.registrationNumberYear"
        :label="$t('diplomas.additionalDocument.registrationNumberYear')"
        :rules="[$validator.required()]"
        class="required"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      xl="3"
    >
      <date-picker
        v-model="value.registrationDate"
        :label="$t('diplomas.additionalDocument.registrationDate')"
        :rules="[$validator.required()]"
        class="required"
      />
    </v-col>
    <v-col
      md="12"
      cols="12"
    >
      <v-alert
        class="mt-5"
        border="left"
        colored-border
        type="info"
        elevation="2"
        md="12"
      >
        В случай, че институцията не съществува или искате да въведете друг адрес, попълнете данните по-долу
      </v-alert>
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      <v-text-field
        v-model="value.institutionName"
        :label="$t('diplomas.additionalDocument.institutionName')"
      />
    </v-col>

    <v-col
      cols="12"
      md="6"
    >
      <v-text-field
        v-model="value.institutionAddress"
        :label="$t('diplomas.additionalDocument.institutionAddress')"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      <v-text-field
        v-model="value.town"
        :label="$t('diplomas.additionalDocument.town')"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      <v-text-field
        v-model="value.municipality"
        :label="$t('diplomas.additionalDocument.municipality')"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      <v-text-field
        v-model="value.region"
        :label="$t('diplomas.additionalDocument.region')"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      <v-text-field
        v-model="value.localArea"
        :label="$t('diplomas.additionalDocument.localArea')"
      />
    </v-col>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-row>
</template>

<script>
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { mapGetters } from 'vuex';

export default {
  name: 'DiplomaAdditionalDocumentEditorComponent',
  components: {
    Autocomplete
  },
  props: {
    value: {
      type: Object,
      required: true,
    },
    mainBasicDocuments: {
      type: Array,
      default() {
        return [];
      }
    }
  },
  data() {
    return {
      loading: false
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId']),
    isNew() {
      return !this.value.id;
    }
  },
  watch: {
    'value.basicDocumentId': function(newVal) {
      this.loadExistingData(newVal);
    }
  },
  mounted() {
    if (this.isNew && !this.value.institutionId && this.userInstitutionId) {
      this.value.institutionId = this.userInstitutionId;
    }
  },
  methods: {
    loadExistingData(selectedBasicDocumentId) {
      if(!selectedBasicDocumentId) {
        this.clearData();
        return;
      }

      const personId = this.$route.params.id;
      if(!personId) {
        return;
      }

      this.loading = true;
      this.$api.diploma.getAdditionalDocumentDetails(personId, selectedBasicDocumentId)
        .then((response) => {
          this.setData(response.data);
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.loadError'));
          console.log(error.response);
        })
        .finally(() => {
          this.loading = false;
        });
    },
    clearData() {
      if(this.value) {
        this.value.series = null;
        this.value.factoryNumber = null;
        this.value.registrationNumber = null;
        this.value.registrationNumberYear = null;
        this.value.registrationDate = null;
      }
    },
    setData(data) {
      if(this.value) {
        this.value.series = data?.series;
        this.value.factoryNumber = data?.factoryNumber;
        this.value.registrationNumber = data?.registrationNumberTotal;
        this.value.registrationNumberYear = data?.registrationNumberYear;
        this.value.registrationDate = data?.registrationDate;
      }
    }
  }
};
</script>
