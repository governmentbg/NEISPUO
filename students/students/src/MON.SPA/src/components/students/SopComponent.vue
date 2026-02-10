<template>
  <div>
    <!-- {{ model }} -->
    <v-alert
      v-for="(sop, index) in model"
      :key="index"
      dense
      class="pa-0 mb-0"
    >
      <template
        v-if="!disabled"
        #close
      >
        <v-col
          align="center"
          :style="{ 'max-width': '40px' }"
          class="pa-0 mb-7"
        >
          <button-tip

            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="onRemove(index)"
          />
        </v-col>
      </template>
      <v-row
        dense
      >
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-if="disabled"
            v-model="sop.sopTypeName"
            :label=" $t('common.type')"
            disabled
            dense
            persistent-placeholder
            outlined
          />
          <v-select
            v-else
            v-model="sop.specialNeedsTypeId"
            :items="filteredSpecialNeedsTypesOptions"
            :label=" $t('common.type')"
            :rules="[$validator.required()]"
            class="required"
            clearable
            dense
            persistent-placeholder
            outlined
            @change="model.specialNeedsSubTypeId = null"
          >
            <template v-slot:selection="data">
              {{ data.item.text }}
            </template>
          </v-select>
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-if="disabled"
            v-model="sop.sopSubTypeName"
            :label=" $t('common.type')"
            disabled
            dense
            persistent-placeholder
            outlined
          />
          <v-select
            v-else-if="specialNeedsSubTypesOptions.filter(filterSopSubType(sop.specialNeedsTypeId)).length > 0"
            v-model="sop.specialNeedsSubTypeId"
            :items="specialNeedsSubTypesOptions.filter(filterSopSubType(sop.specialNeedsTypeId))"
            :label="$t('common.explanations')"
            clearable
            dense
            persistent-placeholder
            outlined
          />
        </v-col>
      </v-row>
    </v-alert>
    <v-row
      v-if="!disabled"
      dense
    >
      <v-col
        cols="12"
      >
        <button-tip
          icon-name="mdi-plus"
          icon-color="primary"
          color="primary"
          text="buttons.sopAddTooltip"
          tooltip="buttons.sopAddTooltip"
          iclass=""
          outlined
          bottom
          small
          @click="onAdd"
        />
      </v-col>
    </v-row>
  </div>
</template>

<script>
export default {
  name: 'SopComponent',
  props: {
    value: {
      type: Array,
      required: true
    },
    disabled: {
      type: Boolean,
      default: false
    },
  },
  data() {
    return {
      model: this.value,
      specialNeedsTypesOptions: [],
      specialNeedsSubTypesOptions: [],
    };
  },
  computed: {
    filteredSpecialNeedsTypesOptions() {
      return this.specialNeedsTypesOptions.filter(x => {
        if(this.model.some(e => e.specialNeedsTypeId === x.value)) {
          x.disabled = true;
        } else {
          x.disabled = false;
        }
        return x;
      });
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
    filterSopSubType(specialNeedsTypeId) {
      return (item) => item.relatedObjectId === specialNeedsTypeId;
    },
    onAdd() {
      this.model.push({ specialNeedsTypeId: null, specialNeedsSubTypeId: null });
    },
    onRemove(index) {
      if (!this.model) return;
      this.model.splice(index, 1);
    },
  }
};
</script>
