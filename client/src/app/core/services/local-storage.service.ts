import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  private readonly LAST_USED_CURRENCY_KEY = 'lastUsedCurrency';

  setLastUsedCurrency(currencyCode: string): void {
    localStorage.setItem(this.LAST_USED_CURRENCY_KEY, currencyCode);
  }

  getLastUsedCurrency(): string {
    return localStorage.getItem(this.LAST_USED_CURRENCY_KEY) || 'USD';
  }
}
