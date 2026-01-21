export class StripZeroesService {
    static transform(valueToTransform: string) {
        // recieves a string like so '000032' and return a string like so '32'
        return parseInt(valueToTransform, 10).toString().trim();
    }
}
