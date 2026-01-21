<template>
  <v-card>
    <v-card-title>
      {{ $tc('studentScholarships.title', 2) }}
    </v-card-title>
    <v-card-subtitle>
      <institution-external-so-provider-checker
        class="mt-2"
      />
    </v-card-subtitle>
    <v-card-text>
      <div
        v-if="hasStudentClassInCurrentYear && !hasStudentClassInCurrentYear"
        class="red--text font-weight-bold text-h6 text-center"
      >
        {{ $t('studentScholarships.noStudentClassMessage') }}
      </div>

      <v-data-table
        ref="scholarshipListTable"
        :items="scholarshipData"
        :items-pers-page="10"
        :headers="headers"
        :loading="loading"
        :search="search"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="scholarshipData"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :file-name="$tc('studentScholarships.title', 2)"
              :headers="headers"
            />
            <v-spacer />
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              :label="$t('common.search')"
              single-line
              hide-details
            />
          </v-toolbar>
        </template>

        <template v-slot:[`item.amountRate`]="{ item }">
          <span v-if="currency.showAltCurrency">{{ item.amountRateStr }}</span>
          <span v-else>{{ item.amountRate }}</span>
        </template>


        <template v-slot:[`item.documents`]="{ item }">
          <doc-downloader
            v-for="doc in item.documents"
            :key="doc.id"
            :value="doc"
            small
          />
        </template>

        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <template>
              <button-tip
                v-if="hasStudentScholarshipReadPermission"
                icon
                icon-name="mdi-eye"
                icon-color="primary"
                tooltip="buttons.review"
                bottom
                iclass=""
                small
                :to="`/student/${personId}/scholarship/${item.id}/details`"
              />
              <button-tip
                v-if="hasStudentScholarshipManagePermission"
                icon
                icon-name="mdi-pencil"
                icon-color="primary"
                tooltip="buttons.edit"
                bottom
                iclass=""
                small
                :disabled="item.isLodFinalized"
                :lod-finalized="item.isLodFinalized"
                :to="`/student/${personId}/scholarship/${item.id}/edit`"
              />
              <button-tip
                v-if="hasStudentScholarshipManagePermission"
                icon
                icon-name="mdi-delete"
                icon-color="error"
                tooltip="buttons.delete"
                bottom
                iclass=""
                small
                :disabled="item.isLodFinalized"
                :lod-finalized="item.isLodFinalized"
                @click="deleteScholarship(item)"
              />
            </template>
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasStudentScholarshipManagePermission"
              small
              color="primary"
              :to="`/student/${personId}/scholarship/add`"
            >
              {{ $t('buttons.newRecord') }}
            </v-btn>
            <v-btn
              small
              color="secondary"
              outlined
              @click="load"
            >
              {{ $t('buttons.reload') }}
            </v-btn>
          </button-group>
        </template>
      </v-data-table>
    </v-card-text>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>

import GridExporter from "@/components/wrappers/gridExporter";
import { StudentScholarshipModel } from '@/models/studentScholarship/studentScholarshipModel.js';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';
import DocDownloader from '@/components/common/DocDownloader.vue';
import InstitutionExternalSoProviderChecker from '@/components/institution/InstitutionExternalSoProviderChecker';


export default {
  name: 'ScholarshipsList',
  components: {
    GridExporter,
    DocDownloader,
    InstitutionExternalSoProviderChecker
  },
  props: {
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
  },
  data() {
    return {
        loading: false,
          headers: [
              {
                  text: this.$t('common.schoolYear'),
                  value: 'schoolYearName'
              },
              {
                  text: this.$t('studentScholarships.scholarshipAmount'),
                  value: 'scholarshipAmountName'
              },
              {
                  text: this.$t('studentScholarships.amountRate'),
                  value: 'amountRate'
              },
              {
                  text: this.$t('studentScholarships.scholarshipFinancingOrgan'),
                  value: 'scholarshipFinancingOrganName'
              },
              {
                  text: this.$t('studentScholarships.periodicity.periodicityLabel'),
                  value: 'periodicity'
              },
              {
                  text: this.$t('studentScholarships.scholarshipType'),
                  value: 'scholarshipTypeName'
              },
              {
                  text: this.$t('studentScholarships.institution'),
                  value: 'institutionName'
              },
              // {
              //     text: this.$t('studentScholarships.commissionNumber'),
              //     value: 'commissionNumber'
              // },
              // {
              //     text: this.$t('studentScholarships.commissionDate'),
              //     value: 'commissionDate'
              // },
              // {
              //     text: this.$t('studentScholarships.orderNumber'),
              //     value: 'orderNumber'
              // },
              // {
              //     text: this.$t('studentScholarships.orderDate'),
              //     value: 'orderDate'
              // },
              {
                  text: this.$t('studentScholarships.startingDateOfReceiving'),
                  value: 'startingDateOfReceiving'
              },
              {
                  text: this.$t('studentScholarships.endDateOfReceiving'),
                  value: 'endDateOfReceiving'
              },
              // {
              //     text: this.$t('studentScholarships.description'),
              //     value: 'description'
              // },
              {
                text: this.$t('studentScholarships.attachedDocuments'),
                value: 'documents',
                sortable: false,
                filterable: false,
              },
              { text: '', value: 'actions', sortable: false, align: 'end' }
          ],
          scholarshipData:[],
          search:'',
          hasStudentClassInCurrentYear: null
        };
    },
    computed: {
      ...mapGetters(['hasStudentPermission', 'currency']),
      hasStudentScholarshipReadPermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentScholarshipRead);
      },
      hasStudentScholarshipManagePermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentScholarshipManage);
      }
    },
    mounted() {
      if(!this.hasStudentPermission(Permissions.PermissionNameForStudentScholarshipRead)
        && !this.hasStudentPermission(Permissions.PermissionNameForStudentScholarshipManage)) {
        return this.$router.push('/errors/AccessDenied');
      }

      this.load();
    },
     methods: {
          load() {
               this.loading = true;

                this.$api.scholarship.getByPersonId(this.personId)
                .then(response => {
                    if (response.data) {
                        this.scholarshipData = [];

                        response.data.scholarshipDetails.forEach(el => {
                            var scholarship = new StudentScholarshipModel(el, this.$moment);
                            if(scholarship.periodicity == 1){
                               scholarship.periodicity = this.$t('studentScholarships.periodicity.monthly');
                            }
                            else{
                                scholarship.periodicity = this.$t('studentScholarships.periodicity.oneTimeOnly');
                            }
                            this.scholarshipData.push(scholarship);
                        });

                        this.hasStudentClassInCurrentYear = response.data.hasStudentClassInCurrentYear;
                    }
                })
                .catch(error => {
                    this.$notifier.error('', this.$t('errors.studentScholarshipsLoad'));
                    console.log(error.response);
                })
                .then(() => {
                    this.loading = false;
                });
          },
         async deleteScholarship(item){
            if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
              this.loading = true;

              this.$api.scholarship.delete(item.id)
              .then(() => {
                  this.$notifier.success('', this.$t('common.deleteSuccess'));
              })
              .catch(error => {
                  this.$notifier.error('', this.$t('errors.scholarshipDelete'));
                  console.log(error.response);
              })
              .then(() => {
                  this.load();
              });
            }
          }
     }
};
</script>
