<template>
  <v-card>
    <v-card-title>
      {{ $t("recognition.title") }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        :headers="headers"
        :items="recognitions"
        :loading="loading"
        :search="search"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
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
        <template v-slot:[`item.id`]="{ item }">
          <doc-downloader
            v-for="document in item.documents"
            :key="document.id"
            :value="document"
            small
          />
        </template>
        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
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
              :to="`/student/${pid}/recognition/${item.id}/details`"
            />
            <button-tip
              v-if="hasManagePermission"
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              tooltip="buttons.edit"
              bottom
              iclass=""
              small
              :disabled="saving"
              :to="`/student/${pid}/recognition/${item.id}/edit`"
            />
            <button-tip
              v-if="hasManagePermission"
              icon
              icon-name="mdi-delete"
              icon-color="error"
              tooltip="buttons.delete"
              bottom
              iclass=""
              small
              :disabled="saving"
              @click="deleteItem(item)"
            />
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasManagePermission"
              small
              color="primary"
              :to="`/student/${pid}/recognition/create`"
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
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import DocDownloader from '@/components/common/DocDownloader.vue';
import { RecognitionModel } from '@/models/recognitionModel';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentRecognitionsList',
  components: {
    DocDownloader
  },
  props: {
    pid: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      saving: false,
      search: '',
      loading: false,
      recognitions: [],
      headers: [
        {
          text: this.$t("recognition.headers.schoolYear"),
          value: "schoolYearName",
          sortable: false,
        },
        {text: this.$t('recognition.headers.educationLevel'), value: "educationLevel", sortable: true},
        {text: this.$t('recognition.headers.basicClass'), value: "basicClass", sortable: true},
        {text: this.$t('recognition.headers.term'), value: "termName", sortable: true},
        {text: this.$t('recognition.headers.sppooProfession'), value: "sppooProfession", sortable: true},
        {text: this.$t('recognition.headers.sppooSpeciality'), value: "sppooSpeciality", sortable: true},
        {text: this.$t('recognition.headers.ruoDocumentNumber'), value: "ruoDocumentNumber", sortable: true},
        {text: this.$t('recognition.headers.ruoDocumentDate'), value: "ruoDocumentDate", sortable: true},
        {text: this.$t('recognition.headers.documents'), value: "id", sortable: false, filterable: false},
        // {text: this.$t('recognition.headers.diplomaNumber'), value: "diplomaNumber", sortable: true},
        // {text: this.$t('recognition.headers.diplomaDate'), value: "diplomaDate", sortable: true},
        // {text: this.$t('recognition.headers.institutionCountry'), value: "institutionCountry", sortable: true},
        // {text: this.$t('recognition.headers.institutionName'), value: "institutionName", sortable: true},
        {text: this.$t("common.actions"), value: 'actions', sortable: false, align: 'end'},
      ]
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentRecognitionRead);
    },
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentRecognitionManage);
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentRecognitionRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
    this.load();
  },
  methods: {
    load() {
          this.loading = true;
          this.$api.recognition
            .getListForPerson(this.pid)
            .then((response) => {
              if(response.data) {
                this.recognitions = response.data.map((el) => new RecognitionModel(el, this.$moment));
              }
            })
            .catch((error) => {
              this.$notifier.error('', this.$t('documents.otherDocumentsLoadErrorMessage'));
              console.log(error.response);
            })
            .then(() => { this.loading = false; });
        },

    async deleteItem(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.recognition.delete(item.id)
          .then(() => {
            this.load();
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('errors.fileDelete'));
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
      }
    }
  }
};
</script>
