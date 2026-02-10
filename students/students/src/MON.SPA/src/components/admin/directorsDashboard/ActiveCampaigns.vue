<template>
  <v-card
    v-if="activeCampaigns"
    class="mt-2"
  >
    <v-card-title>
      {{ this.$t("absenceReport.filter.activeCampaign") }}
    </v-card-title>
    <v-card-text>
      <v-row
        v-cloak
        dense
      >
        <v-col
          v-for="(campaign, index) in activeCampaigns"
          :key="index"
          cols="12"
          sm="6"
          md="4"
          lg="3"
        >
          <campaign-details-card
            :value="campaign"
            :show-extended-details="showExtendedDetails"
            @noAbsencesSubmited="load()"
          />
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
import Constants from "@/common/constants.js";
import CampaignDetailsCard from "@/components/admin/stats/CampaignDetailsCard";

export default {
  name: 'ActiveCampaigns',
  components: {
    CampaignDetailsCard

  },
  props: {
    showExtendedDetails: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      campaigns: [],
      activeCampaigns: null,
      dateFormat: Constants.DATEPICKER_FORMAT,
      componentKey: 0
    };
  },
  mounted(){
    this.load();
    this.$studentHub.$on('absence-campaign-modified', this.onAbsenceCampaignModified);
    this.$studentHub.$on('asp-campaign-modified', this.onAspCampaignModified);
    this.$studentEventBus.$on('absenceImportDelete',this.load);

  },
  destroyed() {
    this.$studentHub.$off('absence-campaign-modified');
    this.$studentHub.$off('asp-campaign-modified');
    this.$studentEventBus.$off('absenceImportDelete');
  },
  methods:{
    load() {
      this.$helper.clearArray(this.activeCampaigns);
      this.$api.admin.getAllActiveCampaigns()
        .then(result => {
          if (result.data) {
            this.activeCampaigns = result.data;
          }
        })
        .catch((error) => {
            console.log(error.response);
        });
    },
    navigate(campaign){
        if(campaign.isAspImport === false){
          this.$router.push({ path: `/absence/import/${campaign.importId}/details` });
        }else{
          this.$router.push({ path: `/asp/monthlyBenefitsImport/${campaign.year}/${campaign.month}/benefitDetails/${campaign.importId}` });
        }
    },
    onAbsenceCampaignModified(id) {
      console.log(id);
      this.load();
    },
    onAspCampaignModified(id){
      console.log(id);
      this.load();
    }
  }
};
</script>
