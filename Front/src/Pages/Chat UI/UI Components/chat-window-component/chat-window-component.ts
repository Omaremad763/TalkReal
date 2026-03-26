import { CommonModule } from '@angular/common';
import { Component, Input, effect, inject, input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import Swal from 'sweetalert2';
import { QraphQlService } from '../../../../core/core services/graphql-service';
import { MessagingService } from '../../../../core/core services/messaging-service';
import * as messagedto from '../../../../core/core-models';
import { AuthService } from '../../../../shared/shared_services/auth.service';
import { MessageBubbleComponent } from '../message-bubble-component/message-bubble-component';

@Component({
  selector: 'app-chat-window',
  standalone: true,
  imports: [CommonModule, FormsModule, MessageBubbleComponent],
  templateUrl: './chat-window-component.html',
})
export class ChatWindowComponent {
  private messagingService = inject(MessagingService);
  private authService = inject(AuthService);
  private QraphQlService = inject(QraphQlService);

  receiverid = input<string>('');
  @Input() senderid!: string;

  messages: messagedto.MessageDto[] = [];
  newMessage = '';

  // ngOnChanges(changes: SimpleChanges): void {
  //   if (changes['selectedUserId'] && this.receiverid) {
  //     this.loadHistory();
  //   }
  // }

  constructor() {
    effect(() => {
      const id = this.receiverid();
      if (id) {
        this.loadHistory();
      }
    });
  }

  loadHistory() {
    const dto: messagedto.ChatHistoryRequestDto = {
      senderid: this.senderid,
      receiverId: this.receiverid(),
    };
    this.QraphQlService.getChatHistory(dto).subscribe({
      next: (res) => (this.messages = res),
    });
  }
  sendMessage() {
    if (!this.newMessage.trim()) return;
    const user = this.authService.currentUser();
    if (!user || !this.receiverid) {
      console.error('Missing Sender or Receiver ID');
      return;
    }
    const message: messagedto.MessageDto = {
      senderId: user.userId,
      receiverId: this.receiverid,
      content: this.newMessage,
      sentAt: new Date(),
      status: messagedto.MessageStatusEnum.Pending,
    } as any;

    this.messagingService.sendmessage(message).subscribe({
      next: () => {
        this.messages.push(message);
        this.newMessage = '';
        this.loadHistory();
      },
      error: (err) => {
        console.error('Failed to send message', err);
        Swal.fire({
          title: 'Error!',
          text: 'Could not send your message. Please try again.',
          icon: 'error',
          confirmButtonText: 'OK',
          confirmButtonColor: '#2563eb',
          background: '#ffffff',
          customClass: {
            popup: 'rounded-none border border-blue-100',
            confirmButton: 'rounded-none px-8 uppercase font-bold text-xs tracking-widest',
          },
        });
      },
    });
  }
}
