<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row
        dense
        class="form-header"
      >
        <v-col
          cols="12"
          sm="6"
          lg="3"
        >
          <c-text-field
            v-if="disabled"
            :value="model.schoolYearName"
            :label="$t('recognition.schoolYear')"
            outlined
            persistent-placeholder
            class="required"
            dense
          />
          <school-year-selector
            v-else
            v-model="model.schoolYear"
            :label="$t('recognition.schoolYear')"
            class="required"
            :clearable="false"
            :rules="[$validator.required()]"
            show-current-school-year-button
            outlined
            persistent-placeholder
            dense
            :show-navigation-buttons="false"
          />
        </v-col>

        <v-col
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.ruoDocumentNumber"
          >
            <c-text-field
              v-model="model.ruoDocumentNumber"
              :label="$t('recognition.ruoDocumentNumber')"
              clearable
              :rules="[$validator.maxLength(100)]"
              outlined
              persistent-placeholder
              dense
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.ruoDocumentDate"
          >
            <date-picker
              id="ruoDocumentDate"
              ref="ruoDocumentDate"
              v-model="model.ruoDocumentDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('recognition.ruoDocumentDate')"
              :rules="model.ruoDocumentNumber
                ? [$validator.required()]
                : []"
              :class="model.ruoDocumentNumber ? 'required' :''"
              outlined
              persistent-placeholder
              dense
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.educationLevel"
          >
            <c-text-field
              v-if="disabled"
              :value="model.educationLeveName"
              :label="$t('recognition.educationLevel')"
              outlined
              persistent-placeholder
              class="required"
              dense
            />
            <custom-autocomplete
              v-else
              id="educationLevel"
              ref="educationLevel"
              v-model="model.educationLevelId"
              api="/api/lookups/GetRecognitionEducationLevel"
              :label="$t('recognition.educationLevel')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :defer-options-loading="false"
              :rules="[$validator.required()]"
              class="required"
              outlined
              persistent-placeholder
              dense
            />
          </c-info>
        </v-col>
        <v-col
          v-if="showBasicClass"
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.basicClass"
          >
            <c-text-field
              v-if="disabled"
              :value="model.basicClassName"
              :label="$t('recognition.basicClass')"
              outlined
              persistent-placeholder
              :class="classEducationLevelSelected ? 'required' :''"
              dense
            />
            <v-autocomplete
              v-else
              id="basicClass"
              ref="basicClass"
              v-model="model.basicClassId"
              :items="basicClassOptions"
              :label="$t('recognition.basicClass')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :rules="classEducationLevelSelected
                ? [$validator.required()]
                : []"
              :class="classEducationLevelSelected ? 'required' :''"
              outlined
              persistent-placeholder
              dense
            />
          </c-info>
        </v-col>
        <v-col
          v-if="showTerm"
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.term"
          >
            <c-text-field
              v-if="disabled"
              :value="getTermText(model.term)"
              :label="$t('recognition.basicClass')"
              outlined
              persistent-placeholder
              :class="classEducationLevelSelected ? 'required' :''"
              dense
            />
            <v-select
              v-else
              id="term"
              ref="term"
              v-model="model.term"
              :items="termOptions"
              :label="$t('recognition.term')"
              clearable
              outlined
              persistent-placeholder
              dense
            />
          </c-info>
        </v-col>
        <v-col
          v-if="showProfession"
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.sppooProfession"
          >
            <c-text-field
              v-if="disabled"
              :value="model.sppooProfessionName"
              :label="$t('recognition.sppooProfession')"
              outlined
              persistent-placeholder
              :class="profQulilificationEduLevelSelected ? 'required' :''"
              dense
            />
            <custom-autocomplete
              v-else
              id="sppooProfession"
              ref="sppooProfession"
              v-model="model.sppooProfessionId"
              api="/api/lookups/GetSPPOOProfession"
              :label="$t('recognition.sppooProfession')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :defer-options-loading="false"
              :rules="profQulilificationEduLevelSelected
                ? [$validator.required()]
                : []"
              :class="profQulilificationEduLevelSelected ? 'required' :''"
              outlined
              persistent-placeholder
              dense
            />
          </c-info>
        </v-col>
        <v-col
          v-if="showSpeciality"
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.sppooSpeciality"
          >
            <c-text-field
              v-if="disabled"
              :value="model.sppooSpecialityName"
              :label="$t('recognition.sppooSpeciality')"
              outlined
              persistent-placeholder
              dense
            >
              <template #append>
                <v-chip
                  v-if="model.vetLevel"
                  color="light"
                  small
                  outlined
                >
                  {{ `${ $t('recognition.sppooSpecialityVetLevel') } ${ model.vetLevel }` }}
                </v-chip>
              </template>
            </c-text-field>
            <custom-autocomplete
              v-else
              id="sppooSpeciality"
              ref="sppooSpeciality"
              v-model="model.sppooSpecialityId"
              api="/api/lookups/GetSPPOOSpeciality"
              :label="$t('recognition.sppooSpeciality')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :defer-options-loading="false"
              :filter="specialityFilter"
              outlined
              persistent-placeholder
              dense
            >
              <template v-slot:item="{ item }">
                <v-list-item-content
                  v-text="item.text"
                />
                <v-list-item-icon>
                  <v-tooltip right>
                    <template #activator="{ on }">
                      <v-chip
                        v-if="item.vetLevel"
                        color="light"
                        small
                        outlined
                        v-on="on"
                      >
                        {{ item.vetLevel }}
                      </v-chip>
                    </template>
                    <span>{{ $t('recognition.sppooSpecialityVetLevel') }}</span>
                  </v-tooltip>
                </v-list-item-icon>
              </template>
              <template v-slot:selection="{ item }">
                <span>{{ item.text }} / </span>
                <v-tooltip right>
                  <template #activator="{ on }">
                    <v-chip
                      v-if="item.vetLevel"
                      color="light"
                      small
                      outlined
                      v-on="on"
                    >
                      {{ `${ $t('recognition.sppooSpecialityVetLevel') } ${ item.vetLevel }` }}
                    </v-chip>
                  </template>
                  <span>{{ $t('recognition.sppooSpecialityVetLevel') }}</span>
                </v-tooltip>
              </template>
            </custom-autocomplete>
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="1"
        >
          <c-info uid="recognition.isSelfEduForm">
            <c-text-field
              v-if="disabled"
              :class="`self-form-box ${model.isSelfEduForm ? 'checked' : ''} details`"
              :label="$t('recognition.isSelfEduForm')"
              outlined
              dense
            >
              <template #prepend-inner>
                <v-checkbox
                  v-model="model.isSelfEduForm"
                  class="mt-n1"
                  hide-details
                />
              </template>
            </c-text-field>
            <v-sheet
              v-else
              :class="`mt-n4 self-form-box ${model.isSelfEduForm ? 'checked' : ''}`"
              height="40"
              rounded
            >
              <v-checkbox
                v-model="model.isSelfEduForm"
                class="pa-2"
                :label="$t('recognition.isSelfEduForm')"
                :ripple="false"
              />
            </v-sheet>
          </c-info>
        </v-col>
        <!-- Марги каза -->
        <!-- <v-col
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.diplomaNumber"
          >
            <v-text-field
              v-model="model.diplomaNumber"
              :label="$t('recognition.diplomaNumber')"
              clearable
              :rules="[$validator.maxLength(100)]"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.diplomaDate"
          >
            <date-picker
              id="diplomaDate"
              ref="diplomaDate"
              v-model="model.diplomaDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('recognition.diplomaDate')"
              :rules="model.diplomaNumber
                ? [$validator.required()]
                : []"
              :class="model.diplomaNumber ? 'required' :''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info
            uid="recognition.institutionCountry"
          >
            <custom-autocomplete
              id="institutionCountry"
              ref="institutionCountry"
              v-model="model.institutionCountryId"
              api="/api/lookups/GetCountriesBySearchString"
              :label="$t('recognition.institutionCountry')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :defer-options-loading="false"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          lg="6"
        >
          <c-info
            uid="recognition.institutionName"
          >
            <v-text-field
              v-model="model.institutionName"
              :label="$t('recognition.institutionName')"
              :rules="[$validator.maxLength(1000)]"
            />
          </c-info>
        </v-col> -->
      </v-row>
      <v-row dense>
        <v-col>
          <file-manager
            v-if="!disabled || (model.documents && model.documents.length > 0)"
            v-model="model.documents"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
      <hr>
      <v-expansion-panels
        v-model="panelModel"
        multiple
        popout
        class="mt-4"
      >
        <v-expansion-panel>
          <v-expansion-panel-header>
            {{ $t('recognition.gradesFromRecognitionExam') }}
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-row
              v-if="requiredSubjects"
              dense
            >
              <v-col
                cols="12"
              >
                <v-alert
                  border="left"
                  colored-border
                  type="warning"
                  elevation="2"
                  dense
                >
                  <h4>{{ $t('recognition.requiredSubjects') }}</h4>
                  <v-row
                    align="center"
                    no-gutters
                  >
                    <v-col dense>
                      <custom-autocomplete
                        v-for="(requiredSubjectId, index) in requiredSubjects"
                        :key="index"
                        :value="requiredSubjectId"
                        api="/api/lookups/GetSubjectOptions"
                        defer-options-loading
                        disabled
                        dense
                      >
                        <template v-slot:item="data">
                          <v-list-item-content
                            v-text="data.item.text"
                          />
                          <v-list-item-icon>
                            <v-chip
                              color="light"
                              small
                              outlined
                            >
                              {{ data.item.value }}
                            </v-chip>
                          </v-list-item-icon>
                        </template>
                        <template v-slot:selection="data">
                          <span>{{ data.item.text }}</span>
                          <v-chip
                            color="light"
                            small
                            outlined
                          >
                            {{ data.item.value }}
                          </v-chip>
                        </template>
                      </custom-autocomplete>
                    </v-col>
                    <v-spacer />
                    <v-col class="shrink">
                      <button-tip
                        color="primary"
                        text="buttons.add"
                        tooltip="buttons.add"
                        iclass=""
                        outlined
                        bottom
                        small
                        @click="onRequiredSubjectsAdd"
                      />
                    </v-col>
                  </v-row>
                </v-alert>
              </v-col>
            </v-row>

            <draggable
              :list="model.equalizations"
              :disabled="disabled"
              :sort="true"
              ghost-class="ghost"
              @change="updateListSortOrder"
            >
              <transition-group>
                <v-alert
                  v-for="(item, index) in model.equalizations"
                  :key="item.uid"
                  class="pa-0 mb-0"
                  dense
                >
                  <template #prepend>
                    <v-col
                      align="center"
                      :style="{ 'max-width': '40px' }"
                      class="pa-0 mb-2"
                    >
                      <c-info
                        uid="recognition.equalization.isRequired"
                      >
                        <v-tooltip
                          top
                        >
                          <template v-slot:activator="{ on }">
                            <v-checkbox
                              v-model="item.isRequired"
                              disabled
                              dense
                              v-on="on"
                            />
                          </template>
                          <span>{{ $t('recognition.equalization.isRequired') }}</span>
                        </v-tooltip>
                      </c-info>
                    </v-col>
                  </template>
                  <template #close>
                    <v-col
                      align="center"
                      :style="{ 'max-width': '40px' }"
                      class="pa-0 mb-10"
                    >
                      <button-tip
                        v-if="!disabled && (!hasToValidateRequiredSubjects || !item.isRequired)"
                        icon
                        icon-color="error"
                        iclass=""
                        icon-name="mdi-delete"
                        tooltip="buttons.delete"
                        bottom
                        @click="onEqualizationRemove(index)"
                      />
                    </v-col>
                  </template>
                  <v-row
                    dense
                    class="small-text"
                  >
                    <v-col
                      cols="12"
                      sm="6"
                      md="4"
                    >
                      <c-info
                        uid="recognition.equalization.subject"
                      >
                        <c-text-field
                          v-if="disabled"
                          :value="item.subjectName"
                          :label="$t('recognition.equalization.subject')"
                          outlined
                          persistent-placeholder
                          class="required"
                          dense
                        >
                          <template #append>
                            <v-chip
                              v-if="item.subjectId"
                              color="light"
                              small
                              outlined
                            >
                              {{ `${ item.subjectId }` }}
                            </v-chip>
                          </template>
                        </c-text-field>
                        <custom-autocomplete
                          v-else
                          id="subject"
                          :ref="'subject' + item.index"
                          v-model="item.subjectId"
                          api="/api/lookups/GetSubjectOptions"
                          :label="$t('recognition.equalization.subject')"
                          :placeholder="$t('buttons.search')"
                          defer-options-loading
                          hide-no-data
                          hide-selected
                          remove-items-on-clear
                          :disabled="disabled"
                          :rules="[$validator.required()]"
                          clearable
                          class="required"
                          outlined
                          persistent-placeholder
                          dense
                          append-icon=""
                        >
                          <template v-slot:item="data">
                            <v-list-item-content
                              v-text="data.item.text"
                            />
                            <v-list-item-icon>
                              <v-chip
                                color="light"
                                small
                                outlined
                              >
                                {{ data.item.value }}
                              </v-chip>
                            </v-list-item-icon>
                          </template>
                          <template v-slot:selection="data">
                            <span>{{ data.item.text }}</span>
                            <v-chip
                              color="light"
                              small
                              outlined
                            >
                              {{ data.item.value }}
                            </v-chip>
                          </template>
                        </custom-autocomplete>
                      </c-info>
                    </v-col>
                    <v-col
                      cols="12"
                      sm="6"
                      md="2"
                    >
                      <c-text-field
                        v-if="disabled"
                        :value="item.subjectTypeName"
                        :label="$t('recognition.equalization.subjectType')"
                        class="required"
                        outlined
                        persistent-placeholder
                        dense
                      />
                      <v-autocomplete
                        v-else
                        v-model="item.subjectTypeId"
                        :items="subjectTypeOptions"
                        :label="$t('recognition.equalization.subjectType')"
                        :rules="[$validator.required()]"
                        class="required"
                        outlined
                        persistent-placeholder
                        dense
                        append-icon=""
                      >
                        <template v-slot:item="data">
                          <v-list-item-content
                            v-text="data.item.text"
                          />
                          <v-list-item-icon>
                            <v-chip
                              color="light"
                              small
                              outlined
                            >
                              {{ data.item.partName }}
                            </v-chip>
                          </v-list-item-icon>
                        </template>
                      </v-autocomplete>
                    </v-col>
                    <v-col>
                      <diploma-subject-grade-editor
                        :value="item"
                        outlined
                        persistent-placeholder
                        dense
                        append-icon=""
                      />
                    </v-col>
                    <v-col
                      v-if="profQulilificationEduLevelSelected"
                      cols="12"
                      sm="6"
                      md="2"
                    >
                      <v-text-field
                        v-if="disabled"
                        :value="item.basicClassName"
                        :label="$t('equalization.basicClass')"
                        outlined
                        persistent-placeholder
                        class="required"
                        dense
                      />
                      <c-info
                        v-else
                        uid="equalization.basicClass"
                      >
                        <v-autocomplete
                          v-model="item.basicClassId"
                          :items="basicClassOptions"
                          :label="$t('recognition.equalization.basicClass')"
                          :placeholder="$t('buttons.search')"
                          clearable
                          hide-no-data
                          hide-selected
                          :rules="[$validator.required()]"
                          class="required"
                          outlined
                          persistent-placeholder
                          dense
                          append-icon=""
                        />
                      </c-info>
                    </v-col>
                    <!-- Марги каза -->
                    <!-- <v-col>
                      <c-info
                        uid="recognition.equalization.originalSubject"
                      >
                        <v-text-field
                          v-model="item.originalSubject"
                          :label="$t('recognition.equalization.originalSubject')"
                          :rules="[$validator.maxLength(400)]"
                          outlined
                          persistent-placeholder
                          dense
                        />
                      </c-info>
                    </v-col>
                    <v-col>
                      <c-info
                        uid="recognition.equalization.originalGrade"
                      >
                        <v-text-field
                          v-model="item.originalGrade"
                          :label="$t('recognition.equalization.originalGrade')"
                          :rules="[$validator.maxLength(100)]"
                        />
                      </c-info>
                    </v-col> -->
                  </v-row>
                </v-alert>
              </transition-group>
            </draggable>

            <v-row
              v-if="!disabled"
              dense
            >
              <v-col
                cols="12"
              >
                <button-tip
                  color="primary"
                  text="recognition.equalization.addNew"
                  tooltip="recognition.equalization.addNew"
                  iclass=""
                  outlined
                  bottom
                  small
                  @click="onEqualizationAdd"
                />
              </v-col>
            </v-row>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-form>
  </div>
