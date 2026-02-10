<template>
  <v-card
    v-if="value"
  >
    <v-card-title>{{ value.message }}</v-card-title>
    <v-card-subtitle>
      {{ value.date ? $moment(value.date).format(dateTimeFormat) : '' }}
    </v-card-subtitle>
    <v-card-text>
      <v-simple-table dense>
        <template v-slot:default>
          <tbody>
            <tr>
              <td>{{ $t('profile.apiErrorDate') }}</td>
              <td>{{ value.date }}</td>
            </tr>
            <tr>
              <td>{{ $t('profile.apiErrorMessage') }}</td>
              <td>{{ value.message }}</td>
            </tr>
            <!-- <tr>
              <td>isError</td>
              <td>{{ value.isError }}</td>
            </tr> -->
            <tr>
              <td>{{ $t('profile.apiErrorDetails') }}</td>
              <td>{{ value.detail }}</td>
            </tr>
            <tr>
              <td>{{ $t('profile.apiErrorData') }}</td>
              <td>{{ value.data }}</td>
            </tr>
            <tr
              v-if="errors && errors.length > 0"
            >
              <td>{{ $t('profile.apiErrorList') }}</td>
              <td>
                <v-expansion-panels
                  flat
                >
                  <v-expansion-panel>
                    <v-expansion-panel-header>
                      <v-chip
                        label
                      >
                        <v-icon left>
                          mdi-alert
                        </v-icon>
                        {{ errors.length }}
                      </v-chip>
                    </v-expansion-panel-header>
                    <v-expansion-panel-content>
                      <v-row
                        dense
                      >
                        <v-col
                          v-if="errorControlIdOptions && errorControlIdOptions.length > 0"
                        >
                          <v-select
                            v-model="selectedErrorControlId"
                            :items="errorControlIdOptions"
                            clearable
                          />
                        </v-col>
                        <v-col
                          v-if="errorIdOptions && errorIdOptions.length > 0"
                        >
                          <v-select
                            v-model="selectedErrorId"
                            :items="errorIdOptions"
                            clearable
                          />
                        </v-col>
                      </v-row>

                      <vue-json-pretty
                        v-if="filteredErrors.length < 100"
                        path="res"
                        :data="filteredErrors"
                        show-length
                      />
                      <v-data-iterator
                        v-else
                        :items="filteredErrors"
                        :search="search"
                        :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions, showCurrentPage: true, showFirstLastPage: true }"
                      >
                        <template #header>
                          <v-row dense>
                            <v-spacer />
                            <v-text-field
                              v-model="search"
                              append-icon="mdi-magnify"
                              :label="$t('buttons.search')"
                              clearable
                              single-line
                              hide-details
                              dense
                            />
                          </v-row>
                        </template>
                        <template
                          #default="props"
                        >
                          <vue-json-pretty
                            v-for="(item, index) in props.items"
                            :key="index"
                            path="res"
                            :data="item"
                            show-length
                          />
                        </template>
                      </v-data-iterator>
                    </v-expansion-panel-content>
                  </v-expansion-panel>
                </v-expansion-panels>
              </td>
            </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-card-text>
  </v-card>
</template>

<script>
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';
import Constants from "@/common/constants.js";
import { mapGetters } from 'vuex';

export default {
  name: 'ApiErrorDetails',
  components: {
    VueJsonPretty
  },
  props: {
    value: {
      type: Object,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      selectedErrorControlId: undefined,
      selectedErrorId: undefined,
      search: '',
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions']),
    errors() {
      if(!this.value) {
        return [];
      }

      if(!this.value.errors || !Array.isArray(this.value.errors)) {
        return [{...this.value}];
      }

      const errors = this.value.errors.map(x => {
        return {
          message: x.message,
          controlID: x.controlID,
          id: x.id,
          data: this.$helper.isJsonParsable(x.data) ? JSON.parse(x.data) : x.data
        };
      });

      return errors;
    },
    filteredErrors() {
      if(!this.errors) {
        return [];
      }

      let filteredErrors = [...this.errors];
      if(this.selectedErrorControlId) {
        filteredErrors =  filteredErrors.filter(x => x.controlID === this.selectedErrorControlId);
      }

      if(this.selectedErrorId) {
        filteredErrors = filteredErrors.filter(x => x.id === this.selectedErrorId);
      }

      return filteredErrors;
    },
    errorControlIdOptions() {
      if(!this.errors || !Array.isArray(this.errors)) return [];

      const set = new Set();
      this.errors.forEach(error => {
        if('controlID' in error && error.controlID) {
          set.add(error.controlID);
        }
      });

      const arr = [];
      set.forEach(x => {
        arr.push({
          value: x,
          text: x
        });
      });

      return arr;
    },
    errorIdOptions() {
      if(!this.errors || !Array.isArray(this.errors)) return [];
      const set = new Set();
      this.errors.forEach(error => {
        if('id' in error && error.id) {
          set.add(error.id);
        }
      });

      const arr = [];
      set.forEach(x => {
        arr.push({
          value: x,
          text: x
        });
      });

      return arr;
    }
  }
};
</script>
