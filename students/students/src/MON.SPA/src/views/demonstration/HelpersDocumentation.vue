<template>
  <div>
    <v-card class="mb-3">
      <v-card-title>
        Notifier plugin
      </v-card-title>
      <v-card-subtitle>
        Във всяка vue инстанция (this) е наличен чрез глобалната променлива $notifier. Има три функции toast, stackbar, modal.
        <ul>
          <li>toast(title, text, severity, options) - показва toast като исползва vue-notification.</li>
          <li>snackbar(title, text, severity) - покзва vuetify stackbar (виж /plugins/notifier/NotificationSnackbar.vue).</li>
          <li>modal(title, text, severity) - покзва vuetify dialog (виж /plugins/notifier/NotificationModal.vue).</li>
          <li>error(title, text) - црентрализирано определяме как да се показват съобщенията за грешки. За сега извиква snackbar(title, text, severity).</li>
          <li>success(title, text)) - црентрализирано определяме как да се показват съобщенията за успех. За сега извиква snackbar(title, text, severity)</li>
          <li>може да се добавят примерно warning, info и всички други, които ни потрябват.</li>
        </ul>
      </v-card-subtitle>

      <v-card-text>
        <div class="ma-2">
          <v-btn
            class="mx-2"
            fab
            dark
            color="light-blue"
            @click="showToast(notificationSeverity.Info)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            fab
            dark
            color="success"
            @click="showToast(notificationSeverity.Success)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            fab
            dark
            color="warning"
            @click="showToast(notificationSeverity.Warn)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            fab
            dark
            color="error"
            @click="showToast(notificationSeverity.Error)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>
        </div>

        <div class="ma-2">
          <v-btn
            class="mx-2"
            outlined
            fab
            color="light-blue"
            @click="showSnackbar(notificationSeverity.Info)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            outlined
            fab
            color="success"
            @click="showSnackbar(notificationSeverity.Success)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            outlined
            fab
            color="warning"
            @click="showSnackbar(notificationSeverity.Warn)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            outlined
            fab
            color="error"
            @click="showSnackbar(notificationSeverity.Error)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>
        </div>

        <div class="ma-2">
          <v-btn
            class="mx-2"
            color="light-blue"
            @click="showModal(notificationSeverity.Info)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            color="success"
            @click="
              showModal( notificationSeverity.Success)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            color="warning"
            @click="
              showModal(notificationSeverity.Warn)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>

          <v-btn
            class="mx-2"
            color="error"
            @click="
              showModal(notificationSeverity.Error)"
          >
            <v-icon dark>
              mdi-plus
            </v-icon>
          </v-btn>
        </div>
      </v-card-text>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        Wrapper на vuetify combobox
      </v-card-title>
      <v-card-subtitle>
        <ul>
          <li>value ([String, Number], default = undefined) - binding property.</li>
          <li>api (String, required) - url на услугата за зареджане на опциите.</li>
          <li>label (String, default = undefined) - Етикет </li>
          <li>clearable (Boolean, default = true) - Покзва бутон(Х) за изчисване на направения избор. </li>
          <li>deferOptionsLoading (Boolean, default = true) - Определя дали опжиите ще се зареждат отложено след въвеждане на минимум определен (от constants.js.SEARCH_INPUT_MIN_LENGTH) брой символи. При зареждането ще ги филтрира по въведената стойност(contains), ако услугата е имплементирана по този начин.</li>
          <li>returnObject (Boolean, default = true) - Връща обект за синхронизиране, а не itemValue-то на избраната опция. </li>
          <li>disabled (Boolean, default = false) - Деактивиране на input-а. </li>
          <li>readonly (Boolean, default = false) - Сетва input-а. в readonly режим. </li>
          <li>persistentHint (Boolean, default = false) - Сетва подсказката да е винаги видима. </li>
          <li>hideSelected (Boolean, default = false) - Скрива избраните елементи от опциите. </li>
          <li>hideNoData (Boolean, default = false) - Hides the menu when there are no options to show. Useful for preventing the menu from opening before results are fetched asynchronously. Also has the effect of opening the menu when the items array changes if not already open. </li>
          <li>itemDisabled ([String, Array, Function], default = 'disabled') - Set property of items’s disabled value. </li>
          <li>itemText ([String, Array, Function], default = 'text') - Определя кое property от опциите ще се изплзва за текс т.е. стойност на input-а.</li>
          <li>itemValue ([String, Array, Function], default = 'value') - Определя кое property от опциите ще се използва за синхронизацията на bind-натото пропърти на компонента.</li>
          <li>placeholder (String, default = undefined) - Placeholder </li>
          <li>hint (String, default = undefined) - Подсказка. Ако е сетнато showDeferredLoadingHint = true, то ще се презапише подсказката с генерирана така, която описва условията за отложено зареждане на опциите. </li>
          <li>removeItemsOnClear (Boolean, default = false) - Изчиства опциите след изчистване на input-a. Подходящо за използване при избрано отложено зареждане т.е. зареждане с търсене. </li>
          <li>showDeferredLoadingHint (Boolean, default = false) - Определя дали да се показва подсказка, която описва условията за отложено зареждане на опциите. </li>
        </ul>
      </v-card-subtitle>
      <v-card-text>
        <div><h4>{{ wrappers.combo }}</h4></div>
        <v-row>
          <v-col
            cols="12"
            md="6"
          >
            <combo
              id="cities"
              v-model="wrappers.combo.city"
              api="/api/lookups/GetCities"
              :defer-options-loading="true"
              :return-object="false"
              :show-deferred-loading-hint="true"
              item-value="text"
              item-text="text"
              label="Комбо с отложено зареждане на опциите"
              :disabled="false"
              :placeholder="$t('buttons.search')"
              :remove-items-on-clear="true"
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
          >
            <combo
              id="district"
              v-model="wrappers.combo.district"
              api="/api/lookups/GetDistricts"
              :return-object="false"
              item-value="text"
              item-text="text"
              label="Комбо без зареждане на опциите"
            />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        Wrapper на vuetify date picker
      </v-card-title>
      <v-card-subtitle>
        <ul>
          <li>value ([Date, String], default = '') - binding property.</li>
          <li>showButtons (Boolean, default = false) - Показва и използва бутоните Cancel и OK. При налияието им селекцията се извършва при натискане на ОК.</li>
          <li>max (String, default = undefined) - Maximum allowed date/month (ISO 8601 format).</li>
          <li>min (String, default = undefined) - Minimum allowed date/month (ISO 8601 format).</li>
          <li>label (String, default = undefined) - Етикет </li>
          <li>showDebugData (Boolean, default = false) - Показва форматираната и ISO дата на кмпонента.</li>
          <li>isCustomDatepicker (Boolean, default = true) - Показва validator pluglin-а, че е CustomDatepicker за да го валидира.</li>
          <li> + всички пропъртита на vuetify datepicker-a.</li>
        </ul>
      </v-card-subtitle>
      <v-card-text>
        <div><h4>{{ wrappers.datePicker }}</h4></div>
        <v-row>
          <v-col
            cols="12"
            md="6"
          >
            <date-picker
              id="date1"
              v-model="wrappers.datePicker.date1"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="true"
              label="Без бутони и datepicker title"
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
          >
            <date-picker
              id="date2"
              v-model="wrappers.datePicker.date2"
              :show-buttons="true"
              :scrollable="true"
              :show-debug-data="true"
              label="С бутони"
            />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        Wrapper на vuetify tooltip and button
      </v-card-title>
      <v-card-subtitle>
        <ul>
          <li>text (String, default = '') - Текст. Ако се подаде валиден ключ от локализационните файлове ще се локализира. Ако не е валиден ще се покажа какъвто е.</li>
          <li>iconName (String, default = '') - Икона.</li>
          <li>color (String, default = 'primary') - Цвят.</li>
          <li>iconColor (String, default = 'white') - Цвят на иконата. </li>
          <li>iclass (String, default = 'mr-2') - Класове на иконата</li>
          <li>tooltip (String, default = '') - Текст на tooltip-а. Ако се подаде валиден ключ от локализационните файлове ще се локализира. Ако не е валиден ще се покажа какъвто е.</li>
          <li>eventname (String, default = 'click') - Име на event, което се връща при click (this.$emit(this.eventname, this.$t(this.text))).</li>

          <li>bottom (Boolean, default = false) - Aligns the component towards the bottom</li>
          <li>top (Boolean, default = false) - Aligns the content towards the top.</li>
          <li>left (Boolean, default = false) - Aligns the component towards the left. This should be used with the absolute or fixed props.</li>
          <li>right (Boolean, default = false) - Aligns the component towards the right. This should be used with the absolute or fixed props.</li>
          <li> + всички пропъртита на vuetify button-a.</li>
        </ul>
      </v-card-subtitle>
      <v-card-text>
        <v-row>
          <v-col
            cols="12"
            md="3"
          >
            <button-tip
              text="Bottom tooltip"
              icon-name="mdi-tooltip"
              tooltip="Bottom tooltip"
              bottom
              depressed
              @click="onButtonTipClick"
            />
          </v-col>

          <v-col
            cols="12"
            md="3"
          >
            <button-tip
              text="Top tooltip"
              icon-name="mdi-tooltip"
              icon-color="primary"
              tooltip="Top tooltip"
              top
              outlined
              color="success"
              @click="onButtonTipClick"
            />
          </v-col>

          <v-col
            cols="12"
            md="3"
          >
            <button-tip
              text="Left tooltip"
              icon-name="mdi-tooltip"
              tooltip="Left tooltip"
              left
              rounded
              color="info"
              @click="onButtonTipClick"
            />
          </v-col>

          <v-col
            cols="12"
            md="3"
          >
            <button-tip
              text="Right tooltip"
              icon-name="mdi-tooltip"
              tooltip="Right tooltip"
              right
              dark
              x-large
              color="error"
              @click="onButtonTipClick"
            />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>Rules validator plugin</v-card-title>
      <v-card-subtitle>
        Във всяка vue инстанция (this) е наличен чрез глобалната променлива $validator. За да валидира компонентите е необходимо те да имат атрибут 'ref'. Има следните функции:
        <ul>
          <li>required() - Валидация за задължително поле.</li>
          <li>minLength(count) - Валидация за минимум {count} символа.</li>
          <li>maxLength(count) - Валидация за максимум {count} символа.</li>
          <li>min(num) - Валидация за минимално число.</li>
          <li>max(num) - Валидация за максимално число.</li>
          <li>email(count) - Валидация за валидна ел.поща.</li>
          <li>numbers() - Валидация за виведени само цифри.</li>
          <li>validate(el) - el е Vue инстацията, която валидираме(this) Валидира всички компоненти, които имате дефиниран 'ref' атрибут и имат validate(). Връща undefined, ако el не е подадено, true, ако има валидационни грещки и false, ако няма валидационни грешки.</li>
          <li>reset(el) - el е Vue инстацията, която валидираме(this). Връща, ако el не е подадено или вика reset() на всички компоненти, които имате дефиниран 'ref' атрибут и имат reset().</li>
        </ul>
        Пример за подаване на рулове: :rules="[$validator.required(), $validator.minLength(5), $validator.maxLength(20)]"
      </v-card-subtitle>
      <v-card-text>
        <v-form ref="validator-example-form">
          <v-row>
            <v-col
              cols="12"
              md="4"
            >
              <v-text-field
                ref="field1"
                v-model="form.field1"
                label="Задължително поле"
                :rules="[$validator.required()]"
              />
            </v-col>

            <v-col
              cols="12"
              md="4"
            >
              <v-text-field
                ref="field2"
                v-model="form.field2"
                label="Задължително поле с мин 5 и макс 20 символи"
                :rules="[$validator.required(), $validator.minLength(5), $validator.maxLength(20)]"
              />
            </v-col>

            <v-col
              cols="12"
              md="4"
            >
              <v-text-field
                ref="field3"
                v-model="form.field3"
                label="Email"
                :rules="[$validator.email()]"
              />
            </v-col>

            <v-col
              cols="12"
              md="4"
            >
              <v-text-field
                ref="field4"
                v-model="form.field4"
                label="Само цифри"
                :rules="[$validator.numbers()]"
              />
            </v-col>

            <v-col
              cols="12"
              md="4"
            >
              <date-picker
                ref="field5"
                v-model="form.field5"
                :validation-ref="'field5'"
                :show-buttons="false"
                :scrollable="false"
                :no-title="true"
                :show-debug-data="true"
                :rules="[$validator.required()]"
                label="Задължителна дата"
              />
            </v-col>

            <v-col
              cols="12"
              md="4"
            >
              <year-picker-combo
                ref="field6"
                v-model="form.field6"
                label="Задължителен избор на година"
                :rules="[$validator.required(), $validator.min(2020), $validator.max(2025)]"
                persistent-hint
                hint="2020-2025"
              />
            </v-col>       
          </v-row>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-btn text>
          Cancel
        </v-btn>
        <v-spacer />
        <v-slide-x-reverse-transition>
          <button-tip 
            v-if="formHasErrors" 
            top
            icon="mdi-refresh" 
            tooltip="Refresh form"
            @click="resetForm"
          />
        </v-slide-x-reverse-transition>
        <v-btn
          ref="btn"
          color="primary"
          text
          @click="submit"
        >
          Submit
        </v-btn>
      </v-card-actions>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        Wrapper на confirm dialog
      </v-card-title>
      <v-card-subtitle>
        <h5>Регистриран е глобален компонент ConfirmDlg. Добавя се в темплейта, където ще се използва. Дефинираме му се ref атрибут, който се използва за викането му т.е.</h5>
        <pre>
         async onSave() {
            if(await this.$refs.confirm.open('Запис', this.$t('common.confirm'))) {
              this.$notifier.success('', this.$t('common.saveSuccess'));
            }
          },
          async onCancel() {
            if(await this.$refs.confirm.open('Отмяна', this.$t('common.confirm'))) {
              this.$notifier.success('', 'Успешен отказ');
            }
          }
        </pre>
        <ul>
          <li>title (String, default = null) - Заглавие.</li>
          <li>message (String, default = null) - Текст (h5).</li>
          <li>options (Object, default = {width: 500, noconfirm: false,}) - Настройки. Максимална ширина. Скриване на cancel бутона.</li>
        </ul>
      </v-card-subtitle>
      <v-card-text>
        <v-row>
          <button-tip
            color="primary"
            icon-name="fas fa-save"
            text="buttons.saveChanges"
            raised
            @click="onSave"
          />
          <button-tip
            color="error"
            icon-name="fas fa-times"
            text="buttons.cancel"
            raised
            @click="onCancel"
          />
        </v-row>
      </v-card-text>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        Year picker with month picker
      </v-card-title>
      <v-card-subtitle />
      <v-card-text>
        <v-row>
          <v-col
            cols="12"
            sm="6"
          >
            {{ wrappers.yearPicker.year }}
            <year-picker
              v-model="wrappers.yearPicker.year"
              label="Year picker"
            />
          </v-col>

          <v-col
            cols="12"
            sm="6"
          >
            {{ wrappers.yearPicker.year }}
            <year-picker
              v-model="wrappers.yearPicker.year"
              label="Year picker"
              :readonly="false"
            />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        Year picker with select and combobox
      </v-card-title>
      <v-card-subtitle>
        <ul>
          <li>allowTyping (Boolean, default = false) - Определя дали е позволено писането.</li>
          <li>min (Number, default = 1900) - Минимално позволена година.</li>
          <li>max (Number, default = 2050) - Максимално позволена година.</li>
          <li>items (Array, default = null) - Предварително заредени опции на селектите. Инак ще се мине през създаване на масив от числа от min до max..</li>
          <li>itemText (String, default = text) - Предварително заредени опции на селектите. Инак ще се мине през създаване на масив от числа от min до max.</li>
          <li>itemValue (String, default = value) - В случай, че подадем предварително заредени опции, които са масив от обекти е необходимо да кажем кое пропърти се байндава за стойноста и кое за текста.</li>
          <li> + всички пропъртита на vuetify select или combobox, в зависимост от allowTyping (allowTyping === true => v-combobox инак v-select).</li>
        </ul>
      </v-card-subtitle>
      <v-card-text>
        <v-row>
          <v-col
            cols="12"
            sm="6"
          >
            {{ wrappers.yearPicker.year1 }}
            <year-picker-combo
              v-model="wrappers.yearPicker.year1"
              label="Без възможност за писане"
              persistent-hint
              hint="Не позволява години изван дефинирания обхват (1900-2050 по подразбиране)"
            />
          </v-col>

          <v-col
            cols="12"
            sm="6"
          >
            {{ wrappers.yearPicker.year2 }}
            <year-picker-combo
              v-model="wrappers.yearPicker.year2"
              label="С възможност за писане"
              allow-typing
              persistent-hint
              hint="Esc за чистене когато е във фокус. Tab за приемане на написаното. Не позволява години изван дефинирания обхват (1900-2050 по подразбиране)"
            />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        School year selector
      </v-card-title>
      <v-card-text>
        {{ wrappers.schoolYear }}
        <school-year-selector
          v-model="wrappers.schoolYear"
          @input="v => $notifier.success(v ? `Избрана е ${v} учебна година` : 'Не е избрана учебна година')"
        />
      </v-card-text>
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        Month picker 
      </v-card-title>
      <v-card-text>
        <v-row>
          <v-col
            cols="12"
            md="6"
          >
            {{ wrappers.month }}
            <month-picker
              v-model="wrappers.month"
              :rules="[$validator.required()]"
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
          >
            {{ customMonth }}
            <custom-month-picker
              v-model="customMonth"
              :rules="[$validator.required()]"
            />
          </v-col>
        </v-row>
      </v-card-text>
      <button-tip
        text="Get selected Month"
        icon-name="mdi-tooltip"
        tooltip="Get selected Month"
        bottom
        depressed
        @click="onGetMonthClick"
      />
    </v-card>

    <v-card class="mb-3">
      <v-card-title>
        Каскадни dropdown-и
      </v-card-title>
      <v-card-text>
        <cascade-dropdowns-demo />
      </v-card-text>
    </v-card>

    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { NotificationSeverity } from '@/enums/enums';
