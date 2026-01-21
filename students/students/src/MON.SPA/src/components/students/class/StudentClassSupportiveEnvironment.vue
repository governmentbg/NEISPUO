<template>
  <v-card>
    <v-card-title>
      {{ $t('enroll.commonAndSpecialSupportiveEnvironment') }}
    </v-card-title>
    <v-card-text>
      <v-row dense>
        <v-col
          cols="12"
          md="4"
        >
          <v-checkbox
            v-model="model.hasSupportiveEnvironment"
            color="primary"
            :label="$t('enroll.supportiveEnvironmentNeeded')"
            @change="onSupportiveEnvironmentChange"
          />
        </v-col>
      </v-row>
      <div v-if="model.hasSupportiveEnvironment">
        <v-row dense>
          <v-col
            cols="12"
            md="6"
          >
            <custom-autocomplete
              id="buildingArea"
              ref="buildingArea"
              :key="autocompleteComponentKey"
              v-model="model.buildingAreas"
              api="/api/lookups/GetBuildingAreas"
              :label="$t('enroll.buildingAreas')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              :defer-options-loading="false"
              :disabled="!model.hasSupportiveEnvironment"
              multiple
              no-filter
              chips
              deletable-chips
              loading
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
          >
            <custom-autocomplete
              id="buildingRooms"
              ref="buildingRooms"
              :key="autocompleteComponentKey"
              v-model="model.buildingRooms"
              api="/api/lookups/GetBuildingRooms"
              :label="$t('enroll.buildingRooms')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :defer-options-loading="false"
              :disabled="!model.hasSupportiveEnvironment || !buildingAreasStr"
              :filter="buildingAreasFilter"
              multiple
              no-filter
              chips
              deletable-chips
              loading
              :rules="[$validator.required()]"
              :class="'required'"
            />
          </v-col>
        </v-row>
        <v-row dense>
          <v-col>
            <custom-autocomplete
              id="specialEquipment"
              ref="specialEquipment"
              :key="autocompleteComponentKey"
              v-model="model.specialEquipment"
              api="/api/lookups/GetSpecialEquipment"
              :label="$t('enroll.buildingEquipment')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :defer-options-loading="false"
              :disabled="!model.hasSupportiveEnvironment || !buildingRoomsStr"
              :filter="buildingRoomsFilter"
              multiple
              no-filter
              chips
              deletable-chips
              loading
              :rules="[$validator.required()]"
              :class="'required'"
            >
              <template v-slot:[`item`]="{ item }">
                <span
                  v-if="item.isAvailable"
                >
                  {{ item.text }}

                </span>
                <span
                  v-else
                >
                  <v-chip
                    class="ma-2"
                    color="red"
                    outlined
                  >
                    {{ item.text }}
                  </v-chip>
                </span>
              </template>
            </custom-autocomplete>
          </v-col>
        </v-row>
        <v-row dense>
          <v-col>
            <custom-autocomplete
              id="availableArchitecture"
              ref="availableArchitecture"
              :key="autocompleteComponentKey"
              v-model="model.availableArchitecture"
              api="/api/lookups/GetAvailableArchitecture"
              :label="$t('enroll.availableArchitecture')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :defer-options-loading="false"
              :disabled="!model.hasSupportiveEnvironment"
              multiple
              no-filter
              chips
              deletable-chips
              loading
            />
          </v-col>
        </v-row>
        <v-row dense>
          <v-col>
            <v-textarea
              id="supportiveEnvironment"
              v-model="model.supportiveEnvironment"
              outlined
              prepend-icon="mdi-comment"
              :label="$t('enroll.additionalData')"
              autocomplete="supportiveEnvironment"
              name="supportiveEnvironment"
              :disabled="!model.hasSupportiveEnvironment"
            />
          </v-col>
        </v-row>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'StudentClassSupportiveEnvironment',
  components:{
    CustomAutocomplete
  },
  props: {
    value: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      model: this.value,
      autocompleteComponentKey: 0,
      specialEquipmentsOptions: null
    };
  },
  computed: {
    buildingAreasFilter() {
      return { buildingAreas: this.buildingAreasStr};
    },
    buildingRoomsFilter() {
      return { buildingRooms: this.buildingRoomsStr};
    },
    buildingAreasStr() {
      return this.model.buildingAreas && Array.isArray(this.model.buildingAreas) ? this.model.buildingAreas.join() : '';
    },
    buildingRoomsStr() {
      return this.model.buildingRooms && Array.isArray(this.model.buildingRooms) ? this.model.buildingRooms.join() : '';
    }
  },
 watch: {
    'model.buildingAreas': {
        deep:true,
        handler: function (val){
            if(!val || (Array.isArray(val) && val.length === 0)) {
                this.model.buildingAreas = null;
                this.model.buildingRooms = [];
                this.model.specialEquipment = [];
            } else {
                if(this.model.buildingRooms && Array.isArray(this.model.buildingRooms) && this.model.buildingRooms.length === 0 && this.model.buildingAreas){
                        this.model.buildingRooms = null;
                }else if(this.model.buildingRooms && Array.isArray(this.model.buildingRooms) && this.model.buildingRooms.length > 0) {
                    const options = this.$refs['buildingRooms'].getOptionsList();

                    for (const roomId of this.model.buildingRooms){
                        const item = options.find(x => x.value === roomId);
                        if(item && !val.some(x => x === item.relatedObjectId)) {
                            this.model.buildingRooms.splice(this.model.buildingRooms.indexOf(roomId), 1);
                        }
                    }
                }
            }
        }
    },
    'model.buildingRooms':{
        deep:true,
        handler:function (val) {
            if(!val || (Array.isArray(val) && val.length === 0)) {
                 if(this.model.buildingAreas === null){
                    this.model.specialEquipment = [];
                 }
                 else{
                    this.model.specialEquipment = null;
                    this.model.buildingRooms = null;
                 }
            } else {
                if(this.model.specialEquipment && Array.isArray(this.model.specialEquipment) && this.model.specialEquipment.length > 0) {
                const options = this.$refs['specialEquipment'].getOptionsList();

                    for (const eqId of this.model.specialEquipment){
                            const item = options.find(x => x.value === eqId);
                            if(item && !val.some(x => x === item.relatedObjectId)) {
                            this.model.specialEquipment.splice(this.model.buildingRooms.indexOf(eqId), 1);
                        }
                    }
                }
            }
        }
    },
    'model.specialEquipment':{
        deep:true,
        handler:function (val) {
            if(!val || (Array.isArray(val) && val.length === 0)) {
                if(this.model.buildingAreas === null && this.model.specialEquipment.length !== 0){
                    this.model.specialEquipment = [];
                }else if (this.model.buildingAreas !== null){
                    this.model.specialEquipment = null;
                }
            }
        }
      }
  },
  mounted(){
      this.autocompleteComponentKey++;
    },
  methods: {
    onSupportiveEnvironmentChange() {
      if(!this.model.hasSupportiveEnvironment) {
        this.model.supportiveEnvironment = '';
        this.model.availableArchitecture = null;
        this.model.buildingRooms = null;
        this.model.buildingAreas = null;
        this.model.specialEquipment = null;
        this.model.supportiveEnvironment = null;
      }
    }
  }
};
</script>
