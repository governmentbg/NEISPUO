<template>
  <div>
    <v-form
      ref="form"
      @submit.prevent="validate"
    >
      <RelativeForm
        v-if="form != null"
        ref="relativeForm"
        :key="relativeFormComponentKey"
        :person-id="personId"
        :is-edit-form-mode="true"
        :form.sync="form"
        :saving="saving"
        @updatePersonDataFromRegix="updatePersonDataFromRegix"
        @regixQueryStarted="regixQueryStarted"
        @regixQueryCompleted="regixQueryCompleted"
      />
    </v-form>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import RelativeForm from "@/components/tabs/environmentCharacteristics/RelativeForm";
import { StudentRelativeModel } from "@/models/environmentCharacteristics/studentRelativeModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
    name: 'RelativeEdit',
    components: {
       RelativeForm
    },
    props: {
        personId: {
            type: Number,
            required: false,
            default: 0
        },
        relativeId: {
            type: Number,
            required: true
        },
    },
    data()
    {
        return {
                saving: false,
                form: null,
                isEditMode: false,
                relativeFormComponentKey:0
            };
    },
    computed: {
        ...mapGetters(['hasStudentPermission']),
    },
    mounted() {
        if(!this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicManage)) {
            return this.$router.push('/errors/AccessDenied');
        }
       this.loadData();
    },
    methods: {
        loadData() {
            this.saving = true;

            this.$api.environmentCharacteristics.getRelative(this.relativeId, this.personId)
            .then(response => {
                if (response.data) {
                    this.form = new StudentRelativeModel(response.data, this);
                }
            }).catch(error => {
                this.$notifier.error('', this.$t('errors.studentRelativeLoad'));
                console.log(error);
            })
           .finally(() => {
               this.saving = false;
               this.relativeFormComponentKey++;
           });
        },
        async validate() {
            let hasErrors = this.$validator.validate(this);

            if(hasErrors) {
                this.$notifier.error('', this.$t('validation.hasErrors'));
                return;
            }

            if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))) {

                this.saving = true;

                var pinType;
                if(this.form.pinType.value != undefined && this.form.pinType.value.value != undefined ){
                    pinType = this.form.pinType.value;
                }else{
                    pinType = this.form.pinType;
                }

                const payload = {
                    id: this.form.id,
                    relativeType: this.form.relativeType,
                    notes: this.form.notes,
                    personId: this.personId,
                    workStatus:  this.form.workStatus,
                    description:  this.form.description,
                    firstName: this.form.firstName,
                    middleName: this.form.middleName,
                    lastName: this.form.lastName,
                    address: this.form.address,
                    pinType: pinType,
                    pin: this.form.pin,
                    index: this.form.index,
                    email: this.form.email,
                    phoneNumber: this.form.phoneNumber,
                    educationType: this.form.educationType
                };

                this.$api.environmentCharacteristics.updateRelative(payload)
                .then(() => {
                    this.$router.push(`/student/${this.personId}/environmentCharacteristics`);
                })
                .catch((error) => {
                    if(error.response.data.message === 'This relative identificator already exists for this student.'){
                      this.$notifier.error('',this.$t("errors.environmentCharacteristicsRelativeIdAlreadyExists"));
                    }else{
                      this.$notifier.error('',this.$t("errors.environmentCharacteristicsRelativeAdd"));
                    }
                    console.log(error.response.data.message);
                })
                .finally(() => {
                    this.saving = false;
                    this.relativeFormComponentKey++;
                });
            }
        },
        updatePersonDataFromRegix(personDetails){
          if(!personDetails) return;

          this.form.address = personDetails.address;
          this.form.firstName = personDetails.firstName;
          this.form.middleName = personDetails.middleName;
          this.form.lastName = personDetails.lastName;
        },
        regixQueryStarted(){
         this.saving = true;
        },
        regixQueryCompleted(){
         this.saving = false;
        }
    }
};
</script>
