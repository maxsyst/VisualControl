<template>
    <v-row>
            <v-chip class="ma-4" label x-large>
                <span>
                    {{singleCharacteristics[0].name}}
                </span>
            </v-chip>
            <v-col lg="3" v-for="singleCharacteristic in singleCharacteristics" :key="singleCharacteristic.id">
                <LivePointCard :singleCharacteristic="singleCharacteristic"></LivePointCard>
            </v-col>           
        </v-row>
</template>

<script>
import LivePointCard from './LivePointCard.vue';

export default {
  props: ['singleCharacteristics'],
  components: {
    LivePointCard,
  },
  data() {
    return {

    };
  },
};
</script>
