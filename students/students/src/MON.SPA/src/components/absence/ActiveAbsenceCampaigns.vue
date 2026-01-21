<template>
  <v-expansion-panels
    v-model="panel"
    multiple
  >
    <v-expansion-panel>
      <v-expansion-panel-header class="py-0">
        <v-card-title>
          {{ $t('absenceCampaign.activCampaignsTitle') }}
          <v-chip
            v-for="campaign in activeAbsenceCampaigns"
            :key="campaign.id"
            class="mx-2"
            color="success"
            label
            outlined
            @click:close="chip4 = false"
          >
            {{ `${campaign.schoolYearName} - ${$helper.getMonthName(campaign.month)}` }}
          </v-chip>
        </v-card-title>
      </v-expansion-panel-header>
      <v-expansion-panel-content>
        <v-row
          v-if="hasActiveCampaigns"
          v-cloak
          dense
        >
          <v-col
            v-for="campaign in activeAbsenceCampaigns"
            :key="campaign.id"
            cols="12"
            sm="6"
            md="3"
          >
            <absence-campaign-details-card
              :value="campaign"
              @noAbsencesSubmited="onNoAbsencesSubmited"
            />
          </v-col>
        </v-row>
        <v-alert
          v-else
          outlined
          type="warning"
          prominent
          dense
        >
          <h4>{{ $t('absenceCampaign.missingActiveCampaigns') }}</h4>
        </v-alert>
      </v-expansion-panel-content>
    </v-expansion-panel>
  </v-expansion-panels>
</template>

<script>
import AbsenceCampaignDetailsCard from '@/components/absence/AbsenceCampaignDetailsCard.vue';

export default {
  name: 'ActiveAbsenceCampaigns',
  components: { AbsenceCampaignDetailsCard },
  props: {
    expanded: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      activeAbsenceCampaigns: null,
      panel: this.expanded ? [0] : []
    };
  },
  computed: {
    hasActiveCampaigns() {
      return this.activeAbsenceCampaigns && this.activeAbsenceCampaigns.length > 0;
    }
  },
  mounted() {
    this.loadActiveAbsenceCampaigns();
    this.$studentHub.$on('absence-campaign-modified', this.onAbsenceCampaignModified);
    this.$studentEventBus.$on('absenceImportDelete',this.onAbsenceCampaignModified);

  },
  destroyed() {
    this.$studentHub.$off('absence-campaign-modified');
    this.$studentEventBus.$off('absenceImportDelete');
  },
  methods: {
    loadActiveAbsenceCampaigns() {
      this.$helper.clearArray(this.activeAbsenceCampaigns);

      this.$api.absenceCampaign.getActive()
      .then(response => {
          if (response.data) {
            this.activeAbsenceCampaigns = response.data;
            this.$emit('campaignsLoaded', this.activeAbsenceCampaigns);
          }
      })
      .catch(error => {
          this.$notifier.error('', this.$t('errors.absenceCampaignLoad'));
          console.log(error.response);
      });
    },
    // eslint-disable-next-line no-unused-vars
    onAbsenceCampaignModified(id) {
      this.loadActiveAbsenceCampaigns();
    },
    onNoAbsencesSubmited() {
      this.loadActiveAbsenceCampaigns();
      this.$emit('noAbsencesSubmited');
    }
  }
};
</script>
