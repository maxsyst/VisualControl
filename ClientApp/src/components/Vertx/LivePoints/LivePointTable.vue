<template>
    <v-container>

    </v-container>
</template>

<script>
import * as signalR from '@aspnet/signalr';

export default {
  data() {
    return {
      polling: null,
      connection: null,
    };
  },

  async created() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/livepoint')
      .build();

    this.connection.start().catch((err) => console.log(err));
    this.connection.on('lastValues', (value) => console.log(value));
    this.pollData();
  },

  methods: {
    pollData() {
      this.polling = setInterval(() => {
        this.connection.send('getLastValues');
      }, 10000);
    },
  },
};
</script>

<style>
</style>
