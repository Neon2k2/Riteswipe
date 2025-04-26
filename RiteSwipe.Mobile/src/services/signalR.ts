import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import Config from 'react-native-config';
import { store } from '@/store';

class SignalRService {
  private connection: HubConnection | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;

  async startConnection() {
    try {
      const token = store.getState().auth.token;
      
      this.connection = new HubConnectionBuilder()
        .withUrl(`${Config.API_URL}/hubs/chat`, {
          accessTokenFactory: () => token!
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            if (retryContext.previousRetryCount >= this.maxReconnectAttempts) {
              return null;
            }
            return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
          }
        })
        .build();

      await this.connection.start();
      console.log('SignalR Connected');

      this.setupConnectionEvents();
    } catch (error) {
      console.error('SignalR Connection Error:', error);
      throw error;
    }
  }

  private setupConnectionEvents() {
    if (!this.connection) return;

    this.connection.onreconnecting(() => {
      console.log('SignalR Reconnecting...');
      this.reconnectAttempts++;
    });

    this.connection.onreconnected(() => {
      console.log('SignalR Reconnected');
      this.reconnectAttempts = 0;
    });

    this.connection.onclose(() => {
      console.log('SignalR Connection Closed');
      if (this.reconnectAttempts < this.maxReconnectAttempts) {
        setTimeout(() => this.startConnection(), 5000);
      }
    });
  }

  subscribeToMessages(callback: (message: any) => void) {
    if (!this.connection) return;
    this.connection.on('ReceiveMessage', callback);
  }

  subscribeToTyping(callback: (data: { userId: string; isTyping: boolean }) => void) {
    if (!this.connection) return;
    this.connection.on('UserTyping', callback);
  }

  async sendMessage(recipientId: string, content: string) {
    if (!this.connection) return;
    await this.connection.invoke('SendMessage', recipientId, content);
  }

  async sendTypingStatus(recipientId: string, isTyping: boolean) {
    if (!this.connection) return;
    await this.connection.invoke('SendTypingStatus', recipientId, isTyping);
  }

  async stopConnection() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }
}

export const signalRService = new SignalRService();
