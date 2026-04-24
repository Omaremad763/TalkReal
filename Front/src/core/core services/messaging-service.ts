import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { environment } from '../../environment';
import { ApiResponse } from '../../shared/shared_models/api-response.model';

@Injectable({ providedIn: 'root' })
export class MessagingService {
  private hubConnection?: signalR.HubConnection;
  private baseUrl = `${environment.apiUrl}/Messaging`;
  onlineUsers = signal<string[]>([]);
  private http = inject(HttpClient);
  public isConnected = new BehaviorSubject<boolean>(false);
  createHubConnection(token: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/api/hubs/presence`, {
        accessTokenFactory: () => token,
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .configureLogging(LogLevel.None)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connected to Presence Hub');
        this.isConnected.next(true);
      })
      .catch((err) => console.error(err));

    this.hubConnection.on('UserIsOnline', (userId) => {
      this.onlineUsers.update((users) => [...users, userId]);
    });

    this.hubConnection.on('UserIsOffline', (userId) => {
      this.onlineUsers.update((users) => users.filter((id) => id !== userId));
    });

    this.hubConnection.on('GetOnlineUsers', (userIds: string[]) => {
      this.onlineUsers.set(userIds);
    });
  }
  stopHubConnection() {
    this.hubConnection?.stop().catch((error) => console.log('Error stopping Hub:', error));
  }
  sendmessage(data: FormData): Observable<boolean> {
    return this.http
      .post<ApiResponse<boolean>>(`${this.baseUrl}/SendMessage`, data)
      .pipe(map((res) => res.data));
  }
}