import YearPicker from '@/components/wrappers/YearPicker';
import YearPickerCombo from '@/components/wrappers/YearPickerCombo';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import MonthPicker from '@/components/wrappers/MonthPicker';
import CustomMonthPicker from '@/components/wrappers/CustomMonthPicker';
import CascadeDropdownsDemo from './CascadeDropdownsDemo.vue';

export default {
    name: 'TestView',
    components: {
      YearPicker,
      YearPickerCombo,
      SchoolYearSelector,
      MonthPicker,
      CustomMonthPicker,
      CascadeDropdownsDemo
    },
    data() {
      return {
        notificationSeverity: NotificationSeverity,
        customMonth: 2,
        wrappers: {
          combo: {
            city: null,
            district: null
          },
          datePicker: {
            date1: null,
            date2: null
          },
          yearPicker: {
            year: '2050',
            year1: 2023,
            year2: 2024
          },
          schoolYear: new Date().getFullYear(),
          month: 0,
        },
        formRef: this.$refs['validator-example-form'],
        form: {
          field1: '',
          field2: '',
          field3: '',
          field4: '',
          field5: '',
          field6: undefined
        },
        formHasErrors: false,
      };
    },
    methods: {
       onGetMonthClick(){
           alert(this.wrappers.month);
        },
      showToast(notificationSeverity) {
        this.$notifier.toast(
          'Toast title',
          'Toast text',
          notificationSeverity
        );
      },
      showSnackbar(notificationSeverity) {
        this.$notifier.snackbar(
            'Stackbar title',
            'Stackbar text',
            notificationSeverity
        );
      },
      showModal(notificationSeverity) {
        this.$notifier.modal(
            'Modal title',
            'Modal text',
            notificationSeverity
        );
      },
      onButtonTipClick(btnText) {
        alert(`${btnText} clicked`);
      },
      resetForm () {
        this.formHasErrors = false;

        this.$validator.reset(this);
      },
      submit () {
        this.formHasErrors = this.$validator.validate(this);
      },
      async onSave() {
        if(await this.$refs.confirm.open('Запис', this.$t('common.confirm'))) {
          this.$notifier.success('', this.$t('common.saveSuccess'));
        }
      },
      async onCancel() {
        if(await this.$refs.confirm.open('Отмяна', this.$t('common.confirm'))) {
          this.$notifier.success('', 'Успешен отказ');
        }
      }
    },
};
</script>