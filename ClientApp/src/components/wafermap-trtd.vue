<template>
<v-container fluid grid-list-lg>

    <svg :style="svgRotation" :height="fieldHeight" :width="fieldWidth" :viewBox="fieldViewBox">

     <polyline fill="none"  stroke="#fc0" stroke-width="4" stroke-dasharray="25"
                    :points="cutting" />

      <g v-for="(die, key) in dies" :key="die.id">
        <rect :dieIndex="key" :x="die.x" :y="die.y" :width="die.width" :height="die.height" :fill="die.fill"
              @click="showmenu" @contextmenu="showmenu" />

      </g>

    </svg>
    <v-menu v-model="menu"
            :position-x="x"
            :position-y="y"
            absolute
            offset-y>
      <v-list>
        <v-list-item v-for="(item, index) in menuItems"
                     :key="index"
                     @click="sendToWaferMapFull">
          <v-list-item-title>{{ item.title }}</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>

  <v-bottom-navigation v-model="showNav"
                :active.sync="currentOrientation"
                background-color="transparent">
    <v-btn :value="0" text color="#fc0">
      0°

    </v-btn>

    <v-btn :value="90" text color="#fc0">
      90°

    </v-btn>

    <v-btn :value="180" text color="#fc0">
      180°

    </v-btn>

    <v-btn :value="270" text color="#fc0">
      270°

    </v-btn>

  </v-bottom-navigation>

</v-container>
</template>

<script>
export default {
  props: ['waferId', 'defectiveDiesSearchProps', 'streetSize', 'fieldHeight', 'fieldWidth'],
  data() {
    return {
      dies: [],
      isloading: false,
      selectedDieId: 0,
      activeBtn: 1,
      showNav: false,
      x: 0,
      y: 0,
      initialOrientation: -1,
      currentOrientation: -1,
      defectiveDies: [],
      fieldViewBox: '',
      menuItems: [
        { title: 'Показать все дефекты на кристалле' },
      ],
      menu: false,

    };
  },

  methods:
    {
      sendToWaferMapFull() {
        this.$http.get(`/api/defect/getbydieid?dieId=${this.selectedDieId}`)
          .then((response) => {
            const defects = response.data;
            const request = { defects, dieCode: this.dies.find((x) => x.id === this.selectedDieId).code };
            this.$emit('show-footer', request);
          });
      },

      showmenu(e) {
        e.preventDefault();
        if (this.dies[+e.currentTarget.attributes.dieIndex.value].isActive) {
          this.showMenu = false;
          this.x = e.clientX;
          this.y = e.clientY;
          this.selectedDieId = this.dies[+e.currentTarget.attributes.dieIndex.value].id;
          this.$nextTick(() => {
            this.menu = true;
          });
        }
      },
    },

  watch:
    {
      waferId: {
        immediate: true,
        async handler(newValue) {
          this.isloading = true;
          const fieldObject = {};
          fieldObject.waferId = newValue;
          fieldObject.fieldHeight = this.fieldHeight;
          fieldObject.fieldWidth = this.fieldWidth;
          fieldObject.streetSize = this.streetSize;
          await this.$http
            .get(`/api/wafermap/getformedwafermap?waferMapFieldViewModelJSON=${JSON.stringify(fieldObject)}`)
            .then((response) => {
              if (response.status === 200) {
                this.dies = [...response.data.waferMapFormed];
                this.initialOrientation = +response.data.orientation;
                this.currentOrientation = this.initialOrientation;
                this.isloading = false;
                this.showNav = true;
              }
            })
            .catch((error) => {
              if (error.response.status === 400) {
                this.dies = [];
                this.isloading = false;
              }
            });
        },
      },

      defectiveDiesSearchProps() {
        if (this.defectiveDiesSearchProps.defectType === 'all') {
          this.$http.get(`/api/defectivedie/getbydangerlevel?waferId=${this.waferId}&dangerLevelId=${this.defectiveDiesSearchProps.dangerLevel}`)
            .then((response) => {
              this.defectiveDies = response.data;
            })
            .catch(() => {
              this.defectiveDies = [];
            });
        } else {
          this.$http.get(`/api/defectivedie/getbydefecttype?waferId=${this.waferId}
                                                            &defectTypeId=${this.defectiveDiesSearchProps.defectType}
                                                            &dangerLevelId=${this.defectiveDiesSearchProps.dangerLevel}`)
            .then((response) => {
              this.defectiveDies = response.data;
            })
            .catch(() => {
              this.defectiveDies = [];
            });
        }
      },

      defectiveDies() {
        if (this.defectiveDiesSearchProps.dangerLevel === 1 && this.defectiveDiesSearchProps.defectType === 'all') {
          this.dies.forEach((die) => { die.fill = '#1E5938'; die.isActive = false; });
        } else {
          this.dies.forEach((die) => { die.fill = '#4f4c4c'; die.isActive = false; });
        }

        if (this.dies.length > 0 && this.defectiveDies.length > 0) {
          for (let i = 0; i < this.defectiveDies.length; i += 1) {
            // Bad smell
            this.dies.find((d) => d.id === this.defectiveDies[i].dieId).fill = this.defectiveDies[i].color;
            this.dies.find((d) => d.id === this.defectiveDies[i].dieId).isActive = true;
          }
        }
      },

      fieldWidth: {
        immediate: true,
        handler(newValue) {
          this.fieldViewBox = `0 0 ${this.fieldHeight} ${newValue}`;
        },
      },

    },

  computed:
    {
      cutting() {
        // ONLY IF HEIGHT === WIDTH
        const bBorder = this.fieldHeight / 6;
        const tBorder = (this.fieldHeight / 6) * 5;
        if (this.initialOrientation === -1) {
          return '0,0 0,0';
        }
        switch (this.initialOrientation) {
          case 0:
            return `${bBorder},${this.fieldHeight} ${tBorder},${this.fieldHeight}`;
          case 90:
            return `0,${bBorder} 0,${tBorder}`;
          case 180:
            return `${bBorder},0 ${tBorder},0`;
          case 270:
            return `${this.fieldHeight},${bBorder} ${this.fieldHeight},${tBorder}`;
          default:
            return '0,0 0,0';
        }
      },

      svgRotation() {
        return {
          transform: `rotate(${this.currentOrientation - this.initialOrientation}deg)`,

        };
      },
    },
};
</script>

<style scoped>
  rect:hover {
    stroke: #fc0;
    stroke-width: 2;
    stroke-opacity: 0.3
  }

</style>
