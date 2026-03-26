import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MessagingService } from '../../core/core services/messaging-service';
import { HeaderComponent } from '../header/header';
import { AuthService } from '../shared_services/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, HeaderComponent, RouterOutlet],
  template: `
    <app-header></app-header>
    <main class="h-[calc(100vh-5rem)]"><router-outlet></router-outlet></main>
  `,
})
export class MainLayoutComponent {
  private authService = inject(AuthService);
  private messagingService = inject(MessagingService);
  ngOnInit(): void {
    const token = this.authService.getToken();
    if (token) {
      this.messagingService.createHubConnection(token);
    }
  }
  logout() {
    this.messagingService.stopHubConnection();
    this.authService.logout();
  }
}
