
export class CurrencyModel {
  constructor(obj = {}) {
    this.showAltCurrency = obj.showAltCurrency;
    this.currency = obj.currency;
    this.altCurrency = obj.altCurrency;
  }
}
