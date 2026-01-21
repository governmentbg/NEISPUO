<template>
  <v-container
    fluid
    ma-0
    pa-0
  >
    <v-card v-if="classGroup">
      <v-card-title
        class="py-0"
      >
        <v-col
          class="pl-0"
        >
          {{ classGroupHeaderName }}
          <v-chip
            v-if="classGroup.isValid !== true"
            color="error"
            outlined
            small
            class="ml-2"
          >
            {{ $t('institution.details.invalidClass') }}
          </v-chip>
        </v-col>
        <v-col
          class="text-right pr-0"
        >
          <button-tip
            v-if="isDebug"
            color="default"
            icon
            icon-color="default"
            icon-name="as fa-shield-alt"
            iclass=""
            tooltip="profile.permissions"
            bottom
            @click="dialog = true"
          />
        </v-col>
      </v-card-title>
      <v-card-subtitle>
        <v-expansion-panels
          v-model="panel"
          multiple
          flat
          class="ma-0"
        >
          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ `${classGroup.schoolYearName} / ${classGroup.institutionName}` }}
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <v-simple-table
                dense
              >
                <template v-slot:default>
                  <tbody>
                    <tr>
                      <td>{{ $t('common.schoolYear') }}</td>
                      <td>{{ classGroup.schoolYearName }}</td>
                    </tr>
                    <tr>
                      <td>{{ $t('common.institution') }}</td>
                      <td>{{ classGroup.institutionName }}</td>
                    </tr>
                    <tr>
                      <td>{{ $t('institution.details.headers.eduForm') }}</td>
                      <td>{{ classGroup.classEduFormName }}</td>
                    </tr>
                    <tr>
                      <td>{{ $t('institution.details.headers.classType') }}</td>
                      <td>{{ classGroup.classTypeName }}</td>
                    </tr>
                    <tr>
                      <td>{{ $t('documents.specialty') }}</td>
                      <td>{{ classGroup.classSpecialityName }}</td>
                    </tr>
                  </tbody>
                </template>
              </v-simple-table>
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-card-subtitle>
    </v-card>
    <app-loader
      v-else
    />
    <v-dialog
      v-model="dialog"
    >
      <v-card>
        <v-card-title class="text-h5">
          {{ $t('profile.permissions') }}
        </v-card-title>

        <v-card-text>
          <pre>{{ permissionsForClass }}</pre>
        </v-card-text>

        <v-divider />

        <v-card-actions>
          <v-spacer />
          <v-btn
            color="primary"
            text
            @click="dialog = false"
          >
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import AppLoader from '@/components/wrappers/loader.vue';
import { mapGetters } from 'vuex';

export default {
    name: 'ClassProfile',
    components: { AppLoader },
    props:{
        classGroupId: {
          type: Number,
          default() {
            return null;
          }
        }
    },
    data()
    {
      return {
        classGroup: undefined,
        panel: [],
        dialog: false
      };
    },
    computed: {
      ...mapGetters(['isDebug', 'permissionsForClass']),
      classGroupHeaderName() {
        const classGroupHeaderName = this.classGroup.basicClassDescription ?
          `${this.classGroup.className} / ${this.classGroup.basicClassDescription}` : this.classGroup.className;

        return classGroupHeaderName;
      }
    },
    mounted(){
      this.load();
    },
    methods:{
      load() {
        this.$api.classGroup
          .getById(this.classGroupId)
          .then((response) => {
            this.classGroup = response.data;
          })
          .catch((error) => {
            console.log(error.response);
          });
      }
    }

};
</script>
