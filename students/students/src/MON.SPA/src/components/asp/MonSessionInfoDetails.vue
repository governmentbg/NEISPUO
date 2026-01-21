<template>
  <v-row
    v-if="value != undefined"
  >
    <v-col>
      <v-alert
        v-if="value"
        border="top"
        colored-border
        type="success"
        elevation="2"
      >
        <v-card-title>
          {{ $t('absenceCampaign.headers.hasMonAspResponseSession') }}
        </v-card-title>

        <v-simple-table>
          <template>
            <thead>
              <tr>
                <th>{{ $t('common.schoolYearFor') }}</th>
                <th>{{ $t('common.monthFor') }}</th>
                <th>{{ $t('aspAsking.headers.infoType') }}</th>
                <th>{{ $t('aspAsking.headers.proccessTime') }}</th>
                <th>{{ $t('aspAsking.headers.recordsCount') }}</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>{{ value.year }}</td>
                <td>{{ $helper.getMonthName(value.month) }}</td>
                <td>{{ value.infoType }}</td>
                <td>{{ value.monProcessed ? $moment(value.monProcessed).format(dateTimeFormat) : '' }}</td>
                <td>{{ value.confirmationRecordsCount }}</td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>

        <v-expansion-panels
          v-if="value"
          v-model="expandablePanelModel"
          multiple
          popout
        >
          <v-expansion-panel class="mt-2">
            <v-expansion-panel-header>
              {{ this.$t('aspConfirms.listTitle') }}
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <asp-submitted-mon-confirms-list
                :session-no="value.sessionNo"
              />
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-alert>
      <v-alert
        v-else
        border="top"
        colored-border
        type="warning"
        elevation="2"
      >
        {{ $t('absenceCampaign.missingMonResponseInfo') }}
      </v-alert>
    </v-col>
  </v-row>
</template>

<script>
  import Constants from "@/common/constants.js";
  import AspSubmittedMonConfirmsList from '@/components/asp/AspSubmittedMonConfirmsList';

  export default {
    name: 'MonSessionInfoDetails',
    components: { AspSubmittedMonConfirmsList },
    props: {
      value: {
        type: Object,
        default() {
          return undefined;
        }
      },
      isAbsenceSession: {
        type: Boolean,
        default() {
          return false;
        }
      },
      isConfirmationSession: {
        type: Boolean,
        default() {
          return false;
        }
      }
    },
    data() {
      return {
        dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
        expandablePanelModel: []
      };
    }

  };
</script>
