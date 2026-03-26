import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output, effect, inject } from '@angular/core';
import { filter, take } from 'rxjs';
import { QraphQlService } from '../../../../core/core services/graphql-service';
import { MessagingService } from '../../../../core/core services/messaging-service';
import * as messagedto from '../../../../core/core-models';

@Component({
  selector: 'app-chat-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chat-sidebar-component.html',
})
export class ChatSidebarComponent implements OnInit {
  private MessagingService = inject(MessagingService);
  private QraphQlService = inject(QraphQlService);
  constructor() {
    effect(() => {
      const onlineIds = this.MessagingService.onlineUsers();

      this.users = this.users.map((user) => ({
        ...user,
        isOnline: onlineIds.includes(user.userId),
      }));
    });
  }
  users: messagedto.UserStatusDto[] = [];

  @Output() userSelected = new EventEmitter<string>();

  ngOnInit() {
    this.MessagingService.isConnected
      .pipe(
        filter((connected) => connected),
        take(1),
      )
      .subscribe(() => {
        this.loadUsers();
      });
  }
  loadUsers() {
    this.QraphQlService.getOnlineUsers().subscribe({
      next: (res: messagedto.UserStatusDto[]) => {
        this.users = res;
      },
    });
  }

  selectUser(userId: string) {
    this.userSelected.emit(userId);
  }
}
