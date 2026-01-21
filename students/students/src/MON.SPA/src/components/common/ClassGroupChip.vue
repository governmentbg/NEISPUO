<template>
  <span
    v-if="value"
  >
    <v-tooltip top>
      <template v-slot:activator="{ on }">
        <v-chip
          class="mr-2"
          :color="value.positionId === 3 ? 'primary' : 'default'"
          outlined
          v-on="on"
        >
          <span>
            {{ $t('profile.class') }}: <strong>{{ value.className }}</strong>
          </span>
          <span
            v-if="value.institutionName && (!userInstitutionId || userInstitutionId != value.institutionId)"
            class="ml-1"
          >
            {{ $t('profile.school') }}: <strong>{{ value.institutionName }}</strong>
          </span>
        </v-chip>
      </template>
      <span>
        {{ value.position }}
      </span>
    </v-tooltip>
  </span>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  name: 'ClassGroupChip',
  props: {
    value: {
      type: undefined,
      default() {
        return undefined;
      }
    }
  },
  computed: {
    ...mapGetters(['userInstitutionId'])
  }
};
</script>
