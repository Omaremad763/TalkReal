import { Component, computed, signal } from '@angular/core';
import { AuthService } from '../../shared/shared_services/auth.service';
import { ChatSidebarComponent } from './UI Components/chat-sidebar-component/chat-sidebar-component';
import { ChatWindowComponent } from './UI Components/chat-window-component/chat-window-component';

@Component({
  selector: 'app-chat-page',
  standalone: true,
  imports: [ChatSidebarComponent, ChatWindowComponent],
  templateUrl: './chatPage.html',
})
export class ChatPageComponent {
  constructor(private authService: AuthService) {}
  receiverid = signal<string | ''>('');
  senderid = computed(() => this.authService.currentUser()?.userId || 'unknown');
  onUserSelected(userId: string) {
    this.receiverid.set(userId);
  }
}
