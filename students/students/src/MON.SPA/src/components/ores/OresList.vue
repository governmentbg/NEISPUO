<template>
  <div>
    <grid
      v-if="mounted"
      :ref="'oresListTable' + _uid"
      url="/api/ores/list"
      :headers="headers"
      :title="$t('ores.listTitle')"
      :filter="{
        personId: personId,
        classId: classId,
        institutionId: institutionId || gridFilters.institutionId,
        schoolYear: gridFilters.schoolYear,
        oresTypeId: gridFilters.oresTypeId,
        inheritanceType: gridFilters.inheritanceType,
        startDate: gridFilters.oresRange && gridFilters.oresRange.startDate ? $helper.parseDateToIso(gridFilters.oresRange.startDate) : '',
        endDate: gridFilters.oresRange && gridFilters.oresRange.endDate ? $helper.parseDateToIso(gridFilters.oresRange.endDate) : ''
      }"
      :ref-key="refKey"
      :debounce="1000"
      item-key="uid"
      :group-by="classId ? 'pin' : (personId ? 'schoolYearName' : 'pin')"
      multi-sort
      :file-export-name="$t('ores.listTitle')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    >
      <template #subtitle>
        <v-row dense>
          <v-col
            cols="12"
            sm="6"
            lg="2"
          >
            <school-year-selector
              v-model="gridFilters.schoolYear"
              show-current-school-year-button
              :show-navigation-buttons="false"
            />
          </v-col>
          <v-col
            cols="12"
            sm="6"
            lg="6"
          >
            <autocomplete
              v-model="gridFilters.oresTypeId"
              :defer-options-loading="false"
              api="/api/lookups/getORESTypesOptions"
              :label="$t('ores.oresKing')"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            sm="6"
            lg="2"
          >
            <c-select
              v-model="gridFilters.inheritanceType"
              :items="oresInheritanceTypeOptions"
              :label="$t('ores.oresType')"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            sm="6"
            lg="2"
          >
            <c-select
              v-model="gridFilters.oresRange"
              :items="oresRangeOptions"
              :label="$t('ores.headers.startEndDate')"
              return-object
              clearable
            />
          </v-col>
          <v-col
            v-if="!institutionId && !classId && !userInstitutionId"
          >
            <autocomplete
              v-model="gridFilters.institutionId"
              :defer-options-loading="false"
              api="/api/lookups/getInstitutionOptions"
              :label="$t('common.institution')"
              clearable
              :filter="{
                regionId: userRegionId
              }"
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`group.header`]="{ group, isOpen, toggle, remove }">
        <th :colspan="headers.length">
          <v-row dense>
            <v-icon
              @click="toggle"
            >
              {{ isOpen ? 'mdi-chevron-up' : 'mdi-chevron-down' }}
            </v-icon>
            <span class="text-h6 ml-3">{{ group }}</span>
            <v-btn
              icon
              color="secondary"
              class="ml-3"
              @click.stop="remove"
            >
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </v-row>
        </th>
      </template>

      <template v-slot:[`item.pin`]="{ item }">
        {{ `${item.pin} - ${item.pinTypeName}` }}
      </template>

      <template v-slot:[`item.startDate`]="{ item }">
        {{ item.startDate ? $moment(item.startDate).format(dateFormat) : '' }}
        - {{ item.endDate ? $moment(item.endDate).format(dateFormat) : '' }}
      </template>

      <template v-slot:[`item.isInheritedFromClass`]="{ item }">
        <v-tooltip
          v-if="item.isInheritedFromClass === true"
          bottom
        >
          <template v-slot:activator="{ on }">
            <v-icon
              color="secodary"
              small
              v-on="on"
            >
              mdi-google-classroom
            </v-icon>
          </template>
          <span>{{ $t('ores.inheritanceToolttip', {
            name: item.className,
            startDate: item.startDate ? $moment(item.startDate).format(dateFormat) : '',
            endDate: item.endDate ? $moment(item.endDate).format(dateFormat) : ''
          }) }}</span>
        </v-tooltip>
        <v-tooltip
          v-if="item.isInheritedFromInstitution === true"
          bottom
        >
          <template v-slot:activator="{ on }">
            <v-icon
              color="secodary"
              small
              v-on="on"
            >
              fa-university
            </v-icon>
          </template>
          <span>{{ $t('ores.inheritanceToolttip', {
            name: item.institutionName,
            startDate: item.startDate ? $moment(item.startDate).format(dateFormat) : '',
            endDate: item.endDate ? $moment(item.endDate).format(dateFormat) : ''
          }) }}</span>
        </v-tooltip>
      </template>

      <template #footerPrepend>
        <v-btn
          v-if="hasManagePermission"
          small
          color="primary"
          :to="oresCreateNavigationLink"
        >
          {{ $t("buttons.newRecord") }}
        </v-btn>
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.details"
            bottom
            iclass=""
            small
            :to="personId ? `/student/${personId}/ores/${item.item.id}/details` : `/ores/${item.item.id}/details`"
          />
          <button-tip
            v-if="hasManagePermission && item.item.hasManagePermission === true"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            :to="personId ? `/student/${personId}/ores/${item.item.id}/edit` : `/ores/${item.item.id}/edit`"
          />
          <button-tip
            v-if="hasManagePermission && item.item.hasManagePermission === true"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="deleteOres(item.item)"
          />
        </button-group>
      </template>
    </grid>
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="deleteConfirm" />
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import Constants from '@/common/constants.js';
import { mapGetters } from 'vuex';
import { Permissions, UserRole } from '@/enums/enums';


