<template>
  <v-form
    :ref="'diplomaCreateRequestEditorForm' + _uid"
    :disabled="disabled"
  >
    <!-- <v-row
      dense
    >
      {{ value }}
    </v-row> -->
    <v-row
      v-if="value"
      dense
    >
      <v-col
        cols="12"
      >
        <c-info
          uid="diplomaCreateRequest.oldInstitution"
        >
          <v-row
            dense
          >
            <v-col
              v-if="value.arbitraryCurrentInstitutionName"
              cols="12"
              md="10"
            >
              <v-text-field
                v-model="value.currentInstitutionName"
                :label="$t('diplomas.createRequest.headers.currentInstitution')"
                clearable
              />
            </v-col>
            <v-col
              v-else
              cols="12"
              md="10"
            >
              <autocomplete
                :ref="'DiplomaCreateRequestEditorCurrentInstitution_' + _uid"
                v-model="value.currentInstitutionId"
                api="/api/lookups/GetInstitutionOptions"
                :label="$t('diplomas.createRequest.headers.currentInstitution')"
                :placeholder="$t('common.choose')"
                clearable
                @change="onCurrentInstitutionChange"
              />
            </v-col>
            <v-col
              cols="12"
              md="2"
            >
              <v-checkbox
                v-model="value.arbitraryCurrentInstitutionName"
                color="primary"
                :label="$t('diplomas.createRequest.arbitraryName')"
              />
            </v-col>
          </v-row>
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
      >
        <c-info
          uid="diplomaCreateRequest.requestingInstitution"
        >
          <autocomplete
            v-model="value.basicDocumentId"
            api="/api/lookups/GetBasicDocumentTypes"
            :label="$t('diplomas.createRequest.headers.basicDocument')"
            :placeholder="$t('common.choose')"
            clearable
            :defer-options-loading="false"
            class="required"
            :rules="[$validator.required()]"
            :filter="{ isValidation: false, filterByDetailedSchoolType: true }"
          />
        </c-info>
      </v-col>

      <v-col
        cols="12"
        md="6"
        xl="2"
      >
        <c-info
          uid="diplomaCreateRequest.registrationNumber"
        >
          <v-text-field
            v-model="value.registrationNumber"
            :label="$t('diplomas.createRequest.headers.registrationNumber')"
            :rules="[$validator.required()]"
            class="required"
            clearable
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        xl="2"
      >
        <c-info
          uid="diplomaCreateRequest.registrationNumberYear"
        >
          <v-text-field
            v-model="value.registrationNumberYear"
            :label="$t('diplomas.createRequest.headers.registrationNumberYear')"
            clearable
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        xl="2"
      >
        <c-info
          uid="diplomaCreateRequest.registrationDate"
        >
          <date-picker
            v-model="value.registrationDate"
            :label="$t('diplomas.createRequest.headers.registrationDate')"
            clearable
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
      >
        <c-info
          uid="diplomaCreateRequest.note"
        >
          <v-textarea
            v-model="value.note"
            outlined
            rows="2"
            prepend-icon="mdi-comment"
            :label="$t('environmentCharacteristics.notes')"
          />
        </c-info>
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';


export default {
  name: "DiplomaCreateRequestEditorFormComponent",
  components: {
    Autocomplete
  },
  props: {
    value: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    },
  },
  data() {
    return {};
  },
  mounted() {},
  methods: {
    validate() {
      const form = this.$refs['diplomaCreateRequestEditorForm' + this._uid];
      return form ? form.validate() : false;
    },
    onCurrentInstitutionChange(currentInstitutionId) {
      const selector = this.$refs[`DiplomaCreateRequestEditorCurrentInstitution_${this._uid}`];
       if (selector) {
        const selectedItem = selector.getOptionsList().find(x => x.value === currentInstitutionId);
        this.value.currentInstitutionName = selectedItem?.text;
      }
    },
  }
};
</script>
