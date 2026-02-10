<template>
  <v-card
    v-if="hasReadPermission || hasManagePermission"
    class="mt-2"
  >
    <v-card-title>{{ $t('environmentCharacteristics.relativesTitle') }}</v-card-title>
    <v-card-subtitle>{{ $t('environmentCharacteristics.relativesSubTitle') }}</v-card-subtitle>
    <v-card-text>
      <v-data-table
        ref="relativesListTable"
        :items="relativesData"
        :headers="headers"
        :loading="loading"
        :search="search"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="relativesData"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :file-name="$t('lod.sanctions.studentSanctionsTitle')"
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
          <button-group v-if="hasManagePermission">
            <button-tip
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              iclass=""
              small
              tooltip="buttons.edit"
              bottom
              @click="editItem(item)"
            />
            <button-tip
              icon
              icon-name="mdi-delete"
              icon-color="error"
              iclass=""
              small
              tooltip="buttons.delete"
              bottom
              @click="deleteRelative(item)"
            />
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group v-if="hasManagePermission">
            <v-btn
              small
              color="primary"
              :to="`/student/${personId}/environmentCharacteristics/relative/add`"
            >
              {{ $t('buttons.newRecord') }}
            </v-btn>
            <v-btn
              small
              color="secondary"
              outlined
              @click="loadData"
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
import { StudentRelativeModel } from "@/models/environmentCharacteristics/studentRelativeModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';

export default {
    name: 'RelativesList',
    components: {
        GridExporter
    },
    props: {
        personId: {
            type: Number,
            required: true
        }
    },
    data() {
        return {
             loading: false,
             headers: [
                    {
                        text: this.$t('environmentCharacteristics.pinType'),
                        value: 'pinType.name'
                    },
                    {
                        text: this.$t('environmentCharacteristics.pin'),
                        value: 'pin'
                    },
                    {
                        text: this.$t('studentTabs.firstName'),
                        value: 'firstName'
                    },
                    {
                        text:  this.$t('studentTabs.middleName'),
                        value: 'middleName'
                    },
                    {
                        text:  this.$t('studentTabs.lastName'),
                        value: 'lastName'
                    },
                    {
                        text: this.$t('environmentCharacteristics.studentRelationshipLabel'),
                        value: 'relativeType.name'
                    },
                    {
                        text:  this.$t('environmentCharacteristics.email'),
                        value: 'email'
                    },
                    {
                        text:  this.$t('studentTabs.phoneNumber'),
                        value: 'phoneNumber'
                    },
                    {
                        text: this.$t('studentTabs.address'),
                        value: 'address'
                    },
                    {
                        text: this.$t('environmentCharacteristics.employment'),
                        value: 'workStatus.name'
                    },
                    {
                        text: this.$t('environmentCharacteristics.education'),
                        value: 'educationType.name'
                    },
                    {
                        text: this.$t('environmentCharacteristics.notes'),
                        value: 'notes'
                    },
                    {
                        text: this.$t('studentTabs.description'),
                        value: 'description'
                    },
                    { text: '', value: 'actions', sortable: false, align: 'end' }
                ],
            relativesData:[],
            search:''
        };
    },
    computed: {
      ...mapGetters(['gridItemsPerPageOptions', 'hasStudentPermission']),
      hasReadPermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicRead);
      },
      hasManagePermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicManage);
      }
    },
    mounted() {
      this.loadData();
    },
    methods: {
           loadData() {
               this.loading = true;

                this.$api.environmentCharacteristics.getRelatives(this.personId)
                .then(response => {
                    if (response.data) {
                        this.relativesData = [];
                        response.data.forEach(el => {
                           var relative = new StudentRelativeModel(el, this);
                           this.relativesData.push(relative);
                        });
                    }
                })
                .catch(error => {
                    this.$notifier.error('', this.$t('errors.studentRelativeLoad'));
                    console.log(error.response);
                })
                .finally(() => {
                    this.loading = false;
                });
          },
         async deleteRelative(item){
              if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
                    this.loading = true;

                    this.$api.environmentCharacteristics.deleteRelative(item.id, this.personId).catch(error => {
                        this.$notifier.error('', this.$t('errors.environmentCharacteristicsRelativeDelete'));
                        console.log(error);
                    })
                    .finally(() => {
                        this.loadData();
                        this.$emit('reloadData');
                    });
              }
          },
          editItem(item){
            this.$router.push(`/student/${this.personId}/environmentCharacteristics/relative/${item.id}/edit`);
          }
     }
};
</script>
