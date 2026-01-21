import Vue from 'vue';
import moment from 'moment';

moment.locale( 'bg' );
Object.defineProperty( Vue.prototype, '$moment', { value: moment });

export default moment;