<template>
  <div>
    <grid
      v-if="schoolYearLoaded"
      :ref="'lodFinalizationListGrid' + _uid"
      url="/api/studentLod/list"
      :title="$t('lodFinalization.title')"
      :headers="headers"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{ personId: personId, schoolYear: gridFilters.schoolYear, institutionId: gridFilters.institutionId }"
    >
      <template
        v-if="!personId"
        #subtitle
      >
        <v-row dense>
          <v-col
            cols="12"
            sm="4"
            lg="2"
          >
            <school-year-selector
              v-model="gridFilters.schoolYear"
              show-current-school-year-button
              :show-navigation-buttons="false"
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            sm="8"
            lg="10"
          >
            <autocomplete
              v-model="gridFilters.institutionId"
              :defer-options-loading="false"
              api="/api/lookups/getInstitutionOptions"
              :label="$t('common.institution')"
              clearable
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`item.isApproved`]="{ item }">
        <v-chip
          :color="item.isApproved === true ? 'success' : 'light'"
          outlined
          small
        >
          <yes-no :value="item.isApproved" />
        </v-chip>
      </template>

      <template v-slot:[`item.isFinalized`]="{ item }">
        <v-chip
          :color="item.isFinalized === true ? 'success' : 'light'"
          outlined
          small
        >
          <yes-no :value="item.isFinalized" />
        </v-chip>
      </template>

      <template v-slot:[`item.pin`]="{ item }">
        {{ `${item.pin} - ${item.pinType}` }}
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="item.item.document && item.item.document.blobId"
            icon
            icon-name="mdi-file-word"
            icon-color="primary"
            :tooltip="item.item.document.noteFileName"
            bottom
            iclass=""
            small
            :href="`${item.item.document.blobServiceUrl}/${item.item.document.blobId}?t=${item.item.document.unixTimeSeconds}&h=${item.item.document.hmac}`"
          />
        </button-group>
        <lod-finalization
          v-if="hasManagePermission"
          class="mr-1"
          :school-year="item.item.schoolYear"
          :selected-students="generateStudent(item.item)"
          :can-be-undone="item.item.canBeUndone"
          @lodFinalizationEnded="lodFinalizationEnded"
        />
      </template>
    </grid>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';
import Constants from "@/common/constants.js";
import LodFinalization from '../lod/LodFinalization.vue';
import { LodFinalizationStudentModel } from '@/models/lodModels/lodFinalizationStudentModel.js';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';


export default {
  name: 'LodFinalizationList',
  components: {
    Grid,
    LodFinalization,
    Autocomplete,
    SchoolYearSelector
  },
  props: {
    personId: {
      type: Number,
      required: false,
      default() {
        return undefined;
      }
    },
  },
  data() {
    return {
      refKey: 'lodFInalizationList',
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      defaultGridFilter: {
        schoolYear: null,
        institutionId: null
      },
      schoolYearLoaded: false,
      headers: this.personId
        ?
       [
        {
          text: this.$t('lodFinalization.schoolYearName'),
          value: 'schoolYearName',
        },
        {
            text: this.$t('lodFinalization.institutionCode'),
            value: 'institutionId'
        },
        {
            text: this.$t('lodFinalization.institutionName'),
            value: 'institutionName'
        },
        {
            text: this.$t('student.headers.lodApproval'),
            value: 'isApproved'
        },
        {
            text: this.$t('student.headers.lodFinalization'),
            value: 'isFinalized'
        },
        {
          text: '',
          value: 'controls',
          filterable: false,
          sortable: false,
          align: 'end',
        },
      ]
      :  [
        {
          text: this.$t('lodFinalization.schoolYearName'),
          value: 'schoolYearName',
        },
        {
          text: this.$t('lodFinalization.personName'),
          value: 'fullName',
        },
        {
          text: this.$t('lodFinalization.pin'),
          value: 'pin',
        },
        {
            text: this.$t('lodFinalization.institutionCode'),
            value: 'institutionId'
        },
        {
            text: this.$t('lodFinalization.institutionName'),
            value: 'institutionName'
        },
        {
            text: this.$t('student.headers.lodApproval'),
            value: 'isApproved'
        },
        {
            text: this.$t('student.headers.lodFinalization'),
            value: 'isFinalized'
        },
        {
          text: '',
          value: 'controls',
          filterable: false,
          sortable: false,
          align: 'end',
        },
      ]
    };
  },
  computed: {
    ...mapGetters([
      'gridItemsPerPageOptions',
      'hasStudentPermission',
      'userInstitutionId'
    ]),
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForLodStateManage
      );
    },
    gridFilters: {
        get () {
          if (this.refKey in this.$store.state.gridFilters) {
            return this.$store.state.gridFilters[this.refKey] || {

            };
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
    if(!this.personId) {
      try {
        if(!this.gridFilters.schoolYear) {
          const currentSchoolYear = Number((await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data);
            if(!isNaN(currentSchoolYear)) {
              this.gridFilters.schoolYear = currentSchoolYear;
            }
        }
      } finally {
        this.schoolYearLoaded = true;
      }
    } else {
      this.schoolYearLoaded = true;
    }
  },
  methods: {
    init(){
      this.refresh();
    },
    generateStudent(item){
      return [new LodFinalizationStudentModel({
        personId: item.personId,
        fullName: '',
        isLodApproved: item.isApproved,
        isLodFinalized: item.isFinalized,
      })];
    },
    refresh() {
      const grid = this.$refs['lodFinalizationListGrid' + this._uid];
      if (grid) {
        grid.get();
      }
    },
    lodFinalizationEnded() {
      this.init();
    }
  },

};
</script>

