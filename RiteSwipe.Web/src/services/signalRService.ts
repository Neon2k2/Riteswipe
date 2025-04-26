import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { TaskItem, TaskApplication, EscrowPayment, TaskDispute, TaskReview, Notification } from '../types';
import { store } from '../store';
import { addNotification, updateNotificationCount } from '../store/slices/notificationSlice';
import { updateTask, addApplication, updateEscrowPayment, updateDispute } from '../store/slices/taskSlice';
import { addReview } from '../store/slices/reviewSlice';

class SignalRService {
  private notificationConnection: HubConnection | null = null;
  private taskConnection: HubConnection | null = null;

  public async startConnections(token: string): Promise<void> {
    await this.startNotificationConnection(token);
    await this.startTaskConnection(token);
  }

  public async stopConnections(): Promise<void> {
    if (this.notificationConnection) {
      await this.notificationConnection.stop();
      this.notificationConnection = null;
    }
    if (this.taskConnection) {
      await this.taskConnection.stop();
      this.taskConnection = null;
    }
  }

  private async startNotificationConnection(token: string): Promise<void> {
    try {
      this.notificationConnection = new HubConnectionBuilder()
        .withUrl(`${process.env.REACT_APP_API_URL}/hubs/notification`, {
          accessTokenFactory: () => token
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      this.setupNotificationHandlers();

      await this.notificationConnection.start();
      console.log('NotificationHub connected');
    } catch (error) {
      console.error('Error connecting to NotificationHub:', error);
      throw error;
    }
  }

  private async startTaskConnection(token: string): Promise<void> {
    try {
      this.taskConnection = new HubConnectionBuilder()
        .withUrl(`${process.env.REACT_APP_API_URL}/hubs/task`, {
          accessTokenFactory: () => token
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      this.setupTaskHandlers();

      await this.taskConnection.start();
      console.log('TaskHub connected');
    } catch (error) {
      console.error('Error connecting to TaskHub:', error);
      throw error;
    }
  }

  private setupNotificationHandlers(): void {
    if (!this.notificationConnection) return;

    this.notificationConnection.on('ReceiveNotification', (notification: Notification) => {
      store.dispatch(addNotification(notification));
      store.dispatch(updateNotificationCount());
    });

    this.notificationConnection.on('NewReview', (review: TaskReview) => {
      store.dispatch(addReview(review));
    });

    this.notificationConnection.on('StatusUpdate', (status: string) => {
      // Handle user status updates if needed
      console.log('User status updated:', status);
    });
  }

  private setupTaskHandlers(): void {
    if (!this.taskConnection) return;

    this.taskConnection.on('TaskUpdated', (task: TaskItem) => {
      store.dispatch(updateTask(task));
    });

    this.taskConnection.on('NewApplication', (application: TaskApplication) => {
      store.dispatch(addApplication(application));
    });

    this.taskConnection.on('TaskMatch', (task: TaskItem) => {
      // Handle task matches (swipe right)
      console.log('New task match:', task);
    });

    this.taskConnection.on('EscrowUpdated', (escrow: EscrowPayment) => {
      store.dispatch(updateEscrowPayment(escrow));
    });

    this.taskConnection.on('DisputeUpdated', (dispute: TaskDispute) => {
      store.dispatch(updateDispute(dispute));
    });
  }

  public async joinTaskGroup(taskId: string): Promise<void> {
    if (!this.taskConnection) {
      throw new Error('Task connection not established');
    }
    await this.taskConnection.invoke('JoinTaskGroup', taskId);
  }

  public async leaveTaskGroup(taskId: string): Promise<void> {
    if (!this.taskConnection) {
      throw new Error('Task connection not established');
    }
    await this.taskConnection.invoke('LeaveTaskGroup', taskId);
  }
}

export const signalRService = new SignalRService();
