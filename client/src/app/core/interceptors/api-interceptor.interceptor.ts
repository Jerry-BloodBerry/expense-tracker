import { HttpInterceptorFn, HttpEvent } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { API_CONFIG } from './config';
import { timeout } from 'rxjs';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const baseUrl = environment.apiBaseUrl;
  const headers = {...API_CONFIG.headers, ...req.headers.keys().reduce((acc: Record<string, string>, key: string) => {
    acc[key] = req.headers.get(key) ?? '';
    return acc;
  }, {})};
  const url = req.url.startsWith('/') ? `${baseUrl}${req.url}` : req.url;
  const apiReq = req.clone({ url, setHeaders: headers });

  return next(apiReq).pipe(
    timeout<HttpEvent<unknown>>(API_CONFIG.timeout)
  );
};
