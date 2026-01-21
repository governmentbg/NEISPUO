<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row
        dense
      >
        <v-col
          cols="12"
          md="6"
          lg="3"
        >
          <school-year-selector
            v-model="model.schoolYear"
            :label="$t('common.schoolYear')"
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
      </v-row>
      <sop
        v-model="model.sopDetails"
        :disabled="disabled"
      />
      <v-row>
        <v-col>
          <file-manager
            v-model="model.documents"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
    </v-form>
  </div>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import FileManager from '@/components/common/FileManager.vue';
import Sop from './SopComponent.vue';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: 'SopForm',
  components: {
    SchoolYearSelector,
    FileManager,
    Sop
  },
  props: {
    value: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      model: this.value,
      specialNeedsTypesOptions: [],
      specialNeedsSubTypesOptions: [],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'studentFinalizedLods']),
    hasManagePermisssion() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentSopManage);
    }
  },
  watch: {
    value() {
      this.model = this.value;
    },
   'model.sopDetails':{
        deep:true,
        handler:function (val) {
           if(Array.isArray(val) && val.length !== 0) {
                      var toDisable = val.map(function(a) {
                          if(a.specialNeedsTypeId){
                            return a.specialNeedsTypeId;
                          }
                      });

                      this.specialNeedsTypesOptions = this.specialNeedsTypesOptions.filter(function(el) {
                                                          if(toDisable.includes( el.value ))
                                                          {
                                                            el.isAvailable = false;
                                                            el.disabled = true;
                                                          }else{
                                                            el.isAvailable = true;
                                                            el.disabled = false;
                                                          }
                                                          return el;
                                                        });
           }
        }
      }

  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getSpecialNeedsTypesOptions()
        .then((response) => {
          this.specialNeedsTypesOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.specialNeedsTypesOptionsLoad'));
          console.log(error);
        });

      this.$api.lookups.getSpecialNeedsSubTypesOptions()
        .then((response) => {
          this.specialNeedsSubTypesOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.specialNeedsSubTypesOptionsLoad'));
          console.log(error);
        });
    },
    validate() {
      // Използва се в StudentSopCreate.vue и StudentSopEdit.vue
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
    filterSop(specialNeedsTypeId) {
      return (item) => item.relatedObjectId === specialNeedsTypeId;
    },
    onAddNewSopClick() {
      if(!this.model) return;

      if(!this.model.sopDetails) {
        this.model.sopDetails = [];
      }

      this.model.sopDetails.push({ specialNeedsTypeId: null, specialNeedsSubTypeId: null });
    },
    onDeleteSopClick(index){
      if(!this.model || !this.model.sopDetails) return;
      this.model.sopDetails.splice(index, 1);
    },
  }
};
</script>
