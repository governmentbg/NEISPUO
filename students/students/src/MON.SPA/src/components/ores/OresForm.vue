<template>
  <div>
    <v-card-subtitle
      class="pa-0 mb-4"
    >
      <v-row dense>
        <v-col>
          <div v-if="personDetails">
            <v-chip
              color="light"
            >
              {{ `${personDetails.firstName} ${personDetails.middleName} ${personDetails.lastName}` }}
            </v-chip>
          </div>
          <div v-if="classDetails">
            <v-chip
              color="light"
            >
              {{ classDetails.className }}
            </v-chip>
          </div>
          <div v-if="institutionDetails">
            <v-chip
              color="light"
            >
              {{ institutionDetails.text }}
            </v-chip>
          </div>
        </v-col>
        <v-spacer />
        <v-col
          v-if="!disabled"
          class="text-right"
        >
          <button-group>
            <button-tip
              color="light"
              tooltip="common.currentMonthTooltip"
              text="common.currentMonth"
              bottom
              iclass=""
              small
              @click="setCurrentMonthDates"
            />
            <button-tip
              color="light"
              tooltip="common.currentWeekTooltip"
              text="common.currentWeek"
              bottom
              iclass=""
              small
              @click="setCurrentWeekDates"
            />
          </button-group>
        </v-col>
      </v-row>
    </v-card-subtitle>
    <v-form
      :ref="'oresForm' + _uid"
      :disabled="disabled"
    >
      <v-row
        dense
      >
        <v-col
          cols="12"
          md="6"
          dense
        >
          <c-info
            uid="ores.oresType"
          >
            <c-text-field
              v-if="disabled"
              :label="$t('ores.oresType')"
              :value="model.oresTypeName"
              outlined
              persistent-placeholder
            />

            <c-select
              v-else
              v-model="model.oresTypeId"
              :items="oresTypeOptions"
              :label="$t('ores.oresType')"
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
          md="6"
          lg="3"
        >
          <c-info
            uid="ores.startDate"
          >
            <date-picker
              id="startDate"
              ref="startDate"
              v-model="model.startDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('ores.startDate')"
              clearable
              :max="model.endDate"
              :rules="[$validator.required()]"
              :class="!disabled ? 'required' : ''"
              outlined
              persistent-placeholder
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="6"
          lg="3"
        >
          <c-info
            uid="ores.endDate"
          >
            <date-picker
              id="endDate"
              ref="endDate"
              v-model="model.endDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('ores.endDate')"
              clearable
              :min="model.startDate"
              :rules="[$validator.required()]"
              :class="!disabled ? 'required' : ''"
              outlined
              persistent-placeholder
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
        >
          <c-textarea
            v-model="model.description"
            :label="$t('ores.description')"
            outlined
            dense
            persistent-placeholder
          />
        </v-col>
      </v-row>
    </v-form>
    <file-manager
      v-if="!disabled || (model.documents && model.documents.length > 0)"
      v-model="model.documents"
      :disabled="disabled"
      class="mt-3"
    />
  </div>
</template>

<script>
import FileManager from '@/components/common/FileManager.vue';
import Constants from '@/common/constants.js';

export default {
  name: 'OresFormComponent',
  components: {
    FileManager
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
    return {
      model: this.value,
      oresTypeOptions: null,
      personDetails: null,
      classDetails: null,
      institutionDetails: null,
    };
  },
  watch: {
    value() {
      this.model = this.value;
      this.loadDetails();
    }
  },
  mounted() {
    this.loadOresTypeOptions();
    this.loadDetails();
  },
  methods: {
    loadOresTypeOptions() {
      this.$api.lookups.getORESTypesOptions()
      .then((response) => {
        this.oresTypeOptions = response.data ?? [];
      })
      .catch((error) => {
        console.log(error.response);
      });
    },
    loadDetails() {
      if(this.model) {
        if(this.model.personId) {
          this.loadPersonDetails(this.model.personId);
        } else {
          this.personDetails = null;
        }

        if(this.model.classId) {
          this.loadClassDetails(this.model.classId);
        } else {
          this.classDetails = null;
        }

        if(this.model.institutionId) {
          this.loadInstitutionDetails(this.model.institutionId);
        } else {
          this.institutionDetails = null;
        }
      }
    },
    loadPersonDetails(personId){
      this.$api.student.getPersonDataById(personId)
        .then((response) => {
          this.personDetails = response?.data;
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    loadClassDetails(classId){
      this.$api.classGroup.getById(classId)
        .then((response) => {
          this.classDetails = response?.data;
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    loadInstitutionDetails(institutionId){
      this.$api.institution.getDropdownModelById(institutionId)
        .then((response) => {
          this.institutionDetails = response?.data;
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    validate() {
      const form = this.$refs['oresForm' + this._uid];
      return form ? form.validate() : false;
    },
    setCurrentMonthDates() {
      if(!this.model) return;

      const date = new Date();
      const firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
      const lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
      this.model.startDate = this.$moment(firstDay).format(Constants.DATEPICKER_FORMAT);
      this.model.endDate = this.$moment(lastDay).format(Constants.DATEPICKER_FORMAT);
    },
    setCurrentWeekDates() {
      if(!this.model) return;

      const date = new Date();
      const day = date.getDay();
      const diff = date.getDate() - day + (day === 0 ? -6 : 1);
      const firstDay = new Date(date.setDate(diff));

      const lastDay = new Date(firstDay);
      lastDay.setDate(lastDay.getDate() + 6);

      this.model.startDate = this.$moment(firstDay).format(Constants.DATEPICKER_FORMAT);
      this.model.endDate = this.$moment(lastDay).format(Constants.DATEPICKER_FORMAT);
    }
  }
};
</script>
