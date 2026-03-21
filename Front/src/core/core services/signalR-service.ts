import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class PresenceService {
  private hubConnection?: signalR.HubConnection;
  onlineUsers = signal<string[]>([]);
  constructor() {}
  createHubConnection(token: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7260/hubs/presence`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log('Error starting Hub:', error));

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
}
