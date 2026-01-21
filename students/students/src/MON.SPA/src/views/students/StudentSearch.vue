<template>
  <v-card
    :loading="loading"
    :flat="flat"
  >
    <!-- {{ value }} -->
    <v-card-title>
      <font-awesome-icon icon="child" /> {{ $tc('student.title', 2) }}
    </v-card-title>
    <v-card-subtitle>{{ $tc('student.search.title', 2) }}</v-card-subtitle>
    <v-card-text class="pb-0">
      <v-row dense>
        <v-col
          cols="12"
          sm="6"
        >
          <v-text-field
            v-model="value.pin"
            :label="$t('student.headers.identifier')"
            clearable
            :disabled="disabled"
            @keyup="onKeyUp($event, 'pin')"
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
        >
          <v-text-field
            id="publicEduNumber"
            v-model="value.publicEduNumber"
            :label="$t('student.headers.publicEduNumber')"
            name="publicEduNumber"
            autocomplete="publicEduNumber"
            clearable
            :disabled="disabled"
            @keyup="onKeyUp($event, 'publicEduNumber')"
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
        >
          <v-text-field
            id="firstНame"
            v-model="value.firstName"
            :label="$t('student.headers.firstName')"
            name="firstНame"
            autocomplete="firstНame"
            clearable
            :disabled="disabled"
            @keyup="onKeyUp($event, 'firstName')"
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
        >
          <v-text-field
            id="middleName"
            v-model="value.middleName"
            :label="$t('student.headers.middleName')"
            name="middleName"
            autocomplete="middleName"
            clearable
            :disabled="disabled"
            @keyup="onKeyUp($event, 'middleName')"
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
        >
          <v-text-field
            id="lastName"
            v-model="value.lastName"
            :label="$t('student.headers.lastName')"
            name="lastName"
            autocomplete="lastName"
            clearable
            :disabled="disabled"
            @keyup="onKeyUp($event, 'lastName')"
          />
        </v-col>
        <v-col
          v-if="isElevated"
          cols="12"
          sm="6"
          md="6"
        >
          <combo
            id="district"
            v-model="value.district"
            api="/api/lookups/GetDistricts"
            :defer-options-loading="true"
            :return-object="false"
            :show-deferred-loading-hint="true"
            item-value="text"
            item-text="text"
            :label="$t('student.headers.district')"
            :disabled="disabled"
            :placeholder="$t('buttons.search')"
            :remove-items-on-clear="true"
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
        >
          <combo
            v-if="isElevated || isInRole(roleRuo)"
            id="municipality"
            v-model="value.municipality"
            api="/api/lookups/GetMunicipalities"
            :defer-options-loading="true"
            :return-object="false"
            :show-deferred-loading-hint="true"
            item-value="text"
            item-text="text"
            :label="$t('student.headers.municipality')"
            :disabled="disabled"
            :placeholder="$t('buttons.search')"
            :remove-items-on-clear="true"
          />
        </v-col>
        <v-col
          v-if="isElevated || isInRole(roleRuo)"
          cols="12"
          sm="6"
          md="4"
        >
          <combo
            id="school"
            ref="school"
            v-model="value.school"
            item-value="clearName"
            item-text="text"
            api="/api/lookups/GetInstitutionOptions"
            :placeholder="$t('buttons.search')"
            :defer-options-loading="true"
            :return-object="false"
            :show-deferred-loading-hint="true"
            :label="$t('student.headers.school')"
            :disabled="disabled || isInRole(roleSchool)"
            :remove-items-on-clear="true"
          />
        </v-col>
      </v-row>
      <v-row dense>
        <v-col
          v-if="!hideOwnInstitutionCheckbox && isInRole(roleSchool)"
          cols="12"
          sm="6"
        >
          <v-checkbox
            id="onlyOwnInstitution"
            v-model="value.onlyOwnInstitution"
            :label="$t('student.search.onlyOwnInstitution')"
            :disabled="disabled"
          />
        </v-col>
        <v-col
          v-if="!hideExactMatchCheckbox"
          cols="12"
          sm="6"
        >
          <v-checkbox
            id="exactMatch"
            v-model="value.exactMatch"
            :label="$t('student.search.exactMatch')"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions
      class="justify-end pt-0"
    >
      <slot name="actions" />
    </v-card-actions>
  </v-card>
</template>

<script>
import { UserRole } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentSearch',
  props: {
    value: {
      type: Object,
      required: true,
      default() {
        return undefined;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    loading: {
      type: Boolean,
      default() {
        return false;
      }
    },
    flat: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hideOwnInstitutionCheckbox: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hideExactMatchCheckbox: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      roleMon: UserRole.Mon,
      roleSchool: UserRole.School,
      roleRuo: UserRole.Ruo
    };
  },
  computed: {
    ...mapGetters(['isInRole']),
    isElevated() {
      return this.isInRole(UserRole.Mon) || this.isInRole(UserRole.Cioo) || this.isInRole(UserRole.MonExpert) || this.isInRole(UserRole.Consortium);
    },
  },
  methods: {
    onKeyUp(event, prop) {
      if(event.keyCode === 27) { // Escape was pressed
        event.preventDefault();
        if (prop && this.value) {
          this.value[prop] = null;
        }
      }

      if(event.keyCode === 13) { // Enter was pressed
        event.preventDefault();
        return this.$emit('field-enter-click');
      }
    },
  }
};
</script>