export default {
  name: 'OresListComponent',
  components: { Grid, SchoolYearSelector, Autocomplete },
  props: {
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
    classId: {
      type: Number,
      default() {
        return null;
      }
    },
    institutionId: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      saving: false,
      showDialog: false,
      mounted: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      oresInheritanceTypeOptions: Constants.OresInheritanceTypeOptions,
      defaultGridFilters: {},
      oresRangeOptions: null,
      refKey: this.personId
        ? 'studentOresList'
        : (this.classId
          ? 'classOresList'
          : 'oresList'
        ),
    };
  },
  computed: {
    ...mapGetters(['turnOnOresModule', 'hasPermission', 'isInRole', 'mode', 'userRegionId', 'userInstitutionId']),
    hasReadPermission() {
      return this.turnOnOresModule && this.hasPermission(Permissions.PermissionNameForOresRead);
    },
    hasManagePermission() {
      return this.turnOnOresModule && this.hasPermission(Permissions.PermissionNameForOresManage);
    },
    oresCreateNavigationLink() {
      if (this.personId) {
        return `/student/${this.personId}/ores/create`;
      }

      if (this.classId) {
        return `/ores/create?classId=${this.classId}`;
      }

      if (this.institutionId) {
        return `/ores/create?institutionId=${this.institutionId}`;
      }

      return '';
    },
    headers() {
      const headers = [];
      if (this.mode !== 'prod' || this.isInRole(UserRole.Consortium)) {
        headers.push({
            text: '',
            value: "isInheritedFromClass",
            groupable: false,
            filterable: false,
            sortable: false
          }
        );
      }

      if (!this.userInstitutionId) {
        // За роли, които не са институции или РУО нека колоните да бъдат:
        // "Област", "Община", "Населено място", "Код", "Институция"

        //За РУО:
        //"Община", "Населено място", "Код", "Институция"
          if (!this.userRegionId) {
            headers.push({
              text: this.$t("ores.headers.region"),
              value: "regionName",
            });
          }

          headers.push({
            text: this.$t("ores.headers.municipality"),
            value: "municipalityName",
          });

          headers.push({
            text: this.$t("ores.headers.town"),
            value: "townName",
          });

          headers.push({
            text: this.$t("ores.headers.institutionCode"),
            value: "institutionId",
          });

          headers.push({
            text: this.$t("ores.headers.institution"),
            value: "institutionName",
          });
      }

      return [...headers, ...[
        {
          text: this.$t("ores.headers.name"),
          value: "fullName",
        },
        {
          text: this.$t("ores.headers.pin"),
          value: "pin",
        },
        {
          text: this.$t("ores.headers.basicClass"),
          value: "basicClassName",
        },
        {
          text: this.$t("ores.headers.class"),
          value: "className",
        },
        {
          text: this.$t("ores.headers.type"),
          value: "oresTypeName",
        },
        {
          text: this.$t("ores.headers.startEndDate"),
          value: "startDate",
        },
        {
          text: this.$t("ores.headers.calendarDays"),
          value: "personOresCalendarDaysCount",
          groupable: false,
        },
        {
          text: this.$t("ores.headers.workDays"),
          value: "personOresWorkDaysCount",
          groupable: false,
        },
        {
          text: "",
          value: "controls",
          groupable: false,
          filterable: false,
          sortable: false,
          align: "end",
        },
      ]];
    },
    gridFilters: {
      get () {
        if (this.refKey in this.$store.state.gridFilters) {
          return this.$store.state.gridFilters[this.refKey] || {};
        }
        else {
          return this.defaultGridFilters;
        }
      },
      set (value) {
        if (this.refKey in this.$store.state.gridFilters) {
          this.$store.commit('updateGridFilter', { options: value, refKey: this.refKey });
        }
        else {
          return this.defaultGridFilters = value;
        }
      }
    },
  },
  async mounted() {
    try {
      if(!this.gridFilters.schoolYear) {
        const currentSchoolYear = await this.getCurrentSchoolYear();
        if(!isNaN(currentSchoolYear)) {
          this.gridFilters.schoolYear = currentSchoolYear;
        }
      }

    } finally {
      this.mounted = true;
    }

    this.getOresRangeOptions();
  },
  methods: {
    getOresRangeOptions() {
      this.$api.ores.getOresRangeDropdownOptions(this.institutionId, this.classId, this.personId, this.gridFilters.schoolYear)
      .then(response => {
        this.oresRangeOptions = response?.data;
      })
      .catch(error => {
        console.log(error.response);
      });
    },
    gridReload() {
      const grid = this.$refs['oresListTable' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async deleteOres(item) {
      const promptMessages = item.isInheritedFromClass
        ? this.$t('ores.deletePrompt', { name: this.$t('student.class').toLocaleLowerCase() })
        : item.isInheritedFromInstitution
          ? this.$t('ores.deletePrompt', { name: this.$t('common.institution').toLocaleLowerCase() })
          : this.$t('common.confirm');

      if(await this.$refs.confirm.open(this.$t('buttons.delete'), promptMessages)){
        this.saving = true;
        this.$api.ores.delete(item.id)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'), 5000);
            this.gridReload();
            this.$emit('oresDelete');
          })
          .catch(error => {
            this.$notifier.error('', this.$t('common.deleteError'), 5000);
            console.error(error.response);
          })
          .then(() => { this.saving = false; });
      }
    },
    async getCurrentSchoolYear() {
      const currentSchoolYear = Number((await this.$api.institution.getCurrentYear())?.data);
      return currentSchoolYear;
    }
  }
};
</script>

