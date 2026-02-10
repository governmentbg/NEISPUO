<!-- eslint-disable vue/valid-v-for -->
<template>
  <div>
    <v-card>
      <!-- <v-expansion-panels
        v-model="expandablePanelModel"
        popout
        class="mb-4"
      >
        <v-expansion-panel>
          <v-expansion-panel-header />
          <v-expansion-panel-content>
            {{ `isCplrInstitution: ${isCplrInstitution}` }}
            <hr />
            {{ `isCplrEnrollment: ${isCplrEnrollment}` }}
            <hr />
            {{ `isMainClassEnrollment: ${isMainClassEnrollment}` }}
            <hr />
            {{ `isAdditionalEnrollment: ${isAdditionalEnrollment}` }}
            <hr />
            {{ classGroup }}
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels> -->
      <v-card-title v-show="validationErrors?.length > 0">
        <v-toolbar flat>
          <!-- <span>{{ $t('common.enroll') }}</span> -->
          <v-spacer />
          <v-btn
            color="red"
            outlined
            @click="validationDialog = true"
          >
            <v-icon
              large
              class="mr-2"
            >
              mdi-alert-circle-outline
            </v-icon>
            Виж детайли на последната грешка
          </v-btn>
        </v-toolbar>
      </v-card-title>
      <v-card-text>
        <v-row dense>
          <v-col cols="5">
            <app-loader
              v-if="loadingStudentsForEnrollment"
              type="list-item-avatar-two-line,list-item-avatar-two-line,list-item-avatar-two-line,list-item-avatar-two-line,list-item-avatar-two-line"
            />
            <div v-else>
              <!-- {{ selectedToEnroll }} -->
              <v-toolbar flat>
                <v-tooltip bottom>
                  <template #activator="{ on }">
                    <span v-on="on">
                      <v-checkbox
                        v-model="selectAllToEnroll"
                        class="mt-0 pt-0 mr-3"
                        hide-details
                        :indeterminate="isIndeterminateToEnroll"
                        @change="toggleSelectAllToEnroll"
                      />
                    </span>
                  </template>
                  <span>{{ $t('buttons.checkAll') }} / {{ $t('buttons.uncheckAll') }}</span>
                </v-tooltip>
                <v-toolbar-title>
                  За записване
                </v-toolbar-title>
                <v-spacer />
                <v-text-field
                  v-model="selectedToEnrollSearch"
                  append-icon="mdi-magnify"
                  :label="$t('common.search')"
                  single-line
                  hide-details
                  clearable
                />
              </v-toolbar>
              <v-row
                v-if="classGroupsOptions && classGroupsOptions.length > 0"
                dense
                class="px-4"
              >
                <v-col>
                  <custom-autocomplete
                    v-model="classGroupFilter"
                    :items="classGroupsOptions"
                    :label="$t('enroll.class')"
                    hide-no-data
                    hide-details
                    outlined
                    clearable
                    dense
                  />
                </v-col>
              </v-row>
              <v-list
                two-line
                dense
              >
                <v-list-item-group
                  v-model="selectedToEnroll"
                  color="primary"
                  multiple
                >
                  <draggable
                    :list="studentsForEnrollment"
                    group="students"
                    filter=".ignore"
                    :disabled="true"
                  >
                    <v-list-item
                      v-for="(student) in sortedStudentsForEnrollment"
                      :key="`toEnroll-${student.pin}`"
                      :value="student"
                      dense
                      :class="classStudents.some(x => x.pin === student.pin && x.pinType === student.pinType) ? 'list-group-item ignore' : 'list-group-item'"
                      :readonly="classStudents.some(x => x.pin === student.pin && x.pinType === student.pinType)"
                      @click="selectedToUnenroll = []"
                    >
                      <template v-slot:default="{ active }">
                        <v-list-item-action
                          v-if="!student.id"
                          class="mr-3"
                        >
                          <v-checkbox
                            :input-value="active"
                            :disabled="classStudents.some(x => x.pin === student.pin && x.pinType === student.pinType)"
                          />
                        </v-list-item-action>

                        <v-list-item-avatar size="24">
                          <v-icon
                            v-if="student.id"
                            color="error"
                          >
                            mdi-minus-thick
                          </v-icon>
                          <v-icon
                            v-else
                            color="primary"
                          >
                            {{ student.icon ?? 'mdi-account-school' }}
                          </v-icon>
                        </v-list-item-avatar>
                        <v-list-item-content class="ml-2">
                          <!-- {{ `toEnroll-${student.pin}-${student.pinType}-${index}` }} -->
                          <v-list-item-title>{{ student.fullName }}</v-list-item-title>
                          <v-list-item-subtitle>{{ `${student.pin} - ${student.pinType} / ${student.personBirthDate ? $moment(student.personBirthDate).format(dateFormatDot) : ''} / ${student.personAge}г. ${student.mainClassName ? ` / ${student.mainClassName}` : ''}` }}</v-list-item-subtitle>
                          <v-divider class="mt-2" />
                        </v-list-item-content>
                        <v-list-item-action>
                          <button-tip
                            icon
                            icon-color="primary"
                            icon-name="mdi-information"
                            iclass=""
                            tooltip="student.details"
                            top
                            small
                            :to="`/student/${student.personId}/details`"
                          />
                        </v-list-item-action>
                      </template>
                    </v-list-item>
                  </draggable>
                </v-list-item-group>
              </v-list>
              <v-row
                v-if="studentsForEnrollment.length"
                dense
              >
                <v-col>
                  <v-pagination
                    v-model="page"
                    :length="Math.ceil(studentsForEnrollment.length / pageSize)"
                    :total-visible="10"
                  />
                </v-col>
              </v-row>
            </div>
          </v-col>
          <v-col
            cols="2"
            class="text-center"
          >
            <button-group
              class="fixed-to-top"
            >
              <button-tip
                icon
                icon-name="mdi-arrow-left-bold-outline"
                icon-color="primary"
                tooltip="enroll.removeFromClass"
                bottom
                iclass=""
                :disabled="!selectedToUnenroll.length || !selectedToUnenroll.some(x => !x.id)"
                @click="moveToUnenroll"
              />
              <button-tip
                icon
                icon-name="mdi-arrow-right-bold-outline"
                icon-color="primary"
                tooltip="enroll.moveToClass"
                bottom
                iclass=""
                :disabled="!selectedToEnroll.length"
                @click="moveToEnroll"
              />
            </button-group>
          </v-col>
          <v-col cols="5">
            <app-loader
              v-if="loadingClassStudents"
              type="list-item-avatar-two-line,list-item-avatar-two-line,list-item-avatar-two-line,list-item-avatar-two-line,list-item-avatar-two-line"
            />
            <div
              v-else
              class="fixed-to-top"
            >
              <!-- {{ selectedToUnenroll }} -->
              <v-toolbar flat>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <span v-on="on">
                      <v-checkbox
                        v-model="selectAllToUnenroll"
                        class="mt-0 pt-0 mr-3"
                        hide-details
                        :indeterminate="isIndeterminateToUnenroll"
                        @change="toggleSelectAllToUnenroll"
                      />

                    </span>
                  </template>
                  <span>{{ $t('buttons.checkAll') }} / {{ $t('buttons.uncheckAll') }}</span>
                </v-tooltip>
                <v-toolbar-title>Записани</v-toolbar-title>
                <v-spacer />
                <v-text-field
                  v-model="classStudentsSearch"
                  append-icon="mdi-magnify"
                  :label="$t('common.search')"
                  single-line
                  hide-details
                  clearable
                />
              </v-toolbar>
              <v-list
                two-line
                dense
                :style="{ minHeight: classStudents.length === 0 ? '200px' : 'auto' }"
              >
                <v-list-item-group
                  v-model="selectedToUnenroll"
                  color="primary"
                  multiple
                >
                  <draggable
                    :list="classStudents"
                    group="students"
                    filter=".ignore"
                    :style="{ minHeight: classStudents.length === 0 ? '250px' : 'auto' }"
                    class="draggable-area"
                    :disabled="true"
                  >
                    <!-- Empty state placeholder -->
                    <v-list-item
                      v-if="classStudents.length === 0"
                      class="empty-drop-zone"
                    >
                      <v-list-item-content class="text-center">
                        <v-list-item-title class="text--disabled">
                          <v-icon
                            color="grey lighten-2"
                            size="48"
                          >
                            mdi-drag
                          </v-icon>
                        </v-list-item-title>
                        <v-list-item-subtitle class="text--disabled no-ellipsis">
                          <h5>Преместете учениците за записване като използвате стрелките</h5>
                        </v-list-item-subtitle>
                      </v-list-item-content>
                    </v-list-item>
                    <v-list-item
                      v-for="(student) in sortedClassStudents"
                      :key="`toUnenroll-${student.pin}`"
                      :value="student"
                      dense
                      :class="student.id ? 'ignore' : ''"
                      :readonly="student.id"
                      @click="selectedToEnroll = []"
                    >
                      <template v-slot:default="{ active }">
                        <v-list-item-action
                          v-if="!student.id"
                          class="mr-3"
                        >
                          <v-checkbox
                            :input-value="active"
                          />
                        </v-list-item-action>
                        <v-list-item-avatar size="24">
                          <v-icon
                            v-if="!student.id"
                            color="success"
                          >
                            mdi-plus-thick
                          </v-icon>
                          <v-icon
                            v-else
                            color="primary"
                          >
                            {{ student.icon ?? 'mdi-account-school' }}
                          </v-icon>
                        </v-list-item-avatar>
                        <v-list-item-content class="ml-2">
                          <!-- {{ `toUnenroll-${student.pin}-${student.pinType}-${index}` }} -->
                          <v-list-item-title>{{ student.fullName }}</v-list-item-title>
                          <v-list-item-subtitle>
                            {{ `${student.pin} - ${student.pinType}${student.eduFormName ? ` / ${student.eduFormName}` : ''}` }}
                            <!-- <v-chip
                              :color="student.isLodApproved === true ? 'success' : 'light'"
                              outlined
                              small
                              label
                            >
                              <yes-no :value="student.isLodApproved" />
                            </v-chip> -->
                          </v-list-item-subtitle>
                          <v-divider class="mt-2" />
                        </v-list-item-content>
                        <v-list-item-action>
                          <button-tip
                            icon
                            icon-color="primary"
                            icon-name="mdi-information"
                            iclass=""
                            tooltip="student.details"
                            top
                            small
                            :to="`/student/${student.personId}/details`"
                          />
                        </v-list-item-action>
                      </template>
                    </v-list-item>
                  </draggable>
                </v-list-item-group>
              </v-list>
              <v-row
                v-if="classStudents.length"
                dense
              >
                <v-col>
                  <v-pagination
                    v-model="secondaryPage"
                    :length="Math.ceil(classStudents.length / pageSize)"
                    :total-visible="10"
                  />
                </v-col>
              </v-row>
            </div>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions
        class="fixed-to-bottom"
      >
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="onEnroll"
        >
          <v-icon left>
            fas fa-save
          </v-icon>
          {{ $t('buttons.save') }}
        </v-btn>

        <v-btn
          raised
          color="error"
          :to="`/class/${classId}/details?schoolYear=${schoolYear}`"
        >
          <v-icon left>
            fas fa-times
          </v-icon>
          {{ $t('buttons.cancel') }}
        </v-btn>
      </v-card-actions>>
    </v-card>
    <v-overlay :value="enrollment">
      <v-row justify="center">
        <v-progress-circular
          :value="enrollmentProgressPercentage"
          color="primary"
          size="128"
          width="13"
        >
          <h2 class="white--text">
            {{ `${processedStudentsCount}/${sudentsToProcess.length}` }}
          </h2>
        </v-progress-circular>
      </v-row>
      <div class="text-center mt-5">
        <h3>Записване на:</h3>
        <h3>{{ `${processedStudent?.fullName} / ${processedStudent?.pin} - ${processedStudent?.pinType}` }}</h3>
      </div>
    </v-overlay>
    <v-dialog
      v-model="enrollmentDialog"
      min-width="800px"
      persistent
    >
      <v-card
        v-if="enrollmentModel"
        class="my-2"
      >
        <v-card-text>
          <v-form
            ref="enrollmentForm"
          >
            <div
              v-if="isMainClassEnrollment"
            >
              <v-card flat>
                <v-card-title>
                  {{ $t("enroll.basicData") }}
                </v-card-title>
                <v-card-text>
                  <!-- {{ enrollmentModel }} -->
                  <v-row dense>
                    <v-col
                      v-if="classGroup"
                      cols="12"
                      sm="6"
                      lg="3"
                    >
                      <c-text-field
                        :value="classGroup.schoolYearName"
                        :label="$t('common.schoolYear')"
                        persistent-placeholder
                        class="required"
                        disabled
                      />
                    </v-col>
                    <v-col
                      cols="12"
                      sm="6"
                      lg="3"
                    >
                      <date-picker
                        id="enrollmentDate"
                        ref="enrollmentDate"
                        v-model="enrollmentModel.enrollmentDate"
                        :show-buttons="false"
                        :scrollable="false"
                        :no-title="true"
                        :show-debug-data="false"
                        :rules="[$validator.required()]"
                        :label="$t('documents.admissionDateLabel')"
                        class="required"
                      />
                    </v-col>
                    <v-col
                      v-if="classGroup"
                      cols="12"
                      sm="6"
                      lg="6"
                    >
                      <c-text-field
                        :value="`${classGroup.basicClassRomeName} - ${classGroup.className} - ${classGroup.classEduFormName} - ${classGroup.classTypeName}`"
                        :label="$t('enroll.class')"
                        persistent-placeholder
                        class="required"
                        disabled
                      />
                    </v-col>
                    <v-col
                      v-if="!isKindergartenInstitution"
                      cols="12"
                      sm="6"
                      lg="3"
                    >
                      <c-text-field
                        :value="classGroup.basicClassDescription"
                        :label="$t('student.basicClass')"
                        persistent-placeholder
                        disabled
                      />
                    </v-col>
                    <v-col
                      v-if="!isKindergartenInstitution"
                      cols="12"
                      sm="6"
                      lg="3"
                    >
                      <c-text-field
                        :value="classGroup.classEduFormName"
                        :label="$t('enroll.classEduFormName')"
                        persistent-placeholder
                        disabled
                        class="required"
                      />
                    </v-col>
                    <v-col
                      v-if="!isKindergartenInstitution"
                      cols="12"
                      sm="6"
                      lg="3"
                    >
                      <c-text-field
                        :value="classGroup.classProfessionName"
                        :label="$t('student.profession')"
                        persistent-placeholder
                        disabled
                      />
                    </v-col>
                    <v-col
                      v-if="!isKindergartenInstitution"
                      cols="12"
                      sm="6"
                      lg="3"
                    >
                      <c-text-field
                        :value="classGroup.classSpecialityName"
                        :label="$t('student.speciality')"
                        persistent-placeholder
                        disabled
                      />
                    </v-col>
                    <v-col
                      v-if="isKindergartenInstitution"
                      cols="12"
                      sm="6"
                      lg="3"
                    >
                      <date-picker
                        id="entryDate"
                        ref="entryDate"
                        v-model="enrollmentModel.entryDate"
                        :show-buttons="false"
                        :scrollable="false"
                        :no-title="true"
                        :show-debug-data="false"
                        :label="$t('documents.entryDateLabel')"
                      />
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
              <v-card
                v-if="enrollmentModel"
                class="mt-2"
              >
                <v-card-title>
                  {{ $t('enroll.additionalData') }}
                </v-card-title>
                <v-card-text>
                  <v-row dense>
                    <v-col>
                      <custom-autocomplete
                        v-model="enrollmentModel.commuterTypeId"
                        :api="'/api/lookups/GetCommuterOptions'"
                        :label="$t('enroll.traveling')"
                        hide-no-data
                        :defer-options-loading="false"
                        :rules="[$validator.required()]"
                        class="required"
                        disabled
                      />
                    </v-col>
                    <v-col
                      v-if="!isKindergartenInstitution"
                    >
                      <custom-autocomplete
                        v-model="enrollmentModel.repeaterId"
                        :api="'/api/lookups/GetRepeaterReasons'"
                        :label="$t('enroll.reEnter')"
                        hide-no-data
                        :defer-options-loading="false"
                        :rules="[$validator.required()]"
                        class="required"
                        disabled
                      />
                    </v-col>
                    <v-col>
                      <v-checkbox
                        v-model="enrollmentModel.hasIndividualStudyPlan"
                        :label="$t('enroll.individualCurriculum')"
                        color="primary"
                        disabled
                      />
                    </v-col>
                    <v-col
                      v-if="isKindergartenInstitution"
                    >
                      <v-checkbox
                        v-model="enrollmentModel.isHourlyOrganization"
                        :label="$t('enroll.hourlyOrganization')"
                        color="primary"
                        disabled
                      />
                    </v-col>
                    <v-col
                      v-if="!isKindergartenInstitution"
                    >
                      <v-checkbox
                        v-model="enrollmentModel.isNotForSubmissionToNra"
                        :label="$t('enroll.nraNotSubmitted')"
                        color="primary"
                        disabled
                      />
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </div>
            <v-card
              v-else-if="isAdditionalEnrollment"
              flat
            >
              <v-card-title>
                {{ $t("enroll.title", { institution: classGroup.institutionName }) }}
              </v-card-title>
              <v-card-text>
                <!-- {{ enrollmentModel }} -->
                <v-row dense>
                  <v-col
                    v-if="classGroup"
                    cols="12"
                    sm="6"
                    lg="3"
                  >
                    <c-text-field
                      :value="classGroup.schoolYearName"
                      :label="$t('common.schoolYear')"
                      persistent-placeholder
                      class="required"
                      disabled
                    />
                  </v-col>
                  <v-col
                    cols="12"
                    sm="6"
                    lg="3"
                  >
                    <date-picker
                      id="enrollmentDate"
                      ref="enrollmentDate"
                      v-model="enrollmentModel.enrollmentDate"
                      :show-buttons="false"
                      :scrollable="false"
                      :no-title="true"
                      :show-debug-data="false"
                      :rules="[$validator.required()]"
                      :label="$t('documents.admissionDateLabel')"
                      class="required"
                    />
                  </v-col>
                  <v-col
                    v-if="classGroup"
                    cols="12"
                    sm="6"
                    lg="6"
                  >
                    <c-text-field
                      :value="`${classGroup.basicClassRomeName} - ${classGroup.className} - ${classGroup.classEduFormName} - ${classGroup.classTypeName}`"
                      :label="$t('enroll.class')"
                      persistent-placeholder
                      class="required"
                      disabled
                    />
                  </v-col>
                  <v-col
                    cols="12"
                    sm="6"
                    lg="3"
                  >
                    <c-text-field
                      :value="classGroup.basicClassDescription"
                      :label="$t('student.basicClass')"
                      persistent-placeholder
                      disabled
                    />
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>
            <v-card
              v-else-if="isCplrEnrollment"
              flat
            >
              <v-card-title>
                {{ $t("enroll.title", { institution: classGroup.institutionName }) }}
              </v-card-title>
              <v-card-text>
                <!-- {{ enrollmentModel }} -->
                <v-row dense>
                  <v-col
                    v-if="classGroup"
                    cols="12"
                    sm="6"
                    lg="3"
                  >
                    <c-text-field
                      :value="classGroup.schoolYearName"
                      :label="$t('common.schoolYear')"
                      persistent-placeholder
                      class="required"
                      disabled
                    />
                  </v-col>
                  <v-col
                    cols="12"
                    sm="6"
                    lg="3"
                  >
                    <date-picker
                      id="enrollmentDate"
                      ref="enrollmentDate"
                      v-model="enrollmentModel.enrollmentDate"
                      :show-buttons="false"
                      :scrollable="false"
                      :no-title="true"
                      :show-debug-data="false"
                      :rules="[$validator.required()]"
                      :label="$t('documents.admissionDateLabel')"
                      class="required"
                    />
                  </v-col>
                  <v-col
                    v-if="classGroup"
                    cols="12"
                    sm="6"
                    lg="6"
                  >
                    <c-text-field
                      :value="`${classGroup.basicClassRomeName} - ${classGroup.className} - ${classGroup.classEduFormName} - ${classGroup.classTypeName}`"
                      :label="$t('enroll.class')"
                      persistent-placeholder
                      class="required"
                      disabled
                    />
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn
            raised
            color="primary"
            @click.stop="onEnrollmentConfirm"
          >
            <v-icon left>
              fas fa-save
            </v-icon>
            {{ $t('buttons.save') }}
          </v-btn>

          <v-btn
            raised
            color="error"
            @click.stop="onEnrollmentCancel"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('buttons.cancel') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog
      v-model="validationDialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-card>
        <v-card-title>
          <v-toolbar
            color="red"
            outlined
          >
            <v-btn
              icon
              dark
              @click="validationDialog = false"
            >
              <v-icon>mdi-close</v-icon>
            </v-btn>
            <v-spacer />
            <v-toolbar-items>
              <button-tip
                icon
                icon-name="fa-copy"
                tooltip="buttons.copy"
                bottom
                iclass=""
                small
                @click="copyError()"
              />
            </v-toolbar-items>
          </v-toolbar>
        </v-card-title>
        <v-card-text>
          <api-error-details
            v-for="(error, index) in validationErrors"
            :key="index"
            :value="error"
            class="my-1"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            raised
            color="light"
            @click.stop="validationDialog = false"
          >
            <v-icon left>
              mdi-close
            </v-icon>
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import Constants from '@/common/constants.js';
import ButtonGroup from '../wrappers/ButtonGroup.vue';
import Draggable from 'vuedraggable';
import { StudentClass } from '@/models/studentClass/studentClass';
import CustomAutocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import ApiErrorDetails from '@/components/admin/ApiErrorDetails.vue';
import { mapGetters } from 'vuex';
import { InstType, ClassKind } from '@/enums/enums';
import AppLoader from '@/components/wrappers/loader.vue';

