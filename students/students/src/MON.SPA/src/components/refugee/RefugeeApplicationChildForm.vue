<template>
  <div>
    <v-row
      v-if="!isDetails"
      dense
    >
      <v-col
        md="4"
      >
        <v-text-field
          v-model="model.firstName"
          :label="$t('refugee.headers.firstName')"
          :rules="[$validator.required()]"
          class="required"
          clearable
          :disabled="disabled"
        />
      </v-col>
      <v-col
        md="4"
      >
        <v-text-field
          v-model="model.middleName"
          :label="$t('refugee.headers.middleName')"
          clearable
          :disabled="disabled"
        />
      </v-col>
      <v-col
        md="4"
      >
        <v-text-field
          v-model="model.lastName"
          :label="$t('refugee.headers.lastName')"
          :rules="[$validator.required()]"
          class="required"
          clearable
          :disabled="disabled"
        />
      </v-col>
    </v-row>
    <v-row
      justify="center"
      dense
    >
      <v-col
        cols="12"
        md="8"
      >
        <person-id-details
          v-if="isDetails"
          :personal-id-type="model.personalIdTypeName"
          :personal-id="model.personalId"
          :disabled="disabled"
        />
        <PersonUniqueId
          v-else
          ref="personUniqueId"
          :personal-i-d.sync="model.personalId"
          :personal-i-d-type.sync="model.personalIdTypeModel"
          :initial-personal-i-d="model.personalId"
          :initial-personal-type="model.personalIdType.toString()"
          :pin-required="true"
          :initial-pin-types="[0, 1]"
          :disabled="disabled"
        />
      </v-col>
    </v-row>
    <v-row
      dense
    >
      <v-col
        cols="12"
        sm="6"
        md="6"
        lg="4"
      >
        <v-container
          class="px-0"
          fluid
        >
          <v-radio-group
            v-model="model.hasDualCitizenship"
            mandatory
            row
            :disabled="disabled"
          >
            <template v-slot:label>
              <div>{{ $t("refugee.headers.hasDualCitizenship") }}</div>
            </template>
            <v-radio
              :label="$t('common.no')"
              :value="false"
            />
            <v-radio
              :label="$t('common.yes')"
              :value="true"
            />
          </v-radio-group>
        </v-container>
      </v-col>
    </v-row>
    <v-row
      dense
    >
      <v-col
        cols="12"
        sm="6"
        md="3"
        lg="1"
      >
        <v-text-field
          v-if="isDetails"
          v-model="model.genderName"
          :label="$t('createStudent.gender')"
          :disabled="disabled"
        />
        <v-select
          v-else
          id="gender"
          ref="gender"
          v-model="model.gender"
          :label="$t('createStudent.gender')"
          :items="genderOptions"
          clearable
          :rules="[$validator.required()]"
          class="required"
          :disabled="disabled"
        />
      </v-col>
      <v-col
        cols="12"
        sm="6"
        md="3"
        lg="2"
      >
        <date-picker
          id="birthDate"
          ref="birthDate"
          v-model="model.birthDate"
          :show-buttons="false"
          :scrollable="false"
          :no-title="true"
          :show-debug-data="false"
          :label="$t('refugee.headers.birthDate')"
          :rules="[$validator.required()]"
          :class="!isDetails ? 'required' : ''"
          :disabled="disabled"
        />
      </v-col>
      <v-col
        cols="12"
        sm="6"
        md="4"
        lg="3"
      >
        <v-text-field
          v-if="isDetails"
          v-model="model.nationality"
          :label="$t('createStudent.nationality')"
          :disabled="disabled"
        />
        <autocomplete
          v-else
          v-model="model.nationalityId"
          api="/api/lookups/GetCountriesBySearchString"
          :label="$t('createStudent.nationality')"
          :placeholder="$t('buttons.search')"
          clearable
          hide-no-data
          hide-selected
          :rules="[$validator.required()]"
          class="required"
          :disabled="disabled"
        />
      </v-col>

      <v-col
        cols="12"
        sm="6"
        md="4"
        lg="3"
      >
        <v-text-field
          v-model="model.email"
          prepend-inner-icon="mdi-email"
          :label="$t('refugee.headers.email')"
          clearable
          :disabled="disabled"
        />
      </v-col>
      <v-col
        cols="12"
        sm="6"
        md="4"
        lg="3"
      >
        <v-text-field
          v-model="model.phone"
          prepend-inner-icon="mdi-phone"
          :label="$t('refugee.headers.phone')"
          clearable
          :disabled="disabled"
        />
      </v-col>
    </v-row>

    <v-row dense>
      <v-col
        cols="12"
        sm="12"
        md="12"
        lg="12"
      >
        <v-container
          class="px-0"
          fluid
        >
          <v-radio-group
            v-model="model.protectionStatus"
            mandatory
            row
            :disabled="disabled"
          >
            <v-radio
              :label="$t('common.internationalProtection')"
              :value="1"
            />
            <v-radio
              :label="$t('common.temporaryProtection')"
              :value="2"
            />
            <v-radio
              :label="$t('common.noProtection')"
              :value="3"
            />
          </v-radio-group>
        </v-container>
      </v-col>
    </v-row>
    <v-row dense>
      <v-col
        cols="12"
        sm="6"
      >
        <v-text-field
          v-if="isDetails"
          v-model="model.town"
          :label="$t('refugee.headers.town')"
          :disabled="disabled"
        />
        <autocomplete
          v-else
          id="town"
          ref="town"
          v-model="model.townId"
          api="/api/lookups/GetAddressesBySearchString"
          :label="$t('refugee.headers.town')"
          :placeholder="$t('buttons.search')"
          clearable
          hide-no-data
          hide-selected
          :rules="[$validator.required()]"
          class="required"
          :disabled="disabled"
        />
      </v-col>
      <v-col
        cols="12"
        sm="6"
      >
        <v-text-field
          v-model="model.address"
          :label="$t('refugee.headers.address')"
          clearable
          :disabled="disabled"
        />
      </v-col>
    </v-row>

    <v-expansion-panels
      v-model="expandablePanelModel"
      multiple
      popout
      flat
    >
      <v-expansion-panel>
        <v-expansion-panel-header>
          <v-alert
            border="left"
            colored-border
            type="info"
            elevation="2"
          >
            1. Езикови умения
          </v-alert>
        </v-expansion-panel-header>
        <v-expansion-panel-content>
          <v-row dense>
            <v-col
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-if="isDetails"
                :value="
                  model.bgLanguageSkill !== null
                    ? languageSkillOptions.find(
                      (x) => x.value === model.bgLanguageSkill
                    ).name
                    : ''
                "
                :label="$t('refugee.bgLanguageSkill')"
                :disabled="disabled"
              />
              <v-select
                v-else
                id="bgLanguageSkill"
                ref="bgLanguageSkill"
                v-model="model.bgLanguageSkill"
                :label="$t('refugee.bgLanguageSkill')"
                :items="languageSkillOptions"
                item-text="name"
                item-value="value"
                clearable
                :rules="[$validator.required()]"
                class="required"
                :disabled="disabled"
              />
            </v-col>
            <v-col
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-if="isDetails"
                :value="
                  model.enLanguageSkill !== null
                    ? languageSkillOptions.find(
                      (x) => x.value === model.enLanguageSkill
                    ).name
                    : ''
                "
                :label="$t('refugee.enLanguageSkill')"
                :disabled="disabled"
              />
              <v-select
                v-else
                id="enLanguageSkill"
                ref="enLanguageSkill"
                v-model="model.enLanguageSkill"
                :label="$t('refugee.enLanguageSkill')"
                :items="languageSkillOptions"
                :rules="[$validator.required()]"
                class="required"
                item-text="name"
                item-value="value"
                clearable
                :disabled="disabled"
              />
            </v-col>
            <v-col
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-if="isDetails"
                :value="
                  model.deLanguageSkill !== null
                    ? languageSkillOptions.find(
                      (x) => x.value === model.deLanguageSkill
                    ).name
                    : ''
                "
                :label="$t('refugee.deLanguageSkill')"
                :disabled="disabled"
              />
              <v-select
                v-else
                id="deLanguageSkill"
                ref="deLanguageSkill"
                v-model="model.deLanguageSkill"
                :label="$t('refugee.deLanguageSkill')"
                :items="languageSkillOptions"
                :rules="[$validator.required()]"
                class="required"
                item-text="name"
                item-value="value"
                clearable
                :disabled="disabled"
              />
            </v-col>
            <v-col
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-if="isDetails"
                :value="
                  model.frLanguageSkill !== null
                    ? languageSkillOptions.find(
                      (x) => x.value === model.frLanguageSkill
                    ).name
                    : ''
                "
                :label="$t('refugee.frLanguageSkill')"
                :disabled="disabled"
              />
              <v-select
                v-else
                id="frLanguageSkill"
                ref="frLanguageSkill"
                v-model="model.frLanguageSkill"
                :label="$t('refugee.frLanguageSkill')"
                :items="languageSkillOptions"
                :rules="[$validator.required()]"
                class="required"
                item-text="name"
                item-value="value"
                clearable
                :disabled="disabled"
              />
            </v-col>
            <v-col
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-model="model.otherLanguage"
                :label="$t('refugee.otherLanguageName')"
                clearable
                :disabled="disabled"
              />
            </v-col>

            <v-col
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-if="isDetails"
                :value="
                  model.otherLanguageSkill !== null
                    ? languageSkillOptions.find(
                      (x) => x.value === model.otherLanguageSkill
                    ).name
                    : ''
                "
                :label="$t('refugee.otherLanguageSkill')"
                :disabled="disabled"
              />
              <v-select
                v-else
                id="otherLanguageSkill"
                ref="otherLanguageSkill"
                v-model="model.otherLanguageSkill"
                :label="$t('refugee.otherLanguageSkill')"
                :items="languageSkillOptions"
                item-text="name"
                item-value="value"
                clearable
                :disabled="disabled"
              />
            </v-col>
          </v-row>
        </v-expansion-panel-content>
      </v-expansion-panel>
      <v-expansion-panel>
        <v-expansion-panel-header>
          <v-alert
            border="left"
            colored-border
            type="info"
            elevation="2"
          >
            2. Последно посещавана група от детска градина или клас в училище:
          </v-alert>
        </v-expansion-panel-header>
        <v-expansion-panel-content>
          <v-row dense>
            <v-col
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-if="isDetails"
                :value="model.lastInstitutionCountryName"
                :label="$t('refugee.lastInstitutionCountry')"
                :disabled="disabled"
              />
              <autocomplete
                v-else
                v-model="model.lastInstitutionCountry"
                api="/api/lookups/GetCountriesBySearchString"
                :label="$t('refugee.lastInstitutionCountry')"
                :placeholder="$t('buttons.search')"
                :rules="[$validator.required()]"
                class="required"
                clearable
                hide-no-data
                hide-selected
                :disabled="disabled"
              />
            </v-col>

            <v-col
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-if="isDetails"
                :value="
                  model.lastInstitutionType !== null
                    ? institutionTypeOptions.find(
                      (x) => x.value === model.lastInstitutionType
                    ).name
                    : ''
                "
                :label="$t('refugee.lastInstitutionType')"
                :disabled="disabled"
              />
              <v-select
                v-else
                id="lastInstitutionType"
                ref="lastInstitutionType"
                v-model="model.lastInstitutionType"
                :label="$t('refugee.lastInstitutionType')"
                :items="institutionTypeOptions"
                item-text="name"
                item-value="value"
                clearable
                :disabled="disabled"
              />
            </v-col>
            <v-col
              v-if="model.lastInstitutionType && model.lastInstitutionType != 1"
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <c-info uid="recognition.basicClass">
                <v-text-field
                  v-if="isDetails"
                  :value="model.lastBasicClassName"
                  :label="$t('refugee.headers.basicClass')"
                  :disabled="disabled"
                />
                <v-autocomplete
                  v-else
                  id="basicClass"
                  ref="basicClass"
                  v-model="model.lastBasicClassId"
                  :items="basicClassOptions"
                  :label="$t('refugee.headers.basicClass')"
                  :placeholder="$t('buttons.search')"
                  clearable
                  hide-no-data
                  hide-selected
                  :disabled="disabled"
                  :rules="
                    model.lastInstitutionType != 1
                      ? [$validator.required()]
                      : []
                  "
                  :class="model.lastInstitutionType != 1 ? 'required' : ''"
                />
              </c-info>
            </v-col>
            <v-col
              v-if="model.lastInstitutionType && model.lastInstitutionType != 1"
              cols="12"
              sm="6"
              md="4"
              lg="3"
            >
              <v-text-field
                v-model="model.profession"
                :label="$t('refugee.headers.profession')"
                clearable
                :disabled="disabled"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col
              v-if="model.lastInstitutionType && model.lastInstitutionType != 1"
              cols="12"
              sm="12"
              md="6"
              lg="4"
            >
              <v-container
                class="px-0"
                fluid
              >
                <v-radio-group
                  v-model="model.isClassCompleted"
                  mandatory
                  row
                  :disabled="disabled"
                >
                  <template v-slot:label>
                    <div>{{ $t("refugee.headers.isClassCompleted") }}</div>
                  </template>
                  <v-radio
                    :label="$t('common.no')"
                    :value="false"
                  />
                  <v-radio
                    :label="$t('common.yes')"
                    :value="true"
                  />
                </v-radio-group>
              </v-container>
            </v-col>
            <v-col
              v-if="model.lastInstitutionType && model.lastInstitutionType != 1"
              cols="12"
              sm="6"
              md="6"
              lg="4"
            >
              <v-container
                class="px-0"
                fluid
              >
                <v-radio-group
                  v-model="model.hasDocumentForCompletedClass"
                  mandatory
                  row
                  :disabled="disabled"
                >
                  <template v-slot:label>
                    <div>
                      {{ $t("refugee.headers.hasDocumentForCompletedClass") }}
                    </div>
                  </template>
                  <v-radio
                    :label="$t('common.no')"
                    :value="false"
                  />
                  <v-radio
                    :label="$t('common.yes')"
                    :value="true"
                  />
                </v-radio-group>
              </v-container>
            </v-col>
            <v-col
              cols="12"
              sm="12"
              md="6"
              lg="4"
            >
              <v-container
                class="px-0"
                fluid
              >
                <v-radio-group
                  v-model="model.hasNeedForTextbooks"
                  mandatory
                  row
                  :disabled="disabled"
                >
                  <template v-slot:label>
                    <div>{{ $t("refugee.headers.hasNeedForTextbooks") }}</div>
                  </template>

                  <v-radio
                    :label="$t('common.no')"
                    :value="false"
                  />
                  <v-radio
                    :label="$t('common.yes')"
                    :value="true"
                  />
                </v-radio-group>
              </v-container>
            </v-col>
            <v-col
              cols="12"
              sm="12"
              md="6"
              lg="4"
            >
              <v-container
                class="px-0"
                fluid
              >
                <v-radio-group
                  v-model="model.hasNeedForResourceSupport"
                  mandatory
                  row
                  :disabled="disabled"
                >
                  <template v-slot:label>
                    <div>
                      {{ $t("refugee.headers.hasNeedForResourceSupport") }}
                    </div>
                  </template>
                  <v-radio
                    :label="$t('common.no')"
                    :value="false"
                  />
                  <v-radio
                    :label="$t('common.yes')"
                    :value="true"
                  />
                </v-radio-group>
              </v-container>
            </v-col>
          </v-row>
        </v-expansion-panel-content>
      </v-expansion-panel>
      <v-expansion-panel>
        <v-expansion-panel-header>
          <v-alert
            border="left"
            colored-border
            type="info"
            elevation="2"
          >
            3. Попълва се от РУО
          </v-alert>
        </v-expansion-panel-header>
        <v-expansion-panel-content>
          <v-row dense>
            <v-col
              cols="12"
              sm="6"
              md="3"
            >
              <v-text-field
                v-model="model.ruoDocNumber"
                :label="$t('refugee.ruoDocNumber')"
                :disabled="disabled"
              />
            </v-col>
            <v-col
              cols="12"
              sm="6"
              md="3"
            >
              <date-picker
                id="ruoDocDate"
                ref="ruoDocDate"
                v-model="model.ruoDocDate"
                :show-buttons="false"
                :scrollable="false"
                :no-title="true"
                :show-debug-data="false"
                :label="$t('refugee.ruoDocDate')"
                :rules="model.ruoDocNumber ? [$validator.required()] : []"
                :class="model.ruoDocNumber ? 'required' : ''"
                :disabled="disabled"
              />
            </v-col>
            <v-col
              cols="12"
              sm="6"
            >
              <c-info uid="admissionDocument.institutionOptions">
                <v-text-field
                  v-if="isDetails"
                  :value="model.institution"
                  :label="$t('documents.institutionDropdownLabel')"
                  :disabled="disabled"
                />
                <autocomplete
                  v-else
                  id="institutionDropdown"
                  ref="institutionDropdown"
                  v-model="model.institutionId"
                  api="/api/lookups/GetInstitutionOptions"
                  :filter="{ regionId: userRegionId }"
                  :label="$t('documents.institutionDropdownLabel')"
                  :placeholder="$t('buttons.search')"
                  :rules="model.ruoDocNumber ? [$validator.required()] : []"
                  :class="model.ruoDocNumber ? 'required' : ''"
                  hide-no-data
                  hide-selected
                  clearable
                  :disabled="disabled"
                />
              </c-info>
            </v-col>
          </v-row>
        </v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
  </div>
