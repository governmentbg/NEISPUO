<template>
  <div>
    <v-card v-if="institution">
      <v-card-text>
        <v-row dense>
          <v-col
            v-if="institution"
            cols="12"
          >
            <v-icon left>
              mdi-town-hall
            </v-icon><v-chip>{{ institution.institutionId }}</v-chip> {{
              `${$t("account.institution", {
                institution: institution.institutionName,
              })}${"" ? ` / ${""}` : ""}`
            }}
          </v-col>
          <v-col
            v-if="institution.address"
            cols="12"
          >
            <v-icon left>
              mdi-map-marker-outline
            </v-icon>{{ $t("account.address", { address: institution.address }) }}
            <v-chip
              v-for="department in institution.departments"
              :key="department.departmentId"
              color="text--secondary"
              outlined
            >
              {{ department.name }} {{ department.address }}
            </v-chip>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import Constants from "@/common/constants.js";
import {VChip} from "vuetify/lib";
//import InstitutionModel from "@/models/institutionModel";

export default {
  name: 'InstitutionDetails',
  components: {VChip},
  props: {
    institution: {
      type: Object,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      dateTimeFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`
    };
  }
};
</script>
