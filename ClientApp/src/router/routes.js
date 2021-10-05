import HomePage from '../components/home-page.vue';
import DefectCard from '../components/defect-card.vue';
import DefectSingle from '../components/defect-single.vue';
import DefectVue from '../components/defect-mainvue.vue';
import WaferMeas from '../components/WaferMeasurement/FullView.vue';
import DangerLevel from '../components/dangerlevel-crud.vue';
import DefectType from '../components/defecttype-crud.vue';
import Device from '../components/device-crud.vue';
import WaferMap from '../components/wafermap-full.vue';
import SelectBasic from '../components/select-basic.vue';
import Kurbatov from '../components/export-kurb.vue';
import StageTable from '../components/stage-table.vue';
import VerificationSettings from '../components/verification-settings.vue';
import StandartParameter from '../components/standartparameter-view.vue';
import IdmrVoc from '../components/idmr-voc.vue';
import DefectMassiveUploader from '../components/massive-uploader.vue';
import DieTypeSettings from '../components/dietype-settings.vue';
import Uploader from '../components/uploader-ng.vue';
import UploaderGraphic4 from '../components/Uploading/S2PGraphic4Uploading.vue';
import ElementType from '../components/element-type.vue';
import UploaderFg from '../components/uploader-filegraphic.vue';
import UploaderFinal from '../components/uploader-final.vue';
import LivePoint from '../components/Vertx/LivePoints/LivePointTable.vue';
import MDVCreation from '../components/Vertx/CreationForms/MdvCreation.vue';
import MeasurementCreation from '../components/Vertx/CreationForms/MeasurementCreation.vue';

// Service components
import LoginPage from '../components/login-page.vue';
import ShortLinkHandler from '../components/shortlink-handler.vue';
import RegistrationPage from '../components/registration-page.vue';
import NotFound from '../components/error-404.vue';

