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
  receiver: messagedto.UserStatusDto | null = null;
  receiverid = input.required<string>();
  @Input() senderid!: string;

  messages: messagedto.MessageDto[] = [];
  users: messagedto.UserStatusDto[] = [];

  selectedFile: File | null = null;
  newMessage = '';

  constructor() {
    effect(() => {
      const id = this.receiverid();
      if (id) {
        this.loadHistory();
        this.loadUsers();
      }
    });
  }

  loadUsers() {
    this.QraphQlService.getOnlineUsers().subscribe({
      next: (res: messagedto.UserStatusDto[]) => {
        this.users = res;
        this.updateReceiverHeader();
      },
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
    if (!this.newMessage.trim() && !this.selectedFile) return;
    const user = this.authService.currentUser();
    if (!user || !this.receiverid) {
      console.error('Missing Sender or Receiver ID');
      return;
    }
    const MessageData = new FormData();
    MessageData.append('senderId', user.userId);
    MessageData.append('receiverId', this.receiverid());
    MessageData.append('content', this.newMessage || '');
    if (this.selectedFile) {
      MessageData.append('file', this.selectedFile);
    }
    const localMessage: any = {
      senderId: user.userId,
      receiverId: this.receiverid,
      content: this.newMessage?.trim() || (this.selectedFile ? 'no text content' : ''),
      sentAt: new Date().toISOString(),
      status: messagedto.MessageStatusEnum.Pending,
      attachmentUrl: this.selectedFile ? URL.createObjectURL(this.selectedFile) : null,
    };
    this.messagingService.sendmessage(localMessage).subscribe({
      next: () => {
        this.messages.push(localMessage);
        this.newMessage = '';
        this.selectedFile = null;
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
  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  private updateReceiverHeader() {
    const id = this.receiverid();
    if (this.users.length > 0 && id) {
      this.receiver = this.users.find((u) => u.userId === id) || null;
    }
  }
}