</template>

<script>
import { RefugeeApplicationChildModel } from "@/models/refugee/refugeeApplicationChildModel";
import Autocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import PersonUniqueId from "@/components/person/PersonUniqueId.vue";
import PersonIdDetails from "@/components/person/PersonIdDetails.vue";
import { mapGetters } from "vuex";

export default {
  name: "RefugeeApplicationChildForm",
  components: { Autocomplete, PersonUniqueId, PersonIdDetails },
  props: {
    document: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
    isDetails: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      model:
        this.document ?? new RefugeeApplicationChildModel({}, this.$moment),
      expandablePanelModel: [0, 1, 2],
      genderOptions: [],
      basicClassOptions: [],
      languageSkillOptions: [
        { value: 1, name: "няма" },
        { value: 2, name: "основно" },
        { value: 3, name: "добро" },
      ],
      institutionTypeOptions: [
        { value: 1, name: "няма" },
        { value: 2, name: "детска градина" },
        { value: 3, name: "училище" },
      ],
    };
  },
  computed: {
    ...mapGetters(['userRegionId']),
  },
  watch: {
    "model.lastInstitutionType": function (val) {
      if (val == null && this.model && this.model.lastBasicClassId !== null) {
        this.model.lastBasicClassId = null;
      }
      this.loadBasicClassOptions(val);
    },
  },
  created() {},
  mounted() {
    if (!this.isDetails) {
      this.loadBasicClassOptions(this.model.lastInstitutionType);
      this.loadDropdownOptions();
    }
  },
  methods: {
    loadBasicClassOptions(lastInstitutionType) {
      var minId = -6;
      var maxId = 15;
      switch (lastInstitutionType) {
        case 1: {
          minId = -6;
          maxId = 15;
          break;
        }
        case 2: {
          minId = -6;
          maxId = -1;
          break;
        }
        case 3: {
          minId = 1;
          maxId = 15;
          break;
        }
      }

      this.$api.lookups
        .getBasicClassOptions({ minId: minId, maxId: maxId })
        .then((response) => {
          if (response.data) {
            this.basicClassOptions = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    loadDropdownOptions() {
      this.$api.lookups
        .getGenders()
        .then((response) => {
          const genders = response.data;
          if (genders) {
            this.genderOptions = genders;
          }
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.genderOptionsLoad"));
          console.log(error);
        });
    },
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
  },
};
</script>
