<template>
<v-container>  
    <svg :style="svgRotation" :height="size.fieldHeight" :width="size.fieldWidth" :viewBox="fieldViewBox">
      <polyline fill="none"  stroke="#fc0" stroke-width="4" stroke-dasharray="25" :points="cutting" />
      <g v-for="(die, key) in dies" :key="die.id">
        <rect :dieIndex="key" :x="die.x" :y="die.y" :width="die.width" :height="die.height" :fill="die.fill" :fill-opacity="die.fillOpacity" @click="selectDie" @contextmenu="showmenu" />
        <text :x="die.x" :y="die.y+die.height/1.5" font-family="Verdana" :font-size="fontSize" :fill="die.text">{{die.code}}</text>
      </g>
    </svg>
    <v-menu v-model="menu"
            :position-x="x"
            :position-y="y"
            absolute
            offset-y>
      <v-list>
        <v-list-item v-for="(item, index) in menuItems" :key="index">                    
          <v-list-item-title>{{ item.title }}</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>

    <v-bottom-navigation 
      v-if="dies.length>0" 
      :value="showNav"
      v-model="currentOrientation"
      color="transparent">

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
  import Loading from 'vue-loading-overlay';
  export default {
    props: ['avbSelectedDies', 'mapMode'],
    components: { Loading },
    data() {
      return {
        dies: [],
        activeBtn: 1,
        showNav: false,
        x: 0,
        y: 0,
        initialOrientation: -1,
        currentOrientation: -1,
        fieldViewBox: "",
        menuItems: [
         { title: "Mocking" }
        ],
        menu: false
      }
    },

    methods:
    {
      selectDie(e) {
        e.preventDefault()
        let dieId = this.dies[+e.currentTarget.attributes.dieIndex.value].id      
        if (this.dies[+e.currentTarget.attributes.dieIndex.value].isActive)
        {
          let position =  this.selectedDies.indexOf(dieId);
          if ( ~position ) {
            this.selectedDies.splice(position, 1);
            this.$store.dispatch("wafermeas/updateSelectedDies", this.selectedDies);
          } 
          else {
            this.selectedDies.push(dieId);
            this.$store.dispatch("wafermeas/updateSelectedDies", this.selectedDies);
          }
        }
      
      },

      showmenu(e) {
        e.preventDefault()
        if (this.dies[+e.currentTarget.attributes.dieIndex.value].isActive) {
          this.showMenu = false
          this.x = e.clientX;
          this.y = e.clientY;
          let selectedDie = this.dies[+e.currentTarget.attributes.dieIndex.value]
          this.$nextTick(() => {
            this.menu = true
            this.menuItems[0].title = selectedDie.code
          });
        }
      }
    },
    
    watch:
    {
      'wafer.id': function(newVal) {
          this.dies = this.wafer.formedMapBig.dies              
          this.initialOrientation = +this.wafer.formedMapBig.orientation;
          this.currentOrientation = this.initialOrientation;
          this.showNav = false;
          this.dies.forEach(die => {
            die.fill = "#A1887F";
            die.text = "#303030"
            die.fillOpacity = 1.0
            die.isActive = false;
          });
      },    

      fieldWidth: {
        immediate: true,
        handler(newVal, oldVal) {
          this.fieldViewBox = `0 0 ${this.size.fieldHeight} ${this.size.fieldWidth}`;
        }
      },

      mapMode: function(newVal) {
        if(newVal === "selected") {
            for (let i = 0; i < this.avbSelectedDies.length; i++) {
              let die = this.dies.find(d => d.id === this.avbSelectedDies[i])
              die.fill = "#8C9EFF";
              die.text = "#303030"
              die.fillOpacity = 1.0
              die.isActive = true;
            }  
        
            for (let i = 0; i < this.selectedDies.length; i++) {   
              let die = this.dies.find(d => d.id === this.selectedDies[i])         
              die.fill = "#3D5AFE"
              die.text = "#FFF9C4"      
            }        
        }

        if(newVal === "dirty") {
          this.avbSelectedDies.forEach(avb => {
            let die = this.dies.find(d => d.id === avb)
            die.fill = this.dirtyCells.statList.includes(die.id) ? "#E91E63" : "#4CAF50"
            die.fillOpacity = this.selectedDies.includes(die.id) ? 1.0 : 0.5
            die.text = this.selectedDies.includes(die.id) ? "#303030" : "#FFF9C4" 
            die.isActive = true
          })
        }
      },

      selectedDies: function() {
        if(this.dies.length > 0) {
          if (this.avbSelectedDies.length > 0 && this.selectedDies.length > 0) {
            this.dies.forEach(function(die) {
              die.fill = "#A1887F"
              die.text = "#303030"
              die.fillOpacity = 1.0
              die.isActive = false;
            });
            if(this.mapMode === "selected") {
              for (let i = 0; i < this.avbSelectedDies.length; i++) {
                let die = this.dies.find(d => d.id === this.avbSelectedDies[i])
                die.fill = "#8C9EFF";
                die.text = "#303030"
                die.isActive = true;
              }  
        
              for (let i = 0; i < this.selectedDies.length; i++) {            
                let die = this.dies.find(d => d.id === this.selectedDies[i])         
                die.fill = "#3D5AFE"
                die.text = "#FFF9C4"           
              }          
            }
            if(this.mapMode === "dirty") {
              this.avbSelectedDies.forEach(avb => {
                let die = this.dies.find(d => d.id === avb)
                die.fill = this.dirtyCells.statList.includes(die.id) ? "#E91E63" : "#4CAF50"
                die.fillOpacity = this.selectedDies.includes(die.id) ? 1.0 : 0.5
                die.text = this.selectedDies.includes(die.id) ? "#303030" : "#FFF9C4" 
                die.isActive = true;
              })
            }
          }
        }
        
      }
    },

    computed:
    {
      dirtyCells() {
        return this.$store.getters['wafermeas/dirtyCells']
      },

      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      },

      size() {
        return this.$store.getters['wafermeas/size']("big")
      },

      wafer() {
        return this.$store.getters['wafermeas/wafer']
      },

      cutting() {
        //ONLY IF HEIGHT === WIDTH
        let bBorder = this.size.fieldHeight / 6;
        let tBorder = this.size.fieldHeight / 6 * 5;
        if (this.initialOrientation === -1) {
          return `0,0 0,0`
        }
        switch (this.initialOrientation) {
          case 0:
            return `${bBorder},${this.size.fieldHeight} ${tBorder},${this.size.fieldHeight}`
            break;
          case 90:
            return `0,${bBorder} 0,${tBorder}`;
            break
          case 180:
            return `${bBorder},0 ${tBorder},0`;
            break;
          case 270:
            return `${this.size.fieldHeight},${bBorder} ${this.size.fieldHeight},${tBorder}`;
            break;
          default:
            return `0,0 0,0`;
        }

      },

      fontSize() {
        return this.dies[0].height > 15 || !this.dies[0].code.includes('-') ? 12 : 2
      },

      svgRotation() {
        return {
          transform: `rotate(${this.currentOrientation - this.initialOrientation}deg)`
        }
      }
    }
  }
</script>

<style scoped>
  rect:hover {
    stroke: #fc0;
    stroke-width: 3;
    stroke-opacity: 0.6
  }

  svg text{
   -webkit-user-select: none;
   -moz-user-select: none;
   -ms-user-select: none;
   user-select: none;
  }

 
</style>
