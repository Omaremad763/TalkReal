import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from '../../environment';
import { ApiResponse } from './../../shared/shared_models/api-response.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/User`;

  AddPhoto(file: FormData, userid: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.baseUrl}/AddPhoto/${userid}`, file);
  }
  GetImageById(userid: string): Observable<any> {
    return this.http
      .get<ApiResponse<any>>(`${this.baseUrl}/GetImageById/${userid}`)
      .pipe(map((res) => res.data));
  }
  DeleteImagetById(userid: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.baseUrl}/DeleteImagetById/${userid}`);
  }
}
