import { format } from 'date-fns';
import Constants from './constants.js';

export default {
    info: (message, data) => {
        console.log(`Log ${format(Date.now(), Constants.DISPLAY_TIME_FORMAT)}: ${message}`);
        if (data) {
            console.log(JSON.stringify(data, null, 2));
        }
    }
};