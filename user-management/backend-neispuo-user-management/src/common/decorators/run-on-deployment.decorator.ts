import { DeploymentGroup } from '../constants/enum/deployment-group.enum';

interface RunOnDeploymentParams {
    names?: DeploymentGroup[];
}
export function RunOnDeployment(params: RunOnDeploymentParams) {
    // do nothing if the nest js instance is not ran with a specific APP_GROUP
    // we check if the supplied group equals the one from env file.
    return function (target: any, propertyKey: string, descriptor: PropertyDescriptor) {
        const originalMethod = descriptor.value;

        const isMethod = originalMethod instanceof Function;

        if (!isMethod) {
            return;
        }

        descriptor.value = function (...args: any[]) {
            const isMatchingDeployment = params.names.includes(DeploymentGroup[process.env.APP_GROUP]);
            const isProd = process.env.APP_ENV !== 'development';
            if (isProd && !isMatchingDeployment) {
                return;
            }
            const result = originalMethod.apply(this, args);
            return result;
        };

        Object.defineProperty(target, 'handleCron', descriptor);
    };
}
