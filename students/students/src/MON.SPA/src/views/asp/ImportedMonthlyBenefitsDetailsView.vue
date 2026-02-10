<template>
  <div>
    <import-details
      :imported-file-id="importedFileId"
      :grid-status-filter="gridStatusFilter"
      :year="year"
      :month="month"
    />
    <v-divider />
    <InstitutionsNotApprovedBenefitsList
      v-if="hasAspBenefitDetailsReadPermission"
      :year="year"
      :month="month"
      :campaign-id="importedFileId"
    />
  </div>
</template>

<script>
import ImportDetails from '@/components/asp/ImportedMonthlyBenefitsDetails';
import InstitutionsNotApprovedBenefitsList from '@/components/asp/InstitutionsNotApprovedBenefitsList';
import { mapGetters } from 'vuex';
import { Permissions} from '@/enums/enums';

export default {
  name: 'ImportedMonthlyBenefitsDetailsView',
  components: { ImportDetails, InstitutionsNotApprovedBenefitsList },
  props: {
    importedFileId: {
      type: Number,
      required: true,
    },
    year: {
      type: Number,
      required: true,
    },
    month: {
      type: Number,
      required: true,
    },
    gridStatusFilter: {
      type: Number,
      default() {
        return undefined;
      }
    }
  },
   computed: {
    ...mapGetters(['hasPermission']),
    hasAspBenefitDetailsReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForASPBenefitDetailsRead);
    }
  }
};
</script>
