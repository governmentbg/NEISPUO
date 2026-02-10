<template>
  <grid
    url="/api/ores/list"
    :headers="headers"
    :filter="{
      oresId: oresId
    }"
    title=""
    item-key="uid"
    multi-sort
    :file-export-name="$t('ores.listTitle')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
  >
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
  </grid>
</template>

<script>
import Grid from "@/components/wrappers/grid";
import { mapGetters } from 'vuex';
import Constants from '@/common/constants.js';
import { Permissions, UserRole } from '@/enums/enums';

export default {
  name: 'OresStudentsList',
  components: {
    Grid
  },
  props: {
    oresId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      dateFormat: Constants.DATEPICKER_FORMAT,
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'turnOnOresModule', 'mode', 'isInRole']),
    hasReadPermission() {
      return this.turnOnOresModule && this.hasPermission(Permissions.PermissionNameForOresRead);
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
        }
      ]];
    }
  },
  mounted() {
    if(!this.hasReadPermission) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
};
</script>
