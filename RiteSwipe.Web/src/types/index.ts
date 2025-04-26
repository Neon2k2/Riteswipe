export interface TaskItem {
  id: string;
  title: string;
  description: string;
  budget: number;
  status: string;
  ownerId: string;
  workerId?: string;
  createdAt: string;
  updatedAt: string;
  skills: string[];
  location?: string;
  deadline?: string;
}

export interface TaskApplication {
  id: string;
  taskId: string;
  applicantId: string;
  status: string;
  proposedPrice: number;
  message: string;
  createdAt: string;
}

export interface EscrowPayment {
  id: string;
  taskId: string;
  amount: number;
  status: string;
  payerId: string;
  payeeId: string;
  createdAt: string;
  releasedAt?: string;
}

export interface TaskDispute {
  id: string;
  taskId: string;
  raisedByUserId: string;
  reason: string;
  evidence?: string;
  status: string;
  resolution?: string;
  createdAt: string;
  updatedAt: string;
}

export interface TaskReview {
  id: string;
  taskId: string;
  reviewerId: string;
  rating: number;
  comment: string;
  reviewType: 'TaskOwnerReview' | 'WorkerReview';
  createdAt: string;
}

export interface Notification {
  id: string;
  userId: string;
  type: string;
  title: string;
  message: string;
  read: boolean;
  data?: any;
  createdAt: string;
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  bio?: string;
  skills: string[];
  rating?: number;
  tasksCompleted: number;
  createdAt: string;
  updatedAt: string;
}
