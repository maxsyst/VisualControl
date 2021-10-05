<template>
    <v-row>
            <v-card class="elevation-8">
              <v-list>
                  <v-list-item>
                    <v-list-item-content>
                    <v-list-item-title>Номер пластины</v-list-item-title>
                     <v-chip label>
                          <span>{{ singleCharacteristics[0].name.split('&&')[0] }}</span>
                      </v-chip>
                    </v-list-item-content>
                  </v-list-item>
                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>Номер образца</v-list-item-title>
                       <v-chip label>
                          <span>{{ singleCharacteristics[0].name.split('&&')[1] }}</span>
                      </v-chip>
                    </v-list-item-content>
                  </v-list-item>
                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>Номер измерения</v-list-item-title>
                       <v-chip label>
                          <span>{{ singleCharacteristics[0].name.split('&&')[2] }}</span>
                      </v-chip>
                    </v-list-item-content>
                  </v-list-item>
              </v-list>
            </v-card>
            <v-col lg="2" v-for="singleCharacteristic in singleCharacteristics" :key="singleCharacteristic.id">
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
