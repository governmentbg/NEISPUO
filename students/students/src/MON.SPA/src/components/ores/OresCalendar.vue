<template>
  <div>
    <v-toolbar
      flat
      dense
    >
      <v-btn
        outlined
        class="mr-4"
        color="grey darken-2"
        small
        @click="setToday"
      >
        {{ $t('common.currentMonth') }}
      </v-btn>
      <v-btn
        fab
        text
        small
        color="grey darken-2"
        @click="prev"
      >
        <v-icon small>
          mdi-chevron-left
        </v-icon>
      </v-btn>
      <v-btn
        fab
        text
        small
        color="grey darken-2"
        @click="next"
      >
        <v-icon small>
          mdi-chevron-right
        </v-icon>
      </v-btn>
      <v-toolbar-title v-if="$refs.calendar">
        {{ $refs.calendar.title }}
      </v-toolbar-title>
      <v-spacer />
      <!-- <c-select
        v-model="type"
        :items="types"
        dense
        outlined
        hide-details
        class="ma-2"
        :label="$t('ores.viewMode')"
      />
      <c-select
        v-model="mode"
        :items="modes"
        dense
        outlined
        hide-details
        label="event-overlap-mode"
        class="ma-2"
      /> -->
    </v-toolbar>
    <v-calendar
      ref="calendar"
      v-model="value"
      color="primary"
      :weekdays="weekday"
      :type="type"
      :events="events"
      :event-overlap-mode="mode"
      :event-overlap-threshold="30"
      :event-color="getEventColor"
      locale="BG"
      @click:event="showEvent"
      @change="getEvents"
    >
      <template v-slot:event="{ event }">
        {{ event.name }}
      </template>
      <template v-slot:day-label="{ present, day, month, date }">
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-btn
              v-if="present === true"
              color="primary"
              small
              fab
              v-on="on"
            >
              {{ day }}
            </v-btn>
            <v-btn
              v-else-if="day === 1"
              small
              fab
              text
              v-on="on"
            >
              {{ `${day}.${month.toString().padStart(2, '0')}` }}
            </v-btn>
            <v-btn
              v-else
              small
              fab
              text
              v-on="on"
            >
              {{ day }}
            </v-btn>
          </template>
          <span>{{ $moment(date).format(dateFormat) }}</span>
        </v-tooltip>
      </template>
    </v-calendar>
    <v-dialog
      v-model="selectedOpen"
      :close-on-content-click="false"
      :activator="selectedElement"
      offset-x
    >
      <v-card
        color="grey lighten-4"
        min-width="350px"
        flat
      >
        <v-toolbar
          :color="selectedEvent.color"
          dark
        >
          <v-toolbar-title>
            {{ selectedEvent.name }}
          </v-toolbar-title>
          <v-spacer />
          <v-btn
            icon
            @click="selectedOpen = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-toolbar>
        <v-card-text>
          <v-row
            v-if="selectedEvent.details"
            class="mt-5"
            dense
          >
            <v-col dense>
              <c-text-field
                :value="selectedEvent.details.oresTypeName"
                :label="$t('ores.oresType')"
                disabled
                dense
                persistent-placeholder
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
              lg="2"
            >
              <c-text-field
                :value="$moment(selectedEvent.details.startDate).format(dateFormat)"
                :label="$t('ores.startDate')"
                disabled
                dense
                persistent-placeholder
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
              lg="2"
            >
              <c-text-field
                :value="$moment(selectedEvent.details.endDate).format(dateFormat)"
                :label="$t('ores.endDate')"
                disabled
                dense
                persistent-placeholder
              />
            </v-col>
            <v-col
              cols="12"
            >
              <c-textarea
                :value="selectedEvent.details.description"
                :label="$t('ores.description')"
                disabled
                dense
                persistent-placeholder
                outlined
              />
            </v-col>
          </v-row>

          <ores-students
            v-if="selectedOpen"
            :ores-id="selectedEvent.details.id"
            class="mt-4"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            text
            color="secondary"
            @click="selectedOpen = false"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('buttons.cancel') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>


<script>
import OresStudents from '@/components/ores/OresStudents.vue';
import Constants from '@/common/constants.js';

export default {
  name: 'OresCalendarComponent',
  components: {
    OresStudents
  },
  data() {
    return {
      loading: false,
      dateFormat: Constants.DATE_FORMAT,
      value: '',
      events: [],
      type: 'month',
      types: ['month', 'week', 'day'],
      mode: 'stack',
      modes: ['stack', 'column'],
      weekday: [1, 2, 3, 4, 5, 6, 0],
      colors: ['blue', 'indigo', 'deep-purple', 'cyan', 'green', 'orange', 'grey darken-1'],
      selectedEvent: {},
      selectedElement: null,
      selectedOpen: false,
    };
  },
  mounted() {
    this.checkChange();
  },
  methods: {
    checkChange() {
      this.$refs.calendar.checkChange();
    },
    refresh() {
      const calendar = this.$refs.calendar;
      if(!calendar) return;

      this.getEvents({ start: calendar.lastStart, end: calendar.lastEnd });
    },
    async getEvents({ start, end }) {
      this.loading = true;

      try {
        const oresList = (await this.$api.ores.getCalendarDetails(start?.date, end?.date))?.data;
        if(!oresList) {
          this.events = [];
        }

        const events = [];
        oresList.forEach(e => {
          events.push({
            name: this.$tc('ores.calendar.eventTitle', e.studentsCount, {
              name: e.calendarEventTitle,
              count: e.studentsCount
            }),
            start: e.startDate,
            end: e.endDate,
            timed: false,
            color: this.colors[this.rnd(0, this.colors.length - 1)],
            details: e
          });
        });

        this.events = events;
        this.loading = false;
      } catch (error) {
        this.loading = false;
        console.log(error);
      }

    },
    getEventColor (event) {
      return event.color;
    },
    rnd (a, b) {
      return Math.floor((b - a + 1) * Math.random()) + a;
    },
    setToday () {
      this.value = '';
    },
    prev () {
      this.$refs.calendar.prev();
    },
    next () {
      this.$refs.calendar.next();
    },
    showEvent ({ nativeEvent, event }) {
      const open = () => {
        this.selectedEvent = event;
        this.selectedElement = nativeEvent.target;
        requestAnimationFrame(() => requestAnimationFrame(() => this.selectedOpen = true));
      };

      if (this.selectedOpen) {
        this.selectedOpen = false;
        requestAnimationFrame(() => requestAnimationFrame(() => open()));
      } else {
        open();
      }

      nativeEvent.stopPropagation();
    },
  }
};
</script>
