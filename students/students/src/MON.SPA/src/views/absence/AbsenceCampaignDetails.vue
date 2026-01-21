<template>
  <div
    v-if="loading"
  >
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
  </div>
  <div
    v-else
  >
    <form-layout class="mb-3">
      <template #title>
        <h3>{{ $t('absenceCampaign.reviewTitle') }}</h3>
      </template>
      <template #default>
        <absence-campaign-details-card
          :value="model"
          show-extended-details
        />
        <asp-session-info-details
          :value="aspAskingSession"
          class="mt-2"
          is-absence-session
        />
      </template>
      <template #actions>
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="backClick"
        >
          <v-icon left>
            fas fa-chevron-left
          </v-icon>
          {{ $t('buttons.back') }}
        </v-btn>
      </template>
    </form-layout>
    <asp-asking-list
      v-if="hasAbsenceCampaignManagePermission"
      :campaign-id="id"
    />
    <asp-submitted-absences-list
      v-if="hasAbsenceCampaignManagePermission"
      :campaign-id="id"
      class="mt-2"
    />
    <asp-submitted-zp-list
      v-if="hasAbsenceCampaignManagePermission"
      :campaign-id="id"
      class="mt-2"
    />
  </div>
</template>

<script>
import AbsenceCampaignDetailsCard from '@/components/absence/AbsenceCampaignDetailsCard.vue';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';
import AspAskingList from '@/components/asp/AspAskingList';
import AspSubmittedAbsencesList from '@/components/asp/AspSubmittedAbsencesList';
import AspSubmittedZpList from '@/components/asp/AspSubmittedZpList';
import AspSessionInfoDetails from '@/components/asp/AspSessionInfoDetails.vue';

export default {
  name: 'AbsenceCampaignDetails',
  components: {
    AspSessionInfoDetails,
    AspAskingList,
    AspSubmittedAbsencesList,
    AspSubmittedZpList,
    AbsenceCampaignDetailsCard
  },
  props: {
    id: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: true,
      model: null,
      aspAskingSession: undefined
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasAbsenceCampaignManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceCampaignManage);
    }
  },
  mounted() {
    if (!this.hasPermission(Permissions.PermissionNameForStudentAbsenceCampaignRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.absenceCampaign.getDetailsById(this.id)
        .then(response => {
          if (response.data) {
            this.model = response.data;
            this.checkAspAskingSession(this.model.schoolYear, this.model.month, 'ЗАПИТВАНЕ');
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('errors.absenceCampaignLoad'));
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    checkAspAskingSession(schoolYear, month, infoType) {
      this.$api.absenceCampaign.getAspSession(schoolYear, month, infoType)
        .then((response) => {
          this.aspAskingSession = response.data;
        });
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
