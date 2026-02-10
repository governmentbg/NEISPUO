<template>
  <v-card>
    <v-card-title>
      {{ $t('enroll.additionalData') }}
    </v-card-title>
    <v-card-text>
      <v-row dense>
        <v-col
          cols="12"
          sm="6"
          md="4"
        >
          <v-select
            id="commuterTypeId"
            v-model="model.commuterTypeId"
            :items="commuterOptions"
            :label="$t('enroll.traveling')"
            :rules="[$validator.required()]"
            name="commuterType"
            item-text="text"
            item-value="value"
            class="required"
            clearable
          />
        </v-col>
        <v-col
          v-if="!isKindergartenInstitution"
          cols="12"
          sm="6"
          md="4"
        >
          <v-select
            id="repeaterId"
            v-model="model.repeaterId"
            :items="repeaterOptions"
            :label="$t('enroll.reEnter')"
            :rules="[$validator.required()]"
            name="repeater"
            item-text="text"
            item-value="value"
            class="required"
            clearable
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
        >
          <v-select
            id="oresTypeId"
            v-model="model.oresTypeId"
            :items="oresTypesOptions"
            :label="$t('enroll.oresTypes')"
            name="oresType"
            item-text="text"
            item-value="value"
            clearable
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
        >
          <v-checkbox
            v-model="model.hasIndividualStudyPlan"
            :label="$t('enroll.individualCurriculum')"
            color="primary"
          />
        </v-col>
        <v-col
          v-if="isKindergartenInstitution"
          cols="12"
          sm="6"
          md="4"
        >
          <v-checkbox
            v-model="model.isHourlyOrganization"
            :label="$t('enroll.hourlyOrganization')"
            color="primary"
          />
        </v-col>
        <v-col
          v-if="!isKindergartenInstitution"
          cols="12"
          sm="6"
          md="4"
        >
          <v-checkbox
            v-model="model.isNotForSubmissionToNra"
            :label="$t('enroll.nraNotSubmitted')"
            color="primary"
          />
        </v-col>
        <v-col
          v-if="showIsFTACOutsorced"
          cols="12"
          sm="6"
          md="4"
        >
          <v-checkbox
            v-model="model.IsFTACOutsourced "
            :label="$t('enroll.FTACOutsourced')"
            color="primary"
          />
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
import Constants from '@/common/constants';
import { InstType } from "@/enums/enums";
import { mapGetters } from 'vuex';

export default {
  name: 'StudentClassAdditionalData',
  props: {
    value: {
      type: Object,
      required: true
    },
    baseSchoolTypeId: {
      type: Number,
      default() {
        return undefined;
      }
    },
      instTypeId: {
      type: Number,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      model: this.value,
      commuterOptions: [],
      repeaterOptions: [],
      oresTypesOptions: []
    };
  },
  computed: {
    ...mapGetters(['isInstType']),
    isKindergartenInstitution() {
      return this.isInstType(InstType.Kindergarten);
    },
    showIsFTACOutsorced(){
      return this.instTypeId === Constants.FTACOutsorcedInstitutionTypeId;
    }
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups
        .getCommuterOptions()
        .then((response) => {
        this.commuterOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.commuterOptionsLoad'));
          console.log(error);
        });

      this.$api.lookups.getRepeaterReasons()
        .then((response) => {
          this.repeaterOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.repeaterOptionsLoad'));
          console.log(error);
        });

      this.$api.lookups.getORESTypesOptions()
        .then((response) => {
          this.oresTypesOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.oresTypesOptionsLoad'));
          console.log(error);
        });
    }
  }
};
</script>
