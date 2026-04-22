import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import * as messagedto from '../../../../core/core-models';

@Component({
  selector: 'app-message-bubble',
  standalone: true,
  templateUrl: './message-bubble-component.html',
  imports: [CommonModule],
})
export class MessageBubbleComponent {
  @Input() message!: messagedto.MessageDto;
  @Input() currentUserId!: string;

  isMine(): boolean {
    return this.message.senderId === this.currentUserId;
  }
}
