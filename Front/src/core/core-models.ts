export interface MessageDto {
  id: string;
  senderId: string;
  receiverId: string;
  conversationId: string;
  content: string;
  sentAt: string;
  status: MessageStatusEnum;
  file: File;
  attachmentUrl: string;
}
export enum MessageStatusEnum {
  Pending = 0,
  Delivered = 1,
  Read = 2,
}

export interface UserStatusDto {
  userId: string;
  userName: string;
  isOnline: boolean;
  lastSeen: string;
  profileImageUrl: string;
}
export interface ChatHistoryRequestDto {
  senderid: string;
  receiverId: string;
  take?: number;
}
