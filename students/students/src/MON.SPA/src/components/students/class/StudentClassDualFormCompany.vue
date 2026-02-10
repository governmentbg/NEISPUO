<template>
  <v-card dense>
    <v-card-title>
      <v-toolbar
        dense
        flat
      >
        <span>{{ $t('company.toolbarTitle', { companyName: model.name}) }}</span>
        <v-spacer />
        <button-tip
          v-if="!disabled"
          icon
          icon-name="mdi-delete"
          icon-color="red"
          bottom
          iclass=""
          tooltip="buttons.delete"
          @click="$emit('delete', model)"
        />
      </v-toolbar>
    </v-card-title>
    <v-card-text>
      <v-row dense>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.uic"
            :label="$t('company.uic')"
            dense
            outlined
            persistent-placeholder
            :disabled="disabled"
            :rules="disabled ? [] : [$validator.required()]"
            :class="disabled ? '' : 'required'"
            clearable
            :append-icon="disabled || !model.uic ? '' : 'mdi-magnify'"
            :loading="loading"
            @click:append="getCompanyDetails"
            @keydown.enter.prevent="getCompanyDetails"
            @keydown.esc.prevent="model.uic = ''"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.name"
            :label="$t('company.name')"
            dense
            outlined
            persistent-placeholder
            :disabled="disabled"
            :rules="disabled ? [] : [$validator.required()]"
            :class="disabled ? '' : 'required'"
            clearable
            :loading="loading"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.email"
            :label="$t('company.email')"
            dense
            outlined
            persistent-placeholder
            clearable
            :disabled="disabled"
            :loading="loading"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.phone"
            :label="$t('company.phone')"
            dense
            outlined
            persistent-placeholder
            clearable
            :disabled="disabled"
            :loading="loading"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.district"
            :label="$t('location.district')"
            dense
            outlined
            persistent-placeholder
            clearable
            :disabled="disabled"
            :loading="loading"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.municipality"
            :label="$t('location.municipality')"
            dense
            outlined
            persistent-placeholder
            clearable
            :disabled="disabled"
            :loading="loading"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.settlement"
            :label="$t('location.town')"
            dense
            outlined
            persistent-placeholder
            clearable
            :disabled="disabled"
            :loading="loading"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.area"
            :label="$t('location.localArea')"
            dense
            outlined
            persistent-placeholder
            clearable
            :disabled="disabled"
            :loading="loading"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-text-field
            v-model="model.address"
            :label="$t('location.address')"
            dense
            outlined
            persistent-placeholder
            clearable
            :disabled="disabled"
            :loading="loading"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <date-picker
            id="startDate"
            ref="startDate"
            v-model="model.startDate"
            :show-buttons="false"
            :scrollable="false"
            :no-title="true"
            :show-debug-data="false"
            :label="$t('ores.startDate')"
            clearable
            :max="model.endDate ? helper.parseDateToIso(model.endDate) : undefined"
            :rules="disabled ? [] : [$validator.required()]"
            :class="disabled ? '' : 'required'"
            outlined
            persistent-placeholder
            dense
            :disabled="disabled"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <date-picker
            id="endDate"
            ref="endDate"
            v-model="model.endDate"
            :show-buttons="false"
            :scrollable="false"
            :no-title="true"
            :show-debug-data="false"
            :label="$t('ores.endDate')"
            clearable
            :min="model.startDate ? helper.parseDateToIso(model.startDate) : undefined"
            outlined
            persistent-placeholder
            dense
            :disabled="disabled"
          />
        </v-col>
      </v-row>
    </v-card-text>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import { StudentClassDualFormCompanyModel } from '@/models/studentClass/studentClassDualFormCompanyModel';
import Helper from '@/components/helper.js';

export default{
  name: 'StudentClassDualFormCompany',
  props: {
    value: {
      type: StudentClassDualFormCompanyModel,
      required: true
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: this.value,
      loading: false,
      helper: Helper,
    };
  },
  methods: {
    getCompanyDetails() {
      if(!this.model.uic) {
        return;
      }

      this.loading = true;

      this.$api.regix.getCompanyDetails(this.model.uic)
      .then((response) => {
        if(response.data) {
          const details = response.data;
          this.model.raw = details;
          this.model.uic = details.uic;
          this.model.name = details.name;
          this.model.email = details.email;
          this.model.phone = details.phone;
          this.model.district = details.district;
          this.model.municipality = details.municipality;
          this.model.settlement = details.settlement;
          this.model.area = details.area;
          this.model.address = details.address;
          this.model.startDate = details.startDate;
          this.model.endDate = details.endDate;
        }
      })
      .finally(() => {
        this.loading = false;
      });
    },
  }
};
</script>
