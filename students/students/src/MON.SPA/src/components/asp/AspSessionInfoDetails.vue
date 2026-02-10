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
          {{ $t('absenceCampaign.headers.hasAspAskingSession') }}
        </v-card-title>

        <v-simple-table>
          <template>
            <thead>
              <tr>
                <th>{{ $t('common.schoolYearFor') }}</th>
                <th>{{ $t('common.monthFor') }}</th>
                <th>{{ $t('aspAsking.headers.infoType') }}</th>
                <th>{{ $t('aspAsking.headers.proccessTime') }}</th>
                <th
                  v-if="isAbsenceSession"
                >
                  {{ $t('aspAsking.headers.absencesCount') }}
                </th>
                <th
                  v-if="isAbsenceSession"
                >
                  {{ $t('aspAsking.headers.zpCount') }}
                </th>
                <th
                  v-if="isConfirmationSession"
                >
                  {{ $t('aspAsking.headers.recordsCount') }}
                </th>
                <th
                  v-if="isConfirmationSession && !value.hasConfirmationCampaign"
                />
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>{{ value.year }}</td>
                <td>{{ $helper.getMonthName(value.month) }}</td>
                <td>{{ value.infoType }}</td>
                <td>{{ value.monProcessed ? $moment(value.monProcessed).format(dateTimeFormat) : '' }}</td>
                <td
                  v-if="isAbsenceSession"
                >
                  {{ value.absenceCount }}
                </td>
                <td
                  v-if="isAbsenceSession"
                >
                  {{ value.zpCount }}
                </td>
                <td
                  v-if="isConfirmationSession"
                >
                  {{ value.confirmationRecordsCount }}
                </td>
                <td
                  v-if="isConfirmationSession && !value.hasConfirmationCampaign"
                >
                  <button-tip
                    icon-name="fa-file-import"
                    text="asp.importLabel"
                    tooltip="asp.importLabel"
                    color="primary"
                    bottom
                    @click="$emit('importAspConfirmationSessionRecords', value)"
                  />
                </td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>
      </v-alert>
      <v-alert
        v-else
        border="top"
        colored-border
        type="warning"
        elevation="2"
      >
        {{ $t('absenceCampaign.missingAspAskingInfo') }}
      </v-alert>
    </v-col>
  </v-row>
</template>

<script>
  import Constants from "@/common/constants.js";

  export default {
    name: 'AspSessionInfoDetails',
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
      };
    }

  };
</script>
