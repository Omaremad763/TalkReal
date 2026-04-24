import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import * as messagedto from '../core-models';

@Injectable({
  providedIn: 'root',
})
export class QraphQlService {
  private readonly graphqlUrl = '/api/graphql';
  constructor(private http: HttpClient) {}
  getOnlineUsers(): Observable<any[]> {
    const query = {
      query: `
      query GetaLL {
        onlineUsers {
          userId
          userName
          isOnline
          profileImageUrl
        }
      }
    `,
    };
    return this.http.post<any>(this.graphqlUrl, query).pipe(
      map((res) => {
        if (res.errors) {
          console.error('GraphQL Validation Errors:', res.errors);
          return [];
        }
        return res.data.onlineUsers;
      }),
    );
  }

  getChatHistory(
    chatHistoryDto: messagedto.ChatHistoryRequestDto,
  ): Observable<messagedto.MessageDto[]> {
    const query = `
    query GetChatHistory($input: ChatHistoryRequestDtoInput!) {
      chatHistory(chatHistoryDTO: $input) {
        id
        senderId
        receiverId
        conversationId
        content
        sentAt
        status
        attachmentUrl
      }
    }
  `;
    const variables = {
      input: chatHistoryDto,
    };
    return this.http.post<any>(this.graphqlUrl, { query, variables }).pipe(
      map((res) => {
        if (res.errors) {
          console.error('GraphQL Errors:', res.errors);
          return [];
        }
        return res.data.chatHistory;
      }),
    );
  }
}
