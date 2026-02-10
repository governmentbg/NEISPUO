<template>
  <v-card>
    <v-card-title>
      <div v-if="isEditFormMode">
        {{ $t('environmentCharacteristics.editRelativeTitle') }}
      </div>
      <div v-else>
        {{ $t('environmentCharacteristics.addRelativeTitle') }}
      </div>
      <v-spacer v-if="isEditFormMode" />
      <edit-button
        v-if="isEditFormMode"
        v-model="isEditMode"
      />
    </v-card-title>
    <v-card-text>
      <v-card-text>
        <v-row
          dense
          justify="center"
        >
          <v-col
            cols="12"
            md="6"
          >
            <PersonUniqueId
              ref="personUniqueId"
              :personal-i-d.sync="form.pin"
              :personal-i-d-type.sync="form.pinType.value"
              :initial-personal-i-d="form.pin"
              :initial-personal-type="form.pinType.name"
              :disabled="isEditFormMode"
              :unique-check="false"
              :pin-required="false"
              @updatePersonDataFromRegix="updatePersonDataFromRegix"
              @regixQueryStarted="regixQueryStarted"
              @regixQueryCompleted="regixQueryCompleted"
            />
          </v-col>
        </v-row>
        <v-row dense>
          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <v-text-field
              v-model="form.firstName"
              :label="$t('studentTabs.firstName')"
              :disabled="!isEditMode"
              :value="form.firstName"
            />
          </v-col>

          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <v-text-field
              v-model="form.middleName"
              :label="$t('studentTabs.middleName')"
              :disabled="!isEditMode"
              :value="form.middleName"
            />
          </v-col>

          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <v-text-field
              v-model="form.lastName"
              :label="$t('studentTabs.lastName')"
              :disabled="!isEditMode"
              :value="form.lastName"
            />
          </v-col>
          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <autocomplete
              v-model="form.relativeType"
              api="/api/lookups/GetStudentRelativeTypeOptions"
              :label="$t('environmentCharacteristics.studentRelationshipLabel')"
              :return-object="true"
              :disabled="saving || !isEditMode"
              :defer-options-loading="false"
              class="required"
              :rules="[$validator.required()]"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <email-field
              v-model="form.email"
              :disabled="!isEditMode"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <phone-field
              v-model="form.phoneNumber"
              :disabled="!isEditMode"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <v-text-field
              v-model="form.address"
              :label="$t('studentTabs.address')"
              :disabled="!isEditMode"
              :value="form.address"
            />
          </v-col>
          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <autocomplete
              v-model="form.workStatus"
              api="/api/lookups/GetStudentRelativeWorkStatusOptions"
              :label="$t('environmentCharacteristics.employment')"
              :return-object="true"
              :disabled="saving || !isEditMode"
              :defer-options-loading="false"
              class="required"
              :rules="[$validator.required()]"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            md="4"
            sm="6"
          >
            <autocomplete
              v-model="form.educationType"
              api="/api/lookups/GetEducationTypeOptions"
              :label="$t('environmentCharacteristics.education')"
              :return-object="true"
              :disabled="saving || !isEditMode"
              :defer-options-loading="false"
              class="required"
              :rules="[$validator.required()]"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
          >
            <v-textarea
              v-model="form.notes"
              outlined
              prepend-icon="mdi-comment"
              :label="$t('environmentCharacteristics.notes')"
              :disabled="!isEditMode"
              :value="form.notes"
            />
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-btn
          raised
          color="primary"
          class="mb-3 ml-3"
          :disabled="saving"
          @click.stop="redirectToList"
        >
          {{ $t('environmentCharacteristics.environmentCharacteristicsTitle') }}
        </v-btn>
        <v-spacer />
        <v-btn
          ref="submit"
          raised
          color="primary"
          :disabled="saving || !isEditMode"
          type="submit"
        >
          <v-icon left>
            fas fa-save
          </v-icon>
          {{ $t("buttons.saveChanges") }}
        </v-btn>
        <v-btn
          v-if="!isEditFormMode"
          raised
          color="error"
          :disabled="saving || !isEditMode"
          @click="onReset"
        >
          <v-icon left>
            fas fa-times
          </v-icon>
          {{ $t("buttons.clear") }}
        </v-btn>
      </v-card-actions>
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
import { StudentRelativeModel } from "@/models/environmentCharacteristics/studentRelativeModel.js";
import PersonUniqueId from "@/components/person/PersonUniqueId.vue";
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
    name: 'RelativeForm',
    components: {
     PersonUniqueId,
     Autocomplete
    },
    props: {
        personId: {
            type: Number,
            required: true
        },
        form:{
            type: StudentRelativeModel,
            required: true
        },
        isEditFormMode:{
            type: Boolean,
            required: true
        },
        saving:{
            type: Boolean,
            required: true
        }
    },
    data()
    {
        return {
            isEditMode: false
        };
    },
    mounted() {
        if(!this.isEditFormMode){
            this.isEditMode = true;
        }
    },
    methods: {
        redirectToList() {
            this.$router.push(`/student/${this.personId}/environmentCharacteristics`);
        },
        async onReset() {
            if(await this.$refs.confirm.open(this.$t('buttons.clear'), this.$t('lod.awards.resetData'))) {
                this.$emit('ResetForm');
            }
        },
        updatePersonDataFromRegix(personDetails){
            this.$emit('updatePersonDataFromRegix', personDetails);
        },
        regixQueryStarted(){
            this.$emit('regixQueryStarted');
        },
        regixQueryCompleted(){
            this.$emit('regixQueryCompleted');
        }
    }
};
</script>