export const routes = [
  {
    name: 'home', path: '/', component: HomePage, display: 'Начальный экран', nav: true, icon: 'home',
  },
  {
    name: 'defectuploader', path: '/defectuploader', component: DefectMassiveUploader, display: 'MassiveUploader', nav: true, icon: 'cloud_upload',
  },
  { name: 'registration', path: '/registration', component: RegistrationPage },
  { name: 'login', path: '/login', component: LoginPage },
  {
    name: 'wafermap', path: '/wafermap', component: WaferMap, display: 'WaferMap', nav: true, icon: 'blur_circular',
  },
  {
    name: 'defecttypeCRUD', path: '/defecttype', component: DefectType, display: 'DefectType', nav: true, icon: 'category',
  },
  {
    name: 'device', path: '/devices', component: Device, display: 'Device', nav: true, icon: 'category',
  },
  {
    name: 'dangerlevelCRUD', path: '/dangerlevel', component: DangerLevel, display: 'DangerLevel', nav: true, icon: 'report_problem',
  },
  {
    name: 'testing', path: '/testing', component: SelectBasic, display: 'Просмотр иcпытаний', nav: true,
  },
  { path: '/defect/:defectid', component: DefectCard },
  {
    name: 'adddefect', path: '/adddefect', component: DefectSingle, display: 'Добавление дефекта', nav: true,
  },
  {
    name: 'defects', path: '/defects', component: DefectVue, display: 'Просмотр дефектов', nav: true,
  },
  {
    name: 'pwafer', path: '/pwafer', component: () => import('../components/pwafer.vue'), display: 'Просмотр измерений', nav: true,
  },
  { name: 'shortlink-handler', path: '/sl/:guid', component: ShortLinkHandler },
  {
    name: 'control-charts',
    path: '/controlCharts',
    display: 'Контрольные карты',
    component: () => import('../components/ControlCharts/FullView.vue'),
    nav: true,
  },
  { name: 'wafer-path', path: '/waferpath/:waferId', component: () => import('../components/WaferPath/FullView.vue') },
  {
    name: 'wafermeasurement',
    path: '/wafermeas',
    component: WaferMeas,
    children: [
      {
        name: 'wafermeasurement-shortlink',
        path: 'waferId/:waferId/measurement/:measurementName/sl/:guid',
        props: (route) => ({ shortLinkVm: route.params.shortLinkVm, guid: route.params.guid }),
        component: WaferMeas,
      },

      {
        name: 'wafermeasurement-onlywafer',
        path: 'waferId/:waferId',
        component: WaferMeas,
      },
      {
        name: 'wafermeasurement-fullselected',
        path: 'waferId/:waferId/measurement/:measurementName',
        component: WaferMeas,
      }],
  },
  {
    name: 'kurbatov', path: '/export-kurb', component: Kurbatov, display: 'Экспорт', nav: true,
  },
  {
    name: 'elementtype', path: '/element-type', component: ElementType, display: 'Et', nav: true,
  },
  {
    name: 'standartparameter', path: '/standart-parameter', component: StandartParameter, display: 'StandartParameter',
  },
  {
    name: 'kurbatovparameter',
    path: '/kb-parameter',
    component: () => import('../views/KurbatovPatternEditor.vue'),
    display: 'KParameter',
    nav: true,
    children: [
      {
        name: 'kurbatovparameter-initial-typeisselected',
        path: 'dieType/:dieType',
        component: () => import('../views/KurbatovPatternEditor.vue'),
      },
      {
        name: 'kurbatovparameter-creating',
        path: 'dieType/:dieType/mode/creating',
        component: () => import('../views/KurbatovPatternEditor.vue'),
      },
      {
        name: 'kurbatovparameter-updating',
        path: 'dieType/:dieType/mode/updating/patternId/:patternId',
        component: () => import('../views/KurbatovPatternEditor.vue'),
      },
    ],
  },
  {
    name: 'stagetable', path: '/stt/:processId', component: StageTable, props: (route) => ({ processId: +route.params.processId }),
  },
  {
    name: 'uploader', path: '/uu', component: Uploader, display: 'Загрузка измерений', nav: true, uploadingArea: true,
  },
  {
    name: 'idmrvocstart', path: '/idmr-voc', component: IdmrVoc, display: 'Редактирование измерений', nav: true, uploadingArea: true,
  },
  {
    name: 'idmrvoc', path: '/idmr-voc/:waferId/:selectedDieType', component: IdmrVoc, display: 'Редактирование измерений', props: true,
  },
  {
    name: 'dietypesettings', path: '/dts', component: DieTypeSettings, display: 'Настройка элементов', nav: true, uploadingArea: true,
  },
  {
    name: 'uploader-graphic4', path: '/ug4', component: UploaderGraphic4, display: 'Загрузка особенных графиков', nav: true, uploadingArea: true,
  },
  {
    name: 'uploader-fg', path: '/ufg', component: UploaderFg, display: 'Типы измерений', nav: true, uploadingArea: true,
  },
  {
    name: 'uploader-final', path: '/uploading', component: UploaderFinal, props: true,
  },
  {
    name: 'uploader-cp', path: '/uu/:selectedCodeProductFolder', component: Uploader, display: 'Загрузка', props: true,
  },
  {
    name: 'uploader-cpw',
    path: '/uu/:selectedCodeProductFolder/:selectedWaferFolder',
    component: Uploader,
    display: 'Загрузка измерений',
    props: true,
  },
  {
    name: 'uploader-cpwi',
    path: '/uu/:selectedCodeProductFolder/:selectedWaferFolder/:mrArray',
    component: Uploader,
    display: 'Загрузка измерений',
    props: true,
  },

  {
    name: 'verificationsettings', path: '/vsettings', component: VerificationSettings, display: 'Редактирование параметров испытаний', nav: true,
  },

  {
    name: 'livePoint', path: '/vertx/livePointScreen', component: LivePoint,
  },

  {
    name: 'mdvCreation', path: '/vertx/mdvCreation', component: MDVCreation,
  },

  {
    name: 'measurementCreation', path: '/vertx/measurementCreation', component: MeasurementCreation,
  },

  {
    path: '/vertx/MeasurementAttemptsLast',
    name: 'MeasurementAttemptsLastView',
    component: () => import('../components/Vertx/Views/MeasurementAttemptsLast.vue'),
  },
  {
    path: '/vertx/measurementAttempt/:measurementAttemptId',
    name: 'measurementAttempt',
    component: () => import('../components/Vertx/Views/MeasurementAttempt.vue'),
  },

  {
    name: 'measurement-single',
    path: '/vertx/measurement/:measurementId',
    component: () => import('../components/Vertx/Views/SingleMeasurement.vue'),
  },

  {
    name: 'defectsbywafer',
    path: '/defects/:selectedWafer',
    display: 'Просмотр дефектов',
    component: DefectVue,
    props: true,

  },

  {
    name: 'singlediedefects',
    path: '/defects/:selectedWafer/singledie/:selectedsingledieId/:dangerlevelspec',
    display: 'Просмотр дефектов',
    component: DefectVue,
    props: true,

  },

  {
    path: '/404',
    name: '404',
    component: NotFound,
  }, {
    path: '*',
    redirect: '/404',
  },
];
