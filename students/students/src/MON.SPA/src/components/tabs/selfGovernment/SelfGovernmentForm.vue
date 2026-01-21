<template>
  <v-form
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <v-row dense>
      <v-col
        v-if="!isDetails"
        cols="12"
      >
        <v-alert
          border="left"
          colored-border
          type="warning"
          elevation="2"
        >
          Промяната на мобилен телефон или електронна поща ще се отрази в профила на лицето в НЕИСПУО.
        </v-alert>
      </v-col>
      <v-col
        cols="12"
        sm="6"
        md="2"
      >
        <c-info
          uid="selfGovernment.schoolYear"
        >
          <slot name="schoolYear">
            <school-year-selector
              v-model="model.schoolYear"
              :institution-id="userInstitutionId"
              :label="$t('common.schoolYear')"
              class="required"
              :clearable="false"
              :rules="[$validator.required()]"
              show-current-school-year-button
              outlined
              persistent-placeholder
            />
          </slot>
        </c-info>
      </v-col>
      <v-col
        cols="12"
        sm="6"
        md="5"
      >
        <c-info
          uid="selfGovernment.participation"
        >
          <v-text-field
            v-if="disabled"
            :value="model.participationName"
            :label="$t('lod.selfGovernment.participation')"
            outlined
            persistent-placeholder
          />
          <v-select
            v-else
            v-model="model.participationId"
            :items="participationOptions"
            :label="$t('lod.selfGovernment.participation')"
            clearable
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        sm="6"
        md="5"
      >
        <c-info
          uid="selfGovernment.position"
        >
          <v-text-field
            v-if="disabled"
            :value="model.positionName"
            :label="$t('lod.selfGovernment.position')"
            outlined
            persistent-placeholder
          />
          <v-select
            v-else
            v-model="model.positionId"
            :items="positionOptions"
            :label="$t('lod.selfGovernment.position')"
            clearable
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        v-if="model.participationId === 8"
        cols="12"
      >
        <c-info
          uid="selfGovernment.participationAdditionalInformation"
        >
          <v-textarea
            v-model="model.participationAdditionalInformation"
            :label="$t('lod.selfGovernment.participationAdditionalInformation')"
            prepend-icon="mdi-comment"
            outlined
            clearable
            rows="3"
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        sm="6"
      >
        <c-info
          uid="selfGovernment.mobilePhone"
        >
          <phone-field
            v-model="model.mobilePhone"
            clearable
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        sm="6"
      >
        <c-info
          uid="selfGovernment.email"
        >
          <email-field
            v-model="model.email"
            clearable
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
      >
        <c-info
          uid="selfGovernment.additionalInformation"
        >
          <v-textarea
            v-model="model.additionalInformation"
            :label="$t('common.additionalInformation')"
            prepend-icon="mdi-comment"
            outlined
            persistent-placeholder
            rows="3"
          />
        </c-info>
      </v-col>
    </v-row>
  </v-form>
</template>

<script>

import { StudentSelfGovernmentModel } from '@/models/studentSelfGovernmentModel.js';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import { mapGetters } from 'vuex';

export default {
  name: 'SelfGovernmentForm',
  components: {
    SchoolYearSelector
  },
  props: {
    value: {
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    isDetails: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      positionOptions: [],
      participationOptions: [],
      model: this.value ?? new StudentSelfGovernmentModel()
    };
  },
  computed: {
    ...mapGetters(['studentFinalizedLods', 'userInstitutionId'])
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getStudentSelfGovernmentPositions()
      .then((response) => {
        if(response.data) {
          this.positionOptions = response.data;
        }
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('errors.studentSelfGovernmentPositionOptionsLoad'));
        console.log(error.response);
      });

      this.$api.lookups.getStudentParticipations()
      .then((response) => {
        if(response.data) {
          this.participationOptions = response.data;
        }
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('errors.studentSelfGovernmentParticipationOptionsLoad'));
        console.log(error.response);
      });
    },
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    }
  }
};
</script>
