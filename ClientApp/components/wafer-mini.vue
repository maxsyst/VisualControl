<template>
    <v-container>
      <v-row class="d-flex align-center justify-center">
        <svg :style="svgRotation" :height="size.fieldHeight" :width="size.fieldWidth" :viewBox="fieldViewBox">      
            <g v-for="(die, key) in dies" :key="die.id">
                <rect :dieIndex="key" :x="die.x" :y="die.y" :width="die.width" :height="die.height" :fill="die.fill" />
            </g>
        </svg>
      </v-row>
    </v-container>
</template>

<script>
  import Loading from 'vue-loading-overlay';
  export default {
    props: ['avbSelectedDies', 'dirtyCells'],
    components: { Loading },
    data() {
      return {
        dies: [],
        isloading: false,
        activeBtn: 1,
        showNav: false,
        x: 0,
        y: 0,
        initialOrientation: -1,
        currentOrientation: -1,
        fieldViewBox: ""
      }
    },

    mounted()
    {      
        this.dies = _.cloneDeep(this.wafer.formedMapMini.dies)             
        this.initialOrientation = +this.wafer.formedMapMini.orientation;
        this.currentOrientation = this.initialOrientation;  
        for (let i = 0; i < this.avbSelectedDies.length; i++) {
            let die = this.dies.find(d => d.id === this.avbSelectedDies[i])
            if(this.dirtyCells.includes(die.id)) {
                die.fill = "#E91E63";
            }
            else {
                die.fill = "#4CAF50";
            }          
        }        
    },
    
    watch:
    {
      fieldWidth: {
        immediate: true,
        handler(newVal, oldVal) {
          this.fieldViewBox = `0 0 ${this.size.fieldHeight} ${this.size.fieldWidth}`;
        }
      }
    },

    computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      },
      size() {
        return this.$store.getters['wafermeas/size']("mini")
      },
      wafer() {
        return this.$store.getters['wafermeas/wafer']
      },
      svgRotation() {
        return {
           transform: `rotate(${this.currentOrientation - this.initialOrientation}deg)`         
        }
      }
    }
  }
</script>
