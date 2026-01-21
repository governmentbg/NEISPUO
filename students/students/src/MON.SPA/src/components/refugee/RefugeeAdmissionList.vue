<template>
  <grid
    :ref="'refugeeAdmissionListGrid' + _uid"
    url="/api/refugee/AdmissionList"
    :headers="headers"
    :title="''"
    :file-export-name="$t('refugee.admissionDetails.listTitle')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    item-key="personId"
  >
    <template v-slot:[`item.admissionStatus`]="props">
      <v-chip
        :color="props.item.admissionStatus === 'Да' ? 'success': 'warning'"
        small
      >
        {{ props.item.admissionStatus }}
      </v-chip>
    </template>

    <template v-slot:[`item.ruodocDate`]="{ item }">
      {{ item.ruodocDate ? $moment(item.ruodocDate).format(dateFormat) : "" }}
    </template>

    <template v-slot:[`item.pin`]="{ item }">
      {{ `${item.pin} - ${item.pinType}` }}
    </template>

    <template #actions="item">
      <button-group>
        <button-tip
          :to="`/student/${item.item.personId}/details`"
          icon
          icon-color="primary"
          iclass=""
          icon-name="mdi-eye"
          small
          tooltip="student.details"
          bottom
        />
        <button-tip
          :to="`/refugee/application/${item.item.applicationId}/details`"
          icon
          icon-color="primary"
          iclass=""
          icon-name="mdi-umbrella"
          small
          tooltip="refugee.details"
          bottom
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import { mapGetters } from 'vuex';
import Constants from "@/common/constants.js";

export default {
  name: 'RefugeeAdmissionList',
  components: {
    Grid
  },
  data() {
    return {
      dateFormat: Constants.DATE_FORMAT,
      headers: [
        {
          text: this.$t("refugee.headers.firstName"),
          value: "fullName",
        },
        {
          text: this.$t("refugee.headers.personalIdTypeName"),
          value: "pin",
        },
        {
          text: this.$t("refugee.headers.institutionCode"),
          value: "institutionCode",
        },
        {
          text: this.$t("refugee.headers.institutionName"),
          value: "institutionName",
        },
        {
          text: this.$t("refugee.headers.town"),
          value: "institutionTown",
        },
        {
          text: this.$t("refugee.headers.region"),
          value: "institutioRegion",
        },
        {
          text: this.$t("refugee.headers.ruoDocNumber"),
          value: "ruodocNumber",
        },
        {
          text: this.$t("refugee.headers.ruoDocDate"),
          value: "ruodocDate",
        },
        {
          text: this.$t("refugee.headers.admissionStatus"),
          value: "admissionStatus",
        },
        {
          text: this.$t("refugee.headers.currentInstitutionCode"),
          value: "currentInstitutionId",
        },
        {
          text: this.$t("refugee.headers.classes"),
          value: "classes",
        },
        { text: '', value: 'controls', sortable: false, align: 'end' }
      ],
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId', 'userRegionId'])
  },
  created() {
    this.$studentHub.$on('refugee-enrolled-in-institution', this.refugeeEnrolledInInstitution);
  },
  destroyed() {
    this.$studentHub.$off('refugee-enrolled-in-institution');
  },
  methods: {
    gridReload() {
      const grid = this.$refs['refugeeAdmissionListGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    refugeeEnrolledInInstitution(_personId, institutionId, regionId) {
      if (this.userInstitutionId) {
        // Ако сме институция презареждаме грида, ако детето е записано в нашата институция.
        if(this.userInstitutionId === institutionId) this.gridReload();
      } else if(this.userRegionId) {
        // Ако сме РУО презареждаме грида, ако детето е записано в институция от нашия регион.
        if(this.userRegionId === regionId) this.gridReload();
      } else {
        // За всички останали (МОН и др.) презареждаме винаги.
        this.gridReload();
      }
    }
  }
};
</script>

