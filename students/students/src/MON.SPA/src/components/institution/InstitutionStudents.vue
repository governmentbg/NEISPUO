<template>
  <div>
    <grid
      url="/api/institution/StudentsList"
      :headers="selectedHeaders"
      :filter="{
        institutionId: institutionId,
        selectedClasses: gridFilters.selectedClassesFilter ? gridFilters.selectedClassesFilter.join('|') : ''
      }"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :title="$t('institution.students')"
      :file-export-name="$t('institution.students')"
      :ref-key="refKey"
      :debounce="1000"
      :group-by="!gridFilters.selectedClassesFilter || gridFilters.selectedClassesFilter.length > 1 ? 'className' : []"
    >
      <template #subtitle>
        <v-autocomplete
          v-model="gridFilters.selectedClassesFilter"
          :label="$t('enroll.class')"
          item-text="text"
          item-value="value"
          :items="classGroupsOptions"
          multiple
          chips
          deletable-chips
          clearable
        />
      </template>

      <template v-slot:[`item.pin`]="{ item }">
        {{ `${item.pin} - ${item.pinType}` }}
      </template>

      <template v-slot:[`item.hasSpecialNeeds`]="{ item }">
        <v-btn
          :color="item.hasSpecialNeeds === true ? 'success' : 'light'"
          outlined
          rounded
          x-small
          :to="`/student/${item.personId}/sop`"
        >
          <yes-no :value="item.hasSpecialNeeds" />
        </v-btn>
      </template>

      <template v-slot:[`item.resourceSupportStatus`]="{ item }">
        <v-btn
          :color="item.resourceSupportStatus === 2 ? 'error' : ( item.resourceSupportStatus === 1 ? 'success' : 'light')"
          outlined
          rounded
          x-small
          :to="`/student/${item.personId}/resourceSupports`"
        >
          <span v-if="item.resourceSupportStatus === 0">{{ $t('common.no') }}</span>
          <span v-else-if="item.resourceSupportStatus === 1">{{ $t('common.yes') }}</span>
          <span v-else-if="item.resourceSupportStatus === 2">{{ $t('resourceSupport.suspended') }}</span>
        </v-btn>
      </template>

      <template v-slot:[`group.header`]="{ group, toggle, isOpen }">
        <td :colspan="headers.length">
          <v-btn
            :ref="group"
            x-small
            icon
            @click="toggle"
          >
            <v-icon v-if="isOpen">
              mdi-chevron-up
            </v-icon>
            <v-icon v-else>
              mdi-chevron-down
            </v-icon>
          </v-btn>
          <span class="mx-5 font-weight-bold">{{ group }}</span>
        </td>
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.details"
            bottom
            iclass=""
            small
            :to="`/student/${item.item.personId}/details`"
          />
        </button-group>
      </template>
    </grid>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import { mapGetters } from 'vuex';
import { InstType } from '@/enums/enums';

export default {
  name: 'InstitutionStudents',
  components: { Grid },
  props:{
    institutionId: {
      type: Number,
      required: true
    },
  },
  data(){
    return {
      dateFormat: Constants.DATEPICKER_FORMAT,
      defaultGridFilters: {},
      classGroupsOptions: [],
      refKey: 'institutionStudentslList',
      headers: [
        {
          text: this.$t('institutionStudents.headers.name'),
          value: 'fullName',
        },
        {
          text: this.$t('institutionStudents.headers.identifier'),
          value: 'pin'
        },
        {
          text: this.$t('institutionStudents.headers.class'),
          value: 'className'
        },
        {
          text: this.$t('institutionStudents.headers.basicClass'),
          value: 'basicClassName'
        },
        {
          text: this.$t('institutionStudents.headers.position'),
          value: 'position',
        },
        {
          text: this.$t('institutionStudents.headers.classType'),
          value: 'classTypeName'
        },
        {
          text: this.$t('institutionStudents.headers.profession'),
          value: 'profession'
        },
        {
          text: this.$t('institutionStudents.headers.speciality'),
          value: 'speciality'
        },
        {
          text: this.$t('institutionStudents.headers.eduForm'),
          value: 'eduFormName'
        },
        {
          text: this.$t('institutionStudents.headers.mainClassName'),
          value: 'mainClassName',
        },
        {
          text: this.$t('institutionStudents.headers.hasSpecialNeeds'),
          value: 'hasSpecialNeeds'
        },
        {
          text: this.$t('institutionStudents.headers.hasResourceSupport'),
          value: 'resourceSupportStatus'
        },
        { text: '', value: 'controls', sortable: false, groupable: false, align: 'end' },
      ]
    };
  },
  computed: {
    ...mapGetters(['userDetails', 'userInstitutionId', 'isInstType']),
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
    selectedHeaders() {
      if(this.isInstType(InstType.School) || this.isInstType(InstType.Kindergarten)) {
        return this.headers;
      } else {
        return this.headers.filter(x => x.value !== 'mainClassName');
      }
    }
  },
  async mounted() {
    if (!this.userInstitutionId) {
      // Не е институция. Трием запазания state на dropdown-a с паралелките
      this.gridFilters = this.defaultGridFilters;
    }
    await this.loadClassGroupsOptions();
  },
  methods: {
    async loadClassGroupsOptions() {
      const schoolYear = Number((await this.$api.institution.getCurrentYear(this.institutionId))?.data);
      this.$api.lookups
          .getClassGroupsOptions(
            this.institutionId,
            schoolYear,
            undefined,
            undefined,
            undefined,
            undefined,
            true
          )
        .then((response) => {
          this.classGroupsOptions = response.data;
      })
        .catch((error) => {
          console.log(error);
        });
    }
  }
};
</script>
