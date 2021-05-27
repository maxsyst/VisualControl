<template>
    <v-container>
        <v-btn v-if="!_.isEmpty(measurements)" @click="goToAttemptTable">К таблице измерений</v-btn>
        <div>
            <LivePointRow class="ma-4" v-for="measurement in measurements"
                          :key="measurement.name"
                          :singleCharacteristics="measurement">
            </LivePointRow>
        </div>
    </v-container>
</template>

<script>
import * as signalR from '@aspnet/signalr';
import LivePointRow from './LivePointRow.vue';

export default {
  data() {
    return {
      polling: null,
      connection: null,
      measurements: [],
    };
  },

  components: {
    LivePointRow,
  },

  async created() {
    this.showLoading('Получение данных от приборов...');
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/livepoint')
      .build();
    this.connection.start();
    this.connection.on('lastValues', (value) => this.livePointsRedraw(value));
    this.pollData();
  },

  watch: {
    measurements(newValue) {
      if (!_.isEmpty(newValue)) {
        this.closeLoading();
      } else {
        this.showLoading('Получение данных от приборов');
      }
    },
  },

  methods: {

    goToAttemptTable() {
      this.$router.push({ name: 'MeasurementAttemptsLastView' });
    },

    showLoading(text) {
      this.$store.dispatch('loading/show', text);
    },

    closeLoading() {
      this.$store.dispatch('loading/cloak');
    },

    pollData() {
      this.polling = setInterval(() => {
        this.connection.send('getLastValues');
      }, 2000);
    },

    livePointsRedraw(value) {
      let measurements = Object.fromEntries([...new Set(value.map((x) => x.measurementName))].map((x) => [x, []]));
      const sortArray = ['Pout', 'Dr.Eff', 'Id', 'Ig'];
      value.forEach((v) => {
        let color = 'success';
        const diffMinutes = this.moment().diff(this.moment(v.date), 'minutes');
        if (diffMinutes > 30) {
          color = 'pink';
        } else if (diffMinutes > 5) {
          color = 'orange';
        }
        measurements[v.measurementName].push({
          name: v.measurementName,
          value: v.value,
          characteristic: v.characteristicName,
          unit: v.characteristicUnit,
          date: this.moment(v.date).fromNow(),
          dateColor: color,
        });
      });
      measurements = Object.values(measurements);
      this.measurements = measurements.map((m) => _.sortBy(m, (item) => sortArray.indexOf(item.characteristic)));
    },
  },
};
</script>
