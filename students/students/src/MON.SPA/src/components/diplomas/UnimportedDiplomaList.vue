<template>
  <div>
    <grid
      v-if="schoolYearLoaded"
      :ref="'unimportedDiplomaListGrid' + _uid"
      url="/api/diploma/listUnimported"
      :headers="headers"
      title=" "
      show-expand
      :expanded.sync="expandedItems"
      :filter="{
        year: year,
      }"
      @pagination="clearSelections"
    >
      <template #subtitle>
        <v-row dense>
          <v-col>
            <school-year-selector
              v-model="year"
              item-text="value"
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`expanded-item`]="{ item }">
        <td :colspan="headers.length">
          <vue-json-pretty
            path="res"
            :data="item"
            show-length
          />
        </td>
      </template>

      <template v-slot:[`item.personalId`]="{ item }">
        {{ `${item.personalId} - ${item.personalIdTypeName}` }}
      </template>

      <template v-slot:[`item.isSigned`]="{ item }">
        <v-chip
          v-if="item.diplomaId"
          :color="item.diplomaIsSigned === true ? 'success' : 'error'"
          outlined
          small
        >
          <yes-no :value="item.diplomaIsSigned" />
        </v-chip>
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="item.item.personId"
            icon
            icon-color="primary"
            icon-name="fas fa-info-circle"
            iclass=""
            tooltip="student.details"
            top
            small
            @click="onStudentDetailsClick(item.item.personId)"
          />
          <button-tip
            v-if="item.item.personId"
            icon
            icon-color="primary"
            icon-name="fas fa-certificate"
            iclass=""
            tooltip="menu.diplomas.title"
            bottom
            small
            @click="onDiplomaListClick(item.item.personId)"
          />
        </button-group>
      </template>
    </grid>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import { mapGetters } from "vuex";
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";

export default {
  name: "UnimportedDiplomaListComponent",
  components: {
    Grid,
    SchoolYearSelector,
    VueJsonPretty
  },
  props: {
    institutionId: {
      type: Number,
      required: false,
      default: null,
    }
  },
  data() {
    return {
      year: null,
      schoolYearLoaded: false,
      expandedItems: [],
      headers: [
        {
          text: this.$t("diplomas.headers.personName"),
          value: "fullName",
        },
        {
          text: this.$t("diplomas.headers.pin"),
          value: "personalId",
        },
        {
          text: this.$t("diplomas.headers.schoolYear"),
          value: "studentClassSchoolYearName",
        },
        {
          text: this.$t("diplomas.headers.basicClass"),
          value: "studentClassBasicClassId",
        },
        {
          text: this.$t("diplomas.isSignedStatus"),
          value: "isSigned",
        },
        {
          text: "",
          value: "controls",
          filterable: false,
          sortable: false,
          align: "end",
        },
      ],
    };
  },
  computed: {
    ...mapGetters(["hasPermission"])
  },
  async created() {
    try {
      const currentYear = Number((await this.$api.institution.getCurrentYear(this.institutionId))?.data);
        if (!isNaN(currentYear)) {
          this.year = currentYear;
          this.refresh();
        } else {
          this.$helper.getYear();
        }
    } catch (error) {
      console.log(error);
      this.$helper.getYear();
    } finally {
      this.schoolYearLoaded = true;
    }
  },
  methods: {
    refresh() {
      const grid = this.$refs["unimportedDiplomaListGrid" + this._uid];
      if (grid) {
        grid.get();
      }
    },
    clearSelections() {
      this.$helper.clearArray(this.expandedItems);
    },
    onStudentDetailsClick(personId) {
      let routeData = this.$router.resolve({name: 'StudentDetails', params: { id: personId }});
      window.open(routeData.href, '_blank');
    },
    onDiplomaListClick(personId) {
      let routeData = this.$router.resolve({name: 'StudentDiplomasList', params: { id: personId }});
      window.open(routeData.href, '_blank');
    }
  },
};
</script>
