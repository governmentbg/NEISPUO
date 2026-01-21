<template>
  <div>
    <v-card
      v-if="currentTermGrades"
    >
      <v-card-text>
        <v-tabs
          v-model="currentGradesTab"
          color="success"
        >
          <v-tab
            key="firstTerm"
          >
            {{ $t('lod.evaluations.firstTermGrade') }}
          </v-tab>
          <v-tab
            key="secondTerm"
          >
            {{ $t('lod.evaluations.secondTermGrade') }}
          </v-tab>
          <v-tabs-items
            v-model="currentGradesTab"
          >
            <v-tab-item
              key="firstTerm"
            >
              <v-data-table
                :headers="headers"
                :items="currentTermGrades.filter(x => x.term === 1)"
                item-key="uid"
                class="transparent elevation-0 my-4"
                disable-sort
                :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions }"
                :items-per-page="-1"
                group-by="curriculumPart"
                show-group-by
                dense
              >
                <template v-slot:[`group.header`]="{ group, items, isOpen, toggle }">
                  <th :colspan="headers.length">
                    <v-row dense>
                      <v-icon
                        @click="toggle"
                      >
                        {{ isOpen ? 'mdi-chevron-up' : 'mdi-chevron-down' }}
                      </v-icon>
                      <span class="text-h6 ml-3">{{ group }}{{ ` - ${items.length} учебни предмета` }}</span>
                    </v-row>
                  </th>
                </template>
                <template v-slot:[`item.subjectName`]="{ item }">
                  <span>{{ item.subjectName }}</span>
                  <v-chip
                    color="light"
                    x-small
                    outlined
                    class="mx-1"
                  >
                    {{ item.subjectID }}
                  </v-chip>
                  <span>
                    {{ ` / ${item.subjectTypeName}` }}
                  </span>
                </template>
                <template v-slot:[`item.grades`]="{ item }">
                  <v-chip
                    v-for="grade in item.grades"
                    :key="grade.uid"
                    color="light"
                    small
                    outlined
                    class="mr-1"
                  >
                    {{ grade.gradeText }}
                  </v-chip>
                </template>
              </v-data-table>
            </v-tab-item>
            <v-tab-item
              key="secondTerm"
            >
              <v-data-table
                :headers="headers"
                :items="currentTermGrades.filter(x => x.term === 2)"
                item-key="uid"
                class="transparent elevation-0 my-4"
                disable-sort
                :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions }"
                :items-per-page="-1"
                group-by="curriculumPart"
                show-group-by
                dense
              >
                <template v-slot:[`group.header`]="{ group, items, isOpen, toggle }">
                  <th :colspan="headers.length">
                    <v-row dense>
                      <v-icon
                        @click="toggle"
                      >
                        {{ isOpen ? 'mdi-chevron-up' : 'mdi-chevron-down' }}
                      </v-icon>
                      <span class="text-h6 ml-3">{{ group }}{{ ` - ${items.length} учебни предмета` }}</span>
                    </v-row>
                  </th>
                </template>
                <template v-slot:[`item.subjectName`]="{ item }">
                  <span>{{ item.subjectName }}</span>
                  <v-chip
                    color="light"
                    x-small
                    outlined
                    class="mx-1"
                  >
                    {{ item.subjectID }}
                  </v-chip>
                  <span>
                    {{ ` / ${item.subjectTypeName}` }}
                  </span>
                </template>
                <template v-slot:[`item.grades`]="{ item }">
                  <v-chip
                    v-for="grade in item.grades"
                    :key="grade.uid"
                    color="light"
                    small
                    outlined
                    class="mr-1"
                  >
                    {{ grade.gradeText }}
                  </v-chip>
                </template>
              </v-data-table>
            </v-tab-item>
          </v-tabs-items>
        </v-tabs>
      </v-card-text>
    </v-card>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  name: 'RelocationDocumentCurrentGradesComponent',
  props: {
    docId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      grades: null,
      loading: false,
      currentGradesTab: null,
      currentTermGrades: null,
      headers: [
            {
                text: this.$t('studentEvaluations.tableSubjectHeader'),
                value: "subjectName",
                groupable: false
            },
            {
                text: this.$t('addCurriculum.headers.sectionDescription'),
                value: "curriculumPartName",
                groupable: false
            },
            {
                text: this.$t('relocationDocument.currentGrades'),
                value: "grades",
                sortable: false,
                groupable: false,
                filterable: false,
            },
            // {
            //     text: this.$t('relocationDocument.currentGrades'),
            //     value: "gradesString",
            //     sortable: false,
            //     groupable: false,
            //     filterable: false,
            // }
        ],
    };
  },
  computed: {
    ...mapGetters(["gridItemsPerPageOptions"])
},
  mounted() {
    this.loadCurrentGrades();
  },
  methods: {
    loadCurrentGrades(){
        this.loading = true;
        this.$api.relocationDocument.getStudentCurrentTermGrades(this.docId)
            .then((response) => {
                if(response.data) {
                    this.currentTermGrades = response.data.termGrades;
                }
            })
            .catch((err) => {
              this.$notifier.error('', this.$t('common.loadError'));
              console.log(err.response);
            }).finally(() => { this.loading = false; });
    },
  }
};
</script>
