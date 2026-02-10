<template>
  <v-form
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <!-- {{ model }} -->
    <v-row dense>
      <v-col
        cols="12"
        md="6"
        lg="4"
      >
        <c-info
          uid="resourceSupport.schoolYear"
        >
          <school-year-selector
            v-model="model.schoolYear"
            :label="$t('common.schoolYear')"
            show-current-school-year-button
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="4"
      >
        <c-info
          uid="resourceSupport.reportNumber"
        >
          <v-text-field
            v-model="model.reportNumber"
            :label="$t('resourceSupport.numberLabel')"
            clearable
            :rules="[$validator.required()]"
            class="required"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="4"
      >
        <c-info
          uid="resourceSupport.reportDate"
        >
          <date-picker
            id="reportDate"
            ref="reportDate"
            v-model="model.reportDate"
            :label="$t('resourceSupport.reportDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
            :show-debug-data="false"
            :rules="[$validator.required()]"
            class="required"
          />
        </c-info>
      </v-col>
    </v-row>

    <resource-support
      v-model="model.resourceSupports"
      :disabled="disabled"
    />

    <v-row
      dense
    >
      <v-col>
        <file-manager
          v-model="model.documents"
          :disabled="disabled"
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import { StudentResourceSupportModel } from "@/models/studentResourceSupportModel.js";
import FileManager from '@/components/common/FileManager.vue';
// import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import { mapGetters } from 'vuex';
import { NotificationSeverity } from '@/enums/enums';
import ResourceSupport from './ResourceSupportComponent.vue';


export default {
  name: 'ResourceSupportForm',
  components: {
    SchoolYearSelector,
    FileManager,
    ResourceSupport

    // Autocomplete
  },
  props: {
    value:{
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
  },
  data() {
    return {
      model: this.value ?? new StudentResourceSupportModel()
    };
  },
  computed: {
    ...mapGetters(['studentFinalizedLods'])
  },
  mounted() {
  },
  methods: {
    validate() {
      const form = this.$refs['form' + this._uid];

      var rsValid = true;

      this.model.resourceSupports.forEach((rs) => {
          if(rs.resourceSupportSpecialists.length === 0){

            rsValid = false;
          }
      });

      var formValid = form ? form.validate() : false;

      if(formValid == false){
        return formValid;
      }
      else if(rsValid === false){
        this.$notifier.toast('',this.$t('resourceSupport.supportSpecialistMissigError'), NotificationSeverity.Warn);
      }else{
        return formValid && rsValid;
      }
    },
    onResourceSupportAdd() {
      if(!this.model.resourceSupports) this.model.resourceSupports = [];

      this.model.resourceSupports.push({ resourceSupportSpecialists: [] });
    },
    onResourceSupportDelete(index) {
      if(!this.model.resourceSupports) return;

      this.model.resourceSupports.splice(index, 1);
    },
    onResourceSupportSpecialistAdd(index) {
      if(!this.model.resourceSupports) return;

      if(!this.model.resourceSupports[index].resourceSupportSpecialists){
       this.model.resourceSupports[index].resourceSupportSpecialists = [];
      }
      this.model.resourceSupports[index].resourceSupportSpecialists.push({});
    },
    onResourceSupportSpecialistDelete(index, rSSindex) {
      if(!this.model.resourceSupports || !this.model.resourceSupports[index].resourceSupportSpecialists) return;

      this.model.resourceSupports[index].resourceSupportSpecialists.splice(rSSindex, 1);

    }
  }
};
</script>
