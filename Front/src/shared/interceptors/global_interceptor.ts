import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, finalize, map, throwError } from 'rxjs';
import Swal from 'sweetalert2';
import { AuthService } from '../shared_services/auth.service';
import { LoadingService } from '../shared_services/loading.service';
import { NotificationService } from '../shared_services/notification.service';

export const appInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const loadingService = inject(LoadingService);
  const notification = inject(NotificationService);

  loadingService.show();

  const authReq = addTokenHeader(req, authService.getToken());

  return next(authReq).pipe(
    map((event) => {
      // تجاهل refresh-token من أي wrapping logic
      if (req.url.includes('/refresh-token')) {
        return event;
      }

      if (event instanceof HttpResponse) {
        const body = event.body as any;

        if (body?.success === false) {
          throw new HttpErrorResponse({
            status: 400,
            error: body,
          });
        }
      }

      return event;
    }),

    catchError((error: HttpErrorResponse) => {
      // Unauthorized fallback فقط (مش refresh)
      if (error.status === 401) {
        if (req.url.includes('/Login')) {
          return throwError(() => error);
        }

        // هنا فقط fallback لو النظام الأساسي فشل
        authService.expireSession();
        return throwError(() => error);
      }

      handleGlobalErrors(error, notification);
      return throwError(() => error);
    }),

    finalize(() => loadingService.hide()),
  );
};

function addTokenHeader(request: HttpRequest<any>, token: string | null) {
  return token
    ? request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      })
    : request;
}

function handleGlobalErrors(error: HttpErrorResponse, notification: NotificationService) {
  if (error.status === 429) {
    Swal.fire({
      title: 'Wait',
      text: 'Too Many Requests. Try again later.',
      icon: 'error',
    });
    return;
  }

  const backendMessage =
    error.error?.errors?.join(', ') || error.error?.message || error.message || 'Server Error';

  notification.showError(backendMessage);
}