export default {
  name: 'ClassEnrollmentView',
  components: {
    ButtonGroup,
    Draggable,
    CustomAutocomplete,
    ApiErrorDetails,
    AppLoader
 },
  props: {
    classId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      classGroup: null,
      classStudents: [],
      studentsForEnrollment: [],
      dateFormat: Constants.DATEPICKER_FORMAT,
      selectedToEnroll: [],
      selectedToUnenroll: [],
      enrollment: false,
      processedStudent: null,
      sudentsToProcess: [],
      processedStudentsCount: 0,
      enrollmentDialog: false,
      enrollmentModel: null,
      validationDialog: false,
      validationErrors: [],
      selectedToEnrollSearch: '',
      classStudentsSearch:  '',
      dateFormatDot: Constants.DATEPICKER_FORMAT_DOTS,
      page: 1,
      pageSize: 30,
      secondaryPage: 1,
      loadingClassStudents: false,
      loadingStudentsForEnrollment: false,
      classGroupsOptions: null,
      classGroupFilter: null,
      expandablePanelModel: null
    };
  },
  computed: {
    ...mapGetters(['isInstType', 'userInstitutionId']),
    isKindergartenInstitution() {
      return this.isInstType(InstType.Kindergarten);
    },
    isCplrInstitution() {
      return this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ);
    },
    enrollmentProgressPercentage() {
      if (!this.processedStudent || !Array.isArray(this.sudentsToProcess)) {
        return 0;
      }

      return (this.processedStudentsCount / this.sudentsToProcess.length) * 100;
    },
    // Pure computed property for filtered and sorted students
    filteredClassStudents() {
      if(!this.classStudents || this.classStudents.length === 0) {
        return [];
      }

      const search = this.classStudentsSearch?.toLowerCase() || '';
      let result;

      if (!search) {
        result = [...this.classStudents].sort((a, b) => (a.id ?? 0) - (b.id ?? 0));
      } else {
        result = this.classStudents
          .filter(x =>
            (x.fullName || '').toLowerCase().includes(search) ||
            (x.pin || '').toLowerCase().includes(search)
          )
          .sort((a, b) => (a.id ?? 0) - (b.id ?? 0));
      }

      return result;
    },
    // Computed property for paginated results
    sortedClassStudents() {
      const startIndex = (this.secondaryPage - 1) * this.pageSize;
      return this.filteredClassStudents.slice(startIndex, startIndex + this.pageSize);
    },
    // Pure computed property for filtered and sorted students
    filteredStudentsForEnrollment() {
      if (!this.studentsForEnrollment || this.studentsForEnrollment.length === 0) {
        return [];
      }

      const search = this.selectedToEnrollSearch?.toLowerCase() || '';
      const classGroupFilterValue = this.classGroupFilter;

      // First filter out students who are already in the class
      let result = this.studentsForEnrollment.filter(student =>
        !this.classStudents.some(classStudent =>
          classStudent.pin === student.pin && classStudent.pinType === student.pinType
        )
      );

      // Filter by search text
      if (search) {
        result = result.filter(x => {
          const searchTerms = [
            x.fullName || '',
            x.pin || '',
            x.mainClassName || '',
            `${x.personAge}г.`,
            this.$moment(x.personBirthDate).format(this.dateFormatDot)
          ];
          return searchTerms.some(term => term.toLowerCase().includes(search));
        });
      }

      // Filter by class group
      if (classGroupFilterValue) {
        result = result.filter(x => x.mainClassId === classGroupFilterValue);
      }

      // Sort the results
      result.sort((a, b) => (b.id ?? 0) - (a.id ?? 0));

      return result;
    },
    // Computed property for paginated results
    sortedStudentsForEnrollment() {
      const startIndex = (this.page - 1) * this.pageSize;
      return this.filteredStudentsForEnrollment.slice(startIndex, startIndex + this.pageSize);
    },
    // Total count for pagination controls
    totalStudentsForEnrollment() {
      return this.filteredStudentsForEnrollment.length;
    },
    // Check if current page is valid
    isValidPage() {
      return this.totalStudentsForEnrollment === 0 || this.page <= Math.ceil(this.totalStudentsForEnrollment / this.pageSize);
    },
    // Total count for pagination controls
    totalClassStudents() {
      return this.filteredClassStudents.length;
    },
    // Check if current page is valid
    isValidSecondaryPage() {
      return this.totalClassStudents === 0 || this.secondaryPage <= Math.ceil(this.totalClassStudents / this.pageSize);
    },
    isMainClassEnrollment() {
      if(!this.classGroup) return false;

      return !this.isCplrInstitution && (this.classGroup.isNotPresentForm === true || this.classGroup.classKindId === ClassKind.Basic);
    },
    isAdditionalEnrollment() {
      if(!this.classGroup) return false;

      return !this.isCplrInstitution && [ClassKind.Cdo, ClassKind.Other].includes(this.classGroup.classKindId);
    },
    isCplrEnrollment() {
      if(!this.classGroup) return false;

      return this.isCplrInstitution && this.classGroup.classKindId === ClassKind.Other;
    },
    // Select all checkbox state for "To Enroll" list
    selectAllToEnroll: {
      get() {
        const selectableStudents = this.sortedStudentsForEnrollment.filter(s =>
          !s.id && !this.classStudents.some(x => x.pin === s.pin && x.pinType === s.pinType)
        );
        if (selectableStudents.length === 0) return false;
        return selectableStudents.every(student =>
          this.selectedToEnroll.some(selected =>
            selected.pin === student.pin && selected.pinType === student.pinType
          )
        );
      },
      set() {
        // This will be handled by the toggleSelectAllToEnroll method
      }
    },
    // Indeterminate state for "To Enroll" checkbox
    isIndeterminateToEnroll() {
      const selectableStudents = this.sortedStudentsForEnrollment.filter(s =>
        !s.id && !this.classStudents.some(x => x.pin === s.pin && x.pinType === s.pinType)
      );
      if (selectableStudents.length === 0) return false;

      const selectedCount = selectableStudents.filter(student =>
        this.selectedToEnroll.some(selected =>
          selected.pin === student.pin && selected.pinType === student.pinType
        )
      ).length;

      return selectedCount > 0 && selectedCount < selectableStudents.length;
    },
    // Select all checkbox state for "To Unenroll" list
    selectAllToUnenroll: {
      get() {
        const selectableStudents = this.sortedClassStudents.filter(s => !s.id);
        if (selectableStudents.length === 0) return false;
        return selectableStudents.every(student =>
          this.selectedToUnenroll.some(selected =>
            selected.pin === student.pin && selected.pinType === student.pinType
          )
        );
      },
      set() {
        // This will be handled by the toggleSelectAllToUnenroll method
      }
    },
    // Indeterminate state for "To Unenroll" checkbox
    isIndeterminateToUnenroll() {
      const selectableStudents = this.sortedClassStudents.filter(s => !s.id);
      if (selectableStudents.length === 0) return false;

      const selectedCount = selectableStudents.filter(student =>
        this.selectedToUnenroll.some(selected =>
          selected.pin === student.pin && selected.pinType === student.pinType
        )
      ).length;

      return selectedCount > 0 && selectedCount < selectableStudents.length;
    },
  },
  watch: {
    // Reset page when search changes or when page becomes invalid
    selectedToEnrollSearch() {
      this.page = 1;
    },
    classGroupFilter() {
      this.page = 1;
    },
    isValidPage(isValid) {
      if (!isValid && this.page > 1) {
        this.page = Math.max(1, Math.ceil(this.totalStudentsForEnrollment / this.pageSize));
      }
    },
    classStudentsSearch() {
      this.secondaryPage = 1;
    },
    isSecondaryPageValid(isValid) {
      if (!isValid && this.secondaryPage > 1) {
        this.secondaryPage = Math.max(1, Math.ceil(this.totalClassStudents.length / this.pageSize));
      }
    }
  },
  async mounted() {
    this.loadClassStudents();
    this.loadStudentsForEnrollment();
    this.loadClassGroupDetails();
    await this.loadClassGroups();
  },
  methods: {
    loadClassGroupDetails() {
      this.$api.classGroup.getById(this.classId).then((response) => {
        this.classGroup = response.data;

        // this.filterEduForms();
     });
    },
    loadClassStudents() {
      this.loadingClassStudents = true;

      this.$api.classGroup.getStudents(this.classId)
        .then(response => {
          if (response.data) {
            this.classStudents = response.data;
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('errors.load'));
          console.log(error.response);
        })
        .finally(() => {
          this.loadingClassStudents = false;
        });
    },
    loadStudentsForEnrollment() {
      this.loadingStudentsForEnrollment = true;

      this.$api.classGroup.getStudentsForMassEnrollment(this.classId)
        .then(response => {
          if (response.data) {
            this.studentsForEnrollment = response.data;
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('errors.load'));
          console.log(error.response);
        })
        .finally(() => {
          this.loadingStudentsForEnrollment = false;
        });
    },
    async loadClassGroups() {
      const currentSchoolYear = Number((await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data);
      if(currentSchoolYear) {
        this.classGroupsOptions = (await this.$api.lookups.getClassGroupsOptions(this.userInstitutionId, currentSchoolYear))?.data;
      }

    },
    onEnroll() {
      if(this.isMainClassEnrollment) {
        this.enrollmentModel = new StudentClass({
          classGroup: this.classGroup,
          classProfessionId: this.classGroup?.classProfessionId,
          studentProfessionId: this.classGroup?.classProfessionId,
          studentSpecialityId: this.classGroup?.classSpecialityId,
          studentEduFormId: this.classGroup?.classEduFormId,
          selectedClassTypeId: this.classGroup?.classTypeId,
          basicClassId: this.classGroup?.basicClassId,
          schoolYear: this.classGroup?.schoolYear,
          classId: this.classId,
          commuterTypeId: 1,
          repeaterId: 1
        });

        this.enrollmentModel.classGroup.classProfessionId = this.classGroup?.classProfessionId;
        this.enrollmentModel.classGroup.basicClassId = this.classGroup?.basicClassId;
      } else if (this.isAdditionalEnrollment) {
        this.enrollmentModel = new StudentClass({
          classGroup: this.classGroup,
          basicClassId: this.classGroup?.basicClassId,
          schoolYear: this.classGroup?.schoolYear,
          classId: this.classId,
        });

      } else if (this.isCplrEnrollment) {
        this.enrollmentModel = new StudentClass({
          classGroup: this.classGroup,
          studentProfessionId: this.classGroup?.classProfessionId,
          studentSpecialityId: this.classGroup?.classSpecialityId,
          studentEduFormId: this.classGroup?.classEduFormId,
          selectedClassTypeId: this.classGroup?.classTypeId,
          basicClassId: this.classGroup?.basicClassId,
          schoolYear: this.classGroup?.schoolYear,
          classId: this.classId,
        });
      }
      else {
         return this.$notifier.error('Неразпознат тип паралелка');
      }

      this.enrollmentDialog = true;
    },
    async onEnrollmentConfirm() {
      const isValid = this.$refs.enrollmentForm.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      } else {
        this.enrollmentDialog = false;
        this.enrollment = true;
        this.processedStudentsCount = 0;
        this.processedStudent = null;
        this.$helper.clearArray(this.validationErrors);
        this.sudentsToProcess = this.classStudents.filter(x => !x.id);

        try {
          if(this.isMainClassEnrollment) {
            await this.mainEnrollment();
          } else if (this.isAdditionalEnrollment) {
            await this.additionalEnrollment();
          } else if (this.isCplrEnrollment) {
            await this.cplrEnrollment();
          } else {
            return this.$notifier.error('Неразпознат тип паралелка');
          }

          this.loadClassStudents();
          this.loadStudentsForEnrollment();

          // Clear filters after successful enrollment
          this.classGroupFilter = null;
          this.selectedToEnrollSearch = '';
          this.selectedToEnroll = [];
          this.selectAllToUnenroll = [];

        } finally {
          this.enrollment = false;
          this.processedStudentsCount = 0;
          this.processedStudent = null;
          this.enrollmentModel = null;
          this.enrollmentDialog = false;
        }
      }
    },
    onEnrollmentCancel() {
      this.enrollmentModel = null;
      this.enrollmentDialog = false;
    },
    async mainEnrollment() {
      for (const student of this.sudentsToProcess) {
          this.processedStudentsCount += 1;
          this.processedStudent = student;

          try {
              const payload = {
                personId: student.personId,
                schoolYear: this.enrollmentModel.schoolYear,
                hasIndividualStudyPlan: this.enrollmentModel.hasIndividualStudyPlan,
                isHourlyOrganization: this.enrollmentModel.isHourlyOrganization,
                IsFTACOutsourced : false,
                commuterTypeId: this.enrollmentModel.commuterTypeId,
                repeaterId: this.enrollmentModel.repeaterId,
                classId: this.enrollmentModel.classId,
                hasSupportiveEnvironment: false,
                supportiveEnvironment: null,
                admissionDocumentId: student.admissionDocumentId,
                isNotForSubmsisionToNra: this.enrollmentModel.isNotForSubmissionToNra,
                studentSpecialityId: this.enrollmentModel.studentSpecialityId,
                selectedClassTypeId: this.enrollmentModel.selectedClassTypeId,
                studentProfessionId: this.enrollmentModel.studentProfessionId,
                studentEduFormId: this.enrollmentModel.studentEduFormId,
                basicClassId: this.enrollmentModel.basicClassId,
                enrollmentDate: this.$helper.parseDateToIso(this.enrollmentModel.enrollmentDate, ''),
                entryDate: this.$helper.parseDateToIso(this.enrollmentModel.entryDate, ''),
                initialEnrollmentPosition: student.positionId,
              };

              await this.$api.studentClass.enrollInClass(payload);
              this.$studentEventBus.$emit('studentMovementUpdate', student.personId);
          } catch (error) {
              const {message, errors} = this.$helper.parseError(error.response);
              this.validationErrors.push({ date: new Date(), message: message,  detail: `${student?.fullName} / ${student?.pin} - ${student?.pinType}`, data: errors });
          }
      }
    },
    async additionalEnrollment() {
      for (const student of this.sudentsToProcess) {
          this.processedStudentsCount += 1;
          this.processedStudent = student;

          try {
              this.enrollmentModel.personId = student.personId;
              this.enrollmentModel.enrollmentDate = this.$helper.parseDateToIso(this.enrollmentModel.enrollmentDate, '');

              await this.$api.studentClass.enrollInAdditionalClass(this.enrollmentModel);
              this.$studentEventBus.$emit('studentMovementUpdate', student.personId);
          } catch (error) {
             const {message, errors} = this.$helper.parseError(error.response);
             this.validationErrors.push({ date: new Date(), message: message,  detail: `${student?.fullName} / ${student?.pin} - ${student?.pinType}`, data: errors });
          }
      }
    },
    async cplrEnrollment() {
      for (const student of this.sudentsToProcess) {
          this.processedStudentsCount += 1;
          this.processedStudent = student;

          try {
              const payload = {
                personId: student.personId,
                schoolYear: this.enrollmentModel.schoolYear,
                classId: this.enrollmentModel.classId,
                basicClassId: this.enrollmentModel.basicClassId,
                studentSpecialityId: this.enrollmentModel.studentSpecialityId,
                selectedClassTypeId: this.enrollmentModel.selectedClassTypeId,
                studentProfessionId: this.enrollmentModel.studentProfessionId,
                studentEduFormId: this.enrollmentModel.studentEduFormId,
                admissionDocumentId: student.admissionDocumentId,
                enrollmentDate: this.$helper.parseDateToIso(this.enrollmentModel.enrollmentDate, ''),
              };

              if (payload.basicClassId == null) {
                payload.basicClassId = 0; // Стойност по подразбиране инак не се байндва модела в контролера
              }

              await this.$api.studentClass.enrollInCplrClass(payload);
              this.$studentEventBus.$emit('studentMovementUpdate', student.personId);
          } catch (error) {
              const {message, errors} = this.$helper.parseError(error.response);
              this.validationErrors.push({ date: new Date(), message: message,  detail: `${student?.fullName} / ${student?.pin} - ${student?.pinType}`, data: errors });
          }
      }
    },
    copyError() {
      let vm = this;
      navigator.clipboard.writeText(JSON.stringify(this.validationErrors)).then(
        function () {
          vm.$notifier.success("", vm.$t("common.clipboardSuccess"));
        },
        function () {
          vm.$notifier.error("", vm.$t("common.clipboardError"));
        }
      );
    },
    toggleSelectAllToEnroll() {
      const selectableStudents = this.sortedStudentsForEnrollment.filter(s =>
        !s.id && !this.classStudents.some(x => x.pin === s.pin && x.pinType === s.pinType)
      );

      // Check if all are currently selected
      const allSelected = selectableStudents.every(student =>
        this.selectedToEnroll.some(selected =>
          selected.pin === student.pin && selected.pinType === student.pinType
        )
      );

      if (allSelected) {
        // Deselect all visible students
        selectableStudents.forEach(student => {
          const index = this.selectedToEnroll.findIndex(selected =>
            selected.pin === student.pin && selected.pinType === student.pinType
          );
          if (index > -1) {
            this.selectedToEnroll.splice(index, 1);
          }
        });
      } else {
        // Select all visible students that aren't already selected
        selectableStudents.forEach(student => {
          const isAlreadySelected = this.selectedToEnroll.some(selected =>
            selected.pin === student.pin && selected.pinType === student.pinType
          );
          if (!isAlreadySelected) {
            this.selectedToEnroll.push(student);
          }
        });
      }

      // Clear the other selection
      this.selectedToUnenroll = [];
    },
    toggleSelectAllToUnenroll() {
      const selectableStudents = this.sortedClassStudents.filter(s => !s.id);

      // Check if all are currently selected
      const allSelected = selectableStudents.every(student =>
        this.selectedToUnenroll.some(selected =>
          selected.pin === student.pin && selected.pinType === student.pinType
        )
      );

      if (allSelected) {
        // Deselect all visible students
        selectableStudents.forEach(student => {
          const index = this.selectedToUnenroll.findIndex(selected =>
            selected.pin === student.pin && selected.pinType === student.pinType
          );
          if (index > -1) {
            this.selectedToUnenroll.splice(index, 1);
          }
        });
      } else {
        // Select all visible students that aren't already selected
        selectableStudents.forEach(student => {
          const isAlreadySelected = this.selectedToUnenroll.some(selected =>
            selected.pin === student.pin && selected.pinType === student.pinType
          );
          if (!isAlreadySelected) {
            this.selectedToUnenroll.push(student);
          }
        });
      }

      // Clear the other selection
      this.selectedToEnroll = [];
    },
    moveToUnenroll() {
      if(!this.selectedToUnenroll.length) return;

      this.selectedToUnenroll.forEach(student => {
        const index = this.classStudents.findIndex(x => x.pin === student.pin && x.pinType === student.pinType);
        this.studentsForEnrollment.push({...student});
        this.classStudents.splice(index,1);

      });

      this.selectedToUnenroll = [];
    },
    moveToEnroll() {
      if(!this.selectedToEnroll.length) return;

      this.selectedToEnroll.forEach(student => {
        const index = this.studentsForEnrollment.findIndex(x => x.pin === student.pin && x.pinType === student.pinType);
        this.classStudents.push({...student});
        this.studentsForEnrollment.splice(index,1);
      });

      this.selectedToEnroll = [];
    }
  },

};
</script>

<style scoped>
.fixed-to-top {
  position: sticky;
  position: -webkit-sticky; /* for Safari */
  top: 6em;
  z-index: 2;
}

.fixed-to-bottom {
  position: sticky;
  position: -webkit-sticky; /* for Safari */
  bottom: 5em;
  z-index: 2;
}

.draggable-area {
  min-height: inherit;
}

.empty-drop-zone {
  border: 2px dashed #ccc;
  border-radius: 8px;
  margin: 8px;
  background-color: #fafafa;
  transition: all 0.3s ease;
}

.empty-drop-zone:hover {
  border-color: #1976d2;
  background-color: #e3f2fd;
}

.no-ellipsis {
  white-space: normal !important;
  overflow: visible !important;
  text-overflow: unset !important;
}

.sortable-ghost {
  opacity: 0.5;
}

.sortable-chosen {
  opacity: 0.8;
}

.sortable-drag {
  opacity: 0.7;
}
</style>

