import { Inject, LOCALE_ID, Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatCurrency'
})
export class FormatCurrencyPipe implements PipeTransform {

  constructor(@Inject(LOCALE_ID) private locale: string) {}

  transform(value: string | number, isoCode: string): any {

    if (!value) {
      value = 0;
    }

    if (typeof value === 'string') {
      value = parseFloat(value);
    }

    return new Intl.NumberFormat(this.locale, { style: 'currency', currency: isoCode }).format(value);
  }

}