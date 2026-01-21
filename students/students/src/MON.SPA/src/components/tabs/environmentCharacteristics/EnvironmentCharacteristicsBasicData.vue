<template>
  <v-card>
    <v-card-text>
      <v-card>
        <v-card-title>
          <v-icon left>
            fa-user-md
          </v-icon>
          {{ $t('environmentCharacteristics.studentGp') }}
        </v-card-title>
        <v-card-text>
          <v-row dense>
            <v-col
              cols="12"
              md="6"
              sm="12"
            >
              <v-text-field
                v-model="form.gpName"
                :label=" $t('studentTabs.lastName' )"
                :disabled="!isEditMode"
                autocomplete="gpName"
              />
            </v-col>
            <v-col
              cols="12"
              md="6"
              sm="12"
            >
              <v-text-field
                v-model="form.gpPhone"
                :label="$t('studentTabs.phoneNumber')"
                :disabled="!isEditMode"
                autocomplete="gpPhone"
                prepend-icon="fa-phone-alt"
              />
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <v-row dense>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-checkbox
            v-model="form.hasParentConsent"
            :label=" $t('environmentCharacteristics.parentsConsentedToSubmitData') "
            color="primary"
            hide-details
            :disabled="!isEditMode"
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-checkbox
            v-model="form.representedByTheMajor"
            :label=" $t('environmentCharacteristics.representedByTheMajor') "
            color="primary"
            hide-details
            :disabled="!isEditMode"
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-checkbox
            v-model="form.livesWithFosterFamily"
            :label="$t('environmentCharacteristics.studentLivesInHospice') "
            color="primary"
            hide-details
            :disabled="!isEditMode"
          />
        </v-col>
        <v-col
          v-if="!form.hasParentConsent"
          cols="12"
          class="red--text font-weight-bold text-h6"
        >
          <span>
            {{ $t('environmentCharacteristics.noEnvironmentCharacteristicsForTheStudent') }}
          </span>
        </v-col>
      </v-row>

      <div
        v-if="form.hasParentConsent"
      >
        <v-row dense>
          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <autocomplete
              v-model="form.nativeLanguage"
              api="/api/lookups/GetLanguages"
              :label="$t('environmentCharacteristics.environmentLanguage')"
              :disabled="!isEditMode"
              clearable
            />
          </v-col>
          <v-col
            align-self="center"
            cols="12"
            md="4"
            sm="6"
            align
          >
            <span class="mx-5 font-weight-bold text-h6">{{ this.$t('environmentCharacteristics.familyWorkStatusWeight') }} : <v-chip>{{ form.familyWorkStatusWeight }}</v-chip></span>
          </v-col>
          <v-col
            align-self="center"
            cols="12"
            md="4"
            sm="6"
          >
            <span class="mx-5 font-weight-bold text-h6">{{ this.$t('environmentCharacteristics.familyEducationWeight') }}  :   <v-chip>{{ form.familyEducationWeight }}</v-chip> </span>
          </v-col>
        </v-row>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { EnvironmentCharacteristicsModel } from "@/models/environmentCharacteristics/environmentCharacteristicsModel.js";

export default {
       name: "EnvironmentCharacteristicsBasicData",
       components: { Autocomplete },
        props: {
            personId: {
                type: Number,
                required: true
            },
            form:{
                type: EnvironmentCharacteristicsModel,
                required: true
            },
            isEditMode:{
                type: Boolean,
                required: false
            },
            saving:{
                type: Boolean,
                required: true
            }
        }
};
</script>
