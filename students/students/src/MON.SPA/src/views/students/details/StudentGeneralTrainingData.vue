<template>
  <grid
    url="/api/studentLOD/generalTrainingDataList"
    file-export-name="StudentGeneralTrainingData"
    :headers="headers"
    :title="$t('generalTrainingData.title')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    :filter="{ studentId: id }"
  >
    <template v-slot:[`item.institution`]="props">
      {{ `${props.item.institutionId} - ${props.item.institution}` }}
    </template>

    <template v-slot:[`item.admissionDate`]="props">
      {{ props.item.admissionDate ? $moment.utc(props.item.admissionDate).local().format(dateFormat) : '' }}
    </template>

    <template v-slot:[`item.dischargeDate`]="props">
      {{ props.item.dischargeDate ? $moment.utc(props.item.dischargeDate).local().format(dateFormat) : '' }}
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
          :to="`/student/${id}/generalTrainingData/${item.item.institutionId}/details?classId=${item.item.id}`"
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentGeneralTrainingData',
  components: {
    Grid
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      headers: [
        {text: this.$t('generalTrainingData.headers.schoolYear'), value: "schoolYearName", sortable: true},
        {text: this.$t('generalTrainingData.headers.institution'), value: "institution", sortable: true},
        {text: this.$t('generalTrainingData.headers.class'), value: "className", sortable: true},
        {text: this.$t('generalTrainingData.headers.classType'), value: "classTypeName", sortable: true},
        {text: this.$t('generalTrainingData.headers.eduForm'), value: "classEduFormName", sortable: true},
        {text: this.$t('generalTrainingData.headers.position'), value: "position", sortable: true},
        {text: this.$t('generalTrainingData.headers.admissionDate'), value: "admissionDate", sortable: true},
        {text: this.$t('generalTrainingData.headers.dischargeDate'), value: "dischargeDate", sortable: true},
        {text: '', value: "controls", sortable: false, align: 'end'},
      ],
      dateFormat: Constants.DATEPICKER_FORMAT,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission'])
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentGeneralTrainingDataRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  methods: {}
};
</script>
