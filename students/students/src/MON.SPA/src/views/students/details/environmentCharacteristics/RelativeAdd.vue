<template>
  <div>
    <v-form
      ref="form"
      @submit.prevent="validate"
    >
      <RelativeForm
        ref="relativeForm"
        :key="componentKey"
        :person-id="personId"
        :is-edit-form-mode="false"
        :form.sync="form"
        :saving="saving"
        @ResetForm="onReset"
        @updatePersonDataFromRegix="updatePersonDataFromRegix"
        @regixQueryStarted="regixQueryStarted"
        @regixQueryCompleted="regixQueryCompleted"
      />
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { NotificationSeverity } from '@/enums/enums';
import RelativeForm from "@/components/tabs/environmentCharacteristics/RelativeForm";
import { StudentRelativeModel } from "@/models/environmentCharacteristics/studentRelativeModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
    name: 'RelativeAdd',
    components: {
       RelativeForm,
    },
    props: {
        personId: {
            type: Number,
            required: true
        },
    },
    data()
    {
        return {
            saving: false,
            form: new StudentRelativeModel(),
            sanctionTypeOptions: [],
            componentKey:0,
            notificationSeverity: NotificationSeverity
        };
    },
    computed: {
        ...mapGetters(['hasStudentPermission']),
    },
   watch: {
    'form.pin': {
      handler() {
        if(this.form && this.$refs.relativeForm.$refs.personUniqueId.validatePin(this.getPinType(), this.form.pin)) {

            this.saving = true;

            this.$api.environmentCharacteristics.getRelatedRelativeByPin(this.form.pin, this.personId).then(response => {

                if(response.status == 200){
                    this.form = new StudentRelativeModel(response.data);

                    this.$notifier.toast(this.$t('environmentCharacteristics.existingRelativeTitle'), this.$t('environmentCharacteristics.existingRelativeLoaded') ,this.notificationSeverity.info);
                }
            })
            .catch((error) => {
                 if(error.response.data.message === 'This child already has relative with this pin.'){
                    this.$notifier.error('',this.$t("errors.environmentCharacteristicsRelativeIdAlreadyExists"));
                 }

                console.log(error.response.data.message);
            })
            .finally(() => { this.saving = false; });
        }
      },
    }
   },
    mounted() {
        if(!this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicManage)) {
             return this.$router.push('/errors/AccessDenied');
        }
    },
    methods: {
        async validate() {
            let hasErrors = this.$validator.validate(this);

            var allsubComponentsValid = false;

            if(this.$refs.personUniqueId != undefined && this.$refs.personUniqueId.length != 0){
                this.$refs.personUniqueId.forEach(function(entry) {
                            entry.$v.$touch();
                        allsubComponentsValid = !entry.$v.$invalid;
                    });
            }else{
                allsubComponentsValid = true;
            }

            if(hasErrors && allsubComponentsValid) {
                this.$notifier.error('', this.$t('validation.hasErrors'));
                return;
            }

            const payload = {
                relativeType: this.form.relativeType,
                notes: this.form.notes,
                personId: this.personId,
                workStatus:  this.form.workStatus,
                description:  this.form.description,
                firstName: this.form.firstName,
                middleName: this.form.middleName,
                lastName: this.form.lastName,
                address: this.form.address,
                pinType:  this.getPinType(),
                pin: this.form.pin,
                index: this.form.index,
                email: this.form.email,
                phoneNumber: this.form.phoneNumber,
                educationType: this.form.educationType
            };

            if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))) {
                this.saving = true;

                this.$api.environmentCharacteristics.addRelative(payload).then(() => {
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
                .finally(() => { this.saving = false; });
            }
        },
        async onReset() {
            this.form = new StudentRelativeModel();
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
        },
        getPinType(){
            var pinType;

            if(this.form.pinType.value != undefined && this.form.pinType.value.value != undefined ){
                pinType = this.form.pinType.value;
            }else{
                pinType = this.form.pinType;
            }

            return pinType;
        }
    }
};
</script>
