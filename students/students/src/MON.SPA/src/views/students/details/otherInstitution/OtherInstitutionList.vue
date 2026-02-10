<template>
  <v-card>
    <v-card-title>
      {{ this.$t('studentOtherInstitutions.title') }}
    </v-card-title>

    <v-card-text>
      <v-data-table
        :items="otherInstitutionsList" 
        :headers="headers"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="otherInstitutionsList"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :file-name="$t('lod.awards.studentAwardsTitle')"
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
        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <template>
              <button-tip
                v-if="hasReadPermission"
                icon
                icon-name="mdi-eye"
                icon-color="primary"
                tooltip="buttons.review"
                bottom
                iclass=""
                small
                :disabled="saving"
                :to="`/student/${personId}/otherInstitutions/${item.id}/details`"
              />
              <button-tip
                v-if="hasManagePermission"
                icon
                icon-name="mdi-pencil"
                icon-color="primary"
                tooltip="buttons.edit"
                iclass=""
                small
                bottom
                :to="`/student/${personId}/otherInstitutions/${item.id}/edit`"
              />
              <button-tip
                v-if="hasManagePermission"
                icon
                icon-name="mdi-delete"
                icon-color="error"
                tooltip="buttons.delete"
                iclass=""
                small
                bottom
                @click="deleteOtherInstitution(item)"
              />
            </template>
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasManagePermission"
              small
              color="primary"
              :to="`/student/${personId}/otherInstitutions/create`"
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
      <confirm-dlg ref="confirm" />
      <v-overlay :value="saving">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </v-overlay>
    </v-card-text>
  </v-card>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import { StudentOtherInstitutionModel } from '@/models/studentOtherInstitutionModel.js';

export default {
name: 'OtherInstitutionList',
components: { 
    GridExporter,
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
        search:'',
        saving: false,
        otherInstitutionsList: [],
        headers: [
                { 
                    text: this.$t('studentOtherInstitutions.reasonLabel'), 
                    value: 'reason' 
                },
                { 
                    text: this.$t('common.validFrom'), 
                    value: 'validFrom' 
                },
                { 
                    text: this.$t('common.validTo'), 
                    value: 'validTo' 
                },
                { 
                    text: this.$t('studentOtherInstitutions.institution'), 
                    value: 'institution.text' 
                },
            { text: '', value: 'actions', sortable: false, align: 'end' }
        ],
    };
},
computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions']),
    hasReadPermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentOtherInstitutionRead);
    },
    hasManagePermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentOtherInstitutionManage);
    },
},
mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherInstitutionRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
    this.load();
},
methods: {
    load() {
      this.loading = true;

      this.$api.otherInstitution.getStudentOtherInstitutions(this.personId)
      .then(response => {
        if (response.data) {

           this.otherInstitutionsList = [];

           response.data.forEach(el => {
               const otherInstitutionModel = new StudentOtherInstitutionModel(el, this.$moment);
               this.otherInstitutionsList.push(otherInstitutionModel);
           });
        }
      })
      .catch(error => {
          this.$notifier.error('', this.$t('studentOtherInstitutions.loadError'));
          console.log(error.response);
      })
      .then(() => {
          this.loading = false; 
      });
    },
    async deleteOtherInstitution(item){
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.otherInstitution.delete(item.id)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.load();
        })
        .catch(error => {
            this.$notifier.error('', this.$t('studentOtherInstitutions.deleteError'));
            console.log(error.response);
        })
        .then(() => { this.saving = false; });
      }
    },
  } 
};
</script>