</template>

<script>
import FileManager from '@/components/common/FileManager.vue';
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import DiplomaSubjectGradeEditor from '@/components/diplomas/DiplomaSubjectGradeEditor';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Draggable from 'vuedraggable';
import { RecognitionModel } from '@/models/recognitionModel';
import { RecognitionEducationLevel } from '@/enums/enums';
import { mapGetters } from 'vuex';
import groupBy from 'lodash.groupby';
import Constants from '@/common/constants.js';

export default {
  name: "RecognitionForm",
  components: {
    FileManager,
    CustomAutocomplete,
    SchoolYearSelector,
    DiplomaSubjectGradeEditor,
    Draggable
  },
  props: {
    document: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: this.document ?? new RecognitionModel(),
      panelModel: [0],
      requiredSubjectsByEducationLevel: null,
      basicClassOptions: [],
      grades: [],
      subjectTypeOptions: [],
      requiredSubjects: null,
      requiredSubjectsInfo: null,
      hasToValidateRequiredSubjects: undefined,
    };
  },
  computed: {
    ...mapGetters(['termOptions']),
    specialityFilter() {
      return { relatedObject: this.model.sppooProfessionId };
    },
    profQulilificationEduLevelSelected() {
      return this.model && this.model.educationLevelId === RecognitionEducationLevel.ProfessionalQualification;
    },
    classEducationLevelSelected() {
      return this.model && this.model.educationLevelId === RecognitionEducationLevel.Class;
    },
    showProfession() {
      return this.profQulilificationEduLevelSelected;
    },
    showSpeciality() {
      return this.showProfession;
    },
    showBasicClass() {
      return this.classEducationLevelSelected;
    },
    showTerm() {
      return this.showBasicClass;
    },
  },
  watch: {
    'model.sppooProfessionId': {
      handler() {
        if(this.model && !this.model.sppooProfessionId && this.model.sppooSpecialityId) {
          this.model.sppooSpecialityId = null;
        }
      },
    },
    'model.educationLevelId': {
      handler() {
        if(this.model && !this.profQulilificationEduLevelSelected) {
          if(this.model.sppooProfessionId) this.model.sppooProfessionId = null; // Това ще активира горния watcher, което ще нулира и sppooSpecialityId

          // if(this.model.equalizations) {
          //   for (const item of this.model.equalizations.filter(x => x.basicClassId)) {
          //     item.basicClassId = null;
          //   }

          //   return;
          // }
        }

        if(this.model && !this.classEducationLevelSelected) {
          if(this.model.basicClassId) this.model.basicClassId = null;
          if(this.model.term) this.model.term = null;
        }


        this.validateRequiredSubjects();
      },
    },
    'model.basicClassId': {
      handler() {
        this.validateRequiredSubjects();
      },
    }
  },
  mounted() {
    this.loadAppConfiguration();
    this.loadRecognitionRequiredSubjects();
    this.loadBasicClassOptions();
    this.loadSubjectTypeOptions();
    this.loadGrades();
  },
  methods: {
    loadAppConfiguration() {
      this.$api.appConfiguration.getValueByKey('RecognitionRequiredSubjectsValidation')
      .then(response => {
        this.hasToValidateRequiredSubjects = response.data ? (response.data.toLowerCase() == "true") : false;
      })
      .catch(error => {
        console.log(error.response);
      });
    },
    loadRecognitionRequiredSubjects() {
      this.$api.recognition.getRecognitionRequiredSubjects()
      .then(response => {
        if(response.data) {
          this.requiredSubjectsByEducationLevel = JSON.parse(response.data);
        }
      })
      .catch(error => {
        console.log(error.response);
      });
    },
    loadBasicClassOptions() {
      this.$api.lookups.getBasicClassOptions({ minId: 1, maxId: 15})
      .then(response => {
        if(response.data) {
          this.basicClassOptions = response.data;
        }
      })
      .catch(error => {
        console.log(error.response);
      });
    },
    loadGrades() {
      this.$api.lookups.getGradesByBasicClassId({ filterSpecialGrade: true })
      .then(response => {
        if(response.data) {
          this.grades = response.data;
        }
      })
      .catch(error => {
        console.log(error.response);
      });
    },
    loadSubjectTypeOptions() {
      this.$api.lookups.getSubjectTypeOptions()
        .then((response) => {
          if (response.data) {
            const groups = groupBy(response.data, 'partName');

            let options = [];
            for (const key in groups) {
              options.push({ header: key });
              options = [...options, ...groups[key].map(x => {
                return { value: x.value, text: x.text, partId: x.partId, partName: x.partName };
              })];
              options.push({ divider: true });
            }

            this.subjectTypeOptions = options;
          }
        })
        .catch((error) => {
          console.log(error);
        });
    },
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
    onEqualizationAdd() {
      if(!this.model.equalizations) this.model.equalizations = [];

      this.model.equalizations.push({
        id: null,
        subjectId: null,
        gradeCategory: 1,
        grade: null,
        specialNeedsGrade: null,
        otherGrade: null,
        originalSubject: null,
        originalGrade: null,
        isRequired: false,
        uid: this.$uuid.v4()
      });

      this.updateListSortOrder();
    },
    onEqualizationRemove(index) {
      if(!this.model.equalizations) return; // Няма от какво да махаме
      this.model.equalizations.splice(index,1);
      this.updateListSortOrder();
    },
    validateRequiredSubjects() {
      const requiredSubjects = this.getRequiredSubjects(this.model.educationLevelId, this.model.basicClassId);
      this.requiredSubjectsInfo = null;

      if(!requiredSubjects) {
        // Няма задължителни предмети за избраното образователно
        if(this.model.equalizations) {
          for (const item of this.model.equalizations.filter(x => x.isRequired === true)) {
            item.isRequired = false;
          }

          return;
        }
      }

      if(!this.model.equalizations) this.model.equalizations = [];

      for (const basicSubjectId of requiredSubjects) {
        const find = this.model.equalizations.find(x => x.subjectId === basicSubjectId);
        if(find) {
          find.isRequired = true;
        } else {
          if(this.hasToValidateRequiredSubjects) {
            this.addRequiredSubject(basicSubjectId);
          }
        }
      }
    },
    getRequiredSubjectsByEduLevel(educationLevelId) {
      if(!this.requiredSubjectsByEducationLevel || !educationLevelId) return null;
      return this.requiredSubjectsByEducationLevel[educationLevelId];
    },
    getRequiredSubjects(educationLevelId, basicClass) {
      const byEduLevel = this.getRequiredSubjectsByEduLevel(educationLevelId);
      if(!byEduLevel) return null;
      const requiredSubjects = Array.isArray(byEduLevel)
        ? byEduLevel
        : byEduLevel[basicClass];

        this.requiredSubjects = requiredSubjects;

        return requiredSubjects;
    },
    addRequiredSubject(subjectId) {
      if(!this.model.equalizations) this.model.equalizations = [];

      // Добавяне на задължителна оценка в списъка с "Оценки от Приравняване и признаване"
      this.model.equalizations.splice(0,0, {
        id: null,
        subjectId: subjectId,
        subjectTypeId: Constants.OopSubjectTypeId, // OOP
        gradeCategory: 1,
        grade: null,
        specialNeedsGrade: null,
        otherGrade: null,
        originalSubject: null,
        originalGrade: null,
        isRequired: true,
        uid: this.$uuid.v4()
      });
    },
    onRequiredSubjectsAdd() {
      if(!this.model.equalizations) this.model.equalizations = [];

      // подредени в desc ред. addRequiredSubject ги добавя най-отпред
      for (const requiredSubjectId of this.requiredSubjects.sort()) {
        const find = this.model.equalizations.find(x => x.subjectId === requiredSubjectId);
        if(find) {
          find.isRequired = true;
        } else {
          this.addRequiredSubject(requiredSubjectId);
        }
      }

      this.updateListSortOrder();
    },
    updateListSortOrder () {
      if (!this.model || !this.model.equalizations) return;

      const newList = [...this.model.equalizations].map((item, index) => {
        const newSort = index + 1;
       // also add in a new property called has changed if you want to style them / send an api call
        item.hasChanged = item.sortOrder !== newSort;
        if (item.hasChanged) {
          item.sortOrder = newSort;
        }
        return item;
      });

      this.model.equalizations = newList;
    },
    getTermText(term) {
      if(!this.termOptions) return '';

      return this.termOptions.find(x => x.value === term)?.text ?? '';
    }
  }
};
</script>

<style lang="scss" scoped>
  ::v-deep {
    .small-text {
      & .v-select__slot {
        font-size: 0.8em !important;
      }
      & .v-text-field__slot {
        font-size: 0.8em !important;
      }
    }

    .self-form-box label {
      color: rgb(93 76 76);
    }
  }
  
  .self-form-box {
    background-color: inherit;
    transition: background-color 500ms ease-in;
  }

  .self-form-box.checked {
    background-color: rgb(10 108 255 / 31%);
    transition: background-color 500ms ease-in;
  }
  
  .self-form-box.details {
    height: 40px;
  }

  .form-header .col-12 {
    height: 64px;
  }
</style>
