export class Paging {
    from: number;

    numberOfElements: number;

    constructor(from: number, numberOfElements: number) {
        this.from = from;
        this.numberOfElements = numberOfElements;
    }
}
