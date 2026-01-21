<template>
  <v-form
    :ref="'commonPersonalDevelopmentSupportForm' + _uid"
    :disabled="disabled"
  >
    <v-row>
      <v-col>
        <school-year-selector
          v-model="model.schoolYear"
          :label="$t('common.schoolYear')"
          :rules="[$validator.required()]"
          class="required"
        />
      </v-col>
    </v-row>
    <v-alert
      v-for="(item, index) in model.items"
      :key="item.uid"
      class="pa-0 mb-0"
      dense
    >
      <template #close>
        <v-col
          align="center"
          :style="{ 'max-width': '40px' }"
          class="pa-0 mb-10"
        >
          <button-tip
            v-if="!disabled"
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
            v-model="item.typeName"
            :label="$t('studentOtherInstitutions.reasonLabel')"
            disabled
            dense
            persistent-placeholder
            outlined
          />
          <c-select
            v-else
            v-model="item.typeId"
            :items="commonSupportTypeOptions"
            :label="$t('studentOtherInstitutions.reasonLabel')"
            :rules="[$validator.required()]"
            class="required"
            clearable
            dense
            persistent-placeholder
            outlined
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-textarea
            v-model="item.details"
            :label="$t('common.detailedInformation')"
            outlined
            dense
            persistent-placeholder
            clearable
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
          text="buttons.lodCommonSupportTypeAddTooltip"
          tooltip="buttons.lodCommonSupportTypeAddTooltip"
          iclass=""
          outlined
          bottom
          small
          @click="onAdd"
        />
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <file-manager
          v-if="!disabled || (model.documents && model.documents.length > 0)"
          v-model="model.documents"
          :disabled="disabled"
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import FileManager from '@/components/common/FileManager.vue';

export default {
  name: 'CommonPersonalDevelopmentSupportForm',
  components: {
    SchoolYearSelector,
    FileManager
  },
  props: {
    value: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    },
  },
  data() {
    return {
      model: this.value,
      commonSupportTypeOptions: [],
    };
  },
  watch: {
    value() {
      this.model = this.value;
    }
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getCommonSupportTypeOptions()
        .then((response) => {
          this.commonSupportTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.commonSupportTypeOptionsLoad'));
          console.log(error.response);
        });
    },
    validate() {
      const form = this.$refs['commonPersonalDevelopmentSupportForm' + this._uid];
      return form ? form.validate() : false;
    },
    onAdd() {
      this.model.items.push({
        uid: this.$uuid.v4()
      });
    },
    onRemove(index) {
      if (!this.model.items) return;
      this.model.items.splice(index, 1);
    },
  }
};
</script>